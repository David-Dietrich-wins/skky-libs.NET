using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace skky.util
{
	public class ExcelWorkbook
	{
		public enum FileType {
			CSV,
			XLS,
			XLSX,
		};

		// First row is the header. And look at all rows for the Metadata.
		//private static readonly string CONST_ExcelDataSourceRead = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=\"{0}\"; Extended Properties='Excel 8.0;HDR=YES;IMEX=1';";
		//private static readonly string CONST_ExcelDataSourceWrite = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=\"{0}\"; Extended Properties='Excel 8.0';";
		//private static readonly string CONST_ExcelDataSource = "Driver={Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)};DBQ=";

		private static readonly string CONST_ACEProvider = "Provider=Microsoft.ACE.OLEDB.12.0;";
		private static readonly string CONST_JetProvider = "Provider=Microsoft.Jet.OLEDB.4.0;";

		//String connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Projects\data.xlsx;Extended Properties='Excel 12.0 Xml;HDR=YES';";

		// From http://stackoverflow.com/questions/1139390/excel-external-table-is-not-in-the-expected-format
		//
		//xls
		//private const string connstring = "Provider=Microsoft.Jet.OLEDB.4.0;" & "Data Source=" + path + ";Extended Properties=""Excel 8.0;HDR=YES;""";
		//xlsx
		//private const string connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=Excel 12.0;";

		//private static readonly string CONST_ExcelCSVDataSource = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"{0}\";Extended Properties=\"Excel 8.0;\"";
		private static readonly string CONST_ExcelCSVDataSourceBase = CONST_JetProvider + "Data Source=\"{0}\";";
		private static readonly string CONST_ExcelCSVDataSourceRead = CONST_ExcelCSVDataSourceBase + "Extended Properties=\"Text;HDR=Yes;FMT=CSVDelimited;\"";
		private static readonly string CONST_ExcelCSVDataSourceWrite = CONST_ExcelCSVDataSourceBase + "Extended Properties=\"Text;HDR=Yes;FMT=CSVDelimited;\"";
		//"Extended Properties=\"text;HDR=No;FMT=Delimited(;)\"";
//		private static readonly string CONST_ExcelCSVDataSourceWrite = CONST_ExcelCSVDataSourceBase + "Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
		//private static readonly string CONST_ExcelCSVDataSource = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"{0}\";Extended Properties=\"text;HDR=Yes;FMT=Delimited\"";

		private static readonly string CONST_ExcelXLSXDataSourceWrite = CONST_ACEProvider + "Data Source=\"{0}\";Extended Properties=\"Excel 12.0 XML;HDR=YES;\"";
		private static readonly string CONST_ExcelXLSXDataSourceRead = CONST_ACEProvider + "Data Source=\"{0}\";Extended Properties=\"Excel 12.0;IMEX=1;HDR=YES;\"";
		private static readonly string CONST_ExcelXLSDataSourceWrite = CONST_ACEProvider + "Data Source=\"{0}\";Extended Properties=\"Excel 8.0;\"";
		private static readonly string CONST_ExcelXLSDataSourceRead = CONST_ACEProvider + "Data Source=\"{0}\";Extended Properties=\"Excel 8.0;IMEX=1;HDR=YES;\"";
		//private static readonly string CONST_ExcelDataSource = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"{0}\";Extended Properties=\"Excel 14.0;IMEX=1;HDR=YES;\"";
		//private static readonly string CONST_ExcelDataSource = "Driver={{Microsoft Excel Driver (*.xls)}};DBQ=\"{0}\";Readonly=False";
		//** Provider=Microsoft.Jet.OLEDB.4.0; Data Source=foo.xls; Extended Properties=Excel 8.0;"				

		private string filename = string.Empty;
		private bool isCSVFile = false;

		private bool isSchemaLoaded = false;

		private bool Writable = false;

		private List<string> sheetNames = new List<string>();
		private List<ExcelSpreadsheet> sheets = new List<ExcelSpreadsheet>();
		private OleDbConnection excelConnection = null;

		public ExcelWorkbook()
			: this(string.Empty)
		{ }
		public ExcelWorkbook(string file)
			: this(file, false)
		{ }
		public ExcelWorkbook(string file, bool writable)
		{
			Filename = file;
			Writable = writable;
		}
		/*
		public ExcelWorkbook(string file, OleDbConnection oledbConnection)
		{
			Filename = file;
			excelConnection = oledbConnection;
		}
		*/
		/** begin properties **/
		/** ---------------- **/
		public string Filename
		{
			get { return filename ?? String.Empty; }
			set
			{
				string newFile = (value ?? string.Empty);
				isCSVFile = false;
				if (newFile.EndsWith(".csv"))
					isCSVFile = true;

				filename = newFile;
			}
		}

		public List<string> SheetNames
		{
			get { return sheetNames; }
		}

		public List<ExcelSpreadsheet> Sheets
		{
			get { return sheets; }
		}

		public bool IsCSV
		{
			get { return isCSVFile; }
			set { isCSVFile = value; }
		}

		public OleDbConnection ExcelConnection
		{
			get { return excelConnection; }
			set { excelConnection = value; }
		}
		/** end properties **/
		/** -------------- **/

		public static bool isCSV(string fileName)
		{
			if (!string.IsNullOrEmpty(fileName) && fileName.ToLower().EndsWith(".csv"))
				return true;

			return false;
		}
		public static bool isXLS(string fileName)
		{
			if (!string.IsNullOrEmpty(fileName) && fileName.ToLower().EndsWith(".xls"))
				return true;

			return false;
		}
		public static bool isXLSX(string fileName)
		{
			if (!string.IsNullOrEmpty(fileName) && fileName.ToLower().EndsWith(".xlsx"))
				return true;

			return false;
		}

		public static ExcelWorkbook Open(string fileName)
		{
			return Open(fileName, false);
		}
		public static ExcelSpreadsheet Open(string fileName, string sheetName)
		{
			ExcelWorkbook workbook = ExcelWorkbook.Open(fileName);
			if (workbook != null)
			{
				ExcelSpreadsheet sheet = workbook.LoadSheet(string.IsNullOrEmpty(sheetName) ? "sheet1" : sheetName);
				return sheet;
			}

			return null;
		}
        public static ExcelSpreadsheet GetFirstSheet(string excelFileName, string sheetStartsWith = null)
        {
            ExcelWorkbook workbook = ExcelWorkbook.Open(excelFileName);
            if (workbook != null)
            {
                return workbook.LoadFirstSheet(sheetStartsWith);
            }

            return null;
        }
        public ExcelSpreadsheet GetFirstSheet()
		{
			return LoadFirstSheet();
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint GetShortPathName(
			string lpszLongPath, StringBuilder lpszShortPath, uint cchBuffer);

		private static string GetShortPathFilenameWithPath(FileInfo file)
		{
			StringBuilder buffer = new StringBuilder();
			string path = file.FullName;
			if (!File.Exists(path))
			{
				FileStream fs = File.Create(path);
				if (null != fs)
					fs.Close();
			}
			uint length = GetShortPathName(path, buffer, 0);
			if (length > 0)
			{
				buffer.Capacity = (int)length;
				GetShortPathName(path, buffer, length);
				return buffer.ToString();
			}

			return string.Empty;
		}
		public static string GetShortFilename(string fileName)
		{
			FileInfo fi = new FileInfo(fileName);
			if (null != fi)
			{
				string path = GetShortPathFilenameWithPath(fi);
				if (!string.IsNullOrEmpty(path))
				{
					fi = new FileInfo(path);
					if (null != fi)
						return fi.Name;
				}
			}

			return string.Empty;
		}
		public string GetShortFilename()
		{
			return GetShortFilename(Filename);
		}

		public OleDbConnection GetOpenConnection()
		{
			if (IsOpen())
				return excelConnection;

			excelConnection = null;

			if (string.IsNullOrEmpty(Filename))
				return null;

			FileInfo fi = null;

			//** if the file doesn't exist, and we dont want to create the file, return a blank connection
			if (!File.Exists(Filename) && !Writable)
				return null;

			//var constring = CONST_ExcelDataSource + "\"" + fileName + "\"";
			//var constring = string.Format(create ? CONST_ExcelDataSourceWrite : CONST_ExcelDataSourceRead, fileName);
			var constring = string.Format(Writable ? CONST_ExcelXLSDataSourceWrite : CONST_ExcelXLSDataSourceRead, Filename);
			//** substitute the filename into the default connection string

			bool isCSV = Filename.EndsWith(".csv");
			bool isXLSX = Filename.EndsWith(".xlsx");
			if (isCSV)
			{
				fi = new FileInfo(Filename);
				constring = string.Format(Writable ? CONST_ExcelCSVDataSourceWrite : CONST_ExcelCSVDataSourceRead, fi.DirectoryName);
			}
			else if (isXLSX)
			{
				constring = string.Format(Writable ? CONST_ExcelXLSXDataSourceWrite : CONST_ExcelXLSXDataSourceRead, Filename);
			}

			//			if (!File.Exists(fileName) && create)
			//				File.Create(fileName);
			//** try and open a connection to the spreadsheet
			var newcon = new OleDbConnection(constring);
			try
			{
				newcon.Open();
				excelConnection = newcon;
				return newcon;
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical(ex);
				throw ex;
			}
		}
		public static ExcelWorkbook Open(string fileName, bool writable)
		{
			//** initialize the excel workbook
			var book = new ExcelWorkbook(fileName, writable);
			if (null != book)
			{
				var con = book.GetOpenConnection();

				if (book.IsOpen())
				{
					book.LoadSchema();
				}
			}

			return book;
		}

		public static string GetSheetNamesString(string filename)
		{
			string str = string.Empty;

			ExcelWorkbook workbook = Open(filename);
			for (int i = 0; i < workbook.SheetNames.Count; i++)
				str += (i > 0 ? "," : "") + (workbook.SheetNames[i] == null ? "" : workbook.SheetNames[i].ToString().WrapInQuotes());

			return str;
		}
		public static string GetSheetNamesAndValues(string fileName)
		{
			return GetSheetNamesAndValues(fileName, null);
		}
		public static string GetSheetNamesAndValues(string fileName, string sheetName)
		{
			string str = string.Empty;

			ExcelSpreadsheet sheet = Open(fileName, sheetName);
			if (sheet != null)
			{
				bool commaWritten = false;
				foreach (var col in sheet.Columns)
				{
					col.DataType = Type.GetType("String");
					str += (commaWritten ? "," : "Column Names: ") + col.Name ?? "No Name";
					commaWritten = true;
				}
				str += "<br />";

				foreach (DataRow row in sheet.MyDataTable.Rows)
				{
					commaWritten = false;

					foreach (var r in row.ItemArray)
					{
						str += (commaWritten ? "," : "Column: ") + (r == null ? "No Row Value" : r.ToString());
						commaWritten = true;
					}

					str += "<br />";
				}
			}

			return str;
		}

		/** begin methods **/
		/** ------------- **/
		public bool IsOpen()
		{
			if (null != excelConnection)
				return excelConnection.State == ConnectionState.Open;

			return false;
		}

		public void Open()
		{
			GetOpenConnection();
			//** try opening the connection
			if (!IsOpen())
			{
				throw new Exception("Trying to Open a workbook without first setting the connection.");
			}
			else
			{
				if (excelConnection.State != ConnectionState.Open)
					excelConnection.Open();
			}
		}

		public void Close()
		{
			if (null != excelConnection)
			{
				//** close the connection if not already done
				if (excelConnection.State != ConnectionState.Closed)
					excelConnection.Close();
			}
		}

		public static string GetCSVName(string fileName)
		{
			FileInfo fi = new FileInfo(fileName);
			if (fi != null)
			{
				string sheetName = fi.Name;
				var name = Regex.Replace(sheetName, "[.]", "#");
				sheetName = name;
				//sheetName = name.Left(64);

				return sheetName;
			}

			return fileName;
		}

		public ExcelSpreadsheet GetSheet(string sheetName)
		{
			string s = string.Empty;
			//** format/correct the sheetname
			sheetName = ExcelSpreadsheet.FormatName(sheetName);

			//** return the sheet with the given name
			foreach (var sheet in this.Sheets)
			{
				s = sheet.Name.ToLower();
				if (s == sheetName.ToLower() || s == sheetName.ToLower().WrapInSingleQuotes())
					return sheet;
			}

			return null;
		}

		/*

		public ExcelSpreadsheet LoadSheet(string sheetName)
		{
			//** format/correct the sheetname
			ExcelSpreadsheet sheet = GetSheet(sheetName);
			if (sheet != null)
				sheet.Load();

			return sheet;
		}
		 */
		public ExcelSpreadsheet FindSheet(string sheetName)
		{
			foreach (var sheet in Sheets)
			{
				if(sheet.Name == sheetName)
					return sheet;
			}

			return null;
		}
		public ExcelSpreadsheet LoadSheet(string sheetName)
		{
			var sheet = FindSheet(sheetName);
			if(null == sheet)
			{
				sheet = new ExcelSpreadsheet(this);
				sheet.Name = sheetName;
				sheet.LoadSchema();

				Sheets.Add(sheet);
			}

			return sheet;
		}
		private ExcelSpreadsheet LoadFirstSheet(string sheetStartsWith = null)
		{
			string[] excludeSuffixes = {
										   "_FilterDatabase"
									   };
			int i = 0;
			string sheetNameToLoad = string.Empty;

			EnsureSchemaLoaded();

            for (i = 0; i < SheetNames.Count(); ++i)
            {
				string sheetName = SheetNames[i];
				skky.util.Trace.MethodInformation(this.GetType().Name, "LoadFirstSheet", "Sheet #" + (i + 1) + ": " + sheetName + ".");
            }

			string sheetStartsWithLower = (sheetStartsWith ?? string.Empty).ToLower();
            for (i = 0; i < SheetNames.Count() && string.IsNullOrEmpty(sheetNameToLoad); ++i)
            {
                string name = SheetNames[i];
                string nameLower = name.ToLower().Replace("'", string.Empty);

                if (nameLower.StartsWith(sheetStartsWithLower))
                {
					if (null != excludeSuffixes && excludeSuffixes.Length > 0)
					{
						foreach (var excludeSuffix in excludeSuffixes)
						{
							if (!name.EndsWith(excludeSuffix))
							{
								sheetNameToLoad = name;
								break;
							}
						}
					}
                }
			}

			// Take the forst on if nothing came up.
			if (string.IsNullOrEmpty(sheetNameToLoad) && SheetNames.Count() > 0)// && string.IsNullOrEmpty(sheetStartsWith))
			{
				sheetNameToLoad = SheetNames[0];
			}

			if (string.IsNullOrEmpty(sheetNameToLoad))
				return null;

			return LoadSheet(sheetNameToLoad);
		}
		/*
				public void LoadSchema()
				{
					DataTable schemaTable;

					//** make sure the connection is open
					if (!this.IsOpen())
						this.Open();

					try
					{
						//** get the schema of the workbook's tables (sheets)
						schemaTable = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
					}
					catch
					{
						this.Close();
						throw new Exception("[Excelworkbook.LoadSchema] There was an error loading the workbook's schema.");
					}

					try
					{
						//** add all the sheetnames to the workbook info
						var sheetName = string.Empty;
						foreach (DataRow dr in schemaTable.Rows)
						{
							//** get the name of the current sheet/table
							sheetName = dr["TABLE_NAME"].ToString();

							//** since ole will always have 2 forms of the same sheet (with and without the dollar sign), only pay attention to the columns to the sheets ending in $
							if (sheetName.IndexOf("$") == -1)
								continue;

							//** create and load the sheet object
							ExcelSpreadsheet sheet = new ExcelSpreadsheet(this);
							sheet.Name = sheetName;

							//** load its table structure
							sheet.LoadSchema();

							//** add the sheet and sheetname to the workbook object				
							this.Sheets.Add(sheet);
							this.SheetNames.Add(sheet.Name);
						}
					}
					catch
					{
						throw new Exception("[Excelworkbook.LoadSchema] There was an error loading the workbook's data.");
					}
					finally
					{
						this.Close();
					}
				}
		*/
		public void EnsureSchemaLoaded()
		{
			if (!isSchemaLoaded)
				LoadSchema();
		}
		private void LoadSchema()
		{
			//if (string.IsNullOrEmpty(filename))
			//    throw new Exception("LoadSchemaForCSV() was not provided with a filename.");

			sheetNames.Clear();
			sheets.Clear();

			try
			{
				string csvSheetName = string.Empty;
				if(IsCSV)
					csvSheetName = GetCSVName(filename);

				var ole = GetOpenConnection();
				DataTable dtSchema = ole.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
				foreach (DataRow row in dtSchema.Rows)
				{
					string tableName = row["TABLE_NAME"].ToString();
					if (!string.IsNullOrEmpty(tableName))
					{
						if (IsCSV)
						{
							if (tableName == csvSheetName || tableName == (csvSheetName + "$"))
							{
								string sheetName = GetShortFilename();

								sheetNames.Add(sheetName);
							}
						}
						else
						{
							//if(!isCSV || tableName.EndsWith("$"))
							sheetNames.Add(tableName);
						}
					}
				}

				isSchemaLoaded = true;
			}
			catch (Exception ex)
			{
				isSchemaLoaded = false;
				throw new Exception("[ExcelSpreadsheet.LoadSchema] There was a problem loading the spreadsheet \"" + filename + "\".", ex);
			}
			finally
			{
				this.Close();
			}
		}

		public ExcelSpreadsheet CreateSheet(string sheetName, DataTable sourceTable)
		{
			if (IsCSV)
				return null;

			// No such thing as sheets in CSV.
			var columnList = new List<ExcelColumn>();

			//** make a column list from the table
			foreach (DataColumn col in sourceTable.Columns)
			{
				columnList.Add(new ExcelColumn
				{
					Name = col.ColumnName,
					DataType = col.DataType,
					Ordinal = col.Ordinal
				});
			}

			//** create the sheet with the new column list, importing the source table
			return this.CreateSheet(sheetName, columnList, sourceTable);
		}
		public ExcelSpreadsheet CreateSheet(string sheetName, DataColumnCollection columns)
		{
			if (IsCSV)
				return null;

			var columnList = new List<ExcelColumn>();

			//** create excelcolumns from all the datacolumns 
			foreach (DataColumn col in columns)
			{
				columnList.Add(new ExcelColumn
				{
					Name = col.ColumnName,
					DataType = col.DataType,
					Ordinal = col.Ordinal
				});
			}

			//** create the sheet with the new column list
			return this.CreateSheet(sheetName, columnList);
		}
		public ExcelSpreadsheet CreateSheet(string sheetName, List<ExcelColumn> columns)
		{
			return this.CreateSheet(sheetName, columns, null);
		}
		public ExcelSpreadsheet CreateSheet(string sheetName, List<ExcelColumn> columns, DataTable importData)
		{
			if (IsCSV)
				return null;

			//** make sure we have a valid sheetname
			if (string.IsNullOrEmpty(sheetName))
				throw new Exception("[ExcelWorkbook.CreateSheet] SheetName cannot be null or empty");

			//** make sure the sheetname doesn't already exist
			ExcelSpreadsheet sheet = GetSheet(sheetName);
			if (sheet != null)
				throw new Exception("[ExcelWorkbook.CreateSheet] A Spreadsheet already exists with the name \"" + sheetName + "\"");

			if (columns.Count == 0)
				throw new Exception("[ExcelWorkbook.CreateSheet] Cannot Create a sheet with zero columns.");

			//** verify the connection is open
			if (!this.IsOpen())
				this.Open();

			try
			{
				//** create the query to make the table
				var query = "create table [" + sheetName.Replace("$", "") + "] (";
				var buf = "";
				foreach (ExcelColumn col in columns)
					buf += (buf == "" ? "" : ",") + col.Name.WrapInBrackets() + " " + col.GetExcelDataType();
				query += buf + ")";

				//** create the table/spreadsheet
				var cmd = new OleDbCommand(query, excelConnection);
				cmd.ExecuteNonQuery();

				//** create the spreadsheet object
				sheet = new ExcelSpreadsheet(this);
				sheet.Name = sheetName;

				//** load its schema
				sheet.LoadSchema();

				//** add the sheet to the workbook
				sheetNames.Add(sheet.Name);
				sheets.Add(sheet);

				//** import the given table if it has data
				if (importData != null)
					sheet.ImportData(importData);
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical(ex);
				throw new Exception("[ExcelWorkbook.DeleteSheet] There was an error creating the sheet \"" + sheetName + "\". " + ex.ToString());
			}
			finally
			{
				//** close the connection				
				this.Close();
			}

			//** return the new sheet object
			return sheet;
		}

		/* Note: this doesn't actually delete the sheet in the workbook, just completely blanks it out */
		public void DeleteSheet(string sheetName)
		{
			//** make sure the sheet exists...
			if (this.GetSheet(sheetName) == null)
				throw new Exception("[ExcelWorkbook.DeleteSheet] Sheet named \"" + sheetName + "\" does not exist.");

			//** format/correct the sheetname
			sheetName = ExcelSpreadsheet.FormatName(sheetName);

			//** setup the db objects
			var query = "drop table " + sheetName.WrapInBrackets();
			var cmd = new OleDbCommand(query, excelConnection);

			//** make sure the workbook is open
			if (!this.IsOpen())
				this.Open();

			try
			{
				//** remove the table
				cmd.ExecuteNonQuery();
			}
			catch
			{
				throw new Exception("[ExcelWorkbook.DeleteSheet] There was an error removing the sheet \"" + sheetName + "\".");
			}
			finally
			{
				//** release the connection to the workbook				
				this.Close();
			}

			//** rebuild the schema collections
			var newNames = new List<string>();
			var newSheets = new List<ExcelSpreadsheet>();

			//** filter our deleted sheet
			foreach (ExcelSpreadsheet sheet in sheets)
			{
				if (sheet.Name.ToLower() != sheetName.ToLower())
				{
					newNames.Add(sheet.Name);
					newSheets.Add(sheet);
				}
			}

			//** reset the collection
			sheets = newSheets;
			sheetNames = newNames;
		}
	}
}
