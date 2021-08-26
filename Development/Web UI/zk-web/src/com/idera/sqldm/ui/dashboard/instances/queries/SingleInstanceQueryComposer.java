package com.idera.sqldm.ui.dashboard.instances.queries;

import java.util.ArrayList;
import java.util.List;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zkplus.databind.BindingListModelList;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.ListModel;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Paging;

import com.idera.common.Utility;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.instances.Query;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstancePreferencesBean;

public class SingleInstanceQueryComposer{

	private final Logger log = Logger.getLogger(SingleInstanceQueryComposer.class);
	private static final int QUERIES_SUMMARY_ROWS = 10;
	private int instanceId;
	private String productInstanceName;
	private ListModel<Query> instancesModel;
	private AnnotateDataBinder binder;
	private int prevPageSize;
	private int queriesListRowsCount;
	private int listRowsCount;

	private Grid singleInstanceQueryGrid;
	protected Intbox queriesSummaryListRowsBox;
	protected Paging queriesSummaryListPgId;

	@Init
	public void init() {
		int defaultAlertsRowCount = QUERIES_SUMMARY_ROWS;
		DashboardPreferencesBean dpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		if (dpb != null && dpb.getQueriesGridPageCount() != -1) {
			defaultAlertsRowCount = dpb.getQueriesGridPageCount();
		}
		prevPageSize = defaultAlertsRowCount;
		setListRowsCount(prevPageSize);
		queriesListRowsCount = prevPageSize;
		
	}
	
	@AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view){
        Selectors.wireComponents(view, this, false);

		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "id");
    	productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		} else {
			//fallback
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
				instanceId = (Integer) param;
		}

		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
        pref.setSelectedCategory(2);
        pref.setSelectedSubCategory(1);
        PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

/*		binder = new AnnotateDataBinder(view);

		instancesModel = new BindingListModelList<Query>(new ArrayList<Query>(), false);
		binder.bindBean("instancesModel", instancesModel);
		binder.bindBean("nameSortAsc", SingleInstanceQueriesFacade.QUERY_NAME_COMPARATOR_ASC);
		binder.bindBean("nameSortDesc", SingleInstanceQueriesFacade.QUERY_NAME_COMPARATOR_DESC);
		
		binder.loadAll();
		int defaultAlertsRowCount = QUERIES_SUMMARY_ROWS;
		DashboardPreferencesBean dpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		if (dpb != null && dpb.getQueriesGridPageCount() != -1) {
			defaultAlertsRowCount = dpb.getQueriesGridPageCount();
		}
		prevPageSize = defaultAlertsRowCount;
		setListRowsCount(prevPageSize);
		queriesListRowsCount = prevPageSize;
		queriesSummaryListRowsBox.setValue(prevPageSize);
		setQueriesSummaryListRowsCount();
		updateViewModel(getQueriesForThisInstance());
		singleInstanceQueryGrid.setPaginal(queriesSummaryListPgId);
*/	}
	
	//@Listen("onOK = #queriesSummaryListRowsBox")
	@Command("setQueriesSummaryListRowsCount")
	public void setQueriesSummaryListRowsCount() {
		try {
			int pageSize = this.queriesListRowsCount;
			setListRowsCount(this.queriesListRowsCount);
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(null, null, null, null, -1, -1,null, -1,-1,pageSize, -1,-1,-1);
			//queriesSummaryListPgId.setPageSize(pageSize);
			prevPageSize = pageSize;
		} catch (Exception ex) {
			log.error("Invalid value provided for alerts row configuration. Row count provided:" + queriesSummaryListRowsBox.getValue());
			setListRowsCount(prevPageSize);
		}
	}

	private void updateViewModel(List<Query> queriesForThisInstance) {

	//	addTestData(queriesForThisInstance);
		ListModelList<Query> list = new BindingListModelList<Query>(queriesForThisInstance, false);
		singleInstanceQueryGrid.setModel(list);

	}

	private List<Query> getQueriesForThisInstance() {
		List<Query> queries = new ArrayList<>();
		try {
			List<Query> queriesList = SingleInstanceQueriesFacade.getQueryInstance(productInstanceName,instanceId);
			queries.addAll(queriesList);
		} catch (InstanceException e) {
			singleInstanceQueryGrid.setEmptyMessage("");
		}
		return queries;
	}
	public int getQueriesListRowsCount() {
		return queriesListRowsCount;
	}

	public void setQueriesListRowsCount(int queriesListRowsCount) {
		this.queriesListRowsCount = queriesListRowsCount;
	}

	public int getListRowsCount() {
		return listRowsCount;
	}

	public void setListRowsCount(int listRowsCount) {
		this.listRowsCount = listRowsCount;
		BindUtils.postNotifyChange(null, null, this, "listRowsCount");
	}

}
