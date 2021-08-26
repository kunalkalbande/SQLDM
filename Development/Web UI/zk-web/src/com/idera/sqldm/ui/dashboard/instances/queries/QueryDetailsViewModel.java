package com.idera.sqldm.ui.dashboard.instances.queries;

import java.util.List;

import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Execution;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Tab;
import org.zkoss.zul.Tabbox;
import org.zkoss.zul.Window;

import com.idera.sqldm.data.queries.QueryStatementColumns;

public class QueryDetailsViewModel extends SelectorComposer<Window> {

	private static final Logger log = Logger
			.getLogger(QueryDetailsViewModel.class);

	private static final long serialVersionUID = 1L;
	@Wire
	private Window queryDetailsWindow;
	@Wire
	private Tabbox queryDetailsTabbox;

	@Wire
	private Tab planDiagramTab;
	@Wire
	private Tab planXmlTab;
	@Wire
	private Tab sqlTextTab;
	@Wire
	private Tab queryColumnsTab;

	boolean visibilityFlag;
	int defaultTab;
	Boolean isActual = null;
	
	private String tabHeading = "";

	@SuppressWarnings("unchecked")
	@Override
	public void doAfterCompose(Window comp) throws Exception {

		super.doAfterCompose(comp);

		Selectors.wireEventListeners(comp, this);

		Execution execution = Executions.getCurrent();

		Sessions.getCurrent().setAttribute("SqlText",
				(String) execution.getArg().get("SqlText"));

		visibilityFlag = Boolean.parseBoolean((String) execution.getArg().get(
				"VisibilityFlag"));
		defaultTab = Integer.parseInt((String) execution.getArg().get(
				"DefaultTab"));
		if(execution.getArg().get("IsActualFlag") != null)
			isActual = Boolean.parseBoolean(execution.getArg().get(
					"IsActualFlag").toString());

		if (visibilityFlag == false) {

			planDiagramTab.close();
			planXmlTab.close();
			queryColumnsTab.close();

			defaultTab = 2;

		} else {

			if(isActual != null) {
				if(isActual)
					tabHeading = " (Actual)";
				else
					tabHeading = " (Estimated)";
			}
			
			planDiagramTab.setLabel(planDiagramTab.getLabel() + tabHeading);
			planXmlTab.setLabel(planXmlTab.getLabel() + tabHeading);
			Sessions.getCurrent().setAttribute("XmlPlanString",
					(String) execution.getArg().get("XmlPlanString"));
			Sessions.getCurrent().setAttribute(
					"QueryStatementColumns",
					(List<QueryStatementColumns>) execution.getArg().get(
							"QueryStatementColumns"));

		}

		switch (defaultTab) {
		case 0:
			queryDetailsTabbox.setSelectedTab(planDiagramTab);
			Events.sendEvent("onSelect", planDiagramTab, null);
			break;

		case 1:
			queryDetailsTabbox.setSelectedTab(planXmlTab);
			Events.sendEvent("onSelect", planXmlTab, null);
			break;

		case 2:
			queryDetailsTabbox.setSelectedTab(sqlTextTab);
			Events.sendEvent("onSelect", sqlTextTab, null);
			break;

		case 3:
			queryDetailsTabbox.setSelectedTab(queryColumnsTab);
			Events.sendEvent("onSelect", queryColumnsTab, null);
			break;
		}

	}

	@Listen("onClick = #closeBtn, #closeLink")
	public void closeWindow() {
		queryDetailsWindow.detach();
	}

	@Listen("onClick = #zoomInLink")
	public void zoomIn() {

	}

	@Listen("onClick = #zoomOutLink")
	public void zoomOut() {

	}

	@Listen("onClick = #zoomToFitLink")
	public void zoomToFit() {

	}
}
