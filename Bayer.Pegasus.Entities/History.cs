using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    public class History
    {

        public long Id {
            get;
            set;
        }

        public string Description {
            get;
            set;
        }

        public DateTime Created {
            get;
            set;
        }
        
        public string Json {
            get;
            set;
        }

    }
}
