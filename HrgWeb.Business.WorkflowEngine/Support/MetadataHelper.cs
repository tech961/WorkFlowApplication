using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HrgWeb.Business.WorkflowEngine.Support
{

    public static class MetadataHelper
    {
        public static bool TryGetMetadataValue(object metadataModel, string key, out object value)
        {
            value = null;
            if (metadataModel == null || string.IsNullOrWhiteSpace(key))
                return false;

            if (metadataModel is IDictionary<string, object> dictObj)
                return dictObj.TryGetValue(key, out value);

            if (metadataModel is IDictionary<string, string> dictStr && dictStr.TryGetValue(key, out var strVal))
            {
                value = strVal;
                return true;
            }

            if (metadataModel is IReadOnlyDictionary<string, object> roDictObj)
                return roDictObj.TryGetValue(key, out value);

            var metaType = metadataModel.GetType();

            var indexer = metaType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p =>
                    p.GetIndexParameters().Length == 1 &&
                    p.GetIndexParameters()[0].ParameterType == typeof(string) &&
                    p.CanRead);

            if (indexer != null)
            {
                try
                {
                    value = indexer.GetValue(metadataModel, new object[] { key });
                    return value != null;
                }
                catch
                {
                }
            }

            var prop = metaType.GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop != null && prop.CanRead)
            {
                value = prop.GetValue(metadataModel);
                return true;
            }

            return false;
        }
    }
}
