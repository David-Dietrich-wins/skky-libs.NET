using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using System.Drawing;
using System.Web.UI;

/// <summary>
/// Generiert eine Excel-Datei
/// </summary>
public sealed class ExcelFileResult : FileResult
{
//	const string CONST_ExportType = "application/ms-excel";
//	const string CONST_ExportType = "application/vnd.ms-excel";
	const string CONST_ExportType = "application/x-ms-excel";

	private DataTable dt;
	private TableStyle tableStyle;
	private TableItemStyle headerStyle;
	private TableItemStyle itemStyle;

	/// <summary>
	/// Z.Bsp. "Exportdatum: {0}" (Standard-Initialisierung) - wenn leerer String, wird Exportdatum
	/// nicht angegeben.
	/// </summary>
	public string TitleExportDate { get; set; }
	/// <summary>
	/// Titel des Exports, wird im Sheet oben links ausgegeben
	/// </summary>
	public string Title { get; set; }


	/// <summary>
	/// Konstruktor
	/// </summary>
	/// <param name="dt">Die zu exportierende DataTable</param>
	public ExcelFileResult(DataTable dt)
		: this(dt, null, null, null)
	{ }

	/// <summary>
	/// Konstruktor
	/// </summary>
	/// <param name="dt">Die zu exportierende DataTable</param>
	/// <param name="tableStyle">Styling für gesamgte Tabelle</param>
	/// <param name="headerStyle">Styling für Kopfzeile</param>
	/// <param name="itemStyle">Styling für die einzelnen Zellen</param>
	public ExcelFileResult(DataTable dt, TableStyle tableStyle, TableItemStyle headerStyle, TableItemStyle itemStyle)
		: base(CONST_ExportType)
	{
		this.dt = dt;
		TitleExportDate = "Export Date: {0}";
		this.tableStyle = tableStyle;
		this.headerStyle = headerStyle;
		this.itemStyle = itemStyle;

		// provide defaults
		
		//if (this.tableStyle == null)
		//{
		//    this.tableStyle = new TableStyle();
		//    this.tableStyle.BorderStyle = BorderStyle.Solid;
		//    this.tableStyle.BorderColor = Color.Black;
		//    this.tableStyle.BorderWidth = Unit.Parse("2px");
		//}
		//if (this.headerStyle == null)
		//{
		//    this.headerStyle = new TableItemStyle();
		//    this.headerStyle.BackColor = Color.LightGray;
		//}
	}


	protected override void WriteFile(HttpResponseBase response)
	{
		// Create HtmlTextWriter
		StringWriter sw = new StringWriter();
		HtmlTextWriter tw = new HtmlTextWriter(sw);

		// Build HTML Table from Items
		if (tableStyle != null)
			tableStyle.AddAttributesToRender(tw);
		tw.RenderBeginTag(HtmlTextWriterTag.Table);

		// Create Title Row
		//tw.RenderBeginTag(HtmlTextWriterTag.Tr);
		//tw.AddAttribute(HtmlTextWriterAttribute.Colspan, (dt.Columns.Count - 2).ToString());
		//tw.RenderBeginTag(HtmlTextWriterTag.Td);
		//tw.Write(Title);
		//tw.RenderEndTag();
		//tw.AddAttribute(HtmlTextWriterAttribute.Colspan, "2");
		//tw.RenderBeginTag(HtmlTextWriterTag.Td);
		//if (!string.IsNullOrEmpty(TitleExportDate))
		//    tw.WriteLineNoTabs(string.Format(TitleExportDate, DateTime.Now.ToString("MM.dd.yyyy")));
		//tw.RenderEndTag();
		//tw.RenderEndTag();	// </tr>

		// Create Title Row if Title is not empty
		if (!string.IsNullOrEmpty(Title))
		{
			tw.RenderBeginTag(HtmlTextWriterTag.Tr);
			tw.RenderBeginTag(HtmlTextWriterTag.Td);
			//tw.RenderBeginTag(HtmlTextWriterTag.Th);
			//tw.RenderBeginTag(HtmlTextWriterTag.Strong);
			tw.WriteLineNoTabs(Title);
			//tw.RenderEndTag();
			tw.RenderEndTag();
			tw.RenderEndTag();
		}

		// Create Header Row
		tw.RenderBeginTag(HtmlTextWriterTag.Tr);
		DataColumn col = null;
		for (Int32 i = 0; i <= dt.Columns.Count - 1; i++)
		{
			col = dt.Columns[i];
			if (headerStyle != null)
				headerStyle.AddAttributesToRender(tw);
			tw.RenderBeginTag(HtmlTextWriterTag.Td);
			//tw.RenderBeginTag(HtmlTextWriterTag.Th);
			//tw.RenderBeginTag(HtmlTextWriterTag.Strong);
			tw.WriteLineNoTabs(col.ColumnName);
			//tw.RenderEndTag();
			tw.RenderEndTag();
		}
		tw.RenderEndTag();

		// Create Data Rows
		foreach (DataRow row in dt.Rows)
		{
			tw.RenderBeginTag(HtmlTextWriterTag.Tr);
			for (Int32 i = 0; i <= dt.Columns.Count - 1; i++)
			{
				if (itemStyle != null)
					itemStyle.AddAttributesToRender(tw);
				tw.RenderBeginTag(HtmlTextWriterTag.Td);
				tw.WriteLineNoTabs(HttpUtility.HtmlEncode(row[i]));
				tw.RenderEndTag();
			}
			tw.RenderEndTag(); //  /tr
		}

		tw.RenderEndTag(); //  /table

		// Write result to output-stream
		Stream outputStream = response.OutputStream;
		byte[] byteArray = Encoding.Default.GetBytes(sw.ToString());
		response.OutputStream.Write(byteArray, 0, byteArray.GetLength(0));
	}
}
