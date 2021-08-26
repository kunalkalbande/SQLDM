package com.idera.sqldm.ui.dashboard.instances.alerts;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.util.Pair;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Constraint;
import org.zkoss.zul.Div;
import org.zkoss.zul.Include;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Popup;
import org.zkoss.zul.Toolbar;
import org.zkoss.zul.Vlayout;

import com.idera.sqldm.data.SeverityCodeToStringEnum;
import com.idera.sqldm.data.alerts.Alert;
import com.idera.sqldm.data.alerts.AlertCategoriesDetails;
import com.idera.sqldm.data.alerts.AlertCategoriesDetails.AlertCategoriesEnum;
import com.idera.sqldm.data.alerts.AlertException;
import com.idera.sqldm.data.alerts.AlertFacade;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.ui.components.charts.bar.IderaTimelineChart;
import com.idera.sqldm.ui.dashboard.instances.InstanceCategoryTab;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm.ui.dashboard.instances.SubCategoryViewModel;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstanceAlertsPreferencesBean;
import com.idera.sqldm.ui.preferences.SingleInstancePreferencesBean;
import com.idera.sqldm.utils.Utility;

public class InstanceAlertsViewModel extends SubCategoryViewModel {

	private final static Logger log = Logger
			.getLogger(InstanceAlertsViewModel.class);
	@Wire
	private Include contentView;
	@Wire
	protected IderaTimelineChart timelineChart;
	@Wire
	protected Div TimelineChartContainerDiv;
	@Wire
	protected Div CategoryChartsContainerDiv;
	@Wire
	protected Vlayout categoryChartsVlayout;
	@Wire
	protected Popup editOptionsPopup;

	private int instanceId;

	private Date fromDate;
	private Date endDate;
	private Date fromTime;
	private Date endTime;

