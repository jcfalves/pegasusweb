using Bayer.Pegasus.Data;
using Bayer.Pegasus.Entities;
using System.Collections.Generic;

namespace Bayer.Pegasus.Business
{
    public class RetroativoBO : BaseBO
    {
        public List<CFOPRegistration> GetAllCFOPs()
        {
            using (var retroativoDAL = new RetroativoDAL())
            {
                return retroativoDAL.GetAllCFOPs();
            }
        }

        public List<Retroativo> GetListArquivosRetroativos(string status)
        {
            using (var retroativoDAL = new RetroativoDAL())
            {
                return retroativoDAL.GetListArquivosRetroativos(status);
            }
        }

        public void SaveArquivoRetroativo(Retroativo retroativo, System.Security.Claims.ClaimsPrincipal user)
        {
            using (var retroativoDAL = new RetroativoDAL())
            {
                retroativoDAL.SaveArquivoRetroativo(retroativo, user.Identity.Name);

            }

        }

        public CFOPRegistration GetCFOPByCode(int cfopCode)
        {
            using (var retroativoDAL = new RetroativoDAL())
            {
                return retroativoDAL.GetCFOPByCode(cfopCode);

            }
        }

        public CFOPRegistration GetCFOP(CFOPRegistration retroativo)
        {
            using (var retroativoDAL = new RetroativoDAL())
            {
                return retroativoDAL.GetCFOP(retroativo);

            }
        }

        public int SaveCFOP(CFOPRegistration cfopRegistration, System.Security.Claims.ClaimsPrincipal user)
        {
            using (var retroativoDAL = new RetroativoDAL())
            {
                return retroativoDAL.SaveArquivoRetroativo(cfopRegistration, user.Identity.Name);

            }

        }
    }
}