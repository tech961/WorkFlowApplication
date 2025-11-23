using System;
using System.Collections.Generic;
using System.Globalization;
using SimpleWorkflowEngine.Runtime;

namespace SimpleWorkflowEngine.Service
{
    /// <summary>
    /// Evaluates simple boolean and date expressions against an execution context.
    /// </summary>
    public sealed class DefaultExpressionEvaluator : IExpressionEvaluator
    {
        public bool EvaluateCondition(IExecutionContext context, string expression)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(expression))
            {
                return false;
            }

            string trimmed = expression.Trim();

            if (trimmed.StartsWith("!", StringComparison.Ordinal))
            {
                string innerExpression = trimmed.Substring(1);
                return !EvaluateCondition(context, innerExpression);
            }

            if (string.Equals(trimmed, "true", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (string.Equals(trimmed, "false", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string left;
            string right;
            if (TrySplit(trimmed, "==", out left, out right))
            {
                object leftValue = ResolveValue(context, left);
                object rightValue = ResolveValue(context, right);
                return AreEqual(leftValue, rightValue);
            }

            if (TrySplit(trimmed, "!=", out left, out right))
            {
                object leftValue = ResolveValue(context, left);
                object rightValue = ResolveValue(context, right);
                return !AreEqual(leftValue, rightValue);
            }

            if (TrySplit(trimmed, ">=", out left, out right))
            {
                return CompareValues(ResolveValue(context, left), ResolveValue(context, right)) >= 0;
            }

            if (TrySplit(trimmed, "<=", out left, out right))
            {
                return CompareValues(ResolveValue(context, left), ResolveValue(context, right)) <= 0;
            }

            if (TrySplit(trimmed, ">", out left, out right))
            {
                return CompareValues(ResolveValue(context, left), ResolveValue(context, right)) > 0;
            }

            if (TrySplit(trimmed, "<", out left, out right))
            {
                return CompareValues(ResolveValue(context, left), ResolveValue(context, right)) < 0;
            }

            object value = ResolveValue(context, trimmed);
            return ConvertToBoolean(value);
        }

        public DateTime EvaluateDateTime(IExecutionContext context, string expression)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(expression))
            {
                return DateTime.UtcNow;
            }

            object value = ResolveValue(context, expression);

            if (value is DateTime)
            {
                return ((DateTime)value).ToUniversalTime();
            }

            if (value is TimeSpan)
            {
                return DateTime.UtcNow.Add((TimeSpan)value);
            }

            string text = value as string;
            if (text != null)
            {
                text = text.Trim();
                if (string.Equals(text, "now", StringComparison.OrdinalIgnoreCase))
                {
                    return DateTime.UtcNow;
                }

                DateTime parsedDateTime;
                if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out parsedDateTime))
                {
                    return parsedDateTime;
                }

                TimeSpan parsedTimeSpan;
                if (TimeSpan.TryParse(text, CultureInfo.InvariantCulture, out parsedTimeSpan))
                {
                    return DateTime.UtcNow.Add(parsedTimeSpan);
                }
            }

