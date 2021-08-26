package com.idera.sqldm.ui.customDashboard;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Set;

import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.WrongValuesException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkmax.zul.Chosenbox;
import org.zkoss.zul.Button;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Hlayout;
import org.zkoss.zul.Image;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Radio;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Timer;
import org.zkoss.zul.Vlayout;

import com.idera.common.rest.RestException;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.DashboardInstanceFacade;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.alerts.AlertException;
import com.idera.sqldm.data.alerts.AlertFacade;
import com.idera.sqldm.data.alerts.Metrics;
import com.idera.sqldm.data.customdashboard.CustomDashboardFacade;
import com.idera.sqldm.data.customdashboard.Types;
import com.idera.sqldm.data.tags.Tag;
import com.idera.sqldm.data.tags.TagException;
import com.idera.sqldm.data.tags.TagFacade;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.alerts.CustomComboBoxModel;

public class CustomWidgetsComposer extends SelectorComposer<Component>{
	
	private Logger log = Logger.getLogger(CustomWidgetsComposer.class);
	private static final long serialVersionUID = 1L;
	@Wire private Button closeBtn,saveBtn;
	@Wire private Textbox widgetNametxtbox;
	@Wire private Radiogroup radiogroup;
	@Wire private Hlayout customWidgetContainer;
	@Wire private Combobox metric,sourceType,instanceCombobox,indentifierErrorBox;
	@Wire private Chosenbox tagchosenBox,instancechosenBox;
	@Wire private Timer cwTimer;
	@Wire("radio")
	private List<Radio> radioBtns;
	private String productInstanceName;
	private String OFFSET_IN_HOURS = "0.0";
	private Map<String,Integer> instancesMap = new HashMap<>();
	private Map<String,Integer> tagsMap = new HashMap<>();
	private Map<Integer,String> metricIdMap;
	private List<Types> oneSourceType = new ArrayList<>();
	private List<Types> allSourceTypes = new ArrayList<>();
	
	private int customDashboardId;
	private int widgetId = -1;
	private String widgetName;
	private int match;
	private int metricId = -1;
	private int widgetTypeId;
	private List<Integer> tags;
	private List<Integer> serverIds;
	
	
	@SuppressWarnings("unchecked")
	@Override
	public void doAfterCompose(Component comp) throws Exception {
		super.doAfterCompose(comp); 
		productInstanceName = (String)Executions.getCurrent().getArg().get("productInstanceName");
		customDashboardId = (Integer)Executions.getCurrent().getArg().get("customDashboardId");
		
		if(Executions.getCurrent().getArg().containsKey("widgetId")) {
			widgetId = (Integer)Executions.getCurrent().getArg().get("widgetId");
			widgetName = (String)Executions.getCurrent().getArg().get("widgetName");
			match = (Integer)Executions.getCurrent().getArg().get("match");
			metricId = (Integer)Executions.getCurrent().getArg().get("metricId");
			widgetTypeId = (Integer)Executions.getCurrent().getArg().get("widgetTypeId");
			tags = (List<Integer>)Executions.getCurrent().getArg().get("tags");
			serverIds = (List<Integer>)Executions.getCurrent().getArg().get("serverIds");
		}
		
		setOffSet();
		initializeWidgetTypes();
		initializeMetricsComboBox();
		initializeMatchComboBox();
		loadIdentifiers();
		
		
		if(widgetName != null ) {
			widgetNametxtbox.setValue(widgetName);
		}
	}
	
	@Listen("onAfterRender = #metric")
	public void defaultWidgetTypeSelection() {
		if(radiogroup != null && radiogroup.getItems() !=null && 
				radiogroup.getItems().size() <= 0)
			return;
		if(widgetId > 0) {
			for(Radio radio: radioBtns) {
				if(Integer.valueOf(radio.getLabel()) == widgetTypeId) {
					radio.setSelected(true);
					((Vlayout)radiogroup.getSelectedItem().getParent()).setStyle("background:#0054a6;color:#fff;");
				}
			}
		} else {
			radiogroup.setSelectedIndex(0);
			((Vlayout)radiogroup.getSelectedItem().getParent()).setStyle("background:#0054a6;color:#fff;");
		}
	}
	
	@Listen("onAfterRender = #sourceType")
	private void defaultSourceTypeSelection() {
		sourceType.setSelectedIndex(0);
	}
	
