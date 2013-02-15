using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Text.RegularExpressions;

namespace skky.util
{
	public class ExcelColumn
	{
		private string name;
		private int ordinal;
		private Type dataType;

		public ExcelColumn() { }
		public ExcelColumn(string name, Type type)
		{
			this.name = name;
			this.dataType = type;
		}

		public string Name
		{
			get { return name ?? string.Empty; }
			set { name = value; }
		}

		public int Ordinal
		{
			get { return ordinal; }
			set { ordinal = value; }
		}

		public Type DataType
		{
			get { return dataType; }
			set { dataType = value; }
		}

		public string GetExcelDataType()
		{
			if (dataType == typeof(string))
				return "Text";

			if (dataType == typeof(int))
				return "Integer";

			if (dataType == typeof(double) || dataType == typeof(long))
				return "Double";

			if (dataType == typeof(decimal) || dataType == typeof(float))
				return "Float";

			if (dataType == typeof(DateTime))
				return "DateTime";

			if (dataType == typeof(bool))
				return "Boolean";

			return String.Empty;
		}
		//public static string parseSpreadsheet3(string filename)
		//{
		//    string str = string.Empty;

		//    try
		//    {
		//        ApplicationClass excel = new ApplicationClass();
		//        Workbook wb = excel.Workbooks.Open(filename, 0, true, 5, "", "", true, XlPlatform.xlWindows, "", false, false, null, false, false, false);
		//        Worksheet sheet = (Worksheet)wb.Worksheets.get_Item(1);
		//        Range range = sheet.get_Range("A1", "C1");
		//        Object[,] vals = (System.Object[,])range.get_Value(Missing.Value);
		//        long iRows = vals.GetUpperBound(0);
		//        long iCols = vals.GetUpperBound(1);

		//        for (long rowCounter = 1; rowCounter <= iRows; rowCounter++)
		//        {
		//            for (long colCounter = 1; colCounter <= iCols; colCounter++)
		//            {

		//                //Write the next value into the string.
		//                str = String.Concat(str,
		//                   vals[rowCounter, colCounter].ToString() + ", ");
		//            }

		//            //Write in a new line.
		//            str = String.Concat(str, "\n");
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        str += ex.ToString();
		//    }

		//    //ExcelWorkbook workbook = ExcelHelper.OpenWorkbook(fileUploadPath);
		//    //for (int i = 0; i < workbook.SheetNames.Count; i++)
		//    //    str += (i > 0 ? "," : "") + "\"" + workbook.SheetNames[i].ToString() + "\"";               
		//    string msg = "File Uploaded: " + filename;
		//    msg += "<br />";
		//    msg += str;

		//    return msg;
		//}
	}
}
