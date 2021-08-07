using System;
using System.Collections.Generic;
using System.Text;
using Bayer.Pegasus.Data;

namespace Bayer.Pegasus.Business
{
    public class CFOPBO : BaseBO
    {
        public List<Entities.CFOP> GetCFOPs(string search)
        {
            using (var cfopDAL = new CFOPDAL())
            {
                return cfopDAL.GetCFOPs(search);

            }

        }
    }
}
