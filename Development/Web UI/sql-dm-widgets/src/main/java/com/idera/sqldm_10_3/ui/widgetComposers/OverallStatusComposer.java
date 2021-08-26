package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.common.dashboard.Composers.DashboardBaseWidgetComposer;
import com.idera.common.rest.CoreRestClient;
import com.idera.core.facade.DashboardWidgetFacade;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.i18n.SQLdmI18NStrings;
import com.idera.sqldm_10_3.server.web.ELFunctions;
import com.idera.sqldm_10_3.server.web.WebUtil;
import com.idera.sqldm_10_3.ui.widgetModels.OverallStatusWidgetModel;
import com.idera.sqldm_10_3.ui.widgetModels.OverallStatusWidgetModel;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Hbox;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;

import java.util.List;

public class OverallStatusComposer extends DashboardBaseWidgetComposer {
	
	private ListModelList<Product> productsModel;
	@Wire
	private Combobox productListCombobox;

	@Wire
	private Hbox mainContainer;
	
	@Wire
	private Hbox errorContainer;
	
	@Wire
	private Hbox totalAlertsBox;
	
	@Wire 
	private Label lastSeenLabel;
	
	private static final long serialVersionUID = 1L;
	private OverallStatusWidgetModel instanceStatus;
	private AnnotateDataBinder binder;	
	
	private String labelId;
	private Product currentProduct;
	private int appendFlag = 0;
	private List<Product> productsList;
	
	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
		binder = new AnnotateDataBinder(getSelf());
		mainContainer.setVisible(false);
				
		productsList = CoreRestClient.getInstance().getProducts();
		productsModel = new ListModelList<>();
		String productName = config.getProduct().getProductNameWithoutInstanceName();
		String instanceName = config.getProduct().getInstanceName();
		if(instanceName == null)
			instanceName = ELFunctions.getMessage(SQLdmI18NStrings.DEFAULT_INSTANCE_NAME);
		
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
		
		labelId = "lastSeenLabel" + instanceName;
		lastSeenLabel.setId(labelId);

	}
	
	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<OverallStatusWidgetModel>(){};
	}

	@Override
	public void setWidgetData(Object obj) {
		try{
			if(obj != null){
				mainContainer.setVisible(true);
				errorContainer.setVisible(false);
								
				if (productListCombobox.getSelectedIndex() == 0) {
					if (appendFlag == 0) {
						instanceStatus = (OverallStatusWidgetModel)obj;
						appendFlag = 1;
					} else {
						OverallStatusWidgetModel newInstanceStatus = (OverallStatusWidgetModel)obj;
						
						int totalAlerts = instanceStatus.getAlertStatus().getTotalAlerts() + newInstanceStatus.getAlertStatus().getTotalAlerts();
						int totalCriticalAlerts = instanceStatus.getAlertStatus().getTotalCriticalAlerts() + newInstanceStatus.getAlertStatus().getTotalCriticalAlerts();
						int totalInformationalAlerts = instanceStatus.getAlertStatus().getTotalInformationalAlerts() + newInstanceStatus.getAlertStatus().getTotalInformationalAlerts();
						int totalWarningAlerts = instanceStatus.getAlertStatus().getTotalWarningAlerts() + newInstanceStatus.getAlertStatus().getTotalWarningAlerts();
						
						instanceStatus.getAlertStatus().setTotalAlerts(totalAlerts);
						instanceStatus.getAlertStatus().setTotalCriticalAlerts(totalCriticalAlerts);
						instanceStatus.getAlertStatus().setTotalInformationalAlerts(totalInformationalAlerts);
						instanceStatus.getAlertStatus().setTotalWarningAlerts(totalWarningAlerts);
					}
				} else
					instanceStatus = (OverallStatusWidgetModel)obj;

				if(instanceStatus.getAlertStatus().getTotalCriticalAlerts()>0)
					binder.bindBean("alertIcon", ELFunctions.getImageURLWithoutSize("alert-big"));
				else if(instanceStatus.getAlertStatus().getTotalWarningAlerts()>0)
					binder.bindBean("alertIcon", ELFunctions.getImageURLWithoutSize("warning-small"));
				else if(instanceStatus.getAlertStatus().getTotalInformationalAlerts()>0)
					binder.bindBean("alertIcon", ELFunctions.getImageURLWithoutSize("info"));
				else
					binder.bindBean("alertIcon", ELFunctions.getImageURLWithoutSize("ok_widget"));

				binder.bindBean("status", instanceStatus.getAlertStatus());
				comp.invalidate();
				binder.loadAll();

				Clients.evalJavaScript("setLastSeen(\'" + labelId + "\')");
			}else{
				binder.bindBean("message", "Error while fetching data. Please check log for more details.");
				instanceStatus = null;
				mainContainer.setVisible(false);
				errorContainer.setVisible(true);
				binder.loadAll();
			}
		}catch(Exception e){
			
		}
	}

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(), OverallStatusComposer.class.getName());
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
		String url = String.format("%s%s", config.getProduct().getRestUrl(), config.getDataURI());
		if(productsModel != null && productsModel.size() > 1) {
			int selectedIndex = productListCombobox.getSelectedIndex();
			if(selectedIndex == 0) {
				return String.format("%s%s", currentProduct.getRestUrl(),
						config.getDataURI());
			}
			
			return String.format("%s%s", productsModel.get(selectedIndex).getRestUrl(),
				config.getDataURI());
		}
		
		return url;
	}
	
	@Listen("onClick = #totalAlertsBox")
	public void linkToDashboard(Event event) {
		Product product;
		if(productsModel != null && productsModel.size() > 1) {
			int selectedProductIndex = productListCombobox.getSelectedIndex();
			if (selectedProductIndex == 0) {
				product = config.getProduct();
				Sessions.getCurrent().setAttribute("AllFlag", true);
			} else
				product = productsModel.get(selectedProductIndex);
		}
		else {
				product = config.getProduct();
		}

		Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(product, "home"));
	}

	public String getLabelId() {
		return labelId;
	}

	public void setLabelId(String labelId) {
		this.labelId = labelId;
	}
	
	private void crossRepoList() {

		for (Product product : productsList) {
			currentProduct = product;
			setWidgetData(DashboardWidgetFacade.getWidgetData(getDataURI(), getModelType()));
		}

	}

}