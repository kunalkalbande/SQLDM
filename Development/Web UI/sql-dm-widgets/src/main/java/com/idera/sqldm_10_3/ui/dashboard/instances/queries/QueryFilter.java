package com.idera.sqldm_10_3.ui.dashboard.instances.queries;

import com.idera.sqldm_10_3.data.queries.QueryGroups;
import com.idera.sqldm_10_3.data.queries.ViewMetrics;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;

public class QueryFilter implements Cloneable {
	static QueryFilter DEFAULT_FILTER = new QueryFilter();

	private ViewMetrics selectedView;
	private QueryGroups selectedGroupBy;
	private String selectedTimePeriod;

	// If Selected Time Period Index is 7
	private Date fromDate;
	private Date endDate;
	private Date fromTime;
	private Date endTime;
	

	String includeSQLText="";
	String excludeSQLText="";
	
	private boolean applicationAllChecked = true;
	private boolean databasesAllChecked = true;
	private boolean usersAllChecked = true;
	private boolean clientsAllChecked = true;

	private boolean showSQLStatements = true;
	private boolean showStoredProcs = true;
	private boolean showSQLBatches = true;
	private boolean includeOverlapping = false;
	private boolean includeIncomplete = false;
	
	private boolean isDrillDown = false;
	
	private Double offsetHours;

	public QueryFilter() {
	}

	public void setFromDate(Date fromDate) {
		this.fromDate = fromDate;
	}

	public void setEndDate(Date toDate) {
		this.endDate = toDate;
	}

	public void setFromTime(Date fromTime) {
		this.fromTime = fromTime;
	}

	public void setEndTime(Date toTime) {
		this.endTime = toTime;
	}
	
	private Calendar formatFromDateTime() {
	
		Calendar c = Calendar.getInstance();

		if (selectedTimePeriod.equals(QueryTimePeriodsEnum.THIRTY_MINUTES
				.getTimePeriod())) {

			c.add(Calendar.MINUTE, -30);

		} else if (selectedTimePeriod.equals(QueryTimePeriodsEnum.ONE_HOUR
				.getTimePeriod())) {

			c.add(Calendar.HOUR, -1);

		} else if (selectedTimePeriod.equals(QueryTimePeriodsEnum.FOUR_HOURS
				.getTimePeriod())) {

			c.add(Calendar.HOUR, -4);

		} else if (selectedTimePeriod.equals(QueryTimePeriodsEnum.EIGHT_HOURS
				.getTimePeriod())) {

			c.add(Calendar.HOUR, -8);

		} else if (selectedTimePeriod
				.equals(QueryTimePeriodsEnum.TWENTY_FOUR_HOURS.getTimePeriod())) {

			c.add(Calendar.HOUR, -24);

		} else if (selectedTimePeriod.equals(QueryTimePeriodsEnum.SEVEN_DAYS
				.getTimePeriod())) {

			c.add(Calendar.DATE, -7);

		} else if (selectedTimePeriod.equals(QueryTimePeriodsEnum.THIRTY_DAYS
				.getTimePeriod())) {

			c.add(Calendar.DATE, -30);

		} else {
			c.setTime(fromDate);
			if (fromTime != null) {
				Calendar timeCal = Calendar.getInstance();
				timeCal.setTime(fromTime);
				c.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
				c.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
			}
		}
		
		return c;

	}
	
	private Calendar formatEndDateTime() {
		
		Calendar c = Calendar.getInstance();

		if (selectedTimePeriod.equals(QueryTimePeriodsEnum.CUSTOM
				.getTimePeriod())) {

			c.setTime(endDate);

			if (fromTime != null) {
				Calendar timeCal = Calendar.getInstance();
				timeCal.setTime(endTime);
				c.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
				c.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
			}
		}

		return c;
		
	}
	
	public String getFromDateTimeAPI() {

		SimpleDateFormat outputFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
		
		Calendar c = formatFromDateTime();

		if(!selectedTimePeriod.equals(QueryTimePeriodsEnum.CUSTOM.getTimePeriod()))
			return outputFormat.format(new Date(c.getTimeInMillis() - (long)(offsetHours * 60 * 60 * 1000)));

		return(outputFormat.format(c.getTime()));		
		
	}
	
	public String getEndDateTimeAPI() {

		SimpleDateFormat outputFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
		
		Calendar c = formatEndDateTime();

		if(!selectedTimePeriod.equals(QueryTimePeriodsEnum.CUSTOM.getTimePeriod()))
			return outputFormat.format(new Date(c.getTimeInMillis() - (long)(offsetHours * 60 * 60 * 1000)));

		return(outputFormat.format(c.getTime()));
				
	}

