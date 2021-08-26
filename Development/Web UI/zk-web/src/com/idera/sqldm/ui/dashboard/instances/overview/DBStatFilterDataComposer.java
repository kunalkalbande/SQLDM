package com.idera.sqldm.ui.dashboard.instances.overview;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.zkoss.zk.ui.Executions;

import com.idera.sqldm.data.instances.DatabaseStatsDetails;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstanceOverviewPreferenceBean;
import com.idera.sqldm.utils.Utility;

public class DBStatFilterDataComposer {
	
	private static long OFFSET_VALUE ;
	static List<DatabaseStatsDetails> dbStats;
	
	static List<DatabaseStatsDetails> filteredTransactionsPerSecStats;
	static List<DatabaseStatsDetails> filteredReadPerSecStats;
	static List<DatabaseStatsDetails> filteredLogFlushesPerSecStats;
	private static ArrayList<DatabaseStatsDetails> filteredWritesPerSecStats;
	private static ArrayList<DatabaseStatsDetails> filteredIOStallPerSecStats;
	
	static List<DatabaseStatsDetails> lastSnapshotDBList;
	static Map<Integer,List<String>> top5FilteredStats;
	static Map<String,IderaLineChartModel> dbStatsLineChartModels;
	static List<String> topWritesPerSecList;
	static List<String> topLogFlushesPerSecList;
	static List<String> topIOStallPerSecList;
	private static ArrayList<String> topReadsPerSecList;
	private static ArrayList<String> topTransactionsPerSecList;

	static List<String> databasesList;
	
