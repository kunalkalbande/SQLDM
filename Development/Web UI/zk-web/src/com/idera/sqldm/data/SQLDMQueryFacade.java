package com.idera.sqldm.data;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import com.idera.common.rest.RestException;
import com.idera.i18n.I18NStrings;
import com.idera.sqldm.data.queries.AggregateFilteredInstancesContainer;
import com.idera.sqldm.data.queries.AggregateFilteredInstancesProperties;
import com.idera.sqldm.data.queries.AggregatedFilter;
import com.idera.sqldm.data.queries.FilteredInstanceDatabasesContainer;
import com.idera.sqldm.data.queries.FilteredInstanceDatabasesProperties;
import com.idera.sqldm.data.queries.FilteredInstancesContainer;
import com.idera.sqldm.data.queries.FilteredInstancesProperties;
import com.idera.sqldm.data.queries.SQLDMQueryResultContainer;
import com.idera.sqldm.rest.SQLDMRestClient;

public class SQLDMQueryFacade {
	
	public static enum ViewBy {
		INSTANCE_COUNT(I18NStrings.INSTANCE_COUNT, "instance count"),
		DATABASE_COUNT(I18NStrings.DATABASE_COUNT, "database count"), 
		DATA_SIZE(I18NStrings.DATA_SIZE_IN_MB, "data size"),
		LOG_SIZE(I18NStrings.LOG_SIZE_IN_MB, "log size"),
		ACTIVITY(I18NStrings.ACTIVITY, "activity");
		
		String i18nKey;
		String elementsString;
		
		ViewBy(String i18nKey, String elementsString){
			this.i18nKey = i18nKey;
			this.elementsString = elementsString;
		}
		
		public String getI18nKey(){
			return i18nKey;
		}
		
		public String getElementsString(){
			return elementsString;
		}
		
		public static int getOrdinal(String value){
			for (ViewBy tmp: ViewBy.values()){
				if (tmp.toString().equals(value))
					return tmp.ordinal();
			}
			return -1;
		}
		
		public static ViewBy fromString(String value){
			for (ViewBy tmp: ViewBy.values()){
				if (tmp.toString().equals(value))
					return tmp;
			}
			return null;
		}
	}
	
	public static enum GroupBy {
		INSTANCE(I18NStrings.INSTANCE, I18NStrings.INSTANCES),
		OWNER(I18NStrings.OWNER, I18NStrings.OWNERS),
		LOCATION(I18NStrings.LOCATION, I18NStrings.LOCATIONS),
		TAG(I18NStrings.TAG, I18NStrings.TAGS),
		VERSION(I18NStrings.SERVER_VERSION, I18NStrings.SERVER_VERSIONS),
		DATABASE(I18NStrings.DATABASE, I18NStrings.DATABASES);
		
		String i18nKey;
		String i18nKeyPlural;
		
		GroupBy(String message, String plural){
			this.i18nKey = message;
			this.i18nKeyPlural = plural;
		}
		
		public String getI18nKey(){
			return i18nKey;
		}
		
		public String getI18nKeyPlural(){
			return i18nKeyPlural;
		}
		
		public static int getOrdinal(String value){
			for (GroupBy tmp: GroupBy.values()){
				if (tmp.toString().equals(value))
					return tmp.ordinal();
			}
			return -1;
		}
		
		public static GroupBy fromString(String value){
			for (GroupBy tmp: GroupBy.values()){
				if (tmp.toString().equals(value))
					return tmp;
			}
			return null;
		}
	}
	
