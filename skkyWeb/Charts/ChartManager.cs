using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.util;
using System.Drawing;
using skky.db;
using skky.Conversions;
using skkyWeb.Security;
using skky.Types;
using skky.web;
using System.Web.UI;

namespace skkyWeb.Charts
{
	public class ChartManager
	{
		public static readonly Color Const_DefaultSeriesColorMain = Color.FromArgb(0x99, 0x99, 0x99);
		public static readonly Color Const_DefaultSeriesColorBorder = Color.FromArgb(0xcc, 0xcc, 0xcc);
		public static readonly Color Const_DefaultMarkerColor = Color.White;
		public static readonly Color Const_DefaultMarkerBorderColor = Color.Black;

		public static readonly Color colorHotelLight = Color.FromArgb(0xc6, 0xcc, 0xee);
		public static readonly Color colorHotelDark = Color.FromArgb(0x33, 0x44, 0xaa);

		public const string Const_DefaultHelpIconLocation = "/img/help-green.16x16.jpg";

		public const string Const_All = "All";

		public const string Const_Department = "Department";

		public const int CONST_YAxisOffsetQuantity = 14;
		public const int CONST_YAxisOffsetLeadTime = 24;

		/*
		public static readonly Color colorStartCityLight = Color.FromArgb(0xcc, 0xff, 0xcc);
		public static readonly Color colorStartCityDark = Color.FromArgb(0x33, 0xff, 0x33);
		public static readonly Color colorEndCityLight = Color.FromArgb(0xff, 0xcc, 0xcc);
		public static readonly Color colorEndCityDark = Color.FromArgb(0xff, 0x33, 0x33);
		public static readonly Color colorHotelLight = Color.FromArgb(0xc6, 0xcc, 0xee);
		public static readonly Color colorHotelDark = Color.FromArgb(0x33, 0x44, 0xaa);

		public static readonly Color[] clrGreenSteps = new Color[]
		{
			Color.FromArgb(0xe2, 0xee, 0xe1),
			Color.FromArgb(0xcc, 0xe1, 0xcb),
			Color.FromArgb(0xb7, 0xd5, 0xb6),
			Color.FromArgb(0xa3, 0xca, 0xa2),
			Color.FromArgb(0x8e, 0xbf, 0x8e),
			Color.FromArgb(0x7b, 0xb5, 0x7d),
			Color.FromArgb(0x67, 0xac, 0x6b),
			Color.FromArgb(0x52, 0xa4, 0x5a),
			Color.FromArgb(0x37, 0x9c, 0x4a),
			Color.FromArgb(0x03, 0x95, 0x3f),
		};
		public static readonly Color[] clrYellowSteps = new Color[]
		{
			Color.FromArgb(0xff, 0xfd, 0xe8),
			Color.FromArgb(0xff, 0xfb, 0xd5),
			Color.FromArgb(0xff, 0xfa, 0xc1),
			Color.FromArgb(0xff, 0xf9, 0xad),
			Color.FromArgb(0xff, 0xf7, 0x99),
			Color.FromArgb(0xff, 0xf6, 0x84),
			Color.FromArgb(0xff, 0xf5, 0x6c),
			Color.FromArgb(0xff, 0xf3, 0x50),
			Color.FromArgb(0xff, 0xf3, 0x29),
			Color.FromArgb(0xff, 0xf2, 0x00),
		};
		public static readonly Color[] clrRedSteps = new Color[]
		{
			Color.FromArgb(0xfe, 0xe6, 0xdc),
			Color.FromArgb(0xfc, 0xd2, 0xc1),
			Color.FromArgb(0xfb, 0xbd, 0xa7),
			Color.FromArgb(0xf9, 0xaa, 0x8f),
			Color.FromArgb(0xf6, 0x96, 0x79),
			Color.FromArgb(0xf5, 0x84, 0x65),
			Color.FromArgb(0xf3, 0x70, 0x52),
			Color.FromArgb(0xf1, 0x5a, 0x40),
			Color.FromArgb(0xef, 0x40, 0x2f),
			Color.FromArgb(0xed, 0x1c, 0x24),
		};

		public const string Const_Airline = "Airline";
		public const string Const_City = "City";

		public const string Const_All = "All";

		public const string Const_Department = "Department";
		public const string Const_EndCity = "End City";
		public const string Const_StartCity = "Start City";

		public const string Const_PlaneType = "Plane Type";
		public const string Const_Vendor = "Vendor";
		public const string Const_InZipCode = "In Zip Code";
		public const string Const_ZipCode = "Zip Code";

		public const string Const_Air = "Air";
		public const string Const_Car = "Car";
		public const string Const_Hotel = "Hotel";
		public const string Const_Plane = "Plane";
		public const string Const_Travel = "Travel";

		public const string Const_CarbonEmittingCities = "Highest Carbon Emitting Cities";

		//private delegate void funcName(ChartManager cm);
		//private static List<KeyValuePair<string, funcName>> charts = new List<KeyValuePair<string, funcName>>();

		public const string Const_AirByStartCity = "AirByStartCity";
		public const string Const_AirByEndCity = "AirByEndCity";
		public const string Const_AirByPlaneType = "AirByPlaneType";
		public const string Const_AirForCitiesByPlaneType = "AirForCitiesByPlaneType";
		public const string Const_AirForCitiesByVendor = "AirForCitiesByVendor";
		public const string Const_AirOverTime = "AirOverTime";
		public const string Const_AirEmissionTrends = "AirEmissionTrends";
		public const string Const_AirByDepartments = "AirByDepartments";
		public const string Const_HotelByCity = "HotelByCity";
		public const string Const_HotelByDepartments = "HotelByDepartments";
		public const string Const_HotelByVendor = "HotelByVendor";
		public const string Const_HotelByZipCodes = "HotelByZipCodes";
		public const string Const_HotelInZipCode = "HotelInZipCode";
		public const string Const_HotelOverTime = "HotelOverTime";
		public const string Const_TravelByCity = "TravelByCity";
		public const string Const_TravelByDepartment = "TravelByDepartment";
		public const string Const_TravelByDepartments = "TravelByDepartments";
		public const string Const_TravelOverTime = "TravelOverTime";
		public const string Const_aPie = "aPie";
		public const string Const_aSpline = "aSpline";
		*/
		public ChartManager(string name, bool isMetric)
		{
			Name = name;
			IsMetric = isMetric;
		}
		public ChartManager(bool isMetric)
		{
			IsMetric = isMetric;
		}

