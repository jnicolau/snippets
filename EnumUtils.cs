
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Web.AdminTool.Utilities.Web;

namespace Web.AdminTool.Utilities.Enums
{
    public static class EnumUtils
    {
        /// <summary>
        /// Converts the number to an enum. A return value indicates whether the conversion succeeded.
        /// Throws ArgumentException if T is not a valid enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="returnValue">The return value.</param>
        /// <returns></returns>
        public static bool TryParseAsEnum<T>(this int rawValue, out T returnValue) where T : struct
        {
            if (typeof(T).BaseType != typeof(Enum))
                throw new ArgumentException("Please use a valid enum as T.");

            returnValue = default(T);

            if (Enum.IsDefined(typeof(T), rawValue))
            {
                returnValue = (T)(object)rawValue;
                return true;
            }

            return false;
        }

        /// <summary>
        /// This is used to cache the descriptions, instead of use reflection every time.
        /// </summary>
        private static readonly Dictionary<Enum, string> cachedEnumLongDescriptions = new Dictionary<Enum, string>();

        /// <summary>
        /// This is used to cache the descriptions, instead of use reflection every time.
        /// </summary>
        private static readonly Dictionary<Enum, string> cachedEnumDescriptions = new Dictionary<Enum, string>();

        /// <summary>
        /// This extension method returns the value of a Descripotion attribute set up on the enum.
        /// </summary>
        /// <remarks>
        /// Usage example: GameCurrency.Real.ToDescriptionString() will return "Real Money". 
        /// </remarks>
        /// <param name="val">The enum value (i.e.: GameCurrency.Real).</param>
        /// <returns></returns>
        public static string ToDescriptionString(this Enum val)
        {
            string result = null;

            lock (cachedEnumDescriptions)
            {
                result = cachedEnumDescriptions.ContainsKey(val) ? cachedEnumDescriptions[val] : GetAndCacheDescription(val, result);
            }

            return result ?? string.Empty;
        }

        public static string GetDescription(this Enum val)
        {
            return ToDescriptionString(val);
        }


        /// <summary>
        /// This extension method returns the value of a Descripotion attribute set up on the enum.
        /// </summary>
        /// <remarks>
        /// Usage example: GameCurrency.Real.ToDescriptionString() will return "Real Money". 
        /// </remarks>
        /// <param name="val">The enum value (i.e.: GameCurrency.Real).</param>
        /// <returns></returns>
        public static string ToLongDescriptionString(this Enum val)
        {
            string result = null;

            lock (cachedEnumLongDescriptions)
            {
                result = cachedEnumLongDescriptions.ContainsKey(val) ? cachedEnumLongDescriptions[val] : GetAndCacheLongDescription(val, result);
            }

            return result ?? string.Empty;
        }

        private static string GetAndCacheDescription(Enum val, string result)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                result = attributes[0].Description;
                cachedEnumDescriptions.Add(val, attributes[0].Description);
            }
            return result;
        }

        private static string GetAndCacheLongDescription(Enum val, string result)
        {
            LongDescriptionAttribute[] attributes = (LongDescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(LongDescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                result = attributes[0].LongDescription;
                cachedEnumLongDescriptions.Add(val, attributes[0].LongDescription);
            }
            return result;
        }

        public static IEnumerable<T> GetAllValues<T>()
            where T : struct
        {
            if (typeof(T).BaseType != typeof(Enum)) throw new ArgumentException("Generic parameter T must be an enum.");
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        // IS THIS EVEN REQUIRED - CANT I DATABIND ENUM LIST????

        //public static IEnumerable<KeyValuePair<string, T>> GetAllValuesWithDescriptions<T>()
        //    where T: struct, System.Int32
        //{
        //    if (typeof(T).BaseType != typeof(Enum)) throw new ArgumentException("Generic parameter T must be an enum.");

        //    var values = new List<KeyValuePair<string, T>>();
        //    foreach(var value in GetAllValues<T>())
        //    {
        //        string description = null;
        //        description = cachedEnumDescriptions.ContainsKey(value) ? cachedEnumDescriptions[value] : GetAndCacheResult(value, description);
        //        DescriptionAttribute[] attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
        //        //values.Add(new KeyValuePair<string, T>(value as T) 
        //    }

        //    return values;
        //}

        /// <summary>
        /// Gets all the descriptions from a given enum type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetDescriptionsFor<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).OfType<Enum>()
                .Select(t => t.ToDescriptionString()).ToArray();
        }

        public static T[] GetValuesFor<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).OfType<T>().ToArray();
        }

        public static ValueDescription[] GetValueDescriptionsFor<T>() where T : struct
        {
            var descriptions = EnumUtils.GetDescriptionsFor<T>();
            var values = (T[])Enum.GetValues(typeof(T));
            var list = new List<ValueDescription>();
            for (int i = 0; i < values.Length; i++)
            {
                list.Add(new ValueDescription { Value = values[i].ToString(), Text = descriptions[i] });
            }
            return list.ToArray();
        }


        public static Dictionary<string, T> GetDictionaryFor<T>() where T : struct
        {
            var names = Enum.GetNames(typeof(T));
            var values = (T[])Enum.GetValues(typeof(T));
            var dict = new Dictionary<string, T>();
            for (int i = 0; i < values.Length; i++)
            {
                dict.Add(names[i], values[i]);
            }
            return dict;
        }

        public static Dictionary<string, string> GetDescriptionsDictionaryFor<T>() where T : struct
        {
            var type = typeof (T);
            var underlyingType = Enum.GetUnderlyingType(type);
            var enumValues = Enum.GetValues(type);
            var values = Array.CreateInstance(underlyingType, enumValues.Length);
            enumValues.CopyTo(values, 0);
            var descriptions = GetDescriptionsFor<T>();
            var dict = new Dictionary<string, string>();
            int i = 0;
            foreach (var value in values)
            {
                dict.Add(value.ToString(), descriptions[i]);
                i++;

            }
            return dict;
        }

    }

    [DataContract]
    public class ValueDescription
    {
        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string Text { get; set; }
    }

}
