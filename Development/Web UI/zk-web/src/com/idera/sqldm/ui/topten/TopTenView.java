package com.idera.sqldm.ui.topten;


import java.util.LinkedList;
import java.util.List;

import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Popup;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Selectbox;
import org.zkoss.zul.Spinner;

import com.idera.sqldm.data.TopTenFacade.Top10EventDataBean;
import com.idera.sqldm.data.topten.TopXEnum;

public class TopTenView extends TopXComposer {

	private final Logger log = Logger.getLogger(TopTenView.class);
	static final long serialVersionUID = 1L;

	@Wire Spinner responseTimeCount;
	@Wire Button applyRTConfig;
	@Wire Popup rtWidgetPopup;
	@Wire Spinner bsCount;
	@Wire Button applyBSConfig;
	@Wire Popup bsWidgetPopup;
	@Wire Spinner ioCount;
	@Wire Button applyIOConfig;
	@Wire Popup ioWidgetPopup;
	@Wire Spinner tempDBCount;
	@Wire Button applyTEMPDBConfig;
	@Wire Popup tempDBWidgetPopup;
	@Wire Spinner alertCount;
	@Wire Button applyALERTConfig;
	@Wire Popup alertWidgetPopup;
	@Wire Spinner largestDBCount;
	@Wire Button applyLARGESTDBConfig;
	@Wire Popup largestDBWidgetPopup;	
	@Wire Spinner topDBCount;
	@Wire Button applyTOPDBConfig;
	@Wire Popup topDBWidgetPopup;	
	@Wire Spinner cpuCount;
	@Wire Button applyCPUConfig;
	@Wire Popup cpuWidgetPopup;	
	@Wire Spinner activeCount;
	@Wire Button applyACTIVEConfig;
	@Wire Popup activeWidgetPopup;	
	@Wire Spinner muCount;
	@Wire Button applyMUConfig;
	@Wire Popup muWidgetPopup;
	@Wire Spinner fdCount;
	@Wire Button applyFDConfig;
	@Wire Popup fdWidgetPopup;
	@Wire Spinner qmCount;
	@Wire Button applyQMConfig;
	@Wire Popup qmWidgetPopup;	
	@Wire Spinner dsCount;
	@Wire Button applyDSConfig;
	@Wire Popup dsWidgetPopup;	
	@Wire Spinner sesCount;
	@Wire Button applySESConfig;
	@Wire Popup sesWidgetPopup;	
	@Wire Spinner iaCount;
	@Wire Button applyIAConfig;
	@Wire Popup iaWidgetPopup;	
	@Wire Spinner waitCount;
	@Wire Button applyWAITConfig;
	@Wire Popup waitWidgetPopup;	
	@Wire Spinner tsCount;
	@Wire Button applyTSConfig;
	@Wire Popup tsWidgetPopup;
	@Wire Selectbox lrTimeFrameSelectBox;
	@Wire Spinner lrCount;
	@Wire Button applyLRConfig;
	@Wire Popup lrWidgetPopup;

	@Wire int lrTimeFrameSelectBoxIndex;
	
	@SuppressWarnings({ "rawtypes", "unchecked" })
	@Override
	public void doAfterCompose(Component comp) throws Exception {
		super.doAfterCompose(comp);
		
		List<String> timeFrames = new LinkedList<String>();
		for (TopXTimeFrameEnum topXTimeFrameEnum : TopXTimeFrameEnum.values()) {
			timeFrames.add(topXTimeFrameEnum.getMessage());
		}
		
		lrTimeFrameSelectBox.setModel(new ListModelList(timeFrames));
		lrTimeFrameSelectBoxIndex = 1;
		
		updateDefaultValue(TopXEnum.LONGEST_RUNNING_QUERIES, lrTimeFrameSelectBox );
		
		updateDefaultValue(TopXEnum.RESPONSE_TIME_WIDGET, responseTimeCount);
		updateDefaultValue(TopXEnum.BLOCKED_SESSIONS, bsCount);
		updateDefaultValue(TopXEnum.IO, ioCount);
		updateDefaultValue(TopXEnum.TEMPDB_UTILIZATION, tempDBCount);
		updateDefaultValue(TopXEnum.DATABASE_WITH_MOST_ALERTS, alertCount);
		updateDefaultValue(TopXEnum.LARGEST_DATABASES_BY_SIZE, largestDBCount);
		updateDefaultValue(TopXEnum.TOP_DATABASES_BY_ACTIVITY, topDBCount);
		updateDefaultValue(TopXEnum.SQL_CPU_LOAD, cpuCount);
		updateDefaultValue(TopXEnum.MOST_ACTIVE_CONNECTIONS, activeCount);
		updateDefaultValue(TopXEnum.SQL_MEMORY_USAGE, muCount);
		updateDefaultValue(TopXEnum.FASTEST_PROJECTED_GROWING_DATABASES, fdCount);
		updateDefaultValue(TopXEnum.QUERY_MONITOR_EVENTS, qmCount);
		updateDefaultValue(TopXEnum.DISK_SPACE, dsCount);
		updateDefaultValue(TopXEnum.SESSIONS, sesCount);
		updateDefaultValue(TopXEnum.INSTANCE_ALERT, iaCount);
		updateDefaultValue(TopXEnum.WAITS, waitCount);
		updateDefaultValue(TopXEnum.LONGEST_RUNNING_QUERIES, lrCount);
		updateDefaultValue(TopXEnum.NUMBER_OF_SESSIONS, tsCount);
		
	}
	
