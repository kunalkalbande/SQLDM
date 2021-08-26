package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.topten.BlockedSession;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.topten.BlockedSession;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class InstancesByBlockedSessionsComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<BlockedSession> listModel;
	private Logger logger = Logger.getLogger(InstancesByBlockedSessionsComposer.class);

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<BlockedSession>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((BlockedSession)instance).getNumberOfBlockedProcess();
		return utilization;
	}

	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.SESSIONS_OVERVIEW;
	}	

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				BlockedSession.class.getName());
	}

	@Override
	public ListModelList<?> getListModel() {
		return listModel;
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setListModel(Object obj) {
		listModel = new ListModelList<BlockedSession>(
				(Collection<? extends BlockedSession>) obj);

		Collections.sort(listModel, new Comparator<BlockedSession>() {
			@Override
			public int compare(BlockedSession o1, BlockedSession o2) {
				Integer v1 = o1.getNumberOfBlockedProcess();
				Integer v2 = o2.getNumberOfBlockedProcess();
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
