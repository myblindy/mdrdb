using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mdrdb.Models;
using Microsoft.EntityFrameworkCore;
using mdrdb.Models.ModelViews;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using mdrdb.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mdrdb.Controllers
{
    public static class DrsControllerExtensions
    {
        public static IQueryable<DrDr> BaseQuery(this IQueryable<DrDr> collection) =>
            collection
                .Include(w => w.DrHistory)
                    .ThenInclude(w => w.StatusNavigation)
                .Include(w => w.DrHistory)
                    .ThenInclude(w => w.AssignedNavigation)
                .Include(w => w.ProjectNavigation)
                .Include(w => w.AtaNavigation);

        public static IQueryable<DrDr> FilterBySearchParameters(this IQueryable<DrDr> collection, SearchState searchstate)
        {
            // dr's project
            if (searchstate.ProjectIDs?.Any() == true)
                collection = collection.Where(dr => searchstate.ProjectIDs.Contains(dr.Project));

            // dr's status (last history's status)
            if (searchstate.StatusIDs?.Any() == true)
                collection = collection.Where(dr => searchstate.StatusIDs.Contains(dr.DrHistory.OrderByDescending(w => w.HistNum).FirstOrDefault().Status));

            // drs assigned to (last history's assigned to)
            if (searchstate.AssignedEmployeeIDs?.Any() == true)
                collection = collection.Where(dr => searchstate.AssignedEmployeeIDs.Contains(dr.DrHistory.OrderByDescending(w => w.HistNum).FirstOrDefault().Assigned.Value));

            // dr's priority
            if (searchstate.PriorityIDs?.Any() == true)
            {
                var stringpriorities = searchstate.PriorityIDs.Select(w => w.ToString()).ToList();
                collection = collection.Where(dr => stringpriorities.Contains(dr.Priority));
            }

            // dr's various dates
            if (searchstate.CreatedFrom.HasValue)
                collection = collection.Where(dr => dr.DrHistory.OrderBy(w => w.HistNum).FirstOrDefault().Date >= searchstate.CreatedFrom);
            if (searchstate.CreatedTo.HasValue)
                collection = collection.Where(dr => dr.DrHistory.OrderBy(w => w.HistNum).FirstOrDefault().Date <= searchstate.CreatedTo);
            if (searchstate.UpdatedFrom.HasValue)
                collection = collection.Where(dr => dr.DrHistory.OrderByDescending(w => w.HistNum).FirstOrDefault().Date >= searchstate.UpdatedFrom);
            if (searchstate.UpdatedTo.HasValue)
                collection = collection.Where(dr => dr.DrHistory.OrderByDescending(w => w.HistNum).FirstOrDefault().Date <= searchstate.UpdatedTo);
            if (searchstate.DueDateFrom.HasValue)
                collection = collection.Where(dr => dr.DueDate >= searchstate.DueDateFrom);
            if (searchstate.DueDateTo.HasValue)
                collection = collection.Where(dr => dr.DueDate >= searchstate.DueDateTo);

            return collection;
        }
    }

    public class DrsController : Controller
    {
        DrdbContext DrdbContext { get; }

        public DrsController(DrdbContext drdbcontext) => DrdbContext = drdbcontext;

        [HttpGet]
        public async Task<IActionResult> List(int? id /*page number*/)
        {
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], DrdbContext.DrProj, DrdbContext.DrStatus, DrdbContext.DrEmployee);
            var collection = DrdbContext.DrDr.BaseQuery().FilterBySearchParameters(searchstate);
            var pager = new Pager(await collection.CountAsync(), id);

            return View(new HomeModelView
            {
                Pager = pager,
                DrCollection = await collection
                    //.OrderBy(w => w.ProjectNavigation.Nn).ThenBy(w => w.ProjDr)
                    .Skip(pager.PageSize * (pager.CurrentPage - 1)).Take(pager.PageSize)
                    .ToListAsync(),
                SearchState = searchstate
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchOptions()
        {
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], DrdbContext.DrProj, DrdbContext.DrStatus, DrdbContext.DrEmployee);

            var obj = new SearchOptionsModelView
            {
                SelectedProjectIDs = searchstate.ProjectIDs ?? new int[0],
                AllProjects = await DrdbContext.DrProj.OrderBy(w => w.Nn).ThenBy(w => w.AcName).Select(w =>
                    new SelectListItem { Text = w.Nn.TrimEnd() + " - " + w.AcName.TrimEnd(), Value = w.Id.ToString() }).ToListAsync(),
                SelectedStatusIDs = searchstate.StatusIDs ?? new int[0],
                AllStatuses = await DrdbContext.DrStatus.OrderBy(w => w.Sortid).Select(w =>
                    new SelectListItem { Text = w.Status.TrimEnd(), Value = w.Id.ToString() }).ToListAsync(),
                SelectedAssignedEmployeeIDs = searchstate.AssignedEmployeeIDs ?? new int[0],
                AllAssignedEmployees = await DrdbContext.DrEmployee.OrderBy(w => w.Lname).ThenBy(w => w.Fname).Select(w =>
                    new SelectListItem { Text = w.Lname.TrimEnd() + " " + w.Fname.TrimEnd(), Value = w.Id.ToString() }).ToListAsync(),
                SelectedPriorityIDs = searchstate.PriorityIDs ?? new int[0],
                AllPriorities = SearchState.AllPriorities.Select(p => new SelectListItem { Text = "P" + p, Value = p.ToString() }).ToList(),
                DueDateFrom = searchstate.DueDateFrom,
                DueDateTo = searchstate.DueDateTo,
                CreatedFrom = searchstate.CreatedFrom,
                CreatedTo = searchstate.CreatedTo,
                UpdatedFrom = searchstate.UpdatedFrom,
                UpdatedTo = searchstate.UpdatedTo,
                Context = DrdbContext
            };
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> SearchOptions([FromForm]SearchOptionsModelView searchOptionsModelView)
        {
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], projects: null, statuses: null, employees: null,
                ProjectsOverrideList: searchOptionsModelView.SelectedProjectIDs, StatusesOverrideList: searchOptionsModelView.SelectedStatusIDs,
                AssignedEmployeesOverrideList: searchOptionsModelView.SelectedAssignedEmployeeIDs, PrioritiesOverrideList: searchOptionsModelView.SelectedPriorityIDs,
                DueDateFromOverride: searchOptionsModelView.DueDateFrom ?? DateTime.MinValue, DueDateToOverride: searchOptionsModelView.DueDateTo ?? DateTime.MaxValue,
                CreatedFromOverride: searchOptionsModelView.CreatedFrom ?? DateTime.MinValue, CreatedToOverride: searchOptionsModelView.CreatedTo ?? DateTime.MaxValue,
                UpdatedFromOverride: searchOptionsModelView.UpdatedFrom ?? DateTime.MinValue, UpdatedToOverride: searchOptionsModelView.UpdatedTo ?? DateTime.MaxValue,
                FillDescription: false);
            Response.Cookies.Delete("searchjson");
            Response.Cookies.Append("searchjson", searchstate.ToString());
            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public async Task<IActionResult> ViewDr(int id) => View(new ViewDrModelView
        {
            Dr = await DrdbContext.DrDr.BaseQuery().FirstAsync(w => w.Id == id)
        });
    }
}
