using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Web;
using System.Configuration;
using System.Runtime.Serialization.Json;

namespace SwsExample
{
	public partial class ClientForm : Form
    {
        #region CONST
        private const string ENDPOINT_AUTHENTICATION = "/authenticate";
        private const string ENDPOINT_GEOCODE = "/geocode";
        private const string ENDPOINT_MAP_TILE = "/map";

        private const string PROTOCOL = "http";

        private const string HTTP_METHOD_POST = "POST";
        private const string HTTP_METHOD_GET = "GET";

        private const string HTTP_CONTENT_TYPE_JSON = "application/json";
        private const string HTTP_CONTENT_TYPE_PNG = "image/png";

        private const string PARAM_GEOCODE_ADDRESS = "address";
        private const string PARAM_GEOCODE_CITY = "city";
        private const string PARAM_GEOCODE_DETAIL = "detail";
        private const string PARAM_GEOCODE_AUTHKEY = "authKey"; 
        private const string PARAM_GEOCODE_HEIGHT = "height";
        private const string PARAM_GEOCODE_WIDTH = "width";
        private const string PARAM_GEOCODE_LAYERS = "layers";
        private const string PARAM_GEOCODE_STYLES = "styles";
        private const string PARAM_GEOCODE_BBOX = "bbox";
        #endregion

        private string swsRoot;
        private Uri baseURI;
        private string authkey;
                        
        /// <summary>
        /// Performs the authentication step for an SWS API consumer.
        /// 
        /// Authentication involves an HTTP POST to the {ENDPOINT_AUTHENTICATION} with the username and password for SWS
        /// passed as a JSON object in the request body. Upon success, an authentication key is returned as a JSON object
        /// with a single 'authKey' property to be used as a query parameter in all future requests.
        /// 
        /// For convenience, the authentication key is cached as a data member of this class and supplied for each
        /// subsequent request in this example.
        /// </summary>
		void ExecuteAuthenticationRequest()
		{
            string responseAsString = "";

            string username = ConfigurationManager.AppSettings["swsUsername"];
            string password = ConfigurationManager.AppSettings["swsPassword"];

            // check if username and password contain text
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                Console.WriteLine("Please set the username and password in app.config before continuing");
                return;
            }

            // build the request
            Uri authUri = BuildUri(this.swsRoot + ENDPOINT_AUTHENTICATION);

            try
            {
                // make the request
                AuthenticationRequest authenticationRequest = new AuthenticationRequest();
                authenticationRequest.Username = username;
                authenticationRequest.Password = password;
                string json = GenerateJsonString(authenticationRequest);
                
                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(authUri);
                request.Method = HTTP_METHOD_POST;
                request.ContentType = HTTP_CONTENT_TYPE_JSON;
                SetBody(request, json);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    responseAsString = ConvertResponseToString(response);
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    responseAsString += HandleErrorResponse(response);
                }

            }
            catch (Exception ex)
            {
                responseAsString += "ERROR: " + ex.Message;
            }
            
