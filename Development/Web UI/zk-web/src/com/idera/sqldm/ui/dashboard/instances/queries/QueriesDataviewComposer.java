package com.idera.sqldm.ui.dashboard.instances.queries;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedHashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.StringTokenizer;

import org.apache.commons.lang.time.DateUtils;
import org.apache.log4j.Logger;
import org.zkoss.addon.columnchooser.Columnchooser;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.DependsOn;
import org.zkoss.bind.annotation.Init;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zkplus.databind.BindingListModelList;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Window;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.DashboardInstanceFacade;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.queries.QueryApplication;
import com.idera.sqldm.data.queries.QueryApplicationDetails;
import com.idera.sqldm.data.queries.QueryClients;
import com.idera.sqldm.data.queries.QueryDatabases;
import com.idera.sqldm.data.queries.QueryGraph;
import com.idera.sqldm.data.queries.QueryGraphFacade;
import com.idera.sqldm.data.queries.QueryGroups;
import com.idera.sqldm.data.queries.QueryGroupsFacade;
import com.idera.sqldm.data.queries.QueryListFacade;
import com.idera.sqldm.data.queries.QueryPlan;
import com.idera.sqldm.data.queries.QueryPlanFacade;
import com.idera.sqldm.data.queries.QueryUsers;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.session.SessionUtil;
import com.idera.sqldm.ui.components.charts.bar.IderaBarChart;
import com.idera.sqldm.ui.components.charts.bar.IderaStackedBarChart;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstanceQueriesPreferencesBean;
import com.idera.sqldm.utils.Utility;

public class QueriesDataviewComposer {
	private static final long serialVersionUID = 1L;
	protected static final Logger log = Logger
			.getLogger(QueriesDataviewComposer.class);

	String offsetValue;

	@Wire
	protected IderaBarChart queryWaitDurationGraph;

	@Wire
	IderaStackedBarChart queryChart;

	@Wire
	Label QueriesTableTitle;
	@Wire
	Grid queriesWaitGrid;
	Paging instancesListPgId;
	Paging queriesListPgId;

	private int instanceId = 0;

	private int queriesListRowsCount;
	private int listRowsCount;
	private int activePage = 0;
	private int defaultQueriesRowCount = 20;

	private boolean drillUp = false;
	private boolean direct = false;

	private String queriesWaitsLabel;
	public String queriesWaitsTitle;
	private String fromDateTime;
	private String endDateTime;
	private String selectedGroupBy = "Application";

	int drillDown = 0;

	private QueryFilter filter;
	private QueryApplicationDetails queryRow;
	// Selected Groups
	private QueryGroups queriesGroupBy1 = new QueryGroups();
	private QueryGroups queriesGroupBy2 = new QueryGroups();

	// queries model for grid
	public ListModelList<QueryApplicationDetails> queriesModel = new BindingListModelList<QueryApplicationDetails>(
			new LinkedList<QueryApplicationDetails>(), false);

	// available column list
	private List<String> columnList = new ArrayList<String>();
	// Displayed column list
	private List<String> defaultColumnList = new ArrayList<String>();

	private List<QueryApplication> selectedApplications;

	private List<QueryDatabases> selectedDatabases;

	private List<QueryClients> selectedClients;

	private List<QueryUsers> selectedUsers;

	private int selectedQuerySignatureId = -1;
	private int selectedEventId = -1;

	private List<QueryApplicationDetails> queriesDetails = new ArrayList<QueryApplicationDetails>();

	private QueryColumns[] columns = new QueryColumns[60];

	// Hashmap for storing properties of each column
	private HashMap<String, QueryColumns> queryColumns = new HashMap<String, QueryColumns>();
	private HashMap<String, String> queryIds = new HashMap<>();

	private DashboardInstance instance;
	private String productInstanceName;
	private Integer queryID;
	private String startDateTimeURL;
	private String endDateTimeURL;
	private Integer statType;
	private Integer querySignID;

