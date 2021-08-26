package com.idera.sqldm_10_3.rest;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.idera.common.rest.CoreRestClient;
import com.idera.common.rest.RestException;
import com.idera.common.rest.RestResponse;
import com.idera.common.rest.ServiceStatusResponse;
import com.idera.core.CoreConfiguration;
import com.idera.core.user.User;
import com.idera.cwf.model.Product;
import com.idera.cwf.model.RestCoreInstance;
import com.idera.i18n.I18NStrings;
import com.idera.server.web.WebConstants;
import com.idera.server.web.session.SessionUtil;
import com.idera.sqldm_10_3.data.*;
import com.idera.sqldm_10_3.data.alerts.Alert;
import com.idera.sqldm_10_3.data.alerts.AlertMetaData;
import com.idera.sqldm_10_3.data.alerts.AlertMetrics;
import com.idera.sqldm_10_3.data.alerts.Metrics;
import com.idera.sqldm_10_3.data.category.resources.*;
import com.idera.sqldm_10_3.data.customdashboard.CustomDashboard;
import com.idera.sqldm_10_3.data.customdashboard.CustomDashboardWidget;
import com.idera.sqldm_10_3.data.customdashboard.CustomDashboardWidgetData;
import com.idera.sqldm_10_3.data.customdashboard.Types;
import com.idera.sqldm_10_3.data.databases.*;
import com.idera.sqldm_10_3.data.instances.*;
import com.idera.sqldm_10_3.data.queries.*;
import com.idera.sqldm_10_3.data.tags.Tag;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;
import com.idera.sqldm_10_3.data.topten.TopXEnum;
import com.idera.sqldm_10_3.i18n.SQLdmI18NStrings;
import com.idera.sqldm_10_3.server.web.WebUtil;
import com.idera.sqldm_10_3.ui.dashboard.*;
import com.idera.sqldm_10_3.utils.SQLdmConstants;
import com.idera.sqldm_10_3.utils.Utility;
import com.idera.sqldm_10_3.data.*;
import com.idera.sqldm_10_3.data.category.resources.*;
import com.idera.sqldm_10_3.data.customdashboard.CustomDashboard;
import com.idera.sqldm_10_3.data.customdashboard.CustomDashboardWidget;
import com.idera.sqldm_10_3.data.databases.*;
import com.idera.sqldm_10_3.data.instances.DatabaseStatsDetails;
import com.idera.sqldm_10_3.data.instances.Query;
import com.idera.sqldm_10_3.data.instances.SessionGraphDetail;
import com.idera.sqldm_10_3.data.instances.VirtualizationStatsDetails;
import com.idera.sqldm_10_3.data.queries.*;
import com.idera.sqldm_10_3.data.tags.Tag;
import com.idera.sqldm_10_3.data.topten.TopXEnum;
import com.idera.sqldm_10_3.ui.dashboard.*;
import org.apache.http.auth.Credentials;
import org.apache.log4j.Logger;
import org.springframework.security.CredentialsExpiredException;
import org.springframework.security.context.SecurityContext;
import org.springframework.security.context.SecurityContextHolder;
import org.zkoss.json.JSONArray;
import org.zkoss.json.JSONObject;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.*;

public class SQLDMRestClient {

	protected static final Logger log = Logger.getLogger(SQLDMRestClient.class);
	private static SQLDMRestClient _instance;
	// public static String PRODUCT_VERSION = "";

	public synchronized static SQLDMRestClient getInstance() {

		if (_instance == null) {
			_instance = new SQLDMRestClient();
		}

		return _instance;
	}

	private SQLDMRestClient() {
		log.debug("Creating SQLdmProductRestClient");
	}

	private String getProductBaseURL(String restUrl) throws IOException {
		return CoreConfiguration.getIderaCoreServiceHost() + restUrl;
	}
	
	/*
	 * @author Chaitanya Tanwar
	 * */
	public List<RestCoreInstance> getCoreInstanceWithName(String instanceName) throws RestException {
		try {
			RestResponse<ArrayList<RestCoreInstance>> restResponse = CoreRestClient.getInstance().get("/Instances?name="+instanceName,
					new TypeReference<ArrayList<RestCoreInstance>>() {
			});
			return restResponse.getResultObject();
		} catch (RestException x) {
			throw this.getRestException(I18NStrings.EXCEPTION_OCCURRED_GETTING_ALL_CORE_SERVICES_INSTANCES, x);
		}
	}
	
	/*
	 * @author Chaitanya Tanwar
	 * */
	public String getSWAurl(int swaID, String instanceName) throws RestException {
		String serverName = Executions.getCurrent().getServerName();
		int port = Executions.getCurrent().getLocalPort();
		return "https://"+serverName+":"+port+"/render?id="+swaID+"#instanceName="+instanceName;
	}

	public <T> T getProductRestResponseCore(String productInstanceName, String methodCallUrl,
			TypeReference<T> responseObjectTypeReference) throws RestException {
		try {
			log.debug("Making a call to method " + methodCallUrl + " of instance " + productInstanceName);
			Product productInstance;
			productInstance = (Product) com.idera.sqldm_10_3.server.web.session.SessionUtil.getSessionVariable("currentSqldmProduct");
			if (productInstanceName != null) {
				if (productInstance == null
						|| !productInstance.getInstanceName().equalsIgnoreCase(productInstanceName)) {
					productInstance = getProductByInstanceName(productInstanceName);
					com.idera.sqldm_10_3.server.web.session.SessionUtil.setSessionVariable("currentSqldmProduct",
							productInstance);
				}

			}

			String restUrl = productInstance.getRestUrl();
			Credentials curr = null;
			try {
				SecurityContext sc = SecurityContextHolder.getContext();
				curr = SessionUtil.getCurrentUserCredentials();
				if (curr != null) {
					com.idera.sqldm_10_3.server.web.session.SessionUtil.setSessionVariable("SqldmUserSetting", curr);
					log.debug("Setting the Credentials " + curr.getUserPrincipal());
				}
			} catch (Exception e) {
				log.error("Error while setting the Credentials ");
			}
			if (curr == null) {
				curr = (Credentials) com.idera.sqldm_10_3.server.web.session.SessionUtil.getSessionVariable("SqldmUserSetting");
				log.debug("Got from session " + curr);
			}
			if (curr == null) {
				log.debug("ERROR: ********* Creds is still empty");
			}
			String baseUrl = getProductBaseURL(restUrl);
			String encodedMethodCallUrl = URLEncoder.encode(methodCallUrl.replace("#", "%23"), "UTF-8");
			encodedMethodCallUrl = encodedMethodCallUrl.replace("+", "%20");
			log.debug("Making a call to method " + encodedMethodCallUrl + " of " + restUrl);
			RestResponse<T> restResponse = CoreRestClient.getInstance().getWithProductBaseURL(baseUrl,
					encodedMethodCallUrl, responseObjectTypeReference, curr);
			log.debug(restResponse.toString());
			return restResponse.getResultObject();
		} catch (Exception x) {

			log.debug("Rest exception in rest get for method " + methodCallUrl, x);
			if (x instanceof CredentialsExpiredException)
				throw new UnauthorizedAccessException();

			throw getRestException(SQLdmI18NStrings.EXCEPTION_OCCURRED_GETTING_SQL_DM_SERVICEBASEURL, x);
		}
	}

	// for http delete
	public <T> T deleteProductRestResponseCore(String productInstanceName, String methodCallUrl, Object requestObject,
			TypeReference<T> responseObjectTypeReference) throws RestException {
		try {
			log.debug("Making a call to method " + methodCallUrl + " of instance " + productInstanceName);
			Product productInstance;
			if (productInstanceName == null) {
				productInstance = (Product) com.idera.sqldm_10_3.server.web.session.SessionUtil
						.getSessionVariable("currentSqldmProduct");
			} else {
				productInstance = getProductByInstanceName(productInstanceName);
				com.idera.sqldm_10_3.server.web.session.SessionUtil.setSessionVariable("currentSqldmProduct",
						productInstance);
			}
			String restUrl = productInstance.getRestUrl();
			String baseUrl = getProductBaseURL(restUrl);
			String encodedMethodCallUrl = URLEncoder.encode(methodCallUrl.replace("#", "%23"), "UTF-8");
			encodedMethodCallUrl = encodedMethodCallUrl.replace("+", "%20");

			RestResponse<T> restResponse = CoreRestClient.getInstance().delete(baseUrl, encodedMethodCallUrl,
					requestObject, responseObjectTypeReference);
			log.debug(restResponse.toString());
			return restResponse.getResultObject();
		} catch (Exception x) {
			log.debug("Rest exception in rest get for method " + methodCallUrl, x);
			throw getRestException(SQLdmI18NStrings.EXCEPTION_OCCURRED_GETTING_SQL_DM_SERVICEBASEURL, x);
		}

	}

	public <T> T postProductRestResponseCore(String productInstanceName, String methodCallUrl, Object requestObject,
			TypeReference<T> responseObjectTypeReference) throws RestException {
		try {
			log.debug("Making a call to method " + methodCallUrl + " of instance " + productInstanceName);
			Product productInstance;
			if (productInstanceName == null) {
				productInstance = (Product) com.idera.sqldm_10_3.server.web.session.SessionUtil
						.getSessionVariable("currentSqldmProduct");
			} else {
				productInstance = getProductByInstanceName(productInstanceName);
				com.idera.sqldm_10_3.server.web.session.SessionUtil.setSessionVariable("currentSqldmProduct",
						productInstance);
			}

			String restUrl = productInstance.getRestUrl();
			Credentials curr = null;
			try {
				SecurityContext sc = SecurityContextHolder.getContext();
				curr = SessionUtil.getCurrentUserCredentials();
				if (curr != null) {
					com.idera.sqldm_10_3.server.web.session.SessionUtil.setSessionVariable("SqldmUserSetting", curr);
					log.debug("Setting the Credentials " + curr.getUserPrincipal());
				}
			} catch (Exception e) {
				log.error("Error while setting the Credentials ");
			}
			if (curr == null) {
				curr = (Credentials) com.idera.sqldm_10_3.server.web.session.SessionUtil.getSessionVariable("SqldmUserSetting");
				log.debug("Got from session " + curr);
			}
			if (curr == null) {
				log.debug("ERROR: ********* Creds is still empty");
			}

			String baseUrl = getProductBaseURL(restUrl);
			String encodedMethodCallUrl = URLEncoder.encode(methodCallUrl.replace("#", "%23"), "UTF-8");
			encodedMethodCallUrl = encodedMethodCallUrl.replace("+", "%20");
			log.debug("Making a call to method " + encodedMethodCallUrl + " of " + restUrl);
			RestResponse<T> restResponse = CoreRestClient.getInstance().postWithProductBaseURL(baseUrl,
					encodedMethodCallUrl, requestObject, responseObjectTypeReference, curr);
			log.debug(restResponse.toString());
			return restResponse.getResultObject();
		} catch (Exception x) {
			log.debug("Rest exception in rest get for method " + methodCallUrl, x);
			throw getRestException(SQLdmI18NStrings.EXCEPTION_OCCURRED_GETTING_SQL_DM_SERVICEBASEURL, x);
		}
	}

