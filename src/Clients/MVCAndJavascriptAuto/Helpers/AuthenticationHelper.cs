using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace MVCAndJavascriptAuto.Helpers
{
    /// <summary>Class AuthenticationHelper provides
    /// help methods to call authentication endpoints</summary>
    public sealed class AuthenticationHelper
    {
        /// <summary>Calls the end point.</summary>
        /// <param name="endPoint">Url of end point.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="encodedCredential">The encoded credential.</param>
        /// <returns>Response text in string</returns>
        public static string CallEndPoint(string endPoint, string requestBody, string encodedCredential)
        {
            string responseText = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
            request.Method = "POST";
            request.Headers.Add(string.Format("Authorization: Basic {0}", encodedCredential));
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*;q=0.8";

            byte[] _byteVersion = Encoding.ASCII.GetBytes(requestBody);
            request.ContentLength = _byteVersion.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            // Get the response
            HttpWebResponse response = null;

            response = (HttpWebResponse)request.GetResponse();
            using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
            {
                // Read response body
                responseText = responseReader.ReadToEnd();
            }
            return responseText;
        }

        /// <summary>
        /// Encodes the credential.
        /// </summary>
        /// <param name="userName">User name or client id</param>
        /// <param name="password">The password or client secret</param>
        /// <returns>Encoded credential in string</returns>
        public static string EncodeCredential(string userName, string password)
        {
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var credential = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", userName, password);
            return Convert.ToBase64String(encoding.GetBytes(credential));
        }

    }

}
