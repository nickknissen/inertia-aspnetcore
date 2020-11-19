using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace InertiaAdapter.Extensions
{
    internal static class Helpers
    {
        internal static T NotNull<T>([NotNull] this T? value) where T : class =>
            value ?? throw new ArgumentNullException(nameof(value));

        internal static bool IsInertiaRequest(this HttpContext? hc) =>
            bool.TryParse(hc.NotNull().Request.Headers["X-Inertia"], out _);

        internal static bool IsInertiaRequest(this ActionContext? ac) =>
            bool.TryParse(ac.NotNull().HttpContext.Request.Headers["X-Inertia"], out _);

        internal static string ToCamelCase(this string str, CultureInfo? culture = null) 
        {
            Regex pattern = new Regex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");

            if (culture == null)
            {
                culture = new CultureInfo("en-US", false);
            }

            return new string(
                culture 
                    .TextInfo
                    .ToTitleCase(string.Join(" ", pattern.Matches(str)).ToLower(culture))
                    .Replace(" ", "", StringComparison.OrdinalIgnoreCase)
                    .Select((x, i) => i == 0 ? char.ToLower(x, culture) : x)
                    .ToArray()
            );
        }
    }
}