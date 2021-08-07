using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    public class AcceraReportItem
    {
        public string CodePartner { get; set; }

        public string PartnerName { get; set; }


        public string CNPJPartner { get; set; }

        public string Criticality {
            get;
            set;
        }

        public string Action {
            get;
            set;
        }

        public string Responsible {
            get;
            set;
        }

        public DateTime LastInteraction {
            get;
            set;
        }

        public DateTime LastStockPosition {
            get;
            set;
        }

        public System.Collections.Generic.Dictionary<int, String> DaysPosition
        {
            get;
            set;
        }
    }
}
