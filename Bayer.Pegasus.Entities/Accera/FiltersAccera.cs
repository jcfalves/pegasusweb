using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Entities.Accera
{
    public class FiltersAccera
    {
        private FiltersAccera[] filters;
        public string field_name { get; set; }
        public string field_type { get; set; }
        public string filter_type { get; set; }
        public List<FiltersAcerraValues> values { get; set; }
    }
}
