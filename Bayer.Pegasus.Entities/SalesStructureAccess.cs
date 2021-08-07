using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;
using System.Linq;
using System.IO;

namespace Bayer.Pegasus.Entities
{
    public class SalesStructureAccess
    {
        public static List<string> RolesAdministrator
        {
            get;
            set;
        }

        public static List<string> RolesSalesDistrict
        {
            get;
            set;
        }

        public static List<string> RolesSalesOffice
        {
            get;
            set;
        }

        public static List<string> RolesSalesRepresentative
        {
            get;
            set;
        }

        public static List<string> RolesPartner
        {
            get;
            set;
        }

        public static List<string> RolesConsulting
        {
            get;
            set;
        }

        public static bool IsRoleAdministrator(string roleName)
        {
            return RolesAdministrator.Contains(roleName);
        }

        public static bool IsRoleSalesDistrict(string roleName)
        {
            return RolesSalesDistrict.Contains(roleName);
        }

        public static bool IsRoleSalesOffice(string roleName)
        {
            return RolesSalesOffice.Contains(roleName);
        }

        public static bool IsRoleSalesRepresentative(string roleName)
        {
            return RolesSalesRepresentative.Contains(roleName);
        }

        public static bool IsRolePartner(string roleName)
        {
            return RolesPartner.Contains(roleName);
        }

        public static bool IsRoleConsulting(string roleName)
        {
            return RolesConsulting.Contains(roleName);
        }

        public bool IsAdministrator {
            get;
            set;
        }

        public bool IsSalesDistrict {
            get;
            set;
        }

        public bool HasSalesStructure {
            get {
                if (IsPartner)
                    return true;


                if (IsAdministrator || IsSalesRepresentative || IsSalesOffice || IsSalesDistrict)
                    return true;


                return false;
            }

        }

        public bool CanAccessMultiplePartners {
            get {
                if (IsPartner)
                    return false;


                if (IsAdministrator || IsSalesRepresentative || IsSalesOffice || IsSalesDistrict)
                    return true;

                
                return false;
            }
        }

        /*
        DIRETORIA DE NEGOCIOS 
        */
        public List<string> SalesDistrict
        {
            get;
            set;
        }

        public bool IsSalesOffice
        {
            get;
            set;
        }

        /*
        REGIONAL
        */
        public List<string> SalesOffice {
            get;
            set;
        }

        public bool IsSalesRepresentative
        {
            get;
            set;
        }

        /*
      RTV
      */
        public List<string> SalesRepresentative
        {
            get;
            set;
        }


        public bool IsPartner
        {
            get;
            set;
        }

        /*
          Parceiros 
        */
        public List<string> Partners
        {
            get;
            set;
        }

        public string Level {
            get;
            set;

        }

        public List<string> RestrictionCodes {
            get;
            set;
        }

        public static SalesStructureAccess GetSalesStructureAccessByUser(System.Security.Claims.ClaimsPrincipal user) {

            var roles = ((ClaimsIdentity)user.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role).ToList();

            SalesStructureAccess structure = new SalesStructureAccess();
            structure.RestrictionCodes = new List<string>();
            String writeLog = "";
            writeLog += string.Join(",", RolesAdministrator.ToArray()) + "\r\n\r\n";

            foreach (var role in roles) {
                var roleName = role.Value.Replace("DEV-", "").Replace("QA-", "");
                writeLog += "\r\n" + roleName;
                if (IsRoleAdministrator(roleName))
                {
                    SetupAsFullAccess(structure);

                    break;
                }
                else if (IsRoleSalesDistrict(roleName))
                {
                    SetupAsSalesDistrict(structure, role);
                    break;
                }
                else if (IsRoleSalesOffice(roleName))
                {
                    SetupAsSalesOffice(structure, role);
                    break;
                }
                else if (IsRoleSalesRepresentative(roleName))
                {
                    SetupAsSalesRepresentative(structure, role);

                    break;
                }
                else if (IsRolePartner(roleName))
                {
                    SetupAsPartner(structure, role);

                    break;
                }
                else if (IsRoleConsulting(roleName)) {

                    if (role.Properties.ContainsKey("LevelName"))
                    {
                        var levelName = role.Properties["LevelName"].ToUpper();

                        if (levelName == "CONSULTA")
                        {
                            SetupAsFullAccess(structure);

                            break;
                        }
                        else if (levelName == "PARCEIRO")
                        {
                            SetupAsPartner(structure, role);
                        }
                        else if (levelName == "REPRESENTANTE")
                        {
                            SetupAsSalesRepresentative(structure, role);
                        }
                        else if (levelName == "DIRETOR")
                        {
                            SetupAsSalesDistrict(structure, role);
                        }
                        else if (levelName == "REGIONAL")
                        {
                            SetupAsSalesOffice(structure, role);
                        }
                    }
                    else {
                        SetupAsFullAccess(structure);

                        break;
                    }
                }
            }

            
            return structure;
        }


        private static void SetupAsSalesDistrict(SalesStructureAccess structure, Claim role)
        {
            structure.Level = "DIRETORIA";

            structure.IsSalesDistrict = true;
            structure.SalesDistrict = new List<string>();

            var levelName = role.Properties["LevelName"];

            var restrictionCodes = role.Properties[levelName].Split(';');

            foreach (var restrictionCode in restrictionCodes)
            {
                structure.SalesDistrict.Add(restrictionCode);
                structure.RestrictionCodes.Add(restrictionCode);
            }

        }

        private static void SetupAsSalesOffice(SalesStructureAccess structure, Claim role)
        {
            structure.Level = "REGIONAL";

            structure.IsSalesOffice = true;
            structure.SalesOffice = new List<string>();

            var levelName = role.Properties["LevelName"];

            var restrictionCodes = role.Properties[levelName].Split(';');
            foreach (var restrictionCode in restrictionCodes)
            {
                structure.SalesOffice.Add(restrictionCode);

                structure.RestrictionCodes.Add(restrictionCode);
            }


        }

        private static void SetupAsSalesRepresentative(SalesStructureAccess structure, Claim role)
        {
            structure.Level = "RTV";

            structure.IsSalesRepresentative = true;
            structure.SalesRepresentative = new List<string>();

            var levelName = role.Properties["LevelName"];

            var restrictionCodes = role.Properties[levelName].Split(';');

            foreach (var restrictionCode in restrictionCodes)
            {
                structure.SalesRepresentative.Add(restrictionCode);

                structure.RestrictionCodes.Add(restrictionCode);
            }

        }

        private static void SetupAsFullAccess(SalesStructureAccess structure) {
            structure.IsAdministrator = true;

            structure.Level = "ADMINISTRADOR";
        }

        private static void SetupAsPartner(SalesStructureAccess structure, Claim role) {

            structure.Level = "PARCEIRO";

            structure.IsPartner = true;
            structure.Partners = new List<string>();

            var levelName = role.Properties["LevelName"];

            var restrictionCodes = role.Properties[levelName].Split(';');

            foreach (var restrictionCode in restrictionCodes)
            {
                var restrictionCodePartner = restrictionCode.Replace("-", "");
                
                structure.Partners.Add(restrictionCodePartner);

                structure.RestrictionCodes.Add(restrictionCodePartner);
            }
        }

    }
}
