package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.MemoryUsageWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.MemoryUsageWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class InstancesByMemoryUsageComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<MemoryUsageWidgetInstance> listModel;
	private Logger logger = Logger.getLogger(InstancesByMemoryUsageComposer.class);

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<MemoryUsageWidgetInstance>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
	}
	
	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((MemoryUsageWidgetInstance)instance).getMemoryUsage();
		return utilization;
	}

	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.RESOURCES_MEMORYVIEW;
	}	

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				MemoryUsageWidgetInstance.class.getName());
	}

	@Override
	public ListModelList<?> getListModel() {
		return listModel;
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setListModel(Object obj) {
		listModel = new ListModelList<MemoryUsageWidgetInstance>(
				(Collection<? extends MemoryUsageWidgetInstance>) obj);

		Collections.sort(listModel, new Comparator<MemoryUsageWidgetInstance>() {
			@Override
			public int compare(MemoryUsageWidgetInstance o1, MemoryUsageWidgetInstance o2) {
				Double v1 = o1.getMemoryUsage();
				Double v2 = o2.getMemoryUsage();
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
