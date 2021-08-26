package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm_10_3.data.TopXWidgetFacade;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.LongestRunningQueryInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.data.TopXWidgetFacade;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.LongestRunningQueryInstance;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import org.apache.log4j.Logger;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listheader;

import java.util.Collection;
import java.util.Collections;
import java.util.List;

public class QueriesByExecutionTimeComposer extends DashboardTopXWidgetsComposer {

	private static final long serialVersionUID = 1L;
	private ListModelList<LongestRunningQueryInstance> listModel;
	private Logger logger = Logger.getLogger(QueriesByExecutionTimeComposer.class);
	@Wire
	private Listheader cpuTime;
	@Wire
	private Listheader queryTime;

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<List<LongestRunningQueryInstance>>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget panel) throws Exception {
		super.doAfterCompose(panel);
		
		cpuTime.setSortAscending(TopXWidgetFacade.CPU_TIME_ASC);
		cpuTime.setSortDescending(TopXWidgetFacade.CPU_TIME_DESC);
		queryTime.setSortAscending(TopXWidgetFacade.QUERY_EXEC_TIME_ASC);
		queryTime.setSortDescending(TopXWidgetFacade.QUERY_EXEC_TIME_DESC);

	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((LongestRunningQueryInstance)instance).getQueryExecTimeInMs();
		return utilization;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.QUERIES_SUMMARY;
	}	

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				LongestRunningQueryInstance.class.getName());
	}

	@Override
	public ListModelList<?> getListModel() {
		return listModel;
	}

	@SuppressWarnings("unchecked")
	@Override
	public void setListModel(Object obj) {
		listModel = new ListModelList<LongestRunningQueryInstance>(
				(Collection<? extends LongestRunningQueryInstance>) obj);

		Collections.sort(listModel, TopXWidgetFacade.QUERY_EXEC_TIME_DESC);
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
