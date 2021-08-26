package com.idera.sqldm.ui.dashboard.instances.databases;

import org.apache.log4j.Logger;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Include;
import org.zkoss.zul.Toolbar;

import com.idera.common.Utility;
import com.idera.common.rest.RestException;
import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.DashboardInstanceFacade;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.ui.dashboard.instances.InstanceCategoryTab;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm.ui.dashboard.instances.SubCategoryViewModel;

public class InstanceDatabasesViewModel extends SubCategoryViewModel{

	private final static Logger log = Logger.getLogger(InstanceDatabasesViewModel.class);
	@Wire private Include contentView;
	@Wire private Toolbar databasesTb;
	private DashboardInstance instance;
	private int instanceId;
		
	@AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view){
		super.afterCompose(view);
		
		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "id");
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		} else {
			//fallback
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
			if(param != null){
				instanceId = (Integer) param;
			}
		}
		Executions.getCurrent().getDesktop().setAttribute("instanceId", instanceId);
    	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

		try {
			instance= DashboardInstanceFacade.getDashboardInstance(productInstanceName, instanceId);
		} catch (InstanceException x) {
			log.error(x.getMessage(), x);
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	@Init
    public void init(@ContextParam(ContextType.VIEW) Component view){
		super.init(view);
	}
	
	@Override
	public Include getContentView() {
		return contentView;
	}

	@Override
	public Toolbar getToolbar() {
		return databasesTb;
	}

	@Override
	public InstanceCategoryTab getInstanceCategoryTab() {
		return InstanceCategoryTab.DATABASES;
	}
	@Override
	public boolean haveSubTab(){
		return true;
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
		// override and empty implementation
	}

	public boolean getIsSQLDB2012OrGreater() {
		if (instance.getOverview() != null) {
			String version = instance.getOverview().getProductVersion();
			if (version != null) {
				int start = version.indexOf(".");
				if (start != -1) {
					String majorVersion = version.substring(0, version.indexOf("."));
					try {
						// WHEN '8.' THEN '2000'
						// WHEN '9.' THEN '2005'
						// WHEN '10' THEN '2008'
						// WHEN '11' THEN '2012'
						return Integer.parseInt(majorVersion) >= 11;
					} catch (Exception ex) {
						log.error("Failed to determine major version: ", ex);
					}
				}
			}
		}
		return false;
	}
}
