package com.idera.sqldm.ui.dashboard.instances.overview;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collection;
import java.util.Date;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedHashMap;
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
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;

import com.idera.common.Utility;
import com.idera.server.web.ELFunctions;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.instances.QueryWaits;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.ui.components.charts.bar.IderaStackedBarChart;
import com.idera.sqldm.ui.dashboard.instances.queries.SingleInstanceQueriesFacade;
import com.idera.sqldm.ui.dashboard.instances.queryWaits.QueryChartOptionsEnum;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstanceOverviewPreferenceBean;

public class OverviewQueryWaitsComposer extends SelectorComposer<Div> {

	private static final long serialVersionUID = 1L;
	private static final Logger log = Logger.getLogger(OverviewQueryWaitsComposer.class);
	private static final int MAX_XAXIS_LABELS = 25; 
	protected AnnotateDataBinder binder = null;
	protected Integer instanceId;
	protected List<QueryWaits> modelData = null;
	protected Map<String, List<QueryWaits>> groupedLineChartData = new HashMap<>();
	private String chartLabelData = "";

	Map<String, Map<String, Number>> timeChartData;
	//Map<String, Integer> idMap;

	Map<String, String> axisLinks = new LinkedHashMap<>();

	private Date fromDate;
	private Date endDate;
	private Date fromTime;
	private Date endTime;

	private Double offsetHours;
	private static final int NUM_DIMENSIONS = 8;
	List<Map<String, Map<String, Number>>> timeStackedChartData;
	//List<Map<String, Integer>> idMaps;

	@Wire
	private IderaStackedBarChart overviewStackedBarChart;
	@Wire
	private Div overviewTimeGraphDiv;

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
		offsetHours = ((int) Sessions.getCurrent().getAttribute(WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))/ (-3600 * 1000.0);

