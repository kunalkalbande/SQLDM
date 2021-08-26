package com.idera.sqldm.ui.dashboard.instances.sessions;

import java.util.Comparator;
import java.util.List;
import java.util.Set;
import java.util.TreeSet;

import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.Div;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Paging;

import com.idera.sqldm.data.InstanceDetailsSession;
import com.idera.sqldm.data.instances.SessionConnection;
import com.idera.sqldm.data.instances.SessionLock;
import com.idera.sqldm.server.web.WebConstants;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.GridExporter;
import com.idera.sqldm.utils.SQLdmConstants;


public class SummaryPageComposer extends SelectorComposer<Div> {
	private static final long serialVersionUID = 1L;

	private final Logger log = Logger.getLogger(SummaryPageComposer.class);
	private static final int SESSION_ROWS = 20;
	private static final String NOT_APPLICABLE = "N/A";

	@Wire
	protected Label titleLabel;

	@Wire
	protected Grid sessionSummaryGrid;

	@Wire
	protected Intbox sessionListRowsBox;
	@Wire
	protected Paging sessionsListPgId;

	private int prevPageSize;
	protected AnnotateDataBinder binder = null;

	public ListModelList<InstanceDetailsSession> sessionsModel = new ListModelList<InstanceDetailsSession>();

	@Override
	public void doAfterCompose(Div comp) throws Exception {
		super.doAfterCompose(comp);

		// Subscribe to refresh event.
		EventQueue<Event> q = EventQueues.lookup(
				WebConstants.INSTANCE_PROPERTIES_REFRESH_EVENT_QUEUE,
				EventQueues.APPLICATION, true);
		if (q != null) {
			q.subscribe(new EventListener<Event>() {
				@Override
				public void onEvent(Event arg) throws Exception {
					if (arg.getData() != null)
						return;
					refreshSessionGrid();
				}
			});
		}

		// bind all grid column sort comparators
		binder = new AnnotateDataBinder(comp);
		binder.bindBean("sessionsModel", sessionsModel);
		binder.loadAll();
		int defaultAlertsRowCount = SESSION_ROWS;
		// Todo - Read from session if we have to
		DashboardPreferencesBean dpb = PreferencesUtil.getInstance()
				.getDashboardPreferencesInSession();
		if (dpb != null && dpb.getSessionsGridPageCount() != -1) {
			defaultAlertsRowCount = dpb.getSessionsGridPageCount();
		}
		prevPageSize = defaultAlertsRowCount;
		sessionListRowsBox.setValue(defaultAlertsRowCount);
		setSessionListRowsCount();
		refreshSessionGrid();
		sessionSummaryGrid.setPaginal(sessionsListPgId);
	}

	@Listen("onOK = #sessionListRowsBox")
	public void setSessionListRowsCount() {
		try {
			int pageSize = sessionListRowsBox.getValue();
			sessionsListPgId.setPageSize(pageSize);
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(
					null, null, null, null, -1, -1, null, -1, pageSize, -1, -1,
					-1, -1);
			prevPageSize = pageSize;
		} catch (Exception ex) {
			log.error("Invalid value provided for Sessions - Summary grid row configuration. Row count provided:"
					+ sessionListRowsBox.getValue());
			sessionsListPgId.setPageSize(prevPageSize);
		}
	}

	@SuppressWarnings("unchecked")
	private void refreshSessionGrid() {
		try {
			Object instanceDatabasesObj = Executions
					.getCurrent()
					.getDesktop()
					.getAttribute(
							SQLdmConstants.DASHBOARD_SCOPE_SINGLE_INSTANCE_SESSION);
			// List<InstanceDetailsSession> instanceDatabases = new
			// ArrayList<>();
			Set<InstanceDetailsSession> sessionsModelData = new TreeSet<>(
					new Comparator<InstanceDetailsSession>() {
						@Override
						public int compare(InstanceDetailsSession o1,
								InstanceDetailsSession o2) {
							if (o1.getConnection() != null
									&& o2.getConnection() != null) {
								return o2.getConnection().getId()
										.compareTo(o1.getConnection().getId());
							} else {
								throw new IllegalArgumentException(
										"Session ID must never be null");
							}
						}
					});

			if (instanceDatabasesObj != null) {
				List<InstanceDetailsSession> instanceDatabases = (List<InstanceDetailsSession>) instanceDatabasesObj;
				// instanceDatabases.addAll((List<InstanceDetailsSession>)
				// instanceDatabasesObj);
				for (InstanceDetailsSession sessionObj : instanceDatabases) {
					SessionConnection sesConnection = sessionObj
							.getConnection();
					if (sesConnection.getIsUserSession()) {
						SessionLock seslock = sessionObj.getLock();

						if (sesConnection != null) {
							if (sesConnection.getCommand() == null
									|| sesConnection.getCommand().length() == 0) {
								sesConnection.setCommand(NOT_APPLICABLE);
							}
							if (sesConnection.getAddress() == null
									|| sesConnection.getAddress().length() == 0) {
								sesConnection.setAddress(NOT_APPLICABLE);
							}
							if (sesConnection.getNetLibrary() == null
									|| sesConnection.getNetLibrary().length() == 0) {
								sesConnection.setNetLibrary(NOT_APPLICABLE);
							}
						}

						if (seslock != null) {
							if (seslock.getWaitType() == null
									|| seslock.getWaitType().length() == 0) {
								seslock.setWaitType(NOT_APPLICABLE);
							}
						}
						sessionsModelData.add(sessionObj);
					}
				}
			}
			sessionSummaryGrid
					.setModel(new ListModelList<InstanceDetailsSession>(
							sessionsModelData));
		} catch (Exception x) {
			log.error(x.getMessage(), x);
		}
	}

	@Listen("onClick = #createPDFMenuitem")
	public void exportToPDF() {

		GridExporter.exportToPdf(sessionSummaryGrid,
				InstanceDetailsSession.class, "getMap", "SessionsSummary");

	}

	@Listen("onClick = #createXLSMenuitem")
	public void exportToExcel() {

		GridExporter.exportToExcel(sessionSummaryGrid,
				InstanceDetailsSession.class, "getMap", "SessionsSummary");

	}

	@Listen("onClick = #createXMLMenuitem")
	public void exportToXml() {

		GridExporter.exportToXml(sessionSummaryGrid, titleLabel.getValue(),
				null, InstanceDetailsSession.class, "getMap",
				"SessionsSummary");

	}

}
