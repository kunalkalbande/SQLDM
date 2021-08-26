package com.idera.sqldm.ui.dashboard.instances.queries;

import java.lang.reflect.Method;
import java.util.Comparator;
import java.util.Date;

import com.idera.sqldm.data.queries.QueryApplication;
import com.idera.sqldm.data.queries.QueryApplicationDetails;
import com.idera.sqldm.data.queries.QueryClients;
import com.idera.sqldm.data.queries.QueryDatabases;
import com.idera.sqldm.data.queries.QueryUsers;

public class QueriesComparator implements Comparator<QueryApplicationDetails> {

	private String sortBy;
	private boolean ascending;

	public QueriesComparator(String sortBy, boolean ascending) {
		this.sortBy = sortBy;
		this.ascending = ascending;
	}

	@SuppressWarnings({ "unchecked", "rawtypes" })
	@Override
	public int compare(QueryApplicationDetails o1, QueryApplicationDetails o2) {
		int v = 0;

		try {
			Class classObject = QueryApplicationDetails.class;
			String methodName = "get" + Character.toUpperCase(sortBy.charAt(0))
					+ sortBy.substring(1);
			Method getterMethod = classObject.getMethod(methodName);

			Object value1 = getterMethod.invoke(o1, null);
			Object value2 = getterMethod.invoke(o2, null);

			Object fieldType = getterMethod.getReturnType();

			if (QueryApplication.class.equals(fieldType)) {

				v = ((QueryApplication) value1).getApplication().compareToIgnoreCase(
						((QueryApplication) value2).getApplication());

			} else if (QueryDatabases.class.equals(fieldType)) {

				v = ((QueryDatabases) value1).getDatabase().compareToIgnoreCase(
						((QueryDatabases) value2).getDatabase());

			} else if (QueryClients.class.equals(fieldType)) {

				v = ((QueryClients) value1).getClient().compareToIgnoreCase(
						((QueryClients) value2).getClient());

			} else if (QueryUsers.class.equals(fieldType)) {

				v = ((QueryUsers) value1).getUser().compareToIgnoreCase(
						((QueryUsers) value2).getUser());

			} else if (fieldType.equals(String.class)) {

				v = ((String) value1).compareToIgnoreCase((String) value2);

			} else if (fieldType.equals(Date.class)) {

				v = ((Date) value1).compareTo((Date) value2);

			} else if (fieldType.equals(int.class)) {

				v = (int) value1 - (int) value2;

			} else if (fieldType.equals(long.class)) {

				if ((long) value1 == (long) value2)
					v = 0;
				else if ((long) value1 > (long) value2)
					v = 1;
				else
					v = -1;

			} else if (fieldType.equals(boolean.class)) {

				v = (boolean) value1.equals((boolean) value2) ? 0
						: ((boolean) value1.equals(true) ? (-1) : 1);

			} else {

				v = ((Double) value1).compareTo((Double) value2);

			}
		} catch (Exception e) {
			e.printStackTrace();
		}

		return ascending ? v : -v;
	}

	public String getSortBy() {
		return sortBy;
	}

	public void setSortBy(String sortBy) {
		this.sortBy = sortBy;
	}

	public boolean isAscending() {
		return ascending;
	}

	public void setAscending(boolean ascending) {
		this.ascending = ascending;
	}
}
