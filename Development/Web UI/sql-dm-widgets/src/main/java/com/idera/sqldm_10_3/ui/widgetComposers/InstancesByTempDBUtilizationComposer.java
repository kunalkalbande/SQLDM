package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.TempDBUtilizationInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.TempDBUtilizationInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class InstancesByTempDBUtilizationComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<TempDBUtilizationInstance> listModel;
	private Logger logger = Logger.getLogger(InstancesByTempDBUtilizationComposer.class);

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<TempDBUtilizationInstance>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((TempDBUtilizationInstance)instance).getSpaceUtilization();
		return utilization;
	}

	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.DATABASES_TEMPDB;
	}	

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				TempDBUtilizationInstance.class.getName());
	}

	@Override
	public ListModelList<?> getListModel() {
		return listModel;
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setListModel(Object obj) {
		listModel = new ListModelList<TempDBUtilizationInstance>(
				(Collection<? extends TempDBUtilizationInstance>) obj);

		Collections.sort(listModel, new Comparator<TempDBUtilizationInstance>() {
			@Override
			public int compare(TempDBUtilizationInstance o1, TempDBUtilizationInstance o2) {
				Double v1 = o1.getSpaceUtilization();
				Double v2 = o2.getSpaceUtilization();
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
