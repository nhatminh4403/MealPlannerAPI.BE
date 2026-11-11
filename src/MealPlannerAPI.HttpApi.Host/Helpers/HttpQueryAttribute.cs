using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MealPlannerAPI.Helpers
{
    public class HttpQueryAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = new string[1] { "QUERY" };

        public HttpQueryAttribute() : base(_supportedMethods)
        {
        }

        public HttpQueryAttribute([StringSyntax("Route")] string template) : base(_supportedMethods, template)
        {
            ArgumentNullException.ThrowIfNull(template, nameof(template));
        }
    }
}
