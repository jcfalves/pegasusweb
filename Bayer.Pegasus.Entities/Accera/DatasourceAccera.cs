using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Entities.Accera
{
    public class DatasourceAccera
    {
        public string datasource { get; set; }
        public List<DimensionsAccera> dimensions { get; set; }
        public List<FiltersAccera> filters { get; set; }      
    }
}