		public ChartManager(ChartSettings cs, bool isMetric)
		{
			ChartSettings = cs;
			IsMetric = isMetric;
		}

		protected bool IsMetric { get; set; }

		private skkyChartBase getNewChartBuilder()
		{
			//return new DundasChartBuilder(ChartSettings);
			return new skkyChartBase(ChartSettings);
		}

		private skkyChartBase chart;
		public skkyChartBase getChart()
		{
			if (chart == null)
			{
				if (SeriesDataList == null || SeriesDataList.Count() < 1)
					FillData();

				if (SeriesDataList != null && SeriesDataList.Count() > 0)
				{
					skkyChartBase cb = getNewChartBuilder();

					if(SeriesDataList.Count() > 0)
					{
						if(SeriesDataList[0].PaletteCustomColors != null)
							cb.SetPaletteCustomColors(SeriesDataList[0].PaletteCustomColors);
					}

					cb.init(SeriesDataList);

					chart = cb;
				}
				//throw new Exception("You must add series data before attempting to retrieve a chart.");
			}

			return chart;
		}

		public bool HasData()
		{
			return (SeriesDataList != null && SeriesDataList.Count() > 0);
		}
		public bool SettingsCanBuildAChart()
		{
			if (!string.IsNullOrEmpty(Name))
				return true;

			return (!string.IsNullOrEmpty(ChartSettings.EmissionType)
				&& !string.IsNullOrEmpty(ChartSettings.TravelType));
		}

		public List<SeriesData> SeriesDataList { get; set; }
		protected void SetSeriesData(SeriesData dsMain)
		{
			SetSeriesData(dsMain, null);
		}
		protected void SetSeriesData(SeriesData dsMain, SeriesData dsSecondary)
		{
			List<SeriesData> sd = new List<SeriesData>();
			sd.Add(dsMain);
			if (dsSecondary == null)
			{
				// Change single item bar charts to blue background.
				if (dsMain.GetChartType() == SeriesSettings.Const_BarChart)
				{
					SetBlueBackground();
				}
			}
			else
			{
				sd.Add(dsSecondary);
			}

			SeriesDataList = sd;
		}

		private ChartSettings chartSettings;
		public ChartSettings ChartSettings
		{
			get
			{
				if (chartSettings == null)
					chartSettings = new ChartSettings();

				return chartSettings;
			}

			set
			{
				chartSettings = value;
			}
		}

