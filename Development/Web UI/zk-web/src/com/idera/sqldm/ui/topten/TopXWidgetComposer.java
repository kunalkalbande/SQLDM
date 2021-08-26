package com.idera.sqldm.ui.topten;

import java.util.LinkedList;
import java.util.List;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zul.ListModel;
import org.zkoss.zul.ListModelList;

import com.idera.cwf.model.Product;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.TopTenFacade.Top10EventDataBean;
import com.idera.sqldm.data.TopXWidgetFacade;
import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.SQLdmConstants;
import com.idera.sqldm.utils.Utility;

public abstract class TopXWidgetComposer{
	
	private final Logger log = Logger.getLogger(TopXWidgetComposer.class);
	public ListModel<IWidgetInstance> instancesModel;
	public String displayMessageForGrid;
	
	public void init() {
		initTopX();
	}
	
	private void initTopX() {
		setDisplayMessageForGrid(ELFunctions.getMessage(SQLdmI18NStrings.LOADING_DATA));
		EventQueue<Event> eq = EventQueues.lookup(getTopXEnum().getRowCountModEventName(), EventQueues.SESSION, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				Top10EventDataBean top10Dbean = (Top10EventDataBean)event.getData();
            	updateViewModel(getInstances(top10Dbean));
            }
        });
		
		EventQueue<Event> parallelEq = EventQueues.lookup(SQLdmConstants.TOPX_PARALLEL_LOAD_DATA_EVENT_QUEUE, EventQueues.SESSION, true);
		parallelEq.subscribe(new EventListener<Event>() {
			@SuppressWarnings("unchecked")
			public void onEvent(Event event) throws Exception {
				if (event.getName().equals(SQLdmConstants.TOPX_PARALLEL_LOAD_DATA_EVENT + getTopXEnum().toString())) {
					updateViewModel((List<IWidgetInstance>)event.getData());
				}
            }
        });
	}

	public String getDisplayMessageForGrid() {
		return displayMessageForGrid;
	}

	public void setDisplayMessageForGrid(String displayMessageForGrid) {
		this.displayMessageForGrid = displayMessageForGrid;
		BindUtils.postNotifyChange(null, null, this, "displayMessageForGrid");
	}

	public List<IWidgetInstance> getInstances(Top10EventDataBean top10EventDataBean) {
		List<IWidgetInstance> instanceList = new LinkedList<IWidgetInstance>(); 
		
		DashboardPreferencesBean dbpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		String productInstanceName="";
		if(dbpb != null){
		productInstanceName = dbpb.getProductInstanceName();
		}
		if(productInstanceName==null)
    	productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		try {
			instanceList = TopXWidgetFacade.getInstances(productInstanceName, getTopXEnum(), top10EventDataBean.getCount(), top10EventDataBean.getDays());
		} catch (InstanceException e) {
			
			if(e.getMsgKey() !=null){
			WebUtil.showErrorBox(e, e.getMsgKey());
			}else {
				
				WebUtil.showErrorBox(e, SQLdmI18NStrings.ERROR_MESSAGE);
			}
		}
		return instanceList;
	}

	private void updateViewModel(List<IWidgetInstance> list) {
		double max= 0, tempVal;
		ListModelList<IWidgetInstance> listModelList = new ListModelList<IWidgetInstance>();
		try {
			if (list != null && list.size() > 0 ){
				listModelList.addAll(list);
				for(IWidgetInstance instance: list) {
					if (getUtilization(instance) != null) {
						tempVal  = Double.parseDouble(getUtilization(instance));
						if(tempVal > max) {
							max  = tempVal;
						}
					}
				}
				IWidgetInstance instance = list.get(0);
				if (getUtilization(instance) != null && Double.parseDouble(getUtilization(instance)) != max) {
					log.error("The first object is not having the max value");
				}
			} else {
				String emptyMessage = getEmptyMessage();
				setDisplayMessageForGrid(emptyMessage);
				//getWidgetList().setEmptyMessage(I18NUtil.getLocalizedMessage(I18NStrings.INSTANCE_DASHBOARD_NO_INSTANCES_REGISTERED));
			}
			if (max > 0) {
				for (IWidgetInstance instance : list) {
					instance.setRelativeSeverityValue(Double.parseDouble(getUtilization(instance))*100/max);
				}
			}
		} catch (Exception e) {
			log.error("An exception has occured. Ex: " + e.getMessage());
		}
		setInstancesModel(listModelList);
	}
	public String getEmptyMessage() {
		return ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE_SENTENCE_CASE);
	}

	@NotifyChange("instancesModel")
	public void setInstancesModel(ListModel<IWidgetInstance> instancesModel) {
		this.instancesModel = instancesModel;
		BindUtils.postNotifyChange(null, null, this, "instancesModel");
	}
	
	public ListModel<IWidgetInstance> getInstancesModel() {
		return instancesModel;
	}
	@Command("redirectToInstance") 
	public void redirectToInstance (@BindingParam("instanceId") Integer instanceId , @BindingParam("product") Product product){
		if(instanceId != null) {
			InstanceSubCategoriesTab isct = getInstanceSubCategory();
			PreferencesUtil.getInstance().setInstanceTabSubTabs(instanceId, isct);
			Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(product , "singleInstance"+"/"+instanceId));
		}
	}
	
	public abstract String getUtilization(IWidgetInstance instance);
	public abstract TopXEnum getTopXEnum();
	public abstract InstanceSubCategoriesTab getInstanceSubCategory();
}