	@Listen("onChange = #sourceType")
	public void onSourceTypeSelection(){
		setSourceTypeVisbility((Integer)sourceType.getSelectedItem().getValue());
	}
	
	private void setSourceTypeVisbility(int sourceType) {
		if(sourceType  == 1) {
			instanceCombobox.setVisible(true);
			tagchosenBox.setVisible(false);
			instancechosenBox.setVisible(false);
			indentifierErrorBox.setVisible(true);
			removeIdentifierErrorMsg();
		}
		
		if(sourceType == 2) {
			instanceCombobox.setVisible(false);
			tagchosenBox.setVisible(false);
			instancechosenBox.setVisible(true);
			indentifierErrorBox.setVisible(true);
			removeIdentifierErrorMsg();
		}
		
		if(sourceType == 3) {
			instanceCombobox.setVisible(false);
			tagchosenBox.setVisible(true);
			instancechosenBox.setVisible(false);
			indentifierErrorBox.setVisible(true);
			removeIdentifierErrorMsg();
		}
		
		if(sourceType == 4) {
			instanceCombobox.setVisible(false);
			tagchosenBox.setVisible(false);
			instancechosenBox.setVisible(false);
			indentifierErrorBox.setVisible(false);
			removeIdentifierErrorMsg();
		}
	}
	
	@Listen("onClick = #closeBtn")
	public void onClose(){
		getSelf().detach();
	}
	
	private void loadIdentifiers() {
		try {
			List<DashboardInstance> instances = DashboardInstanceFacade.getDashboardInstances(productInstanceName);
			List<Tag> tagsList = TagFacade.getTags(productInstanceName);
			
			instancesMap = new HashMap<>();
			tagsMap = new HashMap<>();
			HashSet<Integer> tagSet = new HashSet<Integer>();
			HashSet<Integer> serverSet = new HashSet<Integer>();
			//List<String> instanceList = new LinkedList<>();
			//List<String> tagList = new LinkedList<>();
			if(tags != null && !tags.isEmpty()) {
				for(Integer tagid: tags) {
					tagSet.add(tagid);
				}
			}
			
			if(serverIds != null && !serverIds.isEmpty()) {	
				for(Integer serverId: serverIds) {
					serverSet.add(serverId);
				}
			}
			
			ListModelList<String> tagModel = new ListModelList<>();
			ListModelList<String> instanceModel = new ListModelList<>();
			List<String> selectedChosenboxModel = new LinkedList<>();
			ListModelList<String> instanceChosenboxModel = new ListModelList<>();
			
			for(DashboardInstance di:instances){
				instancesMap.put(di.getOverview().getDisplayName(), di.getOverview().getSQLServerId());
				//instanceList.add(di.getOverview().getInstanceName());
				instanceModel.add(di.getOverview().getDisplayName());
				instanceChosenboxModel.add(di.getOverview().getDisplayName());
				
				if(instanceCombobox.isVisible() && serverSet.contains(di.getOverview().getSQLServerId())) {
					instanceModel.addToSelection(di.getOverview().getDisplayName());
				}
				if(instancechosenBox.isVisible() && serverSet.contains(di.getOverview().getSQLServerId())) {
					selectedChosenboxModel.add(di.getOverview().getDisplayName());
				}
			}
			
			List<String> selectedTagsModel = new LinkedList<>();
			for(Tag t: tagsList){
				tagsMap.put(t.getName(), t.getId());
				//tagList.add(t.getName());
				tagModel.add(t.getName());
				if(tagSet.contains(t.getId())) {
					selectedTagsModel.add(t.getName());
				}
			}
			
			tagchosenBox.setModel(tagModel);
			tagchosenBox.setSelectedObjects(selectedTagsModel);
			instancechosenBox.setModel(instanceChosenboxModel);
			instancechosenBox.setSelectedObjects(selectedChosenboxModel);
			instanceCombobox.setModel(instanceModel);
			
			
		} catch (InstanceException | TagException e) {
			e.printStackTrace();
		}
	}