	public static AggregatedFilter getAggregatedRequestFilter(ViewBy viewBy, GroupBy groupBy, Map<FilterTypes, List<String>> filterMap, int maxCount){
		AggregatedFilter requestFilter = new AggregatedFilter();
		
		if (filterMap.containsKey(FilterTypes.INSTANCES)){
			requestFilter.setFilterInstanceNames(filterMap.get(FilterTypes.INSTANCES));
		}
		
		if (filterMap.containsKey(FilterTypes.LOCATIONS)){
			requestFilter.setFilterLocations(filterMap.get(FilterTypes.LOCATIONS));
		}
		
		if (filterMap.containsKey(FilterTypes.OWNERS)){
			requestFilter.setFilterOwners(filterMap.get(FilterTypes.OWNERS));
		}
		
		if (filterMap.containsKey(FilterTypes.TAGS)){
			requestFilter.setFilterTags(filterMap.get(FilterTypes.TAGS));
		}
		
		if (filterMap.containsKey(FilterTypes.VERSIONS)){
			requestFilter.setFilterVersions(filterMap.get(FilterTypes.VERSIONS));
		}
		
		requestFilter.setGroupBy(groupBy.toString());
		requestFilter.setMaxCount(maxCount);
		requestFilter.setSortBy(viewBy.getElementsString());
		
		return requestFilter;
	}
	
	public static SQLDMQueryResultContainer getDataGroupedByWithFilterList(String productInstanceName,ViewBy viewBy, GroupBy groupBy, Map<FilterTypes, List<String>> filterMap, int maxCount) throws SQLDMQueryException{
		
		AggregatedFilter requestFilter = getAggregatedRequestFilter(viewBy, groupBy, filterMap, maxCount);
		
		try {
			switch(groupBy){
				case INSTANCE:
					return getTranslateFromFilteredInstances(groupBy, SQLDMRestClient.getInstance().getFilteredInstances(productInstanceName,requestFilter));
				case DATABASE:
					return getTranslateFromFilteredInstanceDatabases(groupBy, SQLDMRestClient.getInstance().getFilteredInstanceDatabases(productInstanceName,requestFilter));
				case OWNER:
				case LOCATION:
				case TAG:
				case VERSION:
					return getTranslatedFromAggregateFilteredInstances(groupBy, SQLDMRestClient.getInstance().getAggregateFilteredInstances(productInstanceName,requestFilter));
				default:
						// unknown enum type, new enum was added but not defined here .. it's an internal error.
						return new SQLDMQueryResultContainer();
			}
		}
		catch (RestException e) {
			throw new SQLDMQueryException(e);
		}
	}
	
	protected static SQLDMQueryResultContainer getTranslatedFromAggregateFilteredInstances(GroupBy groupBy, AggregateFilteredInstancesContainer aggregateFilteredInstancesContainer){
		SQLDMQueryResultContainer returnContainer = new SQLDMQueryResultContainer();
		
		if (aggregateFilteredInstancesContainer.getAggregateFilteredInstancesProperties() == null || aggregateFilteredInstancesContainer.getAggregateFilteredInstancesProperties().isEmpty())
			return returnContainer;
		
		List<SQLDMQueryResult> queryResultList = new ArrayList<SQLDMQueryResult>();
		
		for(AggregateFilteredInstancesProperties tmp: aggregateFilteredInstancesContainer.getAggregateFilteredInstancesProperties()){
			
			SQLDMQueryResult result = new SQLDMQueryResult(); 
			
			result.setName(tmp.getName());
			result.setInstanceCount(tmp.getInstanceCount());
			result.setDatabaseCount(tmp.getDatabaseCount());
			result.setTotalDataSize(tmp.getTotalDataSize());
			result.setTotalLogSize(tmp.getTotalLogSize());
			result.setMainEntry(groupBy);
			if (!GroupBy.DATABASE.equals(groupBy))
				result.setDrillable(true);
			result.setTransactionsPerSec(tmp.getTransactionsPerSec());
			
			switch (groupBy){
			case OWNER:
			case LOCATION:
			case VERSION:
			case TAG:
				result.setHasDetails(false);
				result.setDrillable(true);
				break;
			case DATABASE:
				result.setHasDetails(true);
				result.setDrillable(false);
				break;
			default:
				result.setHasDetails(true);
				result.setDrillable(true);
			}
			
			queryResultList.add(result);
		}
		
		returnContainer.setQueryResultList(queryResultList);
		returnContainer.setTotalNumberOfEntries(aggregateFilteredInstancesContainer.getTotalNumberOfEntries());
		return returnContainer;
	}
	