	public void setFromDateTime(String datetime){
		SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
		try {
			Date d  = format.parse(datetime);
			fromDate = d ;
			fromTime = d ;
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		

	}
	
	public void setEndDateTime(String datetime){
		SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
		try {
			Date d  = format.parse(datetime);
			endDate = d ;
			endTime = d ;
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		

	}
	
	
	
	
	public String getFromDateTime() {

		SimpleDateFormat format = new SimpleDateFormat("MM/dd/yy hh:mm aa");

		Calendar c = formatFromDateTime();

		if(!selectedTimePeriod.equals(QueryTimePeriodsEnum.CUSTOM.getTimePeriod()))
			return format.format(new Date(c.getTimeInMillis() - (long)(offsetHours * 60 * 60 * 1000)));
	
		return format.format(c.getTime());
	
	}

	public String getEndDateTime() {

		SimpleDateFormat format = new SimpleDateFormat("MM/dd/yy hh:mm aa");
		
		Calendar c = formatEndDateTime();

		if(!selectedTimePeriod.equals(QueryTimePeriodsEnum.CUSTOM.getTimePeriod()))
			return format.format(new Date(c.getTimeInMillis() - (long)(offsetHours * 60 * 60 * 1000)));
	
		return format.format(c.getTime());

	}

	public Date getFromDate() {
		return fromDate;
	}

	public Date getEndDate() {
		return endDate;
	}

	public Date getFromTime() {
		return fromTime;
	}

	public Date getEndTime() {
		return endTime;
	}

	@Override
	public Object clone() throws CloneNotSupportedException {
		return super.clone();
	}

	public String getIncludeSQLText() {
		return includeSQLText;
	}

	public void setIncludeSQLText(String includeSQLText) {
		this.includeSQLText = includeSQLText;
	}

	public String getExcludeSQLText() {
		return excludeSQLText;
	}

	public void setExcludeSQLText(String excludeSQLText) {
		this.excludeSQLText = excludeSQLText;
	}

	public boolean isShowSQLStatements() {
		return showSQLStatements;
	}

	public void setShowSQLStatements(boolean showSQLStatements) {
		this.showSQLStatements = showSQLStatements;
	}

	public boolean isShowStoredProcs() {
		return showStoredProcs;
	}

	public void setShowStoredProcs(boolean showStoredProcs) {
		this.showStoredProcs = showStoredProcs;
	}

	public boolean isShowSQLBatches() {
		return showSQLBatches;
	}

	public void setShowSQLBatches(boolean showSQLBatches) {
		this.showSQLBatches = showSQLBatches;
	}

	public boolean isIncludeOverlapping() {
		return includeOverlapping;
	}

	public void setIncludeOverlapping(boolean includeOverlapping) {
		this.includeOverlapping = includeOverlapping;
	}

	public boolean isIncludeIncomplete() {
		return includeIncomplete;
	}

	public void setIncludeIncomplete(boolean includeIncomplete) {
		this.includeIncomplete = includeIncomplete;
	}

	public String getSelectedTimePeriod() {
		return selectedTimePeriod;
	}

	public void setSelectedTimePeriod(String selectedTimePeriod) {
		this.selectedTimePeriod = selectedTimePeriod;
	}

	public ViewMetrics getSelectedView() {
		return selectedView;
	}

	public boolean isDrillDown() {
		return isDrillDown;
	}

	public void setDrillDown(boolean isDrillDown) {
		this.isDrillDown = isDrillDown;
	}

	public void setSelectedView(ViewMetrics selectedView) {
		this.selectedView = selectedView;
	}

	public QueryGroups getSelectedGroupBy() {
		return selectedGroupBy;
	}

	public void setSelectedGroupBy(QueryGroups selectedGroupBy) {
		this.selectedGroupBy = selectedGroupBy;
	}

	public Double getOffsetHours() {
		return offsetHours;
	}

	public void setOffsetHours(Double offsetHours) {
		this.offsetHours = offsetHours;
	}

	public boolean isApplicationAllChecked() {
		return applicationAllChecked;
	}

	public void setApplicationAllChecked(boolean applicationAllChecked) {
		this.applicationAllChecked = applicationAllChecked;
	}

	public boolean isDatabasesAllChecked() {
		return databasesAllChecked;
	}

	public void setDatabasesAllChecked(boolean databasesAllChecked) {
		this.databasesAllChecked = databasesAllChecked;
	}

	public boolean isUsersAllChecked() {
		return usersAllChecked;
	}

	public void setUsersAllChecked(boolean usersAllChecked) {
		this.usersAllChecked = usersAllChecked;
	}

	public boolean isClientsAllChecked() {
		return clientsAllChecked;
	}

	public void setClientsAllChecked(boolean clientsAllChecked) {
		this.clientsAllChecked = clientsAllChecked;
	}

}
