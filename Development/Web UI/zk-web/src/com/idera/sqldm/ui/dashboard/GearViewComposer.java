
package com.idera.sqldm.ui.dashboard;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.UiException;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.event.ForwardEvent;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Template;
import org.zkoss.zul.Button;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Constraint;
import org.zkoss.zul.Decimalbox;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Hlayout;
import org.zkoss.zul.Include;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModel;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Row;
import org.zkoss.zul.Rows;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import com.idera.common.rest.CoreRestClient;
import com.idera.common.rest.RestException;
import com.idera.cwf.model.Product;
import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.alerts.Alert;
import com.idera.sqldm.rest.SQLDMRestClient;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.server.web.session.SessionUtil;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.Utility;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean.DashboardGroupByInstances;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean.DashboardInstanceView;

public class GearViewComposer extends SelectorComposer<Window> {
	private String selected_productInstanceName;
	private String productInstanceName;
	private ListModel<InstanceScaleFactorList> instanceModel;
	private ListModel<InstanceScaleFactorList> instanceDropdownModel;
	private ListModel<TagsScaleFactorList> tagModel;
	private ListModel<TagsScaleFactorList> tagDropdownModel;
	private HealthIndexCoefficients hc;
	private List<InstanceScaleFactorList> instanceSetList = new ArrayList<InstanceScaleFactorList>();
	private List<InstanceScaleFactorList> instanceDropdownList = new ArrayList<InstanceScaleFactorList>();
	private List<TagsScaleFactorList> tagSetList = new ArrayList<TagsScaleFactorList>();
	private List<TagsScaleFactorList> tagDropdownList = new ArrayList<TagsScaleFactorList>();
	public List<InstanceScaleFactorList> instanceList = new ArrayList<InstanceScaleFactorList>();
	public List<TagsScaleFactorList> tagList = new ArrayList<TagsScaleFactorList>();
	private List<GearViewBean> gvbList = new ArrayList<GearViewBean>(); 
	private String selected;
	private boolean save=true;
	@Wire
	private Decimalbox test;
	@Wire
	private Row critical;
	@Wire
	private Row warning;
	@Wire
	private Row informational;
	@Wire
	private Hlayout critical_hlayout;
	@Wire
	private Hlayout warning_hlayout;
	@Wire
	private Hlayout informational_hlayout;
	@Wire
	private Label critical_lbl;
	@Wire
	private Label warning_lbl;
	@Wire
	private Label informational_lbl;
	@Wire
	private Textbox critical_txt;
	@Wire
	private Textbox warning_txt;
	@Wire
	private Textbox informational_txt;
	@Wire
	private Window win;
	@Wire
	private Grid instanceGrid;
	@Wire
	private Grid tagGrid;
	@Wire
	private Grid Main;
	@Wire
	private Rows rows;
	@Wire
	private Combobox instanceCombobox;
	@Wire
	private Combobox tagCombobox;
	@Wire
	private Button Add;
	@Wire
	private Button addTag;
	@Wire
	private Textbox setInstance_txt;
	@Wire
	private Textbox setTag_txt;
	@Wire
	private Hlayout tagLayout;
	@Wire
	private Hlayout instanceLayout;

	@Init
	public void init() throws RestException {
	}

