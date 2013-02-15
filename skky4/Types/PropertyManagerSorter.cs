using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Types
{
	public class PropertyManagerSorter : IComparer<PropertyManager>
	{
		int iSortField = 0;

		public PropertyManagerSorter(int sortField)
		{
			iSortField = sortField;
		}

		﻿public int Compare(PropertyManager left, PropertyManager right)
		{
			switch (iSortField)
			{
				case 2:
				case 3:
				case 4:
					double? dLeft = left.GetProperty(iSortField).doubleValue;
					double? dRight = right.GetProperty(iSortField).doubleValue;
					if (null == dLeft)
					{
						if (null == dRight)
							return 0;

						return -1;
					}
					else
					{
						if (null == dRight)
							return 1;

						if (dLeft == dRight)
							return 0;
					}

					return dLeft > dRight ? 1 : -1;
			}

			return string.Compare(left.GetProperty(iSortField).stringValue, right.GetProperty(iSortField).stringValue);
		}
	}
}
