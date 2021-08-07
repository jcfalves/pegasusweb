using System.Collections.Generic;

namespace CacheStrategy.Stores
{
    public interface IBusinessUnitCodeStore
    {
        List<string> ListBUC09();
        List<string> ListBUC15();
        List<string> ListBUC17();
        List<string> ListBUC80();

        //------------------------------------------------------------------------
        //<PBI-1255>
        List<string> ListBUC27();
        //------------------------------------------------------------------------

        List<string> ListaBUC09(List<string> buc);
        List<string> ListaBUC15(List<string> buc);
        List<string> ListaBUC17(List<string> buc);
        List<string> ListaBUC80(List<string> buc);

        //------------------------------------------------------------------------
        //<PBI-1255>
        List<string> ListaBUC27(List<string> buc);
        //------------------------------------------------------------------------

        void RemoveBUC09();
        void RemoveBUC15();
        void RemoveBUC17();
        void RemoveBUC80();
    }
}
