package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.TopDatabaseByActivityWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.TopDatabaseByActivityWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class DatabasesByActivityComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<TopDatabaseByActivityWidgetInstance> listModel;
	private Logger logger = Logger.getLogger(DatabasesByActivityComposer.class);

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<TopDatabaseByActivityWidgetInstance>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((TopDatabaseByActivityWidgetInstance)instance).getTransactionValue();
		return utilization;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.DATABASES_SUMMARY;
	}	

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				TopDatabaseByActivityWidgetInstance.class.getName());
	}

	@Override
	public ListModelList<TopDatabaseByActivityWidgetInstance> getListModel() {
		return listModel;
	}

	@Override
	@SuppressWarnings("unchecked")
	public void setListModel(Object obj) {
		
		listModel = new ListModelList<TopDatabaseByActivityWidgetInstance>(
				(Collection<? extends TopDatabaseByActivityWidgetInstance>) obj);

		Collections.sort(listModel, new Comparator<TopDatabaseByActivityWidgetInstance>() {
			@Override
			public int compare(TopDatabaseByActivityWidgetInstance o1, TopDatabaseByActivityWidgetInstance o2) {
				Double v1 = o1.getTransactionValue();
				Double v2 = o2.getTransactionValue();
				return v2.compareTo(v1);
			}
		});
			
	}

	@Override
	public Product getSelectedInstanceProduct(int selectedIndex) {
		return listModel.get(selectedIndex).getProduct();
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
