using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using skky.Types;
using skky.jqGrid;
using System.Text;
using System.ComponentModel;

namespace skky.util
{
	public static class LinqExtensions
	{
		public static IEnumerable<T> getSorted<T>(this IEnumerable<T> source, ActionParams ap, string defaultSortColumn = "")
		{
			if (null != ap && !string.IsNullOrWhiteSpace(ap.sidx) && !string.IsNullOrWhiteSpace(ap.sord))
			{
				return source.getSorted<T>(ap.sidx, ap.sord);
			}
			else if(!string.IsNullOrWhiteSpace(defaultSortColumn))
			{
				return source.getSorted<T>(defaultSortColumn);
			}

			return source;
		}
		public static IEnumerable<T> getSorted<T>(this IEnumerable<T> source, string sortBy, string sortDirection = ActionParams.CONST_sordAsc)
		{
			if (!string.IsNullOrEmpty(sortBy))
			{
				var param = Expression.Parameter(typeof(T), "item");

				var sortExpression = Expression.Lambda<Func<T, object>>
					(Expression.Convert(Expression.Property(param, sortBy), typeof(object)), param);

				if (ActionParams.CONST_sordDesc == (sortDirection ?? string.Empty).ToLower())
						return source.AsQueryable<T>().OrderByDescending<T, object>(sortExpression);

				return source.AsQueryable<T>().OrderBy<T, object>(sortExpression);
			}

			return source;
		}

