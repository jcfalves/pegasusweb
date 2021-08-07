using System;
using System.Collections.Generic;
using System.Text;
using Bayer.Pegasus.Data;

namespace Bayer.Pegasus.Business
{
    public class CityBO : BaseBO
    {
        public List<Entities.City> GetCities(string search)
        {
            using (var cityDal = new CityDAL())
            {
                return cityDal.GetCities(search);

            }

        }
    }
}
