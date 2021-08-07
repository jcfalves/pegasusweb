using Bayer.Pegasus.Data;
using Bayer.Pegasus.Entities;
using System.Collections.Generic;

namespace Bayer.Pegasus.Business
{
    public class CFOPRegistrationBO : BaseBO
    {
        public List<CFOPRegistration> GetAllCFOPs()
        {
            using (var cfopRegistrationDAL = new CFOPRegistrationDAL())
            {
                return cfopRegistrationDAL.GetAllCFOPs();

            }

        }

        public CFOPRegistration GetCFOPByCode(int cfopCode)
        {
            using (var cfopRegistrationDAL = new CFOPRegistrationDAL())
            {
                return cfopRegistrationDAL.GetCFOPByCode(cfopCode);

            }
        }

        public CFOPRegistration GetCFOP(CFOPRegistration cfopRegistration)
        {
            using (var cfopRegistrationDAL = new CFOPRegistrationDAL())
            {
                return cfopRegistrationDAL.GetCFOP(cfopRegistration);

            }
        }

        public int SaveCFOP(CFOPRegistration cfopRegistration, System.Security.Claims.ClaimsPrincipal user)
        {
            using (var cfopRegistrationDAL = new CFOPRegistrationDAL())
            {
                return cfopRegistrationDAL.SaveCFOP(cfopRegistration, user.Identity.Name);

            }

        }
    }
}