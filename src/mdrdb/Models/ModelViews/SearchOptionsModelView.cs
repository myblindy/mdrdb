using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mdrdb.Models.ModelViews
{
    public class SearchOptionsModelView
    {
        public List<int> SelectedProjectIDs;
        public List<SelectListItem> AllProjects;
        public List<int> SelectedStatusIDs;
        public List<SelectListItem> AllStatuses;
        public DrdbContext Context;
    }
}
