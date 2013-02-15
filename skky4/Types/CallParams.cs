using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class CallParams
	{
		//public const string CallParam_ = "";

		public const string CallParam_act = "act";
		public const string Const_All = "All";
		public const string CallParam_Name = "name";
		public const string CallParam_Title = "title";
		public const string CallParam_Type = "type";
		public const string CallParam_Category = "category";
		public const string CallParam_Classification = "classification";
		public const string CallParam_DateRange = "dateRange";
		public const string CallParam_DateKey = "dateKey";
		public const string CallParam_Order = "order";
		public const string CallParam_UseMetric = "useMetric";

		public CallParams()
		{ }

		[DataMember]
		public string act { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public string Type { get; set; }

		[DataMember]
		public string Category { get; set; }
		[DataMember]
		public string Classification { get; set; }

		private DateSettings dateRange;
		[DataMember]
		public DateSettings DateRange
		{
			get
			{
				if (dateRange == null)
					dateRange = new DateSettings();

				return dateRange;
			}
			set
			{
				dateRange = value;
			}
		}
		[DataMember]
		public string DateKey { get; set; }

		[DataMember]
		public int Order { get; set; }

		[DataMember]
		public bool UseMetric { get; set; }


		public DateSettings GetDateRangeIfEmpty(DateSettings ds)
		{
			if ((dateRange == null || dateRange == new DateSettings()) && ds != null)
				return ds;

			return DateRange;
		}
		public void SetDateRangeIfEmpty(DateSettings ds)
		{
			if ((dateRange == null || dateRange == new DateSettings()) && ds != null)
				dateRange = ds;
		}
	}
}
