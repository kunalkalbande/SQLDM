package com.idera.sqldm.ui.preferences;

import java.util.Calendar;
import java.util.Date;

import org.zkoss.zul.Combobox;
import org.zkoss.zul.Datebox;
import org.zkoss.zul.Timebox;

public class HistoryPanelPreferencesBean {
	
	public final static String SESSION_VARIABLE_NAME = "HistoryPanelSessionDataBean";

	private Datebox fromdate;
	private Datebox todate;
	private Timebox fromtime;
	private Timebox totime;
	private Date fromDate;
	private Date toDate;
	private Date fromTime;
	private Date toTime;
	private Combobox scaleCombobox;
	private Double offsetHours;
	private int numHistoryMinutes;
	public Datebox getFromdate() {
		return fromdate;
	}
	public void setFromdate(Datebox fromdate) {
		this.fromdate = fromdate;
	}
	public Datebox getTodate() {
		return todate;
	}
	public void setTodate(Datebox todate) {
		this.todate = todate;
	}
	public Timebox getFromtime() {
		return fromtime;
	}
	public void setFromtime(Timebox fromtime) {
		this.fromtime = fromtime;
	}
	public Timebox getTotime() {
		return totime;
	}
	public void setTotime(Timebox totime) {
		this.totime = totime;
	}
	public Combobox getScaleCombobox() {
		return scaleCombobox;
	}
	public void setScaleCombobox(Combobox scaleCombobox) {
		this.scaleCombobox = scaleCombobox;
	}
	public Double getOffsetHours() {
		return offsetHours;
	}
	public void setOffsetHours(Double offsetHours) {
		this.offsetHours = offsetHours;
	}
	public int getNumHistoryMinutes() {
		return numHistoryMinutes;
	}
	public void setNumHistoryMinutes(int numHistoryMinutes) {
		this.numHistoryMinutes = numHistoryMinutes;
	}
	public Date getFromDate() {
		return fromDate;
	}
	public void setFromDate(Date fromDate) {
		this.fromDate = fromDate;
	}
	public Date getToDate() {
		return toDate;
	}
	public void setToDate(Date toDate) {
		this.toDate = toDate;
	}
	public Date getFromTime() {
		return fromTime;
	}
	public void setFromTime(Date fromTime) {
		this.fromTime = fromTime;
	}
	public Date getToTime() {
		return toTime;
	}
	public void setToTime(Date toTime) {
		this.toTime = toTime;
	}
	/**
	 * Combines from date and from time values into a single date object
	 * 
	 * @return
	 */
	public Date getFromDateTime() {
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
	public Date getToDateTime() {
		Calendar endCalendar = Calendar.getInstance();
		endCalendar.setTime(toDate);

		Calendar timeCal = Calendar.getInstance();
		timeCal.setTime(toTime);

		endCalendar
				.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
		endCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));

		return endCalendar.getTime();
	}
	
}