		/*
		public static T[] SearchGrid<T>(this IQueryable<T> query, ActionParams grid, out int totalRecords) where T : class
		{
			if (grid._search)
			{
				StringBuilder sb = new StringBuilder();
				int i = 0;
				foreach (var rule in grid.theFilter.rules)
				{
					string op = null;

					switch (rule.op)
					{
						case "eq": op = rule.field + "={0}";
							break;
						case "ne": op = rule.field + "!={0}";
							break;
						case "lt": op = rule.field + "<{0}";
							break;
						case "le": op = rule.field + "<={0}";
							break;
						case "gt": op = rule.field + ">{0}";
							break;
						case "ge": op = rule.field + ">={0}";
							break;
						case "bw": op = rule.field + ".StartsWith({0})";
							break;
						case "bn": op = "!" + rule.field + ".StartsWith({0})";
							break;
						case "in":
							break;
						case "ni":
							break;
						case "ew": op = rule.field + ".EndsWith({0})";
							break;
						case "en": op = "!" + rule.field + ".EndsWith({0})";
							break;
						case "cn": op = rule.field + ".Contains({0})";
							break;
						case "nc": op = "!" + rule.field + ".Contains({0})";
							break;
						case "nu": op = rule.field + "==null";
							break;
						case "nn": op = rule.field + "!=null";
							break;
					}

					if (op == null)
						throw new NotSupportedException("rule.op=" + rule.op);

					op = string.Format(op, "@" + i.ToString());

					sb.Append(op);
					if (rule != grid.theFilter.rules.Last())
					{
						sb.Append(grid.theFilter.groupOp == "AND" ? "&&" : "||");
					}
					i++;
				}

				var predicate = sb.ToString();
				var values = grid.theFilter.rules.Select(r => r.data).ToArray();
				query = query.Where<T>(predicate, values);
			}

			//records
			totalRecords = query.Count();

			//sorting
			query = query.OrderBy<T>(grid.sidx, grid.sord);

			//paging
			var data = query.Skip((grid.page - 1) * grid.rows).Take(grid.rows).ToArray();

			return data;
		}
	*/
		/// <summary>Orders the sequence by specific column and direction.</summary>
		/// <param name="query">The query.</param>
		/// <param name="sortColumn">The sort column.</param>
		/// <param name="ascending">if set to true [ascending].</param>
		public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string sortColumn, string direction = ActionParams.CONST_sordAsc)
		{
			string methodName = string.Format("OrderBy{0}",
				(direction ?? string.Empty).ToLower() == ActionParams.CONST_sordDesc ? "Descending" : "");

			ParameterExpression parameter = Expression.Parameter(query.ElementType, "p");

			MemberExpression memberAccess = null;

			try
			{
				foreach (var property in sortColumn.Split('.'))
					memberAccess = MemberExpression.Property
						(memberAccess ?? (parameter as Expression), property);

				if (null != memberAccess)
				{
					LambdaExpression orderByLambda = Expression.Lambda(memberAccess, parameter);

					MethodCallExpression result = Expression.Call(
								typeof(Queryable),
								methodName,
								new[] { query.ElementType, memberAccess.Type },
								query.Expression,
								Expression.Quote(orderByLambda));

					return query.Provider.CreateQuery<T>(result);
				}
			}
			catch(Exception)
			{
				// Ignore sort order errors.
			}

			return query;
		}
		/*
		public static IQueryable<T> Where<T>(this IQueryable<T> query,
			string column, object value, string whereOperation)
		{
			return Where<T>(query, column, value
				, (WhereOperation)StringEnum.Parse(typeof(WhereOperation), whereOperation));
		}
		public static IQueryable<T> Where<T>(this IQueryable<T> query,
			string column, object value, WhereOperation operation)
		{
			if (string.IsNullOrEmpty(column))
				return query;

			ParameterExpression parameter = Expression.Parameter(query.ElementType, "p");

			MemberExpression memberAccess = null;
			foreach (var property in column.Split('.'))
				memberAccess = MemberExpression.Property
					(memberAccess ?? (parameter as Expression), property);

			//change param value type
			//necessary to getting bool from string
			ConstantExpression filter = Expression.Constant
				(
					Convert.ChangeType(value, memberAccess.Type)
				);

			//switch operation
			Expression condition = null;
			LambdaExpression lambda = null;
			switch (operation)
			{
				case WhereOperation.bn:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), Expression.Constant(value));
					lambda = Expression.Lambda(Expression.Not(condition), parameter);
					break;
				case WhereOperation.bw:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), Expression.Constant(value));
					lambda = Expression.Lambda(condition, parameter);
					break;
				//string.Contains()
				case WhereOperation.cn:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("Contains"), Expression.Constant(value));
					lambda = Expression.Lambda(condition, parameter);
					break;
				case WhereOperation.en:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(value));
					lambda = Expression.Lambda(Expression.Not(condition), parameter);
					break;
				//equal ==
				case WhereOperation.eq:
					condition = Expression.Equal(memberAccess, filter);
					lambda = Expression.Lambda(condition, parameter);
					break;
				case WhereOperation.ew:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(value));
					lambda = Expression.Lambda(condition, parameter);
					break;
				case WhereOperation.ge:
					condition = Expression.GreaterThanOrEqual(memberAccess, filter);
					lambda = Expression.Lambda(condition, parameter);
					break;
				case WhereOperation.gt:
					condition = Expression.GreaterThan(memberAccess, filter);
					lambda = Expression.Lambda(condition, parameter);
					break;
				case WhereOperation.@in:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("Contains"), Expression.Constant(value));
					lambda = Expression.Lambda(condition, parameter);
					break;
				case WhereOperation.le:
					condition = Expression.LessThanOrEqual(memberAccess, filter);
					lambda = Expression.Lambda(condition, parameter);
					break;
				case WhereOperation.lt:
					condition = Expression.LessThan(memberAccess, filter);
					lambda = Expression.Lambda(condition, parameter);
					break;
				case WhereOperation.nc:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("Contains"), Expression.Constant(value));
					lambda = Expression.Lambda(Expression.Not(condition), parameter);
					break;
				//not equal !=
				case WhereOperation.ne:
					condition = Expression.NotEqual(memberAccess, filter);
					lambda = Expression.Lambda(condition, parameter);
					break;
				case WhereOperation.ni:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("Contains"), Expression.Constant(value));
					lambda = Expression.Lambda(Expression.Not(condition), parameter);
					break;
				case WhereOperation.nn:
					condition = Expression.Equal(memberAccess, Expression.Constant(null));
					lambda = Expression.Lambda(Expression.Not(condition), parameter);
					break;
				case WhereOperation.nu:
					condition = Expression.Equal(memberAccess, Expression.Constant(null));
					lambda = Expression.Lambda(condition, parameter);
					break;
			}

			if (null != lambda)
			{
				MethodCallExpression result = Expression.Call(
						typeof(Queryable), "Where",
						new[] { query.ElementType },
						query.Expression,
						lambda);

				return query.Provider.CreateQuery<T>(result);
			}

			return query;
		}
		*/

