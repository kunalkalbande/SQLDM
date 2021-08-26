package com.idera.sqldm.ui.dashboard.instances.databases;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.collections.Transformer;
import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Component;
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
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Div;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Row;

import com.idera.ccl.IderaDropdownList;
import com.idera.common.Utility;
import com.idera.common.rest.RestException;
import com.idera.sqldm.data.databases.InstanceDetailsDatabase;
import com.idera.sqldm.data.databases.InstanceDetailsDatabaseFacade;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.WebConstants;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.server.web.component.zul.grid.GridUtil;
import com.idera.sqldm.ui.components.charts.bar.IderaStackedBarChart;
import com.idera.sqldm.ui.dashboard.instances.overview.DatabaseListComposer;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstanceDatabasesPreferencesBean;

public class SummaryPageComposer extends SelectorComposer<Div> {
	private static final long serialVersionUID = 1L;
	protected static final Logger log = Logger
			.getLogger(DatabaseListComposer.class);
	protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory
			.getLogger(SummaryPageComposer.class);

	private int instanceId = 0;

	@Wire
	protected Grid dbSummaryGrid;
	@Wire
	private Paging databaseSummaryListPgId;
	@Wire
	protected Label errorLabel, infoLabel;
	// @Wire protected Label chartLabel;
	// @Wire protected IderaBarChart barChart;
	// @Wire protected Div barChartContainerDiv;
	@Wire
	private Intbox databaseSummaryListRowsBox;
	@Wire
	protected IderaStackedBarChart stackedBarChart;
	@Wire
	protected Div StackedBarChartContainerDiv;
	@Wire
	protected IderaDropdownList comboBox;
	private String filterBy;
	private Map<String, Map<String, Number>> stackedChartData = new TreeMap<String, Map<String, Number>>();

	SingleInstanceDatabasesPreferencesBean dbSessionBean;

	private int defaultDatabasesRowCount = 5;
	private int prevPageSize;
	// private String barChartTitle;
	// protected Map<String, Double> chartData = new TreeMap<String, Double>();

	protected AnnotateDataBinder binder = null;

	public ListModelList<InstanceDetailsDatabase> databasesModel = new BindingListModelList<InstanceDetailsDatabase>(
			new ArrayList<InstanceDetailsDatabase>(), false);

	public enum FilterLabel {
		DATAINMB("Data Megabytes"), DATAINPERCENT("Data Percent"), LOGINMB(
				"Log Megabytes"), LOGINPERCENT("Log percent");

		private String label;

		private FilterLabel(String label) {
			this.label = label;
		}

		private String getLabel() {
			return this.label;
		}
	};

