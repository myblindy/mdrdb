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

namespace mdrdb.Controllers
{
    public static class DrsControllerExtensions
    {
        public static IQueryable<DrDr> FilterBySearchParameters(this IQueryable<DrDr> collection, SearchState searchstate)
        {
            if (searchstate.ProjectIDs?.Any() == true)
                collection = collection.Where(dr => searchstate.ProjectIDs.Contains(dr.Project));
            if (searchstate.StatusIDs?.Any() == true)
                collection = collection.Where(dr => dr.DrHistory.First().Status == 3);

            return collection;
        }
    }

    public class DrsController : Controller
    {
        DrdbContext DrdbContext { get; }

        public DrsController(DrdbContext drdbcontext)
        {
            DrdbContext = drdbcontext;
        }

        public async Task<IActionResult> List(int? id)
        {
            var collection = DrdbContext.DrDr;
            var pager = new Pager(collection.Count(), id);
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], DrdbContext.DrProj, DrdbContext.DrStatus);

            return View(new HomeModelView
            {
                Pager = pager,
                DrCollection = await collection
                    .Include(w => w.DrHistory)
                        .ThenInclude(w => w.StatusNavigation)
                    .Include(w => w.DrHistory)
                        .ThenInclude(w => w.AssignedNavigation)
                    .Include(w => w.ProjectNavigation)
                    .Include(w => w.AtaNavigation)
                    .FilterBySearchParameters(searchstate)
                    .OrderBy(w => w.ProjectNavigation.Nn).ThenBy(w => w.ProjDr)
                    .Skip(pager.PageSize * (pager.CurrentPage - 1)).Take(pager.PageSize)
                    .ToListAsync(),
                SearchState = searchstate
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchOptions()
        {
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], DrdbContext.DrProj, DrdbContext.DrStatus);
            return View(new SearchOptionsModelView
            {
                Projects = searchstate.Projects == null ? null : await searchstate.Projects?.ToListAsync(),
                Statuses = searchstate.Statuses == null ? null : await searchstate.Statuses?.ToListAsync()
            });
        }

        #region Filter actions
        public async Task<JsonResult> GetProjectFilters()
        {
            return Json(await DrdbContext.DrProj
                .OrderBy(w => w.Nn).ThenBy(w => w.AcName)
                .Select(w => new { ID = w.Id, Name = w.Nn.Trim() + " - " + w.AcName.Trim() })
                .ToListAsync());
        }

        public async Task<JsonResult> GetStatusFilters()
        {
            return Json(await DrdbContext.DrStatus
                .OrderBy(w => w.Sortid)
                .Select(w => new { ID = w.Id, Name = w.Status.Trim() })
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddProjectFilter(int id)
        {
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], projects: DrdbContext.DrProj, statuses: DrdbContext.DrStatus,
                FillDescription: false,
                ProjectsFilter: w => w.Concat(Enumerable.Repeat(id, 1)));
            Response.Cookies.Delete("searchjson");
            Response.Cookies.Append("searchjson", searchstate.ToString());
            return RedirectToAction(nameof(SearchOptions));
        }

        [HttpPost]
        public async Task<IActionResult> AddStatusFilter(int id)
        {
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], projects: DrdbContext.DrProj, statuses: DrdbContext.DrStatus,
                FillDescription: false,
                StatusesFilter: w => w.Concat(Enumerable.Repeat(id, 1)));
            Response.Cookies.Delete("searchjson");
            Response.Cookies.Append("searchjson", searchstate.ToString());
            return RedirectToAction(nameof(SearchOptions));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveProjectFilter(int id)
        {
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], projects: DrdbContext.DrProj, statuses: DrdbContext.DrStatus,
                FillDescription: false,
                ProjectsFilter: w => w.Where(pid => pid != id));
            Response.Cookies.Delete("searchjson");
            Response.Cookies.Append("searchjson", searchstate.ToString());
            return RedirectToAction(nameof(SearchOptions));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveStatusFilter(int id)
        {
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], projects: DrdbContext.DrProj, statuses: DrdbContext.DrStatus,
                FillDescription: false,
                StatusesFilter: w => w.Where(sid => sid != id));
            Response.Cookies.Delete("searchjson");
            Response.Cookies.Append("searchjson", searchstate.ToString());
            return RedirectToAction(nameof(SearchOptions));
        }

        public async Task<IActionResult> ClearProjectFilters()
        {
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], projects: null, statuses: DrdbContext.DrStatus,
                FillDescription: false);
            Response.Cookies.Delete("searchjson");
            Response.Cookies.Append("searchjson", searchstate.ToString());
            return RedirectToAction(nameof(SearchOptions));
        }

        public async Task<IActionResult> ClearStatusFilters()
        {
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], projects: DrdbContext.DrProj, statuses: null,
                FillDescription: false);
            Response.Cookies.Delete("searchjson");
            Response.Cookies.Append("searchjson", searchstate.ToString());
            return RedirectToAction(nameof(SearchOptions));
        }
        #endregion

        public IActionResult ViewDr(int id) =>
            View();
    }
}
