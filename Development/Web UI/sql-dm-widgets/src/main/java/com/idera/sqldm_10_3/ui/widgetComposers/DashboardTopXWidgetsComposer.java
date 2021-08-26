package com.idera.sqldm_10_3.ui.widgetComposers;

import com.idera.common.dashboard.Composers.DashboardBaseWidgetComposer;
import com.idera.common.rest.CoreRestClient;
import com.idera.core.facade.DashboardWidgetFacade;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.server.web.DashboardUtils;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.server.web.WebUtil;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.ui.preferences.PreferencesUtil;
import com.idera.sqldm_10_3.utils.Utility;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.ui.preferences.PreferencesUtil;
import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.*;

import java.util.HashMap;
import java.util.IllegalFormatConversionException;
import java.util.List;
import java.util.Map;

public abstract class DashboardTopXWidgetsComposer extends DashboardBaseWidgetComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<Product> productsModel;
	private List<IWidgetInstance> model;
	private Product currentProduct;
	private int appendFlag = 0;
	private List<Product> productsList;
	private AnnotateDataBinder binder;

	@Wire
	private Listbox topxInstancesListBox;
	@Wire
	private Listbox topxDatabasesListBox;
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

	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
		binder = new AnnotateDataBinder(getSelf());
		Map<String, String> settings = new HashMap<String, String>(
				config.getSettingsMap());
		binder.bindBean("setting", settings);
		binder.loadAll();
		getListBox().setVisible(false);
		getErrorContainer().setVisible(false);

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
			getProductListCombobox().setModel(productsModel);
		} else {
			getProductListCombobox().setVisible(false);
		}
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setWidgetData(Object obj) {
		if (obj == null) {
			String errorMessage = "Error while fetching data. Please check log for more details.";
			binder.bindBean("message", errorMessage);
			binder.loadAll();
			getListBox().setVisible(false);
			getErrorContainer().setVisible(true);
		} else {

			if(productListCombobox.getSelectedIndex() == 0) {
				for (IWidgetInstance instance : (List<IWidgetInstance>)obj) {
					instance.setProduct(currentProduct);
				}
				if(appendFlag == 0) {
					model = (List<IWidgetInstance>) obj;
					appendFlag = 1;
				}
				else
					model.addAll((List<IWidgetInstance>) obj);
			}
			else
				model = (List<IWidgetInstance>) obj;
			
			setListModel(model);
			updateRelativeSeverity(getListModel());
			
			int limit, listSize = getListModel().getSize();
			if(getLimit() != null)
				limit = getLimit().getValue();
			else
				limit = listSize;
			
			int toIndex = (limit < listSize)?limit:listSize;
			ListModelList<?> list = (ListModelList<?>) getListModel().subList(0, toIndex); 
			getListBox().setModel(list);
			getListBox().setVisible(true);
			getErrorContainer().setVisible(false);
		}
	}

	@Listen("onClick=#settingsActionButton")
	public void openSettingsPopup(Event event) {
		getWidgetSettings().open(event.getTarget(), "start_after");
	}


	@Listen("onClick=#save")
	public void onSave(Event event) {
		if (getLimit().getValue() > 10) {
			getLimit().focus();
			getLimit().setErrorMessage("Limit can't be more than 10");
			return;
		}
		Map<String, String> setting = config.getSettingsMap();
		setting.put("Limit", getLimit().getValue().toString());
		DashboardUtils.updateWidgetSettings(viewId, config.getId(), setting);
		getWidgetSettings().close();
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
		getProductListCombobox().setSelectedIndex(index);

	}

	@Override
	public String getDataURI() {
		Map<String, String> setting = config.getSettingsMap();
		int limit = 10, numDays=7;
		String urlParameters = "limit=";
		if (setting != null && setting.containsKey("Limit")) {
			try {
				limit = Integer.parseInt(setting.get("Limit"));
			} catch (IllegalFormatConversionException e) {
				getLogger().error(String
						.format("Unable to parse limit value for widget %s on viewId %d",
								config.getName(), viewId));
			}
		}
		urlParameters = urlParameters + limit;

		if (setting != null && setting.containsKey("numDays")) {
			try {
				numDays = Integer.parseInt(setting.get("numDays"));
				urlParameters = urlParameters + "&numDays=" + numDays;
			} catch (IllegalFormatConversionException e) {
				getLogger().error(String
						.format("Unable to parse number of days value for widget %s on viewId %d",
								config.getName(), viewId));
			}
		}
	
		String dataURI = config.getDataURI();
		if(dataURI.contains("{timeZoneOffset}")) {
			dataURI = dataURI.replace("{timeZoneOffset}", Utility.setOffSet());
		}
		
		if(productsModel != null && productsModel.size() > 1) {
			int selectedIndex = getProductListCombobox().getSelectedIndex();
			if(selectedIndex == 0) {
				return String.format("%s%s?%s", currentProduct.getRestUrl(),
						dataURI, urlParameters);
			}
			
			return String.format("%s%s?%s", productsModel.get(selectedIndex).getRestUrl(),
					dataURI, urlParameters);
		}
		
		return String.format("%s%s?%s", config.getProduct().getRestUrl(),
				dataURI, urlParameters);

	}

	@Listen("onSelect = #topxInstancesListBox, #topxDatabasesListBox")
	public void linkToDashboard(Event event) {
		
		int selectedIndex = getListBox().getSelectedIndex();
		int instanceId = getInstanceId(selectedIndex);
		Product product;
		if(productsModel != null && productsModel.size() > 1) {
			int selectedProductIndex = getProductListCombobox().getSelectedIndex();
			if(selectedProductIndex == 0)
				product = getSelectedInstanceProduct(selectedIndex);
			else
				product = productsModel.get(selectedProductIndex);
		}
		else {
				product = config.getProduct();
		}

		PreferencesUtil.getInstance().setInstanceTabSubTabs(instanceId, getInstanceSubCategory());
		Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(product, "singleInstance"+"/"+instanceId));
	}

	private void crossRepoList() {

		for (Product product : productsList) {
			currentProduct = product;
			setWidgetData(DashboardWidgetFacade.getWidgetData(getDataURI(), getModelType()));
		}

	}

	public void updateRelativeSeverity(ListModelList<?> list) {
		double max=0, tempVal;
		if (list != null && list.size() > 0 ){
			for(Object obj : list) {
				IWidgetInstance instance = (IWidgetInstance) obj;
				if (getUtilization(instance) != null) {
					tempVal  = Double.parseDouble(getUtilization(instance));
					if(tempVal > max) {
						max  = tempVal;
					}
				}
			}
			
			if (max > 0) {
				for (Object obj : list) {
					IWidgetInstance instance = (IWidgetInstance) obj;
					instance.setRelativeSeverityValue(Double.parseDouble(getUtilization(instance))*100/max);
				}
			}
		}
	}
	
	public Listbox getListBox() {
		if(topxInstancesListBox != null)
			return topxInstancesListBox;
		return topxDatabasesListBox;
	}

	public Hbox getErrorContainer() {
		return errorContainer;
	}

	public Popup getWidgetSettings() {
		return widgetSettings;
	}

	public Combobox getProductListCombobox() {
		return productListCombobox;
	}

	public Intbox getLimit() {
		return limit;
	}

	public abstract ListModelList<?> getListModel();
	public abstract void setListModel(Object obj);
	public abstract String getUtilization(IWidgetInstance instance);
	public abstract InstanceSubCategoriesTab getInstanceSubCategory();
	public abstract int getInstanceId(int selectedIndex);
	public abstract Product getSelectedInstanceProduct(int selectedIndex);
	public abstract String getEventName();
	public abstract Logger getLogger();

}
