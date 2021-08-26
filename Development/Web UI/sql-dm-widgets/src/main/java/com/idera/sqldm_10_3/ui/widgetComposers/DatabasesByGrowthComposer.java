package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.topten.FastestProjectedGrowingDatabasesWidgetInstance;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.topten.FastestProjectedGrowingDatabasesWidgetInstance;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class DatabasesByGrowthComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<FastestProjectedGrowingDatabasesWidgetInstance> listModel;
	private Logger logger = Logger.getLogger(DatabasesByGrowthComposer.class);

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<FastestProjectedGrowingDatabasesWidgetInstance>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
	}
	
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((FastestProjectedGrowingDatabasesWidgetInstance)instance).getTotalSizeDiffernceKb();
		return utilization;
	}

	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.DATABASES_SUMMARY;
	}	

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				FastestProjectedGrowingDatabasesWidgetInstance.class.getName());
	}

	@Override
	public ListModelList<?> getListModel() {
		return listModel;
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setListModel(Object obj) {
		listModel = new ListModelList<FastestProjectedGrowingDatabasesWidgetInstance>(
				(Collection<? extends FastestProjectedGrowingDatabasesWidgetInstance>) obj);

		Collections.sort(listModel, new Comparator<FastestProjectedGrowingDatabasesWidgetInstance>() {
			@Override
			public int compare(FastestProjectedGrowingDatabasesWidgetInstance o1, FastestProjectedGrowingDatabasesWidgetInstance o2) {
				Long v1 = o1.getTotalSizeDiffernceKb();
				Long v2 = o2.getTotalSizeDiffernceKb();
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
