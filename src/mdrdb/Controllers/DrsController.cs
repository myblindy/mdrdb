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
        public static IQueryable<DrDr> FilterBySearchParameters(this IQueryable<DrDr> collection, SearchState searchstate)
        {
            if (searchstate.ProjectIDs?.Any() == true)
                collection = collection.Where(dr => searchstate.ProjectIDs.Contains(dr.Project));
            if (searchstate.StatusIDs?.Any() == true)
                collection = collection.Where(dr => searchstate.StatusIDs.Contains(dr.DrHistory.OrderByDescending(w => w.HistNum).FirstOrDefault().Status));

            return collection;
        }
    }

    public class DrsController : Controller
    {
        DrdbContext DrdbContext { get; }

        public DrsController(DrdbContext drdbcontext) => DrdbContext = drdbcontext;

        public async Task<IActionResult> List(int? id)
        {
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], DrdbContext.DrProj, DrdbContext.DrStatus);
            var collection = DrdbContext.DrDr
                .Include(w => w.DrHistory)
                    .ThenInclude(w => w.StatusNavigation)
                .Include(w => w.DrHistory)
                    .ThenInclude(w => w.AssignedNavigation)
                .Include(w => w.ProjectNavigation)
                .Include(w => w.AtaNavigation)
                .FilterBySearchParameters(searchstate);
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
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], DrdbContext.DrProj, DrdbContext.DrStatus);
            return View(new SearchOptionsModelView
            {
                SelectedProjectIDs = (searchstate.Projects == null ? null :
                    await searchstate.Projects?.Select(p => p.Id)?.ToListAsync()) ?? new List<int>(),
                AllProjects = await DrdbContext.DrProj.OrderBy(w => w.Nn).ThenBy(w => w.AcName).Select(w =>
                    new SelectListItem { Text = w.Nn + " - " + w.AcName, Value = w.Id.ToString() }).ToListAsync(),
                SelectedStatusIDs = (searchstate.Statuses == null ? null :
                    await searchstate.Statuses?.Select(s => s.Id)?.ToListAsync()) ?? new List<int>(),
                AllStatuses = await DrdbContext.DrStatus.OrderBy(w => w.Sortid).Select(w =>
                    new SelectListItem { Text = w.Status, Value = w.Id.ToString() }).ToListAsync(),
                Context = DrdbContext
            });
        }

        [HttpPost]
        public async Task<IActionResult> SearchOptions(IEnumerable<int> SelectedProjectIDs, IEnumerable<int> SelectedStatusIDs)
        {
            var searchstate = await SearchState.New(Request.Cookies["searchjson"], projects: null, statuses: null,
               FillDescription: false, ProjectsOverrideList: SelectedProjectIDs, StatusesOverrideList: SelectedStatusIDs);
            Response.Cookies.Delete("searchjson");
            Response.Cookies.Append("searchjson", searchstate.ToString());
            return RedirectToAction(nameof(List));
        }

        public IActionResult ViewDr(int id) => View();
    }
}
