package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.common.dashboard.Composers.DashboardBaseWidgetComposer;
import com.idera.common.rest.CoreRestClient;
import com.idera.core.facade.DashboardWidgetFacade;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.server.web.DashboardUtils;
import com.idera.sqldm_10_3.data.DashboardInstance;
import com.idera.sqldm_10_3.data.DashboardInstanceFacade;
import com.idera.sqldm_10_3.data.topten.InstanceAlertWidgetInstance;
import com.idera.sqldm_10_3.server.web.WebUtil;
import com.idera.sqldm_10_3.server.web.session.SessionUtil;
import com.idera.sqldm_10_3.data.DashboardInstance;
import com.idera.sqldm_10_3.data.DashboardInstanceFacade;
import com.idera.sqldm_10_3.data.topten.InstanceAlertWidgetInstance;
import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.ForwardEvent;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.*;

import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class InstancesListComposer extends DashboardBaseWidgetComposer {

	private static final long serialVersionUID = 1L;
	private List<DashboardInstance> model;
	private ListModelList<DashboardInstance> listModel;
	private ListModelList<Product> productsModel;
	private Logger logger = Logger.getLogger(InstancesListComposer.class);
	@Wire(".top-x-resource-by-count-panel .top-x-instances-by-alerts-count")
	private Listbox instancesList;
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
		return new TypeReference<List<DashboardInstance>>() {
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
		instancesList.setVisible(false);
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
			instancesList.setVisible(false);
			errorContainer.setVisible(true);
		} else {
			if(productListCombobox.getSelectedIndex() == 0) {
				for (DashboardInstance instance : (List<DashboardInstance>)obj) {
					instance.setProduct(currentProduct);
				}
				if(appendFlag == 0) {
					model = (List<DashboardInstance>) obj;
					appendFlag = 1;
				}
				else
					model.addAll((List<DashboardInstance>) obj);
			}
			else
				model = (List<DashboardInstance>) obj;
			
			Collections
					.sort(model,
							DashboardInstanceFacade.DASHBOARD_INSTANCE_SEVERITY_COMPARATOR_DESC);
/*			Map<String, String> setting = config.getSettingsMap();
			int limit = 10;
			if (setting != null && setting.containsKey("Limit")) {
				try {
					limit = Integer.parseInt(setting.get("Limit"));
				} catch (IllegalFormatConversionException e) {
					logger.error(String
							.format("Unable to parse limit value for widget %s on viewId %d",
									config.getName(), viewId));
				}
			}
			if (limit > model.size())*/
				listModel = new ListModelList<DashboardInstance>(model);
/*			else
				listModel = new ListModelList<DashboardInstance>(model.subList(
						0, limit));
*/
			instancesList.setModel(listModel);
			instancesList.setVisible(true);
			errorContainer.setVisible(false);
		}
	}

	@Listen("onClick=#settingsActionButton")
	public void openSettingsPopup(Event event) {
		widgetSettings.open(event.getTarget(), "start_after");
	}

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				InstanceAlertWidgetInstance.class.getName());
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

	@Override
	public String getDataURI() {
/*		Map<String, String> setting = config.getSettingsMap();
		int limit = 10;
		if (setting != null && setting.containsKey("Limit")) {
			try {
				limit = Integer.parseInt(setting.get("Limit"));
			} catch (IllegalFormatConversionException e) {
				logger.error(String
						.format("Unable to parse limit value for widget %s on viewId %d",
								config.getName(), viewId));
			}
		}
*/
		if (productsModel != null && productsModel.size() > 0) {
			int selectedIndex = productListCombobox.getSelectedIndex();
			if(selectedIndex == 0) {
				return String.format("%s%s", currentProduct.getRestUrl(),
						config.getDataURI());
			}
			
			return String.format("%s%s", productsModel.get(selectedIndex)
					.getRestUrl(), config.getDataURI());
		}

		return String.format("%s%s", config.getProduct().getRestUrl(),
				config.getDataURI());

	}

	@Listen("onClickSelectedInstance = #instancesListbox")
	public void onClickSelectedInstance(ForwardEvent event) {
		SessionUtil.getSecurityContext();
		Product product;
		DashboardInstance instance = (DashboardInstance) event.getData();

		if (productsModel != null && productsModel.size() > 1) {
			int selectedProductIndex = productListCombobox.getSelectedIndex();
			if(selectedProductIndex == 0)
				product = instance.getProduct();
			else
				product = productsModel.get(selectedProductIndex);
		} else {
			product = config.getProduct();
		}
		
		Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(product,
				"singleInstance" + "/" + instance.getOverview().getSQLServerId()));
	}

	private void crossRepoList() {

		for (Product product : productsList) {
			currentProduct = product;
			setWidgetData(DashboardWidgetFacade.getWidgetData(getDataURI(), getModelType()));
		}

	}

}
