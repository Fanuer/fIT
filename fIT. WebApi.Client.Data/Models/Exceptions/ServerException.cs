using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;

namespace fIT.WebApi.Client.Data.Models.Exceptions
{
    /// <summary>
    /// Standard Exception, if the ManagementApi receives an error from the server
    /// </summary>
    public class ServerException : Exception
    {
        #region Field
        #endregion

        #region Ctor
        public ServerException(HttpResponseMessage response)
        {
            if (response == null) { throw new ArgumentNullException("reason"); }
            this.Response = response;
            Initialise();
        }
        #endregion

        #region Methods

        private void Initialise()
        {
            var httpErrorObject = this.Response.Content.ReadAsStringAsync().Result;

            // Create an anonymous object to use as the template for deserialization:
            var anonymousErrorObject = new { message = "", ModelState = new Dictionary<string, string[]>() };

            try
            {
                // Deserialize:
                var deserializedErrorObject = JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject);
                // Sometimes, there may be Model Errors:
                if (deserializedErrorObject != null)
                {
                    if (deserializedErrorObject.ModelState != null)
                    {
                        var errors = deserializedErrorObject.ModelState.Select(kvp => string.Join(". ", kvp.Value));
                        for (int i = 0; i < errors.Count(); i++)
                        {
                            // Wrap the errors up into the base Exception.Data Dictionary:
                            this.Data.Add(i, errors.ElementAt(i));
                        }
                    }
                    // Othertimes, there may not be Model Errors:
                    else
                    {
                        var error = JsonConvert.DeserializeObject<Dictionary<string, string>>(httpErrorObject);

                        foreach (var kvp in error)
                        {
                            // Wrap the errors up into the base Exception.Data Dictionary:
                            this.Data.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                else
                {
                    this.Data.Add("Error", "An unknown error has occured");
                }
            }
            catch (JsonReaderException e)
            {
                throw new HttpRequestException(this.Response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Properties
        public HttpResponseMessage Response { get; set; }

        public HttpStatusCode StatusCode
        {
            get
            {
                return this.Response.StatusCode;
            }
        }

        public IEnumerable<string> Errors
        {
            get
            {
                return this.Data.Values.Cast<string>().ToList();
            }
        }
        #endregion


    }
}