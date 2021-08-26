package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.topten.DiskSpaceWidgetInstance;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.topten.DiskSpaceWidgetInstance;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class InstancesByDiskSpaceComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<DiskSpaceWidgetInstance> listModel;
	private Logger logger = Logger.getLogger(InstancesByDiskSpaceComposer.class);

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<DiskSpaceWidgetInstance>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((DiskSpaceWidgetInstance)instance).getDiskSpaceUtilization();
		return utilization;
	}

	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.RESOURCES_DISKVIEW;
	}	

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				DiskSpaceWidgetInstance.class.getName());
	}

	@Override
	public ListModelList<?> getListModel() {
		return listModel;
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setListModel(Object obj) {
		listModel = new ListModelList<DiskSpaceWidgetInstance>(
				(Collection<? extends DiskSpaceWidgetInstance>) obj);

		Collections.sort(listModel, new Comparator<DiskSpaceWidgetInstance>() {
			@Override
			public int compare(DiskSpaceWidgetInstance o1, DiskSpaceWidgetInstance o2) {
				Double v1 = Double.parseDouble(o1.getDiskSpaceUtilization());
				Double v2 = Double.parseDouble(o2.getDiskSpaceUtilization());
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
