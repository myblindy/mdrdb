using mdrdb.Models;
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

        public IEnumerable<int> AssignedEmployeeIDs { get; private set; }
        public IQueryable<DrEmployee> AssignedEmployees { get; private set; }

        public IEnumerable<int> PriorityIDs { get; private set; }
        public static readonly int[] AllPriorities = new[] { 1, 2, 3 };

        public DateTime? DueDateFrom, DueDateTo, CreatedFrom, CreatedTo, UpdatedFrom, UpdatedTo;

        private const string DateFormatString = "yyyy-MM-dd";

        /// <summary>
        /// Initializes a new SearchState object.
        /// </summary>
        /// <param name="s">the JSON string representing the state</param>
        /// <param name="projects">a (queriable) collection of DrProj items</param>
        /// <param name="FillDescription">whether or not to fill the description field (requires additional DB lookups)</param>
        /// <param name="ExtraProjects">what (if any) projects to add besides the JSON string</param>
        /// <returns>An async task representing the SearchState object</returns>
        public static async Task<SearchState> New(string s,
            IQueryable<DrProj> projects, IQueryable<DrStatus> statuses, IQueryable<DrEmployee> employees,
            IEnumerable<int> ProjectsOverrideList = null, IEnumerable<int> StatusesOverrideList = null, IEnumerable<int> AssignedEmployeesOverrideList = null,
            IEnumerable<int> PrioritiesOverrideList = null,
            DateTime? DueDateFromOverride = null, DateTime? DueDateToOverride = null, DateTime? CreatedFromOverride = null, DateTime? CreatedToOverride = null,
            DateTime? UpdatedFromOverride = null, DateTime? UpdatedToOverride = null,
            bool FillDescription = true)
        {
            var ss = new SearchState();

            dynamic searchjson = string.IsNullOrWhiteSpace(s) ? null : JsonConvert.DeserializeObject(s);
            var description = "DRs";

            // assigned to
            ss.AssignedEmployeeIDs = AssignedEmployeesOverrideList ?? (searchjson?.AssignedEmployeeIDs as IEnumerable<JToken>)?.Select(w => w.Value<int>())?.ToList();
            if (ss.AssignedEmployeeIDs?.Any() == true && employees != null)
            {
                ss.AssignedEmployees = employees.Where(w => ss.AssignedEmployeeIDs.Contains(w.Id));
                if (FillDescription)
                    description += " assigned to " + CommaOrJoin(
                        (await ss.AssignedEmployees.Select(w => new { Username = w.Username.TrimEnd(), Name = w.Fname.TrimEnd() + " " + w.Lname.TrimEnd() }).ToListAsync())
                        .Select(w => $"<mark><a data-toggle='tooltip' title='{HtmlEncoder.Default.HtmlEncode(w.Name)}'>{HtmlEncoder.Default.HtmlEncode(w.Username)}</a></mark>")
                        .ToList());
            }

            // project
            ss.ProjectIDs = ProjectsOverrideList ?? (searchjson?.ProjectIDs as IEnumerable<JToken>)?.Select(w => w.Value<int>())?.ToList();
            if (ss.ProjectIDs?.Any() == true && projects != null)
            {
                ss.Projects = projects.Where(p => ss.ProjectIDs.Contains(p.Id));
                if (FillDescription)
                    description += " that belong to " + CommaOrJoin(
                        (await ss.Projects.Select(p => new { Name = p.Nn.TrimEnd(), Description = p.AcName.TrimEnd() }).ToListAsync())
                        .Select(p => $"<mark><a data-toggle='tooltip' title='{HtmlEncoder.Default.HtmlEncode(p.Description)}'>{HtmlEncoder.Default.HtmlEncode(p.Name)}</a></mark>")
                        .ToList());
            }

            // priority
            ss.PriorityIDs = PrioritiesOverrideList ?? (searchjson?.PriorityIDs as IEnumerable<JToken>)?.Select(w => w.Value<int>())?.ToList();
            if (ss.PriorityIDs?.Any() == true)
            {
                if (FillDescription)
                    description += " with priority " + CommaOrJoin(ss.PriorityIDs.Select(p => $"<mark>P{p}</mark>").ToList());
            }

            // date range helper
            void UpdateDateRange(Action<DateTime?> fromsetter, DateTime? fromoverride, JToken fromjsontoken, Action<DateTime?> tosetter, DateTime? tooverride, JToken tojsontoken,
                string descriptionverb)
            {
                var from = fromoverride ?? fromjsontoken?.Value<DateTime?>();
                if (from == DateTime.MinValue) from = null;
                fromsetter(from);

                var to = tooverride ?? tojsontoken?.Value<DateTime?>();
                if (to == DateTime.MaxValue) to = null;
                tosetter(to);

                if (FillDescription)
                    if (from.HasValue && !to.HasValue)
                        description += " " + descriptionverb + " starting from <mark>" + from.Value.ToString(DateFormatString) + "</mark>";
                    else if (!from.HasValue && to.HasValue)
                        description += " " + descriptionverb + " before <mark>" + to.Value.ToString(DateFormatString) + "</mark>";
                    else if (from.HasValue && to.HasValue)
                        description += " " + descriptionverb + " between <mark>" + from.Value.ToString(DateFormatString) + "</mark> and <mark>" + to.Value.ToString(DateFormatString) + "</mark>";
            }

            // date ranges
            UpdateDateRange(d => ss.CreatedFrom = d, CreatedFromOverride, searchjson?.CreatedFrom as JToken, d => ss.CreatedTo = d, CreatedToOverride, searchjson?.CreatedTo as JToken, "created");
            UpdateDateRange(d => ss.UpdatedFrom = d, UpdatedFromOverride, searchjson?.UpdatedFrom as JToken, d => ss.UpdatedTo = d, UpdatedToOverride, searchjson?.UpdatedTo as JToken, "updated");
            UpdateDateRange(d => ss.DueDateFrom = d, DueDateFromOverride, searchjson?.DueDateFrom as JToken, d => ss.DueDateTo = d, DueDateToOverride, searchjson?.DueDateTo as JToken, "due");

            // status
            ss.StatusIDs = StatusesOverrideList ?? (searchjson?.StatusIDs as IEnumerable<JToken>)?.Select(w => w.Value<int>())?.ToList();
            if (ss.StatusIDs?.Any() == true && statuses != null)
            {
                ss.Statuses = statuses.Where(w => ss.StatusIDs.Contains(w.Id));
                if (FillDescription)
                    description = CommaOrJoin(
                        (await ss.Statuses.Select(w => new { Status = w.Status.TrimEnd(), Descr = w.Descr.TrimEnd() }).ToListAsync())
                        .Select(w => $"<mark><a data-toggle='tooltip' title='{HtmlEncoder.Default.HtmlEncode(w.Descr)}'>{HtmlEncoder.Default.HtmlEncode(w.Status)}</a></mark>")
                        .ToList()) + " " + description;
            }


            if (FillDescription && description.StartsWith("DRs"))
                description = "all " + description;

            if (FillDescription) ss.Description = new HtmlString(description);

            return ss;
        }

        private static string CommaOrJoin(ICollection<string> strings)
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
                    sb.Append(" or ").Append(s);

                ++idx;
            }

            return sb.ToString();
        }

        public override string ToString() =>
            JsonConvert.SerializeObject(new { ProjectIDs, StatusIDs, AssignedEmployeeIDs, PriorityIDs, DueDateFrom, DueDateTo, CreatedFrom, CreatedTo, UpdatedFrom, UpdatedTo });
    }
}
