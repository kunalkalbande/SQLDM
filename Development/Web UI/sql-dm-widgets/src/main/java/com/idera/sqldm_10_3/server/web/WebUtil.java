package com.idera.sqldm_10_3.server.web;

import com.idera.common.rest.RestException;
import com.idera.cwf.model.Product;
import com.idera.i18n.I18NStrings;
import com.idera.server.web.WebConstants;
import com.idera.sqldm_10_3.server.web.session.SessionUtil;
import com.idera.sqldm_10_3.ui.components.CustomMessageBox;
import com.idera.sqldm_10_3.ui.components.CustomMessageBox.UserResponse;
import com.idera.sqldm_10_3.server.web.session.SessionUtil;
import com.idera.sqldm_10_3.ui.components.CustomMessageBox;
import org.zkoss.zk.ui.*;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.event.ForwardEvent;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.*;
import util.LocalizedException;

import java.util.*;

//import com.idera.core.instance.Instance;
//import com.idera.core.instance.SelectedInstance;

public class WebUtil {

	protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory
			.getLogger(WebUtil.class);

	public static <T extends Component> List<T> getChildrenByClass(
			Component comp, Class<T> clazz) {
		return getChildrenByClass(comp, clazz, true);
	}

	@SuppressWarnings("unchecked")
	public static <T extends Component> List<T> getChildrenByClass(
			Component comp, Class<T> clazz, boolean recursive) {

		List<T> comps = new ArrayList<T>();

		for (Component c : comp.getChildren()) {

			if (clazz.isAssignableFrom(c.getClass())) {
				comps.add((T) c);
			}

			if (recursive && c.getChildren().size() > 0) {
				comps.addAll(getChildrenByClass(c, clazz));
			}

		}

		return comps;
	}

	public static List<Integer> getCheckedCheckboxesValues(Component parentComp) {
		List<Integer> checkedCheckboxesValues = new ArrayList<Integer>();
		List<Checkbox> checkboxes = getChildrenByClass(parentComp, Checkbox.class);
		for(Checkbox checkbox : checkboxes) {
			if(checkbox.isChecked())
				checkedCheckboxesValues.add(Integer.parseInt(checkbox.getValue().toString()));
		}
		return checkedCheckboxesValues;
	}

	public static void setCheckboxes(Component parentComp, Collection<?> checkboxValues) {

		List<Checkbox> boxes = WebUtil.getChildrenByClass(parentComp, Checkbox.class);

		int checked = 0;

		for( Checkbox box : boxes ) {
			if( checkboxValues.contains(Integer.parseInt(box.getValue().toString())) ) {
				box.setChecked(true);
				if( ++checked == checkboxValues.size() ) break;
			}
		}

	}

	public static <T extends Component> T getFirstChildByClass(Component comp,
			Class<T> clazz) {
		return getFirstChildByClass(comp, clazz, -1);
	}

	public static <T extends Component> T getFirstChildByClass(Component comp,
			Class<T> clazz, int maxDepth) {
		return getFirstChildByClass(comp, clazz, maxDepth, 0);
	}

	public static Object getRowBindingData(Event event) {

		Component target;

		if (event instanceof ForwardEvent) {
			target = ((ForwardEvent) event).getOrigin().getTarget();
		} else {
			target = event.getTarget();
		}

		if (Listbox.class.isAssignableFrom(target.getClass())) {
			Integer index = ((Listbox) target).getSelectedIndex();
			target = ((Listbox) target).getItemAtIndex(index);
			if (target == null)
				return null;
		}

		return getRowBindingByComponent(target);

	}

	@SuppressWarnings("unchecked")
	public static Object getRowBindingByComponent(Component target) {
		if (target == null)
			return null;
		try {

			while (!(target instanceof Row || target instanceof Listitem)) {
				target = target.getParent();
			}

			Map<Object, Object> map = (Map<Object, Object>) target
					.getAttribute("zkplus.databind.TEMPLATEMAP");

			return map.get(target.getAttribute("zkplus.databind.VARNAME"));
		} catch (NullPointerException e) {
			return null;
		}
	}

	@SuppressWarnings("unchecked")
	public static <T extends Component> T getParentByClass(Component comp, Class<T> clazz) {

		Component parent = comp.getParent();

		while( parent != null ) {
			if( clazz.isAssignableFrom(parent.getClass()) ) break;
			parent = parent.getParent();
		}

		return (T) parent;
	}

