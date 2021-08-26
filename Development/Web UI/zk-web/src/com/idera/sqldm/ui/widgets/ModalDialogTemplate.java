package com.idera.sqldm.ui.widgets;

import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.A;
import org.zkoss.zul.Window;


public class ModalDialogTemplate extends SelectorComposer<Window> {

	static final long serialVersionUID = 1L;

	@Wire
	protected A closeLink;
	@Wire
	protected A dialogHelpLink;
	
	public void doAfterCompose(Window component) throws Exception {
		super.doAfterCompose(component);
		
		if (dialogHelpLink != null) {
			dialogHelpLink.setTarget("_blank");
		}
	}
	
	@Listen("onClick = a#closeLink")
	public void closeWindow() {
		getSelf().detach();
	}
}
