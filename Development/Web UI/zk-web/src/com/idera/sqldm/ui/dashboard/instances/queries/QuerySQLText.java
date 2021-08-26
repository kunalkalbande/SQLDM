package com.idera.sqldm.ui.dashboard.instances.queries;

import org.apache.log4j.Logger;
import org.apache.openjpa.lib.jdbc.SQLFormatter;
import org.zkoss.codemirror.Codemirror;
import org.zkoss.util.media.AMedia;
import org.zkoss.zhtml.Filedownload;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;

@SuppressWarnings("rawtypes")
public class QuerySQLText extends SelectorComposer<Div> {

	private final Logger log = Logger.getLogger(QuerySQLText.class);

	private String sqlText;

	@Wire
	private Codemirror codeSqlText;

	@SuppressWarnings("unchecked")
	@Override
	public void doAfterCompose(Div comp) throws Exception {
		super.doAfterCompose(comp);

/*		sqlText = "SELECT CASE WHEN X.Severity = 8 THEN 'Critical' ELSE \n"
				+ "    CASE WHEN X.Severity = 4 THEN 'Warning' ELSE \n"
				+ "    CASE WHEN X.Severity = 2 THEN 'Information' ELSE \n"
				+ "    CASE WHEN X.Severity = 1 THEN 'OK' " + "		END \n"
				+ "	   END " + "	   END " + "	   END, " + "		COUNT ( 0) \n"
				+ "		FROM (SELECT ROW_NUMBER() OVER \n"
				+ "				(PARTITION BY ServerName \n"
				+ "				 ORDER BY Severity DESC) \n" + "				 RowNum1,* \n"
				+ "				 FROM (SELECT ROW_NUMBER() OVER \n"
				+ "					(PARTITION BY ServerName,Severity \n"
				+ "					 ORDER BY UTCOccurrenceDateTime DESC) \n"
				+ "					 RowNum,ServerName,Severity \n"
				+ "					 FROM Alerts A \n"
				+ "					 JOIN MonitoredSQLServers M \n"
				+ "					 ON A.ServerName \n"
				+ "					 COLLATE database_default = M.InstanceName \n"
				+ "					 COLLATE database_default WHERE M.Active = 1 \n"
				+ "				 AND A.ServerName IS NOT NULL) T \n"
				+ "				 WHERE T.RowNum = 1) X \n" + "		WHERE X.RowNum1 = 1 \n"
				+ "		GROUP BY X.Severity;\n";
*/
		sqlText = (String) Sessions.getCurrent().getAttribute("SqlText");
		if(sqlText==null)sqlText="";
		SQLFormatter sqlFormatter = new SQLFormatter();
		sqlFormatter.setMultiLine(true);
		sqlFormatter.setDoubleSpace(true);
		sqlText = (String) sqlFormatter.prettyPrint(sqlText);

		codeSqlText.setValue(sqlText);
	}

	public String getSqlText() {
		return sqlText;
	}

	public void setSqlText(String sqlText) {
		this.sqlText = sqlText;
	}

	@Listen("onClick = #exportLink")
	public void exportToXml() {

		AMedia amedia = new AMedia("SqlText.txt", "txt", "text", sqlText);
		Filedownload.save(amedia);

	}

}