package com.idera.sqldm.ui.dashboard.instances.overview;

import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.ProtocolException;
import java.net.URL;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TimeZone;

import org.apache.commons.lang.StringUtils;
import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.Command;
import org.zkoss.util.Pair;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.Button;
import org.zkoss.zul.Caption;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Constraint;
import org.zkoss.zul.Div;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.Image;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModel;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Panel;
import org.zkoss.zul.Panelchildren;
import org.zkoss.zul.Popup;
import org.zkoss.zul.Row;
import org.zkoss.zul.Rows;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Vlayout;
import org.zkoss.zul.Window;
import org.zkoss.zk.ui.event.ForwardEvent;

import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.idera.common.rest.CoreRestClient;
import com.idera.common.rest.RestException;
import com.idera.cwf.model.Product;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.DashboardInstanceFacade;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.SeverityCodeToStringEnum;
import com.idera.sqldm.data.alerts.Alert;
import com.idera.sqldm.data.alerts.AlertCategoriesDetails;
import com.idera.sqldm.data.alerts.AlertException;
import com.idera.sqldm.data.alerts.AlertFacade;
import com.idera.sqldm.data.alerts.AlertCategoriesDetails.AlertCategoriesEnum;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.category.resources.ResourceCategory;
import com.idera.sqldm.data.instances.SWAInstances;
import com.idera.sqldm.data.instances.InstanceActivityOverTimeSWA;
import com.idera.sqldm.data.instances.SWAXaxisLabels;
import com.idera.sqldm.data.instances.SessionGraphDetail;
import com.idera.sqldm.data.prescreptiveanalysis.PrescreptiveAnalysisRecord;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.rest.SQLDMRestClient;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.session.SessionUtil;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.ui.components.charts.IderaChart;
import com.idera.sqldm.ui.components.charts.area.IderaAreaChartModel;
import com.idera.sqldm.ui.components.charts.bar.IderaBarChartModel;
import com.idera.sqldm.ui.components.charts.bar.IderaStackedBarChart;
import com.idera.sqldm.ui.components.charts.bar.IderaTimelineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.dashboard.AnalysisSummary;
import com.idera.sqldm.ui.dashboard.InstanceScaleFactorList;
import com.idera.sqldm.ui.dashboard.RecommendationSummary;
import com.idera.sqldm.ui.dashboard.instances.InstanceCategoryTab;
import com.idera.sqldm.ui.dashboard.instances.queries.SingleInstanceQueriesFacade;
import com.idera.sqldm.ui.dashboard.instances.queryWaits.QueryChartOptionsEnum;
import com.idera.sqldm.ui.dashboard.instances.queryWaits.QueryWaitsFilter;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstanceAlertsPreferencesBean;
import com.idera.sqldm.ui.preferences.SingleInstancePreferencesBean;
import com.idera.sqldm.utils.SQLdmConstants;
import com.idera.sqldm.utils.Utility;

public class SingleInstanceOverviewComposer extends SelectorComposer<Div> {
	private String OFFSET_IN_HOURS = "0.0";
	static final long serialVersionUID = 1L;
	private final static Logger log = Logger.getLogger(SingleInstanceOverviewComposer.class);

	DashboardInstance instance;
	int instanceId;

	// new code

	private List<PrescreptiveAnalysisRecord> paItemList = new ArrayList<>();
	private ListModel<PrescreptiveAnalysisRecord> paRecord;

	@Wire
	private Div paDiv;

	@Wire
	private Panelchildren panelChild;

	@Wire(".paListBoxClass")
	private Listbox paListboxId;

	@Wire
	private Popup widgetSettings;

	@Wire
	private Intbox limit;
	@Wire
	private Button save;

	private int pageSize = 10;
	private int activePage = 0;
	private int totalSize;

	// ----

	@Wire
	protected Label lblInstanceName, lblSqlVersion, lblSqlEdition, lblRunningSince, lblClustered, lblProcessors,
			lblHost, lblHostOS, lblHostMemory, lblInstanceStatus, lblCriticalAlert, lblWarningAlert,
			lblInformationalAlert, lblHealthIndex, lblUserSession, lblAnalysisType, lblStartupTime, lblDuration;

	// @Wire
	// protected Image imgInstanceStatus;
	@Wire
	protected Label lblServerService, lblAgentService, lblDtcService, lblAllocDataFile, lblUsedDataFile,
			lblAllocLogFile, lblUsedLogFile, lblDbCount;

	@Wire
	protected Groupbox serverPropertiesGroup, serviceStatusGroup, fileUsedGroup;

	@Wire
	protected Caption serverPropertiesCaption, serviceStatusCaption, fileUsedCaption;
	private Date fromDate;
	private Date endDate;
	private Date fromTime;
	private Date endTime;

	private Double offsetHours;
	private List<Alert> modelData;
	private ListModelList<AlertCategoriesDetails> categoryOptions;
	private Map<Integer, Integer> metricSeverityMap = new HashMap<Integer, Integer>();
	private Map<String, List<Pair<String, Number>>> categoryMaps = new HashMap<String, List<Pair<String, Number>>>();
	private Map<String, IderaTimelineChart> categoryCharts = new HashMap<>();
	private List<Pair<String, Number>> innerMap;
	private boolean historyEvent = false;
	@Wire
	protected Vlayout categoryChartsVlayout;
	@Wire
	protected IderaTimelineChart timelineChart;
	@Wire
	protected Div CategoryChartsContainerDiv;
	private ListModel<RecommendationSummary> recommendationSummaryModel;
	@Wire
	private Grid recommendationSummaryGrid;
	@Wire
	protected IderaStackedBarChart queryChart;
	@Wire
	protected Popup editOptionsPopup;

	private List<SessionGraphDetail> sessionDetails;
	private Map<String, Map<String, Number>> swaDataFormat;

	private Map<QueryChartOptionsEnum, QueryWaitsFilter> filters = new LinkedHashMap<>();
	@Wire
	private Button editSaveButton;
	@Wire
	private Button editCancelButton;
	@Wire
	private Div queryChartDiv;
	@Wire
	private Div processorTimeChartWidget;
	@Wire
	private Div launchAlertScreen;
	@Wire
	private Div launchDMConsole;
	@Wire
	private Label divHostOS;
	@Wire
	private Label divHost;
	@Wire
	private Div TimelineChartContainerDiv;
	@Wire
	protected Popup manageGraphPopup;
	@Wire
	protected Grid categoryList;
	@Wire
	private Div showQueryWaitGraph;
	private ListModelList<AlertCategoriesDetails> graphCategoryOptions;

	/*
	 * @Wire protected Image imgInstanceStatus;
	 */

	private static final String OPEN_TRUE = "open-true";
	private static final String OPEN_FALSE = "open-false";

