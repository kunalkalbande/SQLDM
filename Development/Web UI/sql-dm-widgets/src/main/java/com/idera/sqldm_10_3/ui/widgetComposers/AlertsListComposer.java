package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.common.dashboard.Composers.DashboardBaseWidgetComposer;
import com.idera.common.rest.CoreRestClient;
import com.idera.core.facade.DashboardWidgetFacade;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.server.web.DashboardUtils;
import com.idera.sqldm_10_3.data.alerts.Alert;
import com.idera.sqldm_10_3.data.topten.InstanceAlertWidgetInstance;
import com.idera.sqldm_10_3.server.web.WebUtil;
import com.idera.sqldm_10_3.server.web.session.SessionUtil;
import com.idera.sqldm_10_3.ui.alerts.AlertSortByColumns;
import com.idera.sqldm_10_3.utils.Utility;
import com.idera.sqldm_10_3.data.alerts.Alert;
import com.idera.sqldm_10_3.data.topten.InstanceAlertWidgetInstance;
import com.idera.sqldm_10_3.ui.alerts.AlertSortByColumns;
import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.ForwardEvent;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.*;

import java.util.*;

public class AlertsListComposer extends DashboardBaseWidgetComposer {

	private static final long serialVersionUID = 1L;
	private List<Alert> model;
	private ListModelList<Alert> listModel;
	private ListModelList<Product> productsModel;
	private String orderBy = AlertSortByColumns.SEVERITY.getColumnName();
	private String orderType = "desc";
	private Logger logger = Logger.getLogger(AlertsListComposer.class);
	@Wire(".top-x-resource-by-count-panel .top-x-instances-by-alerts-count")
	private Listbox alertsList;
	@Wire
	private Intbox limit;
	@Wire
	private Button save;
	@Wire
	private Hbox errorContainer;
	@Wire
	private Popup widgetSettings;
	@Wire
	private Combobox productListCombobox;

	private Product currentProduct;
	private int appendFlag = 0;
	private List<Product> productsList;
	private AnnotateDataBinder binder;

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<Alert>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
		binder = new AnnotateDataBinder(getSelf());
		Map<String, String> settings = new HashMap<String, String>(
				config.getSettingsMap());
		binder.bindBean("setting", settings);
		binder.loadAll();
		alertsList.setVisible(false);
		errorContainer.setVisible(false);

		productsList = CoreRestClient.getInstance().getProducts();
		productsModel = new ListModelList<>();
		String productName = config.getProduct()
				.getProductNameWithoutInstanceName();

