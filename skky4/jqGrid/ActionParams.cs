using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace skky.jqGrid
{
	[DataContract]
	public class ActionParams
	{
		public const string CONST_ActionAdd = "add";
		public const string CONST_ActionDelete = "del";
		public const string CONST_ActionEdit = "edit";

		[DataContract]
		public class Rule
		{
			[DataMember]
			public string field { get; set; }
			[DataMember]
			public string op { get; set; }
			[DataMember]
			public string data { get; set; }
		};

		[DataContract]
		public class Filter
		{
			[DataMember]
			public string groupOp { get; set; }
			[DataMember]
			public Rule[] rules { get; set; }

			public static Filter Create(string jsonData)
			{
				try
				{
					var serializer =
					  new DataContractJsonSerializer(typeof(Filter));
					System.IO.StringReader reader =
					  new System.IO.StringReader(jsonData);
					System.IO.MemoryStream ms =
					  new System.IO.MemoryStream(
					  Encoding.Default.GetBytes(jsonData));
					return serializer.ReadObject(ms) as Filter;
				}
				catch
				{
					return null;
				}
			}
		};

		[DataMember]
		public bool _search { get; set; }
	
		[DataMember]
		public long nd { get; set; }
		
		[DataMember]
		public int page { get; set; }
		
		[DataMember]
		public int rows { get; set; }
		
		[DataMember]
		public string searchField { get; set; }
		
		[DataMember]
		public string searchOper { get; set; }
		
		[DataMember]
		public string searchString { get; set; }
	
		[DataMember]
		public string sidx { get; set; }

		[DataMember]
		public string sord { get; set; }

		private Filter _Filter = null;
		public Filter theFilter
		{
			get
			{
				if (null == _Filter)
					_Filter = new Filter();

				return _Filter;
			}
			set
			{
				_Filter = value;
			}
		}

		private string _filters;
		[DataMember]
		public string filters
		{
			get
			{
				return _filters;
			}
			set
			{
				_filters = value;
				_Filter = Filter.Create(_filters);
			}
		}

		//[DataMember]
		//public Filter filters2
		//{
		//    get
		//    {
		//        if (null == _filters)
		//            _filters = new Filter();

		//        return _filters;
		//    }
		//    set
		//    {
		//        _filters = value;
		//    }
		//}
		//public string filters { get; set; }

		public bool Fixup()
		{
			bool neededFixup = false;
			if ("\"null\"" == sidx || "null" == sidx || "'null'" == sidx)
			{
				sidx = null;
				neededFixup = true;
			}

			return neededFixup;
		}
	}
}