	private Double offsetHours;
	private List<Alert> modelData;
	private ListModelList<AlertCategoriesDetails> categoryOptions;
	private Map<Integer, Integer> metricSeverityMap =  new HashMap<Integer, Integer>();
	private Map<String, List<Pair<String, Number>>> categoryMaps = new HashMap<String, List<Pair<String, Number>>>();
	private Map<String, IderaTimelineChart> categoryCharts = new HashMap<>();
	private List<Pair<String, Number>> innerMap;
	private boolean historyEvent = false;

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {

		super.afterCompose(view);

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

		offsetHours = Double.parseDouble(Utility.setOffSet());
		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance()
				.getSingleInstancePreferencesInSession(instanceId);
		pref.setSelectedCategory(InstanceCategoryTab.ALERTS.getId());
		pref.setSelectedSubCategory(1);

		SingleInstanceAlertsPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getSingleInstanceAlertsPreferenceInSession(
						instanceId);
		if (sessionBean.getFromDate() == null) {
			if(sessionBean.getCategoryOptions() == null)
				categoryOptions = new ListModelList<>(
						AlertCategoriesEnum.getDefaultList());
			else{
				setCategoryOptions(sessionBean.getCategoryOptions());
			}
			setDateTime("1D");
			setHistoryPanelValues();
		} else {
			updateViewUsingSession();
			refreshView(true);
		}
		EventQueue<Event> eq1 = EventQueues.lookup("historyChange1",
				EventQueues.DESKTOP, true);
		eq1.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {
				refreshGraphs();
			}
		});
		EventQueue<Event> eq2 = EventQueues.lookup("updateAlertsTab",
				EventQueues.DESKTOP, true);
		eq2.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {
				refreshChart();
			}
		});
	}

	@Init
	public void init(@ContextParam(ContextType.VIEW) Component view) {
		super.init(view);
	}
	
	protected void refreshChart() throws CategoryException {
    	if(historyEvent ){
    		historyEvent  = false;
    		setHistoryPanelValues();
    	}
	}

	protected void refreshGraphs() {
    	historyEvent = true;
	}

	/**
	 * Refresh view using API call if value of sessionFlag is false or using
	 * model data stored in session if this flag is true
	 * 
	 * @param sessionsFlag
	 */
	private void refreshView(boolean sessionsFlag) {
		if (!sessionsFlag)
			getModelDataAPI();
		updateMaps();
		updateCharts();
	}
	
	/*
	 * ChaitanyaTanwar DM 10.2 
	 * Setting the values of history panel controls to the variables of this class
	 * */
	public void setHistoryPanelValues() {

		HistoryPanelPreferencesBean prefHistory = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		setEndDate(prefHistory.getToDate());
		setEndTime(prefHistory.getToTime());
		setFromDate(prefHistory.getFromDate());
		setFromTime(prefHistory.getFromTime());

		refreshView(false);

	}

	/**
	 * This method will make API call to get data
	 */
	private void getModelDataAPI() {

		final String productInstanceName = Utility.getUrlParameter(Executions
				.getCurrent().getParameterMap(), "instance");

		try {

			modelData = AlertFacade.getAllAlerts(productInstanceName, false,
					getFromDateTime(), getEndDateTime(), null, instanceId,
					null, -1, null, null, offsetHours.toString());

			if(modelData != null) {
				// Filter model data according to the visibility of categories
				for (int i = modelData.size() - 1; i >= 0; i--) {
	
					Alert alert = modelData.get(i);
					String category = alert.getMetric().getMetricCategory();
					if (category.equals("Resources"))
						category = AlertCategoriesEnum.returnCategoryString(alert
								.getMetric().getMetricId());

					AlertCategoriesEnum categoryEnum = AlertCategoriesEnum
							.returnCategoryEnum(category);
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
	 * Returns the visibility of the passed Alert Category to filter the model
	 * data
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
	 * Returns alert severity state for the given alert category on the basis of 
	 * severities of all metrics of that category 
	 * 
	 * @param alertCategory
	 * @return
	 */
	private int getAlertSeverity(AlertCategoriesEnum alertCategory) {
		
		int severity = SeverityCodeToStringEnum.OK.getId();
		if(alertCategory == null) {
			//If Alert category is null, return severity state of instance considering all categories
			for (int metricSeverity : metricSeverityMap.values()) {
				if(metricSeverity > severity)
					severity = metricSeverity;
			}
		} else if(alertCategory == AlertCategoriesEnum.CUSTOM) {
			//If Alert category is Custom, consider all metrics with id >= 1000
			for (int metricId : metricSeverityMap.keySet()) {
				int metricSeverity = metricSeverityMap.get(metricId);
				if(metricId >= 1000 && metricSeverity > severity)
					severity = metricSeverity;
			}
			
		} else {
			for (Integer metricId : alertCategory.getMetricIds()) {
				if(metricSeverityMap.containsKey(metricId)) {
					int metricSeverity  =  metricSeverityMap.get(metricId);
					if(metricSeverity > severity)
						severity = metricSeverity;
				}
			}
		}
		return severity;
	}

	/**
	 * Iterates over the model data and updates the maps used by timeline charts
	 * for all categories
	 */
	private void updateMaps() {
		
		innerMap = new ArrayList<>();
		categoryMaps = new HashMap<String, List<Pair<String, Number>>>();
		metricSeverityMap = new HashMap<Integer, Integer>();
		
		if(modelData == null)
			return;

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
				timeDifference = alert.getUtcUpdated().getTime()
						- getFromDateTime().getTime();
				currentAlertSeverity = SeverityCodeToStringEnum
						.getSeverityEnumForId(alert.getSeverity()).getLabel();
				lastAlert = alert;
			} else {
				timeDifference = alert.getUtcUpdated().getTime()
						- lastAlert.getUtcUpdated().getTime();
				currentAlertSeverity = SeverityCodeToStringEnum
						.getSeverityEnumForId(getAlertSeverity(null))
						.getLabel();
			}

			String category = alert.getMetric().getMetricCategory();
			// If category of current alert is 'Resources', use metric id to get
			// sub-category
			if (category.equals("Resources"))
				category = AlertCategoriesEnum.returnCategoryString(alert
						.getMetric().getMetricId());

			Alert categoryAlert = lastCategoryAlertMap.get(category);
			String currentCategorySeverity;

			// In case of first alert of a particular category use Start date
			// and time
			if (categoryAlert == null) {
				timeDifferenceCategory = alert.getUtcUpdated().getTime()
						- getFromDateTime().getTime();
				currentCategorySeverity = SeverityCodeToStringEnum
						.getSeverityEnumForId(alert.getPreviousAlertSeverity())
						.getLabel();
				if (alert.getPreviousAlertSeverity() > firstSeverity)
					firstSeverity = alert.getPreviousAlertSeverity();
			} else {
				currentCategorySeverity = SeverityCodeToStringEnum
						.getSeverityEnumForId(getAlertSeverity(AlertCategoriesEnum.returnCategoryEnum(category)))
						.getLabel();
				timeDifferenceCategory = alert.getUtcUpdated().getTime()
						- categoryAlert.getUtcUpdated().getTime();
			}

			// Add values to map
			if (timeDifference != 0) {
				lastAlert = alert;
				innerMap.add(new Pair<String, Number>(currentAlertSeverity,
						timeDifference));
			}
			if (timeDifferenceCategory != 0) {
				lastCategoryAlertMap.put(category, alert);
				List<Pair<String, Number>> categoryInnerMap = categoryMaps
						.get(category);
				if (categoryInnerMap == null)
					categoryInnerMap = new ArrayList<>();
				categoryInnerMap.add(new Pair<String, Number>(
						currentCategorySeverity, timeDifferenceCategory));
				categoryMaps.put(category, categoryInnerMap);
			}
		}

		// Update severity of instance before first alert
		if (innerMap.size() > 0) {
			Pair<String, Number> newPair = innerMap.remove(0);
			innerMap.add(0,
					new Pair<String, Number>(SeverityCodeToStringEnum
							.getSeverityEnumForId(firstSeverity).getLabel(),
							newPair.getY()));
		}

		// Add last entry for Instance and category timelines using End date and
		// time
		if (listSize > 0) {
			timeDifference = getEndDateTime().getTime()
					- lastAlert.getUtcUpdated().getTime();
			if (timeDifference != 0)
				innerMap.add(new Pair<String, Number>(SeverityCodeToStringEnum
						.getSeverityEnumForId(getAlertSeverity(null))
						.getLabel(), timeDifference));

			for (String category : lastCategoryAlertMap.keySet()) {

				Alert categoryAlert = lastCategoryAlertMap.get(category);
				timeDifferenceCategory = getEndDateTime().getTime()
						- categoryAlert.getUtcUpdated().getTime();

				if (timeDifferenceCategory != 0) {
					List<Pair<String, Number>> categoryInnerMap = categoryMaps
							.get(category);
					if (categoryInnerMap == null)
						categoryInnerMap = new ArrayList<>();
					categoryInnerMap.add(new Pair<String, Number>(
							SeverityCodeToStringEnum.getSeverityEnumForId(
									getAlertSeverity(AlertCategoriesEnum.returnCategoryEnum(category))).getLabel(),
							timeDifferenceCategory));
					categoryMaps.put(category, categoryInnerMap);
				}
			}
		}

		// In case of zero records, use OK severity for the timelines
		for (AlertCategoriesEnum ace : AlertCategoriesEnum.values()) {

			if (categoryMaps.get(ace.getCategoryName()) == null) {
				List<Pair<String, Number>> categoryInnerMap = new ArrayList<>();
				timeDifferenceCategory = getEndDateTime().getTime()
						- getFromDateTime().getTime();
				categoryInnerMap.add(new Pair<String, Number>(
						SeverityCodeToStringEnum.OK.getLabel(),
						timeDifferenceCategory));
				categoryMaps.put(ace.getCategoryName(), categoryInnerMap);

			}

		}
		if (innerMap.size() == 0) {
			timeDifference = getEndDateTime().getTime()
					- getFromDateTime().getTime();
			innerMap.add(new Pair<String, Number>(SeverityCodeToStringEnum.OK
					.getLabel(), timeDifference));
		}

	}

	/**
	 * Draws chart for the category passed as parameter using the innerMap
	 * passed
	 * 
	 * @param chart
	 * @param innerMap
	 * @param chartTitle
	 * @param showXAxis
	 */
	private void drawChart(IderaTimelineChart chart,
			List<Pair<String, Number>> innerMap, String chartTitle,
			boolean showXAxis) {

		chart.setOrient("horizontal");
		chart.setChartData(innerMap);
		chart.setVisible(true);
		chart.getChart().setShowXAxis(showXAxis);
		chart.getChart().setInnerPadding(0.1);
		chart.getChart().setOuterPadding(0.1);
		chart.getChart().setColorCodes(
				SeverityCodeToStringEnum.getColorCodesMap());
		chart.getChart().setChartTitle(chartTitle);
		chart.getChart().setStartDate(getFromDateTime());
		chart.getChart().setEndDate(getEndDateTime());

	}

	/**
	 * Updates the visibility and ordering of timeline charts for all categories
	 */
	private void updateCharts() {

		List<Component> layoutChildren = new ArrayList<>(
				categoryChartsVlayout.getChildren());
		for (Component child : layoutChildren) {
			categoryChartsVlayout.removeChild(child);
		}

		// Draw Instance timeline chart
		drawChart(timelineChart, innerMap, "Instance", true);

		// Draw category timeline charts
		int vlayoutHeight = 0;
		int numCategories = categoryOptions.getSize();
		for (int i = 0; i < numCategories; i++) {

			AlertCategoriesDetails categoryDetails = categoryOptions
					.getElementAt(i);
			AlertCategoriesEnum ace = categoryDetails.getCategory();

			IderaTimelineChart chart = categoryCharts.get(ace);
			if (chart == null)
				chart = new IderaTimelineChart();

			chart.setWidth("1300px");
			chart.setHeight("25px");
			chart.setLeftMargin("95px");
			chart.setRightMargin("150px");
			chart.setTopMargin("20px");
			chart.setBottomMargin("20px");

			categoryCharts.put(ace.getCategoryName(), chart);

			if (categoryMaps.get(ace.getCategoryName()) != null
					&& categoryDetails.isVisible()) {

				drawChart(chart, categoryMaps.get(ace.getCategoryName()),
						ace.getCategoryName(), false);
				categoryChartsVlayout.appendChild(chart);
				vlayoutHeight += 30;

			} else {

				if (chart != null)
					chart.setVisible(false);
			}
		}

		categoryChartsVlayout.setHeight(vlayoutHeight + "px");
		Clients.resize(CategoryChartsContainerDiv);
		BindUtils.postNotifyChange(null, null, this, "categoryOptions");

	}

	/**
	 * Updates the values stored in session
	 */
	private void updateSession() {

		SingleInstanceAlertsPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getSingleInstanceAlertsPreferenceInSession(
						instanceId);

		sessionBean.setModelData(modelData);
		sessionBean.setFromDate(fromDate);
		sessionBean.setFromTime(fromTime);
		sessionBean.setEndDate(endDate);
		sessionBean.setEndTime(endTime);
		sessionBean.setCategoryOptions(categoryOptions);
		PreferencesUtil.getInstance()
				.setSingleInstanceAlertsPreferenceInSession(sessionBean);

	}

	/**
	 * Updates local values using those stored in session
	 */
	private void updateViewUsingSession() {

		SingleInstanceAlertsPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getSingleInstanceAlertsPreferenceInSession(
						instanceId);
		setModelData(sessionBean.getModelData());
		setFromDate(sessionBean.getFromDate());
		setFromTime(sessionBean.getFromTime());
		setEndDate(sessionBean.getEndDate());
		setEndTime(sessionBean.getEndTime());
		setCategoryOptions(sessionBean.getCategoryOptions());

	}

	/**
	 * Validates the ordering selected by user
	 * 
	 * @return
	 */
	private boolean isOrderingValid() {

		int numCategories = categoryOptions.getSize();
		boolean validatePosition[] = new boolean[numCategories];

		for (int i = 0; i < numCategories; i++) {
			validatePosition[i] = false;
		}

		for (int i = 0; i < numCategories; i++) {
			AlertCategoriesDetails categoryOption = categoryOptions
					.getElementAt(i);
			int tempPosition = categoryOption.getTempPosition();
			if (tempPosition <= 0 || tempPosition > numCategories
					|| validatePosition[tempPosition - 1])
				return false;

			validatePosition[categoryOption.getTempPosition() - 1] = true;
		}

		return true;

	}

	@Command("filterByCategory")
	public void filterByCategory() {

		int numCategories = categoryOptions.getSize();
		boolean isOrderingValid = isOrderingValid();
		for (int i = 0; i < numCategories; i++) {
			AlertCategoriesDetails categoryOption = categoryOptions
					.getElementAt(i);
			categoryOption.setVisible(categoryOption.isTempVisible());
			if (isOrderingValid)
				categoryOption.setPosition(categoryOption.getTempPosition());
		}

		categoryOptions.sort(new Comparator<AlertCategoriesDetails>() {

			@Override
			public int compare(AlertCategoriesDetails o1,
					AlertCategoriesDetails o2) {
				return o1.getPosition() - o2.getPosition();
			}
		}, true);

		updateSession();
		refreshView(false);
		editOptionsPopup.close();
		if (!isOrderingValid) {
			for (int i = 0; i < numCategories; i++) {
				AlertCategoriesDetails categoryOption = categoryOptions
						.getElementAt(i);
				categoryOption.setTempPosition(categoryOption.getPosition());
			}
			Messagebox.show(
					"Invalid Ordering! Order should have distinct values between 1 and "
							+ numCategories + ".", "Error", Messagebox.OK,
					Messagebox.ERROR, null);
		}
	}

	@Command("cancelFiltering")
	public void cancelFiltering() {

		BindUtils.postNotifyChange(null, null, this, "categoryOptions");
		editOptionsPopup.close();

	}

	/**
	 * Publish event handled in AlertListComposer
	 */
	public void publishEvent() {

		EventQueue<Event> eq = EventQueues.lookup("updateModel",
				EventQueues.DESKTOP, false);
		if (eq != null) {

			updateSession();
			eq.publish(new Event("updateModel", null, null));

		}

	}

	@Command("changeTimeRange")
	public void changeTimeRange() {
		refreshView(false);
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

		fromCalendar.set(Calendar.HOUR_OF_DAY,
				timeCal.get(Calendar.HOUR_OF_DAY));
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

		endCalendar
				.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
		endCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));

		return endCalendar.getTime();
	}

	/**
	 * Initializes date and time values
	 */
	@Command("updateTimeValues")
	public void setDateTime(@BindingParam("label") String label) {

		Calendar c = Calendar.getInstance();
		c.setTime(new Date(c.getTimeInMillis()
				- (long) (getOffsetHours() * 60 * 60 * 1000)));

		setEndDate(c.getTime());
		setEndTime(c.getTime());

		if (label.equals("8H"))
			c.set(Calendar.HOUR, c.get(Calendar.HOUR) - 8);
		else if (label.equals("1D"))
			c.set(Calendar.HOUR, c.get(Calendar.HOUR) - 24);
		else if (label.equals("5D"))
			c.set(Calendar.DATE, c.get(Calendar.DATE) - 5);
		else if (label.equals("4W"))
			c.set(Calendar.DATE, c.get(Calendar.DATE) - 28);

		setFromDate(c.getTime());
		setFromTime(c.getTime());

		refreshView(false);

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

	public Constraint getOrderConstraint() {

		Constraint ctt = new Constraint() {
			public void validate(Component comp, Object value)
					throws WrongValueException {

				if (value == null)
					throw new WrongValueException(comp,
							"Enter a valid number between 1 and 11.");

				try {
					
					int orderValue = Integer.parseInt((String)value);
					if(orderValue < 1 || orderValue > 11)
						throw new WrongValueException(comp,
								"Enter a valid number between 1 and 11.");
					
				} catch(Exception e) {
					throw new WrongValueException(comp,
							"Enter a valid number between 1 and 11.");
				}
			}
		};

		return ctt;

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

	public ListModelList<AlertCategoriesDetails> getCategoryOptions() {
		return categoryOptions;
	}

	public void setCategoryOptions(
			ListModelList<AlertCategoriesDetails> categoryOptions) {
		this.categoryOptions = categoryOptions;
		BindUtils.postNotifyChange(null, null, this, "categoryOptions");
	}

	public void setModelData(List<Alert> modelData) {
		this.modelData = modelData;
	}

	@Override
	public Include getContentView() {
		return contentView;
	}

	@Override
	public boolean haveSubTab() {
		return false;
	}

	@Override
	public InstanceCategoryTab getInstanceCategoryTab() {
		return InstanceCategoryTab.ALERTS;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategoryTab() {
		return InstanceSubCategoriesTab.DATABASES_SUMMARY;
	}

	@Override
	public String getModelName() {
		throw new RuntimeException("method not supported.");
	}

	@Override
	public Object getModelNameData() throws Exception {
		throw new RuntimeException("method not supported.");
	}

	@Override
	public void getModelData() {
		return;
	}

	@Override
	public Toolbar getToolbar() {
		// TODO Auto-generated method stub
		return null;
	}

}