	public List<String> testPing(String productInstanceName, String rStr) throws RestException {
		log.debug("Start of test ping " + rStr);
		return getProductRestResponseCore(productInstanceName, "/ping/" + rStr, new TypeReference<ArrayList<String>>() {
		});
	}

	public List<Product> getProducts(String productInstanceName) throws RestException {

		List<Product> productInstances;
		try {
			/*
			 * if(SQLDMRestClient.PRODUCT_VERSION.isEmpty()||SQLDMRestClient.
			 * PRODUCT_VERSION==null){ SQLDMRestClient.PRODUCT_VERSION =
			 * getVersion(productInstanceName); }
			 */
			productInstances = CoreRestClient.getInstance().getProductInstances(SQLdmConstants.PRODUCT_NAME,
					getVersion(productInstanceName), productInstanceName);

		} catch (RestException e) {
			throw getRestException("Error getting product instance", e);
		}

		return productInstances;
	}

	public Product getProductByInstanceName(String productInstanceName) throws RestException {
		List<Product> product = getProducts(productInstanceName);
		log.debug("In getProductInstance " + productInstanceName);
		if (product.size() == 1) {
			Product p = product.get(0);
			log.debug("** Product" + p.getName() + ", " + p.getInstanceName() + " , " + p.getVersion());
			return p;
		} else {
			log.info("Error. Get Product returned multiple instances " + productInstanceName);
			for (Product p : product) {
				log.info("** Product" + p.getName() + ", " + p.getInstanceName() + " , " + p.getVersion());
			}
		}
		return null;
	}

	private RestException getRestException(String exceptionMessage, Exception restException) {
		return CoreRestClient.getInstance().getRestException(exceptionMessage, restException);
	}

	// TODO To be used once API is there
	public String getVersion(String productInstanceName) throws RestException {
		try {
			List<Product> products = CoreRestClient.getInstance().getProductInstances(SQLdmConstants.PRODUCT_NAME, null,
					productInstanceName);
			if (null != products && products.size() != 0) {
				return products.get(0).getVersion();
			}
		} catch (Exception e) {
			log.error("Error in getting the version of product with product Instance Name:" + productInstanceName
					+ " and error: " + e.getMessage());
		}
		return null;
	}

	// Offset
	// Handling ALL case
	public List<DashboardInstance> getAllDashboardInstances() {
		List<DashboardInstance> allInstancesList = new ArrayList<DashboardInstance>();

		try {
			for (Product product : CoreRestClient.getInstance().getAllProducts()) {
				if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("sqldm")) {

					List<DashboardInstance> singleProductInstances = new ArrayList<DashboardInstance>();
					singleProductInstances = getProductRestResponseCore(product.getInstanceName(),
							RestMethods.GET_DASHBOARD_INSTANCES.getMethodName(),
							new TypeReference<ArrayList<DashboardInstance>>() {
							});

					if (product != null) {
						for (DashboardInstance dashboardInstance : singleProductInstances) {
							dashboardInstance.setProduct(product);
						}

						allInstancesList.addAll(singleProductInstances);
					}

				}
			}
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		return allInstancesList;
	}

	public List<DashboardInstance> getDashboardInstances(String productInstanceName) throws RestException {

		List<DashboardInstance> instancesList = new ArrayList<DashboardInstance>();

		if (productInstanceName != null && productInstanceName.equals("All"))
			return getAllDashboardInstances();

		instancesList.addAll(getProductRestResponseCore(productInstanceName,
				RestMethods.GET_DASHBOARD_INSTANCES.getMethodName(), new TypeReference<ArrayList<DashboardInstance>>() {
				}));
		Product product = getProductByInstanceName(productInstanceName);
		if (product != null) {
			for (DashboardInstance dashboardInstance : instancesList) {
				dashboardInstance.setProduct(product);
			}
		}
		return instancesList;

	}

	public List<DashboardInstance> getDashboardInstances(String productInstanceName, String type, String value)
			throws RestException {
		return getProductRestResponseCore(
				productInstanceName, RestMethods.GET_FILTERED_DASHBOARD_INSTANCES.getMethodName()
						.replace("<field>", type).replace("<value>", value),
				new TypeReference<ArrayList<DashboardInstance>>() {
				});
	}

	public List<DashboardAlertsByCategoryWidget> getNumAlertsByCategory(String productInstanceName)
			throws RestException {
		return getNumAlertsByCategory(productInstanceName, false);
	}

	// Handling ALL products case
	public List<DashboardAlertsByCategoryWidget> getNumAlertsByCategoryAll() {

		List<DashboardAlertsByCategoryWidget> alertsByCategoryWidget = new ArrayList<DashboardAlertsByCategoryWidget>();

		try {
			for (Product product : CoreRestClient.getInstance().getAllProducts()) {
				if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("sqldm")) {
					List<DashboardAlertsByCategoryWidget> listForSingleProduct = new ArrayList<DashboardAlertsByCategoryWidget>();
					listForSingleProduct = getProductRestResponseCore(product.getInstanceName(),
							RestMethods.GET_NUM_ALERT_BY_CATEGORY.getMethodName(),
							new TypeReference<ArrayList<DashboardAlertsByCategoryWidget>>() {
							});
					for (DashboardAlertsByCategoryWidget dashboardAlertsByCategoryWidget : listForSingleProduct) {
						dashboardAlertsByCategoryWidget.setProduct(product);
					}
					alertsByCategoryWidget.addAll(listForSingleProduct);
				}

			}
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		return alertsByCategoryWidget;
	}

	public List<DashboardAlertsByCategoryWidget> getNumAlertsByCategory(String productInstanceName,
                                                                        boolean needInstancesInResponse) throws RestException {
		List<DashboardAlertsByCategoryWidget> list = new ArrayList<DashboardAlertsByCategoryWidget>();

		if (productInstanceName.equals("All"))
			return getNumAlertsByCategoryAll();
		Product product = getProductByInstanceName(productInstanceName);
		list = getProductRestResponseCore(productInstanceName, RestMethods.GET_NUM_ALERT_BY_CATEGORY.getMethodName(),
				new TypeReference<ArrayList<DashboardAlertsByCategoryWidget>>() {
				});
		for (DashboardAlertsByCategoryWidget dashboardAlertsByCategoryWidget : list) {
			dashboardAlertsByCategoryWidget.setProduct(product);
		}
		return list;
	}

	// Handling ALL products case
	public List<DashboardAlertsByDatabaseWidget> getNumAlertsByDatabaseAll() {
		List<DashboardAlertsByDatabaseWidget> alertsByDB = new ArrayList<DashboardAlertsByDatabaseWidget>();

		try {
			for (Product product : CoreRestClient.getInstance().getAllProducts()) {
				if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("sqldm")) {

					List<DashboardAlertsByDatabaseWidget> listForSingleProduct = new ArrayList<DashboardAlertsByDatabaseWidget>();

					listForSingleProduct = getProductRestResponseCore(product.getInstanceName(),
							RestMethods.GET_NUM_ALERT_BY_DATABASE.getMethodName(),
							new TypeReference<ArrayList<DashboardAlertsByDatabaseWidget>>() {
							});
					for (DashboardAlertsByDatabaseWidget dashboardAlertsByDatabaseWidget : listForSingleProduct) {
						dashboardAlertsByDatabaseWidget.setProduct(product);
					}
					alertsByDB.addAll(listForSingleProduct);
				}

			}
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		return alertsByDB;

	}

	public List<DashboardAlertsByDatabaseWidget> getNumAlertsByDatabase(String productInstanceName)
			throws RestException {
		List<DashboardAlertsByDatabaseWidget> list = new ArrayList<DashboardAlertsByDatabaseWidget>();
		Product product = getProductByInstanceName(productInstanceName);

		if (productInstanceName.equals("All"))
			return getNumAlertsByDatabaseAll();
		list = getProductRestResponseCore(productInstanceName, RestMethods.GET_NUM_ALERT_BY_DATABASE.getMethodName(),
				new TypeReference<ArrayList<DashboardAlertsByDatabaseWidget>>() {
				});
		for (DashboardAlertsByDatabaseWidget dashboardAlertsByDatabaseWidget : list) {
			dashboardAlertsByDatabaseWidget.setProduct(product);
		}
		return list;
	}

	// Offset
	// Handling ALL products case
	public List<DashboardWorstResponseTimeWidget> getAllLatestResponseTime(String offSet) {
		List<DashboardWorstResponseTimeWidget> worstResponseTime = new ArrayList<DashboardWorstResponseTimeWidget>();

		try {
			for (Product product : CoreRestClient.getInstance().getAllProducts()) {
				if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("sqldm")) {

					List<DashboardWorstResponseTimeWidget> listForSingleProd = new ArrayList<DashboardWorstResponseTimeWidget>();
					listForSingleProd = getProductRestResponseCore(
							product.getInstanceName(), RestMethods.GET_LATEST_RESPONSE_TIME_BY_INSTANCE.getMethodName()
									.replace("{timeZoneOffset}", offSet),
							new TypeReference<ArrayList<DashboardWorstResponseTimeWidget>>() {
							});
					for (DashboardWorstResponseTimeWidget dashboardWorstResponseTimeWidget : listForSingleProd) {
						dashboardWorstResponseTimeWidget.setProduct(product);
					}
					worstResponseTime.addAll(listForSingleProd);
				}

			}
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		return worstResponseTime;
	}

	public List<DashboardWorstResponseTimeWidget> getLatestResponseTime(String productInstanceName, String offSet)
			throws RestException {

		List<DashboardWorstResponseTimeWidget> list = new ArrayList<DashboardWorstResponseTimeWidget>();
		Product product = getProductByInstanceName(productInstanceName);

		if (productInstanceName.equals("All"))
			return getAllLatestResponseTime(offSet);

		list = getProductRestResponseCore(productInstanceName,
				RestMethods.GET_LATEST_RESPONSE_TIME_BY_INSTANCE.getMethodName().replace("{timeZoneOffset}", offSet),
				new TypeReference<ArrayList<DashboardWorstResponseTimeWidget>>() {
				});
		for (DashboardWorstResponseTimeWidget dashboardWorstResponseTimeWidget : list) {
			dashboardWorstResponseTimeWidget.setProduct(product);
		}

		return list;
	}

