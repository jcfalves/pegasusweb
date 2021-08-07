using System;

namespace Bayer.Pegasus.Utils
{
    public class Configuration : Singleton<Configuration>
    {
        public string LoginURL
        {
            get;
            set;
        }

        public bool WriteOnLog {
            get {
                return true;
            }
        }

        public bool UseServicesOnTokenField {
            get;
            set;
        }

        public string LogoutURL
        {
            get;
            set;
        }


        public string AppId {
            get;
            set;
        }

        public string URLPrefix
        {
            get;
            set;

        }

        
        public string PegasusAPIPartner {
            get {
                return URLPrefix + "/api/Partner";
            }
        }

        public string PegasusAPIUnit
        {
            get {
                return URLPrefix + "/api/Unit";
            }
        }

        public string PegasusAPIClient
        {
            get
            {
                return URLPrefix + "/api/Client";
            }
        }

        public string PegasusAPICity
        {
            get
            {
                return URLPrefix + "/api/City";
            }
        }

        public string PegasusAPIBrand
        {
            get
            {
                return URLPrefix + "/api/Brand";
            }
        }

        public string PegasusAPIProduct
        {
            get
            {
                return URLPrefix + "/api/Product";
            }
        }


        public string AppCulture {
            get;
            set;
        }

        public string AppDomainURL
        {
            get;
            set;
        }

        public string PortalOneDomainURL
        {
            get;
            set;
        }

        public bool UseTokenLifeTime
        {
            get;
            set;
        }

        public int TokenLifeTime
        {
            get;
            set;
        }

        public string UrlApiOauthToken
        {
            get;
            set;
        }

        public string UrlApiAuthServicesLogin
        {
            get;
            set;
        }

        public string UrlApiAuthServicesLoginSSO
        {
            get;
            set;
        }

        public string UrlApiAuthServicesLogout
        {
            get;
            set;
        }

        public string ServiceProductURL
        {
            get;
            set;
        }

        public string ServicePartnerURL
        {
            get;
            set;
        }

        public string ServiceIAMURL
        {
            get;
            set;
        }

        

        public string ServiceEmailURL {
            get;
            set;
        }

        public string ServiceTokenURL {
            get;
            set;
        }

        public string ServiceSalesStructureURL
        {
            get;
            set;
        }

        public string DataDirectory {
            get {
                string dataDir = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
                return dataDir;
            }
        }

        public Bayer.Pegasus.Entities.TokenSAML TokenSAML {
            get;
            set;
        }

        public string ConnectionString
        {
            get;
            set;
        }

        public string ConnectionString_ODS
        {
            get;
            set;
        }

        public string ConnectionString_DW
        {
            get;
            set;
        }

        public string MockConnectionString
        {
            get;
            set;
        }

        public string AppIdLogin
        {
            get;
            set;
        }

        public string ClientId
        {
            get;
            set;
        }

        public string ClientSecret
        {
            get;
            set;
        }

        public string ServiceProductLegaciesURL
        {
            get;
            set;
        }

        public string ServiceProductLegaciesBUURL
        {
            get;
            set;
        }

        public string ErrorMailTo
        {
            get;
            set;
        }

        public bool UseServicesOnTokenFieldProducts
        {
            get;
            set;
        }

        public bool UseServicesOnTokenFieldPartner
        {
            get;
            set;
        }

        public bool UseServicesOnTokenFieldBrand
        {
            get;
            set;
        }

    }
}