	public GearViewComposer() throws RestException {
		GearViewBean gvb=new GearViewBean();
		selected_productInstanceName=PreferencesUtil.getInstance().getDashboardPreferencesInSession().getProductInstanceName();
		
		try {
			gvbList=SQLDMRestClient.getInstance().getScaleFactor(selected_productInstanceName);
			if(gvbList.size()==1)
			{
				gvb=gvbList.get(0);
				instanceList=gvb.getInstanceList();
				tagList=gvb.getTagsList();
			}
			if(gvbList.size()>1)
			{int i=0;
				while(i<gvbList.size())
				{
				List<InstanceScaleFactorList> instanceList1 =  gvbList.get(i).getInstanceList();
				if (instanceList1 != null) {
					instanceList.addAll(instanceList1);
				}
				
				List<TagsScaleFactorList> tagList1 = gvbList.get(i).getTagsList();
				if(tagList1!=null)
				{
					tagList.addAll(tagList1);
				}
				i++;
			}
			}
			if(gvbList.size()>0){
				hc=gvbList.get(0).getHealthIndexCoefficients();
			}
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	
		//GearViewBean gvb = SQLDMRestClient.getInstance().getScaleFactor(productInstanceName);

		/* GearViewBean gvb = new GearViewBean(); */
		if (instanceList != null) {
			for (InstanceScaleFactorList element : instanceList) {
				if (element != null) {
					boolean flag = element.isIsInstanceHealthScaleFactorSet();
					if (selected != null && element.getDisplayName().equals(selected)) {
						flag = !flag;
					}
					if (flag) {
						instanceSetList.add(element);
					} else {
						element.setInstanceHealthScaleFactor(null);
						instanceDropdownList.add(element);
					}
				}
			}
		}
		instanceModel = new ListModelList<InstanceScaleFactorList>(instanceSetList);
		instanceDropdownModel = new ListModelList<InstanceScaleFactorList>(instanceDropdownList);

		if (tagList != null) {
			for (TagsScaleFactorList element : tagList) {
				if (element.isIsTagHealthScaleFactorSet())
					tagSetList.add(element);
				else {
					element.setTagHealthScaleFactor(null);
					tagDropdownList.add(element);
				}
			}
		}
		tagModel = new ListModelList<TagsScaleFactorList>(tagSetList);
		tagDropdownModel = new ListModelList<TagsScaleFactorList>(tagDropdownList);

	}

	@Override
	public void doAfterCompose(Window comp) throws Exception {

		super.doAfterCompose(comp);
		if (hc != null) {
			critical_txt.setValue("" + hc.getCritical());
			informational_txt.setValue("" + hc.getInformational());
			warning_txt.setValue("" + hc.getWarning());
		}
		if (tagDropdownList.size() < 1) {
			addTag.setDisabled(true);
			setTag_txt.setDisabled(true);
			tagCombobox.setDisabled(true);
		}
		if (instanceDropdownList.size() < 1) {
			Add.setDisabled(true);
			setInstance_txt.setDisabled(true);
			instanceCombobox.setDisabled(true);
		}
		if (instanceList.isEmpty()) {
			instanceGrid.detach();
			instanceLayout.detach();
		}
		if (tagList.isEmpty()) {
			tagGrid.detach();
			tagLayout.detach();
		}
		instanceSelected();
		tagSelected();
	}

	public ListModel<InstanceScaleFactorList> getInstanceModel() {
		return instanceModel;
	}

	public ListModel<InstanceScaleFactorList> getInstanceDropdownModel() {
		return instanceDropdownModel;
	}

	public ListModel<TagsScaleFactorList> getTagDropdownModel() {
		return tagDropdownModel;
	}

	public ListModel<TagsScaleFactorList> getTagModel() {
		return tagModel;
	}

	@Listen("onClick = #navigateToHeatmap")
	public void navigateToHeatmap() {
		win.detach();
		EventQueue<Event> eq = EventQueues.lookup("navigateToHeatmap", EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("navigate"));
		}
	}

	@Listen("onClick = #Add")
	public void addBtnClicked() {
		selected = instanceCombobox.getValue();
		int selected_id=instanceCombobox.getSelectedItem().getValue();
		Double value = Double.parseDouble(setInstance_txt.getValue());
		instanceCombobox.setPlaceholder("Choose");
		instanceCombobox.setValue("");

		/*
		 * GearViewBean gvb = new GearViewBean(); instanceList =
		 * gvb.getInstanceList();
		 */
		// saveInstances();

		if (instanceGrid.getChildren().size() > 0) {
			Rows rows = (Rows) instanceGrid.getChildren().get(0);
			List<Component> comps = rows.getChildren();
			for (Component component : comps) {
				Row r = (Row) component;
				Label lbl = (Label) r.getFirstChild();
				String displayName = lbl.getValue();
				Hlayout hlay = (Hlayout) r.getFirstChild().getNextSibling();
				Textbox txt = (Textbox) hlay.getFirstChild();
				Label lbl1= (Label) r.getFirstChild().getNextSibling().getNextSibling().getNextSibling();
				String row_id=lbl1.getValue();
				if (!row_id.equals(selected_id))
					txt.clearErrorMessage();
				for (InstanceScaleFactorList element : instanceList) {
					if ((""+element.getSQLServerId()).equals(row_id)) {
						try {
							element.setInstanceHealthScaleFactor(Double.parseDouble(txt.getValue()));
						} catch (NumberFormatException e) {
							throw new WrongValueException(txt, "Please enter a Number.");
						}
					}
				}
			}
		}

		/*for (InstanceScaleFactorList element : instanceList) {
			if (element != null && instanceDropdownList.size() > 0) {
				if (element.getDisplayName().equals(selected)) {
					element.setIsInstanceHealthScaleFactorSet(true);
					element.setInstanceHealthScaleFactor(value);
					instanceSetList.add(element);
					instanceDropdownList.remove(element);
				}
			}
		}*/

		for (InstanceScaleFactorList element : instanceList) {
			if (element != null && instanceDropdownList.size() > 0) {
				if (element.getSQLServerId()==selected_id) {
					element.setIsInstanceHealthScaleFactorSet(true);
					element.setInstanceHealthScaleFactor(value);
					instanceSetList.add(element);
					instanceDropdownList.remove(element);
					break;
				}
			}
		}
		
		setInstance_txt.setValue("1");
		setInstance_txt.setDisabled(true);
		Add.setDisabled(true);
		if (instanceDropdownList.size() < 1) {
			Add.setDisabled(true);
			setInstance_txt.setDisabled(true);
			instanceCombobox.setDisabled(true);
		}
		instanceModel = new ListModelList<InstanceScaleFactorList>(instanceSetList);
		instanceDropdownModel = new ListModelList<InstanceScaleFactorList>(instanceDropdownList);
		instanceGrid.setModel(instanceModel);
		instanceCombobox.setModel(instanceDropdownModel);
	}

