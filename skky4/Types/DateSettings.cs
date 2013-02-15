using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.util;

namespace skky.Types
{
	[DataContract]
	public class DateSettings
	{
		public const string CONST_day = "day";
		public const string CONST_week = "week";
		public const string CONST_month = "month";
		public const string CONST_quarter = "quarter";
		public const string CONST_year = "year";
		public const string CONST_past = "past";
		public const string CONST_prev = "prev";
		public const string CONST_toda = "toda";
		public const string CONST_todate = "todate";

		public DateSettings()
		{ }

		public override int GetHashCode()
		{
			return NumberOfDays.GetHashCode() ^ NumberOfPeriods.GetHashCode() ^ GetStartDateTime().GetHashCode() ^ GetEndDateTime().GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (!this.GetType().Equals(obj.GetType()))
				return false;

			DateSettings rhs = obj as DateSettings;
			if (rhs == null)
				return false;

			if(StartDateTime.HasValue && !StartDateTime.Equals(rhs.StartDateTime))
				return false;
			else if(rhs.StartDateTime.HasValue && !rhs.StartDateTime.Equals(StartDateTime))
				return false;

			if(EndDateTime.HasValue && !EndDateTime.Equals(rhs.EndDateTime))
				return false;
			else if(rhs.EndDateTime.HasValue && !rhs.EndDateTime.Equals(EndDateTime))
				return false;

			if (TimeConfigOption != rhs.TimeConfigOption)
				return false;
			if (NumberOfDays != rhs.NumberOfDays)
				return false;
			if (NumberOfPeriods != rhs.NumberOfPeriods)
				return false;
			if ((TimeFrame == null && rhs.TimeFrame != null) || (TimeFrame != null && !TimeFrame.Equals(rhs.TimeFrame)))
				return false;
			if ((TimePeriod == null && rhs.TimePeriod != null) || (TimePeriod != null && !TimePeriod.Equals(rhs.TimePeriod)))
				return false;

			if (ShowAllDates != rhs.ShowAllDates)
				return false;
			// use this pattern to compare reference members
			//if (!Object.Equals(Name, cust.Name)) return false;

			// use this pattern to compare value members
			//if (!Age.Equals(cust.Age)) return false;

			return true;
		}
		public static bool operator !=(DateSettings a, DateSettings b)
		{
			return !(a == b);
		}
		public static bool operator ==(DateSettings left, DateSettings rhs)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals(left, rhs))
			{
				return true;
			} 
			if ((object)left == null || (object)rhs == null)
				return false;

			if (left.StartDateTime.HasValue && !left.StartDateTime.Equals(rhs.StartDateTime))
				return false;
			else if (rhs.StartDateTime.HasValue && !rhs.StartDateTime.Equals(left.StartDateTime))
				return false;

			if (left.EndDateTime.HasValue && !left.EndDateTime.Equals(rhs.EndDateTime))
				return false;
			else if (rhs.EndDateTime.HasValue && !rhs.EndDateTime.Equals(left.EndDateTime))
				return false;

			if (left.TimeConfigOption != rhs.TimeConfigOption)
				return false;
			if (left.NumberOfDays != rhs.NumberOfDays)
				return false;
			if (left.NumberOfPeriods != rhs.NumberOfPeriods)
				return false;
			if ((left.TimeFrame == null && rhs.TimeFrame != null) || (left.TimeFrame != null && !left.TimeFrame.Equals(rhs.TimeFrame)))
				return false;
			if ((left.TimePeriod == null && rhs.TimePeriod != null) || (left.TimePeriod != null && !left.TimePeriod.Equals(rhs.TimePeriod)))
				return false;

			if (left.ShowAllDates != rhs.ShowAllDates)
				return false;
			// use this pattern to compare reference members
			//if (!Object.Equals(Name, cust.Name)) return false;

			// use this pattern to compare value members
			//if (!Age.Equals(cust.Age)) return false;

