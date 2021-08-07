using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    public class Report
    {
        public long Id
        {
            get;
            set;
        }

        public string Identifier
        {
            get;
            set;
        }

        public DateTime Created
        {
            get;
            set;
        }

        public string Json
        {
            get;
            set;
        }

        public byte[] SerializedContent {
            get;
            set;
        }


    }
}
