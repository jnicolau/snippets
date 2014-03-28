using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Threading;
using ETC.CommonTypes.ServiceTypes;
using PaymentService.DataContracts;

namespace PaymentsWeb.Resources
{
    public class CustomResourceManager : ResourceManager
    {
        public static readonly String SessionDomainKey = "CustomResourceManager_PlayerDomain";

        private Dictionary<string, string[]> _cachedKeys = new Dictionary<string, string[]>();
        private Dictionary<string, Dictionary<string, string>> _cache = new Dictionary<string, Dictionary<string, string>>();

        public CustomResourceManager(string baseName, Assembly assembly)
            : base(baseName, assembly)
        {
        }

        public CustomResourceManager(string baseName, Assembly assembly, Type usingResourceSet)
            : base(baseName, assembly, usingResourceSet)
        {
        }

        public CustomResourceManager(Type resourceSource)
            : base(resourceSource)
        {
        }

        /// <summary>
        /// Gets the value of the <see cref="T:System.String" /> resource localized for the specified culture.
        /// </summary>
        /// <param name="name">The name of the resource to get.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> object that represents the culture for which the resource is localized. Note that if the resource is not localized for this culture, the lookup will fall back using the current thread's <see cref="P:System.Globalization.CultureInfo.Parent" /> property, stopping after looking in the neutral culture.If this value is null, the <see cref="T:System.Globalization.CultureInfo" /> is obtained using the current thread's <see cref="P:System.Globalization.CultureInfo.CurrentUICulture" /> property.</param>
        /// <returns>
        /// The value of the resource localized for the specified culture. If a best match is not possible, null is returned.
        /// </returns>
        public override string GetString(string name, CultureInfo culture)
        {
            // If culture is null the current culture is assumed
            culture = culture ?? Thread.CurrentThread.CurrentUICulture;

            // Gets the most specialized key that matches with the user-agent string
            var mostSpecializedKey = GetMostSpecializedKey(name, culture);

            // Get the value for that key
            string value = null;
            // If the localized value doesn't exist, uses invariant culture in order to access the default resource file
            if (!GetCachedResourcesFor(culture).TryGetValue(mostSpecializedKey, out value) && culture != CultureInfo.InvariantCulture)
                GetCachedResourcesFor(CultureInfo.InvariantCulture).TryGetValue(mostSpecializedKey, out value);
            return value;
        }

        /// <summary>
        /// Gets cached resources for a given culture.
        /// </summary>
        /// <remarks>We can't rely on the base method GetString(name, culture) for getting localised keys with special characters, it fails, therefore we have implemented our own system.</remarks>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        private Dictionary<string, string> GetCachedResourcesFor(CultureInfo culture)
        {
            Dictionary<string, string> resources = null;

            // Check if culture is already cached
            if (_cache.TryGetValue(culture.Name, out resources))
                return resources;

            // Get resource set and transform into a dictionary
            var entries = GetResourceSet(culture, true, true).Cast<DictionaryEntry>();
            resources = entries.ToDictionary(entry => (string)entry.Key, entry => (string)entry.Value);
            _cache.Add(culture.Name, resources);
            return resources;

        }

        /// <summary>
        /// Gets the most specialized key that matches with the user-agent string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        private string GetMostSpecializedKey(string name, CultureInfo culture)
        {
            // Make sure the keys are cached for a faster access
            string[] keys = null;
            var cultureName = culture.Name;
            if (!_cachedKeys.TryGetValue(cultureName, out keys))
            {
                var allKeys = GetResourceSet(culture, true, true).Cast<DictionaryEntry>().Select(dictionaryEntry => (string)dictionaryEntry.Key);
                // Only need to cache keys that have "."
                keys = allKeys.Where(key => key.Contains(".")).ToArray();
                _cachedKeys.Add(cultureName, keys);
            }

            // Narrow down to keys that start like name
            keys = keys.Where(key => key.StartsWith(name + ".") && key.Length > name.Length).ToArray();

            // Exists a more specialized key?
            if (!keys.Any())
                return name;

            // Group and sort by depth of specialization
            var keysOrderedByDepth = keys.GroupBy(key => key.Count(x => x == '.'), key => key)
                                         .OrderByDescending(grp => grp.Key);

            // Check if any of those keys matches the user-agent (from more to less depth of specialization)
            foreach (var keysWithSameDepth in keysOrderedByDepth)
            {
                // Sort alphabetically
                var sortedKeys = keysWithSameDepth.OrderBy(key => key);

                // Is there a match?
                var matchedKey = sortedKeys.FirstOrDefault(key => IsAMatch(key, culture));
                if (matchedKey != null)
                    return matchedKey;
            }

            // If there are no matches returns the name
            return name;
        }

        /// <summary>
        /// Determines whether the specified key is a match.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        ///   <c>true</c> if the specified key matches; otherwise, <c>false</c>.
        /// </returns>
        static private bool IsAMatch(string key, CultureInfo culture)
        {
            var httpContext = System.Web.HttpContext.Current;

            // If HttpContext or UserAget are not available there will be no matches
            if (httpContext== null || httpContext.Request.UserAgent == null)
                return false;

            var userAgent = System.Web.HttpContext.Current.Request.UserAgent;
            Domain? domain = null;
            if(httpContext.Session != null)
            {
                Domain d;
                var o = httpContext.Session[SessionDomainKey];
                if (o != null)
                {
                    if (Enum.TryParse(o.ToString(), out d))
                        domain = d;
                }
            }

            // Every part of the name is separated by "."
            var parts = key.Split('.');

            // Keys with no dots
            if (parts.Length < 2)
                throw new ArgumentException("The key is not qualified to be used by this method.");

            // Every part after the first dot must exist in the user-agent string in order to be a match
            var r = parts.Skip(1).All(part => culture.CompareInfo.IndexOf(userAgent, part, CompareOptions.IgnoreCase) > 0 ||
                                             (domain.HasValue?domain.Value.ToString().Equals(part,StringComparison.InvariantCultureIgnoreCase):false));

            return r;
        }
    }
}