		public static object ChangeType(object value, Type conversionType)
		{
			// Note: This if block was taken from Convert.ChangeType as is, and is needed here since we're
			// checking properties on conversionType below.
			if (conversionType == null)
			{
				throw new ArgumentNullException("conversionType");
			} // end if

			// If it's not a nullable type, just pass through the parameters to Convert.ChangeType

			if (conversionType.IsGenericType &&
			  conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
			{
				// It's a nullable type, so instead of calling Convert.ChangeType directly which would throw a
				// InvalidCastException (per http://weblogs.asp.net/pjohnson/archive/2006/02/07/437631.aspx),
				// determine what the underlying type is
				// If it's null, it won't convert to the underlying type, but that's fine since nulls don't really
				// have a type--so just return null
				// Note: We only do this check if we're converting to a nullable type, since doing it outside
				// would diverge from Convert.ChangeType's behavior, which throws an InvalidCastException if
				// value is null and conversionType is a value type.
				if (value == null)
				{
					return null;
				} // end if

				// It's a nullable type, and not null, so that means it can be converted to its underlying type,
				// so overwrite the passed-in conversion type with this underlying type
				NullableConverter nullableConverter = new NullableConverter(conversionType);
				//conversionType = nullableConverter.UnderlyingType;
				return nullableConverter.ConvertFrom(value);
			} // end if

			// Now that we've guaranteed conversionType is something Convert.ChangeType can handle (i.e. not a
			// nullable type), pass the call on to Convert.ChangeType
			return Convert.ChangeType(value, conversionType);
		}