	@Override
	public void doAfterCompose(Div comp) throws Exception {
		setOffSet();
		super.doAfterCompose(comp);

		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "id");
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		} else {
			// fallback
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
			if (param != null) {
				instanceId = (Integer) param;
			}
		}
		Executions.getCurrent().getDesktop().setAttribute("instanceId", instanceId);

		String productInstanceName = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		
		try {
			instance = DashboardInstanceFacade.getDashboardInstance(productInstanceName, instanceId);
		} catch (InstanceException x) {
			log.error(x.getMessage(), x);
		}

		if (instance == null || instance.getOverview() == null) {
			Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("dashboard"));
			return;
		}
		
		List<PrescreptiveAnalysisRecord> response = SQLDMRestClient.getInstance()
				.getPrescreptiveAnalysisForInstance(Integer.toString(instanceId));

		PrescreptiveAnalysisRecord record = null;
		for (PrescreptiveAnalysisRecord prescreptiveAnalysisRecord : response) {

			record = new PrescreptiveAnalysisRecord();

			String analysisStartTime = prescreptiveAnalysisRecord.getAnalysisStartTime();
			String[] dateStringArrayOne = analysisStartTime.split("\\(");
			
			Long longDate = System.currentTimeMillis();
			
			String currentDate = ""+longDate.toString();
			
			String[] dateStrignArrayTwo = {currentDate};
			
			if(dateStringArrayOne[1].contains("+")) {
				dateStrignArrayTwo = dateStringArrayOne[1].split("\\+"); 
			} else if(dateStringArrayOne[1].contains("-")) {
				dateStrignArrayTwo = dateStringArrayOne[1].split("\\-"); 
			}
			
			Long dateInLongFormat = Long.parseLong(dateStrignArrayTwo[0]);
			Date date = new Date(dateInLongFormat);
			SimpleDateFormat df2 = new SimpleDateFormat("dd MMMM yyyy HH:mm");
			String dateText = df2.format(date);

			record.setAnalysisStartTime(dateText);
			record.setAnalysisID(prescreptiveAnalysisRecord.getAnalysisID());
			record.setAnalysisDuration(prescreptiveAnalysisRecord.getAnalysisDuration());
			record.setType(prescreptiveAnalysisRecord.getType());
			record.setTotalRecommendationCount(prescreptiveAnalysisRecord.getTotalRecommendationCount());
			record.setComputedRankFactor(prescreptiveAnalysisRecord.getComputedRankFactor());

			paItemList.add(record);

		}

		paRecord = new ListModelList<PrescreptiveAnalysisRecord>(paItemList);
		((ListModelList<PrescreptiveAnalysisRecord>) paRecord).setMultiple(true);
		log.info("PA list size:" + paRecord.getSize());
		paListboxId.setModel(paRecord);

		String notAvailableTxt = ELFunctions.getMessage(SQLdmI18NStrings.N_A);
		offsetHours = Double.parseDouble(Utility.setOffSet());
		lblInstanceName.setValue((instance.getOverview().getInstanceName() == null) ? notAvailableTxt
				: instance.getOverview().getInstanceName());
		// @author Saumyadeep
		lblUserSession.setValue("" + instance.getUserSessions());
		log.debug("severity = " + instance.getServerStatus().getMaxSeverity());

		if (instance.getServerStatus() != null && instance.getServerStatus().getMaxSeverity() != null) {
			SeverityCodeToStringEnum sctse = SeverityCodeToStringEnum
					.getSeverityEnumForId(instance.getServerStatus().getMaxSeverity());
			// imgInstanceStatus.setSrc(ELFunctions.getImageURLWithoutSize(sctse.getServerCat16PixIcon()));
			// lblInstanceStatus.setValue(sctse.getUiLabel());
			lblHealthIndex.setValue("" + instance.getServerStatus().getHealthIndex() + "%");
			log.debug("sctse.getUiLabel() = " + sctse.getUiLabel());
		}

		String lblSqlVer = notAvailableTxt;
		Boolean verNotAvailable = true;
		if (StringUtils.isNotEmpty(instance.getOverview().getProductVersion())
				&& !"N/A".equals(instance.getOverview().getProductVersion())) {
			lblSqlVer = Utility.getSQLServerRelease(instance.getOverview().getProductVersion()) + " ("
					+ instance.getOverview().getProductVersion() + ")";
			verNotAvailable = false;
		}
		int start = instance.getOverview().getProductVersion().indexOf(".");
		if (start != -1) {
			String majorVersion = instance.getOverview().getProductVersion().substring(0,
					instance.getOverview().getProductVersion().indexOf("."));
			if (Integer.parseInt(majorVersion) <= 8) {
				launchDMConsole.setVisible(false);
			}
		}

		lblSqlVersion.setValue(lblSqlVer);
		lblSqlEdition.setValue((instance.getOverview().getSqlServerEdition() == null) ? notAvailableTxt
				: instance.getOverview().getSqlServerEdition());
		lblRunningSince.setValue((verNotAvailable || instance.getOverview().getRunningSince() == null) ? notAvailableTxt
				: Utility.getLocalDateString(instance.getOverview().getRunningSince()));
		lblClustered.setValue((verNotAvailable || instance.getOverview().getIsClustered() == null) ? notAvailableTxt
				: (instance.getOverview().getIsClustered() ? ELFunctions.getMessage(SQLdmI18NStrings.YES)
						: ELFunctions.getMessage(SQLdmI18NStrings.NO)));
		lblProcessors.setValue((verNotAvailable || instance.getOverview().getProcessorCount() == null) ? notAvailableTxt
				: (instance.getOverview().getProcessorsUsed() + " of " + instance.getOverview().getProcessorCount()
						+ " used"));
		lblHost.setValue((verNotAvailable || instance.getOverview().getServerHostName() == null) ? notAvailableTxt
				: instance.getOverview().getServerHostName());
		divHost.setValue(lblHost.getValue());
		lblHostOS.setValue((verNotAvailable || instance.getOverview().getWindowsVersion() == null) ? notAvailableTxt
				: instance.getOverview().getWindowsVersion());
		divHostOS.setValue(lblHostOS.getValue());

		boolean hasOsMetricsStatistics = instance.getOverview().getOsMetricsStatistics() != null;

		if (instance.getServerStatus().isNull())
			lblHostMemory.setValue(notAvailableTxt);
		else if (hasOsMetricsStatistics
				&& instance.getOverview().getOsMetricsStatistics().getTotalPhysicalMemory() != null) {
			try {
				double mem = Double
						.parseDouble(instance.getOverview().getOsMetricsStatistics().getTotalPhysicalMemory());
				mem = Utility.round(mem / (1024 * 1024), 2);
				lblHostMemory.setValue(mem + "");
			} catch (Exception e) {

			}
		}
		if (instance.getServerStatus() == null)
			lblAllocDataFile.setValue(notAvailableTxt);
		else if (hasOsMetricsStatistics
				&& instance.getOverview().getOsMetricsStatistics().getTotalPhysicalMemory() != null) {
			try {
				double mem = Double.parseDouble(instance.getOverview().getDatabaseSummary().getDataFileSpaceAllocated())
						/ (1024 * 1024);
				mem = Utility.round(mem, 2);
				lblAllocDataFile.setValue(mem + " GB");
			} catch (Exception e) {

			}
		}
		if (instance.getServerStatus() == null)
			lblUsedDataFile.setValue(notAvailableTxt);
		else if (hasOsMetricsStatistics
				&& instance.getOverview().getOsMetricsStatistics().getTotalPhysicalMemory() != null) {
			try {
				double mem = Double.parseDouble(instance.getOverview().getDatabaseSummary().getDataFileSpaceUsed())
						/ (1024 * 1024);
				mem = Utility.round(mem, 2);
				lblUsedDataFile.setValue(mem + " GB");
			} catch (Exception e) {

			}
		}
		if (instance.getServerStatus() == null)
			lblAllocLogFile.setValue(notAvailableTxt);
		else if (hasOsMetricsStatistics
				&& instance.getOverview().getOsMetricsStatistics().getTotalPhysicalMemory() != null) {
			try {
				double mem = Double.parseDouble(instance.getOverview().getDatabaseSummary().getLogFileSpaceAllocated())
						/ (1024 * 1024);
				mem = Utility.round(mem, 2);
				lblAllocLogFile.setValue(mem + " GB");
			} catch (Exception e) {

			}
		}
		if (instance.getServerStatus() == null)
			lblUsedLogFile.setValue(notAvailableTxt);
		else if (hasOsMetricsStatistics
				&& instance.getOverview().getOsMetricsStatistics().getTotalPhysicalMemory() != null) {
			try {
				double mem = Double.parseDouble(instance.getOverview().getDatabaseSummary().getLogFileSpaceUsed())
						/ (1024);
				mem = Utility.round(mem, 4);
				lblUsedLogFile.setValue(mem + " MB");
			} catch (Exception e) {

			}
		}
		categoryOptions = new ListModelList<>(AlertCategoriesEnum.getDefaultList());

		SingleInstanceAlertsPreferencesBean sessionBean = PreferencesUtil.getInstance()
				.getSingleInstanceAlertsPreferenceInSession(instanceId);
		if (sessionBean.getFromDate() == null) {
			if (sessionBean.getCategoryOptions() == null) {
				// categoryOptions = new ListModelList<>(AlertCategoriesEnum.getDefaultList());
				sessionBean.setCategoryOptions(categoryOptions);
				PreferencesUtil.getInstance().setSingleInstanceAlertsPreferenceInSession(sessionBean);
			} else
				categoryOptions = sessionBean.getCategoryOptions();
			categoryList.setModel(categoryOptions);
			setHistoryPanelValues();
		} else {
			if (sessionBean.getCategoryOptions() != null)
				categoryList.setModel(sessionBean.getCategoryOptions());
			else
				categoryList.setModel(categoryOptions);
			updateViewUsingSession();
			refreshView(true);
		}
		AnalysisSummary analysisSummaryObj = SQLDMRestClient.getInstance().getAnalysisSummary(productInstanceName,
				instanceId);
		if (analysisSummaryObj.getPreviousAnalysisInformation() != null) {
			lblAnalysisType.setValue(
					(analysisSummaryObj.getPreviousAnalysisInformation().getAnalysisType() == null) ? notAvailableTxt
							: analysisSummaryObj.getPreviousAnalysisInformation().getAnalysisType());
			lblDuration.setValue(
					(analysisSummaryObj.getPreviousAnalysisInformation().getDuration() == null) ? notAvailableTxt
							: analysisSummaryObj.getPreviousAnalysisInformation().getDuration());
			Date startTime = analysisSummaryObj.getPreviousAnalysisInformation().getStartedOn();
			String startTimeString;
			if (startTime != null) {
				SimpleDateFormat sdf = new SimpleDateFormat("EEE, MMM d, yyyy hh:mm:ss a");
				sdf.setTimeZone(TimeZone.getTimeZone("IST"));
				startTimeString = sdf.format(startTime);
			} else {
				startTimeString = notAvailableTxt;
			}
			lblStartupTime.setValue(startTimeString);
		}
		if (analysisSummaryObj.getRecommendationSummaryList() != null) {
			List<RecommendationSummary> recommendationSummaryList = analysisSummaryObj.getRecommendationSummaryList();
			recommendationSummaryModel = new ListModelList<RecommendationSummary>(recommendationSummaryList);
			System.out.println("" + recommendationSummaryList.size());
			recommendationSummaryGrid.setModel(recommendationSummaryModel);
		}
		/*
		 * lblServerService .setValue((instance.getOverview().getSqlServiceStatus() ==
		 * null) ? notAvailableTxt : instance.getOverview().getSqlServiceStatus());
		 * lblAgentService.setValue((instance.getOverview() .getAgentServiceStatus() ==
		 * null) ? notAvailableTxt : instance .getOverview().getAgentServiceStatus());
		 * lblDtcService .setValue((instance.getOverview().getDtcServiceStatus() ==
		 * null) ? notAvailableTxt : instance.getOverview().getDtcServiceStatus());
		 * lblDbCount .setValue((instance.getOverview().getDatabaseSummary() == null ||
		 * instance .getOverview().getDatabaseSummary().getDatabaseCount() == null) ?
		 * notAvailableTxt : instance.getOverview().getDatabaseSummary()
		 * .getDatabaseCount() + "");
		 */
		setServerStatusSection(productInstanceName);

		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance()
				.getSingleInstancePreferencesInSession(instanceId);
		pref.setSelectedCategory(0);
		pref.setSelectedSubCategory(1);
		PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);
		EventQueue<Event> eq1 = EventQueues.lookup("historyChange1", EventQueues.DESKTOP, true);
		eq1.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {
				refreshGraphs();
			}
		});
		EventQueue<Event> eq2 = EventQueues.lookup("overviewHistory", EventQueues.DESKTOP, true);
		eq2.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {
				refreshChart();
			}
		});
		drawChart();

	}

	public ListModel<RecommendationSummary> getRecommendationSummaryModel() {
		return recommendationSummaryModel;
	}

	public void setRecommendationSummaryModel(ListModel<RecommendationSummary> recommendationSummaryModel) {
		this.recommendationSummaryModel = recommendationSummaryModel;
	}

	// ====================alerts graph integration=========================
	protected void refreshChart() throws CategoryException, RestException, IOException {
		if (historyEvent) {
			historyEvent = false;
			setHistoryPanelValues();
			drawChart();
		}
	}

	@SuppressWarnings("unchecked")
	private void drawChart() throws IOException, RestException {
		try {
			InstanceActivityOverTimeSWA swaData = getInstanceActivityOverTimeFromSWA();
			if (swaData == null) {
				processorTimeChartWidget.setVisible(false);
				return;
			} else {
				showQueryWaitGraph.setVisible(false);
			}
			Map<String, Map<String, Number>> swaDataFormat = createSwaData(swaData);
			IderaBarChartModel swaModel = new IderaBarChartModel();
			swaDataFormat = setXAxisDomain(swaDataFormat);
			queryChart.refresh();
			if ((swaDataFormat != null) && (!swaDataFormat.isEmpty())) {

				this.queryChart.setOrient("vertical");
				this.queryChart.setChartData(swaDataFormat);
				this.queryChart.setVisible(true);
				// this.queryChart.getChart().setMouseOverText(filter.getSelectedView().getMetricName()
				// + ": %s");

				// Inner padding adds space between bars
				this.queryChart.getChart().setInnerPadding(0.2);
				this.queryChart.getChart().setDrillable(true);

				// Outer padding adds padding on the outer ticks
				this.queryChart.getChart().setOuterPadding(0.2);
				// Map<String, String> legendLinks = createLegendsLinks(swaData);
				// this.queryChart.getChart().setLegendLinks(legendLinks);
				this.queryChart.getChart().setYAxisTitle("Seconds");

				// this.queryChart.getChart().setAxisLinks(axisLinks);
				this.queryChart.getChart().setAnimateBars(true);
				this.queryChart.getChart().setAnimationDelay(100);
				this.queryChart.getChart().setAnimationLength(500);

			} else {

				setEmptyCharts();
			}
		} catch (Exception swaException) {
			setEmptyCharts();
			processorTimeChartWidget.setVisible(false);
			log.error("Error In Getting InstanceActivityOverTime Data from SWA");
		}

	}

	private Map<String, Map<String, Number>> setXAxisDomain(Map<String, Map<String, Number>> ChartData) {
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		Date fromDateTime = pref.getFromDateTime();
		Date toDateTime = pref.getToDateTime();
		List<String> dummyString = new ArrayList<String>();
		Map<String, Map<String, Number>> modifiedChartData = new LinkedHashMap<String, Map<String, Number>>();
		Collection<Map<String, Number>> dataVal = ChartData.values();
		Iterator<Map<String, Number>> itr = dataVal.iterator();
		while (itr.hasNext()) {
			Map<String, Number> v1 = itr.next();
			Set<String> keys = v1.keySet();
			if (keys.size() != 0) {
				Iterator<String> itrSTr = keys.iterator();
				while (itrSTr.hasNext()) {
					dummyString.add(itrSTr.next());
				}
			}
			break;
		}

		SimpleDateFormat sdf = new SimpleDateFormat("E MMM dd yyyy HH:mm:ss ");

		if (!dummyString.isEmpty()) {
			Map<String, Number> minValue = new HashMap<String, Number>();
			for (int i = 0; i < dummyString.size(); i++) {
				minValue.put(dummyString.get(i), 0.0);
				String key = sdf.format(fromDateTime);
				modifiedChartData.put(key, minValue);
			}
			modifiedChartData.putAll(ChartData);
			Map<String, Number> maxValue = new HashMap<String, Number>();
			for (int i = 0; i < dummyString.size(); i++) {
				maxValue.put(dummyString.get(i), 0.0);
				String key = sdf.format(toDateTime);
				modifiedChartData.put(key, maxValue);
			}
		}

		return modifiedChartData;
		/*
		 * chart.getChart().setxAxisCustomMinDomainValue(true);
		 * chart.getChart().setxAxisCustomMaxDomainValue(true);
		 * chart.getChart().setxAxisMinDomainValue(fromDateTime);
		 * chart.getChart().setxAxisMaxDomainValue(toDateTime);
		 */
	}

	private void setEmptyCharts() {
		queryChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
	}

	/*
	 * Author:Accolite Date : 24th Nov, 2016 Consolidated Overview - SQLDM- 10.2
	 * release
	 */
	@Listen("onClick = #queryChartDiv")
	public void launchSWA() throws IOException, RestException {
		try {
			int swaID = instance.getSwaID();
			String instanceName = instance.getOverview().getInstanceName();
			boolean checkSWAUninstalled = true;
			List<Product> products = CoreRestClient.getInstance().getAllProducts();
			for (int noOfProducts = 0; noOfProducts < products.size(); noOfProducts++) {
				Product product = products.get(noOfProducts);
				if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("SQLWorkloadAnalysis")) {
					if (swaID == product.getProductId()) {
						checkSWAUninstalled = false;
						break;
					}
				}
			}
			if (checkSWAUninstalled)
				Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("/home"));
			else
				Executions.sendRedirect(SQLDMRestClient.getInstance().getSWAurl(swaID, instanceName));
		} catch (Exception e) {
			log.error("Error In Launching SWA : " + e.getMessage());
			Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("/home"));
		}
	}

	@Listen("onClick = #launchAlertScreen,#CategoryChartsContainerDiv")
	public void launchAlertScreen() throws IOException, RestException {
		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance()
				.getSingleInstancePreferencesInSession(instanceId);
		pref.setSelectedCategory(5);
		PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

		Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance" + "/" + instanceId));
	}

	@Listen("onClick = #launchToSession")
	public void launchToSession() throws IOException, RestException {
		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance()
				.getSingleInstancePreferencesInSession(instanceId);
		pref.setSelectedCategory(InstanceCategoryTab.SESSIONS.getId());
		pref.setSelectedSubCategory(1);
		PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

		Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance" + "/" + instanceId));
	}

	@Listen("onClick = #launchToQueries")
	public void launchToQueries() throws IOException, RestException {
		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance()
				.getSingleInstancePreferencesInSession(instanceId);
		pref.setSelectedCategory(InstanceCategoryTab.QUERIES.getId());
		pref.setSelectedSubCategory(1);
		PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

		Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance" + "/" + instanceId));
	}

	@Listen("onClick = #launchToResources")
	public void launchToResources() throws IOException, RestException {
		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance()
				.getSingleInstancePreferencesInSession(instanceId);
		pref.setSelectedCategory(InstanceCategoryTab.RESOURCES.getId());
		pref.setSelectedSubCategory(2);
		PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

		Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance" + "/" + instanceId));
	}

	@Listen("onClick = #launchToDatabases")
	public void launchToDatabases() throws IOException, RestException {
		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance()
				.getSingleInstancePreferencesInSession(instanceId);
		pref.setSelectedCategory(InstanceCategoryTab.DATABASES.getId());
		pref.setSelectedSubCategory(1);
		PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

		Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance" + "/" + instanceId));
	}

	@Listen("onClick = #showQueryWaitGraph")
	public void launchToQueryWait() throws IOException, RestException {
		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance()
				.getSingleInstancePreferencesInSession(instanceId);
		pref.setSelectedCategory(InstanceCategoryTab.QUERY_WAITS.getId());
		pref.setSelectedSubCategory(1);
		PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

		Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance" + "/" + instanceId));
	}

	@Listen("onClick = #launchDMConsole")
	public void launchDMConsole() throws IOException, RestException {
		Executions.sendRedirect("Idera://analysis?instanceid=" + instanceId);
	}

	private Map<String, String> createLegendsLinks(InstanceActivityOverTimeSWA swaData) {
		List<SWAXaxisLabels> labels = swaData.getLabels();
		Map<String, String> legendLinks = new LinkedHashMap<String, String>();
		for (int i = 0; i < labels.size(); i++) {
			legendLinks.put(labels.get(i).getLabel(), "");
		}
		return legendLinks;
	}

	private Map<String, Map<String, Number>> createSwaData(InstanceActivityOverTimeSWA callSWAapi) {
		Map<String, Map<String, Number>> barChart = new LinkedHashMap<String, Map<String, Number>>();
		Map<String, Number> oneBar = new LinkedHashMap<String, Number>();
		List<SWAXaxisLabels> labels = callSWAapi.getLabels();
		List<String> xAxis = callSWAapi.getxAxisTicks();
		List<List<String>> data = callSWAapi.getData();
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		Date startTime = pref.getFromDateTime();
		Date endTime = pref.getToDateTime();
		SimpleDateFormat parseDateFormat = new SimpleDateFormat("dd MMM yyyy HH:mm:ss");
		SimpleDateFormat sdf = new SimpleDateFormat("E MMM dd yyyy HH:mm:ss ");
		for (int i = 0; i < xAxis.size(); i++) {
			oneBar = createBar(labels, data, i);
			String dateSWA = data.get(data.size() - 1).get(i);
			// From date and Time
			Date swaDate = null;
			String key = null;
			try {
				swaDate = (Date) parseDateFormat.parse(dateSWA);
				if (((startTime.compareTo(swaDate) > 0) || (endTime.compareTo(swaDate) < 0))) {
					continue;
				}
				key = sdf.format(swaDate);
			} catch (ParseException e) {
				log.error("Parse Exception while parsing the date received from SWA");
			}
			barChart.put(key, oneBar);
		}
		return barChart;

	}

	private Map<String, Number> createBar(List<SWAXaxisLabels> labels, List<List<String>> data, int labelIndex) {
		Map<String, Number> oneBar = new LinkedHashMap<String, Number>();
		for (int i = 0; i < labels.size() - 1; i++) {
			Number x;
			if (data.get(i).get(labelIndex).equals("")) {
				x = 0;
			} else {
				x = Double.parseDouble(data.get(i).get(labelIndex));
			}
			String label = "";
			switch (labels.get(i).getLabel()) {
			case "INTERNAL_WAIT_SUM":
				label = "Internal Wait";
				break;
			case "LOG_WAIT_SUM":
				label = "Log Wait";
				break;
			case "LOCK_WAIT_SUM":
				label = "Lock Wait";
				break;
			case "TEMP_DB_WAIT_SUM":
				label = "Tempdb I/O Wait";
				break;
			case "WAIT_FOR_CPU_SUM":
				label = "OS Wait";
				break;
			case "USING_CPU_SUM":
				label = "Using CPU";
				break;
			case "IO_WAIT_SUM":
				label = "I/O Wait";
				break;
			case "NET_IO_WAIT_SUM":
				label = "Network I/O Wait";
				break;
			case "REMOTE_WAIT_SUM":
				label = "Remote Wait";
				break;
			case "CLR_WAIT_SUM":
				label = "CLR Wait";
				break;
			}
			oneBar.put(label, x);
		}
		return oneBar;

	}

	/*
	 * Author:Accolite Date : 24th Nov, 2016 Consolidated Overview - SQLDM- 10.2
	 * release GetInstanceActivityOverTime data from SWA
	 */
	private InstanceActivityOverTimeSWA getInstanceActivityOverTimeFromSWA() throws IOException, RestException {
		List<Product> products = CoreRestClient.getInstance().getAllProducts();
		String requestUrl = null;
		Calendar c = Calendar.getInstance();
		Double offSet = null;
		long startTimeLong = 0;
		long endTimeLong = 0;
		if (Sessions.getCurrent() != null) {
			offSet = new Double((Integer) Sessions.getCurrent().getAttribute(WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))
					/ (1000 * 60.0 * 60.0);
			offSet = -offSet;
		}
		Double offsetHours;
		if (offSet != null)
			offsetHours = Double.parseDouble(offSet.toString());
		else
			offsetHours = Double.parseDouble("0.0");
		c.setTime(new Date(c.getTimeInMillis() - (long) (offsetHours * 60 * 60 * 1000)));
		long offsetSWA = -(long) (offsetHours * 60 * 60 * 1000);
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		Date startTime = pref.getFromDateTime();
		Date endTime = pref.getToDateTime();
		startTimeLong = startTime.getTime() - offsetSWA;
		endTimeLong = endTime.getTime() - offsetSWA;
		String swaInstanceId = "0";
		for (int noOfProducts = 0; noOfProducts < products.size(); noOfProducts++) {
			Product product = products.get(noOfProducts);
			if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("SQLWorkloadAnalysis")) {
				requestUrl = product.getLocation() + "/gui/GetDataServlet";
				String isSwaInstance = searchForInstanceInSWA(requestUrl, startTimeLong, offsetSWA, endTimeLong);
				if (!isSwaInstance.equals("0")) {
					swaInstanceId = isSwaInstance;
					break;
				}
			}
		}
		if (swaInstanceId.equals("0")) {
			return null;
		}
		ObjectMapper mapper = new ObjectMapper();
		mapper.configure(DeserializationFeature.ACCEPT_EMPTY_STRING_AS_NULL_OBJECT, true);
		URL myURL = new URL(requestUrl);
		HttpURLConnection myURLConnection = (HttpURLConnection) myURL.openConnection();
		myURLConnection.setDoInput(true);
		myURLConnection.setDoOutput(true);
		myURLConnection.setRequestMethod("POST");
		myURLConnection.setRequestProperty("Accept", "application/json");
		myURLConnection.setRequestProperty("Content-Type", "application/json; charset=UTF-8");
		myURLConnection.setRequestProperty("dataType", "chart");
		myURLConnection.setRequestProperty("from-time", "" + startTimeLong);
		myURLConnection.setRequestProperty("gmt-offset", "" + offsetSWA);
		myURLConnection.setRequestProperty("INSTANCE_ID", swaInstanceId);
		myURLConnection.setRequestProperty("method", "GetInstanceActivityOverTime");
		myURLConnection.setRequestProperty("to-time", "" + endTimeLong);
		if (myURLConnection.getResponseCode() == HttpURLConnection.HTTP_PROXY_AUTH) {
			return null;
		}
		InstanceActivityOverTimeSWA swaData = mapper.readValue(myURLConnection.getInputStream(),
				InstanceActivityOverTimeSWA.class);
		myURLConnection.disconnect();
		return swaData;

	}

	@Listen("onClickSelectedAnalysis = #paListboxId")
	public void onClickSelectedAnalysis(ForwardEvent event) {
		SessionUtil.getSecurityContext();
		Map<Object, Object> args = new HashMap<Object, Object>();
		
		Integer analysisID = (Integer) event.getData();
		
		args.put("analysisID", analysisID);
		args.put("instanceId", instanceId);
		
		Window window = (Window) Executions
				.createComponents(
						"~./sqldm/com/idera/sqldm/ui/prescreptiveAnalysis/prescreptiveAnalysisDetails.zul",
						null, args);
		window.doModal();
	}

	/*
	 * Author:Accolite Date : 24th Nov, 2016 Consolidated Overview - SQLDM- 10.2
	 * release Checks whether the instance is added in SWA or not
	 */
	private String searchForInstanceInSWA(String requestUrl, long startTimeLong, long offsetSWA, long endTimeLong)
			throws IOException {
		String instanceName = instance.getOverview().getInstanceName();
		ObjectMapper mapper = new ObjectMapper();
		mapper.configure(DeserializationFeature.ACCEPT_EMPTY_STRING_AS_NULL_OBJECT, true);
		URL myURL = new URL(requestUrl);
		HttpURLConnection myURLConnection = (HttpURLConnection) myURL.openConnection();
		myURLConnection.setDoInput(true);
		myURLConnection.setDoOutput(true);
		myURLConnection.setRequestMethod("POST");
		myURLConnection.setRequestProperty("Accept", "application/json");
		myURLConnection.setRequestProperty("Content-Type", "application/json; charset=UTF-8");
		myURLConnection.setRequestProperty("dataType", "table");
		myURLConnection.setRequestProperty("from-time", "" + startTimeLong);
		myURLConnection.setRequestProperty("gmt-offset", "" + offsetSWA);
		myURLConnection.setRequestProperty("method", "GetInstancesStatus");
		myURLConnection.setRequestProperty("to-time", "" + endTimeLong);
		if (myURLConnection.getResponseCode() == HttpURLConnection.HTTP_PROXY_AUTH) {
			return "0";
		}
		SWAInstances swaData[] = mapper.readValue(myURLConnection.getInputStream(), SWAInstances[].class);

		for (int i = 0; i < swaData.length; i++) {
			SWAInstances swaInstance = swaData[i];
			if (swaInstance.getInstanceName().equals(instanceName)) {
				return swaInstance.getInstanceId();
			}
		}
		return "0";
	}

	protected void refreshGraphs() {
		historyEvent = true;
	}

	public void setCategoryOptions(ListModelList<AlertCategoriesDetails> categoryOptions) {
		this.categoryOptions = categoryOptions;
		BindUtils.postNotifyChange(null, null, this, "categoryOptions");
	}

	/*
	 * ChaitanyaTanwar DM 10.2 Setting the values of history panel controls to the
	 * variables of this class
	 */
	public void setHistoryPanelValues() {

		HistoryPanelPreferencesBean prefHistory = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		setEndDate(prefHistory.getToDate());
		setEndTime(prefHistory.getToTime());
		setFromDate(prefHistory.getFromDate());
		setFromTime(prefHistory.getFromTime());

		refreshView(false);

	}

	/**
	 * Updates local values using those stored in session
	 */
	private void updateViewUsingSession() {

		SingleInstanceAlertsPreferencesBean sessionBean = PreferencesUtil.getInstance()
				.getSingleInstanceAlertsPreferenceInSession(instanceId);
		setModelData(sessionBean.getModelData());
		setFromDate(sessionBean.getFromDate());
		setFromTime(sessionBean.getFromTime());
		setEndDate(sessionBean.getEndDate());
		setEndTime(sessionBean.getEndTime());
		if (sessionBean.getCategoryOptions() != null)
			setCategoryOptions(sessionBean.getCategoryOptions());

	}

	/**
	 * Refresh view using API call if value of sessionFlag is false or using model
	 * data stored in session if this flag is true
	 * 
	 * @param sessionsFlag
	 */
	private void refreshView(boolean sessionsFlag) {
		if (!sessionsFlag)
			getModelDataAPI();
		updateMaps();
		updateCharts();
	}

	/**
	 * This method will make API call to get data
	 */
	private void getModelDataAPI() {

		final String productInstanceName = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(),
				"instance");

		try {

			modelData = AlertFacade.getAllAlerts(productInstanceName, false, getFromDateTime(), getEndDateTime(), null,
					instanceId, null, -1, null, null, offsetHours.toString());

			if (modelData != null) {
				// Filter model data according to the visibility of categories
				for (int i = modelData.size() - 1; i >= 0; i--) {

					Alert alert = modelData.get(i);
					String category = alert.getMetric().getMetricCategory();
					if (category.equals("Resources"))
						category = AlertCategoriesEnum.returnCategoryString(alert.getMetric().getMetricId());

					AlertCategoriesEnum categoryEnum = AlertCategoriesEnum.returnCategoryEnum(category);
					if (!getCategoryVisibility(categoryEnum))
						modelData.remove(i);

				}

				// Publish event to refresh list data
				publishEvent();

				// Sort model data on the basis of collection time and severity
				Collections.sort(modelData, new Comparator<Alert>() {
					@Override
					public int compare(Alert o1, Alert o2) {
						int c = o1.getUtcUpdated().compareTo(o2.getUtcUpdated());
						if (c != 0)
							return c;
						return o2.getSeverity().compareTo(o1.getSeverity());
					}
				});
			}
		} catch (AlertException e) {
			e.printStackTrace();
		}
	}

	/**
	 * Publish event handled in AlertListComposer
	 */
	public void publishEvent() {

		EventQueue<Event> eq = EventQueues.lookup("updateModel", EventQueues.DESKTOP, false);
		if (eq != null) {

			updateSession();
			eq.publish(new Event("updateModel", null, null));

		}

	}

	/**
	 * Updates the values stored in session
	 */
	private void updateSession() {

		SingleInstanceAlertsPreferencesBean sessionBean = PreferencesUtil.getInstance()
				.getSingleInstanceAlertsPreferenceInSession(instanceId);

		sessionBean.setModelData(modelData);
		sessionBean.setFromDate(fromDate);
		sessionBean.setFromTime(fromTime);
		sessionBean.setEndDate(endDate);
		sessionBean.setEndTime(endTime);
		sessionBean.setCategoryOptions(categoryOptions);
		PreferencesUtil.getInstance().setSingleInstanceAlertsPreferenceInSession(sessionBean);

	}

	/**
	 * Returns the visibility of the passed Alert Category to filter the model data
	 * 
	 * @param ace
	 * @return
	 */
	private boolean getCategoryVisibility(AlertCategoriesEnum ace) {
		for (AlertCategoriesDetails acd : categoryOptions) {
			if (acd.getCategory().equals(ace))
				return acd.isVisible();
		}
		return false;
	}

	/**
	 * Iterates over the model data and updates the maps used by timeline charts for
	 * all categories
	 */
	private void updateMaps() {

		innerMap = new ArrayList<>();
		if (modelData == null)
			return;

		categoryMaps = new HashMap<String, List<Pair<String, Number>>>();
		metricSeverityMap = new HashMap<Integer, Integer>();

		int listSize = modelData.size();
		long timeDifference, timeDifferenceCategory;
		int i;
		Alert alert;

		Map<String, Alert> lastCategoryAlertMap = new HashMap<>();
		int firstSeverity = SeverityCodeToStringEnum.OK.getId();
		Alert lastAlert = null;

		for (i = 0; i < listSize; i++) {

			String currentAlertSeverity;
			alert = modelData.get(i);
			metricSeverityMap.put(alert.getMetric().getMetricId(), alert.getSeverity());

			// In case of first alert use Start date and time
			if (lastAlert == null) {
				timeDifference = alert.getUtcUpdated().getTime() - getFromDateTime().getTime();
				currentAlertSeverity = SeverityCodeToStringEnum.getSeverityEnumForId(alert.getSeverity()).getLabel();
				lastAlert = alert;
			} else {
				timeDifference = alert.getUtcUpdated().getTime() - lastAlert.getUtcUpdated().getTime();
				currentAlertSeverity = SeverityCodeToStringEnum.getSeverityEnumForId(getAlertSeverity(null)).getLabel();
			}

			String category = alert.getMetric().getMetricCategory();
			// If category of current alert is 'Resources', use metric id to get
			// sub-category
			if (category.equals("Resources"))
				category = AlertCategoriesEnum.returnCategoryString(alert.getMetric().getMetricId());

			Alert categoryAlert = lastCategoryAlertMap.get(category);
			String currentCategorySeverity;

			// In case of first alert of a particular category use Start date
			// and time
			if (categoryAlert == null) {
				timeDifferenceCategory = alert.getUtcUpdated().getTime() - getFromDateTime().getTime();
				currentCategorySeverity = SeverityCodeToStringEnum
						.getSeverityEnumForId(alert.getPreviousAlertSeverity()).getLabel();
				if (alert.getPreviousAlertSeverity() > firstSeverity)
					firstSeverity = alert.getPreviousAlertSeverity();
			} else {
				currentCategorySeverity = SeverityCodeToStringEnum
						.getSeverityEnumForId(getAlertSeverity(AlertCategoriesEnum.returnCategoryEnum(category)))
						.getLabel();
				timeDifferenceCategory = alert.getUtcUpdated().getTime() - categoryAlert.getUtcUpdated().getTime();
			}

			// Add values to map
			if (timeDifference != 0) {
				lastAlert = alert;
				innerMap.add(new Pair<String, Number>(currentAlertSeverity, timeDifference));
			}
			if (timeDifferenceCategory != 0) {
				lastCategoryAlertMap.put(category, alert);
				List<Pair<String, Number>> categoryInnerMap = categoryMaps.get(category);
				if (categoryInnerMap == null)
					categoryInnerMap = new ArrayList<>();
				categoryInnerMap.add(new Pair<String, Number>(currentCategorySeverity, timeDifferenceCategory));
				categoryMaps.put(category, categoryInnerMap);
			}
		}

		// Update severity of instance before first alert
		if (innerMap.size() > 0) {
			Pair<String, Number> newPair = innerMap.remove(0);
			innerMap.add(0, new Pair<String, Number>(
					SeverityCodeToStringEnum.getSeverityEnumForId(firstSeverity).getLabel(), newPair.getY()));
		}

		// Add last entry for Instance and category timelines using End date and
		// time
		if (listSize > 0) {
			timeDifference = getEndDateTime().getTime() - lastAlert.getUtcUpdated().getTime();
			if (timeDifference != 0)
				innerMap.add(new Pair<String, Number>(
						SeverityCodeToStringEnum.getSeverityEnumForId(getAlertSeverity(null)).getLabel(),
						timeDifference));

			for (String category : lastCategoryAlertMap.keySet()) {

				Alert categoryAlert = lastCategoryAlertMap.get(category);
				timeDifferenceCategory = getEndDateTime().getTime() - categoryAlert.getUtcUpdated().getTime();

				if (timeDifferenceCategory != 0) {
					List<Pair<String, Number>> categoryInnerMap = categoryMaps.get(category);
					if (categoryInnerMap == null)
						categoryInnerMap = new ArrayList<>();
					categoryInnerMap.add(new Pair<String, Number>(SeverityCodeToStringEnum
							.getSeverityEnumForId(getAlertSeverity(AlertCategoriesEnum.returnCategoryEnum(category)))
							.getLabel(), timeDifferenceCategory));
					categoryMaps.put(category, categoryInnerMap);
				}
			}
		}

		// In case of zero records, use OK severity for the timelines
		for (AlertCategoriesEnum ace : AlertCategoriesEnum.values()) {

			if (categoryMaps.get(ace.getCategoryName()) == null) {
				List<Pair<String, Number>> categoryInnerMap = new ArrayList<>();
				timeDifferenceCategory = getEndDateTime().getTime() - getFromDateTime().getTime();
				categoryInnerMap
						.add(new Pair<String, Number>(SeverityCodeToStringEnum.OK.getLabel(), timeDifferenceCategory));
				categoryMaps.put(ace.getCategoryName(), categoryInnerMap);

			}

		}
		if (innerMap.size() == 0) {
			timeDifference = getEndDateTime().getTime() - getFromDateTime().getTime();
			innerMap.add(new Pair<String, Number>(SeverityCodeToStringEnum.OK.getLabel(), timeDifference));
		}

	}

	/**
	 * Returns alert severity state for the given alert category on the basis of
	 * severities of all metrics of that category
	 * 
	 * @param alertCategory
	 * @return
	 */
	private int getAlertSeverity(AlertCategoriesEnum alertCategory) {

		int severity = SeverityCodeToStringEnum.OK.getId();
		if (alertCategory == null) {
			// If Alert category is null, return severity state of instance
			// considering all categories
			for (int metricSeverity : metricSeverityMap.values()) {
				if (metricSeverity > severity)
					severity = metricSeverity;
			}
		} else if (alertCategory == AlertCategoriesEnum.CUSTOM) {
			// If Alert category is Custom, consider all metrics with id >= 1000
			for (int metricId : metricSeverityMap.keySet()) {
				int metricSeverity = metricSeverityMap.get(metricId);
				if (metricId >= 1000 && metricSeverity > severity)
					severity = metricSeverity;
			}

		} else {
			for (Integer metricId : alertCategory.getMetricIds()) {
				if (metricSeverityMap.containsKey(metricId)) {
					int metricSeverity = metricSeverityMap.get(metricId);
					if (metricSeverity > severity)
						severity = metricSeverity;
				}
			}
		}
		return severity;
	}

	/**
	 * Draws chart for the category passed as parameter using the innerMap passed
	 * 
	 * @param chart
	 * @param innerMap
	 * @param chartTitle
	 * @param showXAxis
	 */
	private void drawChart(IderaTimelineChart chart, List<Pair<String, Number>> innerMap, String chartTitle,
			boolean showXAxis) {

		chart.setOrient("horizontal");
		chart.setChartData(innerMap);
		chart.setVisible(true);
		chart.getChart().setShowXAxis(showXAxis);
		chart.getChart().setInnerPadding(0.1);
		chart.getChart().setOuterPadding(0.1);
		chart.getChart().setColorCodes(SeverityCodeToStringEnum.getColorCodesMap());
		chart.getChart().setChartTitle(chartTitle);
		chart.getChart().setStartDate(getFromDateTime());
		chart.getChart().setEndDate(getEndDateTime());

	}

	/**
	 * Updates the visibility and ordering of timeline charts for all categories
	 */
	private void updateCharts() {

		List<Component> layoutChildren = new ArrayList<>(categoryChartsVlayout.getChildren());
		for (Component child : layoutChildren) {
			categoryChartsVlayout.removeChild(child);
		}

		// Draw Instance timeline chart
		drawChart(timelineChart, innerMap, "Instance", true);

		// Draw category timeline charts
		// int vlayoutHeight = 0;
		int numCategories = categoryOptions.getSize();
		for (int i = 0; i < numCategories; i++) {

			AlertCategoriesDetails categoryDetails = categoryOptions.getElementAt(i);
			AlertCategoriesEnum ace = categoryDetails.getCategory();

			IderaTimelineChart chart = categoryCharts.get(ace);
			if (chart == null)
				chart = new IderaTimelineChart();

			chart.setWidth("1300px");
			chart.setHeight("25px");
			chart.setLeftMargin("95px");
			chart.setRightMargin("150px");
			chart.setTopMargin("25px");
			chart.setBottomMargin("25px");

			categoryCharts.put(ace.getCategoryName(), chart);

			if (categoryMaps.get(ace.getCategoryName()) != null && categoryDetails.isVisible()) {

				drawChart(chart, categoryMaps.get(ace.getCategoryName()), ace.getCategoryName(), false);
				categoryChartsVlayout.appendChild(chart);
				// vlayoutHeight += 30;

			} else {

				if (chart != null)
					chart.setVisible(false);
			}
		}

		// categoryChartsVlayout.setHeight(vlayoutHeight + "px");
		// Clients.resize(CategoryChartsContainerDiv);

		BindUtils.postNotifyChange(null, null, this, "categoryOptions");

	}

	/**
	 * Combines from date and from time values into a single date object
	 * 
	 * @return
	 */
	private Date getFromDateTime() {
		Calendar fromCalendar = Calendar.getInstance();
		fromCalendar.setTime(fromDate);

		Calendar timeCal = Calendar.getInstance();
		timeCal.setTime(fromTime);

		fromCalendar.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
		fromCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));

		return fromCalendar.getTime();
	}

	/**
	 * Combines end date and end time values into a single date object
	 * 
	 * @return
	 */
	private Date getEndDateTime() {
		Calendar endCalendar = Calendar.getInstance();
		endCalendar.setTime(endDate);

		Calendar timeCal = Calendar.getInstance();
		timeCal.setTime(endTime);

		endCalendar.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
		endCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));

		return endCalendar.getTime();
	}

	public void setModelData(List<Alert> modelData) {
		this.modelData = modelData;
	}

	public Date getFromDate() {
		return fromDate;
	}

	public void setFromDate(Date fromDate) {
		this.fromDate = fromDate;
		BindUtils.postNotifyChange(null, null, this, "fromDate");
	}

	public Date getEndDate() {
		return endDate;
	}

	public void setEndDate(Date endDate) {
		this.endDate = endDate;
		BindUtils.postNotifyChange(null, null, this, "endDate");
	}

	public Date getFromTime() {
		return fromTime;
	}

	public void setFromTime(Date fromTime) {
		this.fromTime = fromTime;
		BindUtils.postNotifyChange(null, null, this, "fromTime");
	}

	public Date getEndTime() {
		return endTime;
	}

	public void setEndTime(Date endTime) {
		this.endTime = endTime;
		BindUtils.postNotifyChange(null, null, this, "endTime");
	}

	public Double getOffsetHours() {
		return offsetHours;
	}

	public void setOffsetHours(Double offsetHours) {
		this.offsetHours = offsetHours;
	}

	// ===========================================

	// ======================Query Waits==========================

	/*
	 * public List<String> getQueryWaitsDimensions() { List<String>
	 * queryWaitsOptions = new ArrayList<>(); QueryChartOptionsEnum options[] =
	 * QueryChartOptionsEnum.values(); for (QueryChartOptionsEnum qco : options) {
	 * queryWaitsOptions.add(qco.getLabel()); } return queryWaitsOptions; }
	 * 
	 * public List<String> getQueryWaitsDimensionsUrls() { List<String>
	 * queryWaitsOptions = new ArrayList<>(); QueryChartOptionsEnum options[] =
	 * QueryChartOptionsEnum.values(); for (QueryChartOptionsEnum qco : options) {
	 * queryWaitsOptions.add(qco.getUrl()); } return queryWaitsOptions; }
	 * 
	 *//**
		 * This method will make API call to get data
		 *//*
			 * private void getModelData() {
			 * 
			 * final String productInstanceName = Utility.getUrlParameter(Executions
			 * .getCurrent().getParameterMap(), "instance");
			 * 
			 * try {
			 * 
			 * int waitId = -1; int waitCategoryId = -1; int applicationId = -1; int
			 * statementId = -1; int databaseId = -1; int clientId = -1; int sessionId = -1;
			 * int userId = -1;
			 * 
			 * if (filters.get(QueryChartOptionsEnum.WAITS) != null) { waitId =
			 * filters.get(QueryChartOptionsEnum.WAITS) .getSelectedId(); } if
			 * (filters.get(QueryChartOptionsEnum.WAIT_CATEGORY) != null) { waitCategoryId =
			 * filters.get( QueryChartOptionsEnum.WAIT_CATEGORY).getSelectedId(); } if
			 * (filters.get(QueryChartOptionsEnum.STATEMENTS) != null) { statementId =
			 * filters.get(QueryChartOptionsEnum.STATEMENTS) .getSelectedId(); } if
			 * (filters.get(QueryChartOptionsEnum.APPLICATION) != null) { applicationId =
			 * filters.get(QueryChartOptionsEnum.APPLICATION) .getSelectedId(); } if
			 * (filters.get(QueryChartOptionsEnum.DATABASES) != null) { databaseId =
			 * filters.get(QueryChartOptionsEnum.DATABASES) .getSelectedId(); } if
			 * (filters.get(QueryChartOptionsEnum.CLIENTS) != null) { clientId =
			 * filters.get(QueryChartOptionsEnum.CLIENTS) .getSelectedId(); } if
			 * (filters.get(QueryChartOptionsEnum.SESSIONS) != null) { sessionId =
			 * filters.get(QueryChartOptionsEnum.SESSIONS) .getSelectedId(); } if
			 * (filters.get(QueryChartOptionsEnum.USERS) != null) { userId =
			 * filters.get(QueryChartOptionsEnum.USERS) .getSelectedId(); }
			 * 
			 * modelData1 = SingleInstanceQueriesFacade.getQueryWaitsInstances(
			 * productInstanceName, instanceId, offsetHours.toString(),
			 * getFromDateTime().toString(), getEndDateTime(), waitId, waitCategoryId,
			 * statementId, applicationId, databaseId, clientId, sessionId, userId);
			 * 
			 * } catch (InstanceException e) { e.printStackTrace(); } }
			 */

	// =====================================================
	private void setServerStatusSection(String productInstanceName) {
		List<Alert> alertsList = new LinkedList<>();
		try {
			alertsList.addAll(AlertFacade.getAllAlerts(productInstanceName, true, null, instanceId, null, -1, null,
					null, OFFSET_IN_HOURS));
		} catch (Exception ex) {
		}
		int criticalCount = 0;
		int warningCount = 0;
		int informationalCount = 0;
		for (Alert alert : alertsList) {
			if (alert.getSeverity() == 8) {
				criticalCount++;
			} else if (alert.getSeverity() == 4) {
				warningCount++;
			} else if (alert.getSeverity() == 2) {
				informationalCount++;
			}
		}
		lblCriticalAlert.setValue("" + criticalCount);
		lblWarningAlert.setValue("" + warningCount);
		lblInformationalAlert.setValue("" + informationalCount);
	}

	@Listen("onOpen = #serverPropertiesGroup")
	public void onOpenTagsGroupBox(Event evt) {
		Groupbox tmp = (Groupbox) evt.getTarget();
		serverPropertiesCaption.setSclass(tmp.isOpen() ? OPEN_TRUE : OPEN_FALSE);
	}

	@Listen("onOpen = #serviceStatusGroup")
	public void onOpenServiceStatusGroupBox(Event evt) {
		Groupbox tmp = (Groupbox) evt.getTarget();
		serviceStatusCaption.setSclass(tmp.isOpen() ? OPEN_TRUE : OPEN_FALSE);
	}

	@Listen("onOpen = #fileUsedGroup")
	public void onOpenFileUsedGroupBox(Event evt) {
		Groupbox tmp = (Groupbox) evt.getTarget();
		fileUsedCaption.setSclass(tmp.isOpen() ? OPEN_TRUE : OPEN_FALSE);
	}

	private void setOffSet() {
		Double offSet = null;
		if (Sessions.getCurrent() != null) {
			offSet = new Double((Integer) Sessions.getCurrent().getAttribute(WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))
					/ (1000 * 60.0 * 60.0);
			offSet = -offSet;
		}
		if (offSet != null)
			OFFSET_IN_HOURS = offSet.toString();
	}

	/*
	 * public ListModelList<AlertCategoriesDetails> getcategoryOptions() { return
	 * graphCategoryOptions; }
	 * 
	 * public void setGraphCategoryOptions(ListModelList<AlertCategoriesDetails>
	 * categoryOptions) { this.graphCategoryOptions = categoryOptions;
	 * BindUtils.postNotifyChange(null, null, this, "graphCategoryOptions"); }
	 */
	public Constraint getOrderConstraint() {

		Constraint ctt = new Constraint() {
			public void validate(Component comp, Object value) throws WrongValueException {

				if (value == null)
					throw new WrongValueException(comp, "Enter a valid number between 1 and " + categoryOptions.size());

				try {

					int orderValue = Integer.parseInt((String) value);
					if (orderValue < 1 || orderValue > categoryOptions.size())
						throw new WrongValueException(comp,
								"Enter a valid number between 1 and " + categoryOptions.size());

				} catch (Exception e) {
					log.error("[getOrderConstraint]: " + e.getMessage());
					throw new WrongValueException(comp, "Enter a valid number between 1 and " + categoryOptions.size());
				}
			}
		};

		return ctt;

	}

	@Listen("onClick = #applyButton")
	public void filterByCategory() {

		int numCategories = categoryOptions.getSize();
		ListModelList<AlertCategoriesDetails> tmpGrapgCateOptions = new ListModelList<AlertCategoriesDetails>();// =
																												// categoryOptions;

		// categoryOptions.clear();
		Rows rows = categoryList.getRows();
		Iterator<Component> rowItr = rows.getChildren().iterator();
		while (rowItr.hasNext()) {
			Component rowComp = rowItr.next();
			if (rowComp instanceof Row) {
				Row row = (Row) rowComp;
				List<Component> cols = row.getChildren();
				Textbox orderComp = (Textbox) cols.get(0);
				Checkbox isVis = (Checkbox) cols.get(1);
				Label lbl = (Label) cols.get(2);
				AlertCategoriesDetails gc = setNewCustomizedValues(lbl.getValue(), orderComp.getValue(),
						isVis.isChecked());
				if (gc != null) {
					tmpGrapgCateOptions.add(gc);
				}
			}
		}
		categoryOptions = tmpGrapgCateOptions;

		boolean isOrderingValid = isOrderingValid();
		for (int i = 0; i < numCategories; i++) {
			AlertCategoriesDetails categoryOption = categoryOptions.getElementAt(i);
			categoryOption.setVisible(categoryOption.isTempVisible());
			if (isOrderingValid)
				categoryOption.setPosition(categoryOption.getTempPosition());
		}

		if (manageGraphPopup != null)
			manageGraphPopup.close();
		if (!isOrderingValid) {
			for (int i = 0; i < numCategories; i++) {
				AlertCategoriesDetails graphCategoryOption = categoryOptions.getElementAt(i);
				graphCategoryOption.setTempPosition(graphCategoryOption.getPosition());
			}
			Messagebox.show("Invalid Ordering! Order should have distinct values between 1 and " + numCategories + ".",
					"Error", Messagebox.OK, Messagebox.ERROR, null);
		}
		categoryOptions.sort(new Comparator<AlertCategoriesDetails>() {
			@Override
			public int compare(AlertCategoriesDetails o1, AlertCategoriesDetails o2) {
				return o1.getTempPosition() - o2.getTempPosition();
			}
		}, true);
		categoryList.setModel(categoryOptions);
		updateSession();
		refreshView(false);
	}

	private void processRow(Component rowComp) {
		// TODO Auto-generated method stub
		if (rowComp instanceof Row) {
			Row row = (Row) rowComp;
			List<Component> cols = row.getChildren();
			Textbox orderComp = (Textbox) cols.get(0);
			Checkbox isVis = (Checkbox) cols.get(1);
			Label lbl = (Label) cols.get(2);
			setNewCustomizedValues(lbl.getValue(), orderComp.getValue(), isVis.isChecked());
			Component nextSibl = rowComp.getNextSibling();
			if (nextSibl != null) {
				processRow(nextSibl);
			}
		}
	}

	private AlertCategoriesDetails setNewCustomizedValues(String graphName, String newOrder, boolean isChecked) {
		AlertCategoriesDetails newCat = null;// = new GraphCategories();
		for (int i = 0; i < categoryOptions.size(); i++) {
			AlertCategoriesDetails graphCate = categoryOptions.get(i);
			if (graphCate.getCategory().getCategoryName().equalsIgnoreCase(graphName)) {
				newCat = new AlertCategoriesDetails(graphCate.getCategory());
				newCat.setPosition(graphCate.getPosition());
				try {
					newCat.setTempPosition(Integer.valueOf(newOrder));
				} catch (Exception x) {
					newCat.setTempPosition(graphCate.getPosition());
					Messagebox.show("Invalid Character! Please enter a valid numeric value", "Error", Messagebox.OK,
							Messagebox.ERROR, null);
				}
				newCat.setTempVisible(isChecked);
				// categoryOptions.set(i, newCat);
				// categoryOptions.add(newCat);
				break;
			}
		}
		return newCat;
	}

	@Listen("onClick = #cancelButton")
	public void cancelFiltering() {
		BindUtils.postNotifyChange(null, null, this, "graphCategoryOptions");
		manageGraphPopup.close();
	}

	/**
	 * Validates the ordering selected by user
	 * 
	 * @return
	 */
	private boolean isOrderingValid() {

		int numCategories = categoryOptions.getSize();
		boolean validatePosition[] = new boolean[numCategories];

		for (int i = 0; i < numCategories; i++)
			validatePosition[i] = false;

		for (int i = 0; i < numCategories; i++) {
			AlertCategoriesDetails categoryOption = categoryOptions.getElementAt(i);
			int tempPosition = categoryOption.getTempPosition();
			if (tempPosition <= 0 || tempPosition > numCategories || validatePosition[tempPosition - 1])
				return false;

			validatePosition[categoryOption.getTempPosition() - 1] = true;
		}
		return true;
	}

	// new code