	@Override
	public void doAfterCompose(Div comp) throws Exception {
		super.doAfterCompose(comp);

		// barChartTitle =
		// ELFunctions.getMessage(com.idera.sqldm.i18n.SQLdmI18NStrings.CAPACITY_USAGE);
		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions
				.getCurrent().getParameterMap(), "id");
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
					refreshDatabaseGrid();
				}
			});
		}

		// bind all grid column sort comparators
		binder = new AnnotateDataBinder(comp);

		binder.bindBean("nameSortAsc", new DatabaseNameComparator(true));
		binder.bindBean("nameSortDesc", new DatabaseNameComparator(false));

		binder.bindBean("databasesModel", databasesModel);
		binder.loadAll();
		DashboardPreferencesBean dpb = PreferencesUtil.getInstance()
				.getDashboardPreferencesInSession();
		if (dpb != null && dpb.getDatabaseSummaryGridpageCount() != -1) {
			defaultDatabasesRowCount = dpb.getDatabaseSummaryGridpageCount();
		}

		ListModelList<String> filterModel = new ListModelList<String>();
		for (FilterLabel label : FilterLabel.values()) {
			filterModel.add(label.getLabel());
		}
		comboBox.setModel(filterModel);

		prevPageSize = defaultDatabasesRowCount;
		databaseSummaryListRowsBox.setValue(defaultDatabasesRowCount);
		setDatabaseSummaryListRowsCount();
		refreshDatabaseGrid();
		dbSummaryGrid.setPaginal(databaseSummaryListPgId);
		dbSessionBean = PreferencesUtil.getInstance()
				.getDatabasesPreferenceInSession(instanceId);
	}

	@Listen("onAfterRender = #dbSummaryGrid")
	public void defaultSelect() {
		if (dbSessionBean != null && dbSessionBean.isFirstLoad() == false
				&& dbSessionBean.getDiList() != null) {
			comboBox.setText("Capacity Usage: " + dbSessionBean.getFilterBy());
			drawChart(dbSessionBean.getDiList(), dbSessionBean.getFilterBy());

			// Collecting dbNames from selected items
			@SuppressWarnings("unchecked")
			Collection<String> dbNames = CollectionUtils.collect(dbSessionBean
					.getDiList().iterator(), new Transformer() {
				public Object transform(Object input) {
					return ((InstanceDetailsDatabase) input).getDatabaseName();
				}
			});

			List<Component> rows = (List<Component>) dbSummaryGrid.getRows()
					.getChildren();
			for (Component row : rows) {
				Row r = (Row) row;
				Component k = r.getFirstChild();
				InstanceDetailsDatabase dbDetails = (InstanceDetailsDatabase) r
						.getValue();
				if (dbDetails != null
						&& dbNames.contains(dbDetails.getDatabaseName())
						&& k instanceof Checkbox) {
					((Checkbox) k).setChecked(true);
					r.setClass("clickable_row z-row selected-row");
				}

			}

			return;

		}
		dbSessionBean.setFirstLoad(false);
		PreferencesUtil.getInstance().setDatabasesPreferenceInSession(
				dbSessionBean);
		List<Component> rows = (List<Component>) dbSummaryGrid.getRows()
				.getChildren();
		if(rows.size()!=0){
		Row r = (Row) rows.get(0);
		Component k = r.getFirstChild();
		if (k instanceof Checkbox) {
			((Checkbox) k).setChecked(true);
			r.setClass("clickable_row z-row selected-row");
		}}
		comboBox.setSelectedIndex(0);
		comboBox.setText("Capacity Usage: " + comboBox.getValue());
	}

	@Listen("onChange = idera-cwf-dropdownlist#comboBox")
	public void redrawStackedbarChart(Event evt) {
		String filterby = comboBox.getValue();
		comboBox.setText("Capacity Usage: " + filterby);

		List<Component> rows = (List<Component>) dbSummaryGrid.getRows()
				.getChildren();
		List<InstanceDetailsDatabase> diList = new LinkedList<>();
		for (Component row : rows) {
			Row r = (Row) row;
			Component k = r.getFirstChild();
			if (r.getFirstChild() instanceof Checkbox
					&& ((Checkbox) k).isChecked()) {
				diList.add((InstanceDetailsDatabase) r.getValue());
			}
		}
		// Setting preferences in session
		dbSessionBean.setFilterBy(filterby);
		dbSessionBean.setDiList(diList);
		PreferencesUtil.getInstance().setDatabasesPreferenceInSession(
				dbSessionBean);
		this.filterBy = filterby;
		drawChart(diList, filterby);
	}

	@Listen("onOK = #databaseSummaryListRowsBox")
	public void setDatabaseSummaryListRowsCount() {
		try {
			int pageSize = databaseSummaryListRowsBox.getValue();
			databaseSummaryListPgId.setPageSize(pageSize);
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(
					null, null, null, null, -1, -1, null, -1, -1, -1, -1,
					pageSize, -1);
			prevPageSize = pageSize;
		} catch (Exception ex) {
			log.error("Invalid value provided for Database Summary Grid row configuration. Row count provided:"
					+ databaseSummaryListRowsBox.getValue());
			databaseSummaryListPgId.setPageSize(prevPageSize);
		}
	}

	/*
	 * protected void loadChartData(InstanceDetailsDatabase db) throws
	 * RestException { barChart.setNoDataMessage(SQLdmI18NStrings.
	 * NO_DATABASES_HAVE_ANY_TRANSACTION_ACTIVITY);
	 * chartData.put(ELFunctions.getMessage(SQLdmI18NStrings.DATABASE_FILES),
	 * (double)db.getNoOfFiles());
	 * chartData.put(ELFunctions.getMessage(SQLdmI18NStrings.DATABASE_FILE_SIZE
	 * ), Math.round(db.getCurrentDataFileSizeInMb()*100.0)/100.0);
	 * chartData.put(ELFunctions.getMessage(SQLdmI18NStrings.DATABASE_LOG_SIZE
	 * ), Math.round(db.getCurrentLogFileSizeInMb()*100.0)/100.0);
	 * chartData.put(
	 * ELFunctions.getMessage(SQLdmI18NStrings.DATABASE_DATA_UNUSED_SIZE),
	 * Math.round(db.getUnusedDataSizeInMb()*100.0)/100.0);
	 * chartData.put(ELFunctions
	 * .getMessage(SQLdmI18NStrings.DATABASE_USED_LOG_SIZE ),
	 * Math.round(db.getCurrentLogSizeInMb()*100.0)/100.0); }
	 * 
	 * protected void drawChart(InstanceDetailsDatabase db) { try { // Clear the
	 * chart data chartData.clear();
	 * 
	 * loadChartData(db);
	 * 
	 * if ((chartData != null) && (!chartData.isEmpty())) {
	 * barChart.setShowZeroValues(true); barChart.setFitToHeight(false);
	 * barChart.setOrient("horizontal"); barChart.setAnimateBars(true);
	 * barChart.setAnimationLength(500); barChart.setAnimationDelay(100);
	 * barChart.setChartData(chartData); } else { setEmptyCharts(); }
	 * 
	 * String dbName = ""; if (db.getDatabaseName() != null &&
	 * db.getDatabaseName().length() > 0) { dbName = db.getDatabaseName(); }
	 * chartLabel.setValue(barChartTitle + dbName); } catch (RestException x) {
	 * barChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.
	 * ERROR_OCCURRED_LOADING_CHART)); } }
	 */

	protected void drawChart(List<InstanceDetailsDatabase> diList,
			String filterBy) {
		stackedChartData.clear();
		loadChartData(diList, filterBy);
		drawStackedBarChart();
	}

	public void loadChartData(List<InstanceDetailsDatabase> diList,
			String filterBy) {
		if (diList == null || diList.size() <= 0) {
			// stackedBarChart.setErrorMessage(SQLdmI18NStrings.DASHBOARD_QUERY_DATA_NOT_AVAILABLE);
			StackedBarChartContainerDiv.setVisible(false);
			infoLabel.setVisible(true);
			return;
		}
		infoLabel.setVisible(false);
		StackedBarChartContainerDiv.setVisible(true);

		try {
			// switch(filterBy){
			if (FilterLabel.DATAINPERCENT.getLabel().equals(filterBy)) {
				for (InstanceDetailsDatabase di : diList) {
					Map<String, Number> innerMap = new TreeMap<String, Number>();
					innerMap.put(
							ELFunctions
									.getMessage(SQLdmI18NStrings.DATABASE_DATA_INDEXES_SIZE_PERCENT),
							Math.round((di.getCurrentIndexSizeInMb() / di
									.getCurrentDataFileSizeInMb()) * 10000.0) / 100.0);
					innerMap.put(
							ELFunctions
									.getMessage(SQLdmI18NStrings.DATABASE_DATA_TEXT_SIZE_PERCENT),
							Math.round((di.getCurrentTextSizeInMb() / di
									.getCurrentDataFileSizeInMb()) * 10000.0) / 100.0);
					innerMap.put(
							ELFunctions
									.getMessage(SQLdmI18NStrings.DATABASE_DATA_TABLES_SIZE_PERCENT),
							Math.round((di.getCurrentDataSizeInMb() / di
									.getCurrentDataFileSizeInMb()) * 10000.0) / 100.0);
					innerMap.put(
							ELFunctions
									.getMessage(SQLdmI18NStrings.DATABASE_DATA_UNUSED_SIZE_PERCENT),
							Math.round((di.getUnusedDataSizeInMb() / di
									.getCurrentDataFileSizeInMb()) * 10000.0) / 100.0);
					stackedChartData.put(di.getDatabaseName(), innerMap);
				}
			}
			if (FilterLabel.LOGINMB.getLabel().equals(filterBy)) {
				for (InstanceDetailsDatabase di : diList) {
					Map<String, Number> innerMap = new TreeMap<String, Number>();
					innerMap.put(
							ELFunctions
									.getMessage(SQLdmI18NStrings.DATABASE_UNUSED_LOG_SIZE),
							Math.round(di.getUnusedLogSizeInMb() * 100.0) / 100.0);
					innerMap.put(
							ELFunctions
									.getMessage(SQLdmI18NStrings.DATABASE_USED_LOG_SIZE),
							Math.round(di.getCurrentLogSizeInMb() * 100.0) / 100.0);
					stackedChartData.put(di.getDatabaseName(), innerMap);
				}
			}
			if (FilterLabel.LOGINPERCENT.getLabel().equals(filterBy)) {
				for (InstanceDetailsDatabase di : diList) {
					Map<String, Number> innerMap = new TreeMap<String, Number>();
					innerMap.put(
							ELFunctions
									.getMessage(SQLdmI18NStrings.DATABASE_UNUSED_LOG_SIZE_PERCENT),
							Math.round((di.getUnusedLogSizeInMb() / di
									.getCurrentLogFileSizeInMb()) * 10000.0) / 100.0);
					innerMap.put(
							ELFunctions
									.getMessage(SQLdmI18NStrings.DATABASE_USED_LOG_SIZE_PERCENT),
							Math.round((di.getCurrentLogSizeInMb() / di
									.getCurrentLogFileSizeInMb()) * 10000.0) / 100.0);
					stackedChartData.put(di.getDatabaseName(), innerMap);
				}
			}
			if (FilterLabel.DATAINMB.getLabel().equals(filterBy)) {
				for (InstanceDetailsDatabase di : diList) {
					Map<String, Number> innerMap = new TreeMap<String, Number>();
					innerMap.put(
							ELFunctions
									.getMessage(SQLdmI18NStrings.DATABASE_DATA_INDEXES_SIZE_MB),
							Math.round(di.getCurrentIndexSizeInMb() * 100.0) / 100.0);
					innerMap.put(
							ELFunctions
									.getMessage(SQLdmI18NStrings.DATABASE_DATA_TEXT_SIZE_MB),
							Math.round(di.getCurrentTextSizeInMb() * 100.0) / 100.0);
					innerMap.put(
							ELFunctions
									.getMessage(SQLdmI18NStrings.DATABASE_DATA_TABLES_SIZE_MB),
							Math.round(di.getCurrentDataSizeInMb() * 100.0) / 100.0);
					innerMap.put(
							ELFunctions
									.getMessage(SQLdmI18NStrings.DATABASE_DATA_UNUSED_SIZE_MB),
							Math.round(di.getUnusedDataSizeInMb() * 100.0) / 100.0);
					stackedChartData.put(di.getDatabaseName(), innerMap);
				}
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	private void drawStackedBarChart() {
		if (stackedChartData == null || stackedChartData.isEmpty())
			return;
		stackedBarChart.setOrient("horizontal");
		stackedBarChart.setChartData(stackedChartData);
		stackedBarChart.setVisible(true);
		stackedBarChart.getChart().setMouseOverText("%s");
		stackedBarChart.getChart().setInnerPadding(0.1);
		stackedBarChart.getChart().setOuterPadding(0.1);
	}

	private void refreshDatabaseGrid() {

		dbSummaryGrid.setVisible(instanceId > 0);
		String productInstanceName = Utility.getUrlParameter(Executions
				.getCurrent().getParameterMap(), "instance");

		try {
			List<InstanceDetailsDatabase> instanceDatabases = InstanceDetailsDatabaseFacade
					.getInstanceDetailsDatabases(productInstanceName,
							instanceId);
			dbSummaryGrid.setHeight(null);
			errorLabel.setVisible(false);

			if ((instanceDatabases != null) && (instanceDatabases.size() > 0)) {
				Collections.sort(instanceDatabases, new DatabaseNameComparator(
						true));
				// dbSummaryGrid.setModel(new
				// BindingListModelList<InstanceDetailsDatabase>(instanceDatabases,
				// false));

				dbSummaryGrid
						.setModel(new BindingListModelList<InstanceDetailsDatabase>(
								instanceDatabases, false));

				GridUtil.resetColumnSort(dbSummaryGrid, 0);
				/*
				 * InstanceDetailsDatabase db = instanceDatabases.get(0); if
				 * (dbSummaryGrid.getRows() != null &&
				 * dbSummaryGrid.getRows().getChildren() != null &&
				 * dbSummaryGrid.getRows().getChildren().size() > 0) { Row row =
				 * (Row)dbSummaryGrid.getRows().getChildren().get(0);
				 * row.setClass("clickable_row z-row selected-row"); }
				 * drawChart(db);
				 */
				List<InstanceDetailsDatabase> db = new LinkedList<>();
				db.add(instanceDatabases.get(0));
				drawChart(db, FilterLabel.DATAINMB.getLabel());
			}

		} catch (RestException x) {
			if (instanceId > 0) {
				dbSummaryGrid.setVisible(true);
				databasesModel.clear();
				dbSummaryGrid.setHeight("22px"); // reduce height to only show
													// the header.
				errorLabel.setVisible(true);
			} else {
				dbSummaryGrid.setVisible(false);
			}
		}
	}

	@Listen("onClick = grid#dbSummaryGrid > rows > row > idera-cwf-checkbox")
	public void onClickSelectedDatabase(Event evt) {

		// prevent reentrancy
		if (WebUtil.getComponentById(Executions.getCurrent().getDesktop(),
				"databaseDetailsDialog") != null)
			return;
		List<Component> rows = (List<Component>) dbSummaryGrid.getRows()
				.getChildren();
		List<InstanceDetailsDatabase> diList = new LinkedList<>();
		for (Component row : rows) {
			Row r = (Row) row;
			// r.setClass("clickable_row z-row");
			Component k = r.getFirstChild();
			if (k instanceof Checkbox && ((Checkbox) k).isChecked()) {
				r.setClass("clickable_row z-row selected-row");
				diList.add((InstanceDetailsDatabase) r.getValue());
			} else
				r.setClass("clickable_row z-row");
		}
		// Setting preferences in session

		// dbpb.setFilterBy(filterby);
		dbSessionBean.setDiList(diList);
		PreferencesUtil.getInstance().setDatabasesPreferenceInSession(
				dbSessionBean);

		// Get the row instance.
		/*
		 * InstanceDetailsDatabase database =
		 * getInstanceDetailsDatabaseFromRowComponentEvent(evt); if(database !=
		 * null) { drawChart(database); }
		 */
		drawChart(diList, filterBy == null ? FilterLabel.DATAINMB.getLabel()
				: filterBy);
	}

	/*
	 * private InstanceDetailsDatabase
	 * getInstanceDetailsDatabaseFromRowComponentEvent(Event evt) { // Get row
	 * from event. Row row = (Row) evt.getTarget();
	 * row.setClass("clickable_row z-row selected-row"); // Get
	 * InstanceDetailsDatabase. InstanceDetailsDatabase di = null; if (row !=
	 * null && row.getValue() != null && row.getValue() instanceof
	 * InstanceDetailsDatabase) { di = (InstanceDetailsDatabase) row.getValue();
	 * }
	 * 
	 * return di; }
	 */

	public class DatabaseTypeComparator implements
			Comparator<InstanceDetailsDatabase> {
		private boolean descd;

		public DatabaseTypeComparator(boolean descd) {
			this.descd = descd;
		}

		@Override
		public int compare(InstanceDetailsDatabase db1,
				InstanceDetailsDatabase db2) {

			int ret = 0;

			if (db1 != null && db2 != null && db1.getDatabaseType() != null
					&& db2.getDatabaseType() != null) {
				ret = db1.getDatabaseType().compareToIgnoreCase(
						db2.getDatabaseType());
			}

			return (ret * (descd ? 1 : -1));
		}

	}

	public class DatabaseNameComparator implements
			Comparator<InstanceDetailsDatabase> {
		private boolean descd;

		public DatabaseNameComparator(boolean descd) {
			this.descd = descd;
		}

		@Override
		public int compare(InstanceDetailsDatabase db1,
				InstanceDetailsDatabase db2) {

			int ret = 0;

			if (db1 != null && db2 != null && db1.getDatabaseName() != null
					&& db2.getDatabaseName() != null) {
				ret = db1.getDatabaseName().toLowerCase()
						.compareTo(db2.getDatabaseName().toLowerCase());
			}

			return (ret * (descd ? 1 : -1));
		}
	}

	/*
	 * private void setEmptyCharts() {
	 * barChart.setErrorMessage(ELFunctions.getMessage
	 * (SQLdmI18NStrings.NO_DATA_AVAILABLE)); }
	 */
}
