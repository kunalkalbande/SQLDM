package com.idera.sqldm.ui.widgets;

import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Div;
import org.zkoss.zul.Vlayout;
import org.zkoss.zul.Window;

public abstract class Generic4StepWizardComposer extends AbstractWizardComposer {

	static final long serialVersionUID = 1L;
	
	@Wire protected Vlayout step1Item;
	@Wire protected Vlayout step2Item;
	@Wire protected Vlayout step3Item;
	@Wire protected Vlayout step4Item;
	
	@Wire protected Div step1Tips;
	@Wire protected Div step2Tips;
	@Wire protected Div step3Tips;
	@Wire protected Div step4Tips;
	
	@Wire protected Div step1Content;
	@Wire protected Div step2Content;
	@Wire protected Div step3Content;
	@Wire protected Div step4Content;
	
	@Wire protected Button saveButton;
	
	@Override
	public void doAfterCompose(Window comp) throws Exception {
		super.doAfterCompose(comp);
		activate(new StepComponents(step1Tips, step1Content, step1Item));
	}
	
	@Listen("onClick = a#step1Link")
	public void activateStep1() { 
		activate(new StepComponents(step1Tips, step1Content, step1Item));
	}

	@Listen("onClick = a#step2Link")
	public void activateStep2() { 
		activate(new StepComponents(step2Tips, step2Content, step2Item));
	}
	
	@Listen("onClick = a#step3Link")
	public void activateStep3() { 
		activate(new StepComponents(step3Tips, step3Content, step3Item));
	}
	
	@Listen("onClick = a#step4Link")
	public void activateStep4() { 
		updateSummaryValues();
		activate(new StepComponents(step4Tips, step4Content, step4Item));
	}
	
	@Override
	protected StepComponents getNextStep() {
		
		Div activeDiv = getActiveMainContentDiv();
		
		if( step1Content.equals(activeDiv) ) { 
			validatesTEP1();
			return new StepComponents(step2Tips, step2Content, step2Item);
		}
		else if( step2Content.equals(activeDiv) ) {
			validatesTEP2();
			return new StepComponents(step3Tips, step3Content, step3Item);
		}
		else if( step3Content.equals(activeDiv) ) {
			validatesTEP3();
			return new StepComponents(step4Tips, step4Content, step4Item);
		}
		
		return null;
	}
	
	@Override
	protected StepComponents getPrevStep() {

		Div activeDiv = getActiveMainContentDiv();
		
		if( step2Content.equals(activeDiv) ) { 
			return new StepComponents(step1Tips, step1Content, step1Item);
		}
		else if( step3Content.equals(activeDiv) ) { 
			return new StepComponents(step2Tips, step2Content, step2Item);
		}
		else if( step4Content.equals(activeDiv) ) { 
			return new StepComponents(step3Tips, step3Content, step3Item);
		}
		
		return null;
	}

	@Override
	protected StepComponents getFirstStep() {
		return new StepComponents(step1Tips, step1Content, step1Item);
	}

	@Override
	protected StepComponents getLastStep() {
		return new StepComponents(step4Tips, step4Content, step4Item);
	}
	
	public abstract void validate();
	
	@Override
	@Listen("onDoSave = window")
	public abstract void onDoSave(Event event);
	
	protected abstract void updateSummaryValues();
	public abstract void validatesTEP1();
	public abstract void validatesTEP2();
	public abstract void validatesTEP3();
	
	
}