	public List<ServerStatus> getInstancesSummary(String productInstanceName) throws RestException {
		return getProductRestResponseCore(productInstanceName,
				RestMethods.GET_INSTANCES_SUMMARY.getMethodName() + "?ActiveOnly=true",
				new TypeReference<ArrayList<ServerStatus>>() {
				});
	}

	public List<Tag> getTags(String productInstanceName) throws RestException {
		return getProductRestResponseCore(productInstanceName, RestMethods.GET_DASHBOARD_TAGS.getMethodName(),
				new TypeReference<ArrayList<Tag>>() {
				});
	}

	public DashboardInstance getDashboardInstance(String productInstanceName,
                                                  int instanceId /* , String offSet */) throws RestException {
		String offSet = setOffSet();
		String instanceName=null;
		DashboardInstance dashboardInstance = new DashboardInstance();
		dashboardInstance = getProductRestResponseCore(productInstanceName,
				RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId + "/tzo/" + offSet,
				new TypeReference<DashboardInstance>() {
				});
		try {
			if(dashboardInstance == null){
				return dashboardInstance;
			}
			instanceName = URLEncoder.encode(dashboardInstance.getOverview().getInstanceName() , "UTF-8");
		} catch (UnsupportedEncodingException e1) {
			log.error("Error while encoding the instance name");
			Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(WebUtil.getCurrentProduct(),"/home"));
		}
		catch (NullPointerException e1) {
			log.error("Null pointer exception while accessing the dashboard instance name");
			Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(WebUtil.getCurrentProduct(),"/home"));
		}
		List<RestCoreInstance> restCoreInstanceList = getCoreInstanceWithName(instanceName);
		if(restCoreInstanceList.size()!=0){
			RestCoreInstance restCoreInstance = restCoreInstanceList.get(0);

			List<Product> productList = restCoreInstance.getProducts();
			for(int j=0; j<productList.size(); j++){
				Product product = productList.get(j);
				dashboardInstance.setIsSWAInstance(false);
				if(product.getProductNameWithoutInstanceName().equals("SQLWorkloadAnalysis")){
					dashboardInstance.setIsSWAInstance(true);
					dashboardInstance.setSwaID(product.getProductId());
					break;
				}
			}
		}
		//return dashboardInstance
		return dashboardInstance;
	}

	public ServerStatus getDashboardInstanceServerStats(String productInstanceName, int instanceId,
                                                        int numHistoryMinutes) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.SERVER_STATISTICS.getMethodName() + "/tzo/" + offSet;
		/*
		 * String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() +
		 * "/" + instanceId + RestMethods.SERVER_STATISTICS.getMethodName() +
		 * "/tzo/"+0.0;
		 */

		if (numHistoryMinutes > 0) {
			url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		}
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ServerStatus>() {
		});
	}
	
	/*
	 * ChaitanyaTanwar DM 10.2
	 * */
	public ServerStatus getDashboardInstanceServerStats(String productInstanceName, int instanceId,
                                                        long numHistoryMinutes, Date endTime) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.SERVER_STATISTICS.getMethodName() + "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String toDate = Utility.getUtcDateString(endTime);
		url = url + "&endDate=" + toDate;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ServerStatus>() {
		});
	}

	public Alert getAlert(String productInstanceName, Long id) throws RestException {
		Alert alert;
		if (id == null) {
			return null;
		}
		Product product = getProductByInstanceName(productInstanceName);
		String offset = setOffSet();
		String url = "/Alerts";
		url = url + "/" + id;
		url = url + "/tzo/" + offset;
		alert = getProductRestResponseCore(productInstanceName, url, new TypeReference<Alert>() {
		});
		alert.setProduct(product);
		return alert;

	}

	public List<Metrics> getMetrics(String productInstanceName, String offSet) throws RestException {
		String url = RestMethods.GET_METRICS.getMethodName();
		url = url.replace("{timeZoneOffset}", offSet);
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<Metrics>>() {
		});
	}

	public List<TimedValue> getMetricsHistoryForAlert(String productInstanceName, Long alertId, Integer numHistoryHours,
                                                      String offSet) throws RestException {
		String url = RestMethods.GET_ALERT_HISTORY.getMethodName();
		url = url.replace("{timeZoneOffset}", offSet);
		url = url.replace("{alertId}", alertId.toString());
		StringBuffer requestParams = new StringBuffer("?");
		if (numHistoryHours != null) {
			requestParams.append("numHistoryHours=" + numHistoryHours);
		}
		if (requestParams.length() > 1) {
			url = url + requestParams;
		}
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<TimedValue>>() {
		});
	}

	// Handling All case
	public List<Alert> getAllAbridgeAlerts(boolean isActive, Date startTime, Date endTime, Integer instanceId,
                                           Integer metricId, Integer severity, String category, Integer limit, String offSet) throws RestException {

		String url = RestMethods.GET_ALERTS_ABRIDGE.getMethodName();
		url = url.replace("{timeZoneOffset}", offSet);
		List<Alert> allAbridgedAlerts = new ArrayList<Alert>();

		try {
			for (Product product : CoreRestClient.getInstance().getAllProducts()) {
				if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("sqldm")) {

					List<Alert> listForSingleProduct = new ArrayList<Alert>();

					listForSingleProduct = getAlerts(product.getInstanceName(), isActive, startTime, endTime,
							instanceId, metricId, severity, category, limit, null, null, url, offSet);

					for (Alert alert : listForSingleProduct) {
						alert.setProduct(product);
					}
					allAbridgedAlerts.addAll(listForSingleProduct);
				}

			}
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		return allAbridgedAlerts;
	}

	public List<Alert> getAbridgeAlerts(String productInstanceName, boolean isActive, Date startTime, Date endTime,
                                        Integer instanceId, Integer metricId, Integer severity, String category, Integer limit, String offSet)
					throws RestException {
		String url = RestMethods.GET_ALERTS_ABRIDGE.getMethodName();
		url = url.replace("{timeZoneOffset}", offSet);
		List<Alert> list = new ArrayList<Alert>();
		Product product = getProductByInstanceName(productInstanceName);

		if (productInstanceName.equals("All"))
			return getAllAbridgeAlerts(isActive, startTime, endTime, instanceId, metricId, severity, category, limit,
					offSet);
		list = getAlerts(productInstanceName, isActive, startTime, endTime, instanceId, metricId, severity, category,
				limit, null, null, url, offSet);
		for (Alert alert : list) {
			alert.setProduct(product);
		}

		return list;
	}

	public List<Alert> getAlerts(String productInstanceName, boolean isActive, Date startTime, Date endTime,
                                 Integer instanceId, Integer metricId, Integer severity, String category, String offSet)
					throws RestException {
		return getAlerts(productInstanceName, isActive, startTime, endTime, instanceId, metricId, severity, category,
				-1, null, null, null, offSet);
	}

	public List<Alert> getAlertsForAllProducts(String url) {

		List<Alert> allAlertList = new ArrayList<Alert>();

		try {
			for (Product product : CoreRestClient.getInstance().getAllProducts()) {
				if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("sqldm")) {
					List<Alert> listForSingleProduct = new ArrayList<Alert>();

					try {
						listForSingleProduct = getProductRestResponseCore(product.getInstanceName(), url,
								new TypeReference<ArrayList<Alert>>() {
								});
					} catch (RestException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					}
					for (Alert alert : listForSingleProduct) {
						alert.setProduct(product);
					}
					allAlertList.addAll(listForSingleProduct);
				}
			}
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return allAlertList;
	}

	public List<Alert> getAlerts(String productInstanceName, boolean isActive, Date startTime, Date endTime,
                                 Integer instanceId, Integer metricId, Integer severity, String category, int limit, String orderBy,
                                 String orderType, String urlToUse, String offSet) throws RestException {

		Product product = getProductByInstanceName(productInstanceName);
		List<Alert> list = new ArrayList<Alert>();

		String fromDate = null;
		String toDate = null;
		StringBuffer requestParams = new StringBuffer("?");
		if (startTime != null) {
			fromDate = Utility.getUtcDateString(startTime);
			requestParams.append("startTime=" + fromDate + "&");
		}
		if (endTime != null) {
			toDate = Utility.getUtcDateString(endTime);
			requestParams.append("endTime=" + toDate + "&");
		}
		if (isActive) {
			requestParams.append("activeOnly=" + isActive + "&");
		}
		if (instanceId != null) {
			requestParams.append("instanceId=" + instanceId + "&");
		}
		if (metricId != null) {
			requestParams.append("metric=" + metricId + "&");
		}
		if (severity != null) {
			requestParams.append("severity=" + severity + "&");
		}
		if (category != null) {
			requestParams.append("category=" + category + "&");
		}
		if (limit != -1) {
			requestParams.append("limit=" + limit + "&");
		}
		if (null != orderBy) {
			requestParams.append("orderBy=" + orderBy + "&");
		}
		if (null != orderType) {
			requestParams.append("orderType=" + orderType + "&");
		}
		// remove last & from the querystring
		requestParams.setLength(requestParams.length() - 1);
		String url = (urlToUse != null) ? urlToUse : RestMethods.GET_ALERTS.getMethodName();
		url = url.replace("{timeZoneOffset}", offSet);
		if (requestParams.length() > 1) {
			url = url + requestParams;
		}
		if (productInstanceName != null && productInstanceName.equalsIgnoreCase("All")) {
			return getAlertsForAllProducts(url);
		}

		list = getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<Alert>>() {
		});
		for (Alert alert : list) {
			alert.setProduct(product);
		}
		return list;
	}

	public List<AlertMetaData> getAlertMetaData(String productInstanceName, AlertMetrics metric,
                                                boolean includeTemplate) throws RestException {

		StringBuffer url = new StringBuffer(RestMethods.GET_ALERT_META_DATA.getMethodName());
		url.append("?metric_id=").append(metric.ordinal());
		url.append("&template=").append(Boolean.toString(includeTemplate));
		return getProductRestResponseCore(productInstanceName, url.toString(),
				new TypeReference<List<AlertMetaData>>() {
				});
	}

	public List<Database> getTopDatabasesBySize(String productInstanceName, Integer instanceId, int count)
			throws RestException {
		String instanceIdParameter = "";
		if (instanceId != null) {
			instanceIdParameter = "instance_id=" + instanceId + "&";
		}
		String url = RestMethods.GET_TOP_DATABASES_BY_SIZE.getMethodName() + "?" + instanceIdParameter + "count="
				+ count;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<List<Database>>() {
		});
	}

	public List<Database> getTopDatabasesByActivity(String productInstanceName, Integer instanceId, int count)
			throws RestException {
		String instanceIdParameter = "";
		if (instanceId != null) {
			instanceIdParameter = "instance_id=" + instanceId + "&";
		}

		String url = RestMethods.GET_TOP_DATABASES_BY_ACTIVITY.getMethodName() + "?" + instanceIdParameter + "count="
				+ count;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<List<Database>>() {
		});
	}

	public List<String> getSQLInstanceVersions(String productInstanceName, boolean includeDisabled,
			boolean includeManaged, boolean includeDiscovered, boolean includeIgnored) throws RestException {
		String url = RestMethods.SQL_INSTANCE_VERSION.getMethodName() + "?include_disabled=" + includeDisabled
				+ "&managed=" + includeManaged + "&discovered=" + includeDiscovered + "&ignored=" + includeIgnored;

		return getProductRestResponseCore(productInstanceName, url, new TypeReference<List<String>>() {
		});
	}

	public AggregateFilteredInstancesContainer getAggregateFilteredInstances(String productInstanceName,
                                                                             AggregatedFilter aggregatedFilter) throws RestException {
		String url = RestMethods.GET_AGGREGATED_FILTER_INSTANCE.getMethodName();
		return getProductRestResponseCore(productInstanceName, url,
				new TypeReference<AggregateFilteredInstancesContainer>() {
				});
	}

	public FilteredInstanceDatabasesContainer getFilteredInstanceDatabases(String productInstanceName,
                                                                           AggregatedFilter aggregatedFilter) throws RestException {
		String url = RestMethods.GET_FILTERED_INSTANCE_DATABASE.getMethodName();
		return getProductRestResponseCore(productInstanceName, url,
				new TypeReference<FilteredInstanceDatabasesContainer>() {
				});
	}

	public FilteredInstancesContainer getFilteredInstances(String productInstanceName,
                                                           AggregatedFilter aggregatedFilter) throws RestException {
		String url = RestMethods.GET_SELECTED_FILTERED_DATABASE.getMethodName();
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<FilteredInstancesContainer>() {
		});

	}

	public DatabaseDetails getDatabaseDetails(String productInstanceName, Integer databaseId) throws RestException {
		StringBuffer url = new StringBuffer(RestMethods.GET_DATABASE_DETAILS.getMethodName());
		url.append("?database_id=").append(databaseId.toString());
		return getProductRestResponseCore(productInstanceName, url.toString(), new TypeReference<DatabaseDetails>() {
		});
	}

	public LicenseDetails getLicenseDetails(String productInstanceName) throws RestException {
		return getProductRestResponseCore(productInstanceName, RestMethods.GET_LICENSE_DETAILS.getMethodName(),
				new TypeReference<LicenseDetails>() {
				});
	}

	public ServiceStatusResponse getServiceStatus(String productInstanceName) throws RestException {
		return getProductRestResponseCore(productInstanceName, RestMethods.GET_SERVICE_STATUS.getMethodName(),
				new TypeReference<ServiceStatusResponse>() {
				});
	}

	public SQLInstanceCounts getSqlInstanceCounts(String productInstanceName) throws RestException {
		return getProductRestResponseCore(productInstanceName, RestMethods.GET_SQL_INSTANCE_COUNTS.getMethodName(),
				new TypeReference<SQLInstanceCounts>() {
				});
	}

	public List<InstanceDetailsDatabase> getInstanceDetailsDatabases(String productInstanceName,
                                                                     int instanceId /* , String offSet */) throws RestException {

		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_DATABASES_FOR_SINGLE_INSTANCE_VIEW.getMethodName() + "/tzo/" + offSet;

		return getProductRestResponseCore(productInstanceName, url,
				new TypeReference<ArrayList<InstanceDetailsDatabase>>() {
				});
	}

	public List<InstanceDetailsSession> getInstanceDetailsSession(String productInstanceName,
                                                                  int instanceId /* , String offSet */) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_SESSIONS_FOR_SINGLE_INSTANCE_VIEW.getMethodName() + "/tzo/" + offSet;
		url = url + "?UserSessionsOnly=true&excludeSQLDMSessions=true";
		return getProductRestResponseCore(productInstanceName, url,
				new TypeReference<ArrayList<InstanceDetailsSession>>() {
				});
	}

	/**
	 * This method will return Top X Widgets' instances in SQLdm environment.
	 * 
	 * @param topXEnum
	 * @param limit
	 * @param days
	 * @param typeRef
	 * @return
	 * @throws RestException
	 */
	@SuppressWarnings("unchecked")
	public List<IWidgetInstance> getTopXInstances(String productInstanceName, TopXEnum topXEnum, int limit, int days)
			throws RestException {
		List<IWidgetInstance> responseList = new ArrayList<IWidgetInstance>();
		String url = null;
		StringBuffer requestParams = new StringBuffer("?");
		topXEnum.getOFFSET_IN_HOURS();
		if (days != -1) {
			requestParams.append("numDays=" + days + "&");
		}

		if (limit != -1) {
			requestParams.append("limit=" + limit);
		}

		if (topXEnum.getInstanceId() != -1) {
			requestParams.append("&InstanceID=" + topXEnum.getInstanceId());
		}

		if (topXEnum.name().equalsIgnoreCase("DATABASE_WITH_MOST_ALERTS")
				|| topXEnum.name().equalsIgnoreCase("SQL_MEMORY_USAGE") || topXEnum.name().equalsIgnoreCase("IO")
				|| topXEnum.name().equalsIgnoreCase("SQL_CPU_LOAD")
				|| topXEnum.name().equalsIgnoreCase("INSTANCE_ALERT") || topXEnum.name().equalsIgnoreCase("WAITS")) {
			url = topXEnum.getRestMethod().getMethodName() + requestParams;
		} else
			url = topXEnum.getRestMethod().getMethodName().replace("{timeZoneOffset}", topXEnum.getOFFSET_IN_HOURS())
					+ requestParams;

		if (productInstanceName.equals("All")) {

			try {
				for (Product product : CoreRestClient.getInstance().getAllProducts()) {
					if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("sqldm")) {

						List<IWidgetInstance> listForSingleProduct = new ArrayList<IWidgetInstance>();
						listForSingleProduct = (List<IWidgetInstance>) getProductRestResponseCore(
								product.getInstanceName(), url, topXEnum.getTypeReference());
						for (IWidgetInstance widget : listForSingleProduct) {
							widget.setProduct(product);
						}
						responseList.addAll(listForSingleProduct);
					}

				}
			} catch (RestException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}

			return responseList;

		}

		responseList = (List<IWidgetInstance>) getProductRestResponseCore(productInstanceName, url,
				topXEnum.getTypeReference());
		return responseList;
	}

	/**
	 * This method will get me queries for this instance. This will be displayed
	 * in Query tab of category view.
	 * 
	 * @param instanceId
	 *            of the instance for which we are displaying category view.
	 * @return
	 * @throws RestException
	 */
	public List<Query> getQueryInstance(String productInstanceName, int instanceId, int numHistoryMinutes,
                                        int queriesCount /* , String offSet */) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_QUERY_INSTANCE.getMethodName().replace("{timeZoneOffset}", offSet);
		url = url.replace("{id}", new Integer(instanceId).toString());
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		if (queriesCount > 0) {
			StringBuffer requestParams = new StringBuffer("&");
			requestParams.append("limit=").append(queriesCount);
			url = url + requestParams;
		}
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<Query>>() {
		});
	}

	/**
	 * Author:Accolite
	 * Date : 20th Jan, 2017
	 * Database Availability Group- 10.2 release
	 * @param productInstanceName
	 * @param instanceId
	 * @param numHistoryMinutes
	 * @return
	 * @throws RestException
	 */
	public List<AvailabilityGroupDetails> getAvailabilityGroupDetails(String productInstanceName,
                                                                      int instanceId , int numHistoryMinutes) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DATABASE_AVAILABILITY_GRP_STATS.getMethodName().replace("{timeZoneOffset}", offSet);
		url = url.replace("{id}", new Integer(instanceId).toString());
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url,
				new TypeReference<ArrayList<AvailabilityGroupDetails>>() {
				});
	}
	
	/**
	 * Author:Accolite
	 * Date : 20th Jan, 2017
	 * Database Availability Group- 10.2 release
	 * @param productInstanceName
	 * @param instanceId
	 * @param numHistoryMinutes
	 * @param endDate
	 * @return
	 * @throws RestException
	 */
	public List<AvailabilityGroupDetails> getAvailabilityGroupDetails(String productInstanceName,
			int instanceId, long numHistoryMinutes, Date endDate) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DATABASE_AVAILABILITY_GRP_STATS.getMethodName().replace("{timeZoneOffset}", offSet);
		url = url.replace("{id}", new Integer(instanceId).toString());
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String date = Utility.getUtcDateString(endDate);
		url = url + "&endDate=" + date;
		return getProductRestResponseCore(productInstanceName, url,
				new TypeReference<ArrayList<AvailabilityGroupDetails>>() {
				});
	}

	
