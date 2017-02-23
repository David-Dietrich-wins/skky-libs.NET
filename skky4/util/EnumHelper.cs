using skky.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace skky.util
{
	public static class EnumHelper<T>
	{
		public static List<T> GetValues()
		{
			var enumValues = new List<T>();

			foreach (FieldInfo fi in typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				enumValues.Add((T)Enum.Parse(typeof(T), fi.Name, false));
			}

			return enumValues;
		}

		public static List<idName> GetIdNames()
		{
			var idnames = new List<idName>();

			foreach (FieldInfo fi in typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				T ten = (T)Enum.Parse(typeof(T), fi.Name, false);
				if(null != ten)
					idnames.Add(new idName(Convert.ToInt32(ten), GetDisplayName(ten)));
			}

			return idnames;
		}

		public static T Parse(string value)
		{
			return (T)Enum.Parse(typeof(T), value, true);
		}
		public static T ParseDisplayName(string displayName)
		{
			foreach (FieldInfo fi in typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				T ten = (T)Enum.Parse(typeof(T), fi.Name, false);
				if (null != ten && GetDisplayName(ten) == displayName)
					return ten;
			}

			return (T)Enum.Parse(typeof(T), displayName, true);
		}

		public static List<string> GetNames()
		{
			return typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
		}

		public static List<string> GetDisplayNames()
		{
			return GetNames().Select(obj => GetDisplayName(Parse(obj))).ToList();
		}

		public static string GetDisplayName(T value)
		{
			var fieldInfo = value.GetType().GetField(value.ToString());

			var descriptionAttributes = fieldInfo.GetCustomAttributes(
				typeof(DisplayAttribute), false) as DisplayAttribute[];

			if (descriptionAttributes == null)
				return string.Empty;

			return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
		}
	}
}
