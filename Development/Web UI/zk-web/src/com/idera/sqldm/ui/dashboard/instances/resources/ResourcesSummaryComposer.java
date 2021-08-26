package com.idera.sqldm.ui.dashboard.instances.resources;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.CategoryModel;

import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.category.resources.ResourceCategory;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.utils.SQLdmConstants;
import com.idera.sqldm.utils.Utility;

@SuppressWarnings("rawtypes")
public class ResourcesSummaryComposer extends SelectorComposer{

	private static final long serialVersionUID = -7275998545314218112L;
	private long OFFSET_MILLIS = 0;
	@Wire private IderaLineChart sqlMemoryUsage;
	@Wire private IderaLineChart sqlCPUUsage;
	@Wire private IderaLineChart sqlPhysicalIO;
	@Wire private IderaLineChart cacheHitRatios;

	@SuppressWarnings("unchecked")
	@Override
	public void doAfterCompose(Component comp) throws Exception {
		super.doAfterCompose(comp);
		OFFSET_MILLIS = Utility.cancelOffSetInMillis();
        sqlMemoryUsage.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_SQL_MEMORY_USAGE_IN_MB));
        sqlCPUUsage.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_CPU_USAGE_PERCENTAGE));
        sqlPhysicalIO.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_SQL_SERVER_PHYSICAL_IO));
        cacheHitRatios.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_CACHE_HIT_RATIOS_PERCENT));
		
        refreshModels();
	}
	
	private void refreshModels() throws CategoryException{
		Map<String, CategoryModel> modelsMap = getModelData();
		sqlMemoryUsage.setModel(modelsMap.get("SQL_MEMORY"));
		sqlCPUUsage.setModel(modelsMap.get("SQL_CPU"));
		sqlPhysicalIO.setModel(modelsMap.get("SQL_IO"));
		cacheHitRatios.setModel(modelsMap.get("CACHE_HIT"));
	}
	
	@SuppressWarnings("unchecked")
	private Map<String, CategoryModel> getModelData() throws CategoryException {
		Object resourceCategoryListObj = Executions.getCurrent().getDesktop().getAttribute(SQLdmConstants.DASHBOARD_SCOPE_SINGLE_INSTANCE_RESOURCE);
		List<ResourceCategory> resourceCategoryList = (List<ResourceCategory>) resourceCategoryListObj;
		Map<String, CategoryModel> modelsMap = new HashMap<String, CategoryModel>();
		
		if (resourceCategoryList != null && resourceCategoryList.size()>1) {
		IderaLineChartModel sqlCPUModel = new IderaLineChartModel();
		CategoryModel sqlMemoryModel = new IderaLineChartModel();
		CategoryModel sqlIOModel = new IderaLineChartModel();
		CategoryModel cacheHitModel = new IderaLineChartModel();
		
		modelsMap.put("SQL_MEMORY", sqlMemoryModel);
		modelsMap.put("SQL_CPU", sqlCPUModel);
		modelsMap.put("SQL_IO", sqlIOModel);
		modelsMap.put("CACHE_HIT", cacheHitModel);
		
			for(int i=resourceCategoryList.size()-1; i>=0; i-- ){
				ResourceCategory resource = resourceCategoryList.get(i);
				//Adding Offset for graphs
				long x_value = resource.getUTCCollectionDateTime().getTime() + OFFSET_MILLIS;
				sqlCPUModel.setValue("OS", x_value, resource.getCpu().getOSCPUUsage());
				sqlCPUModel.setValue("SQL", x_value, resource.getCpu().getSqlCPUUsage());
				
				sqlMemoryModel.setValue("SQL Used", x_value, (resource.getMemory().getSqlMemory().get("UsedMemoryInKB"))/1024.0);
				sqlMemoryModel.setValue("SQL Allocated", x_value, (resource.getMemory().getSqlMemory().get("AllocatedMemoryInKB"))/1024.0);
				sqlMemoryModel.setValue("Total Used", x_value, (resource.getMemory().getSqlMemory().get("TotalMemoryInKB"))/1024.0);
				
				sqlIOModel.setValue("Checkpoint Writes", x_value, resource.getDisk().getSqlPhysicalIO().get("CheckPointWrites"));
				sqlIOModel.setValue("Lazy Writer Writes", x_value, resource.getDisk().getSqlPhysicalIO().get("LazyWriterWrites"));
				sqlIOModel.setValue("Read Ahead Reads", x_value, resource.getDisk().getSqlPhysicalIO().get("ReadAheadPages"));
				sqlIOModel.setValue("Page Reads", x_value, resource.getDisk().getSqlPhysicalIO().get("PageReads"));
				
				cacheHitModel.setValue("Buffer Cache", x_value, resource.getMemory().getBufferCacheHitRatio());
				cacheHitModel.setValue("Procedure Cache", x_value, resource.getMemory().getProcedureCacheHitRatio());
			}
		}else{
			
			setEmptyCharts();
		}

		return modelsMap;
	}
	
	private void setEmptyCharts() {
		
		sqlMemoryUsage.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		sqlPhysicalIO.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		cacheHitRatios.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		sqlCPUUsage.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
	}
}