            throw new InvalidOperationException(string.Format("Expression '{0}' could not be resolved to a date value.", expression));
        }

        private static bool TrySplit(string expression, string separator, out string left, out string right)
        {
            int index = expression.IndexOf(separator, StringComparison.Ordinal);
            if (index <= 0)
            {
                left = null;
                right = null;
                return false;
            }

            left = expression.Substring(0, index);
            right = expression.Substring(index + separator.Length);
            return true;
        }

        private static object ResolveValue(IExecutionContext context, string token)
        {
            if (token == null)
            {
                return null;
            }

            string trimmed = token.Trim();
            if (trimmed.Length == 0)
            {
                return null;
            }

            if (trimmed.StartsWith("\"", StringComparison.Ordinal) && trimmed.EndsWith("\"", StringComparison.Ordinal) && trimmed.Length >= 2)
            {
                return trimmed.Substring(1, trimmed.Length - 2);
            }

            if (trimmed.StartsWith("'", StringComparison.Ordinal) && trimmed.EndsWith("'", StringComparison.Ordinal) && trimmed.Length >= 2)
            {
                return trimmed.Substring(1, trimmed.Length - 2);
            }

            bool boolValue;
            if (bool.TryParse(trimmed, out boolValue))
            {
                return boolValue;
            }

            int intValue;
            if (int.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue))
            {
                return intValue;
            }

            decimal decimalValue;
            if (decimal.TryParse(trimmed, NumberStyles.Number, CultureInfo.InvariantCulture, out decimalValue))
            {
                return decimalValue;
            }

            DateTime dateValue;
            if (DateTime.TryParse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out dateValue))
            {
                return dateValue;
            }

            TimeSpan timeSpanValue;
            if (TimeSpan.TryParse(trimmed, CultureInfo.InvariantCulture, out timeSpanValue))
            {
                return timeSpanValue;
            }

            object contextValue;
            if (TryResolveContextValue(context, trimmed, out contextValue))
            {
                return contextValue;
            }

            return trimmed;
        }

        private static bool TryResolveContextValue(IExecutionContext context, string name, out object value)
        {
            value = null;
            if (context == null)
            {
                return false;
            }

            if (string.Equals(name, "UserId", StringComparison.OrdinalIgnoreCase))
            {
                value = context.UserId;
                return true;
            }

            if (string.Equals(name, "CompanyId", StringComparison.OrdinalIgnoreCase))
            {
                value = context.CompanyId;
                return true;
            }

            if (string.Equals(name, "FiscalYearId", StringComparison.OrdinalIgnoreCase))
            {
                value = context.FiscalYearId;
                return true;
            }

            if (string.Equals(name, "VoucherId", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "Voucher.Id", StringComparison.OrdinalIgnoreCase))
            {
                value = context.Voucher != null ? (object)context.Voucher.Id : null;
                return true;
            }

            if (string.Equals(name, "VoucherKind", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "Voucher.Kind", StringComparison.OrdinalIgnoreCase))
            {
                value = context.Voucher != null ? (object)context.Voucher.Kind : null;
                return true;
            }

            if (string.Equals(name, "StepId", StringComparison.OrdinalIgnoreCase))
            {
                value = context.StepId;
                return true;
            }

            if (context.Items != null && TryGetDictionaryValue(context.Items, name, out value))
            {
                return true;
            }

            if (context.WorkflowData != null)
            {
                foreach (IWorkflowMetadata metadata in context.WorkflowData)
                {
                    if (string.Equals(metadata.Key, name, StringComparison.OrdinalIgnoreCase))
                    {
                        value = metadata.Value;
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryGetDictionaryValue(IDictionary<string, object> dictionary, string name, out object value)
        {
            if (dictionary.TryGetValue(name, out value))
            {
                return true;
            }

            foreach (KeyValuePair<string, object> item in dictionary)
            {
                if (string.Equals(item.Key, name, StringComparison.OrdinalIgnoreCase))
                {
                    value = item.Value;
                    return true;
                }
            }

            value = null;
            return false;
        }

        private static bool AreEqual(object left, object right)
        {
            if (left == null || right == null)
            {
                return left == right;
            }

            if (IsNumber(left) && IsNumber(right))
            {
                decimal leftNumber = Convert.ToDecimal(left, CultureInfo.InvariantCulture);
                decimal rightNumber = Convert.ToDecimal(right, CultureInfo.InvariantCulture);
                return leftNumber == rightNumber;
            }

            DateTime leftDateTime;
            DateTime rightDateTime;
            if (TryConvertToDateTime(left, out leftDateTime) && TryConvertToDateTime(right, out rightDateTime))
            {
                return leftDateTime == rightDateTime;
            }

            return string.Equals(Convert.ToString(left, CultureInfo.InvariantCulture), Convert.ToString(right, CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase);
        }

        private static int CompareValues(object left, object right)
        {
            if (left == null || right == null)
            {
                throw new InvalidOperationException("Cannot compare null values.");
            }

            if (IsNumber(left) && IsNumber(right))
            {
                decimal leftNumber = Convert.ToDecimal(left, CultureInfo.InvariantCulture);
                decimal rightNumber = Convert.ToDecimal(right, CultureInfo.InvariantCulture);
                return leftNumber.CompareTo(rightNumber);
            }

            DateTime leftDateTime;
            DateTime rightDateTime;
            if (TryConvertToDateTime(left, out leftDateTime) && TryConvertToDateTime(right, out rightDateTime))
            {
                return leftDateTime.CompareTo(rightDateTime);
            }

            IComparable comparableLeft = left as IComparable;
            IComparable comparableRight = right as IComparable;
            if (comparableLeft != null && comparableRight != null)
            {
                return comparableLeft.CompareTo(comparableRight);
            }

            throw new InvalidOperationException("The supplied values cannot be compared.");
        }

        private static bool ConvertToBoolean(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is bool)
            {
                return (bool)value;
            }

            if (IsNumber(value))
            {
                decimal numericValue = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                return numericValue != 0m;
            }

            string text = value as string;
            if (text != null)
            {
                bool parsedBool;
                if (bool.TryParse(text, out parsedBool))
                {
                    return parsedBool;
                }

                decimal parsedDecimal;
                if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out parsedDecimal))
                {
                    return parsedDecimal != 0m;
                }

                return text.Length > 0;
            }

            return true;
        }

        private static bool IsNumber(object value)
        {
            return value is byte || value is sbyte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal;
        }

        private static bool TryConvertToDateTime(object value, out DateTime result)
        {
            if (value is DateTime)
            {
                result = ((DateTime)value).ToUniversalTime();
                return true;
            }

            string text = value as string;
            if (text != null && DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out result))
            {
                return true;
            }

            result = DateTime.MinValue;
            return false;
        }
    }
}