			return true;
		}

		[DataMember]
		public DateTime? StartDateTime { get; set; }

		[DataMember]
		public DateTime? EndDateTime { get; set; }

		/// <summary>
		/// 0 - Use Date Range, 1 - Use Periods
		/// </summary>
		[DataMember]
		public int TimeConfigOption { get; set; }

		[DataMember]
		public int NumberOfDays { get; set; }

		[DataMember]
		public bool ShowAllDates { get; set; }

		/// <summary>
		/// past, prev, todate
		/// </summary>
		[DataMember]
		public string TimeFrame { get; set; }

		// day, week, month, quarter, year
		[DataMember]
		public string TimePeriod { get; set; }

		[DataMember]
		public int NumberOfPeriods { get; set; }

		public bool HasStartDate()
		{
			// Want to be sure there is no time component.
			return !GetStartDate().Equals(DateTime.MinValue);
		}
		public bool HasEndDate()
		{
			// Want to be sure there is no time component.
			return !GetEndDate().Equals(DateTime.MaxValue.Date);
		}

		public bool OnlyHasStartDate()
		{
			return HasStartDate() && !HasEndDate();
		}
		public bool OnlyHasEndDate()
		{
			return HasEndDate() && !HasStartDate();
		}

		public bool ShouldShowAllDates()
		{
			return ((TimeConfigOption == 0 && ShowAllDates)
				|| (!HasStartDate() && !HasEndDate()));
		}

		public DateTime GetStartDateTime()
		{
			DateTime? dt = GetStartDateTimeWithNull();
			if (dt == null)
				dt = DateTime.MinValue;

			return dt.Value;
		}
		public DateTime? GetStartDateTimeWithNull()
		{
			if (TimeConfigOption == 0)
			{
				if (!ShowAllDates)
				{
					if (StartDateTime.HasValue)
					{
						DateTime dt = StartDateTime.Value;

						if (EndDateTime.HasValue || NumberOfDays >= 0)
							return dt;

						return dt.AddDays(NumberOfDays);
					}

					// No StartDateTime. Try to determine the start date time.
					if (EndDateTime.HasValue && NumberOfDays != 0)
					{
						DateTime dt = EndDateTime.Value;

						if (NumberOfDays > 0)
							return dt;

						return dt.AddDays(NumberOfDays);
					}
				}
			}
			else
			{
				DateTime current = DateTime.Now;
				switch ((TimeFrame ?? string.Empty).Left(4).ToLower())
				{
					case CONST_prev:
						switch ((TimePeriod ?? string.Empty).ToLower())
						{
							case CONST_week:
								return current.AddDays(-7 * NumberOfPeriods).Date;
							case CONST_month:
								current = current.AddMonths(-1 * NumberOfPeriods).Date;
								return new DateTime(current.Year, current.Month, 1);
							case CONST_quarter:
								current = current.AddMonths(-3 * NumberOfPeriods).Date;
								return new DateTime(current.Year, current.Month, 1);
							case CONST_year:
								current = current.AddYears(-1 * NumberOfPeriods).Date;
								return new DateTime(current.Year, 1, 1);
							default:	// day
								return current.AddDays(-1 * NumberOfPeriods).Date;
						}
						//break;
					case CONST_toda:	// todate
						switch ((TimePeriod ?? string.Empty).ToLower())
						{
							case CONST_week:
								return current.AddDays((-7 * (NumberOfPeriods - 1)) + (-1 * (int)current.DayOfWeek) + 1).Date;
							case CONST_month:
								current = current.AddMonths(-1 * (NumberOfPeriods - 1)).Date;
								return new DateTime(current.Year, current.Month, 1).Date;
							case CONST_quarter:
								current = current.AddMonths(-3 * (NumberOfPeriods - 1)).Date;
								return new DateTime(current.Year, current.Month - (current.Month % 3) + 1, 1).Date;
							case CONST_year:
								current = current.AddYears(-1 * (NumberOfPeriods - 1)).Date;
								return new DateTime(current.Year, 1, 1);
							default:	// day
								return current.AddDays(-1 * (NumberOfPeriods - 1)).Date;
						}
						//break;
					default:	// past
						switch ((TimePeriod ?? string.Empty).ToLower())
						{
							case CONST_week:
								return current.AddDays(-7 * NumberOfPeriods).Date;
							case CONST_month:
								return current.AddMonths(-1 * NumberOfPeriods).Date;
							case CONST_quarter:
								return current.AddMonths(-3 * NumberOfPeriods).Date;
							case CONST_year:
								return current.AddYears(-1 * NumberOfPeriods).Date;
							default:	// day
								return current.AddDays(-1 * NumberOfPeriods).Date;
						}
						//break;
				}
			}

			return null;
		}

		// Strip off the time component.
		public DateTime GetStartDate()
		{
			return GetStartDateTime().Date;
		}
		public int GetStartDateKey()
		{
			return GetStartDate().ToDateKey();
		}
		public DateTime? GetStartDateWithNull()
		{
			var d = GetStartDateTimeWithNull();
			if (d.HasValue)
				return d.Value.Date;

			return null;
		}
		public int? GetStartDateKeyWithNull()
		{
			var d = GetStartDateWithNull();
			if (d.HasValue)
				return d.Value.ToDateKey();

			return null;
		}

		public DateTime GetEndDateTime()
		{
			DateTime? dt = GetEndDateTimeWithNull();
			if (dt == null)
				dt = DateTime.MaxValue;

			return dt.Value;
		}
		public DateTime? GetEndDateTimeWithNull()
		{
			if (TimeConfigOption == 0)
			{
				if (!ShowAllDates)
				{
					if (EndDateTime.HasValue)
					{
						DateTime dt = EndDateTime.Value;

						if (StartDateTime.HasValue || NumberOfDays <= 0)
							return dt;

						return dt.AddDays(NumberOfDays);
					}

					// No EndDateTime. Try to determine the end date time.
					if (StartDateTime.HasValue && NumberOfDays != 0)
					{
						DateTime dt = StartDateTime.Value;

						// We starting from before the start date.
						if (NumberOfDays < 0)
							return dt;

						return dt.AddDays(NumberOfDays);
					}
				}
			}
			else
			{
				DateTime current = DateTime.Now;
				switch ((TimeFrame ?? string.Empty).Left(4).ToLower())
				{
					case CONST_prev:
						switch ((TimePeriod ?? string.Empty).ToLower())
						{
							case CONST_week:
								return current.AddDays((-1 * (int)current.DayOfWeek) + 1).Date;
							case CONST_month:
								return new DateTime(current.Year, current.Month, 1).Date;
							case CONST_quarter:
								return new DateTime(current.Year, current.Month - (current.Month % 3) + 1, 1).Date;
							case CONST_year:
								current = current.Date;
								return new DateTime(current.Year, 1, 1);
							default:	// day
								return current.Date;
						}
						//break;
					case CONST_toda:	// todate
						return current.Date;
					default:	// past
						return current.Date;
				}
			}

			return null;
		}
		// Strip off the time component.
		public DateTime GetEndDate()
		{
			return GetEndDateTime().Date;
		}
		public int GetEndDateKey()
		{
			return GetEndDate().ToDateKey();
		}
		public DateTime? GetEndDateWithNull()
		{
			var d = GetEndDateTimeWithNull();
			if (d.HasValue)
				return d.Value.Date;

			return null;
		}
		public int? GetEndDateKeyWithNull()
		{
			var d = GetEndDateWithNull();
			if (d.HasValue)
				return d.Value.ToDateKey();

			return null;
		}

		public string PeriodStringShortDates()
		{
			string s = string.Empty;
			if (ShouldShowAllDates())
			{
				s += "All Dates";
			}
			else
			{
				s += GetStartDate().ToString("M/d/yy");
				s += " - ";
				s += GetEndDate().ToString("M/d/yy");
			}

			return s;
		}

		public string getDateText(bool forHTML = true)
		{
			string s = string.Empty;

			if (ShouldShowAllDates())
			{
				s = "All Dates";
			}
			else if (TimeConfigOption == 0)	// Use Date Range (Time Frame)
			{
				s += "From ";
				s += GetStartDateTime().ToString("M/d/yyyy");
				s += " to ";
				s += GetEndDateTime().ToString("M/d/yyyy");
			}
			else  // Use Time Periods
			{
				switch ((TimeFrame ?? string.Empty).Left(4).ToLower())
				{
					case CONST_prev:
						s += "Previous";
						break;

					case CONST_toda:	// todate
						s += "To Date";
						break;
					default:	// past
						s += "Past";
						break;
				}

				s += " " + NumberOfPeriods.ToString();
				s += " " + (TimePeriod ?? string.Empty).ToLower();
				if (NumberOfPeriods != 1)
					s += "s";
				s += ".";
				if (forHTML)
					s += "<br />";
				else
					s += " (";
				s += GetStartDateTime().ToString("M/d/yyyy");
				s += " to ";
				s += GetEndDateTime().ToString("M/d/yyyy");
				if(!forHTML)
					s += ")";
			}

			return s;
		}
	}
}
