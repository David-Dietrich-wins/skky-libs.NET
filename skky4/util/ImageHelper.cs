using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace skky.util
{
	public static class ImageHelper
	{
﻿		public enum ImageSize
		{
			Small,
			Medium,
			Large
		}

		public class ImageSizeDefaults
		{
			public static int Small = 100;
			public static int Medium = 250;
			public static int Large = 400;
		}

		public static void ResizeImageTo(string imgPath, string toPath, ImageSize size, bool upscale = true)
		{
			//** load the image from the imgPath
			using (var img = Image.FromFile(imgPath))
			{
				int height = 0, width = 0;

				Func<int, int> resizeHeight = (toWidth) =>
				{
					return (int)(((decimal)toWidth / img.Width) * img.Height);
				};

				width = img.Width;
				switch (size)
				{
					case ImageSize.Small:
						//** use the small size
						if (img.Width > ImageSizeDefaults.Small || (upscale && width < ImageSizeDefaults.Small))
							width = ImageSizeDefaults.Small;
						break;
					case ImageSize.Medium:
						//** medium
						if (img.Width > ImageSizeDefaults.Medium || (upscale && width < ImageSizeDefaults.Medium))
							width = ImageSizeDefaults.Medium;
						break;
					case ImageSize.Large:
						//** large
						if (img.Width > ImageSizeDefaults.Large || (upscale && width < ImageSizeDefaults.Large))
							width = ImageSizeDefaults.Large;
						break;
				}

				//** if the image couldn't be resized to the target size, quit
				if (width == 0)
					return;

				//** maintain the aspect, calculating the height
				height = resizeHeight(width);

				using (var bitmap = new System.Drawing.Bitmap(img, width, height))
				using (var g = Graphics.FromImage(bitmap))
				{
					//** high quality interpolation, smoothing, and composition
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
					g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

					//** render the image
					g.DrawImage(bitmap, new Rectangle(0, 0, width, height));

					//** make sure the destination directory exists
					string newPath = toPath.Replace(Path.GetFileName(toPath), "");
					if (!Directory.Exists(newPath))
						Directory.CreateDirectory(newPath);

					//** save to disk
					using (var fstream = System.IO.File.Create(toPath))
						bitmap.Save(fstream, img.RawFormat);
				}
			}
		}
	}
}
