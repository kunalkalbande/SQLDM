package com.idera.sqldm.ui.basepage;

import java.io.File;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import com.idera.common.rest.ICSTokenCredentials;
import org.apache.commons.io.FilenameUtils;
import org.apache.http.auth.Credentials;
import org.apache.log4j.Logger;
import org.springframework.security.context.SecurityContext;
import org.springframework.security.context.SecurityContextHolder;
import org.zkoss.bind.BindUtils;
import org.zkoss.json.JSONObject;
import org.zkoss.json.parser.JSONParser;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Session;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.event.SelectEvent;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.A;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Div;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Selectbox;
import org.zkoss.zul.Vlayout;

import com.idera.common.rest.CoreRestClient;
import com.idera.common.rest.RestException;
import com.idera.cwf.model.Product;
import com.idera.sqldm.data.UserSettingFacade;
import com.idera.sqldm.data.UserSettings;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.rest.SQLDMRestClient;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.server.web.session.UserPreferences;
import com.idera.sqldm.server.web.session.UserSessionSettings;
import com.idera.sqldm.ui.alerts.AlertFilter;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.Utility;

import com.idera.ccl.NavigationBar;
import com.idera.ccl.NavigationModel;

import javax.xml.bind.DatatypeConverter;

public class NavigationBarComposer extends SelectorComposer<Vlayout> {

    private Logger log = Logger.getLogger(NavigationBarComposer.class);

    /**
     * The nav.
     */
    @Wire
    private NavigationBar nav;

    /**
     * The models.
     */
    private List<NavigationModel> models;

    /**
     * The is core page.
     */
    private boolean isCorePage = true;

    private static final long serialVersionUID = 1L;
    private HashMap<String, Product> productMap = new HashMap<String, Product>();
    
    @Wire
    private Div filterDropdown;

    @Wire
    private Combobox comboBox;

    private ListModelList<String> selectListModel;

    private String requestPath = "";

    @Override
    public void doAfterCompose(Vlayout component) throws Exception {

        List<String> selectList = new ArrayList<String>();
        List<Product> productsList = new ArrayList<Product>();
        //BindUtils.postNotifyChange(null, null, this, "listVisible");
        try {
            productsList = CoreRestClient.getInstance().getProducts();
            log.error("NavigationBarComposer: product instance get call passed" + productsList);
            AlertFilter.productMapping.clear();
            AlertFilter.productMapping.put(-1, "All");

            for (Product product : productsList) {
                if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("sqldm")) {
                    System.out.println(product.getInstanceName());
                    log.error("NavigationBarComposer: product instance name" + product.getInstanceName());
                    if (product.getInstanceName() != null)
                        selectList.add(product.getInstanceName());
                    else
                        selectList.add(ELFunctions.getMessage(SQLdmI18NStrings.DEFAULT_INSTANCE_NAME));
                    productMap.put(product.getInstanceName(), product);
                    AlertFilter.productMapping.put(product.getProductId(), product.getInstanceName());
                }
            }
        } catch (RestException e) {
            // TODO Auto-generated catch block
            log.error("NavigationBarComposer: Could not get product list");
            e.printStackTrace();
        }
        selectList.add("All");
        selectListModel = new ListModelList<String>();
        super.doAfterCompose(component);

        models = new ArrayList<>();
        models.add(new NavigationModel(
				ELFunctions.getLabel("SQLdm.Labels.nav-bar-dashboard-label"),
                WebUtil.buildPathRelativeToCurrentProduct("home"),
                isCurrentPage("home"), true));
        models.add(new NavigationModel(
		        ELFunctions.getUpperCaseLabel("SQLdm.Labels.nav-bar-topten-label"),
                WebUtil.buildPathRelativeToCurrentProduct("topten"),
                isCurrentPage("topten"), true));
        models.add(new NavigationModel(
		        ELFunctions.getUpperCaseLabel("SQLdm.Labels.nav-bar-alerts-label"),
                WebUtil.buildPathRelativeToCurrentProduct("alerts"),
                isCurrentPage("alerts"), true));
        models.add(new NavigationModel(
		        ELFunctions.getUpperCaseLabel("SQLdm.Labels.nav-bar-custom-dashboard-label"),
                WebUtil.buildPathRelativeToCurrentProduct("customdashboards"),
                isCurrentPage("customdashboards"), true));
        nav.setVisible(isCorePage);
        nav.setNavigationModels(models);

