package com.idera.sqldm.ui.dashboard.instances.queries;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collection;
import java.util.Date;
import java.util.List;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Constraint;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.ListModel;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Popup;

import com.idera.common.Utility;
import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqldm.data.queries.QueryApplication;
import com.idera.sqldm.data.queries.QueryApplicationFacade;
import com.idera.sqldm.data.queries.QueryClients;
import com.idera.sqldm.data.queries.QueryClientsFacade;
import com.idera.sqldm.data.queries.QueryDatabases;
import com.idera.sqldm.data.queries.QueryDatabasesFacade;
import com.idera.sqldm.data.queries.QueryGroups;
import com.idera.sqldm.data.queries.QueryGroupsFacade;
import com.idera.sqldm.data.queries.QueryUsers;
import com.idera.sqldm.data.queries.QueryUsersFacade;
import com.idera.sqldm.data.queries.ViewMetrics;
import com.idera.sqldm.data.queries.ViewMetricsFacade;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.session.SessionUtil;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstanceQueriesPreferencesBean;

public class LeftNavigationQueryComposer {

	private final Logger log = Logger
			.getLogger(LeftNavigationQueryComposer.class);

	private int instanceId;
	private String productInstanceName;
	private boolean isCustomSelected = false;

	private QueryFilter queryFilters;

	List<ViewMetrics> viewMetricOptions;
	List<QueryGroups> queryGroupOptions;

	private ListModel<QueryApplication> applicationModel;
	private ListModel<QueryDatabases> databasesModel;
	private ListModel<QueryUsers> usersModel;
	private ListModel<QueryClients> clientsModel;

	private String displayMessageForApplication;
	private String displayMessageForDatabases;
	private String displayMessageForUsers;
	private String displayMessageForClients;

	private List<QueryApplication> checkedApplications;
	private List<QueryDatabases> checkedDatabases;
	private List<QueryClients> checkedClients;
	private List<QueryUsers> checkedUsers;

	private static String OPEN_TRUE = "open-true";
	private static String OPEN_FALSE = "open-false";

