package com.idera.sqldm.ui.dashboard.instances.overview;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;
import java.util.StringTokenizer;

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
import org.zkoss.zkplus.databind.BindingListModelList;
import org.zkoss.zul.Div;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Menuitem;
import org.zkoss.zul.Menupopup;
import org.zkoss.zul.Paging;

import com.idera.common.Utility;
import com.idera.common.rest.RestException;
import com.idera.i18n.I18NStrings;
import com.idera.sqldm.data.databases.InstanceDetailsDatabase;
import com.idera.sqldm.data.databases.InstanceDetailsDatabaseFacade;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.WebConstants;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.server.web.component.zul.grid.GridUtil;
import com.idera.sqldm.ui.dashboard.instances.InstanceCategoryTab;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstancePreferencesBean;
import com.idera.sqldm.utils.GridExporter;

public class DatabaseListComposer extends SelectorComposer<Div> {
	private static final long serialVersionUID = 1L;
	protected static final Logger log = Logger.getLogger(DatabaseListComposer.class);
	
	private static final int DATABASE_ROWS = 20;
	
	private int instanceId = 0;
	private int prevPageSize;
	
	@Wire private Intbox databaseListRowsBox;
	@Wire private Paging databaseListPgId;
	@Wire protected Menupopup actionsPopup;
	@Wire protected Label titleLabel;
	@Wire protected Grid databasesGrid;
	@Wire protected Label errorLabel;
	@Wire protected Menupopup reportsMenupopup;
	@Wire protected Menuitem createPDFMenuitem;
	@Wire protected Menuitem createXLSMenuitem;
	@Wire protected Menuitem createXMLMenuitem;

	protected AnnotateDataBinder binder = null;
	public ListModelList<InstanceDetailsDatabase> databasesModel = new BindingListModelList<InstanceDetailsDatabase>(new ArrayList<InstanceDetailsDatabase>(), false);
	