		public string Name
		{
			get
			{
				return ChartSettings.Name;
			}

			set
			{
				ChartSettings.Name = value;
			}
		}
		public string Title
		{
			get
			{
				return ChartSettings.Title;
			}

			set
			{
				ChartSettings.Title = value;
			}
		}
		public string Type
		{
			get
			{
				return ChartSettings.Type;
			}

			set
			{
				ChartSettings.Type = value;
			}
		}

		public string AccountName { get; set; }
		public string AccountNumber
		{
			get
			{
				return ChartSettings.AccountNumber;
			}

			set
			{
				ChartSettings.AccountNumber = value;
			}
		}
		public int AccountId
		{
			get
			{
				return ChartSettings.AccountId;
			}

			set
			{
				ChartSettings.AccountId = value;
			}
		}

		public string DepartmentName { get; set; }
		public int DepartmentId
		{
			get
			{
				return ChartSettings.DepartmentId;
			}

			set
			{
				ChartSettings.DepartmentId = value;
			}
		}
		public string EmissionType
		{
			get
			{
				return ChartSettings.EmissionType;
			}

			set
			{
				ChartSettings.EmissionType = value;
			}
		}

		public DateSettings DateRange
		{
			get
			{
				return ChartSettings.DateRange;
			}

			set
			{
				ChartSettings.DateRange = value;
			}
		}

		protected DataRowManager privateDataRowManager = null;
		public DataRowManager dataRowManager
		{
			get
			{
				return privateDataRowManager;
			}
			set
			{
				privateDataRowManager = value;
			}
		}
		public int MaxRowsToDisplay
		{
			get
			{
				return ChartSettings.MaxRowsToDisplay;
			}

			set
			{
				ChartSettings.MaxRowsToDisplay = value;
			}
		}

		public string ZipCode
		{
			get
			{
				return ChartSettings.ZipCode;
			}

			set
			{
				ChartSettings.ZipCode = value;
			}
		}

		private City cty;
		public City City
		{
			get
			{
				if (cty == null)
					cty = City.GetCity(ChartSettings.CityId, ChartSettings.CityCode, ChartSettings.CityName);

				return cty;
			}

			set
			{
				cty = value;
			}
		}
		public void SetCity(int cityId)
		{
			City = City.FromId(cityId);
		}

		private EquipmentType equipmentType;
		public EquipmentType EquipmentType
		{
			get
			{
				if (equipmentType == null)
					equipmentType = EquipmentType.GetEquipmentType(ChartSettings.Equipment, ChartSettings.Equipment);

				return equipmentType;
			}

			set
			{
				equipmentType = value;
			}
		}

		public int Height
		{
			get
			{
				return ChartSettings.Height;
			}

			set
			{
				ChartSettings.Height = value;
			}
		}
		public int Width
		{
			get
			{
				return ChartSettings.Width;
			}

			set
			{
				ChartSettings.Width = value;
			}
		}

		public void SetBackgroundColors(Color backgroundBegin, Color backgroundEnd)
		{
			ChartSettings.BackgroundColorBegin = backgroundBegin.ToArgb().ToString();
			ChartSettings.BackgroundColorEnd = backgroundEnd.ToArgb().ToString();
		}
		public void SetBlueBackground()
		{
			SetBackgroundColors(colorHotelDark, colorHotelLight);
		}

		public void EnsureDefaults(int width, int height)
		{
			if (Width < 1)
				Width = width;

			if (Height < 1)
				Height = height;

			if (Width < 1)
				Width = 500;

			if (Height < 1)
				Height = 300;
		}

		protected virtual bool FillData()
		{
			//if (cm == null)
			//	throw new Exception("Must have a ChartManager to FillData.");
			if (!SettingsCanBuildAChart())
				throw new Exception("You must specify either a chart name or the proper criteria to build a chart.");

			bool bFound = false;

			return bFound;
		}

		protected static SeriesSettings GetDataSeriesInt(string xAxisTitle, string yAxisTitle, ConversionBase.ConversionIdentifiers ci)
		{
			SeriesSettings dss = new SeriesSettings(xAxisTitle, 1, yAxisTitle, 2, ci);
			dss.YAxis.FieldDescription = DataTypeDescription.GetNumberInt(2);
			dss.MainColor = Color.Orange.ToArgb().ToString();
			dss.MarkerSize = 10;
			dss.BorderWidth = 5;

			return dss;
		}

		public virtual Control BuildDepartmentalTable()
		{
			return null;
		}
	}
}
