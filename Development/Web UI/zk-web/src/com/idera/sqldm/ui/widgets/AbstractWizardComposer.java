package com.idera.sqldm.ui.widgets;

import java.util.List;

import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Button;
import org.zkoss.zul.Div;
import org.zkoss.zul.Hlayout;
import org.zkoss.zul.Panelchildren;
import org.zkoss.zul.Vlayout;
import org.zkoss.zul.Window;

import com.idera.i18n.I18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.WebUtil;

public abstract class AbstractWizardComposer extends SelectorComposer<Window> {

	static final long serialVersionUID = 1L;
	
	public static final String ACTIVE_ITEM_SCLASS = "wizard-item-active";
	
	public static final String PROGRESS_INCOMPLETE_SCLASS = "wizard-item-progress-incomplete";
	public static final String PROGRESS_COMPLETED_SCLASS = "wizard-item-progress-complete";
	
	/* StepComponents is just a class to make it easy to pass around the components associated
	 * with each step in the wizard.
	 */
	public static class StepComponents { 
		
		protected Div tipsContent;
		protected Div mainPanelContent;
		protected Vlayout itemContent; 
				
		public StepComponents(Div tipsContent, Div mainPanelContent, Vlayout itemContent) { 
			this.tipsContent = tipsContent;
			this.mainPanelContent = mainPanelContent;
			this.itemContent = itemContent;
		}
		
		public Div getTipsContent() {
			return tipsContent;
		}
		
		public Div getMainPanelContent() {
			return mainPanelContent;
		}
		
		public Vlayout getItemContent() {
			return itemContent;
		}

		@Override
		public boolean equals(Object obj) {			
			if( obj == null ) return false;
			if( !(obj instanceof StepComponents) ) return false;			
			StepComponents o = (StepComponents) obj;
			
			if( tipsContent == null ) { 
				if( o.getTipsContent() != null ) {
					return false;
				}
			}
			else if( o.getTipsContent() == null ) { 
				return false;
			}
			else if( !tipsContent.equals(o.getTipsContent()) ) { 
				return false;
			}

			if( mainPanelContent == null ) { 
				if( o.getMainPanelContent() != null ) {
					return false;
				}
			}
			else if( o.getMainPanelContent() == null ) { 
				return false;
			}
			else if( !mainPanelContent.equals(o.getMainPanelContent()) ) { 
				return false;
			}

			if( itemContent == null ) { 
				if( o.getItemContent() != null ) {
					return false;
				}
			}
			else if( o.getItemContent() == null ) { 
				return false;
			}
			else if( !itemContent.equals(o.getItemContent()) ) { 
				return false;
			}
			
			return true;
		}
		
		@Override
		public int hashCode() {
			int c = 37;
			int i = 17;
			i = i * c + (tipsContent != null ? tipsContent.hashCode() : 0);
			i = i * c + (itemContent != null ? itemContent.hashCode() : 0);
			i = i * c + (mainPanelContent != null ? mainPanelContent.hashCode() : 0);
			return i;
		}
	}
	
	@Wire protected Div tipsContent;
	@Wire protected Panelchildren mainPanelChildren;
	@Wire protected Hlayout wizardItemsLayout;
	@Wire protected Button closeButton;
	@Wire protected Button prevButton;
	@Wire protected Button nextButton;
	@Wire protected Button saveButton;
	
	@Listen("onClick = button#closeButton")
	public void closeWindow() {
		if (!showWarningOnClose())
			getSelf().detach();
		else if (WebUtil.showConfirmationBoxWithWarning(I18NStrings.ARE_YOU_SURE_YOU_WANT_TO_CLOSE_THE_WIZARD, I18NStrings.CLOSE_WIZARD))
			getSelf().detach();
	}
	
	@Listen("onClick = button#nextButton")
	public void next() {
		StepComponents nextStep = getNextStep();
		if( nextStep != null ) activate(nextStep);
	}
	
	@Listen("onClick = button#prevButton")
	public void prev() {
		StepComponents prevStep = getPrevStep();
		if( prevStep != null ) activate(prevStep);
	}
	
