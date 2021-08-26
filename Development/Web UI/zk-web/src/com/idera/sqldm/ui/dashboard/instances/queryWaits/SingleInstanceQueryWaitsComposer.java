package com.idera.sqldm.ui.dashboard.instances.queryWaits;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.json.JSONObject;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Constraint;
import org.zkoss.zul.Label;
import org.zkoss.zul.Tab;
import org.zkoss.zul.Tabbox;

import com.idera.common.Utility;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.instances.QueryWaits;
import com.idera.sqldm.ui.dashboard.instances.queries.SingleInstanceQueriesFacade;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstancePreferencesBean;
import com.idera.sqldm.ui.preferences.SingleInstanceQueryWaitsPreferencesBean;

public class SingleInstanceQueryWaitsComposer {

	private static final int NUM_DIMENSIONS = 8;

	@Wire
	private Combobox optionsCombobox;

	@Wire
	private Tabbox queryWaitsTabbox;

	@Wire
	private Label instanceNameLabel;

	private int instanceId;

	private Date fromDate;
	private Date endDate;
	private Date fromTime;
	private Date endTime;

	private int limit;
	private String excludeSql;

	private Double offsetHours;

	private Map<QueryChartOptionsEnum, QueryWaitsFilter> filters = new LinkedHashMap<>();
	List<QueryWaits> modelData = null;
	List<Map<String, Map<String, Number>>> durationStackedChartData;
	List<Map<String, Map<String, Number>>> timeStackedChartData;
	List<Map<String, Integer>> idMaps;

	private int selectedOptionForCharting;
	private int selectedWaitOption;