		public static Expression Lambda<T>(this IQueryable<T> query, string column, object value, string whereOperation)
		{
			ParameterExpression parameter = Expression.Parameter(query.ElementType, "p");
			return Lambda(query, parameter, column, value, whereOperation);
		}
		public static Expression Lambda<T>(this IQueryable<T> query, ParameterExpression parameter, string column, object value, string whereOperation)
		{
			if (string.IsNullOrEmpty(column))
				return null;

			WhereOperation operation = (WhereOperation)StringEnum.Parse(typeof(WhereOperation), whereOperation);

			MemberExpression memberAccess = null;
			foreach (var property in column.Split('.'))
				memberAccess = MemberExpression.Property
					(memberAccess ?? (parameter as Expression), property);

			//change param value type
			//necessary to getting bool from string
			//Coalesce to get actual property type...
			Type t = memberAccess.Type;
			//t = Nullable.GetUnderlyingType(t) ?? t;
			object o = ChangeType(value, t);
			Expression filter = Expression.Constant(o, t);
			//ConstantExpression filter = Expression.Constant(value, memberAccess.Type);

			//switch operation
			Expression condition = null;
			switch (operation)
			{
				case WhereOperation.bn:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), Expression.Constant(value));
					condition = Expression.Not(condition);
					break;
				case WhereOperation.bw:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), Expression.Constant(value));
					break;
				//string.Contains()
				case WhereOperation.cn:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("Contains"), Expression.Constant(value));
					break;
				case WhereOperation.cni:
					var methodInfo = typeof(string).GetMethod("IndexOf", new[] { typeof(string), typeof(StringComparison) });
					var callEx = Expression.Call(memberAccess, methodInfo, Expression.Constant(value), Expression.Constant(StringComparison.OrdinalIgnoreCase));
					condition = Expression.NotEqual(callEx, Expression.Constant(-1)); 
					break;
				case WhereOperation.en:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(value));
					condition = Expression.Not(condition);
					break;
				//equal ==
				case WhereOperation.eq:
					condition = Expression.Equal(memberAccess, filter);
					break;
				case WhereOperation.ew:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(value));
					break;
				case WhereOperation.ge:
					condition = Expression.GreaterThanOrEqual(memberAccess, filter);
					break;
				case WhereOperation.gt:
					condition = Expression.GreaterThan(memberAccess, filter);
					break;
				case WhereOperation.@in:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("Contains"), Expression.Constant(value));
					break;
				case WhereOperation.le:
					condition = Expression.LessThanOrEqual(memberAccess, filter);
					break;
				case WhereOperation.lt:
					condition = Expression.LessThan(memberAccess, filter);
					break;
				case WhereOperation.nc:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("Contains"), Expression.Constant(value));
					condition = Expression.Not(condition);
					break;
				//not equal !=
				case WhereOperation.ne:
					condition = Expression.NotEqual(memberAccess, filter);
					break;
				case WhereOperation.ni:
					condition = Expression.Call(memberAccess, typeof(string).GetMethod("Contains"), Expression.Constant(value));
					condition = Expression.Not(condition);
					break;
				case WhereOperation.nn:
					condition = Expression.Equal(memberAccess, Expression.Constant(null));
					condition = Expression.Not(condition);
					break;
				case WhereOperation.nu:
					condition = Expression.Equal(memberAccess, Expression.Constant(null));
					break;
			}

			return condition;
		}
		public static IQueryable<T> SortedList<T>(this IQueryable<T> query, ActionParams ap, string sidxDefault = null, string sordDefault = ActionParams.CONST_sordAsc)
		{
			if (null == ap)
				ap = new ActionParams();

			if (ap._search && null != ap.theFilter && null != ap.theFilter.rules && ap.theFilter.rules.Count() > 0)
			{
				List<Expression> exprs = new List<Expression>();

				ParameterExpression parameter = Expression.Parameter(query.ElementType, "p");

				foreach (var rule in ap.theFilter.rules)
				{
					var expr = query.Lambda<T>(parameter, rule.field, rule.data, rule.op);
					if (null != expr)
						exprs.Add(expr);
				}
				if (exprs.Count() > 0)
				{
					Expression master = null;
					for (int i = 0; i < exprs.Count(); ++i)
					{
						Expression cur = exprs.ElementAt(i);
						if(i == 0)
						{
							master = cur;
						}
						else
						{
							Expression prev = exprs.ElementAt(i - 1);
							//And, All
							var op = (ap.theFilter.groupOp ?? string.Empty).Left(3).ToLower();
							if ("and" == op || "all" == op)
							{
								master = Expression.AndAlso(master, cur);
							}
							else
							{
								master = Expression.Or(master, cur);
							}
						}
					}

					MethodCallExpression result = Expression.Call(
							typeof(Queryable), "Where",
							new[] { query.ElementType },
							query.Expression,
							Expression.Lambda(master, parameter));

					query = query.Provider.CreateQuery<T>(result);
				}
			}

			if (!string.IsNullOrWhiteSpace(ap.sidx) && "\"null\"" != ap.sidx && "null" != ap.sidx && "'null'" != ap.sidx)
			{
				sidxDefault = ap.sidx;

				// We only want to change the sort order if there is and index column.
				if (!string.IsNullOrWhiteSpace(ap.sord))
					sordDefault = ap.sord;
			}

			if(string.IsNullOrWhiteSpace(sordDefault))
				sordDefault = ap.sord;
			

			if (!string.IsNullOrEmpty(sidxDefault))
			{
				if (string.IsNullOrEmpty(sordDefault))
					sordDefault = ActionParams.CONST_sordAsc;
				var orderbyquery = query.OrderBy<T>(sidxDefault, sordDefault);
				query = orderbyquery;
			}

			//if (ap.rows > 1 && ap.page > 1)
			//{
			//	var skipquery = query.Skip(ap.rows * (ap.page - 1));
			//	query = skipquery;
			//}
			//if (ap.rows > 1)
			//{
			//	var takequery = query.Take(ap.rows);
			//	query = takequery;
			//}

			return query;
		}

		public static IQueryable<T> PagedList<T>(this IQueryable<T> query,
			ActionParams ap = null,
			int defaultPageSize = 0)
		{
			int pagesize = 0;
			int totalNumberOfRecords = 0;

			return PagedList(query, ref pagesize, ref totalNumberOfRecords, ap, defaultPageSize);
		}
		public static IQueryable<T> PagedList<T>(this IQueryable<T> query,
			ref int pagesize,
			ref int totalNumberOfRecords,
			ActionParams ap = null,
			int defaultPageSize = 0)
		{
			pagesize = (null != ap && ap.rows > 0 ? ap.rows : defaultPageSize);
			totalNumberOfRecords = query.Count();

			// Handle paging.
			// Start with skipping rows.
			if (null != ap && ap.rows > 1 && ap.page > 1)
			{
				var skipquery = query.Skip(ap.rows * (ap.page - 1));
				query = skipquery;
			}

			if (null != ap && ap.rows > 0)
			{
				var takequery = query.Take(ap.rows);
				query = takequery;
			}
			else if ((null == ap || ap.rows < 1) && pagesize > 0)
			{
				// Need to have a check for the first load.
				var tempquery = query.Take(pagesize);
				query = tempquery;
			}

			return query;
		}

		public static IQueryable<T> PagedList<T>(this IQueryable<T> query, GridModelBase gm, ActionParams ap = null, int defaultPageSize = 0)
		{
			int pagesize = 0;
			int totalNumberOfRecords = 0;

			query = PagedList(query, ref pagesize, ref totalNumberOfRecords, ap, defaultPageSize);

			//persons.Count % rows > 0 ? (persons.Count / rows) + 1 : (persons.Count / rows)
			int totalNumberOfPages = (pagesize < 1 ? 0 : totalNumberOfRecords / pagesize);
			if (pagesize < 1 || ((totalNumberOfRecords % pagesize) > 0))
				++totalNumberOfPages;

			if (null != gm)
			{
				gm.page = (null != ap && ap.page > 0 ? ap.page : 1);
				gm.records = totalNumberOfRecords;
				gm.total = totalNumberOfPages;
			}

			return query;
		}

		public static IEnumerable<T> PagedList<T>(this IEnumerable<T> query, GridModelBase gm, ActionParams ap = null, int defaultPageSize = 0)
		{
			int pagesize = (null != ap && ap.rows > 0 ? ap.rows : defaultPageSize);
			int totalNumberOfRecords = query.Count();
			//persons.Count % rows > 0 ? (persons.Count / rows) + 1 : (persons.Count / rows)
			int totalNumberOfPages = (pagesize < 1 ? 0 : totalNumberOfRecords / pagesize);
			if (pagesize < 1 || ((totalNumberOfRecords % pagesize) > 0))
				++totalNumberOfPages;

			// Handle paging.
			// Start with skipping rows.
			if (null != ap && ap.rows > 1 && ap.page > 1)
			{
				var skipquery = query.Skip(ap.rows * (ap.page - 1));
				query = skipquery;
			}

			if (null != ap && ap.rows > 0)
			{
				var takequery = query.Take(ap.rows);
				query = takequery;
			}
			else if ((null == ap || ap.rows < 1) && pagesize > 0)
			{
				// Need to have a check for the first load.
				var tempquery = query.Take(pagesize);
				query = tempquery;
			}

			if (null != gm)
			{
				gm.page = (null != ap && ap.page > 0 ? ap.page : 1);
				gm.records = totalNumberOfRecords;
				gm.total = totalNumberOfPages;
			}

			return query;
		}
	}
}
