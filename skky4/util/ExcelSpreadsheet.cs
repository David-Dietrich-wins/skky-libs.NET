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
	public class ExcelSpreadsheet
	{
		private string name;
		private List<string> columnNames = new List<string>();
		private List<ExcelColumn> columns = new List<ExcelColumn>();
		private DataTable myDataTable;
		private ExcelWorkbook workbook;

		public ExcelSpreadsheet(ExcelWorkbook book)
		{
			workbook = book;
		}

		/** begin properties **/
		/** ---------------- **/
		public ExcelWorkbook Workbook
		{
			get { return workbook; }
		}

		public string Name
		{
			get { return name ?? String.Empty; }
			set { name = value; }
		}

		public List<string> ColumnNames
		{
			get { return columnNames; }
		}

		public List<ExcelColumn> Columns
		{
			get { return columns; }
		}

		public DataTable MyDataTable
		{
			get {
//				if (null == MyDataTable)
//					LoadRows();

				return myDataTable;
			}
			set { myDataTable = value; }
		}
		/** end properties **/
		/** -------------- **/

		/** begin methods **/
		/** ------------- **/
		public static string FormatName(string sheetName)
		{
			if (!string.IsNullOrEmpty(sheetName) && sheetName.EndsWith("$"))
				return sheetName;

			//** replace non alpha/numeric characters
			var name = Regex.Replace(sheetName ?? string.Empty, "[^a-zA-Z0-9_-]", "");

			return name + "$";
		}

		public DataTable GetSchemaDataTable()
		{
			//** create an empty datatable
			var table = new DataTable();

			//** add all the sheets columns to the datatable, building an empty table with sheet's schema that we can add data to
			foreach (ExcelColumn col in this.Columns)
				table.Columns.Add(new DataColumn(col.Name, col.DataType));

			return table;
		}

		public void ClearColumns()
		{
			//** reset the object
			columnNames.Clear();
			columns.Clear();
		}

		private DataTable LoadSchema(DataTable dtSchema)
		{
			ClearColumns();

			if (null != dtSchema)
			{
				//** build each column
				foreach (DataRow cdr in dtSchema.Rows)
				{
					string colName = (string)cdr["ColumnName"];
					int colOrdinal = (int)cdr["ColumnOrdinal"];
					Type colDataType = (Type)cdr["DataType"];
					ExcelColumn column = new ExcelColumn(colName, colDataType);
					column.Ordinal = colOrdinal;

					ColumnNames.Add(column.Name);
					Columns.Add(column);
				}
			}

			return dtSchema;
		}

		private string GetSelectTop1()
		{
			string str = "SELECT TOP 1 * FROM ";
			str += Name.WrapInBrackets();

			return str;
		}

		public void LoadSchema()
		{
			ClearColumns();

			try
			{
				//** pull the top row out of the current sheet to read its shema
				using (OleDbCommand cmd = new OleDbCommand(GetSelectTop1(), workbook.GetOpenConnection()))
				{
					//** get the tables schema
					using (var reader = cmd.ExecuteReader())
					{
						DataTable colSchema = reader.GetSchemaTable();

						LoadSchema(colSchema);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("[ExcelSpreadsheet.LoadSchema] There was a problem loading the spreadsheet \"" + this.Name + "\".", ex);
			}
		}

		public DataTable LoadRows(bool TrimStrings = true)
		{
            int numRows = 0;

			MyDataTable = null;

			if (null == Workbook)
				throw new Exception("NULL workbook in ExcelSpreadsheet.LoadRows().");

			try
			{
				Workbook.EnsureSchemaLoaded();

				//** get an empty table with the sheet's schema
				DataTable table = GetSchemaDataTable();

				//** build a query to get all this sheet's data
				var query = "SELECT * FROM " + this.Name.WrapInBrackets();

                skky.util.Trace.MethodInformation(this.GetType().Name, "LoadRows", query);

				DataRow row;
				Object value;

				//** execute the query, get a datareader
				using (OleDbCommand cmd = new OleDbCommand(query, Workbook.GetOpenConnection()))
				{
					using (var reader = cmd.ExecuteReader())
					{
                        skky.util.Trace.MethodInformation(this.GetType().Name, "LoadRows", "Beginning to Read Worksheet.");
                        //** add all the data to the datatable
						var blank = false;
						while (reader.Read())
						{
                            ++numRows;
							//** get a new row for the datatable
							row = table.NewRow();
							blank = true;

							foreach (ExcelColumn col in this.Columns)
							{
								//** make sure at least one column has data, otherwise, dont import the row
								if (!reader.IsDBNull(col.Ordinal))
									blank = false;

								//** get the current columns value
								value = reader.GetValue(col.Ordinal);

								if (TrimStrings)
								{
									//** trim any string value
									if (col.DataType == typeof(String))
										value = value.ToString().Trim();
								}

								//** set the column's value
								row[col.Ordinal] = value;
							}

							//** add the row to the table
							if (!blank)
								table.Rows.Add(row);
						}
					}
				}

                skky.util.Trace.MethodInformation(this.GetType().Name, "LoadRows", "Completed Reading of " + numRows + " rows in worksheet.");

				//** set the local data table for cached access
				MyDataTable = table;
				return table;
			}
			catch(Exception ex)
			{
				throw new Exception("[ExcelSpreadsheet.Load] There was a problem loading the spreadsheet \"" + this.Name + "\".", ex);
			}
			finally
			{
				Workbook.Close();
			}
		}

		public int ImportData(DataTable importData)
		{
			DataColumn dc, dc2;
			DataRow dr;
			OleDbCommand cmd;

			//** get this sheets schema table for column set verification
			var schemaTable = GetSchemaDataTable();

			//** do a basic column count verification
			if (schemaTable.Columns.Count != importData.Columns.Count)
				throw new Exception("[ExcelSpreadsheet.ImportData] Invalid Column Schema, use ExcelSpreadsheet.GetSchemaTable as a base import table.");

			//** now dig a little deepper; compare names and datatypes
			for (var i = 0; i < schemaTable.Columns.Count; i++)
			{
				dc = schemaTable.Columns[i];
				dc2 = importData.Columns[i];

				//** validate the name...
				if (dc2.ColumnName != dc.ColumnName)
					throw new Exception("[ExcelSpreadsheet.ImportData] Found invalid column name \"" + dc2.ColumnName + "\" at ordinal " + i + ".");

				//** validate the datatype...
				//if (dc2.DataType != dc.DataType)
				//	throw new Exception("[ExcelSpreadsheet.ImportData] Mismatch in datatype for column \"" + dc.ColumnName + "\". Expected type \"" + dc.DataType + "\".");
			}

			//** build a prototype query string we will substitute our values into
			var querybase = "insert into " + this.Name.WrapInBrackets() + " (";
			for (var i = 0; i < schemaTable.Columns.Count; i++)
			{
				dc = schemaTable.Columns[i];
				querybase += (i == 0 ? "" : ",") + dc.ColumnName.WrapInBrackets();
			}
			querybase += ") values($values$)";

			//** make sure the workbooks connection is open
			if (!workbook.IsOpen())
				workbook.Open();

			//** now that we're speaking the same language, proceed with the data import		
			var buf = string.Empty;
			var query = string.Empty;
			for (var i = 0; i < importData.Rows.Count; i++)
			{
				dr = importData.Rows[i];
				buf = string.Empty;

				//** build the value list for this record
				for (var j = 0; j < dr.ItemArray.Length; j++)
				{
					dc = schemaTable.Columns[j];
					buf += (j == 0 ? "" : ",");

					if (dc.DataType == typeof(string) || dc.DataType == typeof(DateTime))
						buf += dr.ItemArray[j].ToString().Replace("'", "''").WrapInSingleQuotes();
					else if(null != dr.ItemArray[j] && dr.ItemArray[j].GetType() == typeof(System.DBNull))
						buf += "NULL";
					else
						buf += (dr.ItemArray[j] == null ? "NULL" : dr.ItemArray[j]);
				}

				//** substitute it into the querybase resulting in an insert statement
				query = querybase.Replace("$values$", buf);

				//** run the query
				cmd = new OleDbCommand(query, workbook.ExcelConnection);
				cmd.ExecuteNonQuery();
			}

			//** close the connection
			workbook.Close();

			return importData.Rows.Count;
		}

		public bool WriteCSVfile2(DataTable sourceTable)
		{
			if (string.IsNullOrEmpty(Name))
				Name = ExcelWorkbook.GetShortFilename(workbook.Filename);

			string sql = @"SELECT * FROM " + Name.WrapInBrackets();

			using (OleDbConnection conn = workbook.GetOpenConnection())
			{
				using (OleDbCommand cmd = new OleDbCommand(sql, conn))
				{
					// fill the data to a datatable in memory.
					OleDbDataAdapter adp = new OleDbDataAdapter(cmd);
					DataTable dt = new DataTable();
					adp.Fill(dt);

					// write data to csv file using StreamWriter.
					using (StreamWriter writer = new StreamWriter(workbook.Filename))
					{
						if (dt.Rows.Count > 0)
						{
							foreach (DataRow dr in dt.Rows)
							{
								StringBuilder sb = new StringBuilder();
								sb.Append(dr[0].ToString());
								sb.Append(";");
								sb.Append(dr[1].ToString());
								sb.Append(";\n");

								writer.Write(sb.ToString());
							}
						}
						else
						{
							Console.WriteLine("datatable contains no rows.");
						}
					}
				}
			}

			return true;
		}

		public static bool WriteCSVfileFromDataTable(string csvFilename, DataTable dt, bool writeHeader)
		{
			//			using(var fs = new FileStream(workbook.Filename, FileMode.OpenOrCreate);
			if (string.IsNullOrEmpty(csvFilename))
				throw new Exception("ExcelSpreadsheet.WriteCSVfile() received an empty filename.");

			if (null != dt)
			{
				// write data to csv file using StreamWriter.
				using (StreamWriter writer = new StreamWriter(csvFilename))
				{
					string sline;

					if (writeHeader && dt.Columns.Count > 0)
					{
						List<string> colNames = new List<string>();
						foreach (DataColumn col in dt.Columns)
						{
							colNames.Add(col.ColumnName);
						}

						sline = string.Join(",", colNames.ToArray());
						writer.Write(sline + "\n");
					}

					if (null != dt.Rows && dt.Rows.Count > 0)
					{
						foreach (DataRow dr in dt.Rows)
						{
							List<string> rowItems = new List<string>();
							foreach (var obj in dr.ItemArray)
							{
								string str = obj.ToString();
								rowItems.Add(str);
							}
							sline = string.Join(",", rowItems.ToArray());
							writer.Write(sline + "\n");
							//StringBuilder sb = new StringBuilder();
							//sb.Append(dr[0].ToString());
							//sb.Append(",");
							//sb.Append(dr[1].ToString());
							//sb.Append("\n");

							//writer.Write(sb.ToString());
						}
					}
					else
					{
						Console.WriteLine("datatable contains no rows.");
					}
						/*
										foreach (DataRow row in dt.Rows)
										{
											string str = string.Empty;
											for (int i = 0; i < row.Table.Rows.Count; ++i)
											{
												if (i > 0)
													str += ",";

												str += row.Table.Rows[i].ToString();
											}
					
										}
						 */
				}
			}
			/*
						string sql = @"SELECT * FROM " + Name.WrapInBrackets();

						OleDbConnection conn = workbook.GetOpenConnection();
						{
							using (OleDbCommand cmd = new OleDbCommand(sql, conn))
							{
								// fill the data to a datatable in memory.
								OleDbDataAdapter adp = new OleDbDataAdapter(cmd);
								if (null == adp)
								{
									return false;
								}
								else
								{
									//DataSet ds = new DataSet();
									//adp.Fill(ds);
									DataTable dt = new DataTable();
									adp.Fill(dt);

									// write data to csv file using StreamWriter.
									using (StreamWriter writer = new StreamWriter(csvFilename))
									{
										if (dt.Rows.Count > 0)
										{
											foreach (DataRow dr in dt.Rows)
											{
												StringBuilder sb = new StringBuilder();
												sb.Append(dr[0].ToString());
												sb.Append(";");
												sb.Append(dr[1].ToString());
												sb.Append(";\n");

												writer.Write(sb.ToString());
											}
										}
										else
										{
											Console.WriteLine("datatable contains no rows.");
										}
									}
								}
							}
						}
						*/
			return true;
		}

		public bool WriteCSVfile(string csvFilename)
		{
//			using(var fs = new FileStream(workbook.Filename, FileMode.OpenOrCreate);
			if (string.IsNullOrEmpty(csvFilename))
				throw new Exception("ExcelSpreadsheet.WriteCSVfile() received an empty filename.");

			DataTable dt = LoadRows();
			return WriteCSVfileFromDataTable(csvFilename, dt, true);
		}
	}
}