	@Listen("onClick = #applyRTConfig")
	public void refreshResponseTimeWidget(){
		publishWidgetSettings(TopXEnum.RESPONSE_TIME_WIDGET, null, responseTimeCount, rtWidgetPopup);
	}
	
	@Listen("onClick = #applyBSConfig")
	public void refreshBlockedSessionWidget(){
		publishWidgetSettings(TopXEnum.BLOCKED_SESSIONS, null, bsCount, bsWidgetPopup);
	}
	
	@Listen("onClick = #applyIOConfig")
	public void refreshIOWidget(){
		publishWidgetSettings(TopXEnum.IO, null, ioCount, ioWidgetPopup);
	}
	
	@Listen("onClick = #applyTEMPDBConfig")
	public void refreshtempDBWidget(){
		publishWidgetSettings(TopXEnum.TEMPDB_UTILIZATION, null, tempDBCount, tempDBWidgetPopup);
	}
	
	@Listen("onClick = #applyALERTConfig")
	public void refreshAlertWidget(){
		publishWidgetSettings(TopXEnum.DATABASE_WITH_MOST_ALERTS, null, alertCount, alertWidgetPopup);
	}
	
	@Listen("onClick = #applyLARGESTDBConfig")
	public void refreshLargestDBWidget(){
		publishWidgetSettings(TopXEnum.LARGEST_DATABASES_BY_SIZE, null, largestDBCount, largestDBWidgetPopup);
	}
	
	@Listen("onClick = #applyTOPDBConfig")
	public void refreshTopDBWidget(){
		publishWidgetSettings(TopXEnum.TOP_DATABASES_BY_ACTIVITY, null, topDBCount, topDBWidgetPopup);
	}
	
	@Listen("onClick = #applyCPUConfig")
	public void refreshCpuWidget(){
		publishWidgetSettings(TopXEnum.SQL_CPU_LOAD, null, cpuCount, cpuWidgetPopup);
	}
	
	@Listen("onClick = #applyACTIVEConfig")
	public void refreshActiveWidget(){
		publishWidgetSettings(TopXEnum.MOST_ACTIVE_CONNECTIONS, null, activeCount, activeWidgetPopup);
	}
	
	@Listen("onClick = #applyMUConfig")
	public void refreshMUWidget(){
		publishWidgetSettings(TopXEnum.SQL_MEMORY_USAGE, null, muCount, muWidgetPopup);
	}

	@Listen("onClick = #applyFDConfig")
	public void refreshFDWidget(){
		publishWidgetSettings(TopXEnum.FASTEST_PROJECTED_GROWING_DATABASES, null, fdCount, fdWidgetPopup);
	}
	
	@Listen("onClick = #applyQMConfig")
	public void refreshQMWidget(){
		publishWidgetSettings(TopXEnum.QUERY_MONITOR_EVENTS, null, qmCount, qmWidgetPopup);
	}
	
	@Listen("onClick = #applyDSConfig")
	public void refreshDSWidget(){
		publishWidgetSettings(TopXEnum.DISK_SPACE, null, dsCount, dsWidgetPopup);
	}
	
	@Listen("onClick = #applySESConfig")
	public void refreshSESWidget(){
		publishWidgetSettings(TopXEnum.SESSIONS, null, sesCount, sesWidgetPopup);
	}
	
	@Listen("onClick = #applyIAConfig")
	public void refreshIAWidget(){
		publishWidgetSettings(TopXEnum.INSTANCE_ALERT, null, iaCount, iaWidgetPopup);
	}
	
	@Listen("onClick = #applyWAITConfig")
	public void refreshWaitWidget(){
		publishWidgetSettings(TopXEnum.WAITS, null, waitCount, waitWidgetPopup);
	}
	
	@Listen("onClick = #applyLRConfig")
	public void refreshLRWidget() {
		publishWidgetSettings(TopXEnum.LONGEST_RUNNING_QUERIES, lrTimeFrameSelectBox, lrCount, lrWidgetPopup);
	}
	
	@Listen("onClick = #applyTSConfig")
	public void refreshTSWidget(){
		publishWidgetSettings(TopXEnum.NUMBER_OF_SESSIONS, null, tsCount, tsWidgetPopup);
	}
}
