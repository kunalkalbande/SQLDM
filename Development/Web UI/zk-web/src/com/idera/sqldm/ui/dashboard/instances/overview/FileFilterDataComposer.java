package com.idera.sqldm.ui.dashboard.instances.overview;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.zkoss.zk.ui.Executions;

import com.idera.sqldm.data.category.resources.FileStats;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstanceOverviewPreferenceBean;
import com.idera.sqldm.utils.Utility;

public class FileFilterDataComposer {
	private static List<String> selectedComponents;
	static List<FileStats> fileStats;
	static List<FileStats> filteredWritePerSecStats;
	static List<FileStats> filteredStats;
	static List<FileStats> filteredReadPerSecStats;
	static List<FileStats> filteredTransfersPerSecStats;
	static List<FileStats> topFiveFileStats;
	static List<FileStats> lastSnapshotFileList;
	static Map<String,List<String>> top5FilteredStats;
	static Map<Integer,IderaLineChartModel> fileStatsLineChartModels;
	static List<String> topFileReadPerSecList;
	static List<String> topFileWritePerSecList;
	static List<String> topFileTransferPerSecList;
	static List<String> topFiveKeys;
	static List<String> databasesList;
	private static ArrayList<String> filesList;
	
	@SuppressWarnings("unchecked")
	public static void createFilterMapData(int instanceId){
		SingleInstanceOverviewPreferenceBean overviewPref = PreferencesUtil.getInstance().getOverviewPreferenceInSession(instanceId);
		selectedComponents = overviewPref.getSelectedFileComponent();
		fileStats = (List<FileStats>) Executions.getCurrent().getDesktop().getAttribute("fileStats");
		filteredWritePerSecStats = new ArrayList<FileStats>();
		filteredReadPerSecStats = new ArrayList<FileStats>();
		filteredStats = new ArrayList<FileStats>();
		filteredTransfersPerSecStats = new ArrayList<FileStats>();
		topFiveFileStats = new ArrayList<FileStats>();
		lastSnapshotFileList = new ArrayList<FileStats>();
		top5FilteredStats = new HashMap<String, List<String>>();
		fileStatsLineChartModels = new HashMap<Integer, IderaLineChartModel>();
		topFileReadPerSecList = new ArrayList<String>();
		topFileWritePerSecList = new ArrayList<String>();
		topFileTransferPerSecList = new ArrayList<String>();
		IderaLineChartModel readsPerSecFileStatsModel = new IderaLineChartModel();
		IderaLineChartModel writesPerSecFileStatsModel = new IderaLineChartModel();
		IderaLineChartModel tranfersPerSecFileStatsModel = new IderaLineChartModel();
		if(fileStats!=null){
			for(int i = 1; i < selectedComponents.size(); i++){
				for(int j = 0 ; j < fileStats.size() ; j++){
					String type = selectedComponents.get(0);
					String value = selectedComponents.get(i);
					FileStats fileData = new FileStats();
					fileData=fileStats.get(j);
					switch(type){
					case "DatabaseName" :
						if(fileData.getDatebaseName().equals(value)){
							filteredStats.add(fileData);
						}
						break;
					case "Drive" :
						if(fileData.getDrive().equals(value)){
							filteredStats.add(fileData);
						}
						break;
					case "FileName" :
						if(fileData.getFilePath().equals(value)){
							filteredStats.add(fileData);
						}
						break;
					case "AllFiles" :
						filteredStats.add(fileData);
						break;
					}
				}
			}

			Collections.sort(filteredStats, new Comparator<FileStats>() {
				@Override
				public int compare(FileStats o1, FileStats o2) {
					return o2.getUTCCollectionDateTime().compareTo(o1.getUTCCollectionDateTime());
				}
			});
			Date lastSnapshot = filteredStats.get(0).getUTCCollectionDateTime();
			for(int i=0;i<filteredStats.size();i++){
				FileStats fileStat = filteredStats.get(i);
				if(fileStat.getUTCCollectionDateTime().equals(lastSnapshot)){
					lastSnapshotFileList.add(fileStat);
				}
				else{
					break;
				}
			}
			/*
			 * Calculating Top 5 for Read/Sec data
			 * */
			Collections.sort(lastSnapshotFileList, new Comparator<FileStats>() {
				@Override
				public int compare(FileStats o1, FileStats o2) {
					return o2.getStatistics().getFileReadsPerSec().compareTo(o1.getStatistics().getFileReadsPerSec());
				}
			});
			for(int i =0 ;i<5 && i<lastSnapshotFileList.size();i++){
				topFileReadPerSecList.add(lastSnapshotFileList.get(i).getFilePath());
			}

			for(int i=0;i<filteredStats.size();i++){
				FileStats fileStat = filteredStats.get(i);
				if(topFileReadPerSecList.contains(fileStat.getFilePath())){
					filteredReadPerSecStats.add(fileStat);
				}
			}

			/*
			 * Calculating Top 5 for Write/Sec data
			 * */
			Collections.sort(lastSnapshotFileList, new Comparator<FileStats>() {
				@Override
				public int compare(FileStats o1, FileStats o2) {
					return o2.getStatistics().getFileWritesPerSec().compareTo(o1.getStatistics().getFileWritesPerSec());
				}
			});
			for(int i =0 ;i<5 && i<lastSnapshotFileList.size();i++){
				topFileWritePerSecList.add(lastSnapshotFileList.get(i).getFilePath());
			}

			for(int i=0;i<filteredStats.size();i++){
				FileStats fileStat = filteredStats.get(i);
				if(topFileWritePerSecList.contains(fileStat.getFilePath())){
					filteredWritePerSecStats.add(fileStat);
				}
			}
			/*
			 * Calculating Top 5 for Transfer/Sec data
			 * */
			Collections.sort(lastSnapshotFileList, new Comparator<FileStats>() {
				@Override
				public int compare(FileStats o1, FileStats o2) {
					return ((Double)(o2.getStatistics().getFileWritesPerSec()+o2.getStatistics().getFileReadsPerSec()))
							.compareTo((Double)(o1.getStatistics().getFileWritesPerSec()+o1.getStatistics().getFileReadsPerSec()));
				}
			});
			for(int i =0 ;i<5 && i<lastSnapshotFileList.size();i++){
				topFileTransferPerSecList.add(lastSnapshotFileList.get(i).getFilePath());
			}

			for(int i=0;i<filteredStats.size();i++){
				FileStats fileStat = filteredStats.get(i);
				if(topFileTransferPerSecList.contains(fileStat.getFilePath())){
					filteredTransfersPerSecStats.add(fileStat);
				}
			}

			for(int i=0; i<filteredReadPerSecStats.size(); i++){
				FileStats dataObj = filteredReadPerSecStats.get(i);
				long collectionTime = dataObj.getUTCCollectionDateTime().getTime()+Utility.cancelOffSetInMillis();
				readsPerSecFileStatsModel.setValue(dataObj.getFileName(), collectionTime, dataObj.getStatistics().getFileReadsPerSec());
				/*writesPerSecFileStatsModel.setValue(dataObj.getFileName(), collectionTime, dataObj.getStatistics().getFileWritesPerSec());
			tranfersPerSecFileStatsModel.setValue(dataObj.getFileName(), collectionTime, (dataObj.getStatistics().getFileReadsPerSec()+dataObj.getStatistics().getFileReadsPerSec()));*/
			}
			for(int i=0; i<filteredWritePerSecStats.size(); i++){
				FileStats dataObj = filteredWritePerSecStats.get(i);
				long collectionTime = dataObj.getUTCCollectionDateTime().getTime()+Utility.cancelOffSetInMillis();
				writesPerSecFileStatsModel.setValue(dataObj.getFileName(), collectionTime, dataObj.getStatistics().getFileWritesPerSec());

			}
			for(int i=0; i<filteredTransfersPerSecStats.size(); i++){
				FileStats dataObj = filteredTransfersPerSecStats.get(i);
				long collectionTime = dataObj.getUTCCollectionDateTime().getTime()+Utility.cancelOffSetInMillis();
				tranfersPerSecFileStatsModel.setValue(dataObj.getFileName(), collectionTime, (dataObj.getStatistics().getFileWritesPerSec()+dataObj.getStatistics().getFileReadsPerSec()));

			}
		}
		fileStatsLineChartModels.put(0, readsPerSecFileStatsModel);
		fileStatsLineChartModels.put(1, writesPerSecFileStatsModel);
		fileStatsLineChartModels.put(2, tranfersPerSecFileStatsModel);
		top5FilteredStats.put("Top5ReadsPerSec", topFileReadPerSecList);
		top5FilteredStats.put("Top5WritesPerSec", topFileWritePerSecList);
		top5FilteredStats.put("Top5TransfersPerSec", topFileTransferPerSecList);
		Executions.getCurrent().getDesktop().setAttribute("top5FilteredStats",top5FilteredStats);
		Executions.getCurrent().getDesktop().setAttribute("fileStatsLineChartModels",fileStatsLineChartModels);
	}
	
