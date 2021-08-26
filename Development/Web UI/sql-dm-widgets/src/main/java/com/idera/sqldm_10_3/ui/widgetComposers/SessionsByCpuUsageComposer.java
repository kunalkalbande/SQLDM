package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.Sessions;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.Sessions;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class SessionsByCpuUsageComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<Sessions> listModel;
	private Logger logger = Logger.getLogger(SessionsByCpuUsageComposer.class);

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<Sessions>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget panel) throws Exception {
		super.doAfterCompose(panel);
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((Sessions)instance).getSessionCPUUsage();
		return utilization;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.SESSIONS_SUMMARY;
	}	

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				Sessions.class.getName());
	}
	@Override
	public ListModelList<?> getListModel() {
		return listModel;
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setListModel(Object obj) {
		listModel = new ListModelList<Sessions>(
				(Collection<? extends Sessions>) obj);

		Collections.sort(listModel, new Comparator<Sessions>() {
			@Override
			public int compare(Sessions o1, Sessions o2) {
				Double v1 = o1.getSessionCPUUsage();
				Double v2 = o2.getSessionCPUUsage();
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
