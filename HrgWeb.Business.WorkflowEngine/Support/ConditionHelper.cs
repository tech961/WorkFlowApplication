using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HrgWeb.Business.WorkflowEngine.Support
{
    public static class ConditionHelper
    {
        public static string ToDynamicLiteral(string rawVal, Type propertyType)
        {
            string v = (rawVal ?? "").Trim();

            bool quoted = v.Length >= 2 && ((v[0] == '\'' && v[v.Length - 1] == '\'') || (v[0] == '"' && v[v.Length - 1] == '"'));
            if (quoted) v = v.Substring(1, v.Length - 2);

            v = FaDigitsToEn(v);

            Type t = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (IsYesNo(v))
            {
                if (t == typeof(bool))
                    return IsYes(v) ? "true" : "false";

                if (IsNumericType(t))
                    return IsYes(v) ? "1" : "0";

                return QuoteString(v);
            }

            if (t == typeof(string))
                return QuoteString(v);

            if (IsNumericType(t))
            {
                decimal tmp;
                if (!decimal.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp))
                    throw new InvalidOperationException("Value '" + v + "' is not numeric for numeric field.");
                return v;
            }

            if (t == typeof(bool))
            {
                if (v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase)) return "true";
                if (v == "0" || v.Equals("false", StringComparison.OrdinalIgnoreCase)) return "false";
                return "false";
            }

            return quoted ? QuoteString(v) : v;
        }

        public static bool IsYesNo(string v)
        {
            if (string.IsNullOrWhiteSpace(v)) return false;
            v = v.Trim();
            return v == "بله" || v == "خیر" || v.Equals("yes", StringComparison.OrdinalIgnoreCase) || v.Equals("no", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsYes(string v)
        {
            v = (v ?? "").Trim();
            return v == "بله" || v.Equals("yes", StringComparison.OrdinalIgnoreCase);
        }

        public static string QuoteString(string v)
        {
            v = (v ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
            return "\"" + v + "\"";
        }

        public static bool IsNumericType(Type t)
        {
            return t == typeof(byte) || t == typeof(short) || t == typeof(int) || t == typeof(long)
                || t == typeof(float) || t == typeof(double) || t == typeof(decimal);
        }

        public static string FaDigitsToEn(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4")
                    .Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9");
        }

        public static string NormalizeLogicalOperatorsFa(string expr)
        {
            if (string.IsNullOrWhiteSpace(expr)) return expr;

            // " و " -> &&
            expr = Regex.Replace(expr, @"\s+و\s+", " && ", RegexOptions.CultureInvariant);

            // " یا " -> ||
            expr = Regex.Replace(expr, @"\s+یا\s+", " || ", RegexOptions.CultureInvariant);

            return expr;
        }

        public static string NormalizePersianOperators(string expr)
        {
            if (string.IsNullOrWhiteSpace(expr)) return expr;

            expr = Regex.Replace(expr, @"\s+", " ").Trim();

            expr = Regex.Replace(expr, @"\bبزرگتراز\b", " >", RegexOptions.CultureInvariant);

            expr = Regex.Replace(expr, @"\bکوچکتراز\b", " <", RegexOptions.CultureInvariant);

            expr = Regex.Replace(expr, @"\bبزرگترمساوی\b", " >=", RegexOptions.CultureInvariant);

            expr = Regex.Replace(expr, @"\bکوچکترمساوی\b", " <=", RegexOptions.CultureInvariant);

            expr = Regex.Replace(expr, @"\bبرابربا\b", " = ", RegexOptions.CultureInvariant);

            expr = Regex.Replace(expr, @"\bمخالف\b", " != ", RegexOptions.CultureInvariant);

            return expr;
        }
    }
}
