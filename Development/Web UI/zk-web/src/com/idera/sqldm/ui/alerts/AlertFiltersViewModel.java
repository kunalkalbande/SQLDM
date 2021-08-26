package com.idera.sqldm.ui.alerts;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.List;

import com.idera.common.dashboard.Composers.DashboardBaseWidgetComposer;
import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Comboitem;
import org.zkoss.zul.Constraint;
import org.zkoss.zul.Datebox;
import org.zkoss.zul.Div;
import org.zkoss.zul.Radio;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Timebox;

import com.idera.common.rest.CoreRestClient;
import com.idera.cwf.model.Product;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.DashboardInstanceFacade;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.alerts.Alert.AlertSeverity;
import com.idera.sqldm.data.alerts.AlertException;
import com.idera.sqldm.data.alerts.AlertFacade;
import com.idera.sqldm.data.alerts.Metrics;
import com.idera.sqldm.data.instances.ServerStatus;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.session.SessionUtil;
import com.idera.sqldm.ui.alerts.AlertsGroupingModel.GroupBy;
import com.idera.sqldm.ui.preferences.AlertsPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.Utility;

public class AlertFiltersViewModel {
	private static final long serialVersionUID = 1025777743281928721L;
	private Logger log = Logger.getLogger(AlertFiltersViewModel.class);
	private String OFFSET_IN_HOURS = "0.0";
	private Integer offSetInSecs = 0;
	
	@Wire
	private Datebox fromdate;
	@Wire
	private Datebox todate;
	@Wire
	private Timebox fromtime;
	@Wire
	private Timebox totime;
	@Wire
	private Radiogroup radioActive;
	@Wire
	private Div dateRange;
	@Wire
	private Button applyFiltersBtn;
	@Wire
	private Button resetFiltersBtn;
	@Wire
	private Combobox severity;
	@Wire
	private Combobox server;
	@Wire
	private Combobox metric;
	@Wire
	private Combobox repo;
	// private Session session;
	private AlertFilter selectedFilters;
	private AlertFilter customFilter;
	// private AlertSeverity selectedSeverity;
	private List<CustomComboBoxModel> allInstances;
	private List<CustomComboBoxModel> allMetrics;
	private List<CustomComboBoxModel> allSeverity;
	private List<CustomComboBoxModel> allRepos;

	private Date fromDate;
	private Date endDate;
	private Date fromTime;
	private Date endTime;
	
