using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using skky.Types;

namespace skkyWeb.Charts
{
	public interface ChartBuilder
	{
		void init(SeriesData seriesData);
		void init(IEnumerable<SeriesData> seriesData);
		void SetPaletteCustomColors(Color[] colors);
		//string GetChartImageURL();
		void AddHelpImagePaint();

		string getID();
	}
}
