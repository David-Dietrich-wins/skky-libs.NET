using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.util
{
	public class FileNameRotator
	{
		public const int DefaultFileOffsetMax = 300;

		private object FileOffsetLock = new object();
		private int FileOffset { get; set; }

		public string Prefix { get; set; }
		public string Suffix { get; set; }

		public int FileOffsetMaximum { get; set; }


		public FileNameRotator(string prefix, string suffix)
			: this(prefix, suffix, DefaultFileOffsetMax)
		{ }
		public FileNameRotator(string prefix, string suffix, int FileOffsetMax)
		{
			Prefix = prefix;
			Suffix = suffix;
			FileOffsetMaximum = (FileOffsetMax < 1 ? DefaultFileOffsetMax : FileOffsetMax);
		}

		private int NextFileOffset()
		{
			lock (FileOffsetLock)
			{
				if (FileOffset >= FileOffsetMaximum)
					FileOffset = 0;
				else
					++FileOffset;

				return FileOffset;
			}
		}

		public string NextFileName
		{
			get
			{
				return Prefix + NextFileOffset().ToString("0000") + Suffix;
			}
			private set
			{
				;
			}
		}
	}
}
