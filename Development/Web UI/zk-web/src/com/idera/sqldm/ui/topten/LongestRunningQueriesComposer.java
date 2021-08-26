package com.idera.sqldm.ui.topten;

import java.util.Comparator;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.TopXWidgetFacade;
import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.LongestRunningQueryInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;

/**
 * This class is controller Longest Running QUery widget in Top X page.
 * @author Varun
 *
 */
public class LongestRunningQueriesComposer extends TopXWidgetComposer {

	
	private Comparator<LongestRunningQueryInstance> cpuTimeAscComparator;
	private Comparator<LongestRunningQueryInstance> cpuTimeDescComparator;
	private Comparator<LongestRunningQueryInstance> queryTimeAscComparator;
	private Comparator<LongestRunningQueryInstance> queryTimeDescComparator;
	
	
	@Init
	public void init() {
		super.init();
		setCpuTimeAscComparator(TopXWidgetFacade.CPU_TIME_ASC);
		setCpuTimeDescComparator(TopXWidgetFacade.CPU_TIME_DESC);
		setQueryTimeAscComparator(TopXWidgetFacade.QUERY_EXEC_TIME_ASC);
		setQueryTimeDescComparator(TopXWidgetFacade.QUERY_EXEC_TIME_DESC);
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return new Double(((LongestRunningQueryInstance)instance).getQueryExecTimeInMs()).toString();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.LONGEST_RUNNING_QUERIES;
	}

	public Comparator<LongestRunningQueryInstance> getCpuTimeAscComparator() {
		return cpuTimeAscComparator;
	}

	public void setCpuTimeAscComparator(
			Comparator<LongestRunningQueryInstance> cpuTimeAscComparator) {
		this.cpuTimeAscComparator = cpuTimeAscComparator;
	}

	public Comparator<LongestRunningQueryInstance> getCpuTimeDescComparator() {
		return cpuTimeDescComparator;
	}

	public void setCpuTimeDescComparator(
			Comparator<LongestRunningQueryInstance> cpuTimeDescComparator) {
		this.cpuTimeDescComparator = cpuTimeDescComparator;
	}

	public Comparator<LongestRunningQueryInstance> getQueryTimeAscComparator() {
		return queryTimeAscComparator;
	}

	public void setQueryTimeAscComparator(
			Comparator<LongestRunningQueryInstance> queryTimeAscComparator) {
		this.queryTimeAscComparator = queryTimeAscComparator;
	}

	public Comparator<LongestRunningQueryInstance> getQueryTimeDescComparator() {
		return queryTimeDescComparator;
	}

	public void setQueryTimeDescComparator(
			Comparator<LongestRunningQueryInstance> queryTimeDescComparator) {
		this.queryTimeDescComparator = queryTimeDescComparator;
	}	

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.QUERIES_SUMMARY;
	}

	@Override
	public String getEmptyMessage() {
		return ELFunctions.getMessage(SQLdmI18NStrings.TOPTEN_QUERY_DATA_NOT_AVAILABLE);
	}
}