	@SuppressWarnings("unchecked")
	public static void createFilterMapData(int instanceId){
		SingleInstanceOverviewPreferenceBean overviewPref = PreferencesUtil.getInstance().getOverviewPreferenceInSession(instanceId);
		dbStats = (List<DatabaseStatsDetails>) Executions.getCurrent().getDesktop().getAttribute(CustomizationConstants.DB_STAT_DATA_LIST);
		
		OFFSET_VALUE = Utility.cancelOffSetInMillis();
		filteredTransactionsPerSecStats = new ArrayList<DatabaseStatsDetails>();
		filteredReadPerSecStats = new ArrayList<DatabaseStatsDetails>();
		filteredWritesPerSecStats = new ArrayList<DatabaseStatsDetails>();
		filteredIOStallPerSecStats = new ArrayList<DatabaseStatsDetails>();
		filteredLogFlushesPerSecStats = new ArrayList<DatabaseStatsDetails>();
		
		lastSnapshotDBList = new ArrayList<DatabaseStatsDetails>();
		top5FilteredStats = new HashMap<Integer, List<String>>();
		
		dbStatsLineChartModels = new HashMap<String, IderaLineChartModel>();
		
		topReadsPerSecList = new ArrayList<String>();
		topTransactionsPerSecList = new ArrayList<String>();
		topWritesPerSecList = new ArrayList<String>();
		topLogFlushesPerSecList = new ArrayList<String>();
		topIOStallPerSecList = new ArrayList<String>();
		
		IderaLineChartModel transDBModel = new IderaLineChartModel();
		IderaLineChartModel LogFlushesDBModel = new IderaLineChartModel();
		IderaLineChartModel readsDBModel = new IderaLineChartModel();
		IderaLineChartModel writesDBModel = new IderaLineChartModel();
		IderaLineChartModel IOStallDBModel = new IderaLineChartModel();
		
		//Sorting DB stats based on time
		Collections.sort(dbStats, new Comparator<DatabaseStatsDetails>() {
			@Override
			public int compare(DatabaseStatsDetails o1, DatabaseStatsDetails o2) {
				return o2.getUTCCollectionDateTime().compareTo(o1.getUTCCollectionDateTime());
			}
		});
		Date lastSnapshot = dbStats.get(0).getUTCCollectionDateTime();
		
		//fetching DB that have been captured data for last snapshot time
		for(int i=0;i<dbStats.size();i++){
			DatabaseStatsDetails dbCurrStat = dbStats.get(i);
			if(dbCurrStat.getUTCCollectionDateTime().equals(lastSnapshot)){
				lastSnapshotDBList.add(dbCurrStat);
			}
			else
				break;
		}
		/*
		 * Calculating Top 5 for Read/Sec data
		 * */
		Collections.sort(lastSnapshotDBList, new Comparator<DatabaseStatsDetails>() {
			@Override
			public int compare(DatabaseStatsDetails o1, DatabaseStatsDetails o2) {
				return o2.getReadsPerSec().compareTo(o1.getReadsPerSec());
			}
		});
		for(int i =0 ;i<5 && i<lastSnapshotDBList.size();i++){
			topReadsPerSecList.add(lastSnapshotDBList.get(i).getDatabaseName());
		}
		
		for(int i=0;i<dbStats.size();i++){
			DatabaseStatsDetails dbStat = dbStats.get(i);
			if(topReadsPerSecList.contains(dbStat.getDatabaseName())){
				filteredReadPerSecStats.add(dbStat);
			}
		}
		
		/*
		 * Calculating Top 5 for Write/Sec data
		 * */
		Collections.sort(lastSnapshotDBList, new Comparator<DatabaseStatsDetails>() {
			@Override
			public int compare(DatabaseStatsDetails o1, DatabaseStatsDetails o2) {
				return o2.getWritesPerSec().compareTo(o1.getWritesPerSec());
			}
		});
		for(int i =0 ;i<5 && i<lastSnapshotDBList.size();i++){
			topWritesPerSecList.add(lastSnapshotDBList.get(i).getDatabaseName());
		}
		
		for(int i=0;i<dbStats.size();i++){
			DatabaseStatsDetails dbCurrStat = dbStats.get(i);
			if(topWritesPerSecList.contains(dbCurrStat.getDatabaseName())){
				filteredWritesPerSecStats.add(dbCurrStat);
			}
		}
		/*
		 * Calculating Top 5 for Transactions/Sec data
		 * */
		Collections.sort(lastSnapshotDBList, new Comparator<DatabaseStatsDetails>() {
			@Override
			public int compare(DatabaseStatsDetails o1, DatabaseStatsDetails o2) {
				return (o2.getTransactionsPerSec())
						.compareTo(o1.getTransactionsPerSec());
			}
		});
		for(int i =0 ;i<5 && i<lastSnapshotDBList.size();i++){
			topTransactionsPerSecList.add(lastSnapshotDBList.get(i).getDatabaseName());
		}
		
		for(int i=0;i<dbStats.size();i++){
			DatabaseStatsDetails currDbStat = dbStats.get(i);
			if(topTransactionsPerSecList.contains(currDbStat.getDatabaseName())){
				filteredTransactionsPerSecStats.add(currDbStat);
			}
		}
		
		/*
		 * Calculating Top 5 for IO Stall/Sec data
		 * */
		Collections.sort(lastSnapshotDBList, new Comparator<DatabaseStatsDetails>() {
			@Override
			public int compare(DatabaseStatsDetails o1, DatabaseStatsDetails o2) {
				return (o2.getIOStallPerSec())
						.compareTo(o1.getIOStallPerSec());
			}
		});
		for(int i =0 ;i<5 && i<lastSnapshotDBList.size();i++){
			topIOStallPerSecList.add(lastSnapshotDBList.get(i).getDatabaseName());
		}
		
		for(int i=0;i<dbStats.size();i++){
			DatabaseStatsDetails currDbStat = dbStats.get(i);
			if(topIOStallPerSecList.contains(currDbStat.getDatabaseName())){
				filteredIOStallPerSecStats.add(currDbStat);
			}
		}
		
		/*
		 * Calculating Top 5 for log flushes/Sec data
		 * */
		Collections.sort(lastSnapshotDBList, new Comparator<DatabaseStatsDetails>() {
			@Override
			public int compare(DatabaseStatsDetails o1, DatabaseStatsDetails o2) {
				return (o2.getLogFlushesPerSec())
						.compareTo(o1.getLogFlushesPerSec());
			}
		});
		for(int i =0 ;i<5 && i<lastSnapshotDBList.size();i++){
			topLogFlushesPerSecList.add(lastSnapshotDBList.get(i).getDatabaseName());
		}
		
		for(int i=0;i<dbStats.size();i++){
			DatabaseStatsDetails dbCurrStat = dbStats.get(i);
			if(topLogFlushesPerSecList.contains(dbCurrStat.getDatabaseName())){
				filteredLogFlushesPerSecStats.add(dbCurrStat);
			}
		}
		
		
		for(int i=0; i<filteredReadPerSecStats.size(); i++){
			DatabaseStatsDetails dataObj = filteredReadPerSecStats.get(i);
			long collectionTime = dataObj.getUTCCollectionDateTime().getTime() + OFFSET_VALUE ;
			readsDBModel.setValue(dataObj.getDatabaseName(), collectionTime, dataObj.getReadsPerSec());
		}
		for(int i=0; i<filteredTransactionsPerSecStats.size(); i++){
			DatabaseStatsDetails dataObj = filteredTransactionsPerSecStats.get(i);
			long collectionTime = dataObj.getUTCCollectionDateTime().getTime()+OFFSET_VALUE;
			transDBModel.setValue(dataObj.getDatabaseName(), collectionTime, dataObj.getTransactionsPerSec());
			
		}
		for(int i=0; i<filteredLogFlushesPerSecStats.size(); i++){
			DatabaseStatsDetails dataObj = filteredLogFlushesPerSecStats.get(i);
			long collectionTime = dataObj.getUTCCollectionDateTime().getTime()+OFFSET_VALUE;
			LogFlushesDBModel.setValue(dataObj.getDatabaseName(), collectionTime, dataObj.getLogFlushesPerSec());
		}
		for(int i=0; i<filteredWritesPerSecStats.size(); i++){
			DatabaseStatsDetails dataObj = filteredWritesPerSecStats.get(i);
			long collectionTime = dataObj.getUTCCollectionDateTime().getTime()+OFFSET_VALUE;
			writesDBModel.setValue(dataObj.getDatabaseName(), collectionTime, dataObj.getWritesPerSec());
		}
		
		for(int i=0; i<filteredIOStallPerSecStats.size(); i++){
			DatabaseStatsDetails dataObj = filteredIOStallPerSecStats.get(i);
			long collectionTime = dataObj.getUTCCollectionDateTime().getTime()+OFFSET_VALUE;
			IOStallDBModel.setValue(dataObj.getDatabaseName(), collectionTime, dataObj.getIOStallPerSec());
		}
		
		dbStatsLineChartModels.put(CustomizationConstants.TRANS_DB_MODEL, transDBModel);
		dbStatsLineChartModels.put(CustomizationConstants.LOG_FLUSHES_MODEL, LogFlushesDBModel);
		dbStatsLineChartModels.put(CustomizationConstants.READS_DB_MODEL, readsDBModel);
		dbStatsLineChartModels.put(CustomizationConstants.WRITES_DB_MODEL, writesDBModel);
		dbStatsLineChartModels.put(CustomizationConstants.IO_STALL_DB_MODEL, IOStallDBModel);
		
		top5FilteredStats.put(CustomizationConstants.DB_TRANS_KEY, topTransactionsPerSecList);
		top5FilteredStats.put(CustomizationConstants.DB_LOG_FLUSHES_KEY, topLogFlushesPerSecList);
		top5FilteredStats.put(CustomizationConstants.DB_READS_KEY, topReadsPerSecList);
		top5FilteredStats.put(CustomizationConstants.DB_WRITES_KEY, topWritesPerSecList);
		top5FilteredStats.put(CustomizationConstants.DB_IO_STALL_KEY, topIOStallPerSecList);
		
		Executions.getCurrent().getDesktop().setAttribute("top5FilteredDBStats",top5FilteredStats);
		Executions.getCurrent().getDesktop().setAttribute(CustomizationConstants.DB_STAT_MODEL_NAME,dbStatsLineChartModels);
	}
	
}
