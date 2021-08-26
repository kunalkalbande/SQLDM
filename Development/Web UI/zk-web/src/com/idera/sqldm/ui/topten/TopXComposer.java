package com.idera.sqldm.ui.topten;


import java.util.ArrayList;
import java.util.Arrays;
import java.util.LinkedList;
import java.util.List;

import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Popup;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Selectbox;
import org.zkoss.zul.Spinner;
import org.zkoss.zul.Timer;

import com.idera.sqldm.data.TopTenFacade.Top10EventDataBean;
import com.idera.sqldm.data.TopXWidgetFacade;
import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.io.executor.ParallelExecutions;
import com.idera.sqldm.io.executor.ParallelExecutionsPDU;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.SQLdmConstants;

public class TopXComposer extends SelectorComposer<Component> {

	private final Logger log = Logger.getLogger(TopXComposer.class);
	static final long serialVersionUID = 1L;

	@Wire protected Timer timer;
	
	String productInstanceName;

	@SuppressWarnings({ "rawtypes", "unchecked" })
	@Override
	public void doAfterCompose(Component comp) throws Exception {
		super.doAfterCompose(comp);
		
    	/*String productInstanceParameter=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
    	if(productInstanceParameter != null) {
    		productInstanceName = productInstanceParameter;
    	} else {
    		Object param = Executions.getCurrent().getDesktop().getAttribute("instance");
			if(param != null){
				productInstanceName = (String) param;
			}
    	}*/
		DashboardPreferencesBean dbpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();

		if(dbpb != null){
		productInstanceName = dbpb.getProductInstanceName();
		}
		

		if(getTimer() != null){
			getTimer().addEventListener(Events.ON_TIMER, new EventListener() {
				public void onEvent(Event evt) {
					try {
						loadTopXWidgetsData(productInstanceName, null);
					} catch (Exception e) {
						log.error(e.getMessage(), e);
					}
				}
			});
		}
		
		EventQueue<Event> eq = EventQueues.lookup("updateInstanceWidgets",
				EventQueues.DESKTOP, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if (event.getName().equals("refreshData")) {
					List<TopXEnum> teList = (List<TopXEnum>) event.getData();
					loadTopXWidgetsData(productInstanceName, teList);
				}
			}
		});
		
		//Subscribing to Change Product Filter
		EventQueue<Event> productQueue = EventQueues.lookup("changeProduct",
				EventQueues.DESKTOP, true);
		productQueue.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if(event.getName().equals("productChanged")){
					DashboardPreferencesBean dbpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();

					if(dbpb != null){
					productInstanceName = dbpb.getProductInstanceName();
					}
					loadTopXWidgetsData(productInstanceName, null);
				}
			}
		});
		
	}
	
	public Timer getTimer() {
		return timer;
	}

	private void loadTopXWidgetsData(final String productInstanceName, List<TopXEnum> teList) throws Exception {
		
		if(teList == null)
			teList = new ArrayList<TopXEnum>(Arrays.asList(TopXEnum.values()));
		
		final EventQueue<Event> eq = EventQueues.lookup(SQLdmConstants.TOPX_PARALLEL_LOAD_DATA_EVENT_QUEUE, EventQueues.SESSION, true);
    	if(eq != null){
    		ParallelExecutions pe = new ParallelExecutions();
			for (final TopXEnum te: teList) {
				final String eventName = SQLdmConstants.TOPX_PARALLEL_LOAD_DATA_EVENT + te.toString();
				int rowCount = te.getRowDefaultCount();
				if(Executions.getCurrent() != null && Executions.getCurrent().getSession() != null) {
					Object obj = Executions.getCurrent().getSession().getAttribute(te.getRowCountVariableName()); 
					if(obj != null) {
						rowCount = (int)obj;
					}
				}
				final int rowCountForWidget = rowCount;

				ParallelExecutionsPDU pePDU = new ParallelExecutionsPDU() {
					@Override
					public void taskComplete(PDUBean returnObject) {}
					
					@Override
					public PDUBean task() {
						List<IWidgetInstance> instanceList = new LinkedList<IWidgetInstance>(); 
						try {
							instanceList = TopXWidgetFacade.getInstances(productInstanceName, te, rowCountForWidget, te.getRowDefaultDateTime());
						} catch (Exception e) {
							log.error(e.getMessage(), e);
						}
						eq.publish(new Event(eventName, null, instanceList));
						return null;
					}
				};
				pe.addToCallable(pePDU);
        	}
			pe.invokeAll();
			
        }
	}
	
	@SuppressWarnings({ "unchecked", "rawtypes" })
	protected void updateDefaultValue(TopXEnum enumTopXEnum, Selectbox box) {
		Integer days = (Integer) Executions.getCurrent().getSession().getAttribute(enumTopXEnum.getRowTimeDaysVariableName());
		
		if (days != null) {
			for (int i=0; i< box.getModel().getSize(); i++) {
				String message = (String) box.getModel().getElementAt(i);
				TopXTimeFrameEnum topXTimeFrameEnum = TopXTimeFrameEnum.getDaysFromEnum(message);
				if (topXTimeFrameEnum != null) {
					if ((int) days == topXTimeFrameEnum.getDays()) {
//						box.setSelectedIndex(i);
//						lrTimeFrameSelectBoxIndex = i;
						if (box != null) {
							((ListModelList) (box.getModel())).addToSelection(message);
						}
						break;
					}
				}
			}
		} else {
			if (box != null) {
				
				((ListModelList) (box.getModel())).addToSelection((String) box.getModel().getElementAt(1));
			}
		}
	}
	
	@SuppressWarnings("unused")
	protected void updateDefaultValue(TopXEnum enumTopXEnum, Spinner countSpinner, Radiogroup radioGroup) {
		Integer limit = (Integer) Executions.getCurrent().getSession().getAttribute(enumTopXEnum.getRowCountVariableName());
		if (limit == null) {
			limit = enumTopXEnum.getRowDefaultCount();
		}else if(limit == -1 && radioGroup != null) {
			radioGroup.setSelectedIndex(0);
			limit = enumTopXEnum.getRowDefaultCount();
			countSpinner.setDisabled(true);
		}
		if (countSpinner != null) {
			countSpinner.setValue(limit);
		}
	}
	
	protected void updateDefaultValue(TopXEnum enumTopXEnum, Spinner countSpinner) {
		Integer limit = (Integer) Executions.getCurrent().getSession().getAttribute(enumTopXEnum.getRowCountVariableName());
		if (limit == null) {
			limit = enumTopXEnum.getRowDefaultCount();
		} 
		if (countSpinner != null) {
			countSpinner.setValue(limit);
		}
	}
	
	protected void publishWidgetSettings(TopXEnum enumTopXEnum, Selectbox box, Spinner countSpinner, Popup popup) {
		int days = enumTopXEnum.getRowDefaultDateTime();
		int count = enumTopXEnum.getRowDefaultCount();
		
		if (box != null && box.getSelectedIndex() != -1) {
			String message = (String)  box.getModel().getElementAt(box.getSelectedIndex());
			
			TopXTimeFrameEnum topXTimeFrameEnum = TopXTimeFrameEnum.getDaysFromEnum(message);
			if (topXTimeFrameEnum != null) {
				days = topXTimeFrameEnum.getDays();
			}
		}
		
		if( countSpinner.getValue() != null){
			count = countSpinner.getValue();
			if (count > enumTopXEnum.getRowMaxCount()) {
				count = -1;
			}
		}
		updateWidgetData(enumTopXEnum, popup, count, days);
		/*Executions.getCurrent().getSession().setAttribute(enumTopXEnum.getRowCountVariableName(), count);
		Executions.getCurrent().getSession().setAttribute(enumTopXEnum.getRowTimeDaysVariableName(), days);
		EventQueue<Event> eq = EventQueues.lookup(enumTopXEnum.getRowCountModEventName(), EventQueues.SESSION, false);
		if(eq != null){
        	Top10EventDataBean t10EBean = new Top10EventDataBean(count, days);
        	eq.publish(new Event("onClick", null, t10EBean));
        }
		
		popup.close();*/
	}
	
	protected void updateWidgetData(TopXEnum enumTopXEnum, Popup popup, int count, int days) {
		Executions.getCurrent().getSession().setAttribute(enumTopXEnum.getRowCountVariableName(), count);
		Executions.getCurrent().getSession().setAttribute(enumTopXEnum.getRowTimeDaysVariableName(), days);
		EventQueue<Event> eq = EventQueues.lookup(enumTopXEnum.getRowCountModEventName(), EventQueues.SESSION, false);
		if(eq != null) {
        	Top10EventDataBean t10EBean = new Top10EventDataBean(count, days);
        	eq.publish(new Event("onClick", null, t10EBean));
        }
		
		popup.close();		
	}
	
	@SuppressWarnings("unused")
	protected void onclosePopup(TopXEnum te, Radiogroup rg, Spinner spinner){
		//init with 0 or any +ve since default selection is # of records
		int rowCount = 0;
		Object obj = Executions.getCurrent().getSession().getAttribute(te.getRowCountVariableName()); 
		if(obj != null) {
			rowCount = (int)obj;
		}
		if(rowCount != -1){
			rg.setSelectedIndex(1);
			spinner.setDisabled(false);
		}
		else{
			rg.setSelectedIndex(0);
			spinner.setDisabled(true);
		}
	}
		
}
