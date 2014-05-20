/*
 * Copyright (c) 2013, CoreLogic, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 *
 * Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, 
 * this list of conditions and the following disclaimer in the documentation 
 * and/or other materials provided with the distribution.
 * Neither the name of CoreLogic nor the names of its 
 * contributors may be used to endorse or promote products derived from 
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Configuration;
using System.Runtime.Serialization.Json;

namespace SwsExample
{
    public class SwsExample
    {
        #region ENUM
        public enum Task
        {
            AUTHENTICATION, GEOCODE, RETRIEVE_MAP_TILE
        }
        #endregion

        #region CONST
        // global
        private const string SWS_SAMPLE_HOST_KEY = "corelogic.sws.sample.host";
        private const string SWS_SAMPLE_PORT_KEY = "corelogic.sws.sample.port";
        private const string SWS_SAMPLE_ROOT_KEY = "corelogic.sws.sample.root";

        // authentication task
        private const string SWS_SAMPLE_USERNAME_KEY = "corelogic.sws.sample.authentication.username";
        private const string SWS_SAMPLE_PASSWORD_KEY = "corelogic.sws.sample.authentication.password";

        // geocode task
        private const string SWS_SAMPLE_ADDRESS_KEY = "corelogic.sws.sample.geocode.address";
        private const string SWS_SAMPLE_CITYSTATEZIP_KEY = "corelogic.sws.sample.geocode.cityStateZip";

        // map tile task
        private const string SWS_SAMPLE_HEIGHT_KEY = "corelogic.sws.sample.mapTile.height";
        private const string SWS_SAMPLE_WIDTH_KEY = "corelogic.sws.sample.mapTile.width";
        private const string SWS_SAMPLE_LAYERS_KEY = "corelogic.sws.sample.mapTile.layers";
        private const string SWS_SAMPLE_STYLES_KEY = "corelogic.sws.sample.mapTile.styles";
        private const string SWS_SAMPLE_BBOX_KEY = "corelogic.sws.sample.mapTile.bbox";

        // Endpoints
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
        private const string PARAM_GEOCODE_AUTHKEY = "authKey";
        private const string PARAM_GEOCODE_HEIGHT = "height";
        private const string PARAM_GEOCODE_WIDTH = "width";
        private const string PARAM_GEOCODE_LAYERS = "layers";
        private const string PARAM_GEOCODE_STYLES = "styles";
        private const string PARAM_GEOCODE_BBOX = "bbox";
        #endregion

        private int appPort;
        private string appHost;
        private string appRoot;
        private Uri baseURI;
        private string authkey;

        /// <summary>
        /// Create and initialize for making requests to the SWS web application.
        /// Grab values from the app.config file
        /// </summary>
        public SwsExample()
        {
            string portVal = ConfigurationManager.AppSettings[SWS_SAMPLE_PORT_KEY];
            if (!string.IsNullOrEmpty(portVal))
            {
                try
                {
                    this.appPort = Convert.ToInt16(portVal);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Illegal value for integer port value: " + portVal + "; defaulting to 80.");
                    this.appPort = 80;
                }
            }
            else
            {
                this.appPort = 80;
            }

            this.appHost = ConfigurationManager.AppSettings[SWS_SAMPLE_HOST_KEY];
            if (string.IsNullOrEmpty(this.appHost))
            {
                Console.WriteLine("Please make sure the host is valid");
                return;
            }

            this.appRoot = ConfigurationManager.AppSettings[SWS_SAMPLE_ROOT_KEY];
            if (string.IsNullOrEmpty(this.appRoot))
            {
                Console.WriteLine("Please make sure the root is valid");
                return;
            }

            // base URI
            this.baseURI = new UriBuilder(PROTOCOL, this.appHost, this.appPort).Uri;
        }

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
            PrintStart(Task.AUTHENTICATION);

            string username = ConfigurationManager.AppSettings[SWS_SAMPLE_USERNAME_KEY];
            string password = ConfigurationManager.AppSettings[SWS_SAMPLE_PASSWORD_KEY];

            // check if username and password contain text
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                Console.WriteLine("WARNING: Please set the username and password in app.config before continuing.");
                Environment.Exit(0);
            }

            // build the request
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = PROTOCOL;
            uriBuilder.Host = this.appHost;
            uriBuilder.Port = this.appPort;
            uriBuilder.Path = this.appRoot + ENDPOINT_AUTHENTICATION;
            Uri authUri = uriBuilder.Uri;

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

                PrintRequest(request, json);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = CopyAndClose(response.GetResponseStream()))
                    {
                        PrintResponse(response, responseStream);
                        responseStream.Position = 0;
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            string objText = reader.ReadToEnd();
                            AuthenticationResponse authResponse =
                                Deserialize<AuthenticationResponse>(objText);

                            this.authkey = authResponse.Authkey;
                        }
                    }
                }

                PrintComplete(Task.AUTHENTICATION);
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    PrintErrorAndExit(Task.AUTHENTICATION, response);
                    Environment.Exit(0);
                }
            }
        }

        private static Stream CopyAndClose(Stream inputStream)
        {
            const int readSize = 256;
            byte[] buffer = new byte[readSize];
            MemoryStream ms = new MemoryStream();

            int count = inputStream.Read(buffer, 0, readSize);
            while (count > 0)
            {
                ms.Write(buffer, 0, count);
                count = inputStream.Read(buffer, 0, readSize);
            }
            ms.Position = 0;
            inputStream.Close();
            return ms;
        }
        
        /// <summary>
        /// Make a request to SWS to Geocode an address.
        /// 
        /// The required query parameters are set in the {APP.CONFIG} file.  The auth key query parameter has the value of
        /// the authKey obtained in {ExecuteAuthenticationRequest()}.
        /// </summary>
        void ExecuteGeocodeRequest()
        {
            PrintStart(Task.GEOCODE);

            // build the request
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = PROTOCOL;
            uriBuilder.Host = this.appHost;
            uriBuilder.Port = this.appPort;
            uriBuilder.Path = this.appRoot + ENDPOINT_GEOCODE;

            var coll = HttpUtility.ParseQueryString(string.Empty);
            coll[PARAM_GEOCODE_ADDRESS] = ConfigurationManager.AppSettings[SWS_SAMPLE_ADDRESS_KEY];
            coll[PARAM_GEOCODE_CITY] = ConfigurationManager.AppSettings[SWS_SAMPLE_CITYSTATEZIP_KEY];
            coll[PARAM_GEOCODE_AUTHKEY] = this.authkey;

            uriBuilder.Query = coll.ToString();

            Uri geocodeUri = uriBuilder.Uri;

            try
            {
                // make the request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(geocodeUri);
                request.Method = HTTP_METHOD_GET;
                request.ContentType = HTTP_CONTENT_TYPE_JSON;

                PrintRequest(request, null);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = CopyAndClose(response.GetResponseStream()))
                    {
                        PrintResponse(response, responseStream);
                    }
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    PrintErrorAndExit(Task.GEOCODE, response);
                    Environment.Exit(0);
                }
            }

            PrintComplete(Task.GEOCODE);
        }

        /// <summary>
        /// Make a request to SWS to get a map tile.
        /// 
        /// The required query parameters are set in the {APP.CONFIG} file.  The auth key query parameter has the value of
        /// the authKey obtained in {ExecuteAuthenticationRequest()}.
        /// </summary>
        void ExecuteMapTileRequest()
        {
            PrintStart(Task.RETRIEVE_MAP_TILE);

            // build the request
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = PROTOCOL;
            uriBuilder.Host = this.appHost;
            uriBuilder.Port = this.appPort;
            uriBuilder.Path = this.appRoot + ENDPOINT_MAP_TILE;
           
            var coll = HttpUtility.ParseQueryString(string.Empty);
            coll[PARAM_GEOCODE_HEIGHT] = ConfigurationManager.AppSettings[SWS_SAMPLE_HEIGHT_KEY];
            coll[PARAM_GEOCODE_WIDTH] = ConfigurationManager.AppSettings[SWS_SAMPLE_WIDTH_KEY];
            coll[PARAM_GEOCODE_LAYERS] = ConfigurationManager.AppSettings[SWS_SAMPLE_LAYERS_KEY];
            coll[PARAM_GEOCODE_STYLES] = ConfigurationManager.AppSettings[SWS_SAMPLE_STYLES_KEY];
            coll[PARAM_GEOCODE_BBOX] = ConfigurationManager.AppSettings[SWS_SAMPLE_BBOX_KEY];
            coll[PARAM_GEOCODE_AUTHKEY] = this.authkey;

            uriBuilder.Query = coll.ToString();

            try
            {
                // make the request
                Uri mapTileUri = uriBuilder.Uri;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(mapTileUri);
                request.Method = HTTP_METHOD_GET;
                request.ContentType = HTTP_CONTENT_TYPE_PNG;

                PrintRequest(request, null);

                byte[] responseBody = null;
                
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (BinaryReader br = new BinaryReader(response.GetResponseStream()))
                    {
                        responseBody = br.ReadBytes(500000);
                    }
                    PrintResponse(response, null);
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    PrintErrorAndExit(Task.RETRIEVE_MAP_TILE, response);
                    Environment.Exit(0);
                }
            }

            PrintComplete(Task.RETRIEVE_MAP_TILE);
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

        private static T Deserialize<T>(string json)
        {
            var instance = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(instance.GetType());
                return (T)serializer.ReadObject(ms);
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

        void PrintStart(Task task)
        {
            Console.WriteLine("==========");
            Console.WriteLine(string.Format("\r\nStarting {0}...", task));
        }

        void PrintComplete(Task task)
        {
            Console.WriteLine(string.Format("\r\nSuccessfully completed {0}\r\n", task));
        }

	    void PrintRequest (HttpWebRequest request, String requestBody) {
            Console.WriteLine("REQUEST:");
            Console.WriteLine(request.Method + " " + request.RequestUri.ToString());

            Console.WriteLine("\nHttpHeaders:\n{0}",request.Headers);

		    if (requestBody != null) {
                Console.WriteLine(string.Format("Request body:\n{0}", requestBody));
		    }

            Console.WriteLine();
            Console.WriteLine();
	    }
                
        void PrintResponse(HttpWebResponse response, Stream responseStream)
        {
            Console.WriteLine("RESPONSE:");
            Console.WriteLine((int)response.StatusCode + " " + response.StatusCode);

            Console.WriteLine("\nHttpHeaders:\n{0}", response.Headers);
            
            if (responseStream != null)
            {
                StreamReader reader = new StreamReader(responseStream);
                if (reader != null)
                {
                    Console.Write(string.Format("Response body:\r\n{0}", reader.ReadToEnd()));
                }
            }

            Console.WriteLine();
        }

        void PrintErrorAndExit(Task task, HttpWebResponse response)
        {
            try
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string jsonString = reader.ReadToEnd();
                    List<ErrorResponse> errorResponseList =
                        Deserialize<ErrorWrapper>(jsonString).Errors;

                    foreach (ErrorResponse errorResponse in errorResponseList)
                    {
                        Console.WriteLine(string.Format("SWS error during {0} - {1}", task, errorResponse.Message));
                    }

                    Environment.Exit(1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while parsing SWS error response");
                PrintExceptionAndExit(task, e);
            }
        }

        void PrintExceptionAndExit(Task task, Exception e)
        {
            Console.WriteLine(string.Format("Error during {0} - {1}", task, e));
            Environment.Exit(1);
        }
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            // Create a new sample to work with
            SwsExample sws = new SwsExample();

            // Authenticate and receive authKey
            sws.ExecuteAuthenticationRequest();

            // Do a Geocode of an address
            sws.ExecuteGeocodeRequest();

            // Get a map tile
            sws.ExecuteMapTileRequest();
        }
    }
}