        String productInstanceName = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
        
        selectListModel.addAll(selectList);

        comboBox.setModel(selectListModel);

        String _productInstanceName = null, _selectedSqldmTab = null;
        DashboardPreferencesBean dbpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();

        if (dbpb != null && dbpb.getProductInstanceName() != null) {
            _productInstanceName = dbpb.getProductInstanceName();
            _selectedSqldmTab = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

            if (_selectedSqldmTab != null && !_selectedSqldmTab.equals(dbpb.getSelectedSqldmTab())) {
                _productInstanceName = _selectedSqldmTab;
            }
        } else {
            //Setting product instance name value from URL in session
            _productInstanceName = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
            _selectedSqldmTab = _productInstanceName;
        }

        if (_productInstanceName != null)
            PreferencesUtil.getInstance().setDashboardPreferencesInSession(_productInstanceName, _selectedSqldmTab);
        requestPath = Executions.getCurrent().getDesktop().getRequestPath();

        log.error("NavigationBarComposer: product list size: " + productsList.size());

        filterDropdown.setVisible(productsList.size() < 2 ? false : true && (!requestPath.contains("singleInstance"))
                && !requestPath.contains("alerts") && !requestPath.contains("customdashboards"));

        selectListModel.addToSelection(_productInstanceName);

        //In case of redirection from Idera Dashboard widget with All Flag set
        Object allFlag = Sessions.getCurrent().getAttribute("AllFlag");
        if (allFlag != null) {
            selectListModel.addToSelection(selectListModel.get(selectListModel.getSize() - 1));
            productInstanceName = selectListModel.get(selectListModel.getSize() - 1);
            PreferencesUtil.getInstance().setDashboardPreferencesInSession(productInstanceName);
            Sessions.getCurrent().setAttribute("AllFlag", null);
        }
        loadUserSettings();

    }

