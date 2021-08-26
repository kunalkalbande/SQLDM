package com.idera.sqldm.ui.dashboard.instances.resources;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import org.apache.commons.lang.StringUtils;
import org.apache.log4j.Logger;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;

import com.idera.common.Comparators;
import com.idera.common.rest.RestException;
import com.idera.sqldm.data.ResourcesFacade;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.category.resources.FileActivity;
import com.idera.sqldm.data.category.resources.FileDrive;
import com.idera.sqldm.data.databases.InstanceDetailsDatabase;
import com.idera.sqldm.data.databases.InstanceDetailsDatabaseFacade;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstancePreferencesBean;
import com.idera.sqldm.utils.Utility;

public class FileActivityViewModel {
	
	enum FileTypes{
		data("Data"), log("Log"), other("Other");
		private String name;
		
		private FileTypes(String name){
			this.name = name;
		}

		public String getName() {
			return name;
		}

		public void setName(String name) {
			this.name = name;
		}
		
	}

	private static final Logger LOGGER = Logger.getLogger(FileActivityViewModel.class);
	
	private int instanceId = -1;
	private List<FileType> fileTypeList;
	private List<InstanceDetailsDatabase> databasesList = new ArrayList<>();
	private List<FileDrive> allDrives = new ArrayList<>();
	private List<FileActivityModel> fileItems;
	private Map<FileActivityModel, List<FileActivity>> dataModelMap;
	
	private FileActivityFilters filters;
	private long OFFSET_MILLIS;
	@Wire IderaLineChart activityChart;
	
