package com.idera.sqldm_10_3.ui.widgetComposers;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.common.dashboard.Composers.DashboardBaseWidgetComposer;
import com.idera.common.rest.CoreRestClient;
import com.idera.core.facade.DashboardWidgetFacade;
import com.idera.cwf.model.Product;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqldm.d3zk.chart.PieChart;
import com.idera.sqldm_10_3.data.SeverityCodeToStringEnum;
import com.idera.sqldm_10_3.i18n.SQLdmI18NStrings;
import com.idera.sqldm_10_3.server.web.ELFunctions;
import com.idera.sqldm_10_3.server.web.WebUtil;
import com.idera.sqldm_10_3.ui.widgetModels.InstanceStatus;
import com.idera.sqldm_10_3.ui.widgetModels.InstanceStatusColor;
import com.idera.sqldm_10_3.ui.widgetModels.InstanceStatus;
import com.idera.sqldm_10_3.ui.widgetModels.InstanceStatusColor;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.*;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

public class InstanceStatusComposer extends DashboardBaseWidgetComposer {

	@Wire
	private Hbox mainContainer;

	@Wire
	private Hbox errorContainer;

	@Wire(".instance-status-pie-chart")
	protected PieChart pieChart;
	
	@Wire
	private Label mc_errorLabel;

	private PieModel piemodel = new SimplePieModel();

	private static final long serialVersionUID = 1L;
	private InstanceStatus status;
	private AnnotateDataBinder binder;

	// The colors in the chart are generated pseudo-randomly. The colors in the
	// string array below are assigned based on the descending
	// of instance count of each status. So this list maps each color to
	// particular status type to show in the legend.
	private List<InstanceStatusColor> list;
	private Map<String, String> colorList;
	private final String[] colors = { "#9467bd", "#d62728", "#2ca02c",
			"#ff7f0e", "#1f77b4" };
	private ListModelList<Product> productsModel;
	@Wire
	private Combobox productListCombobox;

	@Wire
	private Div totalInstancesDiv;

	@Wire(".instance-status-div1")
	private Div dmDiv1;
	@Wire(".instance-status-div2")
	private Div dmDiv2;
	@Wire(".instance-status-div3")
	private Div dmDiv3;
	@Wire(".instance-status-div4")
	private Div dmDiv4;
	@Wire(".instance-status-div5")
	private Div dmDiv5;

	private Product currentProduct;
	private int appendFlag = 0;
	private List<Product> productsList;

	@Override
	public TypeReference<?> getModelType() {
		return new TypeReference<InstanceStatus>() {
		};
	}

	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
		binder = new AnnotateDataBinder(getSelf());
		mainContainer.setVisible(false);

		// Hack to re render the charts on Maximize.
		// Zkoss raise onMaximize event on clicking Maximize button(even though
		// you toggle it).
		comp.addEventListener("onMaximize", new EventListener<Event>() {
			@Override
			public void onEvent(Event event) {
				loadWidgetData();
			}
		});

		productsList = CoreRestClient.getInstance().getProducts();
		productsModel = new ListModelList<>();
		String productName = config.getProduct()
				.getProductNameWithoutInstanceName();

		for (Product product : productsList) {
			if (product.getProductNameWithoutInstanceName().equals(productName))
				productsModel.add(product);
		}