	@Listen("onClick = #saveBtn")
	public void save(){
		clearErrorMessage();
		validateInput();
		try {
			
				int widgetTypeId = Integer.valueOf(radiogroup.getSelectedItem().getLabel());
				int metricId = metric.getSelectedItem().getValue();
				int matchId =  sourceType.getSelectedItem().getValue();
				List<Integer> tagId = new ArrayList<>();
				List<Integer> sourceServerIds = new LinkedList<>();
			
				if(widgetNametxtbox.getValue() != null && !widgetNametxtbox.getValue().equals(""))
					widgetName = widgetNametxtbox.getValue();
				else
					widgetName = metric.getSelectedItem().getLabel();
			
				if(tagchosenBox.isVisible()){
					Set<String> tagSet = tagchosenBox.getSelectedObjects();
					for(String s: tagSet){
						tagId.add(tagsMap.get(s));
					}
				}
			
				if(instanceCombobox.isVisible()) {
					sourceServerIds.add(instancesMap.get(instanceCombobox.getSelectedItem().getValue()));
				} 
				if(instancechosenBox.isVisible()) {
					Set<String> instanceSet = instancechosenBox.getSelectedObjects();
					for(String s: instanceSet){
						sourceServerIds.add(instancesMap.get(s));
					}
				}
			
				if(widgetId < 0) {
					CustomDashboardFacade.createCustomDashboardWidget(productInstanceName, customDashboardId,
							widgetName,widgetTypeId,metricId,matchId,tagId,sourceServerIds);
				} else {
					CustomDashboardFacade.updateCustomdashboardWidget(productInstanceName, customDashboardId, 
							widgetId, widgetName, widgetTypeId, metricId, matchId, tagId, sourceServerIds);
				}
				
				EventQueue<Event> eqForCustomDashboard = EventQueues.lookup(
						"customdashboard", EventQueues.SESSION,
						true);
				eqForCustomDashboard.publish(new Event("customDashboardWidgetAdded"));
			
			
		} catch (RestException e) {
			e.printStackTrace();
		}
		getSelf().detach();
	}
	
	@Listen("onClick = #customWidgetContainer > vlayout")
	public void onCustomSelection(Event evt) {
		List<Component> comp = customWidgetContainer.getChildren();
		for(Component c: comp){
			((Vlayout)c).setStyle("");
		}
		Vlayout v = (Vlayout)evt.getTarget();
		v.setStyle("background:#0054a6;color:#fff;");
		Radio r = (Radio)v.getFirstChild();
		r.setSelected(true);
		
		ListModelList<Types> model = null;
		if(r.getLabel().equals("4") || r.getLabel().equals("5")) {
			model = new ListModelList<>(allSourceTypes);
			model.addToSelection(model.get(0));
			}
		else {
			model = new ListModelList<>(oneSourceType);
			model.addToSelection(model.get(0));
		}
		sourceType.setModel(model);
		setSourceTypeVisbility(1);
	}
	
	
	private void initializeMetricsComboBox(){
		List<CustomComboBoxModel> allMetrics = new ArrayList<>();
		
		try {
			List<Metrics> metrics = AlertFacade.getAllMetrics(productInstanceName , OFFSET_IN_HOURS);
			//sort the list according to name.
			Collections.sort(metrics, new Comparator<Metrics>() {
				@Override
				public int compare(Metrics o1, Metrics o2) {
					return o1.getName().compareTo(o2.getName());
				}
				
			});
			metricIdMap = new HashMap<>();
			List<CustomComboBoxModel> selectedModel = null;
			for(Metrics metric : metrics){
				if(metric.getIsMetricNumeric()) {
					CustomComboBoxModel model = new CustomComboBoxModel(metric.getMetricId(), metric.getName());
					allMetrics.add(model);
					metricIdMap.put(metric.getMetricId(), metric.getName());
					if(metric.getMetricId() == metricId) {
						selectedModel = new LinkedList<CustomComboBoxModel>();
						selectedModel.add(model);
					}
				}
			}
			
			ListModelList<CustomComboBoxModel> metricModel = new ListModelList<>(allMetrics);
			
			if(selectedModel != null)
				metricModel.setSelection(selectedModel);
			metric.setModel(metricModel);
			
		} catch (AlertException e) {
			log.error("AlertException while getting AlertMetrices");
			log.error(e.getMessage(),e);
		} catch (Exception e){
			log.error("Exception while getting AlertMetrices");
			log.error(e.getMessage(),e);
		}
	}
	
