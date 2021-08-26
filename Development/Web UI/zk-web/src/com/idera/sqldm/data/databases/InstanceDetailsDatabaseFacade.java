package com.idera.sqldm.data.databases;

import java.util.Comparator;
import java.util.Date;
import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.category.resources.ResourceCategory;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.rest.SQLDMRestClient;

public class InstanceDetailsDatabaseFacade {

	public static List<InstanceDetailsDatabase> getInstanceDetailsDatabases(String productInstanceName, int instanceId) throws RestException {
		return SQLDMRestClient.getInstance().getInstanceDetailsDatabases(productInstanceName,instanceId);
	}
	
	public static List<DatabaseCapacityUsageDetails> getDatabaseCapacityUsage(String productInstanceName, int instanceId, int dbId) throws RestException {
		return SQLDMRestClient.getInstance().getDatabaseCapacityUsage(productInstanceName,instanceId,dbId);
	}
	
	public static List<TempDBUsageDetails> getTempDBUsageDetails(String productInstanceName, int instanceId, int numHistoryMinutes) throws RestException {
		return SQLDMRestClient.getInstance().getTempDBUsageDetails(productInstanceName,instanceId, numHistoryMinutes);
	}
	
	/*
	 * Author:Accolite
	 * Date : 12th Dec, 2016
	 * History Panel - SQLDM- 10.2 release
	 * Adding History Panel to Database
	 */
	public static List<TempDBUsageDetails> getTempDBUsageDetails(String productInstanceName, int instanceId, long numHistoryMinutes,Date endTime) throws RestException {
		return SQLDMRestClient.getInstance().getTempDBUsageDetails(productInstanceName,instanceId, numHistoryMinutes,endTime);
	}
	
	public static Comparator<InstanceDetailsDatabase> DATABASE_TYPE_ASC = new Comparator<InstanceDetailsDatabase>() {

		@Override
		public int compare(InstanceDetailsDatabase db1,		InstanceDetailsDatabase db2) {
			
			return compareInstnace(db1, db2, true);
		}
	};

	public static Comparator<InstanceDetailsDatabase> DATABASE_TYPE_DESC = new Comparator<InstanceDetailsDatabase>() {

		@Override
		public int compare(InstanceDetailsDatabase db1,		InstanceDetailsDatabase db2) {

			return compareInstnace(db1, db2, false);
		}
	};


	private static int compareInstnace (InstanceDetailsDatabase db1, InstanceDetailsDatabase db2, boolean asc){

		int ret = 0;	

		if(db1 == null ){
			return asc ? -1 : 1;		
		}else if(db2 == null){
			return asc? 1:-1;
		}else{

			ret =  db1.getDatabaseType().toLowerCase().compareTo(db2.getDatabaseType().toLowerCase());

			return asc ? ret : ret*(-1);
		}

	}
}
