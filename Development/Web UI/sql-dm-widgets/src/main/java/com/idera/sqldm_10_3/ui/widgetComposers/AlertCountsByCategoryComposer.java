package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.common.dashboard.Composers.DashboardBaseWidgetComposer;
import com.idera.common.rest.CoreRestClient;
import com.idera.core.facade.DashboardWidgetFacade;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.DashboardAlertsByCategoryWidget;
import com.idera.sqldm_10_3.server.web.WebUtil;
import com.idera.sqldm_10_3.utils.SQLdmConstants;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.*;

import java.util.*;

public class AlertCountsByCategoryComposer extends DashboardBaseWidgetComposer {

	private static final long serialVersionUID = 1L;
	private List<DashboardAlertsByCategoryWidget> model;
	private ListModelList<DashboardAlertsByCategoryWidget> listModel;
	private ListModelList<Product> productsModel;
	private Map<String, Integer> categoryCounts;

	@Wire(".top-x-resource-by-count-panel .alert-counts-by-category")
	private Listbox alertCountsByCategory;
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
		return new TypeReference<List<DashboardAlertsByCategoryWidget>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget panel) throws Exception {
		super.doAfterCompose(panel);
		binder = new AnnotateDataBinder(getSelf());

		limit.setValue(SQLdmConstants.DASHBOARD_RHS_WIDGET_ALERT_BY_CATEGORY_DEFAULT);
		alertCountsByCategory.setVisible(false);
		errorContainer.setVisible(false);

		productsList = CoreRestClient.getInstance().getProducts();
		productsModel = new ListModelList<>();
		String productName = config.getProduct().getProductNameWithoutInstanceName();
		
		for(Product product: productsList) {
			if(product.getProductNameWithoutInstanceName().equals(productName))
				productsModel.add(product);
		}

		if(productsModel.size() > 1) {
			Product all = new Product();
			all.setName("All");
			all.setInstanceName("firstinstance");
			productsModel.add(0, all);
			productListCombobox.setModel(productsModel);
		} else {
			productListCombobox.setVisible(false);
		}
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setWidgetData(Object obj) {
		if (obj == null) {
			String errorMessage = "Error while fetching data. Please check log for more details.";
			binder.bindBean("message", errorMessage);
			binder.loadAll();
			alertCountsByCategory.setVisible(false);
			errorContainer.setVisible(true);
		} else {

			if(productListCombobox.getSelectedIndex() == 0) {
				if(appendFlag == 0) {
					model = (List<DashboardAlertsByCategoryWidget>) obj;
					appendFlag = 1;
					categoryCounts = new HashMap<>();
					for (DashboardAlertsByCategoryWidget category : model) {
						categoryCounts.put(category.getCategory(), category.getNumOfAlerts());
					}
				}
				else {
					model = (List<DashboardAlertsByCategoryWidget>) obj;
					for (DashboardAlertsByCategoryWidget category : model) {
						categoryCounts.put(category.getCategory(), category.getNumOfAlerts() + categoryCounts.get(category.getCategory()));
					}
					model = new ArrayList<>();
					for (String category : categoryCounts.keySet()) {
						model.add(new DashboardAlertsByCategoryWidget(category, categoryCounts.get(category)));
					}
				}
			}
			else
				model = (List<DashboardAlertsByCategoryWidget>) obj;
			
			Collections.sort(model, new Comparator<DashboardAlertsByCategoryWidget>() {

				@Override
				public int compare(DashboardAlertsByCategoryWidget o1,
						DashboardAlertsByCategoryWidget o2) {
					return o2.getNumOfAlerts().compareTo(o1.getNumOfAlerts());
				}
			});
			
			int limitCount = limit.getValue();
			if(limitCount < model.size())
				listModel = new ListModelList<DashboardAlertsByCategoryWidget>(model.subList(0, limitCount));
			else
				listModel = new ListModelList<DashboardAlertsByCategoryWidget>(model);

			alertCountsByCategory.setModel(listModel);
			alertCountsByCategory.setVisible(true);
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
				DashboardAlertsByCategoryWidget.class.getName());
	}

	@Listen("onClick=#save")
	public void onSave(Event event) {
		if (limit.getValue() > SQLdmConstants.DASHBOARD_RHS_WIDGET_ALERT_BY_CATEGORY_DEFAULT) {
			limit.focus();
			limit.setErrorMessage("Limit can't be more than " + SQLdmConstants.DASHBOARD_RHS_WIDGET_ALERT_BY_CATEGORY_DEFAULT);
			return;
		}

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
		for(int i = productsModel.size() - 1; i >= 0; i--) {
			if(productsModel.get(i).getName().equals(config.getProduct().getName())) {
				index = i;
				break;
			}
		}
		productListCombobox.setSelectedIndex(index);

	}

	@Override
	public String getDataURI() {

		if(productsModel != null && productsModel.size() > 1) {
			int selectedIndex = productListCombobox.getSelectedIndex();
			if(selectedIndex == 0) {
				return String.format("%s%s", currentProduct.getRestUrl(),
						config.getDataURI());
			}
			
			return String.format("%s%s", productsModel.get(selectedIndex).getRestUrl(),
				config.getDataURI());
		}
		
		return String.format("%s%s", config.getProduct().getRestUrl(),
				config.getDataURI());
	}

	@Listen("onSelect = #alertCountsByCategory")
	public void linkToDashboard(Event event) {
		Product product;

		if(productsModel != null && productsModel.size() > 1) {
			int selectedIndex = productListCombobox.getSelectedIndex();
			if (selectedIndex == 0) {
				product = config.getProduct();
				Sessions.getCurrent().setAttribute("AllFlag", true);
			} else
				product = productsModel.get(selectedIndex);
		}
		else {
			product = config.getProduct();
		}
		Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(product, "alerts"));
	}

	private void crossRepoList() {

		for (Product product : productsList) {
			currentProduct = product;
			setWidgetData(DashboardWidgetFacade.getWidgetData(getDataURI(), getModelType()));
		}

	}

}