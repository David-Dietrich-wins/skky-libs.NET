using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Reflection;

namespace skky.util
{
	public static class ExtensionsJson
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger("ExtensionsJson");

		public class ShouldSerializeListContractResolver : DefaultContractResolver
		{
			public static readonly ShouldSerializeListContractResolver Instance = new ShouldSerializeListContractResolver();

			protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
			{
				JsonProperty property = base.CreateProperty(member, memberSerialization);

				bool isDefaultValueIgnored =
					((property.DefaultValueHandling ?? DefaultValueHandling.Ignore)
						& DefaultValueHandling.Ignore) != 0;

				if (isDefaultValueIgnored
						&& !typeof(string).IsAssignableFrom(property.PropertyType)
						&& typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
				{
					Predicate<object> newShouldSerialize = obj => {
						var collection = property.ValueProvider.GetValue(obj) as ICollection;
						// null == collection will still not get serialized since we are using DefaultNullHandling.
						// What it does fix is EF always showing collections as null.
						return null == collection || collection.Count > 0;
					};

					Predicate<object> oldShouldSerialize = property.ShouldSerialize;
					property.ShouldSerialize = oldShouldSerialize != null
						? o => oldShouldSerialize(o) && newShouldSerialize(o)
						: newShouldSerialize;
				}

				return property;
			}
		}

		public static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.Ignore,
			ContractResolver = ShouldSerializeListContractResolver.Instance,
			Formatting = Formatting.None,
			DateTimeZoneHandling = DateTimeZoneHandling.Utc,
		};

		public static string SerializeObject(this object o)
		{
			return JsonSerialize(o, false, null);
		}


		/// <summary>
		/// Returns a serialized iot object.
		/// Returns null if iotObject is null or this is an error.
		/// Typically used for saving to an IotCommand.json[xxx] field in the database. Hence the null return.
		/// </summary>
		/// <param name="iotObject"></param>
		/// <returns></returns>
		public static string SerializeNoException(this object iotObject)
		{
			try
			{
				return iotObject.JsonSerialize(true);
			}
			catch (Exception ex)
			{
				log.Error("SerializeNoException", ex);
			}

			return null;
		}

		public static string JsonSerialize(this object o, bool returnNullIfEmpty = false, JsonSerializerSettings serializerSettings = null)
		{
			if (null == o)
				return (returnNullIfEmpty ? null : string.Empty);

			//return (new JavaScriptSerializer()).Serialize(o);
			return JsonConvert.SerializeObject(o, Formatting.None, null == serializerSettings ? DefaultJsonSerializerSettings : serializerSettings);
		}

		public static T JsonDeserialize<T>(this string s, bool returnNullIfEmpty = true) where T : class, new()
		{
			T item = null;

			if (!string.IsNullOrEmpty(s))
			{
				try
				{
					// return (new JavaScriptSerializer()).Deserialize<T>(s);
					item = JsonConvert.DeserializeObject<T>(s);
				}
				catch (Exception ex)
				{
					log.Error("JsonDeserialize", ex);
					log.Error("Type: " + typeof(T).Name + ". String: " + s);
				}
			}

			return (null == item && !returnNullIfEmpty ? new T() : item);
		}
		public static object JsonDeserializeType(string s, Type t, bool returnNullIfEmpty = true)
		{
			MethodInfo openMethod = typeof(ExtensionsJson).GetMethod("JsonDeserialize");
			MethodInfo typedMethod = openMethod.MakeGenericMethod(t);

			object o = typedMethod.Invoke(null, new object[] { s, returnNullIfEmpty });
			return o;
		}

		public static T JsonDeserializeDynamic<T>(dynamic o, bool returnNullIfEmpty = true) where T : class, new()
		{
			if (null == o)
				return (returnNullIfEmpty ? null : new T());

			return JsonDeserialize<T>(Convert.ToString(o), returnNullIfEmpty);
		}
	}
}