	@Init
	public void init() {

		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions
				.getCurrent().getParameterMap(), "id");
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		} else {
			// fallback
			Object param = Executions.getCurrent().getDesktop()
					.getAttribute("instanceId");
			instanceId = (Integer) param;
		}

		SingleInstanceQueryWaitsPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueryWaitsPreferenceInSession(instanceId);

		offsetHours = ((int) Sessions.getCurrent().getAttribute(
				WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))
				/ (-3600 * 1000.0);

		// Restoring values using session object
		if (sessionBean.getSelectedOptionForCharting() != -1) {

			updateViewUsingSession();

		} else {

			setSelectedWaitOption(0);
			setSelectedOptionForCharting(QueryChartOptionsEnum.WAITS.getTabId());
			offsetHours = ((int) Sessions.getCurrent().getAttribute(
					WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))
					/ (-3600 * 1000.0);
			setDateTime();
			setLimit(10);
			setExcludeSql("");
			getModelData();
			filterModelData();
			updateSession();

		}
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
		Selectors.wireEventListeners(view, this);

		/*SingleInstancePreferencesBean pref = PreferencesUtil.getInstance()
				.getSingleInstancePreferencesInSession(instanceId);
		pref.setSelectedCategory(5);
		pref.setSelectedSubCategory(1);
		PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(
				pref);*/

		Object instanceName = Executions.getCurrent().getDesktop()
				.getAttribute("instanceName");
		if (instanceName != null)
			instanceNameLabel.setValue(instanceName.toString());
		EventQueue<Event> eqTabChanged = EventQueues.lookup("tabChanged",
        		EventQueues.DESKTOP, true);
        eqTabChanged.subscribe(new EventListener<Event>() {

        	@Override
        	public void onEvent(Event event) throws Exception {
        		if(event.getName().equals("querywaitTabSelected")){
        			selectedOptionForCharting = (int) event.getData();
        			queryWaitsTabbox.setSelectedIndex(selectedOptionForCharting);
        			setSelectedOptionForCharting(selectedOptionForCharting);
        			changeChartingOption();
        		}
        	}
        });

	}

	/**
	 * Update session object
	 */
	public void updateSession() {

		SingleInstanceQueryWaitsPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueryWaitsPreferenceInSession(instanceId);

		sessionBean.setSelectedWaitOption(getSelectedWaitOption());
		sessionBean.setSelectedOptionForCharting(selectedOptionForCharting);
		sessionBean.setModelData(modelData);
		sessionBean.setTimeStackedChartData(timeStackedChartData);
		sessionBean.setDurationStackedChartData(durationStackedChartData);
		sessionBean.setIdMaps(idMaps);
		sessionBean.setFromDate(fromDate);
		sessionBean.setFromTime(fromTime);
		sessionBean.setEndDate(endDate);
		sessionBean.setEndTime(endTime);
		sessionBean.setLimit(limit);
		sessionBean.setExcludeSql(excludeSql);
		sessionBean.setFilters(filters);
		PreferencesUtil.getInstance().setQueryWaitsPreferenceInSession(
				sessionBean);

	}

	/**
	 * Update values of local variables using session object
	 */
	public void updateViewUsingSession() {

		SingleInstanceQueryWaitsPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueryWaitsPreferenceInSession(instanceId);

		setModelData(sessionBean.getModelData());
		setTimeStackedChartData(sessionBean.getTimeStackedChartData());
		setDurationStackedChartData(sessionBean.getDurationStackedChartData());
		setIdMaps(sessionBean.getIdMaps());
		setFilters(sessionBean.getFilters());
		setFromDate(sessionBean.getFromDate());
		setFromTime(sessionBean.getFromTime());
		setEndDate(sessionBean.getEndDate());
		setEndTime(sessionBean.getEndTime());
		setLimit(sessionBean.getLimit());
		setExcludeSql(sessionBean.getExcludeSql());
		setSelectedWaitOption(sessionBean.getSelectedWaitOption());
		setSelectedOptionForCharting(sessionBean.getSelectedOptionForCharting());

		BindUtils.postNotifyChange(null, null, this, "selectedWaitOption");
		BindUtils.postNotifyChange(null, null, this, "selectedOptionForCharting");

	}

	/**
	 * This method will make API call to get data
	 */
	private void getModelData() {

		final String productInstanceName = Utility.getUrlParameter(Executions
				.getCurrent().getParameterMap(), "instance");

		try {

			int waitId = -1;
			int waitCategoryId = -1;
			int applicationId = -1;
			int statementId = -1;
			int databaseId = -1;
			int clientId = -1;
			int sessionId = -1;
			int userId = -1;

			if (filters.get(QueryChartOptionsEnum.WAITS) != null) {
				waitId = filters.get(QueryChartOptionsEnum.WAITS)
						.getSelectedId();
			}
			if (filters.get(QueryChartOptionsEnum.WAIT_CATEGORY) != null) {
				waitCategoryId = filters.get(
						QueryChartOptionsEnum.WAIT_CATEGORY).getSelectedId();
			}
			if (filters.get(QueryChartOptionsEnum.STATEMENTS) != null) {
				statementId = filters.get(QueryChartOptionsEnum.STATEMENTS)
						.getSelectedId();
			}
			if (filters.get(QueryChartOptionsEnum.APPLICATION) != null) {
				applicationId = filters.get(QueryChartOptionsEnum.APPLICATION)
						.getSelectedId();
			}
			if (filters.get(QueryChartOptionsEnum.DATABASES) != null) {
				databaseId = filters.get(QueryChartOptionsEnum.DATABASES)
						.getSelectedId();
			}
			if (filters.get(QueryChartOptionsEnum.CLIENTS) != null) {
				clientId = filters.get(QueryChartOptionsEnum.CLIENTS)
						.getSelectedId();
			}
			if (filters.get(QueryChartOptionsEnum.SESSIONS) != null) {
				sessionId = filters.get(QueryChartOptionsEnum.SESSIONS)
						.getSelectedId();
			}
			if (filters.get(QueryChartOptionsEnum.USERS) != null) {
				userId = filters.get(QueryChartOptionsEnum.USERS)
						.getSelectedId();
			}

			modelData = SingleInstanceQueriesFacade.getQueryWaitsInstances(
					productInstanceName, instanceId, offsetHours.toString(),
					getFromDateTimeAPI(), getEndDateTimeAPI(), waitId,
					waitCategoryId, statementId, applicationId, databaseId,
					clientId, sessionId, userId);

		} catch (InstanceException e) {
			e.printStackTrace();
		}
	}

	/**
	 * Filter model data on the basis of limit and exclude SQL String
	 */
	public void filterModelData() {

		durationStackedChartData = new ArrayList<Map<String, Map<String, Number>>>(
				NUM_DIMENSIONS);
		timeStackedChartData = new ArrayList<Map<String, Map<String, Number>>>(
				NUM_DIMENSIONS);
		idMaps = new ArrayList<Map<String, Integer>>(NUM_DIMENSIONS);
		for (int i = 0; i < NUM_DIMENSIONS; i++) {
			durationStackedChartData.add(new LinkedHashMap<String, Map<String, Number>>());
			timeStackedChartData.add(new LinkedHashMap<String, Map<String, Number>>());
			idMaps.add(new LinkedHashMap<String, Integer>());
		}

		prepareDurationChartMaps();
		sortAndTrimMaps();
		prepareTimeChartMaps();

	}

	/**
	 * Create Map for duration chart and apply exclude sql text filter
	 */
	public void prepareDurationChartMaps() {

		// Iterating over model data to prepare maps for duration charts for all
		// dimensions
		for (QueryWaits queryWaits : modelData) {

			if (!excludeSql.equals("")
					&& queryWaits.getSQLStatement().toLowerCase().contains(excludeSql.toLowerCase())) {
				continue;
			}

			String keyInnerMap = queryWaits.getWaitCategory();

			// Iterating over all dimension's maps to add this record
			for (QueryChartOptionsEnum chartOption : QueryChartOptionsEnum
					.values()) {

				try {

					// Using getter method listed in Enum
					Method method = QueryWaits.class.getMethod(
							chartOption.getGetterMethod(), (Class[]) null);
					String key = (String) method.invoke(queryWaits,
							(Object[]) null).toString();

					// Get chart data for this dimension
					Map<String, Map<String, Number>> chartData = durationStackedChartData
							.get(chartOption.getTabId());
					if (chartData == null) {
						chartData = new LinkedHashMap<String, Map<String, Number>>();
					}

					if (chartData.containsKey(key)) {
						Map<String, Number> innerMap = chartData.get(key);
						if (innerMap.containsKey(keyInnerMap)) {
							Double duration = (Double) innerMap
									.get(keyInnerMap);
							duration += queryWaits.getWaitDurationPerSecond();
							innerMap.put(keyInnerMap, com.idera.sqldm.utils.Utility.round(duration, 2));

						} else {
							innerMap.put(
									keyInnerMap, com.idera.sqldm.utils.Utility.round(queryWaits.getWaitDurationPerSecond(), 2));
						}
						chartData.put(key, innerMap);
					} else {
						Map<String, Number> innerMap = new TreeMap<String, Number>();
						innerMap.put(
								keyInnerMap, com.idera.sqldm.utils.Utility.round(queryWaits.getWaitDurationPerSecond(), 2));
						chartData.put(key, innerMap);
					}

					durationStackedChartData.set(chartOption.getTabId(),
							chartData);

				} catch (NoSuchMethodException | SecurityException
						| IllegalAccessException | IllegalArgumentException
						| InvocationTargetException e) {
					e.printStackTrace();
				}

			}

		}

	}

	/**
	 * Sort Map data and trim according to the limit on number of rows
	 */
	public void sortAndTrimMaps() {

		// Iterating over all dimension's maps to add this record
		for (QueryChartOptionsEnum chartOption : QueryChartOptionsEnum.values()) {

			// Get chart data for this dimension
			Map<String, Map<String, Number>> chartData = durationStackedChartData
					.get(chartOption.getTabId());

			// Convert Map to List
			List<Map.Entry<String, Map<String, Number>>> list = new LinkedList<Map.Entry<String, Map<String, Number>>>(
					chartData.entrySet());

			// Sort list with comparator, to compare the Map values
			Collections.sort(list,
					new Comparator<Map.Entry<String, Map<String, Number>>>() {
						public int compare(
								Map.Entry<String, Map<String, Number>> o1,
								Map.Entry<String, Map<String, Number>> o2) {
							Map<String, Number> innerMap = o1.getValue();
							Double duration1 = new Double(0);
							for (Number entry : innerMap.values()) {
								duration1 = duration1.doubleValue()
										+ entry.doubleValue();
							}

							innerMap = o2.getValue();
							Double duration2 = new Double(0);
							for (Number entry : innerMap.values()) {
								duration2 = duration2.doubleValue()
										+ entry.doubleValue();
							}
							return duration2.compareTo(duration1);
						}
					});

			int listSize = list.size();
			list = list.subList(0, (limit < listSize) ? limit : listSize);

			// Convert sorted map back to a Map
			chartData = new LinkedHashMap<String, Map<String, Number>>();
			for (Iterator<Map.Entry<String, Map<String, Number>>> it = list
					.iterator(); it.hasNext();) {
				Map.Entry<String, Map<String, Number>> entry = it.next();
				chartData.put(entry.getKey(), entry.getValue());
			}

			durationStackedChartData.set(chartOption.getTabId(), chartData);

		}

	}

	/**
	 * Create Map for time chart and apply exclude sql text filter
	 */
	public void prepareTimeChartMaps() {
		
		//Iterating over model data to prepare maps for duration charts for all dimensions
		for (QueryWaits queryWaits : modelData) {
			
			if(!excludeSql.equals("") && queryWaits.getSQLStatement().contains(excludeSql)) {
				continue;
			}
			
			// Data for time stacked chart
			SimpleDateFormat sdf = new SimpleDateFormat("MM/dd/yy hh:mm a");
			String key = sdf.format(queryWaits
							.getStatementUTCStartTime());
			
			//Iterating over all dimension's maps to add this record
			for (QueryChartOptionsEnum chartOption : QueryChartOptionsEnum.values()) {
				
				try {
				
					//Using getter method for id
					// Using getter method listed in Enum
					Method methodId = QueryWaits.class.getMethod(
							chartOption.getGetterMethodId(), (Class[]) null);
					Integer id = (Integer) methodId.invoke(queryWaits,
							(Object[]) null);

					//Using getter method listed in Enum
					Method method = QueryWaits.class.getMethod(chartOption.getGetterMethod(), (Class[]) null);
					String keyInnerMap = (String) method.invoke(queryWaits, (Object[])null).toString();
					
					if(!durationStackedChartData.get(chartOption.getTabId()).containsKey(keyInnerMap)) {
						continue;
					}
					
					//Get id map for this dimension
					Map<String, Integer> currentIdMap = idMaps.get(chartOption.getTabId());
					currentIdMap.put(keyInnerMap, id);

					//Get chart data for this dimension
					Map<String, Map<String, Number>> chartData = timeStackedChartData.get(chartOption.getTabId()); 
					if (chartData == null) {
						chartData = new LinkedHashMap<String, Map<String, Number>>();
					}
					
					if (chartData
							.containsKey(key)) {
						Map<String, Number> innerMap = chartData
								.get(key);
						if (innerMap.containsKey(keyInnerMap)) {
							Double duration = (Double) innerMap
									.get(keyInnerMap);
							duration += queryWaits.getWaitDurationPerSecond();
							innerMap.put(keyInnerMap, com.idera.sqldm.utils.Utility.round(duration, 2));

						} else {
							innerMap.put(keyInnerMap, com.idera.sqldm.utils.Utility.round(queryWaits
									.getWaitDurationPerSecond(), 2));
						}
						chartData.put(key, innerMap);
					} else {
						Map<String, Number> innerMap = new TreeMap<String, Number>();
						innerMap.put(keyInnerMap, com.idera.sqldm.utils.Utility.round(queryWaits
								.getWaitDurationPerSecond(), 2));
						chartData.put(key, innerMap);
					}
		
					timeStackedChartData.set(chartOption.getTabId(),
							chartData);
					idMaps.set(chartOption.getTabId(), currentIdMap);

				} catch (NoSuchMethodException | SecurityException
						| IllegalAccessException | IllegalArgumentException
						| InvocationTargetException e) {
					e.printStackTrace();
				}

			}
		}
		
	}

	/**
	 * Publish event handled in QueryViewComposer
	 */
	public void publishEvent(boolean refreshModelFlag) {

		EventQueue<Event> eq = EventQueues.lookup("changeFilters",
				EventQueues.DESKTOP, false);
		if (eq != null) {

			if(refreshModelFlag)
				getModelData();
			filterModelData();
			updateSession();
			eq.publish(new Event("changeFilters", null, null));

		}

	}

	@Command("changeTimeRange")
	public void changeTimeRange() {

		publishEvent(true);

	}

	@Command("changeLimit")
	public void changeLimit() {

		publishEvent(false);

	}

	@Command("changeExcludeSqlFilter")
	public void changeExcludeSqlFilter() {

		publishEvent(false);

	}

	@Command("changeQueryWaitOption")
	public void changeQueryWaitOption() {

		setSelectedWaitOption(optionsCombobox.getSelectedIndex());
		updateSession();
		EventQueue<Event> eq = EventQueues.lookup("changeFilters",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("changeWaitsOption", null,
					getSelectedWaitOption()));

		}
	}

	@Command("changeChartingOption")
	public void changeChartingOption() {

		updateSession();

	}

	@Command("initTabbox")
	public void initTabbox(@BindingParam("target") Tab tab) {

		// Update visibility of tab
		if (filters.keySet().contains(
				QueryChartOptionsEnum.findByLabel(tab.getLabel())))
			tab.setVisible(false);
		else
			tab.setVisible(true);

	}

	@Command("drillUp")
	public void drillUp(@BindingParam("filter") QueryWaitsFilter filter) {

		Map<QueryChartOptionsEnum, QueryWaitsFilter> tempFilters = new LinkedHashMap<>();
		if (filter == null) {
			filters.clear();
		} else {

			for (QueryChartOptionsEnum chartOption : filters.keySet()) {

				tempFilters.put(chartOption, filters.get(chartOption));
				if (filters.get(chartOption).equals(filter)) {
					break;
				}
			}
			setFilters(tempFilters);
		}

		List<Component> tabs = queryWaitsTabbox.getTabs().getChildren();
		for (Component tab : tabs) {
			initTabbox((Tab) tab);
		}

		publishEvent(true);
		BindUtils.postNotifyChange(null, null, this, "filters");

	}

	private void updateTabboxSclass() {

		if (queryWaitsTabbox != null) {
			Tab selectedTab = queryWaitsTabbox.getSelectedTab();
			selectedTab.setSclass("query-waits-tabs-seld");
			for (Component comp : queryWaitsTabbox.getTabs().getChildren()) {
				Tab tab = (Tab) comp;
				if (tab.getIndex() != selectedTab.getIndex())
					tab.setSclass("query-waits-tabs");
			}
		}

	}

	@Listen("onClickGraph=#queryWaitsTabbox")
	public void onClickGraph(Event event) {

		// Set Query Waits By Duration option in combobox
		optionsCombobox.setSelectedIndex(1);
		changeQueryWaitOption();

	}

	@Listen("onApplyFilter=#queryWaitsTabbox")
	public void onApplyFilter(Event event) {

		JSONObject json = (JSONObject) event.getData();
		QueryWaitsFilter filter = new QueryWaitsFilter();

		QueryChartOptionsEnum selectedOption = QueryChartOptionsEnum
				.findByLabel((String) json.get("chartOption"));
		filter.setSelectedId(Integer.parseInt(json.get("filterId").toString()));
		if (!(selectedOption.equals(QueryChartOptionsEnum.APPLICATION)
				|| selectedOption.equals(QueryChartOptionsEnum.STATEMENTS) || selectedOption
					.equals(QueryChartOptionsEnum.USERS))) {

			filter.setFilterName(json.get("filterName").toString());

		} else {

			// Iterate over model data to find filter name
			findFilterName(selectedOption, filter);

		}

		List<Component> tabs = queryWaitsTabbox.getTabs().getChildren();
		Tab waitsTab = (Tab) tabs.get(QueryChartOptionsEnum.WAITS.getTabId());
		boolean flag = false;

		// Update active tab
		if (waitsTab.isVisible()
				&& (QueryChartOptionsEnum.WAITS.getTabId() != selectedOption
						.getTabId())) {
			waitsTab.setSelected(true);
			flag = true;
		} else {
			for (int i = selectedOption.getTabId() + 1; i < NUM_DIMENSIONS; i++) {
				if (((Tab) tabs.get(i)).isVisible()) {
					((Tab) tabs.get(i)).setSelected(true);
					flag = true;
					break;
				}
			}
			if (!flag) {
				for (int i = 0; i < selectedOption.getTabId(); i++) {
					if (((Tab) tabs.get(i)).isVisible()) {
						((Tab) tabs.get(i)).setSelected(true);
						flag = true;
						break;
					}
				}
			}
		}

		// Hide tab according to selected filter
		if (flag) {
			for (Component comp : tabs) {
				Tab tab = (Tab) comp;
				if ((tab).getLabel().equals(selectedOption.getLabel())) {
					tab.setVisible(false);
					break;
				}
			}
			filters.put(selectedOption, filter);
		}

		setSelectedOptionForCharting(queryWaitsTabbox.getSelectedTab()
				.getIndex());

		publishEvent(true);
		BindUtils.postNotifyChange(null, null, this, "filters");

	}

	/**
	 * This method iterates over model data to find filter name
	 * 
	 * @param selectedOption
	 * @param filter
	 */
	public void findFilterName(QueryChartOptionsEnum selectedOption,
			QueryWaitsFilter filter) {

		for (QueryWaits wait : modelData) {

			if (selectedOption.equals(QueryChartOptionsEnum.APPLICATION)
					&& (filter.getSelectedId() == wait.getApplicationID())) {

				filter.setFilterName(wait.getApplicationName());
				break;

			} else if (selectedOption.equals(QueryChartOptionsEnum.USERS)
					&& (filter.getSelectedId() == wait.getLoginID())) {

				filter.setFilterName(wait.getLoginName());
				break;

			} else if (selectedOption.equals(QueryChartOptionsEnum.STATEMENTS)
					&& (filter.getSelectedId() == wait.getSQLStatementID())) {

				filter.setFilterName(wait.getSQLStatement());
				break;

			}
		}

	}

	/**
	 * Initializes date and time values
	 */
	public void setDateTime() {

		Calendar c = Calendar.getInstance();
		c.setTime(new Date(c.getTimeInMillis()
				- (long) (getOffsetHours() * 60 * 60 * 1000)));

		c.set(Calendar.HOUR_OF_DAY, 0);
		c.set(Calendar.MINUTE, 0);
		c.set(Calendar.SECOND, 0);
		setEndDate(c.getTime());
		c.set(Calendar.HOUR_OF_DAY, 23);
		c.set(Calendar.MINUTE, 59);
		setEndTime(c.getTime());
		//Change request DE46151
		c.add(Calendar.DATE, -1);
		c.set(Calendar.HOUR_OF_DAY, 0);
		c.set(Calendar.MINUTE, 0);
		setFromDate(c.getTime());
		setFromTime(c.getTime());

		BindUtils.postNotifyChange(null, null, this, "queryFilters");

	}

	public Constraint getFromDateConstraint() {

		Constraint ctt1 = new Constraint() {
			public void validate(Component comp, Object value)
					throws WrongValueException {

				if (value == null)
					throw new WrongValueException(comp,
							"Start date or time cannot be empty.");

				Calendar fromCalendar = Calendar.getInstance();
				fromCalendar.setTime((Date) value);
				if (getFromTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime(getFromTime());
					fromCalendar.set(Calendar.HOUR_OF_DAY,
							timeCal.get(Calendar.HOUR_OF_DAY));
					fromCalendar.set(Calendar.MINUTE,
							timeCal.get(Calendar.MINUTE));
				}

				Calendar endCalendar = Calendar.getInstance();
				endCalendar.setTime(getEndDate());
				if (getEndTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime(getEndTime());
					endCalendar.set(Calendar.HOUR_OF_DAY,
							timeCal.get(Calendar.HOUR_OF_DAY));
					endCalendar.set(Calendar.MINUTE,
							timeCal.get(Calendar.MINUTE));
				}

				if (value == null
						|| (getEndDate() != null && (new Date(
								fromCalendar.getTimeInMillis()))
								.after(new Date(endCalendar.getTimeInMillis())))
						|| (new Date(fromCalendar.getTimeInMillis()))
								.after(new Date(
										new Date().getTime()
												- (long) (getOffsetHours() * 60 * 60 * 1000))))
					throw new WrongValueException(comp,
							"Start date should not be greater than current date.");
			}
		};

		return ctt1;

	}

	public Constraint getFromTimeConstraint() {

		Constraint ctt1 = new Constraint() {
			public void validate(Component comp, Object value)
					throws WrongValueException {

				if (value == null)
					throw new WrongValueException(comp,
							"Start date or time cannot be empty.");

				Calendar fromCalendar = Calendar.getInstance();
				fromCalendar.setTime(getFromDate());
				if (getFromTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime((Date) value);
					fromCalendar.set(Calendar.HOUR_OF_DAY,
							timeCal.get(Calendar.HOUR_OF_DAY));
					fromCalendar.set(Calendar.MINUTE,
							timeCal.get(Calendar.MINUTE));
				}

				Calendar endCalendar = Calendar.getInstance();
				endCalendar.setTime(getEndDate());
				if (getEndTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime(getEndTime());
					endCalendar.set(Calendar.HOUR_OF_DAY,
							timeCal.get(Calendar.HOUR_OF_DAY));
					endCalendar.set(Calendar.MINUTE,
							timeCal.get(Calendar.MINUTE));
				}

				if (value == null
						|| (getEndDate() != null && (new Date(
								fromCalendar.getTimeInMillis()))
								.after(new Date(endCalendar.getTimeInMillis())))
						|| (new Date(fromCalendar.getTimeInMillis()))
								.after(new Date(
										new Date().getTime()
												- (long) (getOffsetHours() * 60 * 60 * 1000))))
					throw new WrongValueException(comp,
							"Start date should not be greater than current date.");
			}
		};

		return ctt1;
	}

	public Constraint getEndDateConstraint() {

		Constraint ctt = new Constraint() {
			public void validate(Component comp, Object value)
					throws WrongValueException {

				if (value == null)
					throw new WrongValueException(comp,
							"End date or time cannot be empty.");

				Calendar fromCalendar = Calendar.getInstance();
				fromCalendar.setTime(getFromDate());
				if (getFromTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime(getFromTime());
					fromCalendar.set(Calendar.HOUR_OF_DAY,
							timeCal.get(Calendar.HOUR_OF_DAY));
					fromCalendar.set(Calendar.MINUTE,
							timeCal.get(Calendar.MINUTE));
				}

				Calendar endCalendar = Calendar.getInstance();
				endCalendar.setTime((Date) value);
				if (getEndTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime(getEndTime());
					endCalendar.set(Calendar.HOUR_OF_DAY,
							timeCal.get(Calendar.HOUR_OF_DAY));
					endCalendar.set(Calendar.MINUTE,
							timeCal.get(Calendar.MINUTE));
				}

				if (value == null
						|| (getFromDate() != null && (new Date(
								endCalendar.getTimeInMillis()))
								.before(new Date(fromCalendar.getTimeInMillis())))
						|| (new Date(endCalendar.getTimeInMillis()))
								.after(new Date(
										new Date().getTime()
												- (long) (getOffsetHours() * 60 * 60 * 1000))))
					throw new WrongValueException(comp,
							"End date should be less than current date and greater than start date.");
			}
		};

		return ctt;

	}

	public Constraint getEndTimeConstraint() {

		Constraint ctt = new Constraint() {
			public void validate(Component comp, Object value)
					throws WrongValueException {

				if (value == null)
					throw new WrongValueException(comp,
							"End date or time cannot be empty.");

				Calendar fromCalendar = Calendar.getInstance();
				fromCalendar.setTime(getFromDate());
				if (getFromTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime(getFromTime());
					fromCalendar.set(Calendar.HOUR_OF_DAY,
							timeCal.get(Calendar.HOUR_OF_DAY));
					fromCalendar.set(Calendar.MINUTE,
							timeCal.get(Calendar.MINUTE));
				}

				Calendar endCalendar = Calendar.getInstance();
				endCalendar.setTime(getEndDate());
				if (getEndTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime((Date) value);
					endCalendar.set(Calendar.HOUR_OF_DAY,
							timeCal.get(Calendar.HOUR_OF_DAY));
					endCalendar.set(Calendar.MINUTE,
							timeCal.get(Calendar.MINUTE));
				}

				if (value == null
						|| (getFromDate() != null && (new Date(
								endCalendar.getTimeInMillis()))
								.before(new Date(fromCalendar.getTimeInMillis())))
						|| (new Date(endCalendar.getTimeInMillis()))
								.after(new Date(
										new Date().getTime()
												- (long) (getOffsetHours() * 60 * 60 * 1000))))
					throw new WrongValueException(comp,
							"End date should be less than current date and greater than start date.");
			}
		};

		return ctt;

	}

	/**
	 * Formats From date and time according to the format used by API
	 * 
	 * @return
	 */
	public String getFromDateTimeAPI() {

		SimpleDateFormat outputFormat = new SimpleDateFormat(
				"yyyy/MM/dd HH:mm:ss");

		Calendar c = formatFromDateTime();

		return (outputFormat.format(c.getTime()));
	}

	/**
	 * Formats End date and time according to the format used by API
	 * 
	 * @return
	 */
	public String getEndDateTimeAPI() {

		SimpleDateFormat outputFormat = new SimpleDateFormat(
				"yyyy/MM/dd HH:mm:ss");

		Calendar c = formatEndDateTime();

		return (outputFormat.format(c.getTime()));
	}

	private Calendar formatFromDateTime() {

		Calendar c = Calendar.getInstance();
		c.setTime(fromDate);
		if (fromTime != null) {
			Calendar timeCal = Calendar.getInstance();
			timeCal.setTime(fromTime);
			c.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
			c.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
		}
		return c;
	}

	private Calendar formatEndDateTime() {

		Calendar c = Calendar.getInstance();

		c.setTime(endDate);

		if (fromTime != null) {
			Calendar timeCal = Calendar.getInstance();
			timeCal.setTime(endTime);
			c.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
			c.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
		}
		return c;
	}

	public Date getFromDate() {
		return fromDate;
	}

	public void setFromDate(Date fromDate) {
		this.fromDate = fromDate;
	}

	public Date getEndDate() {
		return endDate;
	}

	public void setEndDate(Date endDate) {
		this.endDate = endDate;
	}

	public Date getFromTime() {
		return fromTime;
	}

	public void setFromTime(Date fromTime) {
		this.fromTime = fromTime;
	}

	public Date getEndTime() {
		return endTime;
	}

	public void setEndTime(Date endTime) {
		this.endTime = endTime;
	}

	public int getLimit() {
		return limit;
	}

	public void setLimit(int limit) {
		this.limit = limit;
	}

	public String getExcludeSql() {
		return excludeSql;
	}

	public void setExcludeSql(String excludeSql) {
		this.excludeSql = excludeSql;
	}

	public Double getOffsetHours() {
		return offsetHours;
	}

	public void setOffsetHours(Double offsetHours) {
		this.offsetHours = offsetHours;
	}

	public Map<QueryChartOptionsEnum, QueryWaitsFilter> getFilters() {
		return filters;
	}

	public void setFilters(Map<QueryChartOptionsEnum, QueryWaitsFilter> filters) {
		this.filters = filters;
		BindUtils.postNotifyChange(null, null, this, "filters");
	}

	public void setModelData(List<QueryWaits> modelData) {
		this.modelData = modelData;
	}

	public List<Map<String, Map<String, Number>>> getDurationStackedChartData() {
		return durationStackedChartData;
	}

	public void setDurationStackedChartData(
			List<Map<String, Map<String, Number>>> durationStackedChartData) {
		this.durationStackedChartData = durationStackedChartData;
	}

	public List<Map<String, Map<String, Number>>> getTimeStackedChartData() {
		return timeStackedChartData;
	}

	public void setTimeStackedChartData(
			List<Map<String, Map<String, Number>>> timeStackedChartData) {
		this.timeStackedChartData = timeStackedChartData;
	}

	public List<Map<String, Integer>> getIdMaps() {
		return idMaps;
	}

	public void setIdMaps(List<Map<String, Integer>> idMaps) {
		this.idMaps = idMaps;
	}

	public int getSelectedWaitOption() {
		return selectedWaitOption;
	}

	public void setSelectedWaitOption(int selectedWaitOption) {
		this.selectedWaitOption = selectedWaitOption;
		BindUtils.postNotifyChange(null, null, this, "selectedWaitOption");
	}

	public int getSelectedOptionForCharting() {
		return selectedOptionForCharting;
	}

	public void setSelectedOptionForCharting(int option) {
		updateTabboxSclass();
		this.selectedOptionForCharting = option;
		BindUtils.postNotifyChange(null, null, this,
				"selectedOptionForCharting");
	}

	public List<String> getQueryWaitsDimensions() {
		List<String> queryWaitsOptions = new ArrayList<>();
		QueryChartOptionsEnum options[] = QueryChartOptionsEnum.values();
		for (QueryChartOptionsEnum qco : options) {
			queryWaitsOptions.add(qco.getLabel());
		}
		return queryWaitsOptions;
	}

	public List<String> getQueryWaitsDimensionsUrls() {
		List<String> queryWaitsOptions = new ArrayList<>();
		QueryChartOptionsEnum options[] = QueryChartOptionsEnum.values();
		for (QueryChartOptionsEnum qco : options) {
			queryWaitsOptions.add(qco.getUrl());
		}
		return queryWaitsOptions;
	}

}