	@Init
	public void init() {
		setOffSet();
		setOffSetInSecs();
		initializeDateTimeFilters();
		AlertsPreferencesBean sessionBean = PreferencesUtil.getInstance()
				.getAlertsPreferencesInSession();
		
		if (sessionBean.getCustomFilter() != null) {
			if (sessionBean.getSelectedGroupBy() == GroupBy.CUSTOM) {
				selectedFilters = sessionBean.getCustomFilter();
				try {
					customFilter = (AlertFilter) selectedFilters.clone();
				} catch (CloneNotSupportedException e) {
					e.printStackTrace();
				}
			} else {
				selectedFilters = new AlertFilter(true);
				customFilter = sessionBean.getCustomFilter();
			}
		} else {
			selectedFilters = new AlertFilter(true);
			customFilter = new AlertFilter(true);
			sessionBean.setCustomFilter(customFilter);
			PreferencesUtil.getInstance().setAlertsPreferencesInSession(
					sessionBean);
		}
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
		EventQueue<Event> eq = EventQueues.lookup("changeGroup",
				EventQueues.DESKTOP, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				SessionUtil.getSecurityContext();
				GroupBy selectedGrp = (GroupBy) event.getData();
				onApplyGroups(selectedGrp);
			}
		});
        
    	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

        initializeInstancesList(productInstanceName);
		initializeMetricsComboBox();
		initializeSeverityComboBox();
		initializeRepositoriesComboBox();
		initializeDateRanges();
		//session.setAttribute(ApplicationSessionSettings.ALERT_FILTERS_PROPERTY, selectedFilters);
	}

	private void onApplyGroups(GroupBy selectedGrp) {
		//If selected group is NOT CUSTOM
		if (selectedGrp != GroupBy.CUSTOM ) {
			
			if(selectedGrp != GroupBy.PRODUCT){
			selectedFilters.setActive(true);
			radioActive.setSelectedIndex(0);
			disableDateRanges();
			}
			customFilter.setInstanceId(selectedFilters.getInstanceId());
			customFilter.setMetricId(selectedFilters.getMetricId());
			customFilter.setSeverity(selectedFilters.getSeverity());
			customFilter.setProductId(selectedFilters.getProductId());
			
			selectedFilters.setInstanceId(-1);
			selectedFilters.setMetricId(-1);
			selectedFilters.setSeverity(-1);
			selectedFilters.setProductId(-1);
			
			//Set <ALL> value in combo boxes
			server.setSelectedIndex(0);
			metric.setSelectedIndex(0);
			severity.setSelectedIndex(0);
			repo.setSelectedIndex(0);
		} else {
			selectedFilters.setActive(customFilter.isActive());
			if (customFilter.isActive()) {
				radioActive.setSelectedIndex(0);
				disableDateRanges();
			} else {
				radioActive.setSelectedIndex(1);
				enableDateRanges();

			}
			selectedFilters.setInstanceId(customFilter.getInstanceId());
			selectedFilters.setMetricId(customFilter.getMetricId());
			selectedFilters.setSeverity(customFilter.getSeverity());
			selectedFilters.setProductId(customFilter.getProductId());
			setSelectedCombo(selectedFilters);
		}
		AlertsPreferencesBean sessionBean = PreferencesUtil.getInstance()
				.getAlertsPreferencesInSession();
		sessionBean.setCustomFilter(customFilter);
		sessionBean.setSelectedGroupBy(selectedGrp);
		PreferencesUtil.getInstance()
				.setAlertsPreferencesInSession(sessionBean);
	}

	private void setSelectedCombo(AlertFilter selectedFilters) {
		List<Comboitem> items;
		if (selectedFilters.getInstanceId() != null) {
			items = server.getItems();
			for (Comboitem item : items) {
				if (item.getValue() == selectedFilters.getInstanceId()) {
					server.setSelectedItem(item);
					break;
				}
			}
		}
		if (selectedFilters.getMetricId() != null) {
			items = metric.getItems();
			for (Comboitem item : items) {
				if (item.getValue() == selectedFilters.getMetricId()) {
					metric.setSelectedItem(item);
					break;
				}
			}
		}
		if (selectedFilters.getSeverity() != null) {
			items = severity.getItems();
			for (Comboitem item : items) {
				if (item.getValue() == selectedFilters.getSeverity()) {
					severity.setSelectedItem(item);
					break;
				}
			}
		}
		
		if (selectedFilters.getProductId() != null) {
			items = repo.getItems();
			for (Comboitem item : items) {
				if (item.getValue()==selectedFilters.getProductId()) {
					repo.setSelectedItem(item);
					break;
				}
			}
		}
	
	}

	private void initializeDateRanges() {
		
		
		if (selectedFilters != null) {
			if (selectedFilters.isActive()) {
				initializeDateTimeFilters();
				disableDateRanges();
				radioActive.setSelectedIndex(0);
			} else {
				radioActive.setSelectedIndex(1);
				if (selectedFilters.getToDateTime() == null
						|| selectedFilters.getFromDateTime() == null) {

					initializeDateTimeFilters();
					return;
				}
				todate.setValue(selectedFilters.getToDateTime());
				totime.setValue(selectedFilters.getToDateTime());
				fromdate.setValue(selectedFilters.getFromDateTime());
				fromtime.setValue(selectedFilters.getFromDateTime());
			}
		}
		
		
	}	
	public Constraint getToDateConstraint(){
		
		Constraint ctt = new Constraint() {
			
			public void validate(Component comp, Object value)
					throws WrongValueException {
				Calendar cal = Calendar.getInstance();
				
				
				cal.setTime(new Date());
				
				cal.add(Calendar.SECOND, offSetInSecs);
				
				//Hack to Compare dates upto minutes only
				cal.set(Calendar.SECOND, 0);
				cal.set(Calendar.MILLISECOND, 0);
				
				Date currDateTime = cal.getTime();
								
				
				Calendar calPrecise = Calendar.getInstance();
				//Hack to Compare dates upto minutes only
				calPrecise.setTime(getFromDate());
				calPrecise.set(Calendar.SECOND , 0);
				calPrecise.set(Calendar.MILLISECOND , 0);
				setFromDate(calPrecise.getTime());
				calPrecise.setTime((Date)value);
				calPrecise.set(Calendar.SECOND, 0);
				calPrecise.set(Calendar.MILLISECOND, 0);
	
				
				if (value == null
						|| ( getFromDate()!= null && (calPrecise.getTime())
								.before(getFromDate()))
						|| (calPrecise.getTime()).after(currDateTime))
					throw new WrongValueException(comp,
							"To date should be between from date and today.");
			}
		};
		
		return ctt;
		}
		
	public Constraint getFromDateConstraint(){
	Constraint ctt1 = new Constraint() {
			public void validate(Component comp, Object value)
					throws WrongValueException {
				
				Calendar cal = Calendar.getInstance();
				
				cal.setTime(new Date());
				
				cal.add(Calendar.SECOND, offSetInSecs);
				
				//Hack to Compare dates upto minutes only
				cal.set(Calendar.SECOND, 0);
				cal.set(Calendar.MILLISECOND, 0);
				
				Date currDateTime = cal.getTime();
				
				
				Calendar calPrecise = Calendar.getInstance();
				//Hack to Compare dates upto minutes only
				calPrecise.setTime(getEndDate());
				calPrecise.set(Calendar.SECOND , 0);
				calPrecise.set(Calendar.MILLISECOND , 0);
				setEndDate(calPrecise.getTime());
				calPrecise.setTime((Date)value);
				calPrecise.set(Calendar.SECOND , 0);
				calPrecise.set(Calendar.MILLISECOND , 0);
								
				
				if (value == null || (calPrecise.getTime()).equals(getEndDate())&&(calPrecise.getTime()).after(getEndDate())||((Date) value).after(currDateTime))
					throw new WrongValueException(comp,
							"From date can't be empty or future.");
			}
		};
	return ctt1;	
	}
	
	public Constraint getFromTimeConstraint(){
	
	
		Constraint cttFromTime = new Constraint() {
			
			public void validate(Component comp, Object value)
					throws WrongValueException {
				
				
				SimpleDateFormat formatter = new SimpleDateFormat("dd MMM yyyy HH:mm");

				Calendar cal = Calendar.getInstance();
				
				cal.setTime(new Date());
				
				cal.add(Calendar.SECOND, offSetInSecs);
				
				Calendar calFromDate = Calendar.getInstance();
				calFromDate.setTime(getFromDate());
				Calendar calEndDate = Calendar.getInstance();
				calEndDate.setTime(getEndDate());
				calEndDate.set(Calendar.SECOND, 0);
				calEndDate.set(Calendar.MILLISECOND, 0);
				Calendar boxValue = Calendar.getInstance();
				boxValue.setTime((Date)value);
				
				//handling from time > end time when from date == end date
				if(     formatter.format(getFromDate()).equals(formatter.format(getEndDate()))    ){
					if(value==null || ((Date)value).after(getEndTime())){
						throw new WrongValueException(comp,
								"From datetime should be less than To datetime");
						}
					}
				//handling from time > curr time when from date == curr date
				if(	calFromDate.get(Calendar.DATE) == cal.get(Calendar.DATE)   &&   calFromDate.get(Calendar.MONTH) == cal.get(Calendar.MONTH)	&&   calFromDate.get(Calendar.YEAR) == cal.get(Calendar.YEAR)){
					
					if(value==null || boxValue.get(Calendar.HOUR_OF_DAY) > cal.get(Calendar.HOUR_OF_DAY) || 
							(  
									(boxValue.get(Calendar.HOUR_OF_DAY) == cal.get(Calendar.HOUR_OF_DAY)) 
									&& 
									(boxValue.get(Calendar.MINUTE) > cal.get(Calendar.MINUTE)) 
								) 
					  ){
						throw new WrongValueException(comp,
								"From datetime should be less than Current datetime");
						}
					}
					
					
				}
				
			
		};
	return cttFromTime;
	}	
	
	public Constraint getEndTimecontraint(){
		Constraint cttEndTime = new Constraint() {
			
			public void validate(Component comp, Object value)
					throws WrongValueException {
				
				SimpleDateFormat formatter = new SimpleDateFormat("dd MMM yyyy HH:mm");
				
				Calendar cal = Calendar.getInstance();
				
				cal.setTime(new Date());
				
				cal.add(Calendar.SECOND, offSetInSecs);
				
				Calendar calFromDate = Calendar.getInstance();
				calFromDate.setTime(getFromDate());
				Calendar calEndDate = Calendar.getInstance();
				calEndDate.setTime(getEndDate());
				
				Calendar boxValue = Calendar.getInstance();
				boxValue.setTime((Date)value);
				
				//handling from time > end time when from date == end date
				if(formatter.format(getFromDate()).equals(formatter.format(getEndDate()))){
					if(value==null || ((Date)value).before(getFromTime())){
						throw new WrongValueException(comp,
								"End datetime should be greater than From datetime");
						}
					}			
					
				}	
		};
		return cttEndTime;
}
		
	private void initializeInstancesList(String productInstanceName){
		allInstances = new ArrayList<>();
		allInstances.add(new CustomComboBoxModel(-1, "<All>"));

		try{
			List<DashboardInstance> instances = DashboardInstanceFacade.getDashboardInstances(productInstanceName);
			//sort list according to instance name.
			Collections.sort(instances, new Comparator<DashboardInstance>() {
				@Override
				public int compare(DashboardInstance o1, DashboardInstance o2) {
//					return o1.getInstanceName().toLowerCase().compareTo(o2.getInstanceName().toLowerCase()); @author Saumyadeep
					return o1.getOverview().getDisplayName().toLowerCase().compareTo(o2.getOverview().getDisplayName().toLowerCase());
					
				}
				
			});
			for(DashboardInstance instance : instances){
//				allInstances.add(new CustomComboBoxModel(instance.getSQLServerId(), instance.getInstanceName())); @author Saumyadeep
				allInstances.add(new CustomComboBoxModel(instance.getOverview().getSQLServerId(), instance.getOverview().getDisplayName()));
			}
		}
		catch(InstanceException e){
			log.error("InstanceException while getting instanceSummay");
			log.error(e.getMessage(),e);
		}
		catch(Exception e){
			log.error("Exception while getting instanceSummay");
			log.error(e.getMessage(),e);
		}
	}
	
	private void initializeSeverityComboBox(){
		allSeverity = new ArrayList<>();
		allSeverity.add(new CustomComboBoxModel(-1, "<All>"));
		allSeverity.add(new CustomComboBoxModel(1, "OK"));
		allSeverity.add(new CustomComboBoxModel(2, "INFORMATIONAL"));
		allSeverity.add(new CustomComboBoxModel(4, "WARNING"));
		allSeverity.add(new CustomComboBoxModel(8, "CRITICAL"));	
}
	
	private void initializeRepositoriesComboBox(){
		allRepos = new ArrayList<>();
		allRepos.add(new CustomComboBoxModel(-1, "<All>"));
		try {
			List<Product> repoList = CoreRestClient.getInstance().getProducts();
			//sort the list according to name.
			
			Collections.sort(repoList, new Comparator<Product>() {
				@Override
				public int compare(Product o1, Product o2) {
					return o1.getInstanceName().compareTo(o2.getInstanceName());
				}
				
			});
			for(Product product : repoList){
				if(product.getInstanceName() != null)
					allRepos.add(new CustomComboBoxModel(product.getProductId(), product.getInstanceName()));
				else
					allRepos.add(new CustomComboBoxModel(product.getProductId(), ELFunctions.getMessage(SQLdmI18NStrings.DEFAULT_INSTANCE_NAME)));			}
		} catch (AlertException e) {
			log.error("AlertException while getting Product List");
			log.error(e.getMessage(),e);
		} catch (Exception e){
			log.error("Exception while getting Product List");
			log.error(e.getMessage(),e);
		}
	}
	
	private void initializeMetricsComboBox(){
		allMetrics = new ArrayList<>();
		allMetrics.add(new CustomComboBoxModel(-1, "<All>"));
		String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		try {
			List<Metrics> metrics = AlertFacade.getAllMetrics(productInstanceName , OFFSET_IN_HOURS);
			//sort the list according to name.
			Collections.sort(metrics, new Comparator<Metrics>() {
				@Override
				public int compare(Metrics o1, Metrics o2) {
					return o1.getName().compareTo(o2.getName());
				}
				
			});
			for(Metrics metric : metrics){
				allMetrics.add(new CustomComboBoxModel(metric.getMetricId(), metric.getName()));
			}
		} catch (AlertException e) {
			log.error("AlertException while getting AlertMetrices");
			log.error(e.getMessage(),e);
		} catch (Exception e){
			log.error("Exception while getting AlertMetrices");
			log.error(e.getMessage(),e);
		}
	}

	private void resetDateValues() {
		Calendar c = Calendar.getInstance();
		c.set(Calendar.HOUR_OF_DAY, 0);
		c.set(Calendar.MINUTE, 0);
		c.set(Calendar.SECOND, 0);
		todate.setValue(c.getTime());
		selectedFilters.setToDate(c.getTime());
		c.set(Calendar.HOUR_OF_DAY, 23);
		c.set(Calendar.MINUTE, 59);
		totime.setValue(c.getTime());
		selectedFilters.setToTime(c.getTime());
		c.add(Calendar.DATE, -3);
		c.set(Calendar.HOUR_OF_DAY, 0);
		c.set(Calendar.MINUTE, 0);
		fromdate.setValue(c.getTime());
		fromtime.setValue(c.getTime());
		selectedFilters.setFromDate(c.getTime());
		selectedFilters.setFromTime(c.getTime());
	}
	/**
	 * Disables date filters
	 */
	private void disableDateRanges() {
		fromdate.setButtonVisible(false);
		fromdate.setReadonly(true);
		todate.setButtonVisible(false);
		todate.setReadonly(true);
		fromtime.setButtonVisible(false);
		fromtime.setReadonly(true);
		totime.setButtonVisible(false);
		totime.setReadonly(true);
	}

	/**
	 * Enables date filters.
	 */
	private void enableDateRanges() {
		fromdate.setButtonVisible(true);
		fromdate.setReadonly(false);
		todate.setButtonVisible(true);
		todate.setReadonly(false);
		fromtime.setButtonVisible(true);
		fromtime.setReadonly(false);
		totime.setButtonVisible(true);
		totime.setReadonly(false);
	}

	/**
	 * Disable the dateRanges div when the radio button for active alerts been
	 * checked.
	 */
	@Command
	public void changeRadioActive() {
		Radio selectedItem = radioActive.getSelectedItem();
		if ("activeAlerts".equals(selectedItem.getId())) {
			selectedFilters.setActive(true);
			// when clicked on active radio button change the dates to the
			// previous state.
			if (customFilter.getToDateTime() == null
					|| customFilter.getFromDateTime() == null) {
				initializeDateTimeFilters();
			} else {
				todate.setValue(customFilter.getToDate());
				totime.setValue(customFilter.getToTime());
				fromdate.setValue(customFilter.getFromDate());
				fromtime.setValue(customFilter.getFromTime());
			}
			disableDateRanges();
		} else {
			selectedFilters.setActive(false);
			enableDateRanges();
		}
	}

	/**
	 * Publishes the event for change filters
	 * 
	 * @param filter
	 */
	private void publishChangeFilterEvent(AlertFilter filter) {
		EventQueue<Event> eq = EventQueues.lookup("changeFilter",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("onClick", null, filter));
		}
	}
	
	/**
	 * 
	 */
	/* @Listen("onClick = #applyFiltersBtn") */
	@Command
	public void applyFilters() {
		/*
		 * if(selectedSeverity != null){
		 * selectedFilters.setSeverity(selectedSeverity.getId()); }
		 */
		Radio selectedItem = radioActive.getSelectedItem();
		if ("activeAlerts".equals(selectedItem.getId())) {
			selectedFilters.setActive(true);
		} else {
			selectedFilters.setActive(false);
		}
		if (fromdate.getValue() != null) {
			selectedFilters.setFromDate(fromdate.getValue());
		}
		if (todate.getValue() != null) {
			selectedFilters.setToDate(todate.getValue());
		}
		if (fromtime.getValue() != null) {
			selectedFilters.setFromTime(fromtime.getValue());
		}
		if (totime.getValue() != null) {
			selectedFilters.setToTime(totime.getValue());
		}
		try {
			customFilter = (AlertFilter) selectedFilters.clone();
		} catch (CloneNotSupportedException e) {
			e.printStackTrace();
		}
    	AlertsPreferencesBean sessionBean = PreferencesUtil.getInstance().getAlertsPreferencesInSession();
		sessionBean.setCustomFilter(customFilter);
		PreferencesUtil.getInstance().setAlertsPreferencesInSession(sessionBean);
        publishChangeFilterEvent(customFilter);
    }
	
	@Command
	@NotifyChange("selectedFilters")
    public void resetFilters() {
		server.setSelectedIndex(0);
		metric.setSelectedIndex(0);
		severity.setSelectedIndex(0);
		repo.setSelectedIndex(0);
		//selectedSeverity = null;
	
		initializeDateTimeFilters();
		AlertsPreferencesBean sessionBean = PreferencesUtil.getInstance().getAlertsPreferencesInSession();
		for(Radio item : radioActive.getItems()){
			
			if(sessionBean.getSelectedGroupBy() == GroupBy.CUSTOM ){
				
				if("timedAlerts".equals(item.getId())){
					
					item.setChecked(true);
					enableDateRanges();
				}
			}else{
				
				 if("activeAlerts".equals(item.getId())){
	        	item.setChecked(true);
	        }
	        else{
	        	item.setChecked(false);
	        	disableDateRanges();
	        }
			}  
		}
		try {
			selectedFilters = new AlertFilter(true);
			customFilter = (AlertFilter) selectedFilters.clone();
		} catch (CloneNotSupportedException e) {
			e.printStackTrace();
		}
		//only call after removing session attribute.
		
		sessionBean.setCustomFilter(customFilter);
		PreferencesUtil.getInstance().setAlertsPreferencesInSession(sessionBean);
		publishChangeFilterEvent(customFilter);
    }
	
	public AlertFilter getSelectedFilters() {
		return selectedFilters;
	}

	public void setSelectedFilters(AlertFilter selectedFilters) {
		this.selectedFilters = selectedFilters;
	}

	public List<CustomComboBoxModel> getAllInstances() {
		return allInstances;
	}

	public List<CustomComboBoxModel> getAllRepos() {
		return allRepos;
	}

	public void setAllRepos(List<CustomComboBoxModel> allRepos) {
		this.allRepos = allRepos;
	}

	public void setAllInstances(List<CustomComboBoxModel> allInstances) {
		this.allInstances = allInstances;
	}

	public List<CustomComboBoxModel> getAllMetrics() {
		return allMetrics;
	}

	public Combobox getRepo() {
		return repo;
	}

	public void setRepo(Combobox repo) {
		this.repo = repo;
	}

	public void setAllMetrics(List<CustomComboBoxModel> allMetrics) {
		this.allMetrics = allMetrics;
	}

	public List<CustomComboBoxModel> getAllSeverity() {
		return allSeverity;
	}

	public void setAllSeverity(List<CustomComboBoxModel> allSeverity) {
		this.allSeverity = allSeverity;
	}

	@Command
	public void onCreatePDFMenuitem(Event evt) {

		EventQueue<Event> eq = EventQueues.lookup("exportData",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("exportToPdf"));

		}
	}

	@Command
	public void onCreateXLSMenuitem(Event evt) {

		EventQueue<Event> eq = EventQueues.lookup("exportData",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("exportToExcel"));

		}
	}

	@Command
	public void onCreateXMLMenuitem(Event evt) {

		EventQueue<Event> eq = EventQueues.lookup("exportData",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("exportToXml"));

		}
	}

	private void setOffSetInSecs(){
		if (Sessions.getCurrent() != null) {
			offSetInSecs = (Integer) Sessions.getCurrent().getAttribute(
					WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET);
			offSetInSecs/=1000;
		}
		
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
			OFFSET_IN_HOURS = offSet.toString();
	}

	public Date getEndTime() {
		return endTime;
	}

	public void setEndTime(Date endTime) {
		this.endTime = endTime;
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
	
	public void initializeDateTimeFilters(){
		Calendar cal = Calendar.getInstance();
		Calendar calTime = Calendar.getInstance();
		calTime.setTime(new Date(0));
		cal.setTime(new Date());
		//Offset
		cal.add(Calendar.SECOND, offSetInSecs);
		  //End Date
		  cal.set(Calendar.HOUR_OF_DAY, 0);
		  cal.set(Calendar.MINUTE, 0);
		  cal.set(Calendar.SECOND, 0);
		  setEndDate(cal.getTime());
		  
		  
		  
		  //End Time
		  calTime.set(Calendar.HOUR_OF_DAY, 23);
		  calTime.set(Calendar.MINUTE, 59);
		  calTime.set(Calendar.SECOND, 00);
		  setEndTime(calTime.getTime());

		  cal.add(Calendar.DATE, -30);
		  calTime.set(Calendar.HOUR_OF_DAY, 0);
		  calTime.set(Calendar.MINUTE, 0);
		  setFromDate(cal.getTime());
		  
		  
		  setFromTime(calTime.getTime());
		  
		  BindUtils.postNotifyChange(null, null, this, "fromDate");
		  BindUtils.postNotifyChange(null, null, this, "endDate");
		  BindUtils.postNotifyChange(null, null, this, "fromTime");
		  BindUtils.postNotifyChange(null, null, this, "endTime");
		  
	}
}