///--------------auth-Rishabh Mishra------///
	public GearViewBean updateAllScaleFactor(HealthIndexCoefficients hc,
                                             List<InstanceScaleFactorList> isf, List<TagsScaleFactorList> tsf) throws RestException {
		GearViewBean gvb=new GearViewBean();
		for (Product product : CoreRestClient.getInstance().getAllProducts()) {
			String productInstanceName = product.getInstanceName();
			int productId=product.getProductId();
			if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("sqldm") )
			{List<InstanceScaleFactorList> instanceList1=new ArrayList<InstanceScaleFactorList>();
			for (InstanceScaleFactorList element : isf) {
				if((element.getSQLServerId()/1000)==productId)
					{element.setSQLServerId(element.getSQLServerId()%1000);
					instanceList1.add(element);}
			}
			
			List<TagsScaleFactorList> tagList1=new ArrayList<TagsScaleFactorList>();
			for (TagsScaleFactorList element : tsf) {
				if((element.getTagId()/1000)==productId)
					{element.setTagId(element.getTagId()%1000);
					tagList1.add(element);}
			}
			gvb= SQLDMRestClient.getInstance().updateScaleFactor(productInstanceName, hc, instanceList1, tagList1);
			}
	}
		return gvb;
	}
	
	public GearViewBean updateScaleFactor(String productInstanceName, HealthIndexCoefficients hc,
			List<InstanceScaleFactorList> isf, List<TagsScaleFactorList> tsf) throws RestException {
		GearViewBean gvb=new GearViewBean();
		if (productInstanceName.equalsIgnoreCase("All") ) 
			updateAllScaleFactor( hc, isf,  tsf);
			else{
		Map<String, Object> jsonValues = new HashMap<String, Object>();
		jsonValues.put("healthCoefficient", hc);
		jsonValues.put("ins", isf);
		jsonValues.put("tag", tsf);
		String url = RestMethods.UPDATE_SCALE_FACTORS.getMethodName();
		try {
			JSONObject json = new JSONObject();
			json.putAll(jsonValues);
			log.info("Request Object: " + json);

			gvb= postProductRestResponseCore(productInstanceName, url, json, new TypeReference<GearViewBean>() {
			});
		} catch (Exception e) {
			e.printStackTrace();
		}
			}
		return gvb;
	}

	public List<GearViewBean> getAllScaleFactor() throws RestException {
		List<GearViewBean> gvbList=new ArrayList<GearViewBean>();
			try {
			for (Product product : CoreRestClient.getInstance().getAllProducts()) {
			String	productInstanceName = product.getInstanceName();
				if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("sqldm")) {
					GearViewBean gvb = SQLDMRestClient.getInstance().getScaleFactor(productInstanceName).get(0);
					
					List<InstanceScaleFactorList> instanceList1 =  gvb.getInstanceList();
					if (instanceList1 != null) {
						for (InstanceScaleFactorList element : instanceList1) {
							if (element != null) {
								element.setSQLServerId((product.getProductId()*1000)+element.getSQLServerId());
							}
						}
					}
					
					List<TagsScaleFactorList> tagList1 =gvb.getTagsList();
					if(tagList1!=null)
					{
						for (TagsScaleFactorList element : tagList1) {
							element.setTagId((product.getProductId()*1000)+element.getTagId());
						}
					}
					gvbList.add(gvb);
				}
			}
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
			return gvbList;
	}
	
	public List<GearViewBean> getScaleFactor(String productInstanceName) throws RestException {
		List<GearViewBean> gvbList=new ArrayList<GearViewBean>();
		
		if(productInstanceName.equalsIgnoreCase("All"))
			gvbList=getAllScaleFactor();
		else{
			try{
		GearViewBean gvb=getProductRestResponseCore(productInstanceName, RestMethods.GET_SCALE_FACTORS.getMethodName(),
				new TypeReference<GearViewBean>() {
				});
		gvbList.add(gvb);
			} catch (RestException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	return gvbList;
	}
	
	public AnalysisSummary getAnalysisSummary(String productInstanceName, Integer instanceId) throws RestException {
		AnalysisSummary analysisSummary=new AnalysisSummary();
		
		/*if(productInstanceName.equalsIgnoreCase("All"))
			gvbList=getAllScaleFactor();
		else{*/
		String url =  RestMethods.GET_ANALSIS_SUMMARY.getMethodName();
		url = url.replace("{instanceId}", ""+instanceId);
			try{
				analysisSummary=getProductRestResponseCore(productInstanceName, url,
				new TypeReference<AnalysisSummary>() {
				});
			} catch (RestException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		/*}*/
	return analysisSummary;
	}
//--------------------//
	
	
	public CustomDashboard createCustomDashboard(String productInstanceName, String customDashboardName,
                                                 boolean isDefault, String userSID) throws RestException {
		String url = RestMethods.CREATE_CUSTOMDASHBOARD.getMethodName();

		// customDashboardName = customDashboardName.replaceAll("[\\\\]",
		// "\\\\\\\\");
		// customDashboardName =
		// customDashboardName.replaceAll("[\"]","\\\\\\\"");
		customDashboardName = customDashboardName.replace("\\", "\\\\");
		customDashboardName = customDashboardName.replace("\"", "\\\"");

		String requestObjectString = "{";

		requestObjectString = requestObjectString + "\"customDashboardName\":\"" + customDashboardName + "\", ";
		requestObjectString = requestObjectString + "\"isDefault\":\"" + isDefault + "\"} ";
		/*
		 * requestObjectString = requestObjectString + "\"userSID\":\"" +
		 * userSID + "\"} ";
		 */

		ObjectMapper mapper = new ObjectMapper();
		try {
			JsonNode requestObject = mapper.readTree(requestObjectString);

			log.info("Request Object: " + requestObjectString);

			return postProductRestResponseCore(productInstanceName, url, requestObject,
					new TypeReference<CustomDashboard>() {
					});
		} catch (JsonProcessingException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}

		return postProductRestResponseCore(productInstanceName, url, null, new TypeReference<CustomDashboard>() {
		});

	}

	public boolean copyCustomDashboard(String productInstanceName, Integer customDashboardId) throws RestException {
		String url = RestMethods.COPY_CUSTOMDASHBOARD.getMethodName();
		url = url.replace("{customDashboardid}", customDashboardId.toString());

		return postProductRestResponseCore(productInstanceName, url, null, new TypeReference<Boolean>() {
		});
	}

	public CustomDashboard updateCustomDashboard(String productInstanceName, Integer customDashboardId,
			String customDashboardName, boolean isDefault, String userSID, List<String> tags) throws RestException {
		String url = RestMethods.UPDATE_CUSTOMDASHBOARD.getMethodName();
		url = url.replace("{customDashboardid}", customDashboardId.toString());

		// customDashboardName = customDashboardName.replaceAll("[\\\\]",
		// "\\\\\\\\");
		// customDashboardName =
		// customDashboardName.replaceAll("[\"]","\\\\\\\"");
		customDashboardName = customDashboardName.replace("\\", "\\\\");
		customDashboardName = customDashboardName.replace("\"", "\\\"");

		String tagStr = tags.toString();

		// tagStr = tagStr.replaceAll("[\\\\]", "\\\\\\\\");
		// tagStr = tagStr.replaceAll("[\"]","\\\\\\\"");
		tagStr = tagStr.replace("\\", "\\\\");
		tagStr = tagStr.replace("\"", "\\\"");

		String requestObjectString = "{";

		requestObjectString = requestObjectString + "\"customDashboardName\":\"" + customDashboardName + "\", ";
		requestObjectString = requestObjectString + "\"isDefault\":\"" + isDefault + "\", ";
		/*
		 * requestObjectString = requestObjectString + "\"userSID\":\"" +
		 * userSID + "\", ";
		 */
		requestObjectString = requestObjectString + "\"tags\":\"" + tagStr.substring(1, tagStr.length() - 1) + "\" }";

		ObjectMapper mapper = new ObjectMapper();
		try {
			JsonNode requestObject = mapper.readTree(requestObjectString);

			log.info("Request Object: " + requestObjectString);

			return postProductRestResponseCore(productInstanceName, url, requestObject,
					new TypeReference<CustomDashboard>() {
					});
		} catch (JsonProcessingException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}

		return postProductRestResponseCore(productInstanceName, url, null, new TypeReference<CustomDashboard>() {
		});
	}

	public CustomDashboard deleteCustomDashboard(String productInstanceName, Integer customDashboardId)
			throws RestException {
		String url = RestMethods.DELETE_CUSTOMDASHBOARD.getMethodName();

		url = url.replace("{customDashboardid}", customDashboardId.toString());

		return deleteProductRestResponseCore(productInstanceName, url, null, new TypeReference<CustomDashboard>() {
		});
	}

	public Boolean deleteCustomDashboardWidget(String productInstanceName, Integer customDashboardId, Integer widgetId)
			throws RestException {
		String url = RestMethods.DELETE_WIDGET.getMethodName();
		url = url.replace("{customDashboardid}", customDashboardId.toString());
		url = url.replace("{widgetId}", widgetId.toString());

		return deleteProductRestResponseCore(productInstanceName, url, null, new TypeReference<Boolean>() {
		});
	}

	public CustomDashboardWidget createCustomDashboardWidget(String productInstanceName, Integer customDashboardId,
                                                             String widgetName, int widgetTypeId, int metricId, int match, List<Integer> tagId,
                                                             List<Integer> sourceServerIds) throws RestException {
		String url = RestMethods.CREATE_WIDGET.getMethodName();
		url = url.replace("{customDashboardid}", customDashboardId.toString());

		widgetName = widgetName.replace("\\", "\\\\");
		widgetName = widgetName.replace("\"", "\\\"");

		String instances = sourceServerIds.toString();
		String tagIds = tagId.toString();

		String requestObjectString = "{";

		requestObjectString = requestObjectString + "\"widgetName\":\"" + widgetName + "\", ";
		requestObjectString = requestObjectString + "\"widgetTypeId\":\"" + widgetTypeId + "\", ";
		requestObjectString = requestObjectString + "\"match\":\"" + match + "\", ";
		requestObjectString = requestObjectString + "\"tagId\":\"" + tagIds.substring(1, tagIds.length() - 1) + "\", ";
		requestObjectString = requestObjectString + "\"sourceServerIds\":\""
				+ instances.substring(1, instances.length() - 1) + "\", ";
		requestObjectString = requestObjectString + "\"metricId\":\"" + metricId + "\"} ";

		ObjectMapper mapper = new ObjectMapper();
		try {
			JsonNode requestObject = mapper.readTree(requestObjectString);

			log.info("Request Object: " + requestObjectString);

			return postProductRestResponseCore(productInstanceName, url, requestObject,
					new TypeReference<CustomDashboardWidget>() {
					});
		} catch (JsonProcessingException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}

		return postProductRestResponseCore(productInstanceName, url, null, new TypeReference<CustomDashboardWidget>() {
		});
	}

	public CustomDashboardWidget updateCustomdashboardWidget(String productInstanceName, Integer customDashboardId,
			Integer widgetId, String widgetName, int widgetTypeId, int metricId, int match, List<Integer> tagId,
			List<Integer> sourceServerIds) throws RestException {
		String url = RestMethods.UPDATE_WIDGET.getMethodName();
		url = url.replace("{customDashboardid}", customDashboardId.toString());
		url = url.replace("{widgetId}", widgetId.toString());

		widgetName = widgetName.replace("\\", "\\\\");
		widgetName = widgetName.replace("\"", "\\\"");

		String instances = sourceServerIds.toString();
		String tagIds = tagId.toString();

		String requestObjectString = "{";

		requestObjectString = requestObjectString + "\"widgetName\":\"" + widgetName + "\", ";
		requestObjectString = requestObjectString + "\"widgetTypeId\":\"" + widgetTypeId + "\", ";
		requestObjectString = requestObjectString + "\"match\":\"" + match + "\", ";
		requestObjectString = requestObjectString + "\"tagIds\":\"" + tagIds.substring(1, tagIds.length() - 1) + "\", ";
		requestObjectString = requestObjectString + "\"sourceServerIds\":\""
				+ instances.substring(1, instances.length() - 1) + "\", ";
		requestObjectString = requestObjectString + "\"metricId\":\"" + metricId + "\"} ";

		ObjectMapper mapper = new ObjectMapper();
		try {
			JsonNode requestObject = mapper.readTree(requestObjectString);

			log.info("Request Object: " + requestObjectString);

			return postProductRestResponseCore(productInstanceName, url, requestObject,
					new TypeReference<CustomDashboardWidget>() {
					});
		} catch (JsonProcessingException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}

		return postProductRestResponseCore(productInstanceName, url, null, new TypeReference<CustomDashboardWidget>() {
		});
	}

	public List<CustomDashboardWidget> getCustomDashboardWidgets(String productInstanceName, Integer customDashboardId)
			throws RestException {
		String url = RestMethods.GET_CUSTOMDASHBOARD_WIDGETS.getMethodName();
		url = url.replace("{customDashboardid}", customDashboardId.toString());

		return getProductRestResponseCore(productInstanceName, url,
				new TypeReference<ArrayList<CustomDashboardWidget>>() {
				});
	}

	public List<Types> getAllMatchTypes(String productInstanceName) throws RestException {
		String url = RestMethods.GET_MATCH_TYPES.getMethodName();

		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<Types>>() {
		});
	}

	public List<Types> getAllWidgetTypes(String productInstanceName) throws RestException {
		String url = RestMethods.GET_WIDGET_TYPES.getMethodName();

		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<Types>>() {
		});
	}

	public List<CustomDashboardWidgetData> getWidgetData(String productInstanceName, Integer customDashboardId,
			Integer widgetId, Date startTime, Date endTime, String tzo) throws RestException {

		String url = RestMethods.GET_WIDGET_DATA.getMethodName();
		url = url.replace("{customDashboardid}", customDashboardId.toString());
		url = url.replace("{widgetId}", widgetId.toString());
		url = url.replace("{tzo}", tzo);
		url = url + "?start=" + Utility.getUtcDateString(startTime);
		url = url + "&end=" + Utility.getUtcDateString(endTime);

		return getProductRestResponseCore(productInstanceName, url,
				new TypeReference<ArrayList<CustomDashboardWidgetData>>() {
				});
	}

	public List<CustomDashboard> getAllCustomDashboards(String productInstanceName, String userSID)
			throws RestException {
		String url = RestMethods.GET_CUSTOMDASHBOARDS.getMethodName();

		String requestObjectString = "{}";

		/*
		 * requestObjectString = requestObjectString + "\"userSID\":\"" +
		 * userSID + "\"} ";
		 */

		ObjectMapper mapper = new ObjectMapper();
		try {
			JsonNode requestObject = mapper.readTree(requestObjectString);

			log.info("Request Object: " + requestObjectString);

			return postProductRestResponseCore(productInstanceName, url, requestObject,
					new TypeReference<List<CustomDashboard>>() {
					});
		} catch (JsonProcessingException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}

		return postProductRestResponseCore(productInstanceName, url, null, new TypeReference<List<CustomDashboard>>() {
		});

	}

	public User authenticateUser(String principal, String credential, Credentials c) throws RestException {
		return new User(principal, credential);
	}

	public List<AvailabilityGroupStatistics> getAvailabilityGroupStatistics(String productInstanceName,
                                                                            Integer instanceId, int numHistoryMinutes/* , String offSet */) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DATABASE_AVAILABILITY_GRP_STATS.getMethodName().replace("{timeZoneOffset}",
				offSet);
		url = url.replace("{id}", instanceId.toString());
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url,
				new TypeReference<ArrayList<AvailabilityGroupStatistics>>() {
				});
	}
	
	/*
	 * Author:Accolite
	 * Date : 12th Dec, 2016
	 * History Panel - SQLDM- 10.2 release
	 * Adding History Panel to Database Tab -> Availability Group
	 */
	public List<AvailabilityGroupStatistics> getAvailabilityGroupStatistics(String productInstanceName,
			Integer instanceId, long numHistoryMinutes, Date endTime) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DATABASE_AVAILABILITY_GRP_STATS.getMethodName().replace("{timeZoneOffset}",
				offSet);
		url = url.replace("{id}", instanceId.toString());
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String toDate = Utility.getUtcDateString(endTime);
		url = url + "&endDate=" + toDate;
		return getProductRestResponseCore(productInstanceName, url,
				new TypeReference<ArrayList<AvailabilityGroupStatistics>>() {
				});
	}

	public List<ResourceCategory> getResources(String productInstanceName, Integer instanceId,
                                               int numHistoryMinutes /* , String offSet */) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_CATEGORY_RESOURCES.getMethodName().replace("{timeZoneOffset}", offSet);
		// String url =
		// RestMethods.GET_CATEGORY_RESOURCES.getMethodName().replace("{timeZoneOffset}",
		// "0.0");
		url = url.replace("{id}", instanceId.toString());
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<ResourceCategory>>() {
		});
	}
	
	/*
	 * ChaitanyaTanwar DM 10.2
	 * */
	public List<ResourceCategory> getResources(String productInstanceName,
			Integer instanceId, long numHistoryMinutes, Date endTime) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_CATEGORY_RESOURCES.getMethodName().replace("{timeZoneOffset}", offSet);
		url = url.replace("{id}", instanceId.toString());
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String toDate = Utility.getUtcDateString(endTime);
		url = url + "&endDate=" + toDate;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<ResourceCategory>>() {
		});
	}

	public List<GraphBaseline> getBaseline(String productInstanceName, Integer instanceId, int metricId,
                                           int numHistoryMinutes /* , String offSet */) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_BASELINE.getMethodName().replace("{timeZoneOffset}", offSet);
		// String url =
		// RestMethods.GET_BASELINE.getMethodName().replace("{timeZoneOffset}",
		// "0.0");
		url = url.replace("{INSTANCEID}", instanceId.toString());
		url = url.replace("{METRICID}", "" + metricId);
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<GraphBaseline>>() {
		});
	}
	
	/*
	 * ChaitanyaTanwar DM 10.2
	 * */
	public List<GraphBaseline> getBaseline(String productInstanceName, Integer instanceId, int metricId,
			long numHistoryMinutes, Date endTime) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_BASELINE.getMethodName().replace("{timeZoneOffset}", offSet);
		url = url.replace("{INSTANCEID}", instanceId.toString());
		url = url.replace("{METRICID}", "" + metricId);
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String toDate = Utility.getUtcDateString(endTime);
		url = url + "&endDate=" + toDate;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<GraphBaseline>>() {
		});
	}

	public List<FileDrive> getFileDrives(String productInstanceName, Integer instanceId) throws RestException {
		String url = RestMethods.GET_FILEDRIVES.getMethodName();
		url = url.replace("{id}", instanceId.toString());
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<FileDrive>>() {
		});
	}

	public List<FileActivity> getFileActivity(String productInstanceName, Integer instanceId,
                                              int numHistoryMinutes /* , String offSet */) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_FILEACTIVITY.getMethodName().replace("{timeZoneOffset}", offSet);
		// String url =
		// RestMethods.GET_FILEACTIVITY.getMethodName().replace("{timeZoneOffset}",
		// "0.0");
		url = url.replace("{id}", instanceId.toString());
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<FileActivity>>() {
		});
	}

	public List<ServerWait> getServerWaits(String productInstanceName, Integer instanceId,
                                           int numHistoryMinutes /* , String offSet */) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_SERVER_WAITS.getMethodName().replace("{timeZoneOffset}", offSet);
		// String url =
		// RestMethods.GET_SERVER_WAITS.getMethodName().replace("{timeZoneOffset}",
		// "0.0");
		url = url.replace("{id}", instanceId.toString());
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<ServerWait>>() {
		});
	}
	
	/*
	 * ChaitanyaTanwar DM 10.2
	 * */
	public List<ServerWait> getServerWaits(String productInstanceName, Integer instanceId,
			long numHistoryMinutes, Date endTime) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_SERVER_WAITS.getMethodName().replace("{timeZoneOffset}", offSet);
		url = url.replace("{id}", instanceId.toString());
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String toDate = Utility.getUtcDateString(endTime);
		url = url + "&endDate=" + toDate;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<ServerWait>>() {
		});
	}

	public List<DatabaseCapacityUsageDetails> getDatabaseCapacityUsage(String productInstanceName, int instanceId,
                                                                       int dbId /* , String offSet */) throws RestException {
		String offSet = setOffSet();
		return getProductRestResponseCore(productInstanceName,
				RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
						+ RestMethods.GET_DATABASES_FOR_SINGLE_INSTANCE_VIEW.getMethodName() + "/" + dbId
						+ RestMethods.GET_DATABASES_CAPACITY_USAGE.getMethodName() + "/tzo/" + offSet,
				new TypeReference<ArrayList<DatabaseCapacityUsageDetails>>() {
				});
	}

	public List<TempDBUsageDetails> getTempDBUsageDetails(String productInstanceName, int instanceId,
			int numHistoryMinutes /* , String offSet */) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_DATABASES_FOR_SINGLE_INSTANCE_VIEW.getMethodName()
				+ RestMethods.GET_DATABASES_TEMPDB_USAGE.getMethodName() + "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<TempDBUsageDetails>>() {
		});
	}
	
	/*
	 * Author:Accolite
	 * Date : 12th Dec, 2016
	 * History Panel - SQLDM- 10.2 release
	 * Adding History Panel to Database
	 */
	public List<TempDBUsageDetails> getTempDBUsageDetails(String productInstanceName, int instanceId,
			long numHistoryMinutes, Date endTime) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_DATABASES_FOR_SINGLE_INSTANCE_VIEW.getMethodName()
				+ RestMethods.GET_DATABASES_TEMPDB_USAGE.getMethodName() + "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String toDate = Utility.getUtcDateString(endTime);
		url = url + "&endDate=" + toDate;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<TempDBUsageDetails>>() {
		});
	}

	public List<SessionGraphDetail> getInstanceDetailsSessionGraph(String productInstanceName, int instanceId,
                                                                   int numHistoryMinutes /* , String offSet */) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_SESSIONS_FOR_SINGLE_INSTANCE_GRAPH_VIEW.getMethodName() + "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<SessionGraphDetail>>() {
		});
	}
	
	/*
	 * ChaitanyaTanwar DM 10.2 - History Requirement
	 * */
	public List<SessionGraphDetail> getInstanceDetailsSessionGraph(String productInstanceName, int instanceId,
			long numHistoryMinutes, Date endTime) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_SESSIONS_FOR_SINGLE_INSTANCE_GRAPH_VIEW.getMethodName() + "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String toDate = Utility.getUtcDateString(endTime);
		url = url + "&endDate=" + toDate;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<SessionGraphDetail>>() {
		});
	}

	public List<QueryWaits> getQueryWaitsInstance(String productInstanceName, int instanceId, String offset,
                                                  String fromDateTime, String endDateTime, int waitTypeId, int waitCategoryId, int sqlStatementId,
                                                  int applicationId, int databaseId, int hostId, int sessionId, int loginId) throws RestException {
		String url = RestMethods.GET_QUERY_INSTANCE_WAITS.getMethodName();
		url = url.replace("{INSTANCEID}", new Integer(instanceId).toString());
		url = url.replace("{TIMEZONEOFFSET}", offset);
		url = url + "?startTime=" + fromDateTime;
		url = url + "&endTime=" + endDateTime;

		if (waitTypeId != -1)
			url = url + "&WaitTypeID=" + waitTypeId;
		if (waitCategoryId != -1)
			url = url + "&WaitCategoryID=" + waitCategoryId;
		if (sqlStatementId != -1)
			url = url + "&SQLStatementID=" + sqlStatementId;
		if (applicationId != -1)
			url = url + "&ApplicationID=" + applicationId;
		if (databaseId != -1)
			url = url + "&DatabaseID=" + databaseId;
		if (hostId != -1)
			url = url + "&HostID=" + hostId;
		if (sessionId != -1)
			url = url + "&SessionID=" + sessionId;
		if (loginId != -1)
			url = url + "&LoginID=" + loginId;

		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<QueryWaits>>() {
		});
	}

	// Handling All Case
	public List<DashboardAlertsByInstanceWidget> getAllNumAlertsByInstance() {
		List<DashboardAlertsByInstanceWidget> allNumAlerts = new ArrayList<DashboardAlertsByInstanceWidget>();

		try {
			for (Product product : CoreRestClient.getInstance().getAllProducts()) {
				if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("sqldm")) {

					List<DashboardAlertsByInstanceWidget> listForSingleProduct = new ArrayList<DashboardAlertsByInstanceWidget>();
					listForSingleProduct = getProductRestResponseCore(product.getInstanceName(),
							RestMethods.GET_MOST_ALERT_INSTANCES.getMethodName(),
							new TypeReference<ArrayList<DashboardAlertsByInstanceWidget>>() {
							});
					for (DashboardAlertsByInstanceWidget dashboardAlertsByInstanceWidget : listForSingleProduct) {
						dashboardAlertsByInstanceWidget.setProduct(product);
					}
					allNumAlerts.addAll(listForSingleProduct);
				}

			}
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		return allNumAlerts;

	}

	public List<DashboardAlertsByInstanceWidget> getNumAlertsByInstance(String productInstanceName)
			throws RestException {

		List<DashboardAlertsByInstanceWidget> list = new ArrayList<DashboardAlertsByInstanceWidget>();
		Product product = getProductByInstanceName(productInstanceName);

		if (productInstanceName.equals("All"))
			return getAllNumAlertsByInstance();
		list = getProductRestResponseCore(productInstanceName, RestMethods.GET_MOST_ALERT_INSTANCES.getMethodName(),
				new TypeReference<ArrayList<DashboardAlertsByInstanceWidget>>() {
				});
		for (DashboardAlertsByInstanceWidget dashboardAlertsByInstanceWidget : list) {
			dashboardAlertsByInstanceWidget.setProduct(product);
		}
		return list;
	}

	public List<QueryApplication> getQueryApplications(String productInstanceName, int instanceId, int startLimit,
                                                       int noOfRecords) throws RestException {
		String url = RestMethods.GET_QUERY_APPLICATIONS.getMethodName();
		url = url.replace("{id}", new Integer(instanceId).toString());
		if (startLimit != -1)
			url = url + "?startlimit=" + startLimit;
		if (noOfRecords != -1)
			url = url + "&noofrecords=" + noOfRecords;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<QueryApplication>>() {
		});

	}

	public List<QueryDatabases> getQueryDatabases(String productInstanceName, int instanceId, int startLimit,
                                                  int noOfRecords) throws RestException {

		String url = RestMethods.GET_QUERY_DATABASES.getMethodName();
		url = url.replace("{id}", new Integer(instanceId).toString());
		if (startLimit != -1)
			url = url + "?startlimit=" + startLimit;
		if (noOfRecords != -1)
			url = url + "&noofrecords=" + noOfRecords;

		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<QueryDatabases>>() {
		});
	}

	public List<QueryClients> getQueryClients(String productInstanceName, int instanceId, int startLimit,
                                              int noOfRecords) throws RestException {

		String url = RestMethods.GET_QUERY_CLIENTS.getMethodName();
		url = url.replace("{id}", new Integer(instanceId).toString());
		if (startLimit != -1)
			url = url + "?startlimit=" + startLimit;
		if (noOfRecords != -1)
			url = url + "&noofrecords=" + noOfRecords;

		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<QueryClients>>() {
		});
	}

	public List<QueryUsers> getQueryUsers(String productInstanceName, int instanceId, int startLimit, int noOfRecords)
			throws RestException {

		String url = RestMethods.GET_QUERY_USERS.getMethodName();
		url = url.replace("{id}", new Integer(instanceId).toString());
		if (startLimit != -1)
			url = url + "?startlimit=" + startLimit;
		if (noOfRecords != -1)
			url = url + "&noofrecords=" + noOfRecords;

		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<QueryUsers>>() {
		});
	}

	public List<ViewMetrics> getQueryViewMetrics(String productInstanceName) throws RestException {

		String url = RestMethods.GET_QUERY_VIEW_METRICS.getMethodName();

		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<ViewMetrics>>() {
		});
	}

	public List<QueryApplicationDetails> getList(String productInstanceName, int instanceID, int viewID, int groupID,
                                                 double timeZoneOffSet, String applicationIDs, String dbIDs, String userIDs, String clientIDs,
                                                 String sqlIncludeText, String sqlExculdeText, String advancedFilters, String startTime, String endTime,
                                                 int startIndex, int rowCount, String sortBy, String sortOrder, int sqlSignatureID, int eventId)
					throws RestException {

		String url = RestMethods.GET_QUERY_LIST.getMethodName();
		url = url.replace("{instanceId}", new Integer(instanceID).toString());
		url = url.replace("{viewId}", new Integer(viewID).toString());
		url = url.replace("{groupId}", new Integer(groupID).toString());
		url = url.replace("{tzo}", (new Double(timeZoneOffSet)).toString());

		String requestObjectString = "{";
		if (!applicationIDs.isEmpty() && applicationIDs != null)
			requestObjectString = requestObjectString + "\"ApplicationIds\":\"" + applicationIDs + "\", ";

		if (!dbIDs.isEmpty() && dbIDs != null)
			requestObjectString = requestObjectString + "\"DatabaseIds\":\"" + dbIDs + "\", ";

		if (!userIDs.isEmpty() && userIDs != null)
			requestObjectString = requestObjectString + "\"UserIds\":\"" + userIDs + "\", ";

		if (!clientIDs.isEmpty() && clientIDs != null)
			requestObjectString = requestObjectString + "\"ClientIds\":\"" + clientIDs + "\", ";

		if (!sqlIncludeText.isEmpty() && sqlIncludeText != null) {
			requestObjectString = requestObjectString + "\"SQLInclude\":\"" + sqlIncludeText + "\", ";
		}

		if (!sqlExculdeText.isEmpty() && sqlExculdeText != null) {
			requestObjectString = requestObjectString + "\"SQLExclude\":\"" + sqlExculdeText + "\", ";
		}

		if (!advancedFilters.isEmpty() && advancedFilters != null)
			requestObjectString = requestObjectString + "\"AdvancedFilters\":\"" + advancedFilters + "\", ";
		// "{STARTTIMESTAMP}&endTimestamp={ENDTIMESTAMP}&={STARTINDEX}&{NOOFRECORDS}&{SORTBY}{SORTORDER}"

		if (startTime != null)
			requestObjectString = requestObjectString + "\"StartTimestamp\":\"" + startTime + "\", ";

		if (endTime != null)
			requestObjectString = requestObjectString + "\"EndTimestamp\":\"" + endTime + "\", ";

		/*
		 * requestObjectString = requestObjectString + "\"StartIndex\":\"" +
		 * startIndex + "\", "; requestObjectString = requestObjectString +
		 * "\"RowCount\":\"" + rowCount + "\", ";
		 * 
		 * if (sortBy != null && !sortBy.isEmpty()) requestObjectString =
		 * requestObjectString + "\"SortBy\":\"" + sortBy + "\", ";
		 * 
		 * if (sortOrder != null && !sortOrder.isEmpty()) requestObjectString =
		 * requestObjectString + "\"SortOrder\":\"" + sortOrder + "\", ";
		 */
		if (sqlSignatureID > 0) {
			requestObjectString = requestObjectString + "\"QuerySignatureId\":\"" + sqlSignatureID + "\", ";
			requestObjectString = requestObjectString + "\"EventTypeId\":\"" + eventId + "\", ";
		}

		requestObjectString = requestObjectString.substring(0, requestObjectString.length() - 2);
		requestObjectString = requestObjectString + "}";

		ObjectMapper mapper = new ObjectMapper();
		try {
			JsonNode requestObject = mapper.readTree(requestObjectString);

			log.info("Request Object: " + requestObjectString);

			return postProductRestResponseCore(productInstanceName, url, requestObject,
					new TypeReference<ArrayList<QueryApplicationDetails>>() {
					});
		} catch (JsonProcessingException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}

		return postProductRestResponseCore(productInstanceName, url, null,
				new TypeReference<ArrayList<QueryApplicationDetails>>() {
				});
	}

	public List<QueryGroups> getQueryGroups(String productInstanceName) throws RestException {

		String url = RestMethods.GET_QUERY_GROUPS.getMethodName();

		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<QueryGroups>>() {
		});
	}

	public ArrayList<QueryPlan> getQueryPlan(String productInstanceName, int instanceID, int queryID)
			throws RestException {
		String url = RestMethods.GET_QUERY_PLAN.getMethodName();
		url = url.replace("{instanceId}", new Integer(instanceID).toString());
		url = url.replace("{queryId}", new Integer(queryID).toString());

		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<QueryPlan>>() {
		});
	}

	public List<QueryGraph> getQueryGraphBars(String productInstanceName, int instanceID, int viewID, int groupID,
			double timeZoneOffSet, String applicationIDs, String dbIDs, String userIDs, String clientIDs,
			String sqlIncludeText, String sqlExculdeText, String advancedFilters, String startTime, String endTime,
			int querySignatureID, int eventId) throws RestException {

		String url = RestMethods.GET_QUERY_GRAPH.getMethodName();

		url = url.replace("{instanceId}", (new Integer(instanceID)).toString());
		url = url.replace("{viewId}", (new Integer(viewID)).toString());
		url = url.replace("{groupId}", (new Integer(groupID)).toString());
		url = url.replace("{tzo}", (new Double(timeZoneOffSet)).toString());

		String requestObjectString = "{";
		if (!applicationIDs.isEmpty() && applicationIDs != null)
			requestObjectString = requestObjectString + "\"ApplicationIds\":\"" + applicationIDs + "\", ";

		if (!dbIDs.isEmpty() && dbIDs != null)
			requestObjectString = requestObjectString + "\"DatabaseIds\":\"" + dbIDs + "\", ";

		if (!userIDs.isEmpty() && userIDs != null)
			requestObjectString = requestObjectString + "\"UserIds\":\"" + userIDs + "\", ";

		if (!clientIDs.isEmpty() && clientIDs != null)
			requestObjectString = requestObjectString + "\"ClientIds\":\"" + clientIDs + "\", ";

		if (!sqlIncludeText.isEmpty() && sqlIncludeText != null) {
			requestObjectString = requestObjectString + "\"SQLInclude\":\"" + sqlIncludeText + "\", ";
		}

		if (!sqlExculdeText.isEmpty() && sqlExculdeText != null) {
			requestObjectString = requestObjectString + "\"SQLExclude\":\"" + sqlExculdeText + "\", ";
		}

		if (!advancedFilters.isEmpty() && advancedFilters != null)
			requestObjectString = requestObjectString + "\"AdvancedFilters\":\"" + advancedFilters + "\", ";

		if (startTime != null)
			requestObjectString = requestObjectString + "\"StartTimestamp\":\"" + startTime + "\", ";
		else
			System.out.println("startTime should not be null for Graph API");

		if (endTime != null)
			requestObjectString = requestObjectString + "\"EndTimestamp\":\"" + endTime + "\", ";
		else
			System.out.println("endTime should not be null for Graph API");

		if (querySignatureID > 0) {
			requestObjectString = requestObjectString + "\"QuerySignatureId\":\"" + querySignatureID + "\", ";
			requestObjectString = requestObjectString + "\"EventTypeId\":\"" + eventId + "\", ";
		}

		requestObjectString = requestObjectString.substring(0, requestObjectString.length() - 2);
		requestObjectString = requestObjectString + "}";

		ObjectMapper mapper = new ObjectMapper();
		try {
			JsonNode requestObject = mapper.readTree(requestObjectString);

			return postProductRestResponseCore(productInstanceName, url, requestObject,
					new TypeReference<ArrayList<QueryGraph>>() {
					});
		} catch (JsonProcessingException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		return postProductRestResponseCore(productInstanceName, url, null, new TypeReference<ArrayList<QueryGraph>>() {
		});

	}

	private String setOffSet() {
		Double offSet = null;
		if (Sessions.getCurrent() != null) {
			offSet = new Double((Integer) Sessions.getCurrent().getAttribute(WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))
					/ (1000 * 60.0 * 60.0);
			offSet = -offSet;
		}
		if (offSet != null)
			return offSet.toString();
		else
			return "0.0";
	}
	
	
	/*
	 * Author:Accolite
	 * Date : 15th Nov, 2016
	 * History Panel - SQLDM- 10.2 release
	 * Save and Load the user settings from previous session
	 */
	public String saveUserSessionSettings(String productInstanceName,JSONArray settings) throws RestException {
		String url = RestMethods.SAVE_USER_SETTINGS.getMethodName();
		return postProductRestResponseCore(productInstanceName, url, settings, new TypeReference<String>() {});	

	}
	public List<UserSettings> getUserSessionSettings(String productInstanceName) throws RestException {
		String url = RestMethods.GET_USER_SETTINGS.getMethodName();
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<UserSettings>>() {});	

	}
	
	/*
	 * Overview Screen customization changes: SQLDM- 10.2 release
	 */
	public List<CPUStatDetails> getCPUStats(String productInstanceName, int instanceId,
                                            long numHistoryMinutes ) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_CPU_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
	//	return getDummyData();
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<CPUStatDetails>>() {
		});
	}

	public List<CPUStatDetails> getCPUStats(String productInstanceName,
			Integer instanceId, long numHistoryMinutes, Date endTime) throws RestException  {
		/*// TODO replace this code by api call with endtime
		return getCPUStats(productInstanceName,instanceId,numHistoryMinutes);*/
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_CPU_STATS.getMethodName()
				+ "/tzo/" + offSet;
		
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String toDate = Utility.getUtcDateString(endTime);
		url = url + "&endDate=" + toDate;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<CPUStatDetails>>() {
		});
		
	}
	
	public List<OSPagingData> getOSPagingStats(String productInstanceName,
			int instanceId, int numHistoryMinutes) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_OS_PAGING_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<OSPagingData>>() {
		});
	}

	public List<OSPagingData> getOSPagingStats(String productInstanceName,
			int instanceId, long numHistoryMinutes, Date toDateTime) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_OS_PAGING_STATS.getMethodName()
				+ "/tzo/" + offSet;
		
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String toDate = Utility.getUtcDateString(toDateTime);
		url = url + "&endDate=" + toDate;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<OSPagingData>>() {
		});
	}

	public List<LockWaitData> getLockWaitsData(String productInstanceName,
			int instanceId, long numHistoryMinutes) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_LOCK_WAITS_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<LockWaitData>>() {
		});
	}
	
	public List<LockWaitData> getLockWaitsData(String productInstanceName,
			int instanceId, long numHistoryMinutes, Date endDate) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_LOCK_WAITS_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String date = Utility.getUtcDateString(endDate);
		url = url + "&endDate=" + date;
		
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<LockWaitData>>() {
		});
	}

	public List<DatabaseStatsDetails> getDBStatData(String productInstanceName,
                                                    int instanceId, long numHistoryMinutes, Date endDate) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_DB_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String date = Utility.getUtcDateString(endDate);
		url = url + "&endDate=" + date;
		
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<DatabaseStatsDetails>>() {
		});
	}
	
	public List<DatabaseStatsDetails> getDBStatData(String productInstanceName,
			int instanceId, long numHistoryMinutes) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_DB_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<DatabaseStatsDetails>>() {
		});
	}
	
	/*
	 * Author:Accolite
	 * Date : 20th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 */
	public List<CustomCounterStats> getCustomCounterStats(String productInstanceName,
			int instanceId, long numHistoryMinutes) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_CUSTOM_COUNTER_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<CustomCounterStats>>() {
		});
	}
	
	/*
	 * Author:Accolite
	 * Date : 20th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 */
	public List<CustomCounterStats> getCustomCounterStats(String productInstanceName,
			int instanceId, long numHistoryMinutes, Date endDate) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_CUSTOM_COUNTER_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String date = Utility.getUtcDateString(endDate);
		url = url + "&endDate=" + date;
		
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<CustomCounterStats>>() {
		});
	}
	
	/*
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 */
	public List<NetworkStats> getNetworkStats(String productInstanceName,
			int instanceId, long numHistoryMinutes) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_NETWORK_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<NetworkStats>>() {
		});
	}
	
	/*
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 */
	public List<NetworkStats> getNetworkStats(String productInstanceName,
			int instanceId, long numHistoryMinutes, Date endDate) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_NETWORK_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String date = Utility.getUtcDateString(endDate);
		url = url + "&endDate=" + date;
		
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<NetworkStats>>() {
		});
	}
	
	/*
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 */
	public List<FileStats> getFileStats(String productInstanceName,
			int instanceId, long numHistoryMinutes) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_FILE_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<FileStats>>() {
		});
	}
	
	/*
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 */
	public List<FileStats> getFileStats(String productInstanceName,
			int instanceId, long numHistoryMinutes, Date endDate) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_FILE_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String date = Utility.getUtcDateString(endDate);
		url = url + "&endDate=" + date;
		
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<FileStats>>() {
		});
	}
	/**
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 * @param productInstanceName
	 * @param instanceId
	 * @param numHistoryMinutes
	 * @return
	 * @throws RestException
	 */
	
	public VirtualizationStatsDetails getVirtualizationStatDetails(String productInstanceName,
                                                                   int instanceId, long numHistoryMinutes) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_VIRTUALIZATION_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<VirtualizationStatsDetails>() {
		});
	}
	
	/**
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 * @param productInstanceName
	 * @param instanceId
	 * @param numHistoryMinutes
	 * @param endDate
	 * @return
	 * @throws RestException
	 */
	
	public VirtualizationStatsDetails getVirtualizationStatDetails(String productInstanceName,
			int instanceId, long numHistoryMinutes, Date endDate) throws RestException {
		String offSet = setOffSet();
		String url = RestMethods.GET_DASHBOARD_INSTANCES.getMethodName() + "/" + instanceId
				+ RestMethods.GET_VIRTUALIZATION_STATS.getMethodName()
				+ "/tzo/" + offSet;
		url = url + "?NumHistoryMinutes=" + numHistoryMinutes;
		String date = Utility.getUtcDateString(endDate);
		url = url + "&endDate=" + date;
		
		return getProductRestResponseCore(productInstanceName, url, new TypeReference<VirtualizationStatsDetails>() {
		});
	}
	
	public List<QueryWaits> getOverviewQueryWaitsInstance(String productInstanceName, int instanceId, String offset,
                                                          String fromDateTime, String endDateTime) throws RestException {
		String url = RestMethods.GET_OVERVIEW_QUERY_INSTANCE_WAITS.getMethodName();
		url = url.replace("{INSTANCEID}", new Integer(instanceId).toString());
		url = url.replace("{TIMEZONEOFFSET}", offset);
		url = url + "?startTime=" + fromDateTime;
		url = url + "&endTime=" + endDateTime;

		return getProductRestResponseCore(productInstanceName, url, new TypeReference<ArrayList<QueryWaits>>() {
		});
	}

}
