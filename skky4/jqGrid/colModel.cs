using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace skky.jqGrid
{
	public class colModel
	{
		[DataMember]
		public string name { get; set; }

		[DataMember]
		public string index { get; set; }

		[DataMember]
		public string align { get; set; }

		[DataMember]
		public bool resizable { get; set; }

		[DataMember]
		public int width { get; set; }

		[DataMember]
		public bool sortable { get; set; }

		[DataMember]
		public bool editable { get; set; }

		[DataMember]
		public string edittype { get; set; }

		[DataMember]
		public string formatter { get; set; }

		private EditOptions editOptions = null;
		[DataMember(IsRequired = false)]
		public EditOptions editoptions
		{
			get
			{
				if (null == editOptions)
					editOptions = new EditOptions();

				return editOptions;
			}
			set
			{
				editOptions = value;
			}
		}

		private FormatOptions formatOptions = null;
		[DataMember(IsRequired = false)]
		public FormatOptions formatoptions
		{
			get
			{
				if (null == formatOptions)
					formatOptions = new FormatOptions();

				return formatOptions;
			}
			set
			{
				formatOptions = value;
			}
		}

		[DataContract]
		public class EditOptions
		{
			[DataMember]
			public string value { get; set; }
		}

		[DataContract]
		public class FormatOptions
		{
			public FormatOptions()
			{
				thousandsSeparator = ",";
				decimalPlaces = 2;
				srcformat = "ISO8601Long";
				newformat = "m/d/Y";
			}

			[DataMember]
			public string thousandsSeparator { get; set; }

			[DataMember]
			public int decimalPlaces { get; set; }
			
			[DataMember]
			public string srcformat { get; set; }
			
			[DataMember]
			public string newformat { get; set; }
		}
	}
}
