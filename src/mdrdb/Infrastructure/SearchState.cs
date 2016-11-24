﻿using mdrdb.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mdrdb.Infrastructure
{
    public class SearchState
    {
        private SearchState() { }

        public HtmlString Description { get; private set; }

        public IEnumerable<int> ProjectIDs { get; private set; }
        public IQueryable<DrProj> Projects { get; private set; }

        public IEnumerable<int> StatusIDs { get; private set; }
        public IQueryable<DrStatus> Statuses { get; private set; }

        /// <summary>
        /// Initializes a new SearchState object.
        /// </summary>
        /// <param name="s">the JSON string representing the state</param>
        /// <param name="projects">a (queriable) collection of DrProj items</param>
        /// <param name="FillDescription">whether or not to fill the description field (requires additional DB lookups)</param>
        /// <param name="ExtraProjects">what (if any) projects to add besides the JSON string</param>
        /// <returns>An async task representing the SearchState object</returns>
        public static async Task<SearchState> New(string s, IQueryable<DrProj> projects, IQueryable<DrStatus> statuses,
            bool FillDescription = true,
            Func<IEnumerable<int>, IEnumerable<int>> ProjectsFilter = null, Func<IEnumerable<int>, IEnumerable<int>> StatusesFilter = null)
        {
            var ss = new SearchState();

            dynamic searchjson = string.IsNullOrWhiteSpace(s) ? null : JsonConvert.DeserializeObject(s);
            var description = "DRs";

            ss.ProjectIDs = (searchjson?.ProjectIDs as IEnumerable<JToken>)?.Select(w => w.Value<int>())?.ToList();
            if (ProjectsFilter != null)
                ss.ProjectIDs = ss.ProjectIDs == null ? ProjectsFilter(new int[0]) : ProjectsFilter(ss.ProjectIDs);
            if (ss.ProjectIDs?.Any() == true)
            {
                ss.Projects = projects.Where(p => ss.ProjectIDs.Contains(p.Id));
                if (FillDescription)
                    description += " that belong to " + CommaAndJoin(
                        (await ss.Projects.Select(p => new { Name = p.Nn.Trim(), Description = p.AcName.Trim() }).ToListAsync())
                        .Select(p => $"<abbr title='{HtmlEncoder.Default.HtmlEncode(p.Description)}'>{HtmlEncoder.Default.HtmlEncode(p.Name)}</abbr>")
                        .ToList()) + " ";
            }

            ss.StatusIDs = (searchjson?.StatusIDs as IEnumerable<JToken>)?.Select(w => w.Value<int>())?.ToList();
            if (StatusesFilter != null)
                ss.StatusIDs = ss.StatusIDs == null ? StatusesFilter(new int[0]) : StatusesFilter(ss.StatusIDs);
            if (ss.StatusIDs?.Any() == true)
            {
                ss.Statuses = statuses.Where(w => ss.StatusIDs.Contains(w.Id));
                if (FillDescription)
                    description = CommaAndJoin(await ss.Statuses.Select(w => w.Status).ToListAsync()) + " " + description;
            }

            if (FillDescription && description.StartsWith("DRs"))
                description = "all " + description;

            if (FillDescription) ss.Description = new HtmlString(description);

            return ss;
        }

        private static string CommaAndJoin(ICollection<string> strings)
        {
            if (strings.Count == 1)
                return strings.First();

            var sb = new StringBuilder(strings.First());

            int idx = 0;
            foreach (var s in strings)
            {
                if (idx > 0 && idx < strings.Count - 1)
                    sb.Append(", ").Append(s);
                else if (idx == strings.Count - 1)
                    sb.Append(" and ").Append(s);

                ++idx;
            }

            return sb.ToString();
        }

        public override string ToString() =>
            JsonConvert.SerializeObject(new { ProjectIDs, StatusIDs });
    }
}