//	public List<AnalysisRecordModel> getItemList() {
//		log.info("itemList:" + itemList);
//		return itemList;
//	}
//
//	public void setItemList(List<AnalysisRecordModel> itemList) {
//		this.itemList = itemList;
//	}
//
//	public ListModel<AnalysisRecordModel> getCarsModel() {
//		return analysisRecord;
//	}

//	@Listen("onSelect = listbox")
//	public void updateMessage() {
//		Set<AnalysisRecordModel> selectedCars = ((ListModelList<AnalysisRecordModel>) analysisRecord).getSelection();
//		int size = selectedCars.size();
//		log.info("onSelect of listbox");
//		log.info("Size:" + size);
//		log.info("selectedCars:" + selectedCars);
//		AnalysisRecordModel next = selectedCars.iterator().next();
//
//		Map<Object, Object> args = new HashMap<Object, Object>();
//		args.put("Name", next);
//
//		Window window = (Window) Executions.createComponents("sqldm/com/idera/sqldm/widgets/analysisViewModal.zul", null, args);
//		window.doModal();
//
//	}

	@Command
	// @NotifyChange({"totalSize"})
	public void paging() {
		log.info("On pagin event get called");
	}

	@Listen("onClick=#settingsActionButton")
	public void openSettingsPopup(Event event) {
		getWidgetSettings().open(event.getTarget(), "start_after");
	}

	public Popup getWidgetSettings() {
		return widgetSettings;
	}

	public void setWidgetSettings(Popup widgetSettings) {
		this.widgetSettings = widgetSettings;
	}

	public Intbox getLimit() {
		return limit;
	}

	public void setLimit(Intbox limit) {
		this.limit = limit;
	}

	public Button getSave() {
		return save;
	}

	public void setSave(Button save) {
		this.save = save;
	}

	public int getPageSize() {
		return pageSize;
	}

	public void setPageSize(int pageSize) {
		this.pageSize = pageSize;
	}

	public int getActivePage() {
		return activePage;
	}

	public void setActivePage(int activePage) {
		this.activePage = activePage;
	}

	public int getTotalSize() {
		return totalSize;
	}

	public void setTotalSize(int totalSize) {
		this.totalSize = totalSize;
	}

	//

}
