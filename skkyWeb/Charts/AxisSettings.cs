using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;
using skky.util;
using System.Runtime.Serialization;

namespace skkyWeb.Charts
{
	public enum AxisType
	{
		XAxis,
		YAxis
	};

	[DataContract]
	public class AxisSettings
	{
		public AxisSettings()
			: this(AxisType.XAxis)
		{ }
		public AxisSettings(AxisType type)
			: this(type, type == AxisType.XAxis ? DataTypeDescription.GetString(0) : DataTypeDescription.GetNumberDouble(0))
		{ }
		public AxisSettings(AxisType type, DataTypeDescription dtd)
		{
			Axis = type;
			FieldDescription = dtd;
		}

		private int? axisOffset;
		public int AxisOffset
		{
			get
			{
				if(axisOffset == null || axisOffset == 0)
					return 21;

				return (int)axisOffset;
			}
			set
			{
				axisOffset = value;
			}
		}
		private int? labelSize;
		public int LabelSize {
			get
			{
				if (labelSize == null || labelSize == 0)
					return 8;

				return (int)labelSize;
			}
			set
			{
				labelSize = value;
			}
		}

		private DataTypeDescription fieldDescription = null;
		[DataMember]
		public DataTypeDescription FieldDescription {
			get
			{
				if (fieldDescription == null)
					fieldDescription = DataTypeDescription.GetString(-1);

				return fieldDescription;
			}
			set
			{
				fieldDescription = value;
			}
		}

		[DataMember]
		public AxisType Axis { get; set; }

		[DataMember]
		public string Title { get; set; }

		public void SetFormatString(bool isLongFormat)
		{
			FieldDescription.SetFormatString(isLongFormat);
		}
		/*
		public string GetFormatString(int offset)
		{
			return GetFormatString(offset, false);
		}
		 */
		public void SetFormatString(int offset, bool IsLongFormat)
		{
			 FieldDescription.SetFormatString(offset, IsLongFormat);

			//return offset.ToString().WrapInBraces();
		}
	}
}
