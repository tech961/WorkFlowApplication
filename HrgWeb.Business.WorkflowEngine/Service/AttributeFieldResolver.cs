using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

public interface IFieldResolver
{
    bool TryResolve(Type modelType, string aliasFa, out PropertyInfo property);
}

public sealed class AttributeFieldResolver : IFieldResolver
{
    private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> _cache
        = new ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>>();

    public bool TryResolve(Type modelType, string aliasFa, out PropertyInfo property)
    {
        Dictionary<string, PropertyInfo> map;
        if (!_cache.TryGetValue(modelType, out map))
        {
            map = BuildMap(modelType);
            _cache[modelType] = map;
        }

        return map.TryGetValue(NormalizeFa(aliasFa), out property);
    }

    private static Dictionary<string, PropertyInfo> BuildMap(Type t)
    {
        var map = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

        foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var customFa = TryGetCustomDisplayName(prop);
            if (!string.IsNullOrWhiteSpace(customFa))
                map[NormalizeFa(customFa)] = prop;

            var display = prop.GetCustomAttribute<DisplayAttribute>();
            if (display != null && !string.IsNullOrWhiteSpace(display.Name))
                map[NormalizeFa(display.Name)] = prop;

            var dn = prop.GetCustomAttribute<DisplayNameAttribute>();
            if (dn != null && !string.IsNullOrWhiteSpace(dn.DisplayName))
                map[NormalizeFa(dn.DisplayName)] = prop;

            // optional: property name
            map[NormalizeFa(prop.Name)] = prop;
        }

        return map;
    }

    private static string TryGetCustomDisplayName(PropertyInfo prop)
    {
        var attr = prop.GetCustomAttributes(true)
            .FirstOrDefault(a =>
                a.GetType().Name == "CustomDisplayNameAttribute" ||
                a.GetType().Name == "CustomDisplayName");

        if (attr == null) return null;

        var at = attr.GetType();

        // اگر attribute string مستقیم داشت
        var nameProp = at.GetProperty("Name");
        if (nameProp != null && nameProp.PropertyType == typeof(string))
        {
            var val = nameProp.GetValue(attr, null) as string;
            if (!string.IsNullOrWhiteSpace(val)) return val;
        }

        // ResourceType + Key
        var resourceTypeProp = at.GetProperty("ResourceType");
        var keyProp = at.GetProperty("Key");

        if (resourceTypeProp != null && keyProp != null)
        {
            var resourceType = resourceTypeProp.GetValue(attr, null) as Type;
            var key = keyProp.GetValue(attr, null) as string;

            if (resourceType != null && !string.IsNullOrWhiteSpace(key))
            {
                var rmProp = resourceType.GetProperty("ResourceManager",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (rmProp != null)
                {
                    var rm = rmProp.GetValue(null, null) as ResourceManager;
                    if (rm != null)
                        return rm.GetString(key);
                }
            }
        }

        return null;
    }

    private static string NormalizeFa(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        s = s.Trim()
             .Replace('ي', 'ی')
             .Replace('ك', 'ک')
             .Replace("\u200C", " ");
        s = Regex.Replace(s, @"\s+", " ");
        return s.Trim();
    }
}