	protected static SQLDMQueryResultContainer getTranslateFromFilteredInstanceDatabases(GroupBy groupBy, FilteredInstanceDatabasesContainer filteredInstanceDatabasesContainer){
		SQLDMQueryResultContainer returnContainer = new SQLDMQueryResultContainer();
		
		if (filteredInstanceDatabasesContainer.getFilteredInstanceDatabasesProperties() == null || filteredInstanceDatabasesContainer.getFilteredInstanceDatabasesProperties().isEmpty())
			return returnContainer;
		
		List<SQLDMQueryResult> queryResultList = new ArrayList<SQLDMQueryResult>();
		
		for(FilteredInstanceDatabasesProperties tmp: filteredInstanceDatabasesContainer.getFilteredInstanceDatabasesProperties()){
			SQLDMQueryResult result = new SQLDMQueryResult(); 
			result.setName(tmp.getName());
			result.setInstanceName(tmp.getInstanceName());
			// @author Saumyadeep
			result.setFriendlyServerName(tmp.getFriendlyServerName());
			result.setDisplayName(tmp.getDisplayName());
			
			result.setDatabaseSize(tmp.getDatabaseSize());
			result.setTransactionsPerSec(tmp.getTransactionsPerSec());
			result.setLogSize(tmp.getLogSize());
			result.setDataSize(tmp.getDataSize());
			result.setMainEntry(groupBy);
			result.setId(tmp.getId());
			result.setDrillable(false);
			result.setAvailability(tmp.getAvailability());
			result.setHasDetails(true);
			result.setEnabled(tmp.isEnabled());
			
			queryResultList.add(result);
		}
		
		returnContainer.setQueryResultList(queryResultList);
		returnContainer.setTotalNumberOfEntries(filteredInstanceDatabasesContainer.getTotalNumberOfEntries());
		return returnContainer;
	}
	
	protected static SQLDMQueryResultContainer getTranslateFromFilteredInstances(GroupBy groupBy, FilteredInstancesContainer filteredInstancesContainer){
		SQLDMQueryResultContainer returnContainer = new SQLDMQueryResultContainer();
		
		if (filteredInstancesContainer.getFilteredInstancesProperties() == null || filteredInstancesContainer.getFilteredInstancesProperties().isEmpty())
			return returnContainer;
		
		List<SQLDMQueryResult> queryResultList = new ArrayList<SQLDMQueryResult>();
		
		for(FilteredInstancesProperties tmp: filteredInstancesContainer.getFilteredInstancesProperties()){
			SQLDMQueryResult result = new SQLDMQueryResult(); 
			result.setName(tmp.getName());
			result.setResponseTime(tmp.getResponseTime());
			result.setSqlVersion(tmp.getSqlVersion() == null ? "" : tmp.getSqlVersion());
			result.setDatabaseCount(tmp.getDatabaseCount());
			result.setTotalDataSize(tmp.getTotalDataSize());
			result.setTransactionsPerSec(tmp.getTransactionsPerSec());
			result.setEnabled(tmp.getEnabled());
			result.setMainEntry(groupBy);
			result.setTotalLogSize(tmp.getTotalLogSize());
			result.setId(tmp.getId());
			result.setDrillable(true);
			result.setAvailability(tmp.getAvailability());
			result.setHasDetails(true);
			result.setInstanceState(tmp.getInstanceState());
			
			queryResultList.add(result);
		}
		
		returnContainer.setQueryResultList(queryResultList);
		returnContainer.setTotalNumberOfEntries(filteredInstancesContainer.getTotalNumberOfEntries());
		return returnContainer;
	}

}