	@Listen("onSelect = #tagCombobox")
	public void tagSelected() {
		if (tagCombobox.getValue() == null || tagCombobox.getValue() == "Choose" || tagCombobox.getValue() == "" ) {
			setTag_txt.setDisabled(true);
			addTag.setDisabled(true);
		} else {
			setTag_txt.setDisabled(false);
			addTag.setDisabled(false);
		}
	}

	@Listen("onSelect = #instanceCombobox")
	public void instanceSelected() {
		if (instanceCombobox.getValue() == null || instanceCombobox.getValue() == "Choose" || instanceCombobox.getValue() == "" ) {
			setInstance_txt.setDisabled(true);
			Add.setDisabled(true);
		} else {
			setInstance_txt.setDisabled(false);
			Add.setDisabled(false);
			
		}
	}

	public void saveInstances() {

		if (instanceGrid.getChildren().size() > 0) {
			Rows rows = (Rows) instanceGrid.getChildren().get(0);
			List<Component> comps = rows.getChildren();
			for (Component component : comps) {
				Row r = (Row) component;
				Label lbl = (Label) r.getFirstChild();
				Hlayout hlay = (Hlayout) r.getFirstChild().getNextSibling();
				Textbox txt = (Textbox) hlay.getFirstChild();
				System.out.println(txt.getValue());
				Label lbl1= (Label) r.getFirstChild().getNextSibling().getNextSibling().getNextSibling();
				String row_id=lbl1.getValue();
				//txt.setConstraint("");
				for (InstanceScaleFactorList element : instanceList) {
					if (element.getSQLServerId()==Integer.parseInt(row_id)) {
						element.setIsInstanceHealthScaleFactorSet(true);
						try {
							if (Double.parseDouble(txt.getValue()) < 0 || Double.parseDouble(txt.getValue()) > 10) {
								throw new WrongValueException(txt, "Scale Factor should lie between 0 and 10.");
							}
							element.setInstanceHealthScaleFactor(Double.parseDouble(txt.getValue()));
						} catch (NumberFormatException e) {
							save=false;
							throw new WrongValueException(txt, "Please enter a Number.");
						}
					}
					/*if(selected_productInstanceName.equalsIgnoreCase("All"))
					{
						element.setSQLServerId(element.getSQLServerId()%1000);
					}*/
				}
			}
		}
	}

	public void saveTags() {
		if (tagGrid.getChildren().size() > 0) {
			Rows tagRows = (Rows) tagGrid.getChildren().get(0);
			List<Component> Tagcomps = tagRows.getChildren();
			for (Component component : Tagcomps) {
				Row r = (Row) component;
				Label lbl = (Label) r.getFirstChild();
				String displayName = lbl.getValue();
				Hlayout hlay = (Hlayout) r.getFirstChild().getNextSibling();
				Textbox txt = (Textbox) hlay.getFirstChild();
				System.out.println(txt.getValue());
				Label lbl1= (Label) r.getFirstChild().getNextSibling().getNextSibling().getNextSibling();
				String row_id=lbl1.getValue();
				for (TagsScaleFactorList element : tagList) {
					if (element.getTagId()==Integer.parseInt(row_id)) {
						element.setIsTagHealthScaleFactorSet(true);
						element.setTagHealthScaleFactor(Double.parseDouble(txt.getValue()));
					}
					/*if(selected_productInstanceName.equalsIgnoreCase("All"))
					element.setTagId(element.getTagId()%1000);*/
				}
				
			}
		}
	}

