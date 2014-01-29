using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.Types;

namespace skky.Types
{
    [DataContract]
    public class SkkyCallParams : CallParams
	{
		public const string CallParam_AccountName = "accountName";
		public const string CallParam_AccountNumber = "accountNumber";
		public const string CallParam_AccountId = "accountId";
		public const string CallParam_DepartmentName = "departmentName";
		public const string CallParam_DepartmentId = "departmentId";
		public const string CallParam_EmissionType = "EmissionType";
		public const string CallParam_CityName = "cityName";
		public const string CallParam_CityCode = "cityCode";
		public const string CallParam_CityId = "cityId";
		public const string CallParam_CityCriteria = "cityCriteria";
		public const string CallParam_Equipment = "equipment";
		public const string CallParam_Vendor = "vendor";
		public const string CallParam_ZipCode = "zipCode";

		public const string CallParam_PartName = "partName";
		public const string CallParam_PartId = "partId";
		public const string CallParam_TermName = "termName";
		public const string CallParam_TermId = "termId";
		public const string CallParam_TimeFrame = "timeFrame";
		public const string CallParam_TimePeriod = "timePeriod";
		public const string CallParam_TravelType = "travelType";
		public const string CallParam_BOMItemId = "bomItemId";
		public const string CallParam_PrimaryGrouping = "primaryGrouping";
		public const string CallParam_Grouping = "grouping";

		public const string CallParam_PropertyName = "propertyName";

		public const string CallParam_ReportName = "reportName";
		public const string CallParam_ReportTitle = "reportTitle";
		public const string CallParam_DateTimeTitle = "dateTimeTitle";
		public const string CallParam_DoubleTitle = "doubleTitle";
		public const string CallParam_IntTitle = "intTitle";
		public const string CallParam_Int2Title = "intTitle";
		public const string CallParam_StringTitle = "stringTitle";

		public SkkyCallParams()
		{ }

		[DataMember]
		public string AccountName { get; set; }
		[DataMember]
		public string AccountNumber { get; set; }
		[DataMember]
		public int AccountId { get; set; }

		[DataMember]
		public string DepartmentName { get; set; }
		[DataMember]
		public int DepartmentId { get; set; }

		[DataMember]
		public string CityName { get; set; }
		[DataMember]
		public string CityCode { get; set; }
		[DataMember]
		public int CityId { get; set; }
		[DataMember]
		public string CityCriteria { get; set; }

		[DataMember]
		public string Equipment { get; set; }

		[DataMember]
		public string Vendor { get; set; }

		[DataMember]
		public string ZipCode { get; set; }

		[DataMember]
		public string PartName { get; set; }
		[DataMember]
		public Guid PartId { get; set; }

		[DataMember]
		public string TermName { get; set; }
		[DataMember]
		public Guid TermId { get; set; }

		[DataMember]
		public int BomId { get; set; }

		[DataMember]
		public int BomItemId { get; set; }

		[DataMember]
		public string Grouping { get; set; }
		[DataMember]
		public string PrimaryGrouping { get; set; }

		[DataMember]
		public string TravelType { get; set; }

		[DataMember]
		public string TimeFrame { get; set; }
		[DataMember]
		public string TimePeriod { get; set; }

		[DataMember]
		public string PropertyName { get; set; }
		[DataMember]
		public string SourceName { get; set; }

		[DataMember]
		public string ReportName { get; set; }
		[DataMember]
		public string ReportTitle { get; set; }
		[DataMember]
		public string DateTimeTitle { get; set; }
		[DataMember]
		public string DoubleTitle { get; set; }
		[DataMember]
		public string IntTitle { get; set; }
		[DataMember]
		public string Int2Title { get; set; }
		[DataMember]
		public string StringTitle { get; set; }

		//-DD 1/19/13 - Need to have access to UserWebSettings and ChartSettings.
		[DataMember]
		public List<Guid> SelectedSources { get; set; }

		[DataMember]
		public bool ShowAllSources { get; set; }

		public List<Guid> GetSelectedSources()
		{
			if (ShowAllSources || (null != SelectedSources && SelectedSources.Count() < 1))
				return null;

			return SelectedSources;
		}
	}
}
