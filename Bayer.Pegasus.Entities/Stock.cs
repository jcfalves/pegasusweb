using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    public class Stock
    {

        public Partner Partner {
            get;
            set;
        }


        public Partner Unit
        {
            get;
            set;
        }

        public Product Product
        {
            get;
            set;
        }

        public DateTime StockDate {
            get;
            set;
        }

        public DateTime UpdateDate {
            get;
            set;
        }

        public Dictionary<string, decimal> Quantities {
            get;
            set;
        }
        
    }
}