	@SuppressWarnings("unchecked")
	public static <T extends Component> T getFirstChildByClass(Component comp,
			Class<T> clazz, int maxDepth, int currDepth) {

		currDepth++;

		List<Component> children = new ArrayList<Component>();
		T t = null;

		for (Component c : comp.getChildren()) {
			if (clazz.isAssignableFrom(c.getClass()))
				return (T) c;
			if (c.getChildren().size() > 0)
				children.addAll(c.getChildren());
		}

		if (!children.isEmpty()) {
			for (Component c : children) {

				if (clazz.isAssignableFrom(c.getClass()))
					return (T) c;

				if (maxDepth == -1 || currDepth < maxDepth) {
					t = getFirstChildByClass(c, clazz, maxDepth, currDepth);
					if (t != null)
						return t;
				}

			}
		}

		return t;
	}

	public static void showErrorBox(String msgKey, Object... varargs) {
		showErrorBox(null, msgKey, varargs);
	}

	public static void showErrorBox(boolean suspendExecution, String msgKey,
			Object... varargs) {
		showErrorBox(suspendExecution, null, msgKey, varargs);
	}

	public static void showErrorBox(Throwable ex, String msgKey,
			Object... varargs) {
		showErrorBox(false, ex, msgKey, varargs);
	}

	public static void showErrorBox(boolean suspendExecution, Throwable ex,
			String msgKey, Object... varargs) {

		Clients.clearBusy();

		String msg;

		if (varargs.length == 0) {
			msg = ELFunctions.getMessage(msgKey);
		} else {
			msg = ELFunctions.getMessageWithParams(msgKey, varargs);
		}

		List<String> exStrings = getLocalizedExceptionStrings(ex);		
		String title= ELFunctions.getLabel(I18NStrings.ERROR);
		showBox(suspendExecution, msg, exStrings, title, Messagebox.ERROR, Arrays.asList(CustomMessageBox.UserResponse.OK), false, null, false);
		
	}
	
	public static void showMessageListErrorBox(List<String>exStrings, String msgKey, Object... varargs) {

		Clients.clearBusy();

		String msg;

		if (varargs.length == 0) {
			msg = ELFunctions.getMessage(msgKey);
		} else {
			msg = ELFunctions.getMessageWithParams(msgKey, varargs);
		}		
		String title= I18NStrings.ERROR;
		showBox(false, msg, exStrings, title, Messagebox.ERROR, Arrays.asList(CustomMessageBox.UserResponse.OK), true, null, false);
		
	}
	
	
	public static void showSuccessBoxThatStopsExecution(String msgKey, Object... varargs)
	{
		if (logger != null)
			logger.info(msgKey, varargs);

		try {
			showMessageBox(true, Arrays.asList(ELFunctions.getMessageWithParams(msgKey, varargs)), 
					ELFunctions.getLabel(I18NStrings.SUCCESS), Messagebox.NONE, Arrays.asList(CustomMessageBox.UserResponse.OK), false);
						
		} catch (UiException ex) {
			ex.printStackTrace();
		}
	}

	public static void showSuccessBox(String msgKey, Object... varargs) {
		showSuccessBox((mazz.i18n.Logger) null, msgKey, varargs);
	}

	public static void showSuccessBox(mazz.i18n.Logger logger, String msgKey,
			Object... varargs) {
		showMessageBoxWithParams(logger, msgKey, I18NStrings.SUCCESS, varargs);
	}

	public static void showBox(String msgKey, String label, Object... varargs) {
		showMessageBoxWithParams((mazz.i18n.Logger) null, msgKey, label, varargs);
	}
	
	public static void showMessageBoxWithParams(mazz.i18n.Logger logger, String msgKey,
			String label, Object... varargs) {
		if (logger != null)
			logger.info(msgKey, varargs);

		try {
			showBoxPreFormated(ELFunctions.getMessageWithParams(msgKey, varargs),
							ELFunctions.getLabel(label), Messagebox.NONE);
		} catch (UiException ex) {
			ex.printStackTrace();
		}
	}

	public static boolean showConfirmationBox(String msgKey, String title,
			Object... varargs) {
		return showConfirmationBoxWithIcon(msgKey, title, Messagebox.NONE, true, varargs);
	}

	public static boolean showConfirmationBoxWithWarning(String msgKey, String title,
			Object... varargs) {
		return CustomMessageBox.UserResponse.YES.equals(showMessageBox(true, Arrays.asList(msgKey), title, Messagebox.EXCLAMATION, Arrays.asList(CustomMessageBox.UserResponse.YES, CustomMessageBox.UserResponse.NO), true, varargs)) ? true : false;
	}

