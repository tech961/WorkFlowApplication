using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq.Dynamic.Core;

public static class DbExpressionEngine
{
    public static bool Evaluate(object model, string dbExpression, IFieldResolver resolver)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (resolver == null) throw new ArgumentNullException(nameof(resolver));
        if (string.IsNullOrWhiteSpace(dbExpression)) return false;

        var modelType = model.GetType();

        var expr = dbExpression;

        expr = Regex.Replace(expr, @"\bAND\b", "&&", RegexOptions.IgnoreCase);
        expr = Regex.Replace(expr, @"\bOR\b", "||", RegexOptions.IgnoreCase);

        expr = Regex.Replace(expr, @"(?<![<>=!])=(?![=])", "==");

        expr = FixTypedComparisons(expr, modelType, resolver);

        return EvaluateByRuntimeType(model, expr);
    }

    private static string FixTypedComparisons(string expr, Type modelType, IFieldResolver resolver)
    {
        return Regex.Replace(expr, @"\[(?<field>[^\]]+)\]\s*(?<op>==|!=|>=|<=|>|<)\s*(?<val>[^&|]+)", m =>
        {
            var fieldFa = m.Groups["field"].Value.Trim();
            var op = m.Groups["op"].Value;
            var rawVal = m.Groups["val"].Value.Trim();

            PropertyInfo prop;
            if (!resolver.TryResolve(modelType, fieldFa, out prop))
                throw new InvalidOperationException("Unknown field: [" + fieldFa + "] for model " + modelType.Name);

            var isQuoted = (rawVal.Length >= 2) &&
                           ((rawVal.StartsWith("'") && rawVal.EndsWith("'")) ||
                            (rawVal.StartsWith("\"") && rawVal.EndsWith("\"")));

            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            if (propType == typeof(string))
            {
                if (!isQuoted)
                    rawVal = "\"" + rawVal.Replace("\"", "\\\"") + "\"";

                return prop.Name + " " + op + " " + rawVal;
            }

            if (IsNumericType(propType))
            {
                if (isQuoted)
                    rawVal = rawVal.Substring(1, rawVal.Length - 2);

                rawVal = FaDigitsToEn(rawVal);

                decimal tmp;
                if (!decimal.TryParse(rawVal, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp))
                    throw new InvalidOperationException("Value '" + rawVal + "' is not numeric for field [" + fieldFa + "]");

                return prop.Name + " " + op + " " + rawVal;
            }

            return prop.Name + " " + op + " " + rawVal;
        });
    }

    private static bool EvaluateByRuntimeType(object model, string expr)
    {
        var mi = typeof(DbExpressionEngine).GetMethod("EvaluateGeneric", BindingFlags.NonPublic | BindingFlags.Static);
        var g = mi.MakeGenericMethod(model.GetType());
        return (bool)g.Invoke(null, new object[] { model, expr });
    }

    private static bool EvaluateGeneric<T>(T model, string expr)
    {
        return new[] { model }.AsQueryable().Any(expr);
    }

    private static bool IsNumericType(Type t)
    {
        return t == typeof(byte) || t == typeof(short) || t == typeof(int) || t == typeof(long)
            || t == typeof(float) || t == typeof(double) || t == typeof(decimal);
    }

    private static string FaDigitsToEn(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return s.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4")
                .Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9");
    }
}