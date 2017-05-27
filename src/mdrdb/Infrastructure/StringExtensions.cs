using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mdrdb.Infrastructure
{
    public static class StringExtensions
    {
        public static HtmlString ToMultiLineHtml(this string s) =>
            new HtmlString(s.Replace(Environment.NewLine, "<br>"));

        public static string ToDiscrepancyType(this int dr)
        {
            dr %= 10000;

            if (dr < 1000) return "Test Pilot/TRR";
            else if (dr < 2000) return "IPA";
            else if (dr < 3000) return "OSA";
            else if (dr < 4000) return "Qualification";
            else if (dr < 5000) return "EWO";
            else if (dr < 6000) return "HWT/Install";
            else if (dr < 7000) return "Engineering";
            else if (dr < 8000) return "Action Items";

            throw new ArgumentException("Invalid DR type: " + dr);
        }
    }
}
