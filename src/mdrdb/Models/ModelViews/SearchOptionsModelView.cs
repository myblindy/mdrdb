using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mdrdb.Models.ModelViews
{
    public class SearchOptionsModelView
    {
        public IEnumerable<DrProj> Projects;
        public IEnumerable<DrStatus> Statuses;
    }
}
