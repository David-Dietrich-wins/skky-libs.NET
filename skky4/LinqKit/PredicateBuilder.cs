using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

/*
And instead of:

var inner = PredicateBuilder.False<Product>();
inner = inner.Or (p => p.Description.Contains ("foo"));
inner = inner.Or (p => p.Description.Contains ("far"));

var outer = PredicateBuilder.True<Product>();
outer = outer.And (p => p.Price > 100);
outer = outer.And (p => p.Price < 1000);
outer = outer.And (inner);

write:

var inner = PredicateBuilder.False<Product>();
inner = YourNamespaceHere.Utility.Or (inner, p => p.Description.Contains ("foo"));
inner = YourNamespaceHere.Utility.Or (inner, p => p.Description.Contains ("far"));

var outer = PredicateBuilder.True<Product>();
outer = YourNamespaceHere.Utility.And (outer, p => p.Price > 100);
outer = YourNamespaceHere.Utility.And (outer, p => p.Price < 1000);
outer = YourNamespaceHere.Utility.And (outer, inner);
*/

namespace LinqKit
{
	/// <summary>
	/// See http://www.albahari.com/expressions for information and examples.
	/// </summary>
	public static class PredicateBuilder
	{
		public static Expression<Func<T, bool>> True<T> () { return f => true; }
		public static Expression<Func<T, bool>> False<T> () { return f => false; }

		public static Expression<Func<T, bool>> Or<T> (this Expression<Func<T, bool>> expr1,
												  Expression<Func<T, bool>> expr2)
		{
			var invokedExpr = Expression.Invoke (expr2, expr1.Parameters.Cast<Expression> ());
			return Expression.Lambda<Func<T, bool>>
				 (Expression.OrElse (expr1.Body, invokedExpr), expr1.Parameters);
		}

		public static Expression<Func<T, bool>> And<T> (this Expression<Func<T, bool>> expr1,
												   Expression<Func<T, bool>> expr2)
		{
			var invokedExpr = Expression.Invoke (expr2, expr1.Parameters.Cast<Expression> ());
			return Expression.Lambda<Func<T, bool>>
				 (Expression.AndAlso (expr1.Body, invokedExpr), expr1.Parameters);
		}
	}
}
