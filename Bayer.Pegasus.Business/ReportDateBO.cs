using Bayer.Pegasus.Data;
using Bayer.Pegasus.Entities;
using System.Collections.Generic;

namespace Bayer.Pegasus.Business
{
    public class ReportDateBO : BaseBO
    {
        public List<ReportDate> GetListYearMoviment()
        {

            using (var reportDateDal = new ReportDateDAL())
            {
                return reportDateDal.GetListYearMoviment();
            }
        }
    }
}
