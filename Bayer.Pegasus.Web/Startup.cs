using Bayer.Pegasus.Web.Controllers;
using Bayer.Pegasus.Web.Helpers;
using CacheStrategy.Stores;
using CacheStrategy.Stores.Caching;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;

namespace Bayer.Pegasus.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            //Adiciona o CORS
            services.AddCors();

            services.AddMvc(options =>
            {
                options.InputFormatters.Insert(0, new JObjectBodyInputFormatter());
            });

            SetupConfiguration();


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(
                    options =>
                    {
                        options.LoginPath = Bayer.Pegasus.Utils.Configuration.Instance.LoginURL;
                        options.LogoutPath = Bayer.Pegasus.Utils.Configuration.Instance.LogoutURL;
                    }
                );

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            RegisterServices(services);
            EnableCache(services);
        }

        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<ITokenStore, TokenStore>();
            services.AddScoped<ILoginStore, LoginStore>();
            services.AddScoped<IBusinessUnitCodeStore, BusinessUnitCodeStore>();
        }


        public void EnableCache(IServiceCollection services)
        {
            services.AddScoped<TokenStore>();
            services.AddScoped<ITokenStore, TokenCachingStore<TokenStore>>();

            services.AddScoped<LoginStore>();
            services.AddScoped<ILoginStore, LoginCachingStore<LoginStore>>();

            services.AddScoped<BusinessUnitCodeStore>();
            services.AddScoped<IBusinessUnitCodeStore, BusinessUnitCodeCachingStore<BusinessUnitCodeStore>>();
        }

        

        private void SetupConfiguration()
        {

            //Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString = Configuration.GetConnectionString("PegasusConnection");

            Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString_ODS = Configuration.GetConnectionString("PegasusConnection_ODS");
            Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString_DW = Configuration.GetConnectionString("PegasusConnection_DW");
            Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString = Configuration.GetConnectionString("PegasusConnection_DMi");


            Bayer.Pegasus.Utils.Configuration.Instance.UseTokenLifeTime = Configuration.GetValue<bool>("UseTokenLifeTime");
            Bayer.Pegasus.Utils.Configuration.Instance.TokenLifeTime = Configuration.GetValue<int>("TokenLifeTime");

            Bayer.Pegasus.Utils.Configuration.Instance.UrlApiOauthToken = Configuration.GetSection("ServicesURL")["OauthToken"];
            Bayer.Pegasus.Utils.Configuration.Instance.UrlApiAuthServicesLogin = Configuration.GetSection("ServicesURL")["AuthServicesLogin"];
            Bayer.Pegasus.Utils.Configuration.Instance.UrlApiAuthServicesLoginSSO = Configuration.GetSection("ServicesURL")["AuthServicesLoginSSO"];
            Bayer.Pegasus.Utils.Configuration.Instance.UrlApiAuthServicesLogout = Configuration.GetSection("ServicesURL")["AuthServicesLogout"];


            Bayer.Pegasus.Utils.Configuration.Instance.ServiceProductURL = Configuration.GetSection("ServicesURL")["Product"];
            Bayer.Pegasus.Utils.Configuration.Instance.ServicePartnerURL = Configuration.GetSection("ServicesURL")["Partner"];
            Bayer.Pegasus.Utils.Configuration.Instance.ServiceIAMURL = Configuration.GetSection("ServicesURL")["IAM"];
            Bayer.Pegasus.Utils.Configuration.Instance.ServiceSalesStructureURL = Configuration.GetSection("ServicesURL")["SalesStructure"];
            Bayer.Pegasus.Utils.Configuration.Instance.ServiceTokenURL = Configuration.GetSection("ServicesURL")["Token"];
            Bayer.Pegasus.Utils.Configuration.Instance.ServiceProductLegaciesURL = Configuration.GetSection("ServicesURL")["ProductLegacies"];
            Bayer.Pegasus.Utils.Configuration.Instance.ServiceProductLegaciesBUURL = Configuration.GetSection("ServicesURL")["ProductLegaciesBU"];

            Bayer.Pegasus.Utils.Configuration.Instance.ServiceEmailURL = Configuration.GetSection("ServicesURL")["Email"];

            Bayer.Pegasus.Utils.Configuration.Instance.LoginURL = Configuration["LoginURL"];
            Bayer.Pegasus.Utils.Configuration.Instance.LogoutURL = Configuration["LogoutURL"];
            Bayer.Pegasus.Utils.Configuration.Instance.AppCulture = Configuration["AppCulture"];
            Bayer.Pegasus.Utils.Configuration.Instance.AppId = Configuration["AppId"];

            Bayer.Pegasus.Utils.Configuration.Instance.AppDomainURL = Configuration["AppDomainURL"];
            Bayer.Pegasus.Utils.Configuration.Instance.PortalOneDomainURL = Configuration["PortalOneDomainURL"];


            Bayer.Pegasus.Utils.Configuration.Instance.URLPrefix = Configuration["URLPrefix"];


            Bayer.Pegasus.Utils.Configuration.Instance.UseServicesOnTokenField = Configuration.GetValue<bool>("UseServicesOnTokenField");

            Bayer.Pegasus.Entities.SalesStructureAccess.RolesAdministrator = Configuration.GetSection("Roles")["Administrator"].Split(';').ToList();
            Bayer.Pegasus.Entities.SalesStructureAccess.RolesConsulting = Configuration.GetSection("Roles")["Consult"].Split(';').ToList();
            Bayer.Pegasus.Entities.SalesStructureAccess.RolesSalesOffice = Configuration.GetSection("Roles")["SalesOffice"].Split(';').ToList();
            Bayer.Pegasus.Entities.SalesStructureAccess.RolesSalesDistrict = Configuration.GetSection("Roles")["SalesDistrict"].Split(';').ToList();
            Bayer.Pegasus.Entities.SalesStructureAccess.RolesSalesRepresentative = Configuration.GetSection("Roles")["SalesRepresentative"].Split(';').ToList();
            Bayer.Pegasus.Entities.SalesStructureAccess.RolesPartner = Configuration.GetSection("Roles")["Partner"].Split(';').ToList();

            Bayer.Pegasus.Utils.Configuration.Instance.AppIdLogin = Configuration["AppIdLogin"];
            Bayer.Pegasus.Utils.Configuration.Instance.ClientId = Configuration["ClientId"];
            Bayer.Pegasus.Utils.Configuration.Instance.ClientSecret = Configuration["ClientSecret"];

            Bayer.Pegasus.Utils.Configuration.Instance.ErrorMailTo = Configuration["ErrorMailTo"];

            Bayer.Pegasus.Utils.Configuration.Instance.UseServicesOnTokenFieldProducts = Configuration.GetValue<bool>("UseServicesOnTokenFieldProducts");
            Bayer.Pegasus.Utils.Configuration.Instance.UseServicesOnTokenFieldPartner = Configuration.GetValue<bool>("UseServicesOnTokenFieldPartner");
            Bayer.Pegasus.Utils.Configuration.Instance.UseServicesOnTokenFieldBrand = Configuration.GetValue<bool>("UseServicesOnTokenFieldBrand");

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Environment = env;

            string baseDir = env.ContentRootPath;

            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(baseDir, "App_Data"));


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //Configura o CORS
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseStaticFiles();

            app.UseAuthentication();

            var supportedCultures = new[] { new CultureInfo("pt-BR") };


            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("pt-BR"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Dashboard}/{action=Index}/{id?}");
            });


        }
    }
}