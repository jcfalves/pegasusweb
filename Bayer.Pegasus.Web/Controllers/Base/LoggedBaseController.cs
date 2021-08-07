using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bayer.Pegasus.Web.Controllers.Base
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class LoggedBaseController : Controller
    {
        protected ILogger _logger;
        protected IHttpContextAccessor _accessor;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(LoggedBaseController));

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public Models.GenericReportModel GetGenericReportModel()
        {
            _log4net.Debug($"LoggedBaseController.GetGenericReportModel (Início)");

            var model = new Models.GenericReportModel();

            model.SalesStructureAccess = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(User);

            _log4net.Debug($"LoggedBaseController.GetGenericReportModel (Fim)");

            return model;
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public System.Collections.Generic.List<string> GetListFilterValues(JArray filter)
        {

            return Bayer.Pegasus.Utils.JsonUtils.GetListFilterValues(filter);

        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        protected IActionResult AuthenticationError()
        {
            //----------------------------------------
            //_log4net.Debug($"MethodName: {MethodBase.GetCurrentMethod().Name} - 1");
            _log4net.Debug($"LoggedBaseController.AuthenticationError() (Início)");
            //----------------------------------------

            if (!String.IsNullOrEmpty(Utils.Configuration.Instance.LoginURL))
            {
                _log4net.Debug($"LoggedBaseController.AuthenticationError() - Redirect para a LoginURL: {Utils.Configuration.Instance.LoginURL}");
                _log4net.Debug($"LoggedBaseController.AuthenticationError() (Fim)");
                return Redirect(Utils.Configuration.Instance.LoginURL);
            }
            else
            {
                _log4net.Debug($"LoggedBaseController.AuthenticationError() - LoginURL vazia. Chama a view AuthenticationError");
                _log4net.Debug($"LoggedBaseController.AuthenticationError() (Fim)");
                return View("AuthenticationError");
            }
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task SignAsUserAsync()
        {
            //----------------------------------------
            //_log4net.Debug($"MethodName: {MethodBase.GetCurrentMethod().Name} - 2");
            _log4net.Debug($"LoggedBaseController.SignAsUserAsync() (Início)");
            //----------------------------------------

            if (!String.IsNullOrEmpty(Request.Headers["IV-USER"]) || !String.IsNullOrEmpty(Request.Headers["PD-UID"]))
            {

                var user = "";

                if (!String.IsNullOrEmpty(Request.Headers["IV-USER"]))
                {
                    user = Request.Headers["IV-USER"];
                }

                if (!String.IsNullOrEmpty(Request.Headers["PD-UID"]))
                {
                    user = Request.Headers["PD-UID"];
                }

                _log4net.Debug($"LoggedBaseController.SignAsUserAsync() (Fim)");

                if (User == null || User.Identity == null)
                    await SignAsUserAsync(user);
                else
                {
                    if (!User.Identity.IsAuthenticated || User.Identity.Name != user)
                        await SignAsUserAsync(user);
                }
            }
        }


        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public virtual async Task SignAsUserAsync(string user)
        {
            //----------------------------------------
            //_log4net.Debug($"MethodName: {MethodBase.GetCurrentMethod().Name} - 3");
            _log4net.Debug($"LoggedBaseController.SignAsUserAsync(user) (Início)");
            //----------------------------------------

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,  user),
                    new Claim(ClaimTypes.Name,  user)
                };


            var IAMHelper = new Bayer.Pegasus.ApiClient.Helpers.IAMHelper();
            IAMHelper.WriteOnLog = Pegasus.Utils.Configuration.Instance.WriteOnLog;

            string ip = "127.0.0.1";

            if (_accessor != null)
            {
                ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            _log4net.Debug("ip: " + ip);

            var roles = IAMHelper.GetRoles(user, ip);

            _log4net.Debug("roles: " + roles.ToString());

            if (_logger != null && IAMHelper.WriteOnLog)
            {
                if (roles == null)
                {

                    _logger.LogInformation("AuthService: Role NULL");
                    _log4net.Debug("AuthService: Role NULL");
                }
                else
                {
                    _logger.LogInformation("AuthService: Role não está NULL");
                    _log4net.Debug("AuthService: Role não está NULL");

                    foreach (var role in roles)
                    {
                        _logger.LogInformation(role.Name);
                        _log4net.Debug($"role.Name: {role.Name}");
                    }
                }

                if (IAMHelper.LogInformation != null)
                {
                    foreach (var message in IAMHelper.LogInformation)
                    {
                        _logger.LogInformation(message);
                        _log4net.Debug($"message: {message}");
                    }
                }

            }

            foreach (var role in roles)
            {
                var roleName = role.Name;
                var levelName = role.Level.Name;

                var claimRole = new Claim(ClaimTypes.Role, roleName);

                if (levelName != null && role.Level.RestrictionCodes != null)
                {
                    if (role.Level.RestrictionCodes.Count > 0)
                    {

                        claimRole.Properties["LevelName"] = levelName.ToUpper();
                        claimRole.Properties[levelName.ToUpper()] = String.Join(";", role.Level.RestrictionCodes.ToArray());

                    }
                }

                claims.Add(claimRole);
            }

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. Required when setting the 
                // ExpireTimeSpan option of CookieAuthenticationOptions 
                // set with AddCookie. Also required when setting 
                // ExpiresUtc.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            var userPrincipal = new ClaimsPrincipal(claimsIdentity);

            HttpContext.User = userPrincipal;

            _log4net.Debug($"LoggedBaseController.SignAsUserAsync(user) (Fim)");

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal, authProperties);

        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _log4net.Debug($"----------------------------------------------------");
            _log4net.Debug($"LoggedBaseController.OnActionExecutionAsync (Início)");

            //----------------------------------------
            //_log4net.Debug($"MethodName: {MethodBase.GetCurrentMethod().Name} - 4");
            _log4net.Debug($"IV-USER: {Request.Headers["IV-USER"]}");
            _log4net.Debug($"PD-UID: {Request.Headers["PD-UID"]}");
            //----------------------------------------

            if (!String.IsNullOrEmpty(Request.Headers["IV-USER"]) || !String.IsNullOrEmpty(Request.Headers["PD-UID"]))
            {

                var user = "";

                if (!String.IsNullOrEmpty(Request.Headers["IV-USER"]))
                {
                    user = Request.Headers["IV-USER"];
                }

                if (!String.IsNullOrEmpty(Request.Headers["PD-UID"]))
                {
                    user = Request.Headers["PD-UID"];
                }

                //_log4net.Debug($"User: {User}");
                //_log4net.Debug($"User.Identity: {User.Identity}");
                _log4net.Debug($"User.Identity.IsAuthenticated: {User.Identity.IsAuthenticated}");
                _log4net.Debug($"User.Identity.Name: {User.Identity.Name}");

                //_log4net.Debug($"Teste User: {(User == null ? "nulo" : User.Identity.Name)}");
                //_log4net.Debug($"Teste User.Identity: {(User.Identity == null ? "nulo" : User.Identity.Name)}");

                if (User == null || User.Identity == null)
                {
                    _log4net.Debug("User == null || User.Identity == null");
                    await SignAsUserAsync(user);
                }
                else
                {
                    if (!User.Identity.IsAuthenticated || User.Identity.Name != user)
                    {
                        _log4net.Debug("!User.Identity.IsAuthenticated || User.Identity.Name != user");
                        _log4net.Debug($"user: {user}");
                        await SignAsUserAsync(user);
                    }
                }

            }

            _log4net.Debug($"LoggedBaseController.OnActionExecutionAsync (Fim)");
            _log4net.Debug($"next: {next}");
            _log4net.Debug($"----------------------------------------------------");

            await next();

        }
    }
}