		SingleInstanceOverviewPreferenceBean overPrefBean = PreferencesUtil.getInstance().getOverviewPreferenceInSession(instanceId);
		if(overPrefBean == null || overPrefBean.getQueryWaitsModelData() == null){
	/*		setDateTime();
			getModelDataQueryWait();
			filterModelData();
			updateSession();*/
			createGraphOnOverviewScreen();
		}else {
			updateViewUsingSession(overPrefBean);
		}
		drawCharts();

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
	}

	public void createGraphOnOverviewScreen(){
		setDateTime();
		getModelDataQueryWait();
		//getModelData();
		filterModelData();
		updateSession();
	}

	/**
	 * Update session object
	 */
	public void updateSession() {
		SingleInstanceOverviewPreferenceBean overPrefBean = PreferencesUtil.getInstance().getOverviewPreferenceInSession(instanceId);
		overPrefBean.setQueryWaitsModelData(modelData);
		overPrefBean.setTimeStackedChartData(timeStackedChartData);
		//overPrefBean.setIdMaps(idMaps);
		PreferencesUtil.getInstance().setOverviewPreferenceInSession(
				overPrefBean);

	}

	private void filterModelData() {
		timeStackedChartData = new ArrayList<Map<String, Map<String, Number>>>(
				NUM_DIMENSIONS);
	//	idMaps = new ArrayList<Map<String, Integer>>(NUM_DIMENSIONS);
		for (int i = 0; i < NUM_DIMENSIONS; i++) {
			timeStackedChartData.add(new LinkedHashMap<String, Map<String, Number>>());
		//	idMaps.add(new LinkedHashMap<String, Integer>());
		}

		prepareTimeChartMaps();
		
	}

	/**
	 * Create Map for time chart and apply exclude sql text filter
	 */
	public void prepareTimeChartMaps() {

		//Iterating over model data to prepare maps for duration charts for all dimensions
		for (QueryWaits queryWaits : modelData) {

			// Data for time stacked chart
			SimpleDateFormat sdf = new SimpleDateFormat("MM/dd/yy hh:mm a");
			String key = sdf.format(queryWaits
					.getStatementUTCStartTime());

			QueryChartOptionsEnum chartOption = QueryChartOptionsEnum.WAIT_CATEGORY;

			//Iterating over all dimension's maps to add this record
			//	for (QueryChartOptionsEnum chartOption : QueryChartOptionsEnum.values()) {
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

				//Get id map for this dimension
		//		Map<String, Integer> currentIdMap = idMaps.get(chartOption.getTabId());
		//		currentIdMap.put(keyInnerMap, id);

				//Get chart data for this dimension
				Map<String, Map<String, Number>> chartData = timeStackedChartData.get(chartOption.getTabId()); 
				if (chartData == null) {
					chartData = new LinkedHashMap<String, Map<String, Number>>();
				}

				if (chartData
						.containsKey(key)) {
					Map<String, Number> innerMap = chartData
							.get(key);
					if(keyInnerMap.trim().isEmpty())
						continue;
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
					if(keyInnerMap.trim().isEmpty())
						chartData.put(key, innerMap );
					else{
						innerMap.put(keyInnerMap, com.idera.sqldm.utils.Utility.round(queryWaits
								.getWaitDurationPerSecond(), 2));
						chartData.put(key, innerMap);
					}
				}

				timeStackedChartData.set(chartOption.getTabId(),
						chartData);
		//		idMaps.set(chartOption.getTabId(), currentIdMap);

			} catch (NoSuchMethodException | SecurityException
					| IllegalAccessException | IllegalArgumentException
					| InvocationTargetException e) {
				e.printStackTrace();
			}

			//}
		}
		setTimeChartData(new LinkedHashMap<>(timeStackedChartData.get(getQueryChartOption().getTabId())));
	}
	private void getModelDataQueryWait() {
		final String productInstanceName = Utility.getUrlParameter(Executions
				.getCurrent().getParameterMap(), "instance");

		try {
			modelData = SingleInstanceQueriesFacade.getOverviewQueryWaitsInstances(
					productInstanceName, instanceId, offsetHours.toString(),
					getFromDateTimeAPI(), getEndDateTimeAPI());

		} catch (InstanceException e) {
			e.printStackTrace();
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

	private void setDateTime() {
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		setFromDate(pref.getFromDate());
		setFromTime(pref.getFromTime());
		setEndDate(pref.getToDate());
		setEndTime(pref.getToTime());

	}
	

	/**
	 * Update values of local variables using session object
	 */
	private void updateViewUsingSession(SingleInstanceOverviewPreferenceBean overPrefBean) {
		setModelData(overPrefBean.getQueryWaitsModelData());
		setTimeChartData(new LinkedHashMap<>(overPrefBean.getTimeStackedChartData().get(getQueryChartOption().getTabId())));
	//	setIdMap(new LinkedHashMap<>(overPrefBean.getIdMaps().get(getQueryChartOption().getTabId())));
		setDateTime();
	}

	public void drawCharts() {
		
		if (getTimeGraphDiv() != null) {
			getTimeGraphDiv().setVisible(true);
		}

		loadChartData();
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

		try {

			Set<String> groupByNamesTime = new HashSet<>();
			chartLabelData = "";

			Collection<Map<String, Number>> valueSet = timeChartData.values();
			for (Map<String, Number> map : valueSet) {
				Set<String>keySet = map.keySet();
				for (String key : keySet) {
					if(!groupByNamesTime.contains(key)){
						groupByNamesTime.add(key);
					}
				}
			}

			Set<String> keySets = timeChartData.keySet();
			for (String key : keySets) {
				Map<String, Number> map = timeChartData.get(key);
				for (String label : groupByNamesTime) {
					if (!map.containsKey(label)) {
						map.put(label, 0);
					}
				}
				timeChartData.put(key, map);
			}
			/*for (Map<String, Number> innerMap : timeChartData.values()) {
				for (String label : groupByNamesTime) {
					if (!innerMap.containsKey(label)) {
						innerMap.put(label, 0);
					}
				}
			}*/

		} catch (Exception x) {
			log.error(x.getMessage(), x);
			getStackedBarChartInstance()
			.setErrorMessage(
					ELFunctions
					.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
			}
	}

	private void drawStackedBarChart() {
		//IderaStackedBarChart stackedChart 
		overviewStackedBarChart = getStackedBarChartInstance();
		overviewStackedBarChart.refresh();
		overviewStackedBarChart.setOrient("vertical");
		overviewStackedBarChart.setChartData(timeChartData);
		overviewStackedBarChart.setVisible(true);
		overviewStackedBarChart.getChart().setYAxisTitle("Total Wait Time(milliseconds/second)");

		// Add %s where ever you want bar value to come
		overviewStackedBarChart.getChart().setMouseOverText("Wait Time: %s");
		// Inner padding adds space between bars
		overviewStackedBarChart.getChart().setInnerPadding(0.4);
		// Outer padding adds padding on the outer ticks
		overviewStackedBarChart.getChart().setOuterPadding(0.4);
		overviewStackedBarChart.getChart().setxAxisTicksCount(MAX_XAXIS_LABELS);
		overviewStackedBarChart.getChart().setBarLinks("window.clickBar()");
		if (getStackedBarChartLabel() != null) {
			getStackedBarChartLabel().setValue(chartLabelData);
			getStackedBarChartLabel().setMultiline(true);
		}
		overviewStackedBarChart.setWidth("1110px");
	}

	
	public String getGroupBY(QueryWaits waits) {
		return waits.getWaitCategory().trim();
	}

	public IderaStackedBarChart getStackedBarChartInstance() {
		return overviewStackedBarChart;
	}

	public Div getTimeGraphDiv() {
		return overviewTimeGraphDiv;
	}


	public QueryChartOptionsEnum getQueryChartOption() {
		return QueryChartOptionsEnum.WAIT_CATEGORY;
	}

	public int getGroupById(QueryWaits waits) {
		return waits.getWaitCategoryID();
	}


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
	/*public Map<String, Integer> getIdMap() {
		 return idMap;
	 }

	 public void setIdMap(Map<String, Integer> idMap) {
		 this.idMap = idMap;
	 }*/

}
