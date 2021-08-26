package com.idera.sqldm.ui.dashboard.instances.queries;

import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Include;
import org.zkoss.zul.Toolbar;

import com.idera.sqldm.ui.dashboard.instances.InstanceCategoryTab;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm.ui.dashboard.instances.SubCategoryViewModel;

public class InstanceQueriesViewModel extends SubCategoryViewModel{

	@Wire Include queryContentView;
	@Wire Toolbar queriesTb;

	@AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view){
		super.afterCompose(view);
	}
	
	@Init
    public void init(@ContextParam(ContextType.VIEW) Component view){
		super.init(view);
	}
	
	@Override
	public boolean haveSubTab(){
		return true;
	}
	
	@Override
	public Include getContentView() {
		return queryContentView;
	}

	@Override
	public Toolbar getToolbar() {
		return queriesTb;
	}

	@Override
	public InstanceCategoryTab getInstanceCategoryTab() {
		return InstanceCategoryTab.QUERIES;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategoryTab() {
		return InstanceSubCategoriesTab.QUERIES_SUMMARY;
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
	
}
