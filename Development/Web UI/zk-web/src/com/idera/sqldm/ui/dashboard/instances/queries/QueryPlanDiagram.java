package com.idera.sqldm.ui.dashboard.instances.queries;

import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.Reader;
import java.io.StringReader;
import java.io.StringWriter;
import java.io.Writer;
import java.net.URL;
import java.util.Properties;

import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.stream.StreamResult;
import javax.xml.transform.stream.StreamSource;

import org.apache.commons.lang.StringEscapeUtils;
import org.apache.log4j.Logger;
import org.zkoss.image.AImage;
import org.zkoss.zhtml.Filedownload;
import org.zkoss.zhtml.Span;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Div;
import org.zkoss.zul.Html;
import org.zkoss.zul.Label;

import com.idera.server.web.ELFunctions;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.utils.SQLdmConstants;
import com.lowagie.text.pdf.codec.Base64;

public class QueryPlanDiagram extends SelectorComposer<Component> {

	private static final long serialVersionUID = 1L;

	private final Logger log = Logger.getLogger(QueryPlanDiagram.class);

	@Wire
	private Div divQueryPlan;

	Html htmlCanvas;

	private String xmlString;

	@Override
	public void doAfterCompose(Component comp) throws Exception {
		super.doAfterCompose(comp);
		log.debug("In compose of plan diagram");
		Selectors.wireEventListeners(comp, this);
		xmlString = (String) Sessions.getCurrent()
				.getAttribute("XmlPlanString");

		if (xmlString != null) {

			/*
			 * String xsltPath = GlobalUtil.configDir +
			 * System.getProperty("file.separator") +
			 * ServerConstants.QUERY_PLAN_XSLT_FILE_NAME;
			 */
/*			Properties prop = new Properties();
			String propFileName = "conf/web.properties";
			InputStream input = new FileInputStream(propFileName);
			prop.load(input);

			String xsltPath = prop
					.getProperty(SQLdmConstants.XSLT_PATH_PROPERTY);
*/
// File xsltFile = new File(xsltPath);
//			File xsltFile = new File("metainfo/QueryPlanTransforms.xslt");

//			URL url = new URL("http://localhost:9291/zkau/web/4572ffae/sqldm/com/idera/sqldm/QueryPlanTransforms.xslt");
	
			xmlString = StringEscapeUtils.unescapeXml(xmlString);

			TransformerFactory tf = TransformerFactory.newInstance();
			File xsltFile = new File(SQLdmConstants.XSLT_PATH);
			StreamSource xslt = new StreamSource(xsltFile);
			Transformer tr = tf.newTransformer(xslt);

			Reader inputXML = new StringReader(xmlString);
			StreamSource xml = new StreamSource(inputXML);

			Writer outputWriter = new StringWriter();

			StreamResult output = new StreamResult(outputWriter);

			tr.transform(xml, output);
			String outputString = outputWriter.toString();

			htmlCanvas = new Html(outputString);
			htmlCanvas.setId("htmlQueryPlan");
			divQueryPlan.appendChild(htmlCanvas);
			Clients.evalJavaScript("callDrawLines()");

		}
		else {
			
			String noDataLblValue = ELFunctions
					.getMessage(SQLdmI18NStrings.QUERY_PLAN_DATA_NOT_AVAILABLE);
			Label noDataLbl = new Label();
			noDataLbl.setValue(noDataLblValue);
			divQueryPlan.appendChild(noDataLbl);
		}

	}

	public String getXmlString() {
		return xmlString;
	}

	public void setXmlString(String xmlPlan) {
		this.xmlString = xmlPlan;
	}

	@Listen("onClickSave = #exportLink")
	public void onClickSave(Event event) {

		String dataUrl = (String) event.getData().toString();
		AImage aimage;

		try {
			InputStream stream = new ByteArrayInputStream(Base64.decode(
					dataUrl, Base64.DECODE));

			aimage = new AImage("QueryPlanDiagram.png", stream);
			Filedownload.save(aimage);

		} catch (IOException e) {
			e.printStackTrace();
		}

	}

}