	@Listen("onClick = #addTag")
	public void addTagBtnClicked() {
		selected = tagCombobox.getValue();
		Double value = Double.parseDouble(setTag_txt.getValue());
		int selected_id=tagCombobox.getSelectedItem().getValue();
		tagCombobox.setPlaceholder("Choose");
		tagCombobox.setValue("");

		/*
		 * GearViewBean gvb = new GearViewBean(); tagList = gvb.getTagsList();
		 */

		// saveTags();

		if (tagGrid.getChildren().size() > 0) {
			Rows tagRows = (Rows) tagGrid.getChildren().get(0);
			List<Component> Tagcomps = tagRows.getChildren();
			for (Component component : Tagcomps) {
				Row r = (Row) component;
				Label lbl = (Label) r.getFirstChild();
				String displayName = lbl.getValue();
				Hlayout hlay = (Hlayout) r.getFirstChild().getNextSibling();
				Textbox txt = (Textbox) hlay.getFirstChild();
				Label lbl1= (Label) r.getFirstChild().getNextSibling().getNextSibling().getNextSibling();
				String row_id=lbl1.getValue();
				if (!row_id.equals(selected_id))
					txt.clearErrorMessage();
				for (TagsScaleFactorList element : tagList) {
					if ((""+element.getTagId()).equals(row_id)) {
						element.setTagHealthScaleFactor(Double.parseDouble(txt.getValue()));
					}
				}
			}
		}
		for (TagsScaleFactorList element : tagList) {
			if (element != null && tagDropdownList.size() > 0) {
				if (element.getTagId()==selected_id) {
					element.setIsTagHealthScaleFactorSet(true);
					element.setTagHealthScaleFactor(value);
					tagSetList.add(element);
					tagDropdownList.remove(element);
					break;
				}
			}
		}
		setTag_txt.setValue("1");
		setTag_txt.setDisabled(true);
		if (tagDropdownList.size() < 1) {
			addTag.setDisabled(true);

			tagCombobox.setDisabled(true);
		}
		tagModel = new ListModelList<TagsScaleFactorList>(tagSetList);
		tagDropdownModel = new ListModelList<TagsScaleFactorList>(tagDropdownList);
		tagGrid.setModel(tagModel);
		tagCombobox.setModel(tagDropdownModel);
	}

	@Listen("onDisable = #instanceGrid")
	public void disableInstanceBtnClicked(ForwardEvent evt) {

		String disabled = evt.getData().toString();

		/*
		 * GearViewBean gvb = new GearViewBean(); instanceList =
		 * gvb.getInstanceList();
		 */
		// saveInstances();

		if (instanceGrid.getChildren().size() > 0) {
			Rows rows = (Rows) instanceGrid.getChildren().get(0);
			List<Component> comps = rows.getChildren();

			for (Component component : comps) {
				Row r = (Row) component;
				
				Label lbl = (Label) r.getFirstChild();
				
				String displayName = lbl.getValue();
				Hlayout hlay = (Hlayout) r.getFirstChild().getNextSibling();
				Textbox txt = (Textbox) hlay.getFirstChild();
				//System.out.println("txt_value: "+txt.getValue());
				Label lbl1= (Label) r.getFirstChild().getNextSibling().getNextSibling().getNextSibling();
				String row_id=lbl1.getValue();
				if (!row_id.equals(disabled))
					txt.clearErrorMessage();
				
					
				for (InstanceScaleFactorList element : instanceList) {
					if ( row_id.equals(disabled) && element.getSQLServerId()==Integer.parseInt(row_id) ) {
						Double value;
						try {
							value = Double.parseDouble(txt.getValue());
						} catch (NumberFormatException e) {
							throw new WrongValueException(txt, "Please enter a Number.");
						}
						if (value < 0 || value > 10) {
							throw new WrongValueException(txt, "Scale Factor should lie between 0 and 10.");
						}
						element.setIsInstanceHealthScaleFactorSet(false);
						element.setInstanceHealthScaleFactor(null);
						instanceDropdownList.add(element);	
						instanceSetList.remove(element);
					}
					if (!row_id.equals(disabled) && element.getSQLServerId()==Integer.parseInt(row_id)) {
						element.setInstanceHealthScaleFactor(Double.parseDouble(txt.getValue()));
					}

				}
			}
		}

		instanceModel = new ListModelList<InstanceScaleFactorList>(instanceSetList);
		instanceDropdownModel = new ListModelList<InstanceScaleFactorList>(instanceDropdownList);
		instanceGrid.setModel(instanceModel);
		instanceCombobox.setModel(instanceDropdownModel);

		if (instanceDropdownList.size() != 0) {
			instanceCombobox.setDisabled(false);
		}
	}

