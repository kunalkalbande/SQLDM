package com.idera.sqldm_10_3.utils;

import com.idera.server.web.ELFunctions;
import org.apache.commons.lang.StringEscapeUtils;
import org.apache.poi.ss.usermodel.Cell;
import org.apache.poi.ss.usermodel.RichTextString;
import org.apache.poi.ss.usermodel.Sheet;
import org.apache.poi.ss.usermodel.Workbook;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.bootstrap.DOMImplementationRegistry;
import org.w3c.dom.ls.DOMImplementationLS;
import org.w3c.dom.ls.LSOutput;
import org.w3c.dom.ls.LSSerializer;
import org.zkoss.exporter.excel.ExcelExporter;
import org.zkoss.exporter.pdf.PdfExporter;
import org.zkoss.util.media.AMedia;
import org.zkoss.zhtml.Filedownload;
import org.zkoss.zul.*;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import java.io.*;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.List;

public class GridExporter {

	private static final String TEMP_FILE = "temp.xlsx";
	private static String METHOD_NAME = "getMap";

	@SuppressWarnings({ "unchecked", "rawtypes" })
	private static Grid getNewGrid(Grid grid, ListModelList<Object> listModel,
			Class beanClass) {
		
		if(listModel.getSize() == 0) {
			try {
				listModel.add(beanClass.getConstructor().newInstance());
			} catch (InstantiationException | IllegalAccessException
					| IllegalArgumentException | InvocationTargetException
					| NoSuchMethodException | SecurityException e) {

				e.printStackTrace();
			}
		}

		Grid newGrid = new Grid();
		Method method;
		try {
			method = beanClass.getMethod(METHOD_NAME, null);

			LinkedHashMap<String, List<String>> map = (LinkedHashMap<String, List<String>>) method
					.invoke(null, null);

			Columns columns = new Columns();
			for (String columnName : map.keySet()) {
				Column column = new Column(ELFunctions.getLabel(columnName));
				columns.appendChild(column);
			}

			Rows rows = new Rows();

			for (Object object : listModel) {
				Row row = new Row();
				for (String labelValue : map.keySet()) {

					ArrayList<String> methodsList = new ArrayList<>(
							map.get(labelValue));
					Object labelObject = object;
					Class labelClass = beanClass;
					for (String methodName : methodsList) {

						Method getterMethod = labelClass.getMethod(methodName,
								null);

						labelObject = getterMethod.invoke(labelObject, null);
						if (labelObject != null)
							labelClass = labelObject.getClass();

					}

					Label label;
					if (labelObject != null)
						label = new Label(labelObject.toString());
					else
						label = new Label();

					row.appendChild(label);
				}
				rows.appendChild(row);
			}

			newGrid.appendChild(columns);
			newGrid.appendChild(rows);

		} catch (Exception e) {
			e.printStackTrace();
		}

		return newGrid;

	}

	/**
	 * Export input Grid data to pdf file
	 * 
	 * @param grid
	 */
	@SuppressWarnings("rawtypes")
	public static void exportToPdf(Grid grid, Class beanClass, String methodName, String fileName) {
		
		METHOD_NAME = methodName;

		ListModelList<Object> listModel = (ListModelList<Object>) grid
				.getModel();
		if(listModel.size() == 0) {
			Messagebox.show("No records to export.");
			return;
		}
		try {

			Grid newGrid = getNewGrid(grid, listModel, beanClass);

			ByteArrayOutputStream dataArray = new ByteArrayOutputStream();
			ByteArrayOutputStream out = new ByteArrayOutputStream();

			PdfExporter exporter = new PdfExporter();
			exporter.exportTabularComponent(newGrid, out);

			dataArray.write(out.toByteArray());

			AMedia amedia = new AMedia(fileName + ".pdf", "pdf",
					"application/pdf", out.toByteArray());
			Filedownload.save(amedia);
			out.close();
			dataArray.close();
		} catch (Exception e) {
			e.printStackTrace();
		}

	}

	/**
	 * Export input Grid data to excel file
	 * 
	 * @param grid
	 */
	@SuppressWarnings("rawtypes")
	public static void exportToExcel(Grid grid, Class beanClass, String methodName, String fileName) {

		METHOD_NAME = methodName;

		ListModelList<Object> listModel = (ListModelList<Object>) grid
				.getModel();
		if(listModel.size() == 0) {
			Messagebox.show("No records to export.");
			return;
		}
		try {

			Grid newGrid = getNewGrid(grid, listModel, beanClass);
			ByteArrayOutputStream out = new ByteArrayOutputStream();

			ExcelExporter exporter = new ExcelExporter();
			exporter.export(newGrid, out);

			AMedia amedia = new AMedia(fileName + ".xlsx", "xls",
					"application/file", out.toByteArray());
			Filedownload.save(amedia);
			out.close();
		} catch (Exception e) {
			e.printStackTrace();
		}

	}

