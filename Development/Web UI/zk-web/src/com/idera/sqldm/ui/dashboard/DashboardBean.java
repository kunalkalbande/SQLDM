package com.idera.sqldm.ui.dashboard;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.LinkedList;
import java.util.List;

import com.idera.common.rest.CoreRestClient;
import com.idera.common.rest.RestException;
import com.idera.cwf.model.Product;
import com.idera.cwf.model.RestCoreInstance;
import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.SeverityCodeToStringEnum;
import com.idera.sqldm.rest.SQLDMRestClient;

public class DashboardBean {
	private int maxSeverity;
	private String name;
	private SeverityCodeToStringEnum sctse;
	private List<DashboardInstance> instances;

	public DashboardBean() {
		super();
	}

	public DashboardBean(int maxSeverity, List<DashboardInstance> instances, String name, SeverityCodeToStringEnum sctse) {
		super();
		this.maxSeverity = maxSeverity;
		if (instances == null) {
			this.instances = new LinkedList<DashboardInstance>();
		} else {
			/*
			 * ChaitanyaTanwar - Changes for SWA launch from DM
			 * */
			List<RestCoreInstance> listAllCWFInstances = null;
			try {
				listAllCWFInstances = CoreRestClient.getInstance().getInstances();
			} catch (RestException e) {
				e.printStackTrace();
			}
			for(int k=0; k < instances.size(); k++){
				DashboardInstance dashboardInstance = new DashboardInstance();
				dashboardInstance = instances.get(k);
				for(int i=0; i < listAllCWFInstances.size(); i++){
					RestCoreInstance restCoreInstance = new RestCoreInstance();
					restCoreInstance = listAllCWFInstances.get(i);
					if(restCoreInstance.getName().equals(dashboardInstance.getOverview().getInstanceName())){
						List<Product> productList = restCoreInstance.getProducts();
						for(int j=0; j<productList.size(); j++){
							Product product = productList.get(j);
							dashboardInstance.setIsSWAInstance(false);
							if(product.getProductNameWithoutInstanceName().equals("SQLWorkloadAnalysis")){
								dashboardInstance.setIsSWAInstance(true);
								dashboardInstance.setSwaID(product.getProductId());
								break;
							}
						}
					}
				}
			}
			this.instances = instances;
		}
		
		this.name = name;
		this.sctse = sctse;
	}

	public DashboardBean(int maxSeverity, String name) {
		super();
		this.maxSeverity = maxSeverity;
		this.name = name;
	}

	public int getMaxSeverity() {
		return maxSeverity;
	}

	public void setMaxSeverity(int maxSeverity) {
		this.maxSeverity = maxSeverity;
	}

	public String getCount() {
		if (instances == null) {
			return "?";
		}
		return String.valueOf(instances.size());
	}

	public List<DashboardInstance> getInstances() {
		return instances;
	}

	public void setInstances(List<DashboardInstance> instances) {
		this.instances = instances;
	}

	public void createNewInstancesList() {
		this.instances = new LinkedList<DashboardInstance>();
	}

	public void addInstance(DashboardInstance instance) {
		if (this.instances == null) {
			createNewInstancesList();
		}
		this.instances.add(instance);
	}
	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public SeverityCodeToStringEnum getSctse() {
		return sctse;
	}
}
