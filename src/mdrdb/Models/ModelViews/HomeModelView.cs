using mdrdb.Infrastructure;
using mdrdb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mdrdb.Models.ModelViews
{
    public class HomeModelView
    {
        public ICollection<DrDr> DrCollection;
        public Pager Pager;

        // search information
        public SearchState SearchState;
    }
}