//		setNavElementStyle(homePageLink, homePageNavDiv);
    //	alertsLink.setHref("aler.zul?instance="+productInstanceName);

    /*
     * Author:Accolite
     * Date : 15th Nov, 2016
     * History Panel - SQLDM- 10.2 release
     * Load the user settings from previous session
     */
    private void loadUserSettings() throws CategoryException {
        Session session = Sessions.getCurrent();
        if (session.getAttribute(UserSessionSettings.SQLDM_USER_SESSION_SETTINGS_PROPERTY) == null) {
            UserSessionSettings settings = new UserSessionSettings();
            session.setAttribute(UserSessionSettings.SQLDM_USER_SESSION_SETTINGS_PROPERTY, settings);
        }
        UserSessionSettings settings = (UserSessionSettings) session.getAttribute(UserSessionSettings.SQLDM_USER_SESSION_SETTINGS_PROPERTY);
        String productInstanceName = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
        if (settings.getUserData(UserPreferences.SQLDM_FRESH_LOGIN) == null) {
            List<UserSettings> listSettings = UserSettingFacade.getUserSettings(productInstanceName);
            if (!(listSettings == null)) {
                for (int i = 0; i < listSettings.size(); i++) {
                    UserSettings userSettings = listSettings.get(i);
                    if (UserPreferences.SQLDM_USER_PREFERENCE_LIST.contains(userSettings.getKey())) {
                        settings.setUserData(userSettings.getKey(), userSettings.getValue());
                    }
                }
            }
            settings.setUserData(UserPreferences.SQLDM_FRESH_LOGIN, "");
            session.setAttribute(UserSessionSettings.SQLDM_USER_SESSION_SETTINGS_PROPERTY, settings);
        }
    }

	private void setNavElementStyle(A navLink, Div navDiv) {

		if (isCurrentPage(navLink.getHref())) {
			navDiv.setSclass("padding-t-16 padding-lr-24 nav-bar-element nav-bar-height nav-bar-element-current-page");

		} else {

			navDiv.setSclass("padding-t-16 padding-lr-24 nav-bar-element nav-bar-height");
		}
	}

    public String CredentialsToAuthHeader(Credentials creds) throws UnsupportedEncodingException {
        return "Basic " + DatatypeConverter
                .printBase64Binary((creds.getUserPrincipal().getName() + ":" + creds.getPassword().toString()).getBytes("UTF-8"));
    }

    @Listen("onSelect = #comboBox")
    public void selectProduct(SelectEvent event) {
        // = "";
        //event.getSelectedItems();
        String productInstanceName = selectListModel.get(comboBox.getSelectedIndex());
        Product product = new Product();
        try {
            product = SQLDMRestClient.getInstance().getProductByInstanceName(productInstanceName);
        } catch (RestException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
//	    Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(product, "home"));
        PreferencesUtil.getInstance().setDashboardPreferencesInSession(productInstanceName);
        EventQueue<Event> eq = EventQueues.lookup("changeProduct",
                EventQueues.DESKTOP, false);
        if (eq != null) {
            eq.publish(new Event("productChanged"));
        }

        String header = null;
        try {
            ICSTokenCredentials curr = null;
            try {
                SecurityContext sc = SecurityContextHolder.getContext();
                curr = (ICSTokenCredentials)com.idera.server.web.session.SessionUtil.getCurrentUserCredentials();
                if (curr != null) {
                    com.idera.sqldm.server.web.session.SessionUtil.setSessionVariable("SqldmUserSetting", curr);
                    log.debug("Setting the Credentials " + curr.getUserPrincipal());
                }
            } catch (Exception e) {
                log.error("Error while setting the Credentials "+e.getMessage());
            }
            if (curr == null) {
                curr = (ICSTokenCredentials) com.idera.sqldm.server.web.session.SessionUtil.getSessionVariable("SqldmUserSetting");
                log.debug("Got from session " + curr);
            }
            if(curr instanceof ICSTokenCredentials) {
                header = ((ICSTokenCredentials)curr).getToken();
            }else {
                header = CredentialsToAuthHeader(curr);
            }
        } catch (Exception e) {
            log.error("Error in Getting header "+e.getMessage());
        }
        
        String cwfLocation="";
		try {
			cwfLocation = CoreRestClient.getInstance().getAllProducts().get(0).getLocation().toLowerCase();
		}catch(Exception e) {
		}
		Product currentdm = WebUtil.getCurrentProduct();
		boolean isLocalProduct=false;
		if(currentdm.getRestUrl().toLowerCase().contains(cwfLocation)){
			isLocalProduct=true;
		}
		
        String restUrl = WebUtil.getCurrentProduct().getRestUrl()+ "InstancesByName" + "?instanceName=" + productInstanceName;
        if(requestPath.contains("home")) {
            String eventString="Ext.fireEvent('loadInstancesList', \""+ restUrl + "\", \"" + header + "\", \""+isLocalProduct+"\")";
            Clients.evalJavaScript(eventString);
        }
    }

    private boolean isCurrentPage(String pageNameWithExtension) {

        if ((pageNameWithExtension != null)
                && (!pageNameWithExtension.isEmpty())) {

            String requestPath = Executions.getCurrent().getDesktop()
                    .getRequestPath();

            requestPath = FilenameUtils.removeExtension(new File(requestPath).getName());

            if ((requestPath != null) && (!requestPath.isEmpty())) {

                // get the page name without leading slash
                String pageName = new File(pageNameWithExtension).getName();

                if (requestPath.equalsIgnoreCase(pageName)) {

                    return true;

                } else {
                    // the passed page name is NOT equal the current page name
                    return false;
                }
            } else {
                // the request path could not be retrieved, return false
                return false;
            }

        } else {

            // an invalid page name was passed, return false
            return false;
        }
    }

}
