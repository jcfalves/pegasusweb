using System;

namespace Bayer.Pegasus.Web.Models
{
    public class GenericReportModel
    {
        public GenericReportModel() {
            ShowFilter = true;
            ShowHistory = true;
        }

        public Bayer.Pegasus.Entities.SalesStructureAccess SalesStructureAccess {
            get;
            set;
        }

        public Boolean IsDashboard {
            get;
            set;
        }


        public bool ShowHistory
        {
            get;
            set;
        }

        public bool ShowLabelCFOP
        {
            get;
            set;
        }

        public bool ShowFilter
        {
            get;
            set;
        }

        public bool ShowAbaFilterHC
        {
            get;
            set;
        }
        public bool ShowAbaPrice
        {
            get;
            set;
        }

    }
}