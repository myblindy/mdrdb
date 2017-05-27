using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mdrdb.Models.ModelViews
{
    public class SearchOptionsModelView
    {
        public IEnumerable<int> SelectedProjectIDs;
        public IEnumerable<SelectListItem> AllProjects;
        public IEnumerable<int> SelectedStatusIDs;
        public IEnumerable<SelectListItem> AllStatuses;
        public IEnumerable<int> SelectedAssignedEmployeeIDs;
        public IEnumerable<SelectListItem> AllAssignedEmployees;
        public IEnumerable<int> SelectedPriorityIDs;
        public IEnumerable<SelectListItem> AllPriorities;

        public DateTime? DueDateFrom, DueDateTo, CreatedFrom, CreatedTo, UpdatedFrom, UpdatedTo;

        public DrdbContext Context;
    }
}
