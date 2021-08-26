package com.idera.sqldm.server.web.component.zul.grid;

import java.util.List;

import org.zkoss.zk.ui.Component;
import org.zkoss.zul.Column;
import org.zkoss.zul.Grid;

public class GridUtil {

	public static void resetColumnSort(Grid grid, int sortByColumnNumber) {

		List<Component> columns = grid.getColumns().getChildren();

		// reset all column sorting
		for (Component component : columns) {

			Column column = (Column) component;

			if (column != null) {
				column.setSortDirection("natural");
			}
		}

		// set ascending sort direction for the passed column
		Column sortByColumn = (Column) columns.get(sortByColumnNumber);
		sortByColumn.setSortDirection("ascending");
	}
}

