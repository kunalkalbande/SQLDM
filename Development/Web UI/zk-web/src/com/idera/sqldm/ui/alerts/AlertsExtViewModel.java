package com.idera.sqldm.ui.alerts;

import com.idera.common.rest.CoreRestClient;
import com.idera.common.rest.ICSTokenCredentials;
import com.idera.common.rest.JSONHelper;
import com.idera.common.rest.RestException;
import com.idera.cwf.model.Product;
import com.idera.sqldm.data.Filter;
import com.idera.sqldm.data.alerts.Alert;
import com.idera.sqldm.data.alerts.GridAlert;
import com.idera.sqldm.data.alerts.AlertFacade;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.server.web.session.SessionUtil;
import com.idera.sqldm.utils.SQLdmConstants;

import org.apache.http.auth.Credentials;
import org.apache.log4j.Logger;
import org.springframework.security.context.SecurityContext;
import org.springframework.security.context.SecurityContextHolder;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.GlobalCommand;
import org.zkoss.json.parser.JSONParser;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Div;
import org.zkoss.zul.Window;
import com.fasterxml.jackson.core.type.TypeReference;

import javax.xml.bind.DatatypeConverter;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class AlertsExtViewModel extends SelectorComposer<Window> {

    static final long serialVersionUID = 1L;

    private Logger log = Logger.getLogger(AlertsExtViewModel.class);

    @Wire
    private Div alertsShowDetailsListenerDiv;

    @Override
    public void doAfterCompose(Window comp) throws Exception {
        super.doAfterCompose(comp);
        triggerAlertsListGridLoad();
    }

    public String CredentialsToAuthHeader(Credentials creds) throws UnsupportedEncodingException {
        return "Basic " + DatatypeConverter
                .printBase64Binary((creds.getUserPrincipal().getName() + ":" + creds.getPassword().toString()).getBytes("UTF-8"));
    }

    public void triggerAlertsListGridLoad() {
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
        String restUrl = WebUtil.getCurrentProduct().getRestUrl()+ "AlertsForWebConsoleGrid/tzo/0";
        String productRestUrl = WebUtil.getCurrentProduct().getRestUrl();
        List<Product> products=new ArrayList<Product>();
        try {
			products = CoreRestClient.getInstance().getProductInstances(SQLdmConstants.PRODUCT_NAME, null, null);
		} catch (RestException e) {
			e.printStackTrace();
		}
        log.error("AlertsExtViewModel trigger loadAlertsList event is being trigged restUrl: " + restUrl +" Header: "+ header);
        String p="";
        try {
			 p = JSONHelper.serializeToJson(products);
			 log.error("Products Json data"+p);
		} catch (IOException e) {
			e.printStackTrace();
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
        String event="Ext.fireEvent('loadAlertsList', \""+ restUrl + "\", \"" + header + "\", \""+ productRestUrl + "\", \'"+ p +"\', \""+isLocalProduct+"\")";
        Clients.evalJavaScript(event);
    }

    @Listen("onCustomEvent=#alertsShowDetailsListenerDiv")
    public void showAlertDetailsWindow(Event event) {
        Object[] data = (Object[]) event.getData();
        log.error("AlertsExtViewModel triggerAlertShowDetails event was trigged");
        SessionUtil.getSecurityContext();
        Map<Object, Object> args = new HashMap<Object, Object>();
        GridAlert alert = null;
        try {
            alert = JSONHelper.deserializeFromJson((String) data[0], new TypeReference<GridAlert>() {
            });
        }
        catch (IOException e) {
            log.error("AlertsExtViewModel IOException while parsing alert: ", e);
        }
        List<GridAlert> alertList = null;
        try {
            alertList = JSONHelper.deserializeFromJson((String) data[1], new TypeReference<List<GridAlert>>() {
            });
        }
        catch (IOException e) {
            log.error("AlertsExtViewModel IOException while parsing alertsList: ", e);
        }
        args.put("alert", alert);
        args.put("alertsList", alertList);
        Window window = (Window)Executions.createComponents(
                "~./sqldm/com/idera/sqldm/ui/alerts/alertsView.zul", null, args);
        window.doModal();
    }
}