	@Listen("onClick = button#saveButton") 
	public void save() { 
		Clients.showBusy(ELFunctions.getLabel(I18NStrings.SAVING));
		Events.echoEvent("onDoSave", this.getSelf(), null);
	}
	
	protected void clearActiveStep() { 
		
		List<Vlayout> stepItems = WebUtil.getChildrenByClass(wizardItemsLayout, Vlayout.class);
		
		for( Vlayout stepItem : stepItems ) { 
			
			String sclass = stepItem.getSclass();
			sclass = sclass.replaceAll(ACTIVE_ITEM_SCLASS, "");
			stepItem.setSclass(sclass.trim());
			
		}
		
	}
	
	protected void hideAllCenterContent() { 
		hideAllImmediateChildDivs(mainPanelChildren);
	}

	protected void hideAllTipContent() { 
		hideAllImmediateChildDivs(tipsContent);		
	}
	
	protected void hideAllImmediateChildDivs(Component comp) { 

		List<Div> divs = WebUtil.getChildrenByClass(comp, Div.class, false);
		
		for( Div div : divs ) { 
			div.setVisible(false);
		}
		
	}
	
	protected void activate(StepComponents step) { 
		
		clearActiveStep();
		hideAllCenterContent();
		hideAllTipContent();
		
		String itemSclass = step.getItemContent().getSclass();
		
		if( itemSclass.indexOf(ACTIVE_ITEM_SCLASS) == -1 ) { 
			itemSclass = itemSclass + " " + ACTIVE_ITEM_SCLASS;
		}
		
		List<Vlayout> steps = WebUtil.getChildrenByClass(wizardItemsLayout, Vlayout.class, false);
		boolean foundStep = false;
		
		for( Vlayout stepLayout : steps ) { 
			
			Div progressDiv = WebUtil.getFirstChildByClass(stepLayout, Div.class);
			String divSclass = progressDiv.getSclass() != null ? progressDiv.getSclass() : "";
			
			if( !foundStep ) {
				
				if( divSclass.indexOf(PROGRESS_COMPLETED_SCLASS) == -1 ) { 
					divSclass = divSclass + " " + PROGRESS_COMPLETED_SCLASS;
				}
				
				if( divSclass.indexOf(PROGRESS_INCOMPLETE_SCLASS) != -1 ) { 
					divSclass = divSclass.replaceAll(PROGRESS_INCOMPLETE_SCLASS, "");
				}
				
			}
			else {
				
				if( divSclass.indexOf(PROGRESS_COMPLETED_SCLASS) != -1 ) { 
					divSclass = divSclass.replaceAll(PROGRESS_COMPLETED_SCLASS, "");
				}
				
				if( divSclass.indexOf(PROGRESS_INCOMPLETE_SCLASS) == -1 ) { 
					divSclass = divSclass + " " + PROGRESS_INCOMPLETE_SCLASS;
				}
				
			}
			
			progressDiv.setSclass(divSclass.trim());
			if( stepLayout.equals(step.getItemContent()) ) foundStep = true;
		}
		
		
		step.getItemContent().setSclass(itemSclass.trim());
		step.getMainPanelContent().setVisible(true);
		step.getTipsContent().setVisible(true);
		
		if( step.equals(getLastStep()) ) {
			saveButton.setVisible(true);
			nextButton.setVisible(false);
		}
		else { 
			saveButton.setVisible(false);
			nextButton.setVisible(true);
		}
		
		prevButton.setDisabled(step.equals(getFirstStep()));
	}
	
	protected Div getActiveMainContentDiv() { 
		List<Div> divs = WebUtil.getChildrenByClass(mainPanelChildren, Div.class, false);
		for( Div div : divs ) { if( div.isVisible() ) return div; }
		return null;
	}

	protected abstract StepComponents getNextStep();
	
	protected abstract StepComponents getPrevStep();
	
	protected abstract StepComponents getFirstStep();
	
	protected abstract StepComponents getLastStep();

	public abstract void onDoSave(Event event);
	
	protected abstract boolean showWarningOnClose();
}