		if (productsModel.size() > 1) {
			Product all = new Product();
			all.setName("All");
			all.setInstanceName("firstinstance");
			productsModel.add(0, all);
			productListCombobox.setModel(productsModel);
		} else {
			productListCombobox.setVisible(false);
		}
	}

	@Override
	public void setWidgetData(Object obj) {
		if (obj == null) {
			binder.bindBean("message",
					"Error while fetching data. Please check log for more details.");
			mainContainer.setVisible(false);
			errorContainer.setVisible(true);
			binder.loadAll();
			return;
		}
		try {
			errorContainer.setVisible(false);
			mainContainer.setVisible(true);
			mainContainer.invalidate();

			if (productListCombobox.getSelectedIndex() == 0) {
				if (appendFlag == 0) {
					status = (InstanceStatus) obj;
					appendFlag = 1;
				} else {
					InstanceStatus newStatus = (InstanceStatus) obj;

					int totalCriticalInstances = status.getAlertStatus()
							.getTotalCriticalInstances()
							+ newStatus.getAlertStatus()
									.getTotalCriticalInstances();
					int totalWarningInstances = status.getAlertStatus()
							.getTotalWarningInstances()
							+ newStatus.getAlertStatus()
									.getTotalWarningInstances();
					int totalInformationalInstances = status.getAlertStatus()
							.getTotalInformationalInstances()
							+ newStatus.getAlertStatus()
									.getTotalInformationalInstances();
					int totalOkInstances = status.getAlertStatus()
							.getTotalOkInstances()
							+ newStatus.getAlertStatus().getTotalOkInstances();
					int totalmonitoredInstances = status.getOverview()
							.getTotalMonitoredInstances()
							+ newStatus.getOverview()
									.getTotalMonitoredInstances();
					int totalDisabledInstances = status.getOverview()
							.getTotalDisabledInstances()
							+ newStatus.getOverview()
									.getTotalDisabledInstances();

					status.getAlertStatus().setTotalCriticalInstances(
							totalCriticalInstances);
					status.getAlertStatus().setTotalWarningInstances(
							totalWarningInstances);
					status.getAlertStatus().setTotalInformationalInstances(
							totalInformationalInstances);
					status.getAlertStatus().setTotalOkInstances(
							totalOkInstances);
					status.getOverview().setTotalMonitoredInstances(
							totalmonitoredInstances);
					status.getOverview().setTotalDisabledInstances(
							totalDisabledInstances);
				}
			} else
				status = (InstanceStatus) obj;

			binder.bindBean("status", status);

			if(status.getTotalMonitoredInstances() > 0) {
				drawChart();
				pieChart.setVisible(true);
				mc_errorLabel.getParent().setVisible(false);
			}	else {
				pieChart.setVisible(false);
				mc_errorLabel.setValue(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
				mc_errorLabel.getParent().setVisible(true);
			}
			
			dmDiv1.setStyle("display:inline-block;width:15px;height:10px;background-color:"
					+ colorList.get("Critical"));
			dmDiv2.setStyle("display:inline-block;width:15px;height:10px;background-color:"
					+ colorList.get("Warning"));
			dmDiv3.setStyle("display:inline-block;width:15px;height:10px;background-color:"
					+ colorList.get("Informational"));
			dmDiv4.setStyle("display:inline-block;width:15px;height:10px;background-color:"
					+ colorList.get("Ok"));
			dmDiv5.setStyle("display:inline-block;width:15px;height:10px;background-color:"
					+ colorList.get("Disabled"));

			binder.loadAll();
		} catch (Exception e) {
		}
	}

	@Override
	public String getEventName() {
		return String.format("%s:%s", config.getId(),
				InstanceStatusComposer.class.getName());
	}

	@Listen("onSelect=#productListCombobox")
	public void onSelect(Event event) {

		appendFlag = 0;
		if (productListCombobox.getSelectedIndex() == 0) {
			crossRepoList();
		} else {
			loadWidgetData(); // Trigger Data load in background thread.
		}
	}

	@Listen("onAfterRender=#productListCombobox")
	public void afterRender(Event event) {

		int index = 0;
		for (int i = productsModel.size() - 1; i >= 0; i--) {
			if (productsModel.get(i).getName()
					.equals(config.getProduct().getName())) {
				index = i;
				break;
			}
		}
		productListCombobox.setSelectedIndex(index);

	}

	@Override
	public String getDataURI() {
		if (productsModel != null && productsModel.size() > 1) {
			int selectedIndex = productListCombobox.getSelectedIndex();
			if (selectedIndex == 0) {
				return String.format("%s%s", currentProduct.getRestUrl(),
						config.getDataURI());
			}

			return String.format("%s%s", productsModel.get(selectedIndex)
					.getRestUrl(), config.getDataURI());
		}

		return String.format("%s%s", config.getProduct().getRestUrl(),
				config.getDataURI());
	}

	protected void drawChart() {
		try {
			// Clear the chart data
			loadChartData();
			pieChart.invalidate();
			pieChart.setModel(piemodel);
			pieChart.setHeight("150px");
			pieChart.setAnimationDelay(0);
			pieChart.setInnerRadiusPercentage(0);
			pieChart.setOuterRadiusPercentage(75);
			pieChart.setColorCodes(colorList);
		} catch (Exception x) {
		}
	}

	private void loadChartData() {
		piemodel.setValue("Critical",
				(double) status.getTotalCriticalInstances());
		piemodel.setValue("Warning", (double) status.getTotalWarningInstances());
		piemodel.setValue("Informational",
				(double) status.getTotalInformationalInstances());
		piemodel.setValue("Ok", (double) status.getTotalOkInstances());
		piemodel.setValue("Disabled",
				(double) status.getTotalDisabledInstances());

		list = new LinkedList<InstanceStatusColor>();
		list.add(new InstanceStatusColor("Critical", status
				.getTotalCriticalInstances()));
		list.add(new InstanceStatusColor("Warning", status
				.getTotalWarningInstances()));
		list.add(new InstanceStatusColor("Informational", status
				.getTotalInformationalInstances()));
		list.add(new InstanceStatusColor("Ok", status.getTotalOkInstances()));
		list.add(new InstanceStatusColor("Disabled", status
				.getTotalDisabledInstances()));

		colorList = new HashMap<String, String>();

		colorList.put("Critical", SeverityCodeToStringEnum.CRITICAL.getColorCode());
		colorList.put("Warning", SeverityCodeToStringEnum.WARNING.getColorCode());
		colorList.put("Informational", SeverityCodeToStringEnum.INFORMATIONAL.getColorCode());
		colorList.put("Ok", SeverityCodeToStringEnum.OK.getColorCode());
		colorList.put("Disabled", "#B7B7B7");		
		
/*		Collections.sort(list, new Comparator<InstanceStatusColor>() {

			@Override
			public int compare(InstanceStatusColor o1, InstanceStatusColor o2) {
				// TODO Auto-generated method stub
				return o2.getCount() - o1.getCount();
			}
		});
*/
/*		int i = 0;
		for (InstanceStatusColor element : list)
			colorList.put(element.getType(), colors[i++]);
*/
	}

	@Listen("onClick = #totalInstancesDiv")
	public void linkToDashboard(Event event) {
		Product product;
		if (productsModel != null && productsModel.size() > 1) {
			int selectedProductIndex = productListCombobox.getSelectedIndex();

			if (selectedProductIndex == 0) {
				product = config.getProduct();
				Sessions.getCurrent().setAttribute("AllFlag", true);
			} else
				product = productsModel.get(selectedProductIndex);
		} else {
			product = config.getProduct();
		}

		Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(product,
				"home"));
	}

	private void crossRepoList() {

		for (Product product : productsList) {
			currentProduct = product;
			setWidgetData(DashboardWidgetFacade.getWidgetData(getDataURI(),
					getModelType()));
		}

	}

}
