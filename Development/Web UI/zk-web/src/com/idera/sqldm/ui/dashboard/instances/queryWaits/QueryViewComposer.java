package com.idera.sqldm.ui.dashboard.instances.queryWaits;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.LinkedHashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TreeMap;

import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;

import com.idera.common.Utility;
import com.idera.server.web.ELFunctions;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.instances.QueryWaits;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.ui.components.charts.area.IderaAreaChart;
import com.idera.sqldm.ui.components.charts.bar.IderaStackedBarChart;
import com.idera.sqldm.ui.dashboard.instances.queries.SingleInstanceQueriesFacade;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstanceQueryWaitsPreferencesBean;

public abstract class QueryViewComposer extends SelectorComposer<Div> {

	private static final long serialVersionUID = 1L;
	private static final Logger log = Logger.getLogger(IderaAreaChart.class);
	private static final int MAX_XAXIS_LABELS = 25; 
	protected AnnotateDataBinder binder = null;
	protected Integer instanceId;
	protected List<QueryWaits> modelData = null;
	protected Map<String, List<QueryWaits>> groupedLineChartData = new HashMap<>();
	private String chartLabelData = "";
	
	Map<String, Map<String, Number>> timeChartData;
	Map<String, Map<String, Number>> durationChartData;
	Map<String, Integer> idMap;

	Map<String, String> axisLinks = new LinkedHashMap<>();

	private Date fromDate;
	private Date endDate;
	private Date fromTime;
	private Date endTime;

	private int selectedWaitOption;
	private int selectedOptionForCharting;
	private Double offsetHours;
	private int limit;
	private String excludeSql;
	private static final int NUM_DIMENSIONS = 8;
	List<Map<String, Map<String, Number>>> durationStackedChartData;
	List<Map<String, Map<String, Number>>> timeStackedChartData;
	List<Map<String, Integer>> idMaps;
	private Map<QueryChartOptionsEnum, QueryWaitsFilter> filters = new LinkedHashMap<>();

	@Override
	public void doAfterCompose(Div comp) throws Exception {
		super.doAfterCompose(comp);
		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions
				.getCurrent().getParameterMap(), "id");
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

		/*String isOverviewTab = (String) Sessions.getCurrent().getAttribute("isOverviewTab");
		if(isOverviewTab.equals("overview")){
			createGraphOnOverviewScreen();
		}*/
		SingleInstanceQueryWaitsPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueryWaitsPreferenceInSession(instanceId);
		// Restoring values using session object
		if (sessionBean.getSelectedOptionForCharting() != -1) {
			updateViewUsingSession();
			updateSelectedWaitOption();
			drawCharts();

		}

