package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.TopDatabaseByAlertWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.TopDatabaseByAlertWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class DatabasesByAlertsCountComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<TopDatabaseByAlertWidgetInstance> listModel;
	private Logger logger = Logger.getLogger(DatabasesByAlertsCountComposer.class);

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<TopDatabaseByAlertWidgetInstance>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget panel) throws Exception {
		super.doAfterCompose(panel);
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((TopDatabaseByAlertWidgetInstance)instance).getNumberOfAlerts();
		return utilization;
	}

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				TopDatabaseByAlertWidgetInstance.class.getName());
	}

	@Override
	public ListModelList<?> getListModel() {
		return listModel;
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setListModel(Object obj) {
	
		listModel = new ListModelList<TopDatabaseByAlertWidgetInstance>(
				(Collection<? extends TopDatabaseByAlertWidgetInstance>) obj);

		Collections.sort(listModel, new Comparator<TopDatabaseByAlertWidgetInstance>() {
			@Override
			public int compare(TopDatabaseByAlertWidgetInstance o1, TopDatabaseByAlertWidgetInstance o2) {
				Long v1 = o1.getNumberOfAlerts();
				Long v2 = o2.getNumberOfAlerts();
				return v2.compareTo(v1);
			}
		});		
	}

	@Override
	public Product getSelectedInstanceProduct(int selectedIndex) {
		return listModel.get(selectedIndex).getProduct();
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.DATABASES_SUMMARY;
	}

	@Override
	public int getInstanceId(int selectedIndex) {
		return listModel.get(selectedIndex).getInstanceId();
	}

	@Override
	public Logger getLogger() {
		return logger;
	}

}
