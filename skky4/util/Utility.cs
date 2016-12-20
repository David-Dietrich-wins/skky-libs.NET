using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace skky.util
{
	public static class Utility
	{
		public const string CONST_CopyFromDefaultExcludeColumns = "id,actionedby,actionedon,createdby,createdon,updatedBy,updatedOn";
		/// <summary>
		/// Copies from one object, the source, to the destination object using Reflection.
		/// The only member objects copied to the the destination,
		///  are the ones that contain the same exact named fields in both the source and destination objects.
		/// </summary>
		/// <typeparam name="T1">Any class object.</typeparam>
		/// <typeparam name="T2">Any class object</typeparam>
		/// <param name="src">The T1 source object to copy from.</param>
		/// <param name="dest">The T2 destination object to copy to.</param>
		/// <param name="skipIDCreateActioned">Skip the ID, actionedBy, actionedOn, createdBy and createdOn columns from the copy..</param>
		/// <returns>The T1 used to call this method.</returns>
		public static T1 CopyFrom<T1, T2>(this T1 dest, T2 src, bool skipIDCreateActioned = false, bool copyPrimitiveTypesOnly = true)
			where T1 : class
			where T2 : class
		{
			List<string> changedFields = null;

			return dest.CopyFrom(src, skipIDCreateActioned ? CONST_CopyFromDefaultExcludeColumns : null, copyPrimitiveTypesOnly, ref changedFields);
		}

		public static T1 CopyFrom<T1, T2>(this T1 dest, T2 src, bool skipIDCreateActioned, bool copyPrimitiveTypesOnly, ref List<string> changedFields)
			where T1 : class
			where T2 : class
		{
			return dest.CopyFrom(src, skipIDCreateActioned ? CONST_CopyFromDefaultExcludeColumns : null, copyPrimitiveTypesOnly, ref changedFields);
		}

		public static T1 CopyFrom<T1, T2>(this T1 dest, T2 src, string ignoreList, bool copyPrimitiveTypesOnly = true)
			where T1 : class
			where T2 : class
		{
			List<string> changedFields = null;

			return CopyFrom(dest, src, ignoreList, copyPrimitiveTypesOnly, ref changedFields);
		}

		/// <summary>
		/// Copies from one object, the source, to the destination object using Reflection.
		/// The only member objects copied to the the destination,
		///  are the ones that contain the same exact named fields in both the source and destination objects.
		/// </summary>
		/// <typeparam name="T1">Any class object.</typeparam>
		/// <typeparam name="T2">Any class object</typeparam>
		/// <param name="src">The T1 source object to copy from.</param>
		/// <param name="dest">The T2 destination object to copy to.</param>
		/// <returns>The T1 used to call this method.</returns>
		public static T1 CopyFrom<T1, T2>(this T1 dest, T2 src, string ignoreList, bool copyPrimitiveTypesOnly, ref List<string> changedFields)
			where T1 : class
			where T2 : class
		{
			if (null != src)
			{
				List<string> lstIgnore = Parser.SplitAndTrimString(ignoreList).ToList();

				PropertyInfo[] srcFields = src.GetType().GetProperties(
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

				PropertyInfo[] destFields = dest.GetType().GetProperties(
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

				foreach (var property in srcFields)
				{
					if (lstIgnore.Contains(property.Name) || lstIgnore.Contains(property.Name.ToLower()))
						continue;

					if (copyPrimitiveTypesOnly)
					{
						if(!property.PropertyType.IsPrimitive && !property.PropertyType.IsValueType)
						{
						Type type = property.PropertyType;
						if(type != typeof(string)
							&& type != typeof(decimal)
							&& type != typeof(DateTime)
							&& type != typeof(DateTimeOffset)
							&& type != typeof(TimeSpan)
							&& type != typeof(Guid)
							&& (type == typeof(object) || Type.GetTypeCode(type) == TypeCode.Object))
							continue;
						}
					}

					var destField = destFields.FirstOrDefault(x => x.Name == property.Name);
					if (null != destField && destField.CanWrite)
					{
						var srcValue = property.GetValue(src, null);

						if (null != changedFields)
						{
							bool changed = false;

							var destValue = destField.GetValue(dest, null);
							//var srcField = srcFields.FirstOrDefault(x => x.Name == property.Name);
							//if (null != srcField)
							//{
								//var srcValue = srcField.GetValue(src, null);

								if ((null != destValue && !destValue.Equals(srcValue))
								|| (null != srcValue && !srcValue.Equals(destValue)))
									changed = true;
							//}

							if(changed)
								changedFields.Add(destField.Name);
						}

						destField.SetValue(dest, srcValue, null);
					}
				}
			}

			return dest;
		}
		public static T CopyForSerialization<T>(this T from, T to = null) where T : class, new()
		{
			if (null == to)
				to = new T();

			to.CopyFrom(from);

			return to;
		}

		public static List<T> CopyForSerialization<T>(this IEnumerable<T> from, List<T> to = null) where T : class, new()
		{
			if(null == to)
				to = new List<T>();

			if(null != from)
			{
				foreach (var fromitem in from)
					to.Add(fromitem.CopyForSerialization());
			}

			return to;
		}
	}
}
