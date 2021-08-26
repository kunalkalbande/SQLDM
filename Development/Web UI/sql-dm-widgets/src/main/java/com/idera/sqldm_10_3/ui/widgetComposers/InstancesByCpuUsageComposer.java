package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.topten.CpuLoad;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.topten.CpuLoad;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class InstancesByCpuUsageComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<CpuLoad> listModel;
	private Logger logger = Logger.getLogger(InstancesByCpuUsageComposer.class);

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<CpuLoad>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget panel) throws Exception {
		super.doAfterCompose(panel);
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((CpuLoad)instance).getCpuLoad();
		return utilization;
	}

	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.RESOURCES_CPUVIEW;
	}	

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				CpuLoad.class.getName());
	}

	@Override
	public ListModelList<?> getListModel() {
		return listModel;
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setListModel(Object obj) {
		listModel = new ListModelList<CpuLoad>(
				(Collection<? extends CpuLoad>) obj);

		Collections.sort(listModel, new Comparator<CpuLoad>() {
			@Override
			public int compare(CpuLoad o1, CpuLoad o2) {
				Double v1 = o1.getCpuLoad();
				Double v2 = o2.getCpuLoad();
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
