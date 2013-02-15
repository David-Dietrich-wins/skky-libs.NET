using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace skkyWeb.util
{
	public class JsonpResponseFilter : Stream
	{
		private readonly Stream _responseStream;
		private HttpContext _context;

		public JsonpResponseFilter(Stream responseStream, HttpContext context)
		{
			_responseStream = responseStream;
			_context = context;
		}

		public override bool CanRead { get { return true; } }

		public override bool CanSeek { get { return true; } }

		public override bool CanWrite { get { return true; } }

		public override long Length { get { return 0; } }

		public override long Position { get; set; }

		public override void Write(byte[] buffer, int offset, int count)
		{
			var b1 = Encoding.UTF8.GetBytes(_context.Request.Params[JsonpHttpModule.JSONP_CALLBACK] + "(");
			_responseStream.Write(b1, 0, b1.Length);
			_responseStream.Write(buffer, offset, count);
			var b2 = Encoding.UTF8.GetBytes(");");
			_responseStream.Write(b2, 0, b2.Length);
		}

		public override void Close()
		{
			_responseStream.Close();
		}

		public override void Flush()
		{
			_responseStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return _responseStream.Seek(offset, origin);
		}

		public override void SetLength(long length)
		{
			_responseStream.SetLength(length);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return _responseStream.Read(buffer, offset, count);
		}
	}
}