		EventQueue<Event> eq = EventQueues.lookup("changeFilters",
				EventQueues.DESKTOP, true);
		eq.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {

				if (event.getName().equals("changeWaitsOption")) {

					setSelectedWaitOption((int) event.getData());
					updateSelectedWaitOption();

				} else if (event.getName().equals("changeFilters")) {

					updateViewUsingSession();
					drawCharts();

				}

			}
		});
		EventQueue<Event> eq1 = EventQueues.lookup("historyChange1", EventQueues.DESKTOP, true);
		eq1.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {
				refreshGraphs();
			}
		});

	}
	
	protected void refreshGraphs() {
		createGraphOnOverviewScreen();
		drawCharts();
		EventQueue<Event> eq = EventQueues.lookup("changeFilters",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("changeFilters", null, null));
		}
	}
	
	public void createGraphOnOverviewScreen(){
		setSelectedWaitOption(0);
		setSelectedOptionForCharting(QueryChartOptionsEnum.WAIT_CATEGORY.getTabId());
		offsetHours = ((int) Sessions.getCurrent().getAttribute(
				WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))
				/ (-3600 * 1000.0);
		setDateTime();
		setLimit(10);
		setExcludeSql("");
		getModelDataQueryWait();
		getModelData();
		filterModelData();
		updateSession();
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
	
	private void filterModelData() {
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
	private void getModelDataQueryWait() {
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

	private void setDateTime() {
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		setFromDate(pref.getFromDate());
		setFromTime(pref.getFromTime());
		setEndDate(pref.getToDate());
		setEndTime(pref.getToTime());
		
	}

	public int getSelectedOptionForCharting() {
		return selectedOptionForCharting;
	}

	public void setSelectedOptionForCharting(int option) {
		this.selectedOptionForCharting = option;
	}

	/**
	 * Update visibility of graphs according to selected wait option(Time or
	 * Duration)
	 */
	private void updateSelectedWaitOption() {

		if (getSelectedWaitOption() == 0) {

			if (getTimeGraphDiv() != null) {
				getTimeGraphDiv().setVisible(true);
			}
			if (getTimeGraphDiv() != null) {
				getDurationGraphDiv().setVisible(false);
			}

		} else {

			if (getTimeGraphDiv() != null) {
				getTimeGraphDiv().setVisible(false);
			}
			if (getTimeGraphDiv() != null) {
				getDurationGraphDiv().setVisible(true);
			}
		}

	}

	/**
	 * Update values of local variables using session object
	 */
	private void updateViewUsingSession() {

		SingleInstanceQueryWaitsPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueryWaitsPreferenceInSession(instanceId);

		setModelData(sessionBean.getModelData());
		setTimeChartData(new LinkedHashMap<>(sessionBean.getTimeStackedChartData().get(getQueryChartOption().getTabId())));
		setDurationChartData(new LinkedHashMap<>(sessionBean.getDurationStackedChartData().get(getQueryChartOption().getTabId())));
		setIdMap(new LinkedHashMap<>(sessionBean.getIdMaps().get(getQueryChartOption().getTabId())));
		setFromDate(sessionBean.getFromDate());
		setFromTime(sessionBean.getFromTime());
		setEndDate(sessionBean.getEndDate());
		setEndTime(sessionBean.getEndTime());
		setSelectedWaitOption(sessionBean.getSelectedWaitOption());
		
	}

	public void drawCharts() {
		// Clear the chart data
/*		durationStackedChartData.clear();
		timeStackedChartData.clear();*/
		loadChartData();
		if ((durationChartData != null)
				&& (!durationChartData.isEmpty())) {
			drawDurationChart();
		} else {
			getLineChartInstance()
					.setErrorMessage(
							ELFunctions
									.getMessage(SQLdmI18NStrings.QUERIES_DATA_NOT_AVAILABLE));
		}

		if ((timeChartData != null) && (!timeChartData.isEmpty())) {
			drawStackedBarChart();
		} else {
			getStackedBarChartInstance()
					.setErrorMessage(
							ELFunctions
									.getMessage(SQLdmI18NStrings.QUERIES_DATA_NOT_AVAILABLE));
		}
	}

	public void loadChartData() {
		
		getStackedBarChartInstance().setErrorMessage(
				SQLdmI18NStrings.DASHBOARD_QUERY_DATA_NOT_AVAILABLE);
		getLineChartInstance().setNoDataMessage(
				SQLdmI18NStrings.DASHBOARD_QUERY_DATA_NOT_AVAILABLE);
		
		try {

			Set<String> groupByNamesTime = new HashSet<>();
			Set<String> groupByNamesDuration = new HashSet<>();

			Map<String, String> nameMap = new LinkedHashMap<String, String>();
			int mapCount = 1;
			chartLabelData = "";
			
			Set<String> keySet = new LinkedHashSet<>(durationChartData.keySet());
			for (String key : keySet) {

				String oldKey = key;
				int id = idMap.get(oldKey);
				
				//Add values to group by sets
				Map<String, Number> innerMap = durationChartData.get(key);
				if(innerMap != null) {
				for (String innerMapKey : innerMap.keySet()) {
					groupByNamesDuration.add(innerMapKey);
				}
				}
				
				//Replacing names with aliases in chart data for durations graph
				if (getStackedBarChartLabel() != null) {
					String mappedName = nameMap.get(key);
					if (mappedName == null) {
						mappedName = getLabelPrefix() + mapCount;
						mapCount++;
						nameMap.put(key, mappedName);
						chartLabelData += mappedName + " = " + key
								+ "\n";
					}
					key = mappedName;
					
					//Renaming the key
					durationChartData.put(key, durationChartData.remove(oldKey));					
				}

				groupByNamesTime.add(key);
				axisLinks.put(key.replace("\\", "\\\\"), "window.waitFilter(\""
						+ getQueryChartOption().getLabel() + "\", \""
						+ id + "\", \""
						+ key + "\")");
				
			}
			
			//Replacing names with aliases in chart data for time graph
			if (getStackedBarChartLabel() != null) {
				for (String key : timeChartData.keySet()) {
					Map<String, Number> innerMap = timeChartData.get(key);
					keySet = new LinkedHashSet<>(innerMap.keySet());
					for (String innerMapKey : keySet) {
						//Renaming the inner map key
						innerMap.put(nameMap.get(innerMapKey), innerMap.remove(innerMapKey));
					}
					timeChartData.put(key, innerMap);					
				}
			}
			
			for (Map<String, Number> innerMap : timeChartData.values()) {
				for (String label : groupByNamesTime) {
					if (!innerMap.containsKey(label)) {
						innerMap.put(label, 0);
					}
				}
			}
			
			for (Map<String, Number> innerMap : durationChartData.values()) {
				for (String label : groupByNamesDuration) {
					if (innerMap != null && !innerMap.containsKey(label)) {
						innerMap.put(label, 0);
					}
				}
			}
			
		} catch (Exception x) {
			log.error(x.getMessage(), x);
			getStackedBarChartInstance()
					.setErrorMessage(
							ELFunctions
									.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
			getLineChartInstance()
					.setErrorMessage(
							ELFunctions
									.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
		}
	}

	private void drawStackedBarChart() {
		IderaStackedBarChart stackedChart = getStackedBarChartInstance();
		stackedChart.setOrient("vertical");
		stackedChart.setChartData(timeChartData);
		stackedChart.setVisible(true);
		stackedChart.getChart().setYAxisTitle(
				"Total Wait Time(milliseconds/second)");

		// Add %s where ever you want bar value to come
		stackedChart.getChart().setMouseOverText("Wait Time: %s");
		// Inner padding adds space between bars
		stackedChart.getChart().setInnerPadding(0.4);
		// Outer padding adds padding on the outer ticks
		stackedChart.getChart().setOuterPadding(0.4);
		stackedChart.getChart().setxAxisTicksCount(MAX_XAXIS_LABELS);
		stackedChart.getChart().setBarLinks("window.clickBar()");
		if (getStackedBarChartLabel() != null) {
			getStackedBarChartLabel().setValue(chartLabelData);
			getStackedBarChartLabel().setMultiline(true);
		}
		/*String isOverviewTab = (String) Sessions.getCurrent().getAttribute("isOverviewTab");
		if(isOverviewTab.equals("overview")){
			stackedChart.setWidth("1110px");
			
			setXAxisDomain(stackedChart, timeChartData);
		}*/
	}

	private void drawDurationChart() {

		IderaStackedBarChart stackedChart = getLineChartInstance();
		stackedChart.setOrient("horizontal");
		stackedChart.setChartData(durationChartData);
		stackedChart.setVisible(true);

		stackedChart.getChart().setAxisLinks(axisLinks);
		stackedChart.getChart().setXAxisTitle(
				"Total Wait Time(milliseconds/second)");

		stackedChart.getChart().setMouseOverText("Wait Time: %s");
		// Inner padding adds space between bars
		stackedChart.getChart().setInnerPadding(0.4);
		// Outer padding adds padding on the outer ticks
		stackedChart.getChart().setOuterPadding(0.4);
		if (getLineChartLabel() != null) {
			getLineChartLabel().setValue(chartLabelData);
			getLineChartLabel().setMultiline(true);
		}
	}
	/**
	 * setting domain of querywaits for overview screen
	 */
	private  void setXAxisDomain(IderaStackedBarChart chart,  Map<String, Map<String, Number>> ChartData){
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		Date fromDateTime = pref.getFromDateTime();
		Date toDateTime= pref.getToDateTime();
		Set<String> dummyString= new HashSet<String>();
		Map<String, Map<String, Number>>  modifiedChartData = new LinkedHashMap<String, Map<String, Number>> ();
		Collection<Map<String, Number>> dataVal = ChartData.values();
		Iterator<Map<String, Number>> itr = dataVal.iterator();
		while(itr.hasNext()){
			Map<String, Number> v1 = itr.next();
			Set<String> keys = v1.keySet();
			if(keys.size()!=0){
				Iterator<String> itrSTr = keys.iterator();
				if(itrSTr == null)
					continue;
				while(itrSTr.hasNext()){
					String currVal = itrSTr.next();
					if(!dummyString.contains(currVal))
						dummyString.add(currVal);
				}
			}

		}

		SimpleDateFormat sdf = new SimpleDateFormat("MM/dd/yy hh:mm a");

		if(!dummyString.isEmpty()){
			Map<String, Number> minValue = new HashMap<String, Number>();
			for (String valString : dummyString) {
				minValue.put(valString, 0.0);
			}
			String key = sdf.format(fromDateTime);
			modifiedChartData.put(key, minValue);
			
			modifiedChartData.putAll(ChartData);
			
			Map<String, Number> maxValue = new HashMap<String, Number>();
			for (String valString : dummyString) {
				maxValue.put(valString, 0.0);
			}
			key = sdf.format(toDateTime);
			//minValue.put(dummyString, 0.0);
			modifiedChartData.put(key, maxValue);
		}

		chart.setChartData(modifiedChartData);
	}
	public abstract IderaStackedBarChart getLineChartInstance();

	public abstract IderaStackedBarChart getStackedBarChartInstance();

	public abstract String getGroupBY(QueryWaits waits);

	public abstract int getGroupById(QueryWaits waits);

	public abstract Div getTimeGraphDiv();

	public abstract Div getDurationGraphDiv();

	public abstract QueryChartOptionsEnum getQueryChartOption();

	public Label getStackedBarChartLabel() {
		return null;
	}

	public Label getLineChartLabel() {
		return null;
	}

	public String getLabelPrefix() {
		return "";
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

	public int getSelectedWaitOption() {
		return selectedWaitOption;
	}

	public void setSelectedWaitOption(int selectedWaitOption) {
		this.selectedWaitOption = selectedWaitOption;
	}

	public List<QueryWaits> getModelData() {
		return modelData;
	}

	public void setModelData(List<QueryWaits> modelData) {
		this.modelData = modelData;
	}

	public Map<String, Map<String, Number>> getTimeChartData() {
		return timeChartData;
	}

	public void setTimeChartData(Map<String, Map<String, Number>> timeChartData) {
		this.timeChartData = timeChartData;
	}

	public Map<String, Map<String, Number>> getDurationChartData() {
		return durationChartData;
	}

	public void setDurationChartData(
			Map<String, Map<String, Number>> durationChartData) {
		this.durationChartData = durationChartData;
	}

	public Map<String, Integer> getIdMap() {
		return idMap;
	}

	public void setIdMap(Map<String, Integer> idMap) {
		this.idMap = idMap;
	}

}