			ResponseTextBox.Text = responseAsString;
		}

        /// <summary>
        /// Make a request to SWS to Geocode an address.
        /// 
        /// The required query parameters are set in the {APP.CONFIG} file.  The auth key query parameter has the value of
        /// the authKey obtained in {ExecuteAuthenticationRequest()}.
        /// </summary>
        void ExecuteGeocodeRequest()
        {
            string responseAsString = "";

            // build the request
            var coll = HttpUtility.ParseQueryString(string.Empty);
            coll[PARAM_GEOCODE_ADDRESS] = ConfigurationManager.AppSettings["swsGeocodeAddress"];
            coll[PARAM_GEOCODE_CITY] = ConfigurationManager.AppSettings["swsGeocodeCity"];
            coll[PARAM_GEOCODE_DETAIL] = ConfigurationManager.AppSettings["swsGeocodeIncludeDetail"];
            coll[PARAM_GEOCODE_AUTHKEY] = this.authkey;

            Uri geocodeUri = BuildUri(this.swsRoot + ENDPOINT_GEOCODE, coll.ToString());

            try
            {
                // make the request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(geocodeUri);
                request.Method = HTTP_METHOD_GET;
                request.ContentType = HTTP_CONTENT_TYPE_JSON;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    responseAsString = ConvertResponseToString(response);
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    responseAsString = HandleErrorResponse(response);
                }

            }
            catch (Exception ex)
            {
                responseAsString += "ERROR: " + ex.Message;
            }

            ResponseTextBox.Text = responseAsString;
        }

        /// <summary>
        /// Make a request to SWS to get a map tile.
        /// 
        /// The required query parameters are set in the {APP.CONFIG} file.  The auth key query parameter has the value of
        /// the authKey obtained in {ExecuteAuthenticationRequest()}.
        /// </summary>
        void ExecuteMapTileRequest()
        {
            string responseAsString = "";

            // build the request
            var coll = HttpUtility.ParseQueryString(string.Empty);
            coll[PARAM_GEOCODE_HEIGHT] = ConfigurationManager.AppSettings["swsMapTileHeight"];
            coll[PARAM_GEOCODE_WIDTH] = ConfigurationManager.AppSettings["swsMapTileWidth"];
            coll[PARAM_GEOCODE_LAYERS] = ConfigurationManager.AppSettings["swsMapTileLayers"];
            coll[PARAM_GEOCODE_STYLES] = ConfigurationManager.AppSettings["swsMapTileStyles"];
            coll[PARAM_GEOCODE_BBOX] = ConfigurationManager.AppSettings["swsMapTileBbox"];
            coll[PARAM_GEOCODE_AUTHKEY] = this.authkey;

            Uri mapTileUri = BuildUri(this.swsRoot + ENDPOINT_MAP_TILE, coll.ToString());

            try
            {
                // make the request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(mapTileUri);
                request.Method = HTTP_METHOD_GET;
                request.ContentType = HTTP_CONTENT_TYPE_PNG;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    responseAsString = ConvertResponseToString(response);
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    responseAsString = HandleErrorResponse(response);
                }

            }
            catch (Exception ex)
            {
                responseAsString += "ERROR: " + ex.Message;
            }

            ResponseTextBox.Text = responseAsString;
        }

        private string GenerateJsonString(object o)
        {
            DataContractJsonSerializer ser =
                new DataContractJsonSerializer(o.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                ser.WriteObject(ms, o);
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        
        private Uri BuildUri(string endpoint)
        {
            return BuildUri(endpoint, null);

        }

        private Uri BuildUri(string endpoint, string query)
        {
            if (String.IsNullOrEmpty(endpoint))
            {
                Console.WriteLine("Please make sure you supply a valid URI endpoint");
                return null;
            }

            // form the url request
            if (!string.IsNullOrEmpty(query))
            {
                UriBuilder uriBuilder = new UriBuilder(new Uri(this.baseURI,endpoint));
                uriBuilder.Query = query;

                return uriBuilder.Uri;
            }
            else
            {
                return new Uri(this.baseURI, endpoint);
            }

        }
        
        void SetBody(HttpWebRequest request, string requestBody)
		{
			if (requestBody.Length > 0)
			{
				using (Stream requestStream = request.GetRequestStream())
				using (StreamWriter writer = new StreamWriter(requestStream))
				{
					writer.Write(requestBody);
				}
			}
		}
        
		string ConvertResponseToString(HttpWebResponse response)
		{

			string result = "Status code: " + (int)response.StatusCode + " " + response.StatusCode + "\r\n";

			foreach (string key in response.Headers.Keys)
			{
				result += string.Format("{0}: {1} \r\n", key, response.Headers[key]);
			}

            result += "\r\n";

            if (rbMapTile.Checked)
            {
                Stream receiveStream = response.GetResponseStream();
                using (BinaryReader br = new BinaryReader(receiveStream))
                {
                    byte[] ba = br.ReadBytes(500000);
                    MemoryStream ms = new MemoryStream(ba);
                    Image mapTileImage = Image.FromStream(ms);
                    MapTilePictureBox.Image = mapTileImage;
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string objText = reader.ReadToEnd();
                    if (rbAuthenticate.Checked)
                    {
                        AuthenticationResponse authResponse =
                            Deserialize<AuthenticationResponse>(objText);

                        this.authkey = authResponse.Authkey;
                    }

                    result += objText;
                }
            }

			return result;
		}

        private static T Deserialize<T>(string json)
        {
            var instance = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(instance.GetType());
                return (T)serializer.ReadObject(ms);
            }
        }

        string HandleErrorResponse(HttpWebResponse response)
        {
            try
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string jsonString = reader.ReadToEnd();
                    ErrorResponse errorResponse =
                        Deserialize<Wrapper>(jsonString).Error;

                    return errorResponse.Message;
                }
            }
            catch
            {
                return ConvertResponseToString(response);
            }
        }

		public ClientForm()
		{
			InitializeComponent();
		}

		void ClientForm_Load(object sender, EventArgs e)
		{
            string port = ConfigurationManager.AppSettings["swsPort"];
            if (string.IsNullOrEmpty(port))
            {
                Console.WriteLine("Please make sure the port is valid");
                return;
            }

            int swsPort = Convert.ToInt16(port);

            string swsHost = ConfigurationManager.AppSettings["swsHost"];
            if (string.IsNullOrEmpty(swsHost))
            {
                Console.WriteLine("Please make sure the host is valid");
                return;
            }

            this.swsRoot = ConfigurationManager.AppSettings["swsRoot"];
            if (string.IsNullOrEmpty(this.swsRoot))
            {
                Console.WriteLine("Please make sure the root is valid");
                return;
            }

            // base URI
            this.baseURI = new UriBuilder(PROTOCOL, swsHost, swsPort).Uri;

		}

		void ExecuteButton_Click(object sender, EventArgs e)
        {
            MapTilePictureBox.Image = null;
            ResponseTextBox.Clear();

            if (rbAuthenticate.Checked)
            {
                ExecuteAuthenticationRequest();
            }
            else if (rbGeocode.Checked)
            {
                ExecuteGeocodeRequest();
            }
            else if (rbMapTile.Checked)
            {
                ExecuteMapTileRequest();
            }
		}

	}
}