	@Init
	public void init(){
		OFFSET_MILLIS = Utility.cancelOffSetInMillis();
		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "id");
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		}else{
			//fallback
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
			if(param != null){
				instanceId = (Integer) param;
			}
		}
		
		
		SingleInstancePreferencesBean sipb = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
		if(sipb != null && sipb.getFileActivityFilters() != null){
			try {
				//Cloning actual object so that if filters been changed but no fiter button pressed should not change the saved filters.
				filters = (FileActivityFilters) sipb.getFileActivityFilters().clone();
			} catch (CloneNotSupportedException e) {
				// TODO Auto-generated catch block
			}
        }
		else{
			filters = new FileActivityFilters();
			filters.getFileTypes().add(FileTypes.data.getName());
			filters.getFileTypes().add(FileTypes.log.getName());
			filters.getFileTypes().add(FileTypes.other.getName());
			
		}
		fileTypeList = new ArrayList<>();
		fileTypeList.add(new FileType(FileTypes.data.getName(), filters.getFileTypes().contains(FileTypes.data.getName())));
		fileTypeList.add(new FileType(FileTypes.log.getName(), filters.getFileTypes().contains(FileTypes.log.getName())));
		fileTypeList.add(new FileType(FileTypes.other.getName(), filters.getFileTypes().contains(FileTypes.other.getName())));
		
		//populateDatabaseListBox();
	}
	
	@AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view){
        Selectors.wireComponents(view, this, false);
        populateFileItems();
        populateDatabaseListBox();
        populateFileDrives();
        //wire event listener
//      Selectors.wireEventListeners(view, this);
    }
	
	private void populateFileItems(){
		//
		try {
        	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

			dataModelMap = new HashMap<FileActivityModel, List<FileActivity>>();
			List<FileActivity> items = ResourcesFacade.getFileActivity(productInstanceName, instanceId, 30);
			if(items !=null && items.size()>0){
			for(FileActivity item : items){
				FileActivityModel key = new FileActivityModel(item.getDatebaseName(),item.getDrive(),item.getFileName(), item.getFileType());
				key.setReadsPerSec(item.getStatistics().getFileReadsPerSec());
				key.setWritesPerSec(item.getStatistics().getFileWritesPerSec());
				key.setFilePath(item.getFilePath());
				if(!dataModelMap.containsKey(key)){
					List<FileActivity> activitySet = new ArrayList<FileActivity>();
					activitySet.add(item);
					dataModelMap.put(key, activitySet);
				}
				else{
					dataModelMap.get(key).add(item);
				}
			}
			refreshModels();
			}else{
				
				emptyCharts();
			}
		} catch (CategoryException e) {
			LOGGER.error(e);
		}
	}
	
	private void emptyCharts() {
		activityChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		
	}

	private void refreshModels(){
		List<FileActivityModel> allFiles = new ArrayList<>();
		allFiles.addAll(dataModelMap.keySet());
		boolean isPass = true;
		fileItems = new ArrayList<>();
		for(Iterator<FileActivityModel> it = allFiles.iterator(); it.hasNext();){
			isPass = true;
			FileActivityModel model = it.next();
			
			if(isPass && !StringUtils.isEmpty(filters.getFileNameLike())){
				isPass = Utility.wildCardMatch(model.getFileName(), filters.getFileNameLike());
			}
			if(isPass && !StringUtils.isEmpty(filters.getFilePathLike()) && model.getFilePath() != null){
				isPass = Utility.wildCardMatch(model.getFilePath(), filters.getFilePathLike());
			}
			if(isPass && !StringUtils.isEmpty(filters.getDatabaseBoxValue())){
				List<String> databaseNames = Arrays.asList(filters.getDatabaseBoxValue().split(","));
				isPass = databaseNames.contains(model.getDatabaseName());
			}
			if(isPass && !StringUtils.isEmpty(filters.getDriveBoxValue())){
				List<String> driveNames = Arrays.asList(filters.getDriveBoxValue().split(","));
				isPass = driveNames.contains(model.getDrive());
			}
			if(isPass && filters.getFileTypes().size() < 3){
				isPass = filters.getFileTypes().contains(model.getFileType());
			}
			if(isPass){
				fileItems.add(model);
			}
			
		}
		
		applySorting();
		if(fileItems.size() > 0){
			redrawChart(fileItems.get(0));
		}
		else{
			//activityChart.setTitle("File Activity");
			activityChart.setErrorMessage(ELFunctions.getLabel("Messages.chart-has-no-data"));
		}
	}
	
	private void populateFileDrives(){
    	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

		try {
			allDrives = ResourcesFacade.getFileDrives(productInstanceName, instanceId);
		} catch (CategoryException e) {
			LOGGER.error(e);
		}
	}
	
	@Command
	public void redrawChart(@BindingParam("item") FileActivityModel model){
		activityChart.setTitle(model.getFileName());
		activityChart.setModel(getModelData(model));
		//activityChart.setTitle(ELFunctions.getMessage(I18NStrings.RESOURCES_FILE_ACTIVITY));
	}
	
	private IderaLineChartModel getModelData(FileActivityModel activity){
		IderaLineChartModel activityChartModel = new IderaLineChartModel();
		List<FileActivity> stats = dataModelMap.get(activity);
		Collections.sort(stats, new Comparator<FileActivity>() {
				                @Override
				                public int compare(FileActivity e1,
				                		FileActivity e2) {
				                	//Descending order. mark e2 and e1
				                    return e1.getUTCCollectionDateTime().compareTo(e2.getUTCCollectionDateTime());
				                }
				            });
		// Todo - fix it to read revers sorted values
		for(FileActivity act : stats){
			//Adding for offset for graphs
			long collectionTime = act.getUTCCollectionDateTime().getTime()+ OFFSET_MILLIS;
			//TODO change to file reads and file writes
			activityChartModel.setValue("Reads/Sec", collectionTime, act.getStatistics().getFileReadsPerSec());
			activityChartModel.setValue("Writes/Sec", collectionTime, act.getStatistics().getFileWritesPerSec());
		}
		
		return activityChartModel;
	}
	
	@Command
	public void filterByFileType(@BindingParam("checked") boolean checked, @BindingParam("picked") FileType fileType){
		if(checked){
			filters.getFileTypes().add(fileType.getName());
		}
		else{
			filters.getFileTypes().remove(fileType.getName());
		}
	}
	
	@Command
	@NotifyChange("filters")
    public void databaseSelected() {
		StringBuilder sb = new StringBuilder();
		for(InstanceDetailsDatabase database : filters.getSelectedDatabases()){
			sb.append(database.getDatabaseName()+",");
		}
		if(sb.length() > 0){
			sb.setLength(sb.length() - 1);
		}
		filters.setDatabaseBoxValue(sb.toString());
    }
	
	@Command
	@NotifyChange("filters")
    public void driveSelected() {
		StringBuilder sb = new StringBuilder();
		for(FileDrive drive : filters.getSelectedDrives()){
			sb.append(drive.getDriveName()+",");
		}
		if(sb.length() > 0){
			sb.setLength(sb.length() - 1);
		}
		filters.setDriveBoxValue(sb.toString());
    }
	
	private void populateDatabaseListBox() {
		try
		{	
        	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
			databasesList = InstanceDetailsDatabaseFacade.getInstanceDetailsDatabases(productInstanceName,instanceId);
			
			if ((databasesList != null) && (databasesList.size() > 0))
			{		
				Collections.sort(databasesList, new Comparator<InstanceDetailsDatabase>() {

					@Override
					public int compare(InstanceDetailsDatabase db1,
							InstanceDetailsDatabase db2) {
						int ret = 0;

						if(db1 != null && db2 != null && db1.getDatabaseName() != null && db2.getDatabaseName() != null) {
							ret = db1.getDatabaseName().toLowerCase().compareTo(db2.getDatabaseName().toLowerCase());
						}
						return ret;
					}
				});
			}
		}
		catch (RestException x)
		{
		}
	}
	
	@Command
	@NotifyChange("fileItems")
	public void filter(){
		refreshModels();
		PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(instanceId, filters);
	}
	
	private void applySorting(){
		final boolean isAsc = "ascending".equalsIgnoreCase(filters.getPickedSortDirection()) ? true : false;
		if(filters.getPickedSortByName().equalsIgnoreCase("fileName")){
			Collections.sort(fileItems, Comparators.getComparator(isAsc, new Comparator<FileActivityModel>() {
				@Override
				public int compare(FileActivityModel o1, FileActivityModel o2) {
					return (o1.getFileName().compareTo(o2.getFileName()));
				}
			}));
		}
		else if(filters.getPickedSortByName().equalsIgnoreCase("readsPerSec")){
			Collections.sort(fileItems, Comparators.getComparator(isAsc, new Comparator<FileActivityModel>() {
				@Override
				public int compare(FileActivityModel o1, FileActivityModel o2) {
					return (o1.getReadsPerSec().compareTo(o2.getReadsPerSec()));
				}
			}));

		}
		else if(filters.getPickedSortByName().equalsIgnoreCase("writesPerSec")){
			Collections.sort(fileItems, Comparators.getComparator(isAsc, new Comparator<FileActivityModel>() {
				@Override
				public int compare(FileActivityModel o1, FileActivityModel o2) {
					return (o1.getWritesPerSec().compareTo(o2.getWritesPerSec()));
				}
			}));
		}
		else{
			Collections.sort(fileItems, Comparators.getComparator(isAsc, new Comparator<FileActivityModel>() {
				@Override
				public int compare(FileActivityModel o1, FileActivityModel o2) {
					return (o1.getDatabaseName().compareTo(o2.getDatabaseName()));
				}
			}));
		}
	}
	
	@Command
	@NotifyChange({"fileItems","filters","pickedSortByName","pickedSortDirection","fileTypeList"})
	public void resetFilters(){
		filters.setFilePathLike("");
		filters.setFileNameLike("");
		filters.setDatabaseBoxValue("");
		filters.setDriveBoxValue("");
		filters.setPickedSortByName(FileActivityFilters.DEFAULT_SORT_BY);
		filters.setPickedSortDirection(FileActivityFilters.DEFAULT_SORT_DIRECTION);
		for(FileType type : fileTypeList){
			type.setChecked(true);
		}
		filters.getFileTypes().add(FileTypes.data.getName());
		filters.getFileTypes().add(FileTypes.log.getName());
		filters.getFileTypes().add(FileTypes.other.getName());
		
		if(filters.getSelectedDatabases() != null){
			filters.getSelectedDatabases().clear();
		}
		if(filters.getSelectedDrives() != null){
			filters.getSelectedDrives().clear();
		}
		fileItems = new ArrayList<>();
		fileItems.addAll(dataModelMap.keySet());
		applySorting();
		PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(instanceId, filters);
	}

	public List<FileType> getFileTypeList() {
		return fileTypeList;
	}



	public void setFileTypeList(List<FileType> fileTypeList) {
		this.fileTypeList = fileTypeList;
	}



	public List<InstanceDetailsDatabase> getDatabasesList() {
		return databasesList;
	}



	public void setDatabasesList(List<InstanceDetailsDatabase> databasesList) {
		this.databasesList = databasesList;
	}

	public List<FileDrive> getAllDrives() {
		return allDrives;
	}

	public void setAllDrives(List<FileDrive> allDrives) {
		this.allDrives = allDrives;
	}

	public List<FileActivityModel> getFileItems() {
		return fileItems;
	}

	public void setFileItems(List<FileActivityModel> fileItems) {
		this.fileItems = fileItems;
	}

	public FileActivityFilters getFilters() {
		return filters;
	}

	public void setFilters(FileActivityFilters filters) {
		this.filters = filters;
	}
}
