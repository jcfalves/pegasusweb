using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RestSharp;

namespace Bayer.Pegasus.ApiClient.Helpers
{
    public class SAMLHelper
    {
        public  const String TEMPLATE = "saml-template.xml";
	
	    //properties
	    private static String ASSERTION_TAG = "saml2:Assertion";
        private static String SAML_TOKEN_ENDPOINT = "/UT";
        private String REQUEST_STRING = string.Empty;

        private string BasePath {
            get;
            set;
        }

        public SAMLHelper()
        {
            this.BasePath = Bayer.Pegasus.Utils.Configuration.Instance.ServiceTokenURL;
            this.REQUEST_STRING = LoadTemplate(TEMPLATE, Path.Combine(Bayer.Pegasus.Utils.Configuration.Instance.DataDirectory, "xml"));
        }

        public SAMLHelper(String basePath)
        {
            this.BasePath = basePath;
            this.REQUEST_STRING = LoadTemplate(TEMPLATE, Path.Combine(Bayer.Pegasus.Utils.Configuration.Instance.DataDirectory, "xml"));
        }

        public SAMLHelper(String basePath, string folder) {
            this.BasePath = basePath;
            this.REQUEST_STRING = LoadTemplate(TEMPLATE, folder);
        }

        private string LoadTemplate(string template, string folder) {
            var pathTemplate = System.IO.Path.Combine(folder, template);
            var data = System.IO.File.ReadAllText(pathTemplate);

            return data;
        }

        private String ExecutePost(String resource, String body)
        {
            var client = new RestClient(BasePath);
            var request = new RestRequest(resource, Method.POST) { RequestFormat = DataFormat.Xml };
            request.AddParameter("application/xml", body, ParameterType.RequestBody);
            var response = client.Post(request);
            return response.Content;
        }

        private String LoadStringFromXML(System.Xml.XmlNode node) 
        {
            return node.OuterXml;
        }




        private String EncodeRedirectFormat(String samlXML)
        {

            var output = Ionic.Zlib.DeflateStream.CompressString(samlXML);

            return Convert.ToBase64String(output);

        }


        private System.Xml.XmlDocument LoadXMLFromString(String xml) 
        {
            var doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);

            return doc;
        }

        public String DoSAMLAssertion()
        {
            String response = "";
            if (Bayer.Pegasus.Utils.Configuration.Instance.TokenSAML != null)
            {
                if (Bayer.Pegasus.Utils.Configuration.Instance.TokenSAML.IsUpdated) {
                    return Bayer.Pegasus.Utils.Configuration.Instance.TokenSAML.Token;
                }
            }

            try
            {
                response = ExecutePost(SAML_TOKEN_ENDPOINT, REQUEST_STRING);
                System.Xml.XmlDocument root = LoadXMLFromString(response);
                var list = root.GetElementsByTagName(ASSERTION_TAG);
                var tag = list.Item(0);
                String assertion = LoadStringFromXML(tag);
                String encoded = EncodeRedirectFormat(assertion);
                var token = "SAML " + encoded;

                Bayer.Pegasus.Utils.Configuration.Instance.TokenSAML = new Bayer.Pegasus.Entities.TokenSAML(token);

                return token;
            }
            catch(Exception ex)
            {
                System.IO.File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\customlogs\\auth", "logAuthError-" + DateTime.Now.ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-") + ".txt"), DateTime.Now.ToString() + "\r\n\r\n Endpoint \r\n\r\n" + SAML_TOKEN_ENDPOINT + "\r\n\r\n Request String \r\n\r\n" + REQUEST_STRING + "\r\n\r\n Response \r\n\r\n" + response + "\r\n\r\n" + ex.ToString() + "\r\n\r\n" + ex.StackTrace + "\r\n\r\n" + ex.InnerException);
            }

            return "";

        }
    }
}
