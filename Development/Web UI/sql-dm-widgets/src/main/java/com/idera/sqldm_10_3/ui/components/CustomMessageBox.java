package com.idera.sqldm_10_3.ui.components;

import org.zkoss.zk.ui.Execution;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zkplus.databind.BindingListModelList;
import org.zkoss.zul.*;

import java.util.ArrayList;
import java.util.List;


public class CustomMessageBox extends SelectorComposer<Window>{
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	public static final String URL = "~./sqldm/com/idera/sqldm/ui/components/customMessageBox.zul";
	public static final String STRING_TITLE = "title";
	public static final String MESSAGE_LIST = "messageList";
	public static final String BUTTON_LIST = "buttonList";
	public static final String USER_RESPONSE = "userResponse" ;
	public static final String ICON_URL = "iconURL" ;
	public static final String PRIMARY_ERROR_MESSAGE = "primaryErrorMessage" ;
	
	
	@Wire
	protected Label titleMessage;
	@Wire
	protected Label primaryCause;
	@Wire
	protected Grid messageGrid;
	@Wire
	protected Button okButton;
	@Wire
	protected Button yesButton;
	@Wire
	protected Button noButton;
	@Wire
	protected Button cancelButton;
	
	public static enum UserResponse{
		YES,
		NO,
		CANCEL,
		OK
	}
	
	public ListModelList<String> messageModel = new BindingListModelList<String>(new ArrayList<String>(),false);
	
	protected AnnotateDataBinder binder;
	
	@SuppressWarnings("unchecked")
	@Override
	public void doAfterCompose(Window comp) throws Exception {
		super.doAfterCompose(comp);

		final Execution execution = Executions.getCurrent();
		if (execution.getArg().get(STRING_TITLE) != null && (execution.getArg().get(STRING_TITLE) instanceof String))
			this.titleMessage.setValue((String) execution.getArg().get(STRING_TITLE));
		if (execution.getArg().get(MESSAGE_LIST) != null && (execution.getArg().get(MESSAGE_LIST) instanceof List)){
			for (Object tmp: (List<Object>) execution.getArg().get(MESSAGE_LIST)){
				if (tmp instanceof String)
					messageModel.add((String)tmp);
			}
		}
		if (execution.getArg().get(BUTTON_LIST) != null && (execution.getArg().get(BUTTON_LIST) instanceof List)){
			for (Object tmp: (List<Object>) execution.getArg().get(BUTTON_LIST)){
				if (!(tmp instanceof UserResponse))	continue;
				UserResponse userResponse = (UserResponse)tmp;
				switch (userResponse){
				case YES:
					yesButton.setVisible(true);
					break;
				case NO:
					noButton.setVisible(true);
					break;
				case CANCEL:
					cancelButton.setVisible(true);
					break;
				case OK:
					okButton.setVisible(true);
					break;
				}
			}
		}
		
		
		binder = new AnnotateDataBinder(comp);
		boolean isIconVisible = false;
		if (execution.getArg().get(ICON_URL) != null && execution.getArg().get(ICON_URL) instanceof String){
			binder.bindBean("iconURL", (String) execution.getArg().get(ICON_URL));
			isIconVisible = true;
		}
		binder.bindBean("isIconVisible", isIconVisible);
		if (execution.getArg().get(PRIMARY_ERROR_MESSAGE) != null && execution.getArg().get(PRIMARY_ERROR_MESSAGE) instanceof String){
			this.primaryCause.setValue((String) execution.getArg().get(PRIMARY_ERROR_MESSAGE));
		}
		binder.bindBean("messageListModel", messageModel);
		
		binder.loadAll();

	}

	public void onClick$okButton() { 
		getSelf().detach();
	}
	
	@Listen("onClick = #yesButton")
	public void onClickYesButton(Event evt){
		getSelf().setAttribute(USER_RESPONSE, UserResponse.YES);
		getSelf().detach();
	}
	
	@Listen("onClick = #okButton")
	public void onClickOkButton(Event evt){
		getSelf().setAttribute(USER_RESPONSE, UserResponse.OK);
		getSelf().detach();
	}
	
	@Listen("onClick = #cancelButton")
	public void onClickCancelButton(Event evt){
		getSelf().setAttribute(USER_RESPONSE, UserResponse.CANCEL);
		getSelf().detach();
	}
	
	@Listen("onClick = #noButton")
	public void onClickNoButton(Event evt){
		getSelf().setAttribute(USER_RESPONSE, UserResponse.NO);
		getSelf().detach();
	}

}
