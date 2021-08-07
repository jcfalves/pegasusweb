using Bayer.Pegasus.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Bayer.Pegasus.ApiClient.Helpers
{
    public class IAMHelper
    {
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(IAMHelper));

        public bool WriteOnLog
        {
            get;
            set;
        }

        public List<string> LogInformation
        {
            get;
            set;
        }

        public List<Bayer.Pegasus.Entities.Auth.Role> GetRoles(string cwid, string ip)
        {

            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            bool useLoginsso = false;

            var authServicesApi = new Pegasus.ApiClient.AuthServicesApi(Bayer.Pegasus.Utils.Configuration.Instance.ServiceIAMURL);
            authServicesApi.ApiClient.WriteOnLog = WriteOnLog;

            Bayer.Pegasus.Entities.Auth.LoginSsoModel model = new Entities.Auth.LoginSsoModel();

            model.Login = cwid;

            model.AppId = Bayer.Pegasus.Utils.Configuration.Instance.AppId;

            model.CultureName = Bayer.Pegasus.Utils.Configuration.Instance.AppCulture;

            model.Ip = ip;

            List<Bayer.Pegasus.Entities.Auth.Role> roles = new List<Entities.Auth.Role>();

            //----------------------------------------
            _log4net.Debug($"Buscando as Roles");
            _log4net.Debug($"useLoginsso: {useLoginsso}");
            //----------------------------------------

            if (useLoginsso)
            {
                var result = authServicesApi.Loginsso(model);
                roles = result.Return.Roles;
                LogInformation = authServicesApi.ApiClient.LogInformation;

                //----------------------------------------
                foreach (var role in roles)
                {
                    //----------------------------------------
                    _log4net.Debug($"useLoginsso: {useLoginsso}");
                    _log4net.Debug($"roleName: {role.Name}");
                    _log4net.Debug($"levelName: {role.Level.Name}");
                    //----------------------------------------
                }
                //----------------------------------------

            }
            else
            {
                var result = authServicesApi.GetUsersBySystem(model.AppId, model.Ip, model.Login, null, null);

                //----------------------------------------
                _log4net.Debug($"result: {(result != null ? result.Result.Description : "nulo")}");
                _log4net.Debug($"result.Return: {(result.Return != null ? result.Return.ToString() : "nulo")}");
                //----------------------------------------

                if (result != null && result.Return != null)
                {
                    if (result.Return.Count > 0)
                    {

                        foreach (var item in result.Return)
                        {

                            foreach (var role in item.Roles)
                            {
                                roles.Add(role);
                                //----------------------------------------
                                _log4net.Debug($"useLoginsso: {useLoginsso}");
                                _log4net.Debug($"roleName: {role.Name}");
                                _log4net.Debug($"levelName: {role.Level.Name}");
                                //----------------------------------------
                            }
                        }
                    }
                }

                LogInformation = authServicesApi.ApiClient.LogInformation;
            }

            var salesServicesApi = new Pegasus.ApiClient.SalesStructureAPI(Bayer.Pegasus.Utils.Configuration.Instance.ServiceSalesStructureURL);


            //----------------------------------------
            _log4net.Debug("Buscando os RestrictionCodes");
            //----------------------------------------

            foreach (var role in roles)
            {
                try
                {
                    List<string> restrictionCodes = GetAdditionalRestrictionCodes(salesServicesApi, role, model.Login);

                    //----------------------------------------
                    _log4net.Debug("Verifica se tem RestrictionCodes");
                    _log4net.Debug($"restrictionCodes.Count: {restrictionCodes.Count}");
                    //----------------------------------------

                    if (restrictionCodes.Count > 0)
                    {
                        if (role.Level.RestrictionCodes == null)
                        {
                            role.Level.RestrictionCodes = new List<string>();
                        }
                    }


                    foreach (var restrictionCode in restrictionCodes)
                    {
                        if (!role.Level.RestrictionCodes.Contains(restrictionCode))
                        {
                            role.Level.RestrictionCodes.Add(restrictionCode);
                            //----------------------------------------
                            _log4net.Debug($"restrictionCode: {restrictionCode}");
                            //----------------------------------------
                        }
                    }

                    foreach (var log in salesServicesApi.ApiClient.LogInformation)
                    {
                        if (LogInformation != null)
                        {
                            LogInformation.Add(log);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log4net.Error($"MethodName: {MethodBase.GetCurrentMethod().Name} - 3.2 (GetRoles) - Exception: {e.Message} - InnerException: {e.InnerException} - StackTrace: {e.StackTrace}");

                    if (LogInformation != null)
                    {
                        LogInformation.Add(e.ToString());
                    }

                }
            }

            return roles;
        }

        public List<string> GetAdditionalRestrictionCodes(Pegasus.ApiClient.SalesStructureAPI salesServicesApi, Bayer.Pegasus.Entities.Auth.Role role, string login)
        {

            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            salesServicesApi.ApiClient.WriteOnLog = WriteOnLog;

            List<string> restrictionCodes = new List<string>();

            if (Bayer.Pegasus.Entities.SalesStructureAccess.IsRoleSalesDistrict(role.Name))
            {

                var dataSalesDistrict = salesServicesApi.ListSalesDistrict(null, null, null, login, null, null, null);

                if (dataSalesDistrict == null)
                {
                    dataSalesDistrict = new Entities.SalesStructure.SalesDistrictSingleArray();
                }

                foreach (var item in dataSalesDistrict)
                {
                    if (!restrictionCodes.Contains(item.Code))
                        restrictionCodes.Add(item.Code);
                }
            }
            else if (Bayer.Pegasus.Entities.SalesStructureAccess.IsRoleSalesOffice(role.Name))
            {
                var dataSalesOffice = salesServicesApi.ListSalesOffice(null, null, null, null, login, null, null);


                if (dataSalesOffice == null)
                {
                    dataSalesOffice = new Entities.SalesStructure.SalesOfficeSingleArray();
                }

                foreach (var item in dataSalesOffice)
                {
                    if (!restrictionCodes.Contains(item.Code))
                        restrictionCodes.Add(item.Code);
                }

            }
            else if (Bayer.Pegasus.Entities.SalesStructureAccess.IsRoleSalesRepresentative(role.Name))
            {
                //var dataSalesRepresentative = salesServicesApi.ListSalesRepresentative(null, null, null, null, null, null, login, null, null);

                //if (dataSalesRepresentative == null)
                //{
                //    dataSalesRepresentative = new Entities.SalesStructure.SalesRepresentativeSingleArray();
                //}

                //foreach (var item in dataSalesRepresentative)
                //{
                //    if(!restrictionCodes.Contains(item.Code))
                //        restrictionCodes.Add(item.Code);
                //}

                var dataSalesRepresentative = new SalesRepresentativeDAL().GetSalesRepresentativesCode(login);

                dataSalesRepresentative.ForEach(c =>
                {
                    if (!restrictionCodes.Contains(c.code))
                        restrictionCodes.Add(c.code);
                });
            }

            return restrictionCodes;
        }

    }
}
