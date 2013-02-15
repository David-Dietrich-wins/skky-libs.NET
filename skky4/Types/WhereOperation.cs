using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Types
{
	public enum WhereOperation
	{
		eq,	// =
		ne,	// <>
		lt,	// <
		le,	// <=
		gt,	// >
		ge,	// >=
		bw,	// Begins With
		bn,	// Does not Begin With
		@in,	// IN
		ni,	// Not IN
		ew,	// Ends With
		en,	// Does not End With
		cn,	// Contains
		nc,	// Does not contain
		nn,	// Not Null
		nu	// Null
	}
}