	@Init
	public void init() {
		
		// URL
		queryID = Utility.getIntUrlParameter(Executions.getCurrent()
				.getParameterMap(), "query");
		querySignID = Utility.getIntUrlParameter(Executions.getCurrent()
				.getParameterMap(), "querysignid");
		statType = Utility.getIntUrlParameter(Executions.getCurrent()
				.getParameterMap(), "stattype");
		startDateTimeURL = Utility.getUrlParameter(Executions.getCurrent()
				.getParameterMap(), "starttime");
		endDateTimeURL = Utility.getUrlParameter(Executions.getCurrent()
				.getParameterMap(), "endtime");

		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions
				.getCurrent().getParameterMap(), "id");
		productInstanceName = Utility.getUrlParameter(Executions.getCurrent()
				.getParameterMap(), "instance");

		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		} else {
			// fallback
			Object param = Executions.getCurrent().getDesktop()
					.getAttribute("instanceId");
			if (param != null) {
				instanceId = (Integer) param;
			}
		}

		Clients.evalJavaScript("setOffset()");

		// Set the selected filters from session
		SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueriesPreferenceInSession(instanceId);
		if (sessionBean.getQueryFilter() != null) {

			filter = sessionBean.getQueryFilter();
			selectedApplications = sessionBean.getCheckedApplications();
			selectedDatabases = sessionBean.getCheckedDatabases();
			selectedClients = sessionBean.getCheckedClients();
			selectedUsers = sessionBean.getCheckedUsers();

			setDrillDown(sessionBean.getDrill());
			setDrillUp(getDrillDown() != 0);

			setQueryIds(sessionBean.getQueryIds());
		}

		Integer pageCount = defaultQueriesRowCount;
		if (sessionBean.getPageCount() != -1) {
			// update selected filters if available in session.
			pageCount = sessionBean.getPageCount();
		} else {
			sessionBean.setPageCount(pageCount);
			PreferencesUtil.getInstance().setQueriesPreferenceInSession(
					sessionBean);
		}

		setListRowsCount(pageCount);
		setQueriesListRowsCount(pageCount);

		for (int i = 0; i < 60; i++) {
			columns[i] = new QueryColumns();
		}

		setDisplayMessageForQueriesGrid(ELFunctions
				.getMessage(SQLdmI18NStrings.LOADING_DATA));

		addColumnsToList();
		setVisibleAndHiddenColumnsList();
		setTableTitle();

	}

	public void updateModelRest(String productInstanceName) {
		String selectedApplicationIDs = "";
		String selectedDBIDs = "";
		String selectedUserIDs = "";
		String selectedClientIDs = "";
		String selectedAdvancedFilters = "";
		int selectedGroupId = filter.getSelectedGroupBy().getGroupId();

		for (QueryApplication app : selectedApplications) {
			selectedApplicationIDs += app.getApplicationId() + "|";
		}
		if (selectedApplicationIDs.length() > 0)
			selectedApplicationIDs = selectedApplicationIDs.substring(0,
					selectedApplicationIDs.length() - 1);

		for (QueryClients client : selectedClients) {
			selectedClientIDs += client.getClientId() + "|";
		}
		if (selectedClientIDs.length() > 0)
			selectedClientIDs = selectedClientIDs.substring(0,
					selectedClientIDs.length() - 1);

		for (QueryDatabases db : selectedDatabases) {
			selectedDBIDs += db.getDatabaseId() + "|";
		}
		if (selectedDBIDs.length() > 0)
			selectedDBIDs = selectedDBIDs.substring(0,
					selectedDBIDs.length() - 1);

		for (QueryUsers user : selectedUsers) {
			selectedUserIDs += user.getUserId() + "|";
		}
		if (selectedUserIDs.length() > 0)
			selectedUserIDs = selectedUserIDs.substring(0,
					selectedUserIDs.length() - 1);

		selectedAdvancedFilters += filter.isShowSQLStatements() ? "1" : "0";
		selectedAdvancedFilters += filter.isShowStoredProcs() ? "1" : "0";
		selectedAdvancedFilters += filter.isShowSQLBatches() ? "1" : "0";
		selectedAdvancedFilters += filter.isIncludeIncomplete() ? "1" : "0";
		selectedAdvancedFilters += filter.isIncludeOverlapping() ? "1" : "0";

		SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueriesPreferenceInSession(instanceId);

		if (sessionBean.getQueryGroupOptions() == null) {
			try {
				sessionBean.setQueryGroupOptions(QueryGroupsFacade
						.getQueryGroups(productInstanceName));
				PreferencesUtil.getInstance().setQueriesPreferenceInSession(
						sessionBean);
			} catch (RestException e) {
				e.printStackTrace();
			}
		}

		if (drillDown != 0) {
			if (selectedGroupBy.equals("Application")
					|| selectedGroupBy.equals("Database")
					|| selectedGroupBy.equals("Client")
					|| selectedGroupBy.equals("User")) {

				for (QueryGroups group : sessionBean.getQueryGroupOptions()) {
					if (group.getGroupName()
							.equalsIgnoreCase("Query Signature")) {
						selectedGroupId = group.getGroupId();
					}
				}
			} else if (selectedGroupBy.equals("Query Signature")
					&& drillDown == 2) {

				for (QueryGroups group : sessionBean.getQueryGroupOptions()) {
					if (group.getGroupName()
							.equalsIgnoreCase("Query Statement")) {
						selectedGroupId = group.getGroupId();
					}
				}
			}
		} else if (!selectedGroupBy.equals("Query Signature")) {
			selectedQuerySignatureId = -1;
			selectedEventId = -1;
		}

		try {
			setOffSet();
			queryIds = new HashMap<>();
			queriesDetails = QueryListFacade.getQueryApplications(
					productInstanceName, instanceId, filter.getSelectedView()
							.getMetricId(), selectedGroupId, Double
							.parseDouble(offsetValue), selectedApplicationIDs,
					selectedDBIDs, selectedUserIDs, selectedClientIDs, filter
							.getIncludeSQLText(), filter.getExcludeSQLText(),
					selectedAdvancedFilters, filter.getFromDateTimeAPI(),
					filter.getEndDateTimeAPI(), 0, listRowsCount, null, null,
					selectedQuerySignatureId, selectedEventId);

			for (int i = 0; i < queriesDetails.size(); i++) {

				if (((selectedGroupBy.equals("Application")
						|| selectedGroupBy.equals("User")
						|| selectedGroupBy.equals("Database") || selectedGroupBy
							.equals("Client")) && (drillDown == 1))
						|| ((selectedGroupBy.equals("Query Signature")) && (drillDown != 2))) {

					queriesDetails.get(i).setQueryNum(
							"Query Signature " + (i + 1));
					queryIds.put(queriesDetails.get(i).getSqlSignatureID()
							+ ":" + queriesDetails.get(i).getEventId(),
							queriesDetails.get(i).getQueryNum());

				} else if ((selectedGroupBy.equals("Query Signature"))
						&& (drillDown == 2)) {
					queriesDetails.get(i).setQueryNum(
							"Query Statement " + (i + 1));
					queryIds.put(queriesDetails.get(i).getQueryStatisticsID()
							+ "", queriesDetails.get(i).getQueryNum());
				}
			}
		} catch (RestException e) {
			e.printStackTrace();
		}
		setQueriesModel(queriesDetails);
	}

	@NotifyChange
	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view)
			throws Exception {
		Selectors.wireComponents(view, this, false);
		Selectors.wireEventListeners(view, this);

		try {
			instance = DashboardInstanceFacade.getDashboardInstance(
					productInstanceName, instanceId);

		} catch (InstanceException x) {
			log.error(x.getMessage(), x);
		}

		EventQueue<Event> eq = EventQueues.lookup("changeQueryFilter",
				EventQueues.DESKTOP, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if (event.getName().equals("filterChanged")) {
					SessionUtil.getSecurityContext();
					updateViewUsingSession();
					setVisibleAndHiddenColumnsList();
					setTableTitle();

					Integer pageCount = defaultQueriesRowCount;
					SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
							.getInstance().getQueriesPreferenceInSession(
									instanceId);
					if (sessionBean.getPageCount() != -1) {
						pageCount = sessionBean.getPageCount();
					}

					setListRowsCount(pageCount);
					setQueriesListRowsCount(pageCount);

					// Rest API Call
					updateModelRest(productInstanceName);
					drawChart();

				}
			}
		});

	}

	/**
	 * pop up for column chooser
	 * 
	 * @param ref
	 * @param columnChooser
	 */
	@Command
	public void openDefaultColumnChooser(@BindingParam("ref") Component ref,
			@BindingParam("ref2") Columnchooser columnChooser) {
		columnChooser.open(ref, "after_pointer");
	}

	/**
	 * Updates the view according to session values
	 */
	@NotifyChange
	public void updateViewUsingSession() {

		SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueriesPreferenceInSession(instanceId);

		if (sessionBean.getQueryFilter() != null) {
			setFilter(sessionBean.getQueryFilter());
			setSelectedApplications(sessionBean.getCheckedApplications());
			setSelectedDatabases(sessionBean.getCheckedDatabases());
			setSelectedClients(sessionBean.getCheckedClients());
			setSelectedUsers(sessionBean.getCheckedUsers());

			setActivePage(0);
			setListRowsCount(sessionBean.getPageCount());

			setFromDateTime(filter.getFromDateTime());
			setEndDateTime(filter.getEndDateTime());

			setDrillDown(sessionBean.getDrill());
			setDrillUp(getDrillDown() != 0);

			setQueryIds(sessionBean.getQueryIds());

			BindUtils
					.postNotifyChange(null, null, this, "selectedApplications");
			BindUtils.postNotifyChange(null, null, this, "selectedDatabases");
			BindUtils.postNotifyChange(null, null, this, "selectedUsers");
			BindUtils.postNotifyChange(null, null, this, "selectedClients");
			BindUtils.postNotifyChange(null, null, this, "filter");
			BindUtils.postNotifyChange(null, null, this, "fromDateTime");
			BindUtils.postNotifyChange(null, null, this, "endDateTime");
			BindUtils.postNotifyChange(null, null, this, "activePage");
			BindUtils.postNotifyChange(null, null, this, "listRowsCount");

			// Set from the group box
			if (sessionBean.getSet() == 1)
				drillUp = false;
			// else drillUp =true;

			BindUtils.postNotifyChange(null, null, this, "drillDown");
		} else {
			Messagebox.show(ELFunctions.getMessage(SQLdmI18NStrings.ERROR_MESSAGE_QUERIES));
		}
	}

	protected void drawChart() {
		// try {

		Map<String, String> legendLinks = new LinkedHashMap<>();
		Map<String, String> axisLinks = new LinkedHashMap<>();
		List<QueryGraph> graphBars = new ArrayList<QueryGraph>();
		Map<String, Number> testData = new LinkedHashMap<String, Number>();
		LinkedHashMap<String, Map<String, Number>> chartData = new LinkedHashMap<String, Map<String, Number>>();

		String startDateTime = filter.getFromDateTimeAPI();
		String endDateTime = filter.getEndDateTimeAPI();

		// Making the filters to be passed to the Rest API
		String selectedApplicationIDs = "";
		String selectedDBIDs = "";
		String selectedUserIDs = "";
		String selectedClientIDs = "";
		String selectedAdvancedFilters = "";

		for (QueryApplication app : selectedApplications) {
			selectedApplicationIDs += app.getApplicationId() + "|";
		}
		if (selectedApplicationIDs.length() > 0)
			selectedApplicationIDs = selectedApplicationIDs.substring(0,
					selectedApplicationIDs.length() - 1);

		for (QueryClients client : selectedClients) {
			selectedClientIDs += client.getClientId() + "|";
		}
		if (selectedClientIDs.length() > 0)
			selectedClientIDs = selectedClientIDs.substring(0,
					selectedClientIDs.length() - 1);

		for (QueryDatabases db : selectedDatabases) {
			selectedDBIDs += db.getDatabaseId() + "|";
		}
		if (selectedDBIDs.length() > 0)
			selectedDBIDs = selectedDBIDs.substring(0,
					selectedDBIDs.length() - 1);

		for (QueryUsers user : selectedUsers) {
			selectedUserIDs += user.getUserId() + "|";
		}
		if (selectedUserIDs.length() > 0)
			selectedUserIDs = selectedUserIDs.substring(0,
					selectedUserIDs.length() - 1);

		selectedAdvancedFilters += filter.isShowSQLStatements() ? "1" : "0";
		selectedAdvancedFilters += filter.isShowStoredProcs() ? "1" : "0";
		selectedAdvancedFilters += filter.isShowSQLBatches() ? "1" : "0";
		selectedAdvancedFilters += filter.isIncludeIncomplete() ? "1" : "0";
		selectedAdvancedFilters += filter.isIncludeOverlapping() ? "1" : "0";

		String selectedGroupName = filter.getSelectedGroupBy().getGroupName();
		int selectedGroupId = filter.getSelectedGroupBy().getGroupId();
		try {

			SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
					.getInstance().getQueriesPreferenceInSession(instanceId);

			if (sessionBean.getQueryGroupOptions() == null) {
				sessionBean.setQueryGroupOptions(QueryGroupsFacade
						.getQueryGroups(productInstanceName));
				PreferencesUtil.getInstance().setQueriesPreferenceInSession(
						sessionBean);
			}

			if (((selectedGroupBy.equals("Application")
					|| selectedGroupBy.equals("User")
					|| selectedGroupBy.equals("Database") || selectedGroupBy
						.equals("Client")) && (drillDown == 1))
					|| ((selectedGroupBy.equals("Query Signature")) && (drillDown != 2))) {

				for (QueryGroups group : sessionBean.getQueryGroupOptions()) {
					if (group.getGroupName()
							.equalsIgnoreCase("Query Signature")) {
						selectedGroupId = group.getGroupId();
						selectedGroupName = group.getGroupName();
					}
				}

			} else if ((selectedGroupBy.equals("Query Signature"))
					&& (drillDown == 2)) {

				for (QueryGroups group : sessionBean.getQueryGroupOptions()) {
					if (group.getGroupName()
							.equalsIgnoreCase("Query Statement")) {
						selectedGroupId = group.getGroupId();
						selectedGroupName = group.getGroupName();
					}
				}
			} else if (!selectedGroupBy.equals("Query Signature")
					&& drillDown == 0) {
				selectedQuerySignatureId = -1;
				selectedEventId = -1;
			}

			graphBars = QueryGraphFacade.getQueryGraph(productInstanceName,
					instanceId, filter.getSelectedView().getMetricId(),
					selectedGroupId, Double.parseDouble(offsetValue),
					selectedApplicationIDs, selectedDBIDs, selectedUserIDs,
					selectedClientIDs, filter.getIncludeSQLText(),
					filter.getExcludeSQLText(), selectedAdvancedFilters,
					startDateTime, endDateTime, selectedQuerySignatureId,
					selectedEventId);
		} catch (RestException e) {
			e.printStackTrace();
		}

		SimpleDateFormat sdf1 = new SimpleDateFormat("dd-MM-yyyy HH:mm:ss");
		SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
		List<Date> buckets = new ArrayList<>();
		Date startTime = null;
		Date endTime = null;
		try {
			startTime = sdf.parse(startDateTime);
			endTime = sdf.parse(endDateTime);
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		String bucketType;
		if (graphBars != null && !graphBars.isEmpty()) {
			bucketType = graphBars.get(0).getBucketType();
			while (endTime.compareTo(startTime) == 1
					|| endTime.compareTo(startTime) == 0) {
				buckets.add(startTime);
				if (graphBars.get(0).getBucketType().equals("Hours"))
					startTime = DateUtils.addHours(startTime, 1);
				if (graphBars.get(0).getBucketType().equals("Minutes"))
					startTime = DateUtils.addMinutes(startTime, 1);
				if (graphBars.get(0).getBucketType().equals("Months"))
					startTime = DateUtils.addDays(startTime, 30);
				if (graphBars.get(0).getBucketType().equals("Days"))
					startTime = DateUtils.addDays(startTime, 1);
			}
			buckets.add(endTime);

			Set<String> groupByNames = new HashSet<>();
			Set<String> groupbyNamesQS = new HashSet<>();
			for (Date d : buckets) {
				testData = new LinkedHashMap<String, Number>();
				Date startDate = null;
				Date endDate = null;
				for (QueryGraph record : graphBars) {

					groupByNames.add(record.getGroupByName());
					if (record.getGroupByName().equalsIgnoreCase("Other"))
						groupbyNamesQS.add(record.getGroupByName());
					else if (selectedGroupName.equals("Query Signature")) {
						groupbyNamesQS.add(queryIds.get(record.getGroupByID()));

					} else if (selectedGroupName.equals("Query Statement")) {

						groupbyNamesQS.add(queryIds.get(record.getGroupByID()));

					}

					Date startDateRecord = null;
					Date endDateRecord = null;
					int index = buckets.indexOf(d);
					if (index + 1 < buckets.size()) {
						startDate = d;
						endDate = buckets.get(index + 1);

						try {
							startDateRecord = sdf1.parse(record
									.getBucketStartDateTime());
							endDateRecord = sdf1.parse(record
									.getBucketEndDateTime());
						} catch (ParseException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}

						if ((startDateRecord.compareTo(startDate) > 0 || startDateRecord
								.toString().equalsIgnoreCase(
										startDate.toString()))
								&& (endDateRecord.compareTo(endDate) < 0 || endDateRecord
										.toString().equalsIgnoreCase(
												endDate.toString()))) {
							if (record.getGroupByName().equalsIgnoreCase(
									"Other"))
								testData.put(record.getGroupByName(), Double
										.parseDouble(record.getYAxisValue()));
							else if (selectedGroupName
									.equals("Query Signature")) {

								testData.put(
										queryIds.get(record.getGroupByID()),
										Double.parseDouble(record
												.getYAxisValue()));

							} else if (selectedGroupName
									.equals("Query Statement")) {

								testData.put(
										queryIds.get(record.getGroupByID()),
										Double.parseDouble(record
												.getYAxisValue()));

							} else
								testData.put(record.getGroupByName(), Double
										.parseDouble(record.getYAxisValue()));
						}
					}
				}

				if (testData != null && !testData.isEmpty()) {
					if (selectedGroupName.equals("Query Signature")) {
						for (String groupName : groupbyNamesQS) {
							if (!testData.containsKey(groupName))
								testData.put(groupName, 0);
						}
					} else if (selectedGroupName.equals("Query Statement")) {
						for (String groupName : groupbyNamesQS) {
							if (!testData.containsKey(groupName))
								testData.put(groupName, 0);
						}
					} else {
						for (String groupName : groupByNames) {
							if (!testData.containsKey(groupName))
								testData.put(groupName, 0);
						}
					}

					StringTokenizer startToken = new StringTokenizer(
							startDate.toString());
					StringTokenizer endToken = new StringTokenizer(
							endDate.toString());
					String xAxisDayStart = startToken.nextElement().toString();
					String xAxisDayEnd = endToken.nextElement().toString();
					String xAxisMonthStart = startToken.nextElement()
							.toString();
					String xAxisMonthEnd = endToken.nextElement().toString();
					String xAxisDateStart = startToken.nextElement().toString();
					String xAxisDateEnd = endToken.nextElement().toString();
					String xAxisTimeStart = startToken.nextToken(":")
							.toString()
							+ ":"
							+ startToken.nextToken(":").toString();
					String xAxisTimeEnd = endToken.nextToken(":").toString()
							+ ":" + endToken.nextToken(":").toString();

					if (bucketType.equals("Minutes")) {
						chartData.put(xAxisTimeStart + "-" + xAxisTimeEnd,
								testData);
						axisLinks.put(xAxisTimeStart + "-" + xAxisTimeEnd,
								"window.drillTime(\"" + startDate.toString()
										+ "\",\"" + endDate.toString() + "\")");
					} else if (bucketType.equals("Hours")) {
						chartData.put(xAxisTimeStart + "-" + xAxisTimeEnd,
								testData);
						axisLinks.put(xAxisTimeStart + "-" + xAxisTimeEnd,
								"window.drillTime(\"" + startDate.toString()
										+ "\",\"" + endDate.toString() + "\")");
					} else if (bucketType.equals("Days")) {
						chartData.put(xAxisDayStart + " " + xAxisDateStart
								+ "-" + xAxisDayEnd + " " + xAxisDateEnd,
								testData);
						axisLinks.put(xAxisDayStart + " " + xAxisDateStart
								+ "-" + xAxisDayEnd + " " + xAxisDateEnd,
								"window.drillTime(\"" + startDate.toString()
										+ "\",\"" + endDate.toString() + "\")");
					} else if (bucketType.equals("Months")) {
						chartData.put(xAxisDateStart + " " + xAxisMonthStart
								+ "-" + xAxisDateEnd + " " + xAxisMonthEnd,
								testData);
						axisLinks.put(xAxisDateStart + " " + xAxisMonthStart
								+ "-" + xAxisDateEnd + " " + xAxisMonthEnd,
								"window.drillTime(\"" + startDate.toString()
										+ "\",\"" + endDate.toString() + "\")");
					}
				}

			}
		}

		Map<String, String> legends = new LinkedHashMap<String, String>();

		for (QueryGraph record : graphBars) {
			legends.put(record.getGroupByID(), record.getGroupByName());
		}

		for (String leg : legends.keySet()) {
			if (selectedGroupName.equals("Query Signature")) {

				legendLinks.put(queryIds.get(leg), "window.drillGraph(\"" + leg
						+ "\")");

			} else if (selectedGroupName.equals("Query Statement")) {

				legendLinks.put(queryIds.get(leg), "window.drillGraph(\"" + leg
						+ "\")");

			} else {

				if (!legends.get(leg).equalsIgnoreCase("Other")) {
					legendLinks.put(legends.get(leg), "window.drillGraph(\""
							+ leg + "\")");
				}

			}
		}
		if ((chartData != null) && (!chartData.isEmpty())) {

			this.queryChart.setOrient("vertical");
			this.queryChart.setChartData(chartData);
			this.queryChart.setVisible(true);
			this.queryChart.getChart().setMouseOverText(
					filter.getSelectedView().getMetricName() + ": %s");

			// Inner padding adds space between bars
			this.queryChart.getChart().setInnerPadding(0.2);
			// Outer padding adds padding on the outer ticks
			this.queryChart.getChart().setOuterPadding(0.2);

			this.queryChart.getChart().setLegendLinks(legendLinks);

			this.queryChart.getChart().setAxisLinks(axisLinks);
			this.queryChart.getChart().setAnimateBars(true);
			this.queryChart.getChart().setAnimationDelay(100);
			this.queryChart.getChart().setAnimationLength(500);

		} else {

			setEmptyBarCharts();
		}

	}

	/**
	 * Changes the selected filters when filter is removed
	 * 
	 * @param filterType
	 * @param index
	 */
	@NotifyChange
	@Command("filterRemoved")
	public void filterRemoved(@BindingParam("filterType") String filterType,
			@BindingParam("unchecked") int index) {

		switch (filterType) {
		case "Application":
			selectedApplications.remove(index);
			break;

		case "Database":
			selectedDatabases.remove(index);
			break;

		case "Client":
			selectedClients.remove(index);
			break;

		case "User":
			selectedUsers.remove(index);
			break;

		case "IncludeText":
			filter.setIncludeSQLText("");
			break;

		case "ExcludeText":
			filter.setExcludeSQLText("");
			break;

		case "ShowSQLStatements":
			filter.setShowSQLStatements(false);
			break;

		case "ShowStoredProcs":
			filter.setShowStoredProcs(false);
			break;

		case "ShowSQLBatches":
			filter.setShowSQLBatches(false);
			break;

		case "IncludeOverlappingQueries":
			filter.setIncludeOverlapping(false);
			break;

		case "IncludeIncomplete":
			filter.setIncludeIncomplete(false);
			break;

		}

		updateSession();

		EventQueue<Event> eq = EventQueues.lookup("changeQueryFilter",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("filterRemoved"));
		}

		BindUtils.postNotifyChange(null, null, this, "selectedApplications");
		BindUtils.postNotifyChange(null, null, this, "selectedDatabases");
		BindUtils.postNotifyChange(null, null, this, "selectedUsers");
		BindUtils.postNotifyChange(null, null, this, "selectedClients");
		BindUtils.postNotifyChange(null, null, this, "filter");

		// Rest API Call
		updateModelRest(productInstanceName);
		drawChart();

	}

	/**
	 * Updates the 'All' filter flag
	 * 
	 * @param filterType
	 * @param index
	 */
	@NotifyChange
	@Command("allFilterRemoved")
	public void allFilterRemoved(@BindingParam("filterType") String filterType) {

		switch (filterType) {
		case "Application":
			filter.setApplicationAllChecked(false);
			break;

		case "Database":
			filter.setDatabasesAllChecked(false);
			break;

		case "Client":
			filter.setClientsAllChecked(false);
			break;

		case "User":
			filter.setUsersAllChecked(false);
			break;

		}

		updateSession();

		EventQueue<Event> eq = EventQueues.lookup("changeQueryFilter",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("filterRemoved"));
		}

		BindUtils.postNotifyChange(null, null, this, "filter");

	}

	/**
	 * Changing the title of queries list according to the selected group by
	 */
	@NotifyChange
	public void setTableTitle() {
		if (filter != null)
			selectedGroupBy = filter.getSelectedGroupBy().getGroupName();

		if (drillDown == 0) {
			switch (selectedGroupBy) {
			case "Application":
				queriesWaitsLabel = ELFunctions
						.getLabel(SQLdmI18NStrings.APPLICATION);
				queriesWaitsTitle = queriesWaitsLabel.toUpperCase() + " (?)";
				break;

			case "Database":
				queriesWaitsLabel = ELFunctions
						.getLabel(SQLdmI18NStrings.DATABASE);
				break;

			case "User":
				queriesWaitsLabel = ELFunctions
						.getLabel(SQLdmI18NStrings.FILTER_USERS);
				break;

			case "Client":
				queriesWaitsLabel = ELFunctions
						.getLabel(SQLdmI18NStrings.FILTER_CLIENTS);
				break;

			case "Query Statement":
				queriesWaitsLabel = ELFunctions
						.getLabel(SQLdmI18NStrings.QUERY_STATEMENT);
				break;

			case "Query Signature":
				queriesWaitsLabel = ELFunctions
						.getLabel(SQLdmI18NStrings.QUERY_SIGNATURE);
				break;

			}
		} else if (drillDown == 1) {
			queriesWaitsLabel = ELFunctions
					.getLabel(SQLdmI18NStrings.QUERY_SIGNATURE);
		} else if (drillDown == 2) {
			queriesWaitsLabel = ELFunctions
					.getLabel(SQLdmI18NStrings.QUERY_STATEMENT);
		}

		queriesWaitsTitle = queriesWaitsLabel.toUpperCase() + " (?)";
		BindUtils.postNotifyChange(null, null, this, "queriesWaitTitle");

	}

	public void setQueriesModel(List<QueryApplicationDetails> queriesList) {
		this.queriesModel = new ListModelList<QueryApplicationDetails>(
				queriesList);
		if (queriesList != null) {
			this.queriesWaitsTitle = queriesWaitsLabel.toUpperCase() + " ("
					+ new Integer(queriesList.size()).toString() + ")";
		} else {
			this.queriesWaitsTitle = queriesWaitsLabel.toUpperCase();
		}

		BindUtils.postNotifyChange(null, null, this, "queriesModel");
		BindUtils.postNotifyChange(null, null, this, "queriesWaitsTitle");

	}

	public void setQueriesListRowsCount(int queriesListRowsCount) {
		this.queriesListRowsCount = queriesListRowsCount;
		BindUtils.postNotifyChange(null, null, this, "queriesListRowsCount");
	}

	/**
	 * * @param listRowsCount
	 */
	public void setListRowsCount(int listRowsCount) {
		this.listRowsCount = listRowsCount;
		BindUtils.postNotifyChange(null, null, this, "listRowsCount");
	}

	/**
	 * Updates the session according to the local values
	 */
	private void updateSession() {
		SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueriesPreferenceInSession(instanceId);
		sessionBean.setPageCount(listRowsCount);
		sessionBean.setQueryFilter(filter);
		sessionBean.setCheckedApplications(selectedApplications);
		sessionBean.setCheckedDatabases(selectedDatabases);
		sessionBean.setCheckedClients(selectedClients);
		sessionBean.setCheckedUsers(selectedUsers);
		sessionBean.setQueryIds(queryIds);
		PreferencesUtil.getInstance()
				.setQueriesPreferenceInSession(sessionBean);
	}

	public boolean isDirect() {
		return direct;
	}

	public void setDirect(boolean direct) {
		this.direct = direct;
	}

	@Command("onClickGear")
	public void setQuery(@BindingParam("vref") QueryApplicationDetails query) {
		queryRow = query;
	}

	/**
	 * Set the rows count for the grid
	 * 
	 * @param queriesListPgId
	 */
	@Command("setQueriesRowsCount")
	public void setQueriesRowsCount(@BindingParam("ref") Paging queriesListPgId) {
		setListRowsCount(this.queriesListRowsCount);
		log.debug("setting queries pageCount as " + this.queriesListRowsCount);
		updateSession();
		queriesListPgId.setPageSize(listRowsCount);
		queriesListPgId.setTotalSize(queriesDetails.size());
		queriesWaitGrid.setPaginal(queriesListPgId);
		BindUtils.postNotifyChange(null, null, this, "queriesWaitGrid");
		BindUtils.postNotifyChange(null, null, this, "queriesListPgId");
	}

	public boolean isDrillUp() {
		return drillUp;
	}

	public void setDrillUp(boolean drillUp) {
		this.drillUp = drillUp;
	}

	/**
	 * publish the event when filter options are changed
	 */
	private void publishChangeFilterEvent(boolean flag) {
		EventQueue<Event> eq = EventQueues.lookup("changeQueryFilter",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("filterRemoved", null, flag));
			eq.publish(new Event("filterChanged"));

		}
	}

	@Command("openModalWindow")
	public void openModalWindow(@BindingParam("flag") boolean visibilityFlag,
			@BindingParam("default") int defaultTab) {

		HashMap<String, Object> map = new HashMap<>();
		map.put("VisibilityFlag",
				String.valueOf(visibilityFlag && getIsSQLDB2008OrGreater()));
		map.put("DefaultTab", String.valueOf(defaultTab));

		if (visibilityFlag == true) {
			try {
				List<QueryPlan> list = QueryPlanFacade.getQueryPlan(
						productInstanceName, instanceId,
						queryRow.getQueryStatisticsID());
				if (list.size() > 0) {

					QueryPlan queryPlan = list.get(0);
					map.put("IsActualFlag", queryPlan.isActual());
					map.put("XmlPlanString", queryPlan.getPlanMXL());
					map.put("QueryStatementColumns",
							queryPlan.getQueryColumns());

				} else {

					map.put("XmlPlanString", null);

				}
				map.put("SqlText", queryRow.getSqlText());
			} catch (RestException e) {
				e.printStackTrace();
			}
		} else {
			map.put("SqlText", queryRow.getSignatureSQLText());
		}

		SessionUtil.getSecurityContext();
		Window window = (Window) Executions
				.createComponents(
						"~./sqldm/com/idera/sqldm/ui/dashboard/instances/queries/queryDetailsWindow.zul",
						null, map);
		window.doModal();
	}

	/**
	 * method called on drill down
	 * 
	 * @param query
	 */
	@Command("onClickSelectedRow")
	public void onClickSelectedRow(
			@BindingParam("row") QueryApplicationDetails obj) {
		if (obj != null)
			queryRow = obj;

		SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueriesPreferenceInSession(instanceId);

		if (sessionBean.getQueryGroupOptions() == null) {
			Messagebox.show(ELFunctions.getMessage(SQLdmI18NStrings.ERROR_MESSAGE_QUERIES));
			return;
		}

		for (QueryGroups group : sessionBean.getQueryGroupOptions()) {
			if (group.getGroupName().equalsIgnoreCase("Query Signature")) {
				queriesGroupBy1 = group;
			}
		}

		if (filter != null)
			selectedGroupBy = filter.getSelectedGroupBy().getGroupName();
		sessionBean.setSet(0);
		drillUp = true;

		if ((selectedGroupBy.equals("Application")
				|| selectedGroupBy.equals("User")
				|| selectedGroupBy.equals("Database") || selectedGroupBy
					.equals("Client")) && (drillDown == 0)) {

			sessionBean.setDefaultSession(PreferencesUtil.getInstance()
					.getQueriesPreferenceInSession(instanceId));
			sessionBean.setDrill(1);
			drillDown = 1;

			switch (selectedGroupBy) {
			case "Application":
				List<QueryApplication> selectedApp = new ArrayList<>();
				selectedApp.add(queryRow.getApplication());
				selectedApplications = selectedApp;
				break;

			case "Client":
				List<QueryClients> selectedClient = new ArrayList<>();
				selectedClient.add(queryRow.getClient());
				selectedClients = selectedClient;
				break;

			case "Database":
				List<QueryDatabases> selectedDB = new ArrayList<>();
				selectedDB.add(queryRow.getDatabase());
				selectedDatabases = selectedDB;
				break;

			case "User":
				List<QueryUsers> selectedUser = new ArrayList<>();
				selectedUser.add(queryRow.getUser());
				selectedUsers = selectedUser;
				break;
			}

			filter.setDrillDown(true);
			setListRowsCount(defaultQueriesRowCount);
			updateSession();
			updateModelRest(productInstanceName);
			publishEvents();
		} else if (((selectedGroupBy.equals("Application")
				|| selectedGroupBy.equals("User")
				|| selectedGroupBy.equals("Database") || selectedGroupBy
					.equals("Client")) && (drillDown == 1))
				|| ((selectedGroupBy.equals("Query Signature")) && (drillDown != 2))) {

			filter.setSelectedGroupBy(queriesGroupBy1);
			selectedQuerySignatureId = queryRow.getSqlSignatureID();
			selectedEventId = queryRow.getEventId();
			sessionBean.setQuerySignatureSession(sessionBean);
			sessionBean.getQuerySignatureSession().setDefaultSession(
					sessionBean.getDefaultSession());
			sessionBean.getQuerySignatureSession().setQueryFilter(filter);
			sessionBean.setDrill(2);
			drillDown = 2;
			filter.setDrillDown(true);
			setListRowsCount(defaultQueriesRowCount);
			updateSession();
			updateModelRest(productInstanceName);
			publishEvents();
		} else if ((selectedGroupBy.equals("Query Signature"))
				&& (drillDown == 2)) {

			openModalWindow(true, 0);
		}

	}

	private void publishEvents() {
		BindUtils.postNotifyChange(null, null, this, "queriesDetails");
		BindUtils.postNotifyChange(null, null, this, "selectedApplications");
		BindUtils.postNotifyChange(null, null, this, "selectedDatabases");
		BindUtils.postNotifyChange(null, null, this, "selectedUsers");
		BindUtils.postNotifyChange(null, null, this, "selectedClients");
		BindUtils.postNotifyChange(null, null, this, "listRowsCount");
		BindUtils.postNotifyChange(null, null, this, "drillDown");

		publishChangeFilterEvent(true);
	}

	@Command("onDirectJump")
	public void onDirectJump() {

		onClickSelectedRow(null);
		onClickSelectedRow(null);

	}

	/**
	 * Method called on clicking on Drill Up icon
	 */
	@Command("onDrillUp")
	public void onClickDrillUp() {

		SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueriesPreferenceInSession(instanceId);

		if ((selectedGroupBy.equals("Query Signature"))
				&& sessionBean.getSet() == 0
				&& sessionBean.getQuerySignatureSession() != null) {

			sessionBean = sessionBean.getQuerySignatureSession();

			if (sessionBean.getDefaultSession() != null) {
				sessionBean.getQueryFilter().setSelectedGroupBy(
						sessionBean.getDefaultSession().getQueryFilter()
								.getSelectedGroupBy());
			}

			PreferencesUtil.getInstance().setQueriesPreferenceInSession(
					sessionBean);
			sessionBean.setDrill(1);

			sessionBean.setQuerySignatureSession(null);
			if (sessionBean.getDefaultSession() == null)
				drillUp = false;
			else
				drillUp = true;

		} else if (sessionBean.getSet() == 0
				&& sessionBean.getDefaultSession() != null) {
			// sessionBean.getQuerySignatureSession().setDefaultSession(sessionBean.getDefaultSession());
			sessionBean = sessionBean.getDefaultSession();
			PreferencesUtil.getInstance().setQueriesPreferenceInSession(
					sessionBean);
			sessionBean.setDrill(0);
			sessionBean.setDefaultSession(null);
			drillUp = false;

		}

		selectedQuerySignatureId = -1;
		selectedEventId = -1;
		BindUtils.postNotifyChange(null, null, this, "drillUp");
		publishChangeFilterEvent(true);
	}

	private void setOffSet() {
		Double offSet = null;
		if (Sessions.getCurrent() != null) {
			offSet = new Double((Integer) Sessions.getCurrent().getAttribute(
					WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))
					/ (1000 * 60.0 * 60.0);
			offSet = -offSet;
		}
		if (offSet != null)
			offsetValue = offSet.toString();
	}
	/**
	 * Adding Columns to columns list and setting its properties
	 */
	public void addColumnsToList() {
		// 0 - Does not Exist
		// 1 - Default
		// 2 - Available
		// Application Column
		columns[0].setValue("application");
		// columns[0].setCompareColumn("Application");
		columns[0].setTemplate("application");
		columns[0].setWidth("120px");
		columns[0].groupByFilters[0] = 1;
		columns[0].groupByFilters[1] = 2;
		columns[0].groupByFilters[2] = 2;
		columns[0].groupByFilters[3] = 2;
		columns[0].groupByFilters[4] = 2;
		columns[0].groupByFilters[5] = 2;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.APPLICATION),
				columns[0]);

		// Occurrences Column
		columns[1].setValue("occurrences");
		columns[1].setWidth("100px");
		columns[1].groupByFilters[0] = 1;
		columns[1].groupByFilters[1] = 1;
		columns[1].groupByFilters[2] = 1;
		columns[1].groupByFilters[3] = 1;
		columns[1].groupByFilters[4] = 1;
		columns[1].groupByFilters[5] = 1;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.OCCURRENCES),
				columns[1]);

		// Total Duration
		columns[2].setValue("totalDuration");
		columns[2].setWidth("90px");
		columns[2].setTemplate("numberLabel");
		columns[2].groupByFilters[0] = 1;
		columns[2].groupByFilters[1] = 1;
		columns[2].groupByFilters[2] = 1;
		columns[2].groupByFilters[3] = 1;
		columns[2].groupByFilters[4] = 1;
		columns[2].groupByFilters[5] = 1;
		queryColumns.put(
				ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_DURATION),
				columns[2]);

		// Avg Duration
		columns[3].setValue("avgDuration");
		columns[3].setWidth("90px");
		columns[3].setTemplate("numberLabel");
		columns[3].groupByFilters[0] = 1;
		columns[3].groupByFilters[1] = 1;
		columns[3].groupByFilters[2] = 2;
		columns[3].groupByFilters[3] = 1;
		columns[3].groupByFilters[4] = 2;
		columns[3].groupByFilters[5] = 0;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.AVG_DURATION),
				columns[3]);

		// Total CPU Time
		columns[4].setValue("totalCPUTime");
		columns[4].setWidth("100px");
		columns[4].setTemplate("numberLabel");
		columns[4].groupByFilters[0] = 1;
		columns[4].groupByFilters[1] = 1;
		columns[4].groupByFilters[2] = 1;
		columns[4].groupByFilters[3] = 1;
		columns[4].groupByFilters[4] = 1;
		columns[4].groupByFilters[5] = 1;
		queryColumns.put(
				ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_CPU_TIME),
				columns[4]);

		// Total Reads
		columns[5].setValue("totalReads");
		columns[5].setWidth("100px");
		columns[5].setTemplate("numberLabel");
		columns[5].groupByFilters[0] = 1;
		columns[5].groupByFilters[1] = 1;
		columns[5].groupByFilters[2] = 1;
		columns[5].groupByFilters[3] = 1;
		columns[5].groupByFilters[4] = 1;
		columns[5].groupByFilters[5] = 1;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_READS),
				columns[5]);

		// Total Writes
		columns[6].setValue("totalWrites");
		columns[6].setWidth("100px");
		columns[6].setTemplate("numberLabel");
		columns[6].groupByFilters[0] = 1;
		columns[6].groupByFilters[1] = 1;
		columns[6].groupByFilters[2] = 1;
		columns[6].groupByFilters[3] = 1;
		columns[6].groupByFilters[4] = 1;
		columns[6].groupByFilters[5] = 1;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_WRITES),
				columns[6]);

		// Total IO
		columns[7].setValue("totalIO");
		columns[7].setWidth("90px");
		columns[7].setTemplate("numberLabel");
		columns[7].groupByFilters[0] = 1;
		columns[7].groupByFilters[1] = 1;
		columns[7].groupByFilters[2] = 0;
		columns[7].groupByFilters[3] = 0;
		columns[7].groupByFilters[4] = 0;
		columns[7].groupByFilters[5] = 0;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_IO),
				columns[7]);

		// Total Wait Time
		columns[8].setValue("totalWaitTime");
		columns[8].setWidth("100px");
		columns[8].setTemplate("numberLabel");
		columns[8].groupByFilters[0] = 1;
		columns[8].groupByFilters[1] = 2;
		columns[8].groupByFilters[2] = 2;
		columns[8].groupByFilters[3] = 2;
		columns[8].groupByFilters[4] = 2;
		columns[8].groupByFilters[5] = 1;
		queryColumns.put(
				ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_WAIT_TIME),
				columns[8]);

		// Most Recent Completion
		columns[9].setValue("mostRecentCompletion");
		columns[9].setWidth("120px");
		// columns[9].setTemplate("dateTime");
		columns[9].groupByFilters[0] = 1;
		columns[9].groupByFilters[1] = 1;
		columns[9].groupByFilters[2] = 1;
		columns[9].groupByFilters[3] = 1;
		columns[9].groupByFilters[4] = 1;
		columns[9].groupByFilters[5] = 1;
		queryColumns
				.put(ELFunctions
						.getMessage(SQLdmI18NStrings.MOST_RECENT_COMPLETION),
						columns[9]);

		// Total Blocking Time
		columns[10].setValue("totalBlockingTime");
		columns[10].setWidth("100px");
		columns[10].setTemplate("numberLabel");
		columns[10].groupByFilters[0] = 1;
		columns[10].groupByFilters[1] = 2;
		columns[10].groupByFilters[2] = 2;
		columns[10].groupByFilters[3] = 2;
		columns[10].groupByFilters[4] = 2;
		columns[10].groupByFilters[5] = 1;
		queryColumns.put(
				ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_BLOCKING_TIME),
				columns[10]);

		// Total Deadlocks
		columns[11].setValue("totalDeadlocks");
		columns[11].setWidth("100px");
		columns[11].setTemplate("numberLabel");
		columns[11].groupByFilters[0] = 1;
		columns[11].groupByFilters[1] = 2;
		columns[11].groupByFilters[2] = 2;
		columns[11].groupByFilters[3] = 2;
		columns[11].groupByFilters[4] = 2;
		columns[11].groupByFilters[5] = 1;
		queryColumns.put(
				ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_DEADLOCKS),
				columns[11]);

		// Avg CPU Time
		columns[12].setValue("avgCPUTime");
		columns[12].setWidth("100px");
		columns[12].setTemplate("numberLabel");
		columns[12].groupByFilters[0] = 2;
		columns[12].groupByFilters[1] = 2;
		columns[12].groupByFilters[2] = 2;
		columns[12].groupByFilters[3] = 2;
		columns[12].groupByFilters[4] = 2;
		columns[12].groupByFilters[5] = 0;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.AVG_CPU_TIME),
				columns[12]);

		// Avg CPU per Sec
		columns[13].setValue("avgCPUPerSec");
		columns[13].setWidth("100px");
		columns[13].setTemplate("numberLabel");
		columns[13].groupByFilters[0] = 2;
		columns[13].groupByFilters[1] = 2;
		columns[13].groupByFilters[2] = 2;
		columns[13].groupByFilters[3] = 2;
		columns[13].groupByFilters[4] = 2;
		columns[13].groupByFilters[5] = 2;
		queryColumns.put(
				ELFunctions.getMessage(SQLdmI18NStrings.AVG_CPU_PER_SEC),
				columns[13]);

		// Avg Reads
		columns[14].setValue("avgReads");
		columns[14].setWidth("100px");
		columns[14].setTemplate("numberLabel");
		columns[14].groupByFilters[0] = 2;
		columns[14].groupByFilters[1] = 2;
		columns[14].groupByFilters[2] = 2;
		columns[14].groupByFilters[3] = 2;
		columns[14].groupByFilters[4] = 2;
		columns[14].groupByFilters[5] = 0;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.AVG_READS),
				columns[14]);

		// Avg Writes
		columns[15].setValue("avgWrites");
		columns[15].setWidth("100px");
		columns[15].setTemplate("numberLabel");
		columns[15].groupByFilters[0] = 2;
		columns[15].groupByFilters[1] = 2;
		columns[15].groupByFilters[2] = 2;
		columns[15].groupByFilters[3] = 2;
		columns[15].groupByFilters[4] = 2;
		columns[15].groupByFilters[5] = 0;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.AVG_WRITES),
				columns[15]);

		// Avg IO
		columns[16].setValue("avgIO");
		columns[16].setWidth("100px");
		columns[16].setTemplate("numberLabel");
		columns[16].groupByFilters[0] = 2;
		columns[16].groupByFilters[1] = 2;
		columns[16].groupByFilters[2] = 0;
		columns[16].groupByFilters[3] = 0;
		columns[16].groupByFilters[4] = 0;
		columns[16].groupByFilters[5] = 0;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.AVG_IO),
				columns[16]);

		// Avg Wait Time
		columns[17].setValue("avgWaitTime");
		columns[17].setWidth("100px");
		columns[17].setTemplate("numberLabel");
		columns[17].groupByFilters[0] = 2;
		columns[17].groupByFilters[1] = 1;
		columns[17].groupByFilters[2] = 1;
		columns[17].groupByFilters[3] = 1;
		columns[17].groupByFilters[4] = 1;
		columns[17].groupByFilters[5] = 0;
		queryColumns.put(
				ELFunctions.getMessage(SQLdmI18NStrings.AVG_WAIT_TIME),
				columns[17]);

		// Avg Blocking Time
		columns[18].setValue("avgBlockingTime");
		columns[18].setWidth("100px");
		columns[18].setTemplate("numberLabel");
		columns[18].groupByFilters[0] = 2;
		columns[18].groupByFilters[1] = 1;
		columns[18].groupByFilters[2] = 1;
		columns[18].groupByFilters[3] = 1;
		columns[18].groupByFilters[4] = 1;
		columns[18].groupByFilters[5] = 0;
		queryColumns.put(
				ELFunctions.getMessage(SQLdmI18NStrings.AVG_BLOCKING_TIME),
				columns[18]);

		// Avg Deadlocks
		columns[19].setValue("avgDeadlocks");
		columns[19].setWidth("100px");
		columns[19].setTemplate("numberLabel");
		columns[19].groupByFilters[0] = 2;
		columns[19].groupByFilters[1] = 1;
		columns[19].groupByFilters[2] = 1;
		columns[19].groupByFilters[3] = 1;
		columns[19].groupByFilters[4] = 1;
		columns[19].groupByFilters[5] = 0;
		queryColumns.put(
				ELFunctions.getMessage(SQLdmI18NStrings.AVG_DEADLOCKS),
				columns[19]);

		// CPU as % of List
		columns[20].setValue("cpuAsPercentOfList");
		columns[20].setWidth("100px");
		columns[20].setTemplate("numberLabel");
		columns[20].groupByFilters[0] = 2;
		columns[20].groupByFilters[1] = 2;
		columns[20].groupByFilters[2] = 2;
		columns[20].groupByFilters[3] = 2;
		columns[20].groupByFilters[4] = 2;
		columns[20].groupByFilters[5] = 0;
		queryColumns
				.put(ELFunctions
						.getMessage(SQLdmI18NStrings.CPU_AS_PERCENTAGE_LIST),
						columns[20]);

		// Reads as % of List
		columns[21].setValue("readsAsPercentOfList");
		columns[21].setWidth("100px");
		columns[21].setTemplate("numberLabel");
		columns[21].groupByFilters[0] = 2;
		columns[21].groupByFilters[1] = 2;
		columns[21].groupByFilters[2] = 2;
		columns[21].groupByFilters[3] = 2;
		columns[21].groupByFilters[4] = 2;
		columns[21].groupByFilters[5] = 0;
		queryColumns.put(ELFunctions
				.getMessage(SQLdmI18NStrings.READS_AS_PERCENTAGE_LIST),
				columns[21]);

		// Databases
		columns[22].setValue("database");
		// columns[22].setCompareColumn("Database");
		columns[22].setTemplate("database");
		columns[22].setWidth("120px");
		columns[22].groupByFilters[0] = 2;
		columns[22].groupByFilters[1] = 1;
		columns[22].groupByFilters[2] = 2;
		columns[22].groupByFilters[3] = 2;
		columns[22].groupByFilters[4] = 2;
		columns[22].groupByFilters[5] = 2;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.DATABASE),
				columns[22]);

		// User
		columns[23].setValue("user");
		columns[23].setTemplate("user");
		// columns[23].setCompareColumn("User");
		columns[23].setWidth("100px");
		columns[23].groupByFilters[0] = 0;
		columns[23].groupByFilters[1] = 0;
		columns[23].groupByFilters[2] = 1;
		columns[23].groupByFilters[3] = 0;
		columns[23].groupByFilters[4] = 0;
		columns[23].groupByFilters[5] = 2;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.FILTER_USERS),
				columns[23]);

		// Clients
		columns[24].setValue("client");
		// columns[24].setCompareColumn("Client");
		columns[24].setTemplate("client");
		columns[24].setWidth("100px");
		columns[24].groupByFilters[0] = 0;
		columns[24].groupByFilters[1] = 0;
		columns[24].groupByFilters[2] = 0;
		columns[24].groupByFilters[3] = 1;
		columns[24].groupByFilters[4] = 0;
		columns[24].groupByFilters[5] = 2;
		queryColumns.put(
				ELFunctions.getMessage(SQLdmI18NStrings.FILTER_CLIENTS),
				columns[24]);

		// Avg IO per Sec
		columns[25].setValue("avgIOPerSec");
		columns[25].setWidth("100px");
		columns[25].setTemplate("numberLabel");
		columns[25].groupByFilters[0] = 0;
		columns[25].groupByFilters[1] = 0;
		columns[25].groupByFilters[2] = 2;
		columns[25].groupByFilters[3] = 2;
		columns[25].groupByFilters[4] = 2;
		columns[25].groupByFilters[5] = 0;
		queryColumns.put(
				ELFunctions.getMessage(SQLdmI18NStrings.AVG_IO_PER_SEC),
				columns[25]);

		// Signature SQL Text
		columns[26].setValue("signatureSQLText");
		columns[26].setWidth("180px");
		columns[26].setTemplate("signatureSQLText");
		columns[26].groupByFilters[0] = 0;
		columns[26].groupByFilters[1] = 0;
		columns[26].groupByFilters[2] = 0;
		columns[26].groupByFilters[3] = 0;
		columns[26].groupByFilters[4] = 1;
		columns[26].groupByFilters[5] = 0;
		queryColumns.put(
				ELFunctions.getMessage(SQLdmI18NStrings.SIGNATURE_SQL_TEXT),
				columns[26]);

		// Event Type
		columns[27].setValue("eventType");
		columns[27].setWidth("100px");
		columns[27].groupByFilters[0] = 0;
		columns[27].groupByFilters[1] = 0;
		columns[27].groupByFilters[2] = 0;
		columns[27].groupByFilters[3] = 0;
		columns[27].groupByFilters[4] = 1;
		columns[27].groupByFilters[5] = 1;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.EVENT_TYPE),
				columns[27]);

		// Query Name
		columns[28].setValue("queryName");
		columns[28].setWidth("150px");
		columns[28].setTemplate("queryName");
		columns[28].groupByFilters[0] = 0;
		columns[28].groupByFilters[1] = 0;
		columns[28].groupByFilters[2] = 0;
		columns[28].groupByFilters[3] = 0;
		columns[28].groupByFilters[4] = 0;
		columns[28].groupByFilters[5] = 1;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.QUERY_NAME),
				columns[28]);

		// SQL text
		columns[29].setValue("sqlText");
		columns[29].setWidth("100px");
		columns[29].setTemplate("statementSQLText");
		columns[29].groupByFilters[0] = 0;
		columns[29].groupByFilters[1] = 0;
		columns[29].groupByFilters[2] = 0;
		columns[29].groupByFilters[3] = 0;
		columns[29].groupByFilters[4] = 0;
		columns[29].groupByFilters[5] = 1;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.SQL_TEXT),
				columns[29]);

		// Keep Detailed History Flag
		columns[30].setValue("keepDetailedHistoryFlag");
		columns[30].setWidth("100px");
		columns[30].groupByFilters[0] = 0;
		columns[30].groupByFilters[1] = 0;
		columns[30].groupByFilters[2] = 0;
		columns[30].groupByFilters[3] = 0;
		columns[30].groupByFilters[4] = 1;
		columns[30].groupByFilters[5] = 1;
		queryColumns.put(ELFunctions
				.getMessage(SQLdmI18NStrings.KEEP_DETAILED_HISTORY_FLAG),
				columns[30]);

		// Aggregated
		columns[31].setValue("aggregated");
		columns[31].setWidth("100px");
		columns[31].groupByFilters[0] = 0;
		columns[31].groupByFilters[1] = 0;
		columns[31].groupByFilters[2] = 0;
		columns[31].groupByFilters[3] = 0;
		columns[31].groupByFilters[4] = 1;
		columns[31].groupByFilters[5] = 1;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.AGGREGATED),
				columns[31]);

		// Spid
		columns[32].setValue("spid");
		columns[32].setWidth("100px");
		columns[32].setTemplate("numberLabel");
		columns[32].groupByFilters[0] = 0;
		columns[32].groupByFilters[1] = 0;
		columns[32].groupByFilters[2] = 0;
		columns[32].groupByFilters[3] = 0;
		columns[32].groupByFilters[4] = 0;
		columns[32].groupByFilters[5] = 2;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.SPID),
				columns[32]);

		// Start Time
		columns[33].setValue("startTime");
		columns[33].setWidth("120px");
		// columns[33].setTemplate("dateTime");
		columns[33].groupByFilters[0] = 0;
		columns[33].groupByFilters[1] = 0;
		columns[33].groupByFilters[2] = 0;
		columns[33].groupByFilters[3] = 0;
		columns[33].groupByFilters[4] = 0;
		columns[33].groupByFilters[5] = 2;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.START_TIME),
				columns[33]);

		// Query Number
		columns[34].setValue("queryNum");
		columns[34].setWidth("130px");
		columns[34].groupByFilters[0] = 0;
		columns[34].groupByFilters[1] = 0;
		columns[34].groupByFilters[2] = 0;
		columns[34].groupByFilters[3] = 0;
		columns[34].groupByFilters[4] = 1;
		columns[34].groupByFilters[5] = 1;
		queryColumns.put(ELFunctions.getMessage(SQLdmI18NStrings.QUERY_NUM),
				columns[34]);
	}

	// On click of group by option
	/**
	 * Sets the hidden and visible columns list from total set according to the
	 * selected group by
	 */
	@NotifyChange
	@DependsOn("filter")
	public void setVisibleAndHiddenColumnsList() {
		List<String> hidden = new ArrayList<String>();
		List<String> visible = new ArrayList<String>();
		if (filter != null)
			selectedGroupBy = filter.getSelectedGroupBy().getGroupName();

		if (drillDown != 0) {
			if (selectedGroupBy.equals("Application")
					|| selectedGroupBy.equals("Database")
					|| selectedGroupBy.equals("Client")
					|| selectedGroupBy.equals("User")) {
				selectedGroupBy = "Query Signature";
			} else if (selectedGroupBy.equals("Query Signature")
					&& (drillDown == 2)) {
				selectedGroupBy = "Query Statement";
			}
		}

		ArrayList<String> orderedList = new ArrayList<>();
		orderedList = getOrderedList(selectedGroupBy);
		int col = 0;
		for (int i = 0; i < 60; i++) {
			visible.add("");
		}
		for (String columnName : queryColumns.keySet()) {
			switch (selectedGroupBy) {
			case "Database":
				if (queryColumns.get(columnName).groupByFilters[1] == 1) {
					col = orderedList.indexOf(columnName);
					visible.set(col, columnName);
				} else if (queryColumns.get(columnName).groupByFilters[1] == 2)
					hidden.add(columnName);
				break;
			case "User":
				if (queryColumns.get(columnName).groupByFilters[2] == 1) {
					col = orderedList.indexOf(columnName);
					visible.set(col, columnName);
				} else if (queryColumns.get(columnName).groupByFilters[2] == 2)
					hidden.add(columnName);
				break;
			case "Client":
				if (queryColumns.get(columnName).groupByFilters[3] == 1) {
					col = orderedList.indexOf(columnName);
					visible.set(col, columnName);
				} else if (queryColumns.get(columnName).groupByFilters[3] == 2)
					hidden.add(columnName);
				break;
			case "Query Signature":
				if (queryColumns.get(columnName).groupByFilters[4] == 1) {
					col = orderedList.indexOf(columnName);
					visible.set(col, columnName);
				} else if (queryColumns.get(columnName).groupByFilters[4] == 2)
					hidden.add(columnName);
				break;
			case "Query Statement":
				if (queryColumns.get(columnName).groupByFilters[5] == 1) {
					col = orderedList.indexOf(columnName);
					visible.set(col, columnName);
				} else if (queryColumns.get(columnName).groupByFilters[5] == 2)
					hidden.add(columnName);
				break;
			default:
				if (queryColumns.get(columnName).groupByFilters[0] == 1) {
					col = orderedList.indexOf(columnName);
					visible.set(col, columnName);
				} else if (queryColumns.get(columnName).groupByFilters[0] == 2)
					hidden.add(columnName);
				break;

			}
		}
		columnList = hidden;
		if (defaultColumnList != null) {
			defaultColumnList.clear();
			for (String visCol : visible) {
				if (!visCol.isEmpty() && visCol != null)
					defaultColumnList.add(visCol);
			}
		}

		BindUtils.postNotifyChange(null, null, this, "columnList");
		BindUtils.postNotifyChange(null, null, this, "defaultColumnList");
	}

	/**
	 * Changes the visible and hidden list according to the columns chosen
	 * through column chooser
	 * 
	 * @param visibleColumns
	 *            - list of the visible columns
	 * @param hiddenColumns
	 *            - list of hidden columns
	 * 
	 */
	@Command
	@NotifyChange({ "defaultColumnList", "columnList", "queriesModel" })
	public void doColumnVisibilityChange(
			@BindingParam("visibleColumns") List<String> visibleColumns,
			@BindingParam("hiddenColumns") List<String> hiddenColumns) {
		List<String> hidden = new ArrayList<>();
		List<String> visible = new ArrayList<>();

		List<String> visibleColumnsClone = new ArrayList<>(visibleColumns);

		if (filter != null)
			selectedGroupBy = filter.getSelectedGroupBy().getGroupName();
		if (drillDown != 0) {
			if (selectedGroupBy.equals("Application")
					|| selectedGroupBy.equals("Database")
					|| selectedGroupBy.equals("Client")
					|| selectedGroupBy.equals("User")) {
				selectedGroupBy = "Query Signature";
			} else if (selectedGroupBy.equals("Query Signature")
					&& drillDown == 2) {
				selectedGroupBy = "Query Statement";
			}
		}

		for (String hiddenCol : hiddenColumns) {
			switch (selectedGroupBy) {
			case "Database":
				if (queryColumns.get(hiddenCol).groupByFilters[1] == 2)
					hidden.add(hiddenCol);
				else if (queryColumns.get(hiddenCol).groupByFilters[1] == 1)
					visibleColumnsClone.add(hiddenCol);
				break;
			case "User":
				if (queryColumns.get(hiddenCol).groupByFilters[2] == 2)
					hidden.add(hiddenCol);
				else if (queryColumns.get(hiddenCol).groupByFilters[2] == 1)
					visibleColumnsClone.add(hiddenCol);
				break;
			case "Client":
				if (queryColumns.get(hiddenCol).groupByFilters[3] == 2)
					hidden.add(hiddenCol);
				else if (queryColumns.get(hiddenCol).groupByFilters[3] == 1)
					visibleColumnsClone.add(hiddenCol);
				break;
			case "Query Signature":
				if (queryColumns.get(hiddenCol).groupByFilters[4] == 2)
					hidden.add(hiddenCol);
				else if (queryColumns.get(hiddenCol).groupByFilters[4] == 1)
					visibleColumnsClone.add(hiddenCol);
				break;
			case "Query Statement":
				if (queryColumns.get(hiddenCol).groupByFilters[5] == 2)
					hidden.add(hiddenCol);
				else if (queryColumns.get(hiddenCol).groupByFilters[5] == 1)
					visibleColumnsClone.add(hiddenCol);
				break;
			default:
				if (queryColumns.get(hiddenCol).groupByFilters[0] == 2)
					hidden.add(hiddenCol);
				else if (queryColumns.get(hiddenCol).groupByFilters[0] == 1)
					visibleColumnsClone.add(hiddenCol);
				break;

			}
		}

		visibleColumns = visibleColumnsClone;

		ArrayList<String> orderedList = new ArrayList<>();
		orderedList = getOrderedList(selectedGroupBy);
		int col = 0;
		for (int i = 0; i < 60; i++) {
			visible.add("");
		}
		for (String visibleCol : visibleColumns) {
			switch (selectedGroupBy) {
			case "Database":
				if (queryColumns.get(visibleCol).groupByFilters[1] == 1
						|| queryColumns.get(visibleCol).groupByFilters[1] == 2) {
					col = orderedList.indexOf(visibleCol);
					visible.add(col, visibleCol);
				}

				break;
			case "User":
				if (queryColumns.get(visibleCol).groupByFilters[2] == 1
						|| queryColumns.get(visibleCol).groupByFilters[2] == 2) {
					col = orderedList.indexOf(visibleCol);
					visible.add(col, visibleCol);
				}
				break;
			case "Client":
				if (queryColumns.get(visibleCol).groupByFilters[3] == 1
						|| queryColumns.get(visibleCol).groupByFilters[3] == 2) {
					col = orderedList.indexOf(visibleCol);
					visible.add(col, visibleCol);
				}

				break;
			case "Query Signature":
				if (queryColumns.get(visibleCol).groupByFilters[4] == 1
						|| queryColumns.get(visibleCol).groupByFilters[4] == 2) {
					col = orderedList.indexOf(visibleCol);
					visible.add(col, visibleCol);
				}

				break;
			case "Query Statement":
				if (queryColumns.get(visibleCol).groupByFilters[5] == 1
						|| queryColumns.get(visibleCol).groupByFilters[5] == 2) {
					col = orderedList.indexOf(visibleCol);
					visible.add(col, visibleCol);
				}

				break;
			default:
				if (queryColumns.get(visibleCol).groupByFilters[0] == 1
						|| queryColumns.get(visibleCol).groupByFilters[0] == 2) {
					col = orderedList.indexOf(visibleCol);
					visible.set(col, visibleCol);
				}

				break;

			}
		}

		columnList = hidden;
		defaultColumnList.clear();
		for (String visCol : visible) {
			if (!visCol.isEmpty() && visCol != null)
				defaultColumnList.add(visCol);
		}

	}

	/**
	 * Defines the ordering of columns for particular group by
	 * 
	 * @param selectedGroupBy
	 * @return
	 */
	public ArrayList<String> getOrderedList(String selectedGroupBy) {
		switch (selectedGroupBy) {

		case "Database":
			ArrayList<String> dbOrder = new ArrayList<>();
			// Default
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.DATABASE));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.OCCURRENCES));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_DURATION));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_DURATION));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_CPU_TIME));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_READS));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_WRITES));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_IO));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_WAIT_TIME));
			dbOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.MOST_RECENT_COMPLETION));
			dbOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_BLOCKING_TIME));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_DEADLOCKS));
			// Available
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.APPLICATION));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_CPU_TIME));
			dbOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_CPU_PER_SEC));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_READS));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_WRITES));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_IO));
			dbOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_WAIT_TIME));
			dbOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_BLOCKING_TIME));
			dbOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_DEADLOCKS));
			dbOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.CPU_AS_PERCENTAGE_LIST));
			dbOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.READS_AS_PERCENTAGE_LIST));

			return dbOrder;

		case "User":
			ArrayList<String> userOrder = new ArrayList<>();
			// Default
			userOrder
					.add(ELFunctions.getMessage(SQLdmI18NStrings.FILTER_USERS));
			userOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.OCCURRENCES));
			userOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_DURATION));
			userOrder
					.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_DURATION));
			userOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_CPU_TIME));
			userOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_READS));
			userOrder
					.add(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_WRITES));
			userOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_WAIT_TIME));
			userOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.MOST_RECENT_COMPLETION));
			userOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_BLOCKING_TIME));
			userOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_DEADLOCKS));
			// Available
			userOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.APPLICATION));
			userOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.DATABASE));
			userOrder
					.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_CPU_TIME));
			userOrder
					.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_DURATION));
			userOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_CPU_PER_SEC));
			userOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_READS));
			userOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_WRITES));
			userOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_IO_PER_SEC));
			userOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_WAIT_TIME));
			userOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_BLOCKING_TIME));
			userOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_DEADLOCKS));
			userOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.CPU_AS_PERCENTAGE_LIST));
			userOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.READS_AS_PERCENTAGE_LIST));

			return userOrder;

		case "Client":
			ArrayList<String> clientOrder = new ArrayList<>();
			// Default
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.FILTER_CLIENTS));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.OCCURRENCES));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_DURATION));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_DURATION));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_CPU_TIME));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_READS));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_WRITES));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_WAIT_TIME));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.MOST_RECENT_COMPLETION));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_BLOCKING_TIME));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_DEADLOCKS));
			// Available
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.APPLICATION));
			clientOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.DATABASE));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_CPU_TIME));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_CPU_PER_SEC));
			clientOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_READS));
			clientOrder
					.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_WRITES));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_IO_PER_SEC));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_WAIT_TIME));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_BLOCKING_TIME));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_DEADLOCKS));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.CPU_AS_PERCENTAGE_LIST));
			clientOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.READS_AS_PERCENTAGE_LIST));

			return clientOrder;

		case "Query Signature":
			ArrayList<String> querySignatureOrder = new ArrayList<>();
			// Default
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.QUERY_NUM));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.SIGNATURE_SQL_TEXT));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.OCCURRENCES));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.EVENT_TYPE));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_DURATION));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_DURATION));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_CPU_TIME));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_READS));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_WRITES));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_WAIT_TIME));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.MOST_RECENT_COMPLETION));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_BLOCKING_TIME));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_DEADLOCKS));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.KEEP_DETAILED_HISTORY_FLAG));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AGGREGATED));
			// Available
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.APPLICATION));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.DATABASE));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_CPU_TIME));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_CPU_PER_SEC));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_READS));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_WRITES));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_IO_PER_SEC));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_WAIT_TIME));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_BLOCKING_TIME));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_DEADLOCKS));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.CPU_AS_PERCENTAGE_LIST));
			querySignatureOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.READS_AS_PERCENTAGE_LIST));

			return querySignatureOrder;

		case "Query Statement":
			ArrayList<String> queryStaementOrder = new ArrayList<>();
			// Default
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.QUERY_NUM));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.QUERY_NAME));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.SQL_TEXT));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.EVENT_TYPE));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.OCCURRENCES));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_DURATION));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_CPU_TIME));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_READS));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_WRITES));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_WAIT_TIME));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.MOST_RECENT_COMPLETION));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_BLOCKING_TIME));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_DEADLOCKS));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.KEEP_DETAILED_HISTORY_FLAG));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AGGREGATED));
			// Available
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.APPLICATION));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_CPU_PER_SEC));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.FILTER_CLIENTS));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.DATABASE));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.SPID));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.START_TIME));
			queryStaementOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.FILTER_USERS));

			return queryStaementOrder;

		default:
			ArrayList<String> appOrder = new ArrayList<>();
			// Default
			appOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.APPLICATION));
			appOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.OCCURRENCES));
			appOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_DURATION));
			appOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_DURATION));
			appOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_CPU_TIME));
			appOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_READS));
			appOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_WRITES));
			appOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_IO));
			appOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_WAIT_TIME));
			appOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.MOST_RECENT_COMPLETION));
			appOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_BLOCKING_TIME));
			appOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.TOTAL_DEADLOCKS));
			// Available
			appOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_CPU_TIME));
			appOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_CPU_PER_SEC));
			appOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_READS));
			appOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_WRITES));
			appOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_IO));
			appOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_WAIT_TIME));
			appOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.AVG_BLOCKING_TIME));
			appOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVG_DEADLOCKS));
			appOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.DATABASE));
			appOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.CPU_AS_PERCENTAGE_LIST));
			appOrder.add(ELFunctions
					.getMessage(SQLdmI18NStrings.READS_AS_PERCENTAGE_LIST));

			return appOrder;

		}

	}

	@Listen("onDrillDate=#queryChartDiv")
	public void onDrillDate(Event event) {

		SimpleDateFormat dateReq = new SimpleDateFormat(
				"E MMM dd HH:mm:ss zzz yyyy");
		SimpleDateFormat formatterDate = new SimpleDateFormat("yyyy-MM-dd");
		SimpleDateFormat formatterTime = new SimpleDateFormat("HH:mm:ss.S ");

		String[] time = event.getData().toString().split("&");

		String endDateS = null;
		String endtimeS = null;

		String startDateS = null;
		String startTimeS = null;

		Date endDate = null;
		Date endTime = null;

		Date startDate = null;
		Date startTime = null;

		try {
			endDateS = formatterDate.format(dateReq.parse(time[1]));
			endDate = formatterDate.parse(endDateS);

			endtimeS = formatterTime.format(dateReq.parse(time[1]));
			endTime = formatterTime.parse(endtimeS);

			startDateS = formatterDate.format(dateReq.parse(time[0]));
			startDate = formatterDate.parse(startDateS);

			startTimeS = formatterTime.format(dateReq.parse(time[0]));
			startTime = formatterTime.parse(startTimeS);
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		filter.setSelectedTimePeriod(QueryTimePeriodsEnum.CUSTOM
				.getTimePeriod());
		filter.setSelectedTimePeriod(QueryTimePeriodsEnum.CUSTOM
				.getTimePeriod());
		filter.setEndDate(endDate);
		filter.setFromDate(startDate);
		filter.setEndTime(endTime);
		filter.setFromTime(startTime);
		updateSession();

		BindUtils.postNotifyChange(null, null, this, "filter");
		publishChangeFilterEvent(true);

		// drawChart();
	}

	@Listen("onDrillGraph = #queryChartDiv")
	public void onDrillGraph(Event event) {

		if (filter != null)
			selectedGroupBy = filter.getSelectedGroupBy().getGroupName();
		String id = event.getData().toString();
		int groupID = 1;

		if ((selectedGroupBy.equals("Application")
				|| selectedGroupBy.equals("User")
				|| selectedGroupBy.equals("Database") || selectedGroupBy
					.equals("Client")) && (drillDown == 0)) {

			switch (selectedGroupBy) {
			case "Application":
				groupID = 1;
				break;

			case "Client":
				groupID = 3;
				break;

			case "Database":
				groupID = 2;
				break;

			case "User":
				groupID = 4;
				break;
			}

		} else if (((selectedGroupBy.equals("Application")
				|| selectedGroupBy.equals("User")
				|| selectedGroupBy.equals("Database") || selectedGroupBy
					.equals("Client")) && (drillDown == 1))
				|| ((selectedGroupBy.equals("Query Signature")) && (drillDown == 0))) {
			groupID = 5;
		} else if ((selectedGroupBy.equals("Query Signature"))
				&& (drillDown == 2)) {
			groupID = 6;
		}

		int flag = 0;
		for (QueryApplicationDetails query : queriesModel) {
			if (flag == 1)
				break;
			switch (groupID) {
			case 1:
				if (query.getApplicationID().equals(id)) {
					queryRow = query;
					flag = 1;
				}
				break;
			case 2:
				if (query.getDatabaseID().equals(id)) {
					queryRow = query;
					flag = 1;
				}
				break;
			case 3:
				if (query.getClientID().equals(id)) {
					queryRow = query;
					flag = 1;
				}
				break;
			case 4:
				if (query.getUserID().equals(id)) {
					queryRow = query;
					flag = 1;
				}
				break;
			case 5:
				StringTokenizer str = new StringTokenizer(id, ":");
				if ((query.getSqlSignatureID() == Integer.parseInt(str
						.nextToken()))
						&& (query.getEventId() == Integer.parseInt(str
								.nextToken()))) {
					queryRow = query;
					flag = 1;
				}
				break;
			case 6:
				if (query.getQueryStatisticsID() == Integer.parseInt(id)) {
					queryRow = query;
					flag = 1;
				}
				break;
			}
		}
		onClickSelectedRow(queryRow);
	}

	// For Bar Chart
	private void setEmptyBarCharts() {
		queryChart.setErrorMessage(ELFunctions
				.getMessage(SQLdmI18NStrings.QUERIES_DATA_NOT_AVAILABLE));
	}

	// Getters & Setters

	public QueryFilter getFilter() {
		return filter;
	}

	public void setFilter(QueryFilter filter) {
		this.filter = filter;
	}

	public Paging getInstancesListPgId() {
		return instancesListPgId;
	}

	public void setInstancesListPgId(Paging instancesListPgId) {
		this.instancesListPgId = instancesListPgId;
	}

	public Paging getQueriesWaitsListPgId() {
		return queriesListPgId;
	}

	public void setQueriesWaitsListPgId(Paging queriesListPgId) {
		this.queriesListPgId = queriesListPgId;
	}

	public int getInstanceId() {
		return instanceId;
	}

	public void setInstanceId(int instanceId) {
		this.instanceId = instanceId;
	}

	public IderaBarChart getQueryWaitDurationGraph() {
		return queryWaitDurationGraph;
	}

	public void setQueryWaitDurationGraph(IderaBarChart queryWaitDurationGraph) {
		this.queryWaitDurationGraph = queryWaitDurationGraph;
	}

	public Label getQueriesTableTitle() {
		return QueriesTableTitle;
	}

	public void setQueriesTableTitle(Label queriesTableTitle) {
		QueriesTableTitle = queriesTableTitle;
	}

	public Grid getQueriesWaitGrid() {
		return queriesWaitGrid;
	}

	public void setQueriesWaitGrid(Grid queriesWaitGrid) {
		this.queriesWaitGrid = queriesWaitGrid;
	}

	public ListModelList<QueryApplicationDetails> getQueriesModel() {
		return queriesModel;
	}

	public void setQueriesModel(
			ListModelList<QueryApplicationDetails> queriesModel) {
		this.queriesModel = queriesModel;
	}

	public String getQueriesWaitsLabel() {
		return queriesWaitsLabel;
	}

	public void setQueriesWaitsLabel(String queriesWaitsLabel) {
		this.queriesWaitsLabel = queriesWaitsLabel;
	}

	public static long getSerialversionuid() {
		return serialVersionUID;
	}

	public static Logger getLog() {
		return log;
	}

	public int getListRowsCount() {
		return listRowsCount;
	}

	public int getQueriesListRowsCount() {
		return queriesListRowsCount;
	}

	public List<String> getColumnList() {
		return columnList;
	}

	public void setColumnList(List<String> columnList) {
		this.columnList = columnList;
	}

	public String getQueriesWaitsTitle() {
		return queriesWaitsTitle;
	}

	public void setQueriesWaitsTitle(String queriesWaitsTitle) {
		this.queriesWaitsTitle = queriesWaitsTitle;
	}

	private String displayMessageForQueriesGrid;

	public HashMap<String, QueryColumns> getQueryColumns() {
		return queryColumns;
	}

	public void setQueryColumns(HashMap<String, QueryColumns> queryColumns) {
		this.queryColumns = queryColumns;
	}

	public QueryColumns[] getColumns() {
		return columns;
	}

	public void setColumns(QueryColumns[] columns) {
		this.columns = columns;
	}

	public String getSelectedGroupBy() {
		return selectedGroupBy;
	}

	public void setSelectedGroupBy(String selectedGroupBy) {
		this.selectedGroupBy = selectedGroupBy;
	}

	public QueryGroups getQueriesGroupBy1() {
		return queriesGroupBy1;
	}

	public void setQueriesGroupBy1(QueryGroups queriesGroupBy1) {
		this.queriesGroupBy1 = queriesGroupBy1;
	}

	public QueryGroups getQueriesGroupBy2() {
		return queriesGroupBy2;
	}

	public void setQueriesGroupBy2(QueryGroups queriesGroupBy2) {
		this.queriesGroupBy2 = queriesGroupBy2;
	}

	public List<QueryApplicationDetails> getQueriesDetails() {
		return queriesDetails;
	}

	public void setQueriesDetails(List<QueryApplicationDetails> queriesDetails) {
		this.queriesDetails = queriesDetails;
	}

	public String getDisplayMessageForQueriesGrid() {
		return displayMessageForQueriesGrid;
	}

	public void setDisplayMessageForQueriesGrid(
			String displayMessageForQueriesGrid) {
		this.displayMessageForQueriesGrid = displayMessageForQueriesGrid;
	}

	public List<String> getDefaultColumnList() {
		return defaultColumnList;
	}

	public void setDefaultColumnList(List<String> defaultColumnList) {
		this.defaultColumnList = defaultColumnList;
	}

	public void setSelectedApplications(
			List<QueryApplication> selectedApplications) {
		this.selectedApplications = selectedApplications;
	}

	public void setSelectedDatabases(List<QueryDatabases> selectedDatabases) {
		this.selectedDatabases = selectedDatabases;
	}

	public void setSelectedClients(List<QueryClients> selectedClients) {
		this.selectedClients = selectedClients;
	}

	public void setSelectedUsers(List<QueryUsers> selectedUsers) {
		this.selectedUsers = selectedUsers;
	}

	public List<QueryApplication> getSelectedApplications() {
		return selectedApplications;
	}

	public List<QueryDatabases> getSelectedDatabases() {
		return selectedDatabases;
	}

	public List<QueryClients> getSelectedClients() {
		return selectedClients;
	}

	public List<QueryUsers> getSelectedUsers() {
		return selectedUsers;
	}

	public String getFromDateTime() {
		return fromDateTime;
	}

	public void setFromDateTime(String fromDateTime) {
		this.fromDateTime = fromDateTime;
	}

	public String getEndDateTime() {
		return endDateTime;
	}

	public void setEndDateTime(String endDateTime) {
		this.endDateTime = endDateTime;
	}

	public QueryApplicationDetails getQueryRow() {
		return queryRow;
	}

	public int getActivePage() {
		return activePage;
	}

	public void setActivePage(int activePage) {
		this.activePage = activePage;
		BindUtils.postNotifyChange(null, null, this, "activePage");
	}

	public int getDefaultQueriesRowCount() {
		return defaultQueriesRowCount;
	}

	public void setDefaultQueriesRowCount(int defaultAlertsRowCount) {
		this.defaultQueriesRowCount = defaultAlertsRowCount;
	}

	public Comparator<QueryApplicationDetails> chooseAscComparator(
			String compareColumn) {

		return new QueriesComparator(
				queryColumns.get(compareColumn).getValue(), true);

	}

	public Comparator<QueryApplicationDetails> chooseDescComparator(
			String compareColumn) {

		return new QueriesComparator(
				queryColumns.get(compareColumn).getValue(), false);

	}

	public int getDrillDown() {
		return drillDown;
	}

	public void setDrillDown(int drillDown) {
		this.drillDown = drillDown;
	}

	@Command("loadChartObject")
	public void loadChartObj(@BindingParam("obj") IderaStackedBarChart obj) {
		queryChart = obj;
	}

	@Command("showOptions")
	public void showOptions(@BindingParam("show") String showTop,
			@BindingParam("ref") Paging queriesListPgId) {

		switch (showTop) {

		case "100":
			queriesListRowsCount = 100;
			break;

		case "200":
			queriesListRowsCount = 200;
			break;

		case "All":
			int modelSize = queriesModel.getSize();
			if (modelSize > queriesListRowsCount)
				queriesListRowsCount = modelSize;
			else
				queriesListRowsCount = defaultQueriesRowCount;
			break;

		}

		BindUtils.postNotifyChange(null, null, this, "queriesListRowsCount");
		setQueriesRowsCount(queriesListPgId);

	}

	public boolean getIsSQLDB2008OrGreater() {

		if (instance.getOverview() != null) {
			String version = instance.getOverview().getProductVersion();
			if (version != null) {
				int start = version.indexOf(".");
				if (start != -1) {
					String majorVersion = version.substring(0,
							version.indexOf("."));
					try {
						// WHEN '8.' THEN '2000'
						// WHEN '9.' THEN '2005'
						// WHEN '10' THEN '2008'
						// WHEN '11' THEN '2012'
						return Integer.parseInt(majorVersion) >= 10;
					} catch (Exception ex) {
						log.error("Failed to determine major version: ", ex);
					}
				}
			}
		}
		return false;
	}

	@Listen("onSetOffset = #queryChartDiv")
	public void setOffset(Event event) {

		offsetValue = event.getData().toString();
		filter.setOffsetHours(Double.parseDouble(offsetValue));
		setFromDateTime(filter.getFromDateTime());
		setEndDateTime(filter.getEndDateTime());
		BindUtils.postNotifyChange(null, null, this, "fromDateTime");
		BindUtils.postNotifyChange(null, null, this, "endDateTime");

		if (querySignID != null) {
			
			SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
					.getInstance().getQueriesPreferenceInSession(instanceId);
			QueryApplicationDetails queryRow = new QueryApplicationDetails();
			if (startDateTimeURL != null && endDateTimeURL != null) {
				filter.setFromDateTime(startDateTimeURL);
				filter.setEndDateTime(endDateTimeURL);
				filter.setSelectedTimePeriod(QueryTimePeriodsEnum.CUSTOM
						.getTimePeriod());
			}

			if (sessionBean.getQueryGroupOptions() == null) {
				try {
					sessionBean.setQueryGroupOptions(QueryGroupsFacade
							.getQueryGroups(productInstanceName));
					PreferencesUtil.getInstance().setQueriesPreferenceInSession(
							sessionBean);
				} catch (RestException e) {
					e.printStackTrace();
				}
			}
			// setting group by
			for (QueryGroups group : sessionBean.getQueryGroupOptions()) {
				if (group.getGroupName().equalsIgnoreCase("Query Signature")) {
					filter.setSelectedGroupBy(group);
				}
			}
			
			updateSession();
			publishChangeFilterEvent(true);
			// Drill Down to Query Statements
			queryRow.setSqlSignatureID(querySignID);
			queryRow.setEventType(statType);
			drillDown = 0;
			onClickSelectedRow(queryRow);
			if (queryID != null) {
				for (QueryApplicationDetails queryStat : queriesDetails) {
					if (queryStat.getQueryStatisticsID() == queryID) {
						queryRow = queryStat;
						break;
					}
				}

				onClickSelectedRow(queryRow);
			}
			
		} else {
			updateModelRest(productInstanceName);
			drawChart();
		}

	}

	public String getOffsetValue() {
		return offsetValue;
	}

	public void setOffsetValue(String offsetValue) {
		this.offsetValue = offsetValue;
	}

	public HashMap<String, String> getQueryNames() {
		return queryIds;
	}

	public void setQueryNames(HashMap<String, String> queryNames) {
		this.queryIds = queryNames;
	}

	public HashMap<String, String> getQueryIds() {
		return queryIds;
	}

	public void setQueryIds(HashMap<String, String> queryIds) {
		this.queryIds = queryIds;
	}
}
