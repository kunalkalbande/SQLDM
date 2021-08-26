package com.idera.sqldm.ui.alerts;

import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;

public class AlertFilter implements Cloneable{
	static AlertFilter DEFAULT_FILTER = new AlertFilter(true);
	
	private boolean active;
	private Date fromDate;
	private Date toDate;
	private Date fromTime;
	private Date toTime;
	private Integer instanceId = -1;
	private Integer metricId = -1;
	private Integer severity = -1;
	private Integer productId = -1;
	public static HashMap<Integer, String> productMapping = new HashMap<Integer , String>();
	
	public AlertFilter() {
	}
	
	public AlertFilter(boolean isActive) {
		this.active = isActive;
	}
	
	public boolean isActive() {
		return active;
	}
	public void setActive(boolean isActive) {
		this.active = isActive;
	}
	public void setFromDate(Date fromDate) {
		this.fromDate = fromDate;
	}
	public void setToDate(Date toDate) {
		this.toDate = toDate;
	}
	
	public void setFromTime(Date fromTime) {
		this.fromTime = fromTime;
	}
	public void setToTime(Date toTime) {
		this.toTime = toTime;
	}
	
	public Date getFromDateTime(){
		if(fromDate == null){
			return null;
		}
		Calendar c = Calendar.getInstance();
		c.setTime(fromDate);
		if(fromTime != null){
			Calendar timeCal = Calendar.getInstance();
			timeCal.setTime(fromTime);
			c.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
			c.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
		}
		return c.getTime();
	}
	
	public Date getToDateTime(){
		if(toDate == null){
			return null;
		}
		Calendar c = Calendar.getInstance();
		c.setTime(toDate);
		if(fromTime != null){
			Calendar timeCal = Calendar.getInstance();
			timeCal.setTime(toTime);
			c.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
			c.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
		}
		return c.getTime();
	}
	
	public Integer getInstanceId() {
		return instanceId;
	}
	public void setInstanceId(Integer instanceId) {
		this.instanceId = instanceId;
	}

	public Integer getProductId() {
		return productId;
	}

	public void setProductId(Integer productId) {
		this.productId = productId;
	}

	public Integer getMetricId() {
		return metricId;
	}
	public void setMetricId(Integer metricId) {
		this.metricId = metricId;
	}
	public Integer getSeverity() {
		return severity;
	}
	public void setSeverity(Integer severity) {
		this.severity = severity;
	}
	
	public Date getFromDate() {
		return fromDate;
	}

	public Date getToDate() {
		return toDate;
	}

	public Date getFromTime() {
		return fromTime;
	}

	public Date getToTime() {
		return toTime;
	}


	@Override
	protected Object clone() throws CloneNotSupportedException {
		return super.clone();
	}
}
