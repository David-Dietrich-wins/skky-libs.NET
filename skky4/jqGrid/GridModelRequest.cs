using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace skky.jqGrid
{
	public class GridModelRequest
	{
		public int page { get; set; }

		public int records { get; set; }

		public string sidx { get; set; }

		public string sord { get; set; }

		public GridModelResponse<TTarget> GetResponse<TSource, TTarget>(IQueryable<TSource> items, Func<TSource, TTarget> selector)
		{
			var totalItems = items.Count();

			if (string.IsNullOrEmpty(sidx) == false)
			{
				Expression<Func<DateTime, int>> x = d => d.Date.TimeOfDay.Days;
				//apply sort
				//items = sord == "asc" ? items.OrderBy(sidx) : items.OrderByDescending(sidx);
			}

			return new GridModelResponse<TTarget>
			{
				total = (int)Math.Ceiling((double)totalItems / records),
				page = page,
				records = totalItems,
				rows = items.Skip((page - 1) * records).Take(records).AsEnumerable().Select(selector).ToArray(),
			};
		}

		public GridModelResponse<TTarget> GetResponse<TSource, TTarget>(IEnumerable<TSource> items, Func<TSource, TTarget> selector)
		{
			return GetResponse(items.AsQueryable(), selector);
		}
	}
}