	public static CustomMessageBox.UserResponse showBoxPreFormated(String msgKey, String title, String icon, Object... varargs) {
		return showMessageBox(false, Arrays.asList(msgKey), title, icon, Arrays.asList(CustomMessageBox.UserResponse.OK), false, varargs);
	}
	
	public static boolean showConfirmationBoxWithIcon(String msgKey, String title, String icon, boolean formatMessage, Object... varargs) {
		return CustomMessageBox.UserResponse.YES.equals(showMessageBox(true, Arrays.asList(msgKey), title, icon, Arrays.asList(CustomMessageBox.UserResponse.YES, CustomMessageBox.UserResponse.NO), formatMessage, varargs)) ? true : false;
	}
	
	public static CustomMessageBox.UserResponse showMessageBox(boolean suspendExecution, List<String> msgKey, String title, String iconURL, List<CustomMessageBox.UserResponse> userResponseList, boolean translateMessages, Object... varargs) {
		return showBox(suspendExecution, null, msgKey, title, iconURL, userResponseList, translateMessages, null, false, varargs);
	}

	/**
	 * Method to show a dialog box
	 * @param suspendExecution true for model dialog box
	 * @param primaryFailureMessage failure message if any
	 * @param msgKey user messages or keys list to show as description
	 * @param title dialog box title message or key
	 * @param iconURL dialog box type icon i.e. Question, warning
	 * @param userResponseList list of user response group buttons i.e. YES/NO, OK/CANCEL
	 * @param translateMessages true if the title and msgKey list contents keys
	 * @param prefKey User preferences key to store the user selection for don't show it again
	 * @param savePreferences specify it true if you would like to show user preference checkbox
	 * @param varargs other args
	 * @return UserResponse value
	 */
	public static CustomMessageBox.UserResponse showBox(boolean suspendExecution, String primaryFailureMessage, List<String> msgKey,
                                                        String title, String iconURL, List<CustomMessageBox.UserResponse> userResponseList, boolean translateMessages,
                                                        String prefKey, boolean savePreferences, Object... varargs) {
		Clients.clearBusy();

		final HashMap<String, Object> map = new HashMap<String, Object>();
		
		if (translateMessages){
			List<String> messageList = new ArrayList<String>();
	        map.put(CustomMessageBox.STRING_TITLE, ELFunctions.getLabel(title));
			for (String tmp: msgKey){
				messageList.add(ELFunctions.getLabel(tmp));
			}
	        map.put(CustomMessageBox.MESSAGE_LIST, messageList);
		}
		else {
			map.put(CustomMessageBox.STRING_TITLE, title);
	        map.put(CustomMessageBox.MESSAGE_LIST, msgKey);
		}
		
		map.put(CustomMessageBox.BUTTON_LIST, userResponseList);
		map.put(CustomMessageBox.ICON_URL, iconURL);
		map.put(CustomMessageBox.PRIMARY_ERROR_MESSAGE, primaryFailureMessage);

		Window window = null;

		if (savePreferences && prefKey != null) {
			//map.put(SaveUserPreferenceDialog.PREF_KEY, prefKey);
			//window = (Window) Executions.createComponents(SaveUserPreferenceDialog.URL, null, map);
		}
		else {
			window = (Window) Executions.createComponents(CustomMessageBox.URL, null, map);
		}
		
		if (!suspendExecution) {
			window.doHighlighted();
			return CustomMessageBox.UserResponse.OK;
		}
		
		try {
			window.doModal();
		} catch (SuspendNotAllowedException e) {
			logger.error(e,I18NStrings.COULD_NOT_SUSPEND_EXECUTION_TO_DISPLAY_ERROR_BOX);
		} catch (UiException e) {
			logger.error(e,	I18NStrings.RECEIVED_INTERRUPT_WHILE_SUSPENDED_DISPLAYING_AN_ERROR);
		}
				
		return window.getAttribute(CustomMessageBox.USER_RESPONSE) == null || !(window.getAttribute(CustomMessageBox.USER_RESPONSE) instanceof CustomMessageBox.UserResponse)
				? CustomMessageBox.UserResponse.OK
						: (CustomMessageBox.UserResponse) window.getAttribute(CustomMessageBox.USER_RESPONSE);
	}

