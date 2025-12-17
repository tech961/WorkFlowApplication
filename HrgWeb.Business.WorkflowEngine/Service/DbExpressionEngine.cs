using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq.Dynamic.Core;
using HrgWeb.Business.WorkflowEngine.Support;

public static class DbExpressionEngine
{
    public static bool Evaluate(object model, string dbExpression, IFieldResolver resolver)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (resolver == null) throw new ArgumentNullException(nameof(resolver));
        if (string.IsNullOrWhiteSpace(dbExpression)) return false;

        var modelType = model.GetType();

        var expr = ConditionHelper.NormalizePersianOperators(dbExpression);
        expr = ConditionHelper.NormalizeLogicalOperatorsFa(expr);

        expr = Regex.Replace(expr, @"(?<![<>=!])=(?![=])", "==");

        expr = Regex.Replace(
            expr,
            @"\[(?<field>[^\]]+)\]\s*(?<op>==|!=|>=|<=|>|<)\s*(?<val>.*?)(?=\s*(\&\&|\|\|)\s*|$)",
            m =>
            {
                var aliasFa = m.Groups["field"].Value.Trim();
                var op = m.Groups["op"].Value;
                var rawVal = (m.Groups["val"].Value ?? "").Trim();

                PropertyInfo prop;
                if (!resolver.TryResolve(modelType, aliasFa, out prop) || prop == null)
                    throw new InvalidOperationException("Unknown field: [" + aliasFa + "] for model " + modelType.Name);

                var typedVal = ConditionHelper.ToDynamicLiteral(rawVal, prop.PropertyType);

                var core = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (core == typeof(string) && (op == ">" || op == "<" || op == ">=" || op == "<="))
                {
                    return string.Format(
                        "string.Compare({0}, {1}, StringComparison.Ordinal) {2} 0",
                        prop.Name,
                        typedVal,
                        op
                    );
                }

                return prop.Name + " " + op + " " + typedVal;
            });

        return EvaluateByRuntimeType(model, expr);
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
}