	@Listen("onDisabletag = #tagGrid")
	public void disableTagBtnClicked(ForwardEvent evt) {

		String disabled = evt.getData().toString();

		/*
		 * GearViewBean gvb = new GearViewBean(); tagList = gvb.getTagsList();
		 */

		if (tagGrid.getChildren().size() > 0) {
			Rows tagRows = (Rows) tagGrid.getChildren().get(0);
			List<Component> Tagcomps = tagRows.getChildren();
			for (Component component : Tagcomps) {
				Row r = (Row) component;
				Label lbl = (Label) r.getFirstChild();
				String displayName = lbl.getValue();
				Hlayout hlay = (Hlayout) r.getFirstChild().getNextSibling();
				Textbox txt = (Textbox) hlay.getFirstChild();
				Label lbl1= (Label) r.getFirstChild().getNextSibling().getNextSibling().getNextSibling();
				String row_id=lbl1.getValue();
				if (!row_id.equals(disabled))
					txt.clearErrorMessage();
				for (TagsScaleFactorList element : tagList) {
					if (row_id.equals(disabled) && element.getTagId()==Integer.parseInt(row_id) ) {
						Double value;
						try {
							value = Double.parseDouble(txt.getValue());
						} catch (NumberFormatException e) {
							throw new WrongValueException(txt, "Please enter a Number.");
						}
						if (value < 0 || value > 10) {
							throw new WrongValueException(txt, "Scale Factor should lie between 0 and 10.");
						}
						element.setIsTagHealthScaleFactorSet(false);
						element.setTagHealthScaleFactor(null);
						tagDropdownList.add(element);
						tagSetList.remove(element);
					}
					if (!row_id.equals(disabled) && element.getTagId()==Integer.parseInt(row_id)) {
						element.setTagHealthScaleFactor(Double.parseDouble(txt.getValue()));
					}
				}
			}
		}

		tagModel = new ListModelList<TagsScaleFactorList>(tagSetList);
		tagDropdownModel = new ListModelList<TagsScaleFactorList>(tagDropdownList);
		tagGrid.setModel(tagModel);
		tagCombobox.setModel(tagDropdownModel);
		if (tagDropdownList.size() != 0) {
			tagCombobox.setDisabled(false);
		}
	}

	@Listen("onClick = #Close")
	public void closeBtnClicked() {
		win.detach();
	}

	@Listen("onClick = #Submit")
	public void submitBtnClicked() throws RestException {
		//setInstance_txt.clearErrorMessage();
		//setTag_txt.clearErrorMessage();
		// Saving the values for Alerts,Instances,Tags
		hc.setCritical(Double.parseDouble(critical_txt.getValue()));
		hc.setWarning(Double.parseDouble(warning_txt.getValue()));
		hc.setInformational(Double.parseDouble(informational_txt.getValue()));
		
		if(setInstance_txt.getErrorMessage()!=null)
		{
			throw new WrongValueException(setInstance_txt, setInstance_txt.getErrorMessage());
		}
		
		if(setTag_txt.getErrorMessage()!=null)
		{
			throw new WrongValueException(setTag_txt, setTag_txt.getErrorMessage());
		}
		saveInstances();
		saveTags();
		/// -----------------------

		// what values we are passing to update API------
	/*	System.out.println("Critical: " + hc.getCritical());
		System.out.println("Warning: " + hc.getWarning());
		System.out.println("Informational: " + hc.getInformational());

		for (InstanceScaleFactorList element : instanceList) {
			if (element != null) {
				System.out.println(element.getDisplayName() +  "ID: " + element.getSQLServerId() +  " value: " + element.getInstanceHealthScaleFactor());
			}
		}

		for (TagsScaleFactorList element : tagList) {
			if (element != null) {
				System.out.println(element.getTagName() + " value: " + element.getTagHealthScaleFactor());
			}
		}*/
		//// ------------
		SQLDMRestClient.getInstance().updateScaleFactor(selected_productInstanceName, hc, instanceList, tagList);
		win.detach();
	}
}