	public static List<String> getLocalizedExceptionStrings(Throwable cause) {

		List<String> strings = new ArrayList<String>();

		if (cause != null) {

			Locale locale = SessionUtil.getSelectedLocale();

			Throwable e = cause;
			
			while (e instanceof LocalizedException) {
				if (e instanceof RestException){
					RestException re = (RestException) e;
					if (re.getRestApiMessage() != null)
						strings.add(re.getRestApiMessage());
				}
				else {
					LocalizedException le = (LocalizedException) e;
					String msgKey = le.getMsgKey();
					if(msgKey != null && !msgKey.isEmpty()){
						strings.add(ELFunctions.getMessageWithParams(locale,
							le.getMsgKey(), (Object[]) le.getVarArgs()));
					}
				}
				
				e = e.getCause();
			}

			while (e != null) {
				strings.add(e.getLocalizedMessage());
				e = e.getCause();
			}

		}

		return strings;
	}


/*	public static final Comparator<Instance> CORE_MONITORED_INSTANCE_COMPARATOR = new Comparator<Instance>() {
		@Override
		public int compare(Instance o1, Instance o2) {
			return new CompareToBuilder().append(
					StringUtils.lowerCase(o1.getName()),
					StringUtils.lowerCase(o2.getName()))
					.toComparison();
		}
	};
*/
/*	public static final Comparator<SelectedInstance> CORE_SELECTED_INSTANCE_COMPARATOR = new Comparator<SelectedInstance>() {
		@Override
		public int compare(SelectedInstance o1, SelectedInstance o2) {
			return new CompareToBuilder().append(
					StringUtils.lowerCase(o1.getName()),
					StringUtils.lowerCase(o2.getName()))
					.toComparison();
		}
	};
*/	
	public static Object getStaticField(String name) {
		try {
			int i = name.lastIndexOf(".");
			String field = name.substring(i + 1, name.length());
			name = name.substring(0, i);
			Class<?> clz = Class.forName(name);
			Object obj = clz.getField(field).get(null);
			return obj;
		}
		catch( RuntimeException x ) {
			throw x;
		}
		catch( Exception x ) {
			throw new RuntimeException(x);
		}
	}
	
	public static Component getComponentById(Desktop desktop, String id) {

		Component component = null;

		for( Object obj : desktop.getComponents() ) {
			component = (Component) obj;
			if( component.getId().equals(id) ) break;
			component = null;
		}

		return component;
	}
	
	/**
	 * Queues up an application wide event to refresh instance properties. 
	 * This event signals that instance information has been changed and 
	 * UI components should update to reflect these changes.
	 */
	public static void triggerInstanceRefreshEvent()
	{
		EventQueue<Event> q = EventQueues.lookup(
				WebConstants.INSTANCE_PROPERTIES_REFRESH_EVENT_QUEUE,
				EventQueues.APPLICATION, false);
		if (q != null) {
			q.publish(new Event(WebConstants.INSTANCE_PROPERTIES_REFRESH_EVENT,
					null, null));
		}
	}
	
	/**
	 * Queues up an application wide event to refresh tags. 
	 * This event signals that tag information has been changed and 
	 * UI components should update to reflect these changes.
	 */
	public static void triggerTagRefreshEvent()
	{
		EventQueue<Event> q = EventQueues.lookup(
				WebConstants.TAG_REFRESH_EVENT_QUEUE,
				EventQueues.APPLICATION, false);
		if (q != null) {
			q.publish(new Event(WebConstants.TAG_REFRESH_EVENT,
					null, null));
		}
	}
	
	public static Product getCurrentProduct(){
    	Product currentProduct = (Product)Executions.getCurrent().getDesktop().getAttribute("currentProduct");
    	return currentProduct;
    }
	
	public static String buildPathRelativeToCurrentProduct(String path){
		Product product = getCurrentProduct();
		return buildPathRelativeToProduct(product, path);
	}

	public static String buildPathRelativeToProduct(Product product, String path){
		if(product == null){
			logger.error(com.idera.sqldm_10_3.i18n.SQLdmI18NStrings.CURRENT_PRODUCT_IS_NULL);
			return path;
		}
		String rootContext = product.getRootContext();
		String instanceName = product.getInstanceName();
		if(path.startsWith("/")){
			return String.format("%s/%s%s", rootContext, instanceName, path);	
		}
		return String.format("%s/%s/%s", rootContext, instanceName, path);
	}

}
