using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mdrdb.Models.ModelViews
{
    public class SearchOptionsPaneModelView
    {
        public IEnumerable<Tuple<int, string>> Values;

        public string RemoveAction;
        public string ClearFunction, NoFilterName;
        public string AddName, AddFunction;

        public int Width;
        public string PaneName;
    }
}