	/**
	 * Export input Grid data to XML file
	 * 
	 * @param grid
	 * @param title
	 *            Grid title
	 * @param subTitle
	 *            Grid subtitle
	 */
	@SuppressWarnings("rawtypes")
	public static void exportToXml(Grid grid, String title, String subTitle,
			Class beanClass, String methodName, String fileName) {

		METHOD_NAME = methodName;

		ListModelList<Object> listModel = (ListModelList<Object>) grid
				.getModel();
		if(listModel.size() == 0) {
			Messagebox.show("No records to export.");
			return;
		}
		try {

			Grid newGrid = getNewGrid(grid, listModel, beanClass);

			DocumentBuilderFactory icFactory = DocumentBuilderFactory
					.newInstance();
			DocumentBuilder icBuilder;

			icBuilder = icFactory.newDocumentBuilder();
			Document doc = icBuilder.newDocument();
			Element sqlDm = doc.createElement("SQLDiagnosticManager");

			// Create root element
			doc.createElement("Idera").appendChild(sqlDm);

			// Append heading if title is not null
			if (title != null)
				sqlDm.appendChild(getHeading(doc, title, subTitle));

			// Append data rows
			doc = getData(newGrid, doc, sqlDm);

			// Format xml
			DOMImplementationRegistry registry = DOMImplementationRegistry
					.newInstance();
			DOMImplementationLS impl = (DOMImplementationLS) registry
					.getDOMImplementation("LS");
			LSSerializer writer = impl.createLSSerializer();
			writer.getDomConfig().setParameter("format-pretty-print",
					Boolean.TRUE);

			LSOutput lsOutput = impl.createLSOutput();
			lsOutput.setEncoding("UTF-8");

			Writer stringWriter = new StringWriter();
			lsOutput.setCharacterStream(stringWriter);

			writer.write(sqlDm, lsOutput);
			String xmlString = stringWriter.toString();

			xmlString = StringEscapeUtils.unescapeXml(xmlString);

			AMedia amedia = new AMedia(fileName + ".xml", "xml", "text/xml",
					xmlString);
			Filedownload.save(amedia);

		} catch (Exception e) {
			e.printStackTrace();
		}

	}

	/**
	 * Generate heading element for xml file
	 * 
	 * @param doc
	 * @param title
	 * @param subTitle
	 * @return
	 */
	private static Node getHeading(Document doc, String title, String subTitle) {

		Element heading = doc.createElement("Heading");

		Element titleNode = doc.createElement("Title");
		titleNode.appendChild(doc.createTextNode(title));
		heading.appendChild(titleNode);

		if (subTitle != null) {
			Element subTitleNode = doc.createElement("SubTitle");
			subTitleNode.appendChild(doc.createTextNode(subTitle));
			heading.appendChild(subTitleNode);
		}

		return heading;
	}

	/**
	 * Create temporary xls file
	 * 
	 * @param grid
	 */
	private static void createTempFile(Grid grid) {

		ByteArrayOutputStream out = new ByteArrayOutputStream();

		ExcelExporter exporter = new ExcelExporter();
		try {
			exporter.export(grid, out);

			OutputStream tempFile = new FileOutputStream(TEMP_FILE);
			tempFile.write(out.toByteArray());
			tempFile.close();
			out.close();
		} catch (Exception e) {
			e.printStackTrace();
		}

	}

	/**
	 * Iterate over data and append to XML DOM
	 * 
	 * @param grid
	 * @param doc
	 * @param sqlDm
	 * @return
	 */
	private static Document getData(Grid grid, Document doc, Element sqlDm) {

		createTempFile(grid);

		try {

			InputStream inputStream = null;
			inputStream = new FileInputStream(TEMP_FILE);

			Workbook workBook;
			workBook = new XSSFWorkbook(inputStream);
			Sheet sheet = workBook.getSheetAt(0);
			Iterator<org.apache.poi.ss.usermodel.Row> rows = sheet
					.rowIterator();

			// Fetching column labels
			List<String> columnLabels = new ArrayList<>();
			org.apache.poi.ss.usermodel.Row row = rows.next();
			Iterator<Cell> cells = row.cellIterator();
			while (cells.hasNext()) {

				Cell cell = cells.next();
				RichTextString richTextString = cell.getRichStringCellValue();
				columnLabels.add(richTextString.getString());

			}

			// Appending data rows to xml DOM
			while (rows.hasNext()) {

				row = rows.next();
				sqlDm.appendChild(appendData(doc, columnLabels, row));

			}
		} catch (IOException e) {
			deleteTempFile();
			e.printStackTrace();
		}

		deleteTempFile();

		return doc;

	}

	/**
	 * Iterate over input row and fetch column values
	 * 
	 * @param doc
	 * @param columnLabels
	 * @param row
	 * @return
	 */
	private static Node appendData(Document doc, List<String> columnLabels,
			org.apache.poi.ss.usermodel.Row row) {

		// Get a row, iterate through cells
		Iterator<Cell> cells = row.cellIterator();
		Element dataNode = null;
		int j = 0;

		if (cells.hasNext())
			dataNode = doc.createElement("Data");

		while (cells.hasNext()) {
			Cell cell = cells.next();

			if (columnLabels.get(j).equals("!")) {
				j++;
				continue;
			}

			String columnLabel = columnLabels.get(j).replaceAll(
					"[^a-zA-Z0-9]+", "");
			Element columnNode = doc.createElement(StringEscapeUtils
					.escapeXml(columnLabel));

			String richTextString = null;

			switch (cell.getCellType()) {
			
			case Cell.CELL_TYPE_BOOLEAN:
				richTextString = cell.getBooleanCellValue() + "";
				break;
			
			case Cell.CELL_TYPE_NUMERIC:
				richTextString = cell.getNumericCellValue() + "";
				break;
			
			case Cell.CELL_TYPE_STRING:
				richTextString = cell.getStringCellValue();
				break;
			}
			
			columnNode.appendChild(doc.createTextNode(StringEscapeUtils
					.escapeXml(richTextString)));

			dataNode.appendChild(columnNode);
			j++;

		}

		return dataNode;

	}

	/**
	 * Delete temporary file created
	 */
	private static void deleteTempFile() {

		File file = new File(TEMP_FILE);
		file.delete();

	}

}
