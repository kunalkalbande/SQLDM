package com.idera.sqldm.ui.dashboard;

import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.apache.log4j.Logger;
import org.zkoss.json.JSONArray;
import org.zkoss.json.JSONObject;
import org.zkoss.json.parser.JSONParser;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.A;
import org.zkoss.zul.Div;
import org.zkoss.zul.Image;
import org.zkoss.zul.Include;
import org.zkoss.zul.Label;

import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.SeverityCodeToStringEnum;
import com.idera.sqldm.ui.components.charts.heatmap.IderaHeatmapChart;
import com.idera.sqldm.ui.dashboard.DashboardInstanceViewComposer.FilterType;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.SQLdmConstants;



public class HeatmapViewComposer extends SelectorComposer<Component>{
	private static final long serialVersionUID = 9200286224241425901L;
	
	private final Logger log = Logger.getLogger(DashboardInstanceViewComposer.class);
	@Wire private IderaHeatmapChart heatmapChart;
	@Wire private Div heatmapChartDiv;
	@Wire private Label errorMsg;
	private Div outerDiv = null;
	
	

	@Override
	public void doAfterCompose(Component comp) throws Exception {
		super.doAfterCompose(comp);
		
		EventQueue<Event> eq = EventQueues.lookup(
				SQLdmConstants.DASHBOARD_UPDATE_INSTANCES_EVENT_QUEUE,
				EventQueues.DESKTOP, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				leftNavigationListener(event, event.getName());
            }
        });
		
		EventQueue<Event> eqForDashboardRefresh = EventQueues.lookup(DashboardConstants.DASHBOARD_QUEUE_NAME, EventQueues.SESSION, true);
		eqForDashboardRefresh.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if(DashboardConstants.DASHBOARD_REFRESH_EVENT_NAME.equalsIgnoreCase(event.getName())) {
					loadInstances(true);
				}
				if(event.getName().equals("ClosePopup")){
					detachDiv();
				}
            }
        });
		
		//Subscribing to Change Product Filter
		EventQueue<Event> productQueue = EventQueues.lookup("changeProduct",
				EventQueues.DESKTOP, true);
		productQueue.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if(event.getName().equals("productChanged")){
					loadInstances(true);
				}
			}
		});
		
	}
	
	
	private void leftNavigationListener(Event event, String eventName) {
    	if("onStatusLinkClick".equals(eventName)){
    		String severity = (String) event.getData();
    		filterInstances(FilterType.STATUS, severity);
        } else if("onTagsLinkClick".equals(eventName)){
    		String tag = (String) event.getData();
    		filterInstances(FilterType.TAG, tag);        	
        } else if("onSearch".equals(eventName)){
        	filterInstances(FilterType.SEARCH, "");
        }
    }
	
	@SuppressWarnings("unchecked")
	private void filterInstances(FilterType filterType, String filterId) {
		detachDiv();
		Map<String, DashboardBean> map = null;
		List<DashboardInstance> selectedLeftNavigationFilteredInstances;
		try {
			switch (filterType) {
			case STATUS:
				map = (Map<String, DashboardBean>) (Executions.getCurrent()
						.getDesktop()
						.getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_SEVERITIES));
				break;
			case TAG:
				map = (Map<String, DashboardBean>) Executions
						.getCurrent()
						.getDesktop()
						.getAttribute(
								SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_TAGS);
				break;
			}
			if (!FilterType.SEARCH.equals(filterType) && (map == null || map.get(filterId) == null)) {
				filterId = "ALL";
				map = (Map<String, DashboardBean>) (Executions.getCurrent()
						.getDesktop()
						.getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_SEVERITIES));
			}
			
			List<DashboardInstance> tList = null;
			if(FilterType.SEARCH.equals(filterType)){
				tList = (LinkedList<DashboardInstance>) Executions
						.getCurrent()
						.getDesktop()
						.getAttribute(
								SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_SEARCH);
			} else {
				tList = map.get(filterId).getInstances();
			}
			if (tList != null) {
				selectedLeftNavigationFilteredInstances = new LinkedList<>(tList);
			} else {
				selectedLeftNavigationFilteredInstances = new LinkedList<>();
			}
			refreshInstances(selectedLeftNavigationFilteredInstances);
		} catch (Exception ex) {
			log.error(ex.getMessage(), ex);
		}
	}

	
	public void refreshInstances(List<DashboardInstance> dashboardInstances){
		if(dashboardInstances == null || dashboardInstances.isEmpty()){
			errorMsg.setVisible(true);
			heatmapChartDiv.setVisible(false);
			return;
		}
		errorMsg.setVisible(false);
		heatmapChartDiv.setVisible(true);
		heatmapChart.getChart().setData(getJSONArray(dashboardInstances));
	}
	
	@SuppressWarnings("unchecked")
	public void loadInstances(boolean reload){
		FilterType ft = FilterType.STATUS;
        String name = SeverityCodeToStringEnum.ALL.getUiLabel();
        DashboardPreferencesBean dsdb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
        if (dsdb != null) {
        	if (dsdb.getLeftCategoryGroup() != null) {
        		ft = dsdb.getLeftCategoryGroup().getFilterType();
        	}
        	if (dsdb.getLeftCategoryGroup() != null) {
        		name = dsdb.getLeftCategoryGroup().getName();
        	}
        	/* if(dsdb.getSearchText() != null) {
        		setSearchText(dsdb.getSearchText());
        	} else {
        		setSearchText("");
        	} */
        }
        filterInstances(ft, name);
		/*List<DashboardInstance> dashboardInstances = (LinkedList<DashboardInstance>) (Executions
				.getCurrent().getDesktop()
				.getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_INSTANCES_LIST));
		
		JSONArray ar = getJSONArray(dashboardInstances);
		
		heatmapChart.getChart().setVisible(true);
		heatmapChart.getTitleDiv().setVisible(false);
		heatmapChart.getErrorDiv().setVisible(false);
		heatmapChart.getChart().setData(ar);*/
	}
	
	private JSONArray getJSONArray(List<DashboardInstance> dashboardInstances){
		JSONArray ar = new JSONArray();
		JSONObject obj = null;
		if( dashboardInstances!= null && !dashboardInstances.isEmpty()) {
			for(DashboardInstance instance: dashboardInstances){
				obj = new JSONObject();
				obj.put("productId", instance.getProduct().getProductId());
				obj.put("id", instance.getOverview().getSQLServerId());
				obj.put("idx", instance.getServerStatus().getHealthIndex()/100 );
//				obj.put("label", instance.getOverview().getInstanceName()); @author Saumyadeep
				obj.put("label", instance.getOverview().getDisplayName());
				obj.put("state", instance.getServerStatus().getSeverityString().toLowerCase());
				ar.add(obj);
			}
		}
		log.info(ar);
		return ar;
	}
	
	@SuppressWarnings("unchecked")
	@Listen("onServerBoxClick = #heatmapChartDiv")
	public void onServerBoxClick(Event evt) {
		
		if(evt.getData() == null) {
			detachDiv();
			return;
		}
		List<DashboardInstance> instanceList = (LinkedList<DashboardInstance>) Executions.getCurrent().getDesktop().getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_INSTANCES_LIST);
		JSONObject jsonObject = (JSONObject)new JSONParser().parse((String)evt.getData());
		Integer instanceID = Integer.parseInt(jsonObject.get("id").toString());
		Integer swaID =0;
		for(int i=0;i<instanceList.size();i++){
			DashboardInstance instance = instanceList.get(i);
			if(instance.getOverview().getSQLServerId()==instanceID){
				swaID = instance.getSwaID();
			}
		}
		StringBuilder template = new StringBuilder("~./sqldm/com/idera/sqldm/ui/dashboard/serverDetailsPopup.zul");
		template.append("?id=").append(jsonObject.get("id"));
		template.append("&name=").append(jsonObject.get("name"));
		template.append("&idx=").append(jsonObject.get("idx"));
		template.append("&state=").append(jsonObject.get("state"));
		template.append("&swaID=").append(swaID);
		Include in = new Include("includeId");
		in.setSrc(template.toString());
		outerDiv = new Div();
		Div closeDiv = new Div();
		Div dmConsoleDiv = new Div();
		A dmConsoleUrl = new A();
		
		
		Image closeImg = new Image("~./sqldm/com/idera/sqldm/images/close-small.png");
		Image dmConsoleImg = new Image("~./sqldm/com/idera/sqldm/images/sqldmicon12x12.png");
		Image arrowUpImg = new Image();
		Image arrowDownImg = new Image();
		arrowUpImg.setClass("arrowIcon");
		arrowDownImg.setClass("arrowIconDown");
		dmConsoleUrl.setHref("Idera://instance?instanceid="+jsonObject.get("id"));
		dmConsoleUrl.setTooltiptext("Launch DM Console");
		arrowUpImg.setSrc("~./sqldm/com/idera/sqldm/images/hoverbox-up-arrow-white-"+jsonObject.get("state")+".png");
		arrowDownImg.setSrc("~./sqldm/com/idera/sqldm/images/hoverbox-down-arrow-white.png");
		
		if(((String)jsonObject.get("arrowIconDown")).equals("none")){
			arrowUpImg.setStyle("display: block;"+jsonObject.get("pos")+":15px");
			arrowDownImg.setStyle("display: none;"+jsonObject.get("pos")+":15px");
		} else {
			arrowUpImg.setStyle("display: none;"+jsonObject.get("pos")+":15px");
			arrowDownImg.setStyle("display: block;"+jsonObject.get("pos")+":15px");
		}
		
		outerDiv.appendChild(arrowUpImg);
		closeDiv.setClass("close");
		closeDiv.appendChild(closeImg);
		dmConsoleUrl.appendChild(dmConsoleImg);
		dmConsoleDiv.setClass("launchDm");
		dmConsoleDiv.appendChild(dmConsoleUrl);
		outerDiv.setClass("hoverBoxContainerOuter");
		outerDiv.appendChild(closeDiv);
		outerDiv.appendChild(dmConsoleDiv);
		outerDiv.appendChild(in);
		outerDiv.appendChild(arrowDownImg);
		outerDiv.setPage(this.getSelf().getPage());
		outerDiv.setWidth("232px");
		outerDiv.setTop(jsonObject.get("top")+"px");
		outerDiv.setLeft(jsonObject.get("left")+"px");
		closeImg.addEventListener("onClick", (new EventListener<Event>(){
			public void onEvent(Event event) throws Exception {
				detachDiv();
            }
        }));
		
	}
	
	
	private void detachDiv(){
		if(outerDiv != null){
			outerDiv.detach();
		}
		heatmapChart.getChart().setHide();
	}
	
}