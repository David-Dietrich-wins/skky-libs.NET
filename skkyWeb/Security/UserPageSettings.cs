using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skkyWeb.Charts;
using skky.Types;

namespace skkyWeb.Security
{
	[Serializable]
	public class UserPageSettings
	{
		private KeyedKeyValuePairCollection<int, List<ChartSettings>> chartSettings;

		public KeyedKeyValuePairCollection<int, List<ChartSettings>> myChartSettings
		{
			get
			{
				return chartSettings;
			}
			set
			{
				chartSettings = value;
			}
		}

		public List<ChartSettings> GetCurrentChartSettings(int customerId)
		{
			List<ChartSettings> ch = null;

			if (myChartSettings == null)
			{
				myChartSettings = new KeyedKeyValuePairCollection<int, List<ChartSettings>>();
				myChartSettings.Add(new SerializableKeyValuePair<int, List<ChartSettings>>(customerId, ChartSettings.GetDefaultCharts()));
			}

			foreach (var set in myChartSettings)
				if (set.Key == customerId)
					ch = set.Value;

			if (ch == null)
			{
				ch = new List<ChartSettings>();
				SaveChartSettings(customerId, ch);
			}

			return ch;
		}

		public void SaveChartSettings(int customerId, List<ChartSettings> settings)
		{
			if (myChartSettings == null)
				myChartSettings = new KeyedKeyValuePairCollection<int, List<ChartSettings>>();

			//Remove old settings
			SerializableKeyValuePair<int, List<ChartSettings>> item = new SerializableKeyValuePair<int, List<ChartSettings>>();
			foreach (var set in myChartSettings)
				if (set.Key == customerId)
					item = set;

			if (item.Key != 0)
				myChartSettings.Remove(item);

			//Add new settings
			myChartSettings.Add(new SerializableKeyValuePair<int, List<ChartSettings>>(customerId, settings));
		}
	}
}
