package com.idera.sqldm.ui.dashboard.instances.queries;

import java.io.ByteArrayOutputStream;
import java.util.List;

import org.apache.log4j.Logger;
import org.zkoss.exporter.pdf.PdfExporter;
import org.zkoss.util.media.AMedia;
import org.zkoss.zhtml.Filedownload;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;
import org.zkoss.zul.Grid;
import org.zkoss.zul.ListModelList;

import com.idera.server.web.ELFunctions;
import com.idera.sqldm.data.queries.QueryStatementColumns;
import com.idera.sqldm.i18n.SQLdmI18NStrings;

@SuppressWarnings("rawtypes")
public class QueryDetailsColumns extends SelectorComposer<Div> {

	private final Logger log = Logger.getLogger(QueryDetailsColumns.class);

	@Wire
	private Grid queryColumnsGrid;
	ListModelList<QueryStatementColumns> queryColumns;

	@SuppressWarnings("unchecked")
	@Override
	public void doAfterCompose(Div comp) throws Exception {

		super.doAfterCompose(comp);

		queryColumnsGrid.setEmptyMessage(ELFunctions
				.getMessage(SQLdmI18NStrings.QUERY_PLAN_DATA_NOT_AVAILABLE));
		
		List<QueryStatementColumns> columnsList = (List<QueryStatementColumns>) Sessions.getCurrent()
		.getAttribute("QueryStatementColumns");
		if (columnsList != null) {
			queryColumns = new ListModelList<>(columnsList);
			queryColumnsGrid.setModel(queryColumns);
		}

	}

	@Listen("onClick = #exportLink")
	public void exportToPdf() {

		ByteArrayOutputStream out = new ByteArrayOutputStream();

		PdfExporter exporter = new PdfExporter();
		try {
			exporter.export(queryColumnsGrid, out);

			AMedia amedia = new AMedia("QueryColumns.pdf", "pdf",
					"application/pdf", out.toByteArray());
			Filedownload.save(amedia);
			out.close();
		} catch (Exception e) {
			e.printStackTrace();
		}

	}

}