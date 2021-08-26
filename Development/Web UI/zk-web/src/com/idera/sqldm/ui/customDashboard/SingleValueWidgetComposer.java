package com.idera.sqldm.ui.customDashboard;

import java.util.HashMap;
import java.util.Map;

import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;

import com.idera.sqldm.data.customdashboard.CustomDashboardWidgetData;
import com.idera.sqldm.ui.customDashboard.widgets.CustomDashboardWidget;
import com.idera.sqldm.ui.customDashboard.widgets.CustomDashboardWidgetSizeEnum;
import com.idera.sqldm.ui.customDashboard.widgets.composer.CustomdDashboardBaseWidgetComposer;

public class SingleValueWidgetComposer extends CustomdDashboardBaseWidgetComposer {
	private static final long serialVersionUID = 1L;
	@Wire private Div editSVWidgetDiv,displaySVWidgetDiv,svDataDiv,svErrorDiv;
	@Wire private Label svInstanceName,svInstanceValue,svUnit,svWidgetNameLbl;
	@Wire private Button svdetailsBtn,svRemoveBtn,svEditbtn;
	

	public void doAfterCompose(CustomDashboardWidget comp) throws Exception {
		super.doAfterCompose(comp);
		setWidgetConfig(comp);
		svWidgetNameLbl.setValue((String) Executions.getCurrent().getArg().get("widgetName"));
		svWidgetNameLbl.setTooltiptext((String) Executions.getCurrent().getArg().get("widgetName"));
		onDisplayCustomDashBoard();
		svDataDiv.setVisible(false);
		svErrorDiv.setVisible(false);
	}

	@Override
	protected void setWidgetData() {
		CustomDashboardWidgetData widgetData = null;
		if(metricValuesforInstance != null && metricValuesforInstance.size() > 0)
			widgetData = metricValuesforInstance.get(0);
		if(widgetData != null && widgetData.getmetricValuesforInstance() != null
				&& widgetData.getmetricValuesforInstance().size() > 0) {
//			svInstanceName.setValue(widgetData.getInstanceName()); @author Saumyadeep
			svInstanceName.setValue(widgetData.getDisplayName());
			svInstanceValue.setValue(
							widgetData.getmetricValuesforInstance().get(0).getMetricValue()+"");
		} else {
			setEmptyMessage();
			return;
		}
		svDataDiv.setVisible(true);
		svErrorDiv.setVisible(false);
	}
	
	@Listen("onClick = #svdetailsBtn")
	public void displayDetails() {
		super.displayDetails();
	}
	
	@Listen("onClick = #svRemoveBtn")
	public void removeWidget() {
		super.removeWidget();
	}
	
	@Listen("onClick = #svEditbtn")
	public void editWidget() {
		super.editWidget();
	}

	@Override
	protected void onEditCustomDashBoard() {
		editSVWidgetDiv.setVisible(true);
		displaySVWidgetDiv.setVisible(false);
	}

	@Override
	protected void onDisplayCustomDashBoard() {
		editSVWidgetDiv.setVisible(false);
		displaySVWidgetDiv.setVisible(true);
	}
	
	@Override
	protected void setWidgetConfig(CustomDashboardWidget comp) {
		Map<String,String> widgetConfig = new HashMap<>();
		widgetConfig.put("title", (String) Executions.getCurrent().getArg().get("widgetName"));
		widgetConfig.put("width", CustomDashboardWidgetSizeEnum.small.getWidth());
		widgetConfig.put("height", CustomDashboardWidgetSizeEnum.small.getHeight());
		comp.setWidgetConfig(widgetConfig);
	}

	@Override
	protected void setEmptyMessage() {
		svDataDiv.setVisible(false);
		svErrorDiv.setVisible(true);
	}

}