		for (Product product : productsList) {
			if (product.getProductNameWithoutInstanceName().equals(productName))
				productsModel.add(product);
		}
		if (productsModel.size() > 1) {
			Product all = new Product();
			all.setName("All");
			all.setInstanceName("firstinstance");
			productsModel.add(0, all);
			productListCombobox.setModel(productsModel);
		} else
			productListCombobox.setVisible(false);
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setWidgetData(Object obj) {
		if (obj == null) {
			String errorMessage = "Error while fetching data. Please check log for more details.";
			binder.bindBean("message", errorMessage);
			binder.loadAll();
			alertsList.setVisible(false);
			errorContainer.setVisible(true);
		} else {
			if(productListCombobox.getSelectedIndex() == 0) {
				for (Alert alert : (List<Alert>)obj) {
					alert.setProduct(currentProduct);
				}
				if(appendFlag == 0) {
					model = (List<Alert>) obj;
					appendFlag = 1;
				}
				else
					model.addAll((List<Alert>) obj);
			}
			else
				model = (List<Alert>) obj;
			
			Collections.sort(model, new Comparator<Alert>() {

				@Override
				public int compare(Alert o1,
						Alert o2) {
					return o2.getSeverity().compareTo(o1.getSeverity());
				}
			});

			listModel = new ListModelList<Alert>(model);
			alertsList.setModel(listModel);
			alertsList.setVisible(true);
			errorContainer.setVisible(false);
		}
	}

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				InstanceAlertWidgetInstance.class.getName());
	}

	@Listen("onClick=#settingsActionButton")
	public void openSettingsPopup(Event event) {
		widgetSettings.open(event.getTarget(), "start_after");
	}

	@Listen("onClick=#save")
	public void onSave(Event event) {
		if (limit.getValue() > 10) {
			limit.focus();
			limit.setErrorMessage("Limit can't be more than 10");
			return;
		}
		Map<String, String> setting = config.getSettingsMap();
		setting.put("Limit", limit.getValue().toString());
		DashboardUtils.updateWidgetSettings(viewId, config.getId(), setting);
		widgetSettings.close();
		loadWidgetData(); // Trigger Data load in background thread.
	}

	@Listen("onSelect=#productListCombobox")
	public void onSelect(Event event) {

		appendFlag = 0;
		if(productListCombobox.getSelectedIndex() == 0) {
			crossRepoList();
		}
		else {
			loadWidgetData(); // Trigger Data load in background thread.
		}
	}

	@Listen("onAfterRender=#productListCombobox")
	public void afterRender(Event event) {

		int index = 0;
		for (int i = productsModel.size() - 1; i >= 0; i--) {
			if (productsModel.get(i).getName()
					.equals(config.getProduct().getName())) {
				index = i;
				break;
			}
		}
		productListCombobox.setSelectedIndex(index);

	}

	@Listen("onClickSelectedAlertInstance = #alertsListbox")
	public void onClickSelectedAlertInstance(ForwardEvent event) {
		SessionUtil.getSecurityContext();
		Product product;
		Alert alert = (Alert) event.getData();

		if (productsModel != null && productsModel.size() > 1) {
			int selectedProductIndex = productListCombobox.getSelectedIndex();
			if(selectedProductIndex == 0)
				product = alert.getProduct();
			else
				product = productsModel.get(selectedProductIndex);
		} else {
			product = config.getProduct();
		}
		Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(product,
				"singleInstance" + "/" + alert.getInstanceId()));
	}

	//SQLDM-30499
	@Listen("onClickSelectedAlert = #alertsListbox")
	public void onClickSelectedAlert(ForwardEvent event) {
		SessionUtil.getSecurityContext();
		Map<Object, Object> args = new HashMap<Object, Object>();
		
		Alert instance = (Alert) event.getData();
		Product product;
		if (productsModel != null && productsModel.size() > 1) {
			int selectedProductIndex = productListCombobox.getSelectedIndex();
			if(selectedProductIndex == 0)
				product = instance.getProduct();
			else
				product = productsModel.get(selectedProductIndex);
		} else {
			product = config.getProduct();
		}
		
		args.put("alert", (Alert) event.getData());
		args.put("alertsList", model);
		args.put("product", product);
		Window window = (Window) Executions
				.createComponents(
						"~./sqldm/com/idera/sqldm/widgets/alertsView.zul",
						null, args);
		window.doModal();
	}

	@Override
	public String getDataURI() {
		Map<String, String> setting = config.getSettingsMap();
//		int limit = 10;
		String isActive = "true";
/*		if (setting != null && setting.containsKey("Limit")) {
			try {
				limit = Integer.parseInt(setting.get("Limit"));
			} catch (IllegalFormatConversionException e) {
				logger.error(String
						.format("Unable to parse limit value for widget %s on viewId %d",
								config.getName(), viewId));
			}
		}
*/
		String dataURI = config.getDataURI().replace("{timeZoneOffset}", Utility.setOffSet());
		if (productsModel != null && productsModel.size() > 1) {
			int selectedIndex = productListCombobox.getSelectedIndex();
			if(selectedIndex == 0) {
				return String.format("%s%s?orderBy=%s&orderType=%s&activeOnly=%s",
						currentProduct.getRestUrl(), dataURI,
						orderBy, orderType, isActive);
			}
			
			return String.format(
					"%s%s?orderBy=%s&orderType=%s&activeOnly=%s",
					productsModel.get(selectedIndex).getRestUrl(), dataURI,
					orderBy, orderType, isActive);
		}

		return String.format(
				"%s%s?orderBy=%s&orderType=%s&activeOnly=%s", config
						.getProduct().getRestUrl(), dataURI, 
						orderBy, orderType, isActive);

	}

	private void crossRepoList() {

		for (Product product : productsList) {
			currentProduct = product;
			setWidgetData(DashboardWidgetFacade.getWidgetData(getDataURI(), getModelType()));
		}

	}
}