	@Init
	public void init() {

		// Get instance id from url
		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions
				.getCurrent().getParameterMap(), "id");
		productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		} else {
			// fallback
			Object param = Executions.getCurrent().getDesktop()
					.getAttribute("instanceId");
			if (param != null) {
				instanceId = (Integer) param;
			}
		}

		SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueriesPreferenceInSession(instanceId);

		//Not null only in case of redirection from Desktop Console
		Integer querySignID = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "querysignid");
		
		if (sessionBean.getQueryFilter() == null || querySignID  != null) {

			checkedApplications = new ArrayList<>();
			checkedDatabases = new ArrayList<>();
			checkedClients = new ArrayList<>();
			checkedUsers = new ArrayList<>();

			try {

				viewMetricOptions = ViewMetricsFacade.getQueryViewMetrics(productInstanceName);
				queryGroupOptions = QueryGroupsFacade.getQueryGroups(productInstanceName);

			} catch (RestException e) {
				e.printStackTrace();
			}

			queryFilters = new QueryFilter();
			queryFilters.setSelectedView(viewMetricOptions.get(0));
			queryFilters.setSelectedGroupBy(queryGroupOptions.get(0));
			queryFilters
					.setSelectedTimePeriod(QueryTimePeriodsEnum.THIRTY_MINUTES
							.getTimePeriod());

			generateLeftNavigationModel(0);

			sessionBean.init();
			sessionBean.setQueryFilter(queryFilters);
			sessionBean.setCheckedApplications(checkedApplications);
			sessionBean.setCheckedDatabases(checkedDatabases);
			sessionBean.setCheckedClients(checkedClients);
			sessionBean.setCheckedUsers(checkedUsers);

			sessionBean.setViewMetricOptions(viewMetricOptions);
			sessionBean.setQueryGroupOptions(queryGroupOptions);

			PreferencesUtil.getInstance().setQueriesPreferenceInSession(
					sessionBean);

			Clients.evalJavaScript("setHoursOffset()");
		
		} else {

			// Restoring values using session object

			viewMetricOptions = sessionBean.getViewMetricOptions();
			queryGroupOptions = sessionBean.getQueryGroupOptions();

			queryFilters = sessionBean.getQueryFilter();
			generateLeftNavigationModel(1);

			checkedApplications = sessionBean.getCheckedApplications();
			checkedDatabases = sessionBean.getCheckedDatabases();
			checkedClients = sessionBean.getCheckedClients();
			checkedUsers = sessionBean.getCheckedUsers();

			checkUncheckUsingSession("Applications");
			checkUncheckUsingSession("Databases");
			checkUncheckUsingSession("Clients");
			checkUncheckUsingSession("Users");

			queryFilters.setApplicationAllChecked(checkedApplications.size() == applicationModel
					.getSize());
			queryFilters.setDatabasesAllChecked(checkedDatabases.size() == databasesModel
					.getSize());
			queryFilters.setClientsAllChecked(checkedClients.size() == clientsModel
					.getSize());
			queryFilters.setUsersAllChecked(checkedUsers.size() == usersModel.getSize());

		}
		
		setDisplayMessageForApplication(ELFunctions
				.getMessage(SQLdmI18NStrings.NO_APPLICATIONS_AVAILABLE));
		setDisplayMessageForDatabases(ELFunctions
				.getMessage(SQLdmI18NStrings.NO_DATABASES_AVAILABLE));
		setDisplayMessageForUsers(ELFunctions
				.getMessage(SQLdmI18NStrings.NO_USERS_AVAILABLE));
		setDisplayMessageForClients(ELFunctions
				.getMessage(SQLdmI18NStrings.NO_CLIENTS_AVAILABLE));
		
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
		Selectors.wireEventListeners(view, this);

		EventQueue<Event> eq = EventQueues.lookup("changeQueryFilter",
				EventQueues.DESKTOP, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if (event.getName().equals("filterRemoved")) {

					SessionUtil.getSecurityContext();
					updateUsingSession();
				}
			}
		});
	}


	/**
	 * Callback function from setOffset javascript function
	 * @param event
	 */
	@Listen("onSetOffsetLeftBar = #leftBarDiv")
	public void setOffsetLeftBar(Event event) {

		String offsetValue = event.getData().toString();
		queryFilters.setOffsetHours(Double.parseDouble(offsetValue));
		updateSession();
		setDateTime();
		
	}

	/**
	 * Update values using session object
	 */
	@NotifyChange
	private void updateUsingSession() {

		SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueriesPreferenceInSession(instanceId);
		
		if(sessionBean.getQueryFilter() == null) {
			Messagebox.show(ELFunctions.getMessage(SQLdmI18NStrings.ERROR_MESSAGE_QUERIES));
			return;
		}
		
		queryFilters = sessionBean.getQueryFilter();

		checkedApplications = sessionBean.getCheckedApplications();
		checkedDatabases = sessionBean.getCheckedDatabases();
		checkedClients = sessionBean.getCheckedClients();
		checkedUsers = sessionBean.getCheckedUsers();

/*		if (checkedApplications.size() == applicationModel.getSize())
			queryFilters.setApplicationAllChecked(true);
		else
			queryFilters.setApplicationAllChecked(false);

		if (checkedDatabases.size() == databasesModel.getSize())
			queryFilters.setDatabasesAllChecked(true);
		else
			queryFilters.setDatabasesAllChecked(false);

		if (checkedUsers.size() == usersModel.getSize())
			queryFilters.setUsersAllChecked(true);
		else
			queryFilters.setUsersAllChecked(false);

		if (checkedClients.size() == clientsModel.getSize())
			queryFilters.setClientsAllChecked(true);
		else
			queryFilters.setClientsAllChecked(false);
*/
		checkUncheckUsingSession("Applications");
		checkUncheckUsingSession("Databases");
		checkUncheckUsingSession("Clients");
		checkUncheckUsingSession("Users");

		BindUtils.postNotifyChange(null, null, this, "applicationModel");
		BindUtils.postNotifyChange(null, null, this, "databasesModel");
		BindUtils.postNotifyChange(null, null, this, "clientsModel");
		BindUtils.postNotifyChange(null, null, this, "usersModel");
		BindUtils.postNotifyChange(null, null, this, "queryFilters");

	}

	/**
	 * Publishes the event for change filters
	 * 
	 * @param filter
	 */
	private void publishChangeFilterEvent() {

		EventQueue<Event> eq = EventQueues.lookup("changeQueryFilter",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("filterChanged"));

		}

	}

	/**
	 * Searches for application in checked applications list using application
	 * id
	 * 
	 * @param applicationId
	 * @return
	 */
	private QueryApplication findApplication(int applicationId) {
		int numOfApplications = checkedApplications.size();
		for (int i = 0; i < numOfApplications; i++)
			if (checkedApplications.get(i).getApplicationId() == applicationId)
				return checkedApplications.get(i);

		return null;
	}

	/**
	 * Searches for database in checked databases list using database id
	 * 
	 * @param databaseId
	 * @return
	 */
	private QueryDatabases findDatabase(int databaseId) {
		int numOfDatabases = checkedDatabases.size();
		for (int i = 0; i < numOfDatabases; i++)
			if (checkedDatabases.get(i).getDatabaseId() == databaseId)
				return checkedDatabases.get(i);

		return null;
	}

	/**
	 * Searches for client in checked clients list using client id
	 * 
	 * @param clientId
	 * @return
	 */
	private QueryClients findClient(int clientId) {
		int numOfClients = checkedClients.size();
		for (int i = 0; i < numOfClients; i++)
			if (checkedClients.get(i).getClientId() == clientId)
				return checkedClients.get(i);

		return null;
	}

	/**
	 * Searches for user in checked users list using user id
	 * 
	 * @param userId
	 * @return
	 */
	private QueryUsers findUser(int userId) {
		int numOfUsers = checkedUsers.size();
		for (int i = 0; i < numOfUsers; i++)
			if (checkedUsers.get(i).getUserId() == userId)
				return checkedUsers.get(i);

		return null;
	}

	/**
	 * Updates checked or unchecked status of filters
	 * 
	 * @param filterType
	 */
	public void checkUncheckUsingSession(String filterType) {

		switch (filterType) {

		case "Applications":
			int numOfApplications = applicationModel.getSize();
			QueryApplication tempApp;
			for (int i = 0; i < numOfApplications; i++) {
				tempApp = applicationModel.getElementAt(i);
				tempApp.setChecked(findApplication(tempApp.getApplicationId()) != null);
			}
			break;

		case "Databases":
			int numOfDatabases = databasesModel.getSize();
			QueryDatabases tempDb;
			for (int i = 0; i < numOfDatabases; i++) {
				tempDb = databasesModel.getElementAt(i);
				tempDb.setChecked(findDatabase(tempDb.getDatabaseId()) != null);
			}
			break;

		case "Clients":
			int numOfClients = clientsModel.getSize();
			QueryClients tempClient;
			for (int i = 0; i < numOfClients; i++) {
				tempClient = clientsModel.getElementAt(i);
				tempClient
						.setChecked(findClient(tempClient.getClientId()) != null);
			}
			break;

		case "Users":
			int numOfUsers = usersModel.getSize();
			QueryUsers tempUser;
			for (int i = 0; i < numOfUsers; i++) {
				tempUser = usersModel.getElementAt(i);
				tempUser.setChecked(findUser(tempUser.getUserId()) != null);
			}
			break;

		}

	}

	/**
	 * Initialises date and time values
	 */
	public void setDateTime() {

		Calendar c = Calendar.getInstance();
		c.setTime(new Date(c.getTimeInMillis() - (long)(queryFilters.getOffsetHours() * 60 * 60 * 1000)));
		
		c.set(Calendar.HOUR_OF_DAY, 0);
		c.set(Calendar.MINUTE, 0);
		c.set(Calendar.SECOND, 0);
		queryFilters.setEndDate(c.getTime());
		c.set(Calendar.HOUR_OF_DAY, 23);
		c.set(Calendar.MINUTE, 59);
		queryFilters.setEndTime(c.getTime());

		c.add(Calendar.DATE, -30);
		c.set(Calendar.HOUR_OF_DAY, 0);
		c.set(Calendar.MINUTE, 0);
		queryFilters.setFromDate(c.getTime());
		queryFilters.setFromTime(c.getTime());
		
		BindUtils.postNotifyChange(null, null, this, "queryFilters");

	}

	/**
	 * Generates left navigation bar
	 * 
	 * @param refreshFlag
	 */
	public void generateLeftNavigationModel(int refreshFlag) {
		generateLeftNavigationApplicationsModel(1, 11, refreshFlag);
		generateLeftNavigationDatabasesModel(1, 11, refreshFlag);
		generateLeftNavigationClientsModel(1, 11, refreshFlag);
		generateLeftNavigationUsersModel(1, 11, refreshFlag);
	}

	protected void generateLeftNavigationApplicationsModel(int startLimit,
			int noOfRecords, int refreshFlag) {

		List<QueryApplication> applicationList = null;
		try {
			applicationList = QueryApplicationFacade.getQueryApplications(productInstanceName,
					instanceId, -1, -1);
			
			int numOfApplications = applicationList.size();
			int applicationModelSize = 0;
			if(applicationModel != null)
				applicationModelSize = applicationModel.getSize();
			
			for (int i = 0; i < numOfApplications; i++) {
				if (applicationModel != null
						&& applicationModelSize > i)
					applicationList.get(i).setChecked(
							applicationModel.getElementAt(i).getChecked());
				else {
					applicationList.get(i).setChecked(true);
					if (refreshFlag == 0)
						checkedApplications.add(applicationList.get(i));
				}

				applicationList.get(i).setVisibility(true);
			}

/*			if (noOfRecords != -1 && numOfApplications == 11) {
				applicationList.get(10).setVisibility(false);
				if (refreshFlag == 0)
					checkedApplications.remove(findApplication(applicationList
							.get(10).getApplicationId()));
			}*/

		} catch (RestException e) {
			e.printStackTrace();
		}
		setApplicationModel(applicationList);
	}

	protected void generateLeftNavigationDatabasesModel(int startLimit,
			int noOfRecords, int refreshFlag) {

		List<QueryDatabases> databasesList = null;
		try {
			databasesList = QueryDatabasesFacade.getQueryDatabases(productInstanceName,instanceId,
					-1, -1);
			
			int numOfDatabases = databasesList.size();
			int databasesModelSize = 0;
			if(databasesModel != null)
			databasesModelSize = databasesModel.getSize();
			
			for (int i = 0; i < numOfDatabases; i++) {
				if (databasesModel != null
						&& databasesModelSize > i)
					databasesList.get(i).setChecked(
							databasesModel.getElementAt(i).getChecked());
				else {
					databasesList.get(i).setChecked(true);
					if (refreshFlag == 0)
						checkedDatabases.add(databasesList.get(i));
				}
				databasesList.get(i).setVisibility(true);
			}

/*			if (noOfRecords != -1 && numOfDatabases == 11) {
				databasesList.get(10).setVisibility(false);
				if (refreshFlag == 0)
					checkedDatabases.remove(findApplication(databasesList.get(
							10).getDatabaseId()));
			}*/

		} catch (RestException e) {
			e.printStackTrace();
		}
		setDatabasesModel(databasesList);
	}

	protected void generateLeftNavigationClientsModel(int startLimit,
			int noOfRecords, int refreshFlag) {

		List<QueryClients> clientsList = null;
		try {
			clientsList = QueryClientsFacade.getQueryClients(productInstanceName,instanceId,
					-1, -1);
			
			int numOfClients = clientsList.size();
			int clientsModelSize = 0;
			if(clientsModel != null)
				clientsModelSize = clientsModel.getSize();
			
			for (int i = 0; i < numOfClients; i++) {
				if (clientsModel != null
						&& clientsModelSize > i)
					clientsList.get(i).setChecked(
							clientsModel.getElementAt(i).getChecked());
				else {
					clientsList.get(i).setChecked(true);
					if (refreshFlag == 0)
						checkedClients.add(clientsList.get(i));
				}
				clientsList.get(i).setVisibility(true);
			}

/*			if (noOfRecords != -1 && numOfClients == 11) {
				clientsList.get(10).setVisibility(false);
				if (refreshFlag == 0)
					checkedClients.remove(findClient(clientsList.get(10)
							.getClientId()));
			}*/
		} catch (RestException e) {
			e.printStackTrace();
		}
		setClientsModel(clientsList);
	}

	protected void generateLeftNavigationUsersModel(int startLimit,
			int noOfRecords, int refreshFlag) {

		List<QueryUsers> usersList = null;
		try {
			usersList = QueryUsersFacade.getQueryUsers(productInstanceName,instanceId, -1,
					-1);
			
			int numOfUsers = usersList.size();
			int usersModelSize = 0;
			if(usersModel != null)
				usersModelSize = usersModel.getSize();
			
			for (int i = 0; i < numOfUsers; i++) {
				if (usersModel != null && usersModelSize > i)
					usersList.get(i).setChecked(
							usersModel.getElementAt(i).getChecked());
				else {
					usersList.get(i).setChecked(true);
					if (refreshFlag == 0)
						checkedUsers.add(usersList.get(i));
				}
				usersList.get(i).setVisibility(true);
			}

/*			if (noOfRecords != -1 && numOfUsers == 11) {
				usersList.get(10).setVisibility(false);
				if (refreshFlag == 0)
					checkedUsers
							.remove(findUser(usersList.get(10).getUserId()));
			}*/

		} catch (RestException e) {
			e.printStackTrace();
		}
		setUsersModel(usersList);
	}

	@Command("onOpenGroupBox")
	public void onOpenStatusGroupBox(
			@ContextParam(ContextType.COMPONENT) Component comp) {
		SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueriesPreferenceInSession(instanceId);
		sessionBean.setSet(1);
		Groupbox tmp = (Groupbox) comp;
		// onOpenTagsGroupBox(tmp);
		tmp.getCaption().setClass(tmp.isOpen() ? OPEN_TRUE : OPEN_FALSE);
		;
	}

	/**
	 * Clear all filters
	 */
	@NotifyChange(".")
	@Command("clearFilters")
	public void clearFilters() {

		// Reset all filters
		queryFilters.setApplicationAllChecked(true);
		checkApplicationAll(true);
		queryFilters.setDatabasesAllChecked(true);
		checkDatabasesAll(true);
		queryFilters.setUsersAllChecked(true);
		checkUsersAll(true);
		queryFilters.setClientsAllChecked(true);
		checkClientsAll(true);

		queryFilters.setIncludeSQLText("");
		queryFilters.setExcludeSQLText("");

		queryFilters.setShowSQLStatements(true);
		queryFilters.setShowStoredProcs(true);
		queryFilters.setShowSQLBatches(true);
		queryFilters.setIncludeOverlapping(false);
		queryFilters.setIncludeIncomplete(false);

		updateSession();
		publishChangeFilterEvent();
	}

	@NotifyChange("isCustomSelected")
	@Command("timeGroupOpened")
	public void isCustomSelected() {
		if (queryFilters.getSelectedTimePeriod() == QueryTimePeriodsEnum.CUSTOM
				.getTimePeriod())
			isCustomSelected = true;
		else
			isCustomSelected = false;

		updateSession();
		publishChangeFilterEvent();
	}

	/**
	 * Check or uncheck all applications
	 * 
	 * @param value
	 */
	private void checkApplicationAll(boolean value) {

		int numOfApplications = applicationModel.getSize();
		if (value == true) {
			checkedApplications.clear();
			for (int i = 0; i < numOfApplications; i++) {
				applicationModel.getElementAt(i).setChecked(value);
				checkedApplications.add(applicationModel.getElementAt(i));
			}
		} else {
			for (int i = 0; i < numOfApplications; i++) {
				applicationModel.getElementAt(i).setChecked(value);
				checkedApplications.remove(findApplication(applicationModel
						.getElementAt(i).getApplicationId()));
			}
		}
	}

	/**
	 * Check or uncheck all databases
	 * 
	 * @param value
	 */
	private void checkDatabasesAll(boolean value) {

		int numOfDatabases = databasesModel.getSize();
		if (value == true) {
			checkedDatabases.clear();
			for (int i = 0; i < numOfDatabases; i++) {
				databasesModel.getElementAt(i).setChecked(value);
				checkedDatabases.add(databasesModel.getElementAt(i));
			}
		} else {
			for (int i = 0; i < numOfDatabases; i++) {
				databasesModel.getElementAt(i).setChecked(value);
				checkedDatabases.remove(findDatabase(databasesModel
						.getElementAt(i).getDatabaseId()));
			}
		}

	}

	/**
	 * Check or uncheck all users
	 * 
	 * @param value
	 */
	private void checkUsersAll(boolean value) {

		int numOfUsers = usersModel.getSize();
		if (value == true) {
			checkedUsers.clear();
			for (int i = 0; i < numOfUsers; i++) {
				usersModel.getElementAt(i).setChecked(value);
				checkedUsers.add(usersModel.getElementAt(i));
			}
		} else {
			for (int i = 0; i < numOfUsers; i++) {
				usersModel.getElementAt(i).setChecked(value);
				checkedUsers.remove(findUser(usersModel.getElementAt(i)
						.getUserId()));
			}
		}

	}

	/**
	 * Check or uncheck all clients
	 * 
	 * @param value
	 */
	private void checkClientsAll(boolean value) {

		int numOfClients = clientsModel.getSize();
		if (value == true) {
			checkedClients.clear();
			for (int i = 0; i < numOfClients; i++) {
				clientsModel.getElementAt(i).setChecked(value);
				checkedClients.add(clientsModel.getElementAt(i));
			}
		} else {
			for (int i = 0; i < numOfClients; i++) {
				clientsModel.getElementAt(i).setChecked(value);
				checkedClients.remove(findClient(clientsModel.getElementAt(i)
						.getClientId()));
			}
		}

	}

	/**
	 * Handles event when an application is checked
	 * 
	 * @param application
	 */
	@NotifyChange({ "queryFilters", "applicationModel" })
	@Command("checkApplication")
	public void checkApplication(
			@BindingParam("application") QueryApplication application) {
		if (application == null) {
			if (queryFilters.isApplicationAllChecked())
				checkApplicationAll(true);
		} else if (!application.getChecked()) {
			queryFilters.setApplicationAllChecked(false);
			checkedApplications.remove(findApplication(application
					.getApplicationId()));
		} else {
			checkedApplications.add(application);
		}
		updateSession();
		publishChangeFilterEvent();
	}

	/**
	 * Handles event when a database is checked
	 * 
	 * @param database
	 */
	@NotifyChange({ "queryFilters", "databasesModel" })
	@Command("checkDatabase")
	public void checkDatabase(@BindingParam("database") QueryDatabases database) {
		if (database == null) {
			if (queryFilters.isDatabasesAllChecked())
				checkDatabasesAll(true);
		} else if (!database.getChecked()) {
			queryFilters.setDatabasesAllChecked(false);
			checkedDatabases.remove(findDatabase(database.getDatabaseId()));
		} else {
			checkedDatabases.add(database);
		}

		updateSession();
		publishChangeFilterEvent();
	}

	/**
	 * Handles event when a user is checked
	 * 
	 * @param user
	 */
	@NotifyChange({ "queryFilters", "usersModel" })
	@Command("checkUser")
	public void checkUser(@BindingParam("user") QueryUsers user) {
		if (user == null) {
			if (queryFilters.isUsersAllChecked())
				checkUsersAll(true);
		} else if (!user.getChecked()) {
			queryFilters.setUsersAllChecked(false);
			checkedUsers.remove(findUser(user.getUserId()));
		} else {
			checkedUsers.add(user);
		}

		updateSession();
		publishChangeFilterEvent();
	}

	/**
	 * Handles event when a client is checked
	 * 
	 * @param client
	 */
	@NotifyChange({ "queryFilters", "clientsModel" })
	@Command("checkClient")
	public void checkClient(@BindingParam("client") QueryClients client) {
		if (client == null) {
			if (queryFilters.isClientsAllChecked())
				checkClientsAll(true);
		} else if (!client.getChecked()) {
			queryFilters.setClientsAllChecked(false);
			checkedClients.remove(findClient(client.getClientId()));
		} else {
			checkedClients.add(client);
		}

		updateSession();
		publishChangeFilterEvent();
	}

	/**
	 * Handles event when more link for applications is clicked
	 * 
	 * @param gb
	 * @param moreFilterOptionsPopup
	 */
	@NotifyChange({ "applicationModel" })
	@Command("moreApplications")
	public void showMoreApplications(@BindingParam("grpbx") Groupbox gb,
			@BindingParam("target") Popup moreFilterOptionsPopup) {

		generateLeftNavigationApplicationsModel(-1, -1, 0);
		moreFilterOptionsPopup.open(gb, "end_after");
	}

	/**
	 * Handles event when more link for databases is clicked
	 * 
	 * @param gb
	 * @param moreFilterOptionsPopup
	 */
	@NotifyChange("databasesModel")
	@Command("moreDatabases")
	public void showMoreDatabases(@BindingParam("grpbx") Groupbox gb,
			@BindingParam("target") Popup moreFilterOptionsPopup) {

		generateLeftNavigationDatabasesModel(-1, -1, 0);
		moreFilterOptionsPopup.open(gb, "end_after");
	}

	/**
	 * Handles event when more link for clients is clicked
	 * 
	 * @param gb
	 * @param moreFilterOptionsPopup
	 */
	@NotifyChange("clientsModel")
	@Command("moreClients")
	public void showMoreClients(@BindingParam("grpbx") Groupbox gb,
			@BindingParam("target") Popup moreFilterOptionsPopup) {

		generateLeftNavigationClientsModel(-1, -1, 0);
		moreFilterOptionsPopup.open(gb, "end_after");
	}

	/**
	 * Handles event when more link for users is clicked
	 * 
	 * @param gb
	 * @param moreFilterOptionsPopup
	 */
	@NotifyChange("usersModel")
	@Command("moreUsers")
	public void showMoreUsers(@BindingParam("grpbx") Groupbox gb,
			@BindingParam("target") Popup moreFilterOptionsPopup) {

		generateLeftNavigationUsersModel(-1, -1, 0);
		moreFilterOptionsPopup.open(gb, "end_after");
		/*
		 * int numOfUsers = usersModel.getSize();
		 * if(moreLink.getLabel().equals("More")) { moreLink.setLabel("Less");
		 * for(int i=10; i < numOfUsers; i++)
		 * usersModel.getElementAt(i).setVisibility(true); } else {
		 * moreLink.setLabel("More"); for(int i=10; i < numOfUsers; i++)
		 * usersModel.getElementAt(i).setVisibility(false); }
		 */
	}

	@Command("changeGroupBy")
	public void changeGroupBy(@BindingParam("flag") int flag) {
		if (flag == 1) {
			SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
					.getInstance().getQueriesPreferenceInSession(instanceId);
			if (sessionBean.getDefaultSession() != null) {
				sessionBean = sessionBean.getDefaultSession();
				sessionBean.setDrill(0);
				sessionBean.setDefaultSession(null);
				sessionBean.setSet(1);
				sessionBean.setPageCount(-1);
				sessionBean.setQuerySignatureSession(null);
				sessionBean.setQueryFilter(queryFilters);
				PreferencesUtil.getInstance().setQueriesPreferenceInSession(
						sessionBean);
				updateUsingSession();
			}
			sessionBean.setDrill(0);
		}
		publishChangeFilterEvent();

	}

	@Command("changeFilter")
	public void changeFilter() {
		publishChangeFilterEvent();
		BindUtils.postNotifyChange(null, null, this, "queryFilters");
		
	}

	
	public Constraint getFromDateConstraint() {

		Constraint ctt1 = new Constraint() {
			public void validate(Component comp, Object value)
					throws WrongValueException {

				Calendar fromCalendar = Calendar.getInstance();
				fromCalendar.setTime((Date) value);
				if (queryFilters.getFromTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime(queryFilters.getFromTime());
					fromCalendar.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
					fromCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
				}

				Calendar endCalendar = Calendar.getInstance();
				endCalendar.setTime(queryFilters.getEndDate());
				if (queryFilters.getEndTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime(queryFilters.getEndTime());
					endCalendar.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
					endCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
				}

				if (value == null
						|| (queryFilters.getEndDate() != null && (new Date(fromCalendar.getTimeInMillis()))
								.after(new Date(endCalendar.getTimeInMillis())))
						|| (new Date(fromCalendar.getTimeInMillis()))
								.after(new Date(new Date().getTime() - (long)(queryFilters.getOffsetHours() * 60 * 60 * 1000))))
					throw new WrongValueException(comp,
							"Start date should not be greater than current date.");
			}
		};
		
		return ctt1;

	}
	
	public Constraint getFromTimeConstraint() {

		Constraint ctt1 = new Constraint() {
			public void validate(Component comp, Object value)
					throws WrongValueException {

				Calendar fromCalendar = Calendar.getInstance();
				fromCalendar.setTime(queryFilters.getFromDate());
				if (queryFilters.getFromTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime((Date) value);
					fromCalendar.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
					fromCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
				}

				Calendar endCalendar = Calendar.getInstance();
				endCalendar.setTime(queryFilters.getEndDate());
				if (queryFilters.getEndTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime(queryFilters.getEndTime());
					endCalendar.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
					endCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
				}

				if (value == null
						|| (queryFilters.getEndDate() != null && (new Date(fromCalendar.getTimeInMillis()))
								.after(new Date(endCalendar.getTimeInMillis())))
						|| (new Date(fromCalendar.getTimeInMillis()))
								.after(new Date(new Date().getTime() - (long)(queryFilters.getOffsetHours() * 60 * 60 * 1000))))
					throw new WrongValueException(comp,
							"Start date should not be greater than current date.");
			}
		};
		
		return ctt1;

	}
	
	public Constraint getEndDateConstraint() {
		
		Constraint ctt = new Constraint() {
			public void validate(Component comp, Object value)
					throws WrongValueException {
				
				Calendar fromCalendar = Calendar.getInstance();
				fromCalendar.setTime(queryFilters.getFromDate());
				if (queryFilters.getFromTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime(queryFilters.getFromTime());
					fromCalendar.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
					fromCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
				}

				Calendar endCalendar = Calendar.getInstance();
				endCalendar.setTime((Date) value);
				if (queryFilters.getEndTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime(queryFilters.getEndTime());
					endCalendar.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
					endCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
				}

				if (value == null
						|| (queryFilters.getFromDate() != null && (new Date(endCalendar.getTimeInMillis()))
								.before(new Date(fromCalendar.getTimeInMillis())))
						|| (new Date(endCalendar.getTimeInMillis()))
								.after(new Date(new Date().getTime()  - (long)(queryFilters.getOffsetHours() * 60 * 60 * 1000))))
					throw new WrongValueException(comp,
							"End date should be less than current date and greater than start date.");
			}
		};
		
		return ctt;

	}

	public Constraint getEndTimeConstraint() {
		
		Constraint ctt = new Constraint() {
			public void validate(Component comp, Object value)
					throws WrongValueException {
				
				Calendar fromCalendar = Calendar.getInstance();
				fromCalendar.setTime(queryFilters.getFromDate());
				if (queryFilters.getFromTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime(queryFilters.getFromTime());
					fromCalendar.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
					fromCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
				}

				Calendar endCalendar = Calendar.getInstance();
				endCalendar.setTime(queryFilters.getEndDate());
				if (queryFilters.getEndTime() != null) {
					Calendar timeCal = Calendar.getInstance();
					timeCal.setTime((Date) value);
					endCalendar.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
					endCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));
				}

				if (value == null
						|| (queryFilters.getFromDate() != null && (new Date(endCalendar.getTimeInMillis()))
								.before(new Date(fromCalendar.getTimeInMillis())))
						|| (new Date(endCalendar.getTimeInMillis()))
								.after(new Date(new Date().getTime()  - (long)(queryFilters.getOffsetHours() * 60 * 60 * 1000))))
					throw new WrongValueException(comp,
							"End date should be less than current date and greater than start date.");
			}
		};
		
		return ctt;

	}

	/**
	 * Updates session object using current view selections and filter values
	 */
	private void updateSession() {
		SingleInstanceQueriesPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getQueriesPreferenceInSession(instanceId);
		sessionBean.setQueryFilter(queryFilters);
		sessionBean.setCheckedApplications(checkedApplications);
		sessionBean.setCheckedDatabases(checkedDatabases);
		sessionBean.setCheckedClients(checkedClients);
		sessionBean.setCheckedUsers(checkedUsers);
		PreferencesUtil.getInstance()
				.setQueriesPreferenceInSession(sessionBean);
	}

	public QueryFilter getQueryFilters() {
		return queryFilters;
	}

	public void setQueryFilters(QueryFilter queryFilters) {
		this.queryFilters = queryFilters;
	}

	public void setCheckedApplications(
			List<QueryApplication> checkedApplications) {
		this.checkedApplications = checkedApplications;
	}

	public void setCheckedDatabases(List<QueryDatabases> checkedDatabases) {
		this.checkedDatabases = checkedDatabases;
	}

	public void setCheckedClients(List<QueryClients> checkedClients) {
		this.checkedClients = checkedClients;
	}

	public void setCheckedUsers(List<QueryUsers> checkedUsers) {
		this.checkedUsers = checkedUsers;
	}

	public List<QueryApplication> getCheckedApplications() {
		return checkedApplications;
	}

	public List<QueryDatabases> getCheckedDatabases() {
		return checkedDatabases;
	}

	public List<QueryClients> getCheckedClients() {
		return checkedClients;
	}

	public List<QueryUsers> getCheckedUsers() {
		return checkedUsers;
	}

	public ListModel<QueryApplication> getApplicationModel() {
		return applicationModel;
	}

	@NotifyChange
	public void setApplicationModel(
			Collection<QueryApplication> applicationModel) {
		if (applicationModel.size() == 0) {
			setDisplayMessageForApplication(ELFunctions
					.getMessage(SQLdmI18NStrings.NO_APPLICATIONS_AVAILABLE));
		}

		this.applicationModel = new ListModelList<>(applicationModel);

		BindUtils.postNotifyChange(null, null, this, "applicationModel");
		BindUtils.postNotifyChange(null, null, this, "applicationMore");
		BindUtils.postNotifyChange(null, null, this,
				"applicationsAllVisibility");
	}

	public ListModel<QueryDatabases> getDatabasesModel() {
		return databasesModel;
	}

	@NotifyChange
	public void setDatabasesModel(Collection<QueryDatabases> databasesModel) {
		if (databasesModel.size() == 0) {
			setDisplayMessageForDatabases(ELFunctions
					.getMessage(SQLdmI18NStrings.NO_DATABASES_AVAILABLE));
		}

		this.databasesModel = new ListModelList<>(databasesModel);
		BindUtils.postNotifyChange(null, null, this, "databasesModel");
		BindUtils.postNotifyChange(null, null, this, "databasesMore");
	}

	public ListModel<QueryUsers> getUsersModel() {
		return usersModel;
	}

	@NotifyChange
	public void setUsersModel(Collection<QueryUsers> usersModel) {
		if (usersModel.size() == 0) {
			setDisplayMessageForUsers(ELFunctions
					.getMessage(SQLdmI18NStrings.NO_USERS_AVAILABLE));
		}

		this.usersModel = new ListModelList<>(usersModel);
		BindUtils.postNotifyChange(null, null, this, "usersModel");
		BindUtils.postNotifyChange(null, null, this, "usersMore");
	}

	public ListModel<QueryClients> getClientsModel() {
		return clientsModel;
	}

	@NotifyChange
	public void setClientsModel(Collection<QueryClients> clientsModel) {
		if (clientsModel.size() == 0) {
			setDisplayMessageForClients(ELFunctions
					.getMessage(SQLdmI18NStrings.NO_CLIENTS_AVAILABLE));
		}

		this.clientsModel = new ListModelList<>(clientsModel);
		BindUtils.postNotifyChange(null, null, this, "clientsModel");
		BindUtils.postNotifyChange(null, null, this, "clientsMore");
	}

	public String getDisplayMessageForApplication() {
		return displayMessageForApplication;
	}

	public void setDisplayMessageForApplication(
			String displayMessageForApplication) {
		this.displayMessageForApplication = displayMessageForApplication;
		BindUtils.postNotifyChange(null, null, this,
				"displayMessageForApplication");
	}

	public String getDisplayMessageForDatabases() {
		return displayMessageForDatabases;
	}

	public void setDisplayMessageForDatabases(String displayMessageForDatabases) {
		this.displayMessageForDatabases = displayMessageForDatabases;
		BindUtils.postNotifyChange(null, null, this,
				"displayMessageForDatabases");
	}

	public String getDisplayMessageForUsers() {
		return displayMessageForUsers;
	}

	public void setDisplayMessageForUsers(String displayMessageForUsers) {
		this.displayMessageForUsers = displayMessageForUsers;
		BindUtils.postNotifyChange(null, null, this, "displayMessageForUsers");
	}

	public String getDisplayMessageForClients() {
		return displayMessageForClients;
	}

	public void setDisplayMessageForClients(String displayMessageForClients) {
		this.displayMessageForClients = displayMessageForClients;
		BindUtils
				.postNotifyChange(null, null, this, "displayMessageForClients");
	}

	public List<ViewMetrics> getViewMetricOptions() {
		return viewMetricOptions;
	}

	public List<QueryGroups> getQueryGroupOptions() {
		return queryGroupOptions;
	}

	public List<String> getQueryTimePeriodOptions() {
		List<String> queryTimePeriodOptions = new ArrayList<>();
		for (QueryTimePeriodsEnum qco : QueryTimePeriodsEnum.values()) {
			queryTimePeriodOptions.add(qco.getTimePeriod());
		}
		return queryTimePeriodOptions;
	}

	/**
	 * @param isCustomSelected
	 *            the isCustomSelected to set
	 */
	public boolean getIsCustomSelected() {
		return isCustomSelected;
	}

	public void setIsCustomSelected(boolean isCustomSelected) {
		this.isCustomSelected = isCustomSelected;
	}

}