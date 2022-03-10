using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinance
{
    public static class HtmlUtility
    {
        public static string IsActive(this IHtmlHelper html,
                          string control,
                          string action,
                          string defaultClass = "")
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];

            // must match both
            var returnActive = control == routeControl &&
                               action == routeAction;

            return defaultClass + " " + (returnActive ? "active" : "");
        }
    }
}