	public static List<String> getAllDatabases(){
		databasesList = new ArrayList<String>();
		if(fileStats!=null){
			databasesList.add(fileStats.get(0).getDatebaseName());
			for(int j = 1 ; j < fileStats.size() ; j++){
				if(!databasesList.contains(fileStats.get(j).getDatebaseName()))
					databasesList.add(fileStats.get(j).getDatebaseName());
			}
		}
		return databasesList;
	}
	
	public static List<String> getAllFileNames(){
		filesList = new ArrayList<String>();
		if(fileStats!=null){
			filesList.add(fileStats.get(0).getFileName());
			for(int j = 1 ; j < fileStats.size() ; j++){
				if(!filesList.contains(fileStats.get(j).getFileName()))
					filesList.add(fileStats.get(j).getFileName());
			}
		}
		return filesList;
	}
	
	public static List<String> getAllDrives(){
		filesList = new ArrayList<String>();
		if(fileStats!=null){
			filesList.add(fileStats.get(0).getDrive());
			for(int j = 1 ; j < fileStats.size() ; j++){
				if(!filesList.contains(fileStats.get(j).getDrive()))
					filesList.add(fileStats.get(j).getDrive());
			}
		}
		return filesList;
	}
	
	public static List<String> getAllFilePaths(){
		filesList = new ArrayList<String>();
		if(fileStats!=null){
			filesList.add(fileStats.get(0).getFilePath());
			for(int j = 1 ; j < fileStats.size() ; j++){
				if(!filesList.contains(fileStats.get(j).getFilePath()))
					filesList.add(fileStats.get(j).getFilePath());
			}
		}
		return filesList;
	}
}