	private void initializeMatchComboBox() {
		try {
			allSourceTypes = CustomDashboardFacade.getAllMatchTypes(productInstanceName);
			ListModelList<Types> model = null;
			if(allSourceTypes != null && allSourceTypes.size() > 0) {
				oneSourceType.add(allSourceTypes.get(0));
				
				if(widgetId > 0) {
					if(widgetTypeId == 4 || widgetTypeId == 5) {
						model = new ListModelList<>(allSourceTypes);
						model.addToSelection(model.get(match-1));
						
					} else {
						model = new ListModelList<>(oneSourceType);
						model.addToSelection(model.get(match-1));
					}
					setSourceTypeVisbility(match);
				} else {
					model = new ListModelList<>(oneSourceType);
					model.addToSelection(oneSourceType.get(0));
					setSourceTypeVisbility(1);
				}
				sourceType.setModel(model);
				
			}
		} catch (RestException e) {
			log.error("Exception while getting match types");
			e.printStackTrace();
		}
	}
	
	private void initializeWidgetTypes() {
		try {
			List<Types> widgetTypes = CustomDashboardFacade.getAllWidgetTypes(productInstanceName);
			
			for(Types type: widgetTypes){
				customWidgetContainer.appendChild(createVlayout(type.getId(),type.getValue()));
			}
			
		} catch (RestException e) {
			log.error("Exception while getting widget types");
			e.printStackTrace();
		}
	}
	
	private Vlayout createVlayout(Integer typeId, String typeName) {
		Vlayout v = new Vlayout();
		v.setSclass("custom-widget-radion-btn");
		Radio r = new Radio(typeId.toString());
		r.setSclass("ui-helper-hidden-accessible");
		r.setRadiogroup(radiogroup);
		Label l = new Label(typeName);
		l.setStyle("font-size:10px;");
		Image i = new Image(
				ELFunctions.getImageURLWithoutSize(
						typeName.toLowerCase().replaceAll("\\s+","-")));
		i.setSclass("image");
		i.setHeight("75px");
		v.appendChild(r);
		v.appendChild(l);
		v.appendChild(i);
		return v;
	}
	
	private void validateInput() {
		List<WrongValueException> worngValueEx = new ArrayList<>(3);
		if(metric.getSelectedItem() == null) {
			worngValueEx.add(new WrongValueException(metric,
					"Please select a metric"));
		}
		
		if(sourceType.getSelectedItem() == null) {
			worngValueEx.add(new WrongValueException(sourceType,
					"Please select a source type"));
		}
		
		if(instanceCombobox.isVisible() && instanceCombobox.getSelectedItem() == null) {
			worngValueEx.add(new WrongValueException(indentifierErrorBox,
					"Please select an instance"));
		}
		
		if(tagchosenBox.isVisible() && ( tagchosenBox.getSelectedObjects() == null
				|| tagchosenBox.getSelectedObjects().size() <=0 )) {
			worngValueEx.add(new WrongValueException(indentifierErrorBox,
					"Please select tags"));
		}
		
		if(instancechosenBox.isVisible() && ( instancechosenBox.getSelectedObjects() == null
				|| instancechosenBox.getSelectedObjects().size() <=0 )) {
			worngValueEx.add(new WrongValueException(indentifierErrorBox,
					"Please select instances"));
		}
		if(worngValueEx.size() > 0) {
			WrongValueException[] exArray = new WrongValueException[worngValueEx.size()];
			for(int i=0; i < worngValueEx.size(); i++) {
				exArray[i] = worngValueEx.get(i);
			}
			throw new WrongValuesException(exArray);
		}
	}
	
	private void clearErrorMessage() {
		metric.clearErrorMessage();
		sourceType.clearErrorMessage();
		removeIdentifierErrorMsg();
	}
	
	@Listen("onSelect =  #instancechosenBox,#tagchosenBox,#instanceCombobox")
	public void removeIdentifierErrorMsg() {
		indentifierErrorBox.clearErrorMessage();
	}
	
	@Listen("onClick = #indentifierErrorBox")
	public void openIdentifierBox(){
		if(instancechosenBox.isVisible())
			instancechosenBox.setOpen(true);
		if(tagchosenBox.isVisible())
			tagchosenBox.setOpen(true);
		if(instanceCombobox.isVisible()) {
			instanceCombobox.setOpen(true);
		}
	}
	
	private void setOffSet() {
		Double offSet = null;
		if (Sessions.getCurrent() != null) {
			offSet = new Double((Integer) Sessions.getCurrent().getAttribute(
					WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))
					/ (1000 * 60.0 * 60.0);
			offSet = -offSet;
		}
		if (offSet != null)
			OFFSET_IN_HOURS = offSet.toString();
	}
	
}