	@Override
	public void doAfterCompose(Div comp) throws Exception {
		super.doAfterCompose(comp);
		
		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "id");
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		}
		else{
			//fallback
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
			if(param != null){
				instanceId = (Integer) param;
			}
		}
		// Subscribe to refresh event.
		EventQueue<Event> q = EventQueues.lookup(WebConstants.INSTANCE_PROPERTIES_REFRESH_EVENT_QUEUE, EventQueues.APPLICATION, true);
		if(q != null) {
			q.subscribe(new EventListener<Event>() {
				@Override
				public void onEvent(Event arg) throws Exception {
			    	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
					if (arg.getData() != null) return;					
					refreshDatabaseGrid(productInstanceName);
				} 
			});
		}

		// bind all grid column sort comparators
		binder = new AnnotateDataBinder(comp);
			
		binder.bindBean("nameSortAsc", new DatabaseNameComparator(true));
		binder.bindBean("nameSortDesc", new DatabaseNameComparator(false));
		
		binder.bindBean("typeSortAsc", InstanceDetailsDatabaseFacade.DATABASE_TYPE_ASC);
		binder.bindBean("typeSortDesc", InstanceDetailsDatabaseFacade.DATABASE_TYPE_DESC);
			
		binder.bindBean("databasesModel", databasesModel);
		binder.loadAll();
		DashboardPreferencesBean dpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		int defaultDatabasesRowCount = DATABASE_ROWS;
		if (dpb != null && dpb.getOverviewDBGridPageCount() != -1) {
			defaultDatabasesRowCount = dpb.getOverviewDBGridPageCount();
		}
		prevPageSize = defaultDatabasesRowCount;
		databaseListRowsBox.setValue(defaultDatabasesRowCount);
		setDatabaseListRowsCount();
    	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

		refreshDatabaseGrid(productInstanceName);
		databasesGrid .setPaginal(databaseListPgId);
	}
		
	private void refreshDatabaseGrid(String productInstanceName) {
		try {
			List<InstanceDetailsDatabase> instanceDatabases = InstanceDetailsDatabaseFacade.getInstanceDetailsDatabases(productInstanceName,instanceId);

			databasesGrid.setHeight(null);
			errorLabel.setVisible(false);

			if ((instanceDatabases != null) && (instanceDatabases.size() > 0)) {
				Collections.sort(instanceDatabases, new DatabaseNameComparator(
						true));
	            String databasesLabel = (instanceDatabases.size()==1 ) ? ELFunctions.getLabel(I18NStrings.DATABASE)
                        : ELFunctions.getLabel(I18NStrings.DATABASES);
	            
				titleLabel.setValue(databasesLabel.toUpperCase() + " ("+ instanceDatabases.size()+ ")");
				databasesGrid.setModel(new BindingListModelList<InstanceDetailsDatabase>(instanceDatabases, false));

				GridUtil.resetColumnSort(databasesGrid, 0);
			} else {
				databasesGrid.setVisible(instanceId  > 0);
			}
		} catch (RestException x) {
			if (instanceId  > 0) {
				databasesGrid.setVisible(true);
				databasesModel.clear();
				databasesGrid.setHeight("22px"); // reduce height to only show
													// the header.
				errorLabel.setVisible(true);
			} else {
				databasesGrid.setVisible(false);
			}
		}
	}

	@Listen("onOK = #databaseListRowsBox")
	public void setDatabaseListRowsCount() {
		try {
			int pageSize = databaseListRowsBox.getValue();
			databaseListPgId.setPageSize(pageSize);
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(null, null, null, null, -1, -1, null, pageSize,-1,-1,-1,-1,-1);
			prevPageSize = pageSize;
		} catch (Exception ex) {
			log.error("Invalid value provided for overview tab - database list row configuration. Row count provided:" + databaseListRowsBox.getValue());
			databaseListPgId.setPageSize(prevPageSize);
		}
	}
	
	@Listen("onClick = grid#databasesGrid > rows > row")
	public void onClickSelectedDatabase(Event evt) {

        SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
        pref.setSelectedCategory(InstanceCategoryTab.DATABASES.getId());
        pref.setSelectedSubCategory(1);
        PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));

	}
	
		
	public class DatabaseTypeComparator implements Comparator<InstanceDetailsDatabase> {
		private boolean descd;

		public DatabaseTypeComparator(boolean descd) {
			this.descd = descd;
		}

		@Override
		public int compare(InstanceDetailsDatabase db1, InstanceDetailsDatabase db2) {

			int ret = 0;

			if(db1 != null && db2 != null && db1.getDatabaseType() != null && db2.getDatabaseType() != null) {
				ret = db1.getDatabaseType().compareToIgnoreCase(db2.getDatabaseType());
			}

			return (ret  * (descd ? 1 : -1));
		}

	}
		
	public class DatabaseNameComparator implements Comparator<InstanceDetailsDatabase> {
		private boolean descd;

		public DatabaseNameComparator(boolean descd) {
			this.descd = descd;
		}

		@Override
		public int compare(InstanceDetailsDatabase db1, InstanceDetailsDatabase db2) {

			int ret = 0;

			if(db1 != null && db2 != null && db1.getDatabaseName() != null && db2.getDatabaseName() != null) {
				ret = db1.getDatabaseName().toLowerCase().compareTo(db2.getDatabaseName().toLowerCase());
			}

			return (ret  * (descd ? 1 : -1));
		}
	}

	@Listen("onClick = #createPDFMenuitem")
	public void exportToPdf() {

		GridExporter.exportToPdf(databasesGrid, InstanceDetailsDatabase.class, "getMap", "Databases");
		
	}

	@Listen("onClick = #createXLSMenuitem")
	public void exportToExcel() {

		GridExporter.exportToExcel(databasesGrid, InstanceDetailsDatabase.class, "getMap", "Databases");

	}

	@Listen("onClick = #createXMLMenuitem")
	public void exportToXml() {

		StringTokenizer str = new StringTokenizer(titleLabel.getValue(), "(");
		String title = null, subtitle = null;
		if(str.hasMoreTokens())
			title = str.nextToken();
		if(str.hasMoreTokens())
			subtitle = "(" + str.nextToken();
		GridExporter.exportToXml(databasesGrid, title, subtitle, InstanceDetailsDatabase.class, "getMap", "Databases");

	}


}
