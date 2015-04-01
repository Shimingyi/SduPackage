using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SduPackage.Funcitons
{
    public class DoPost
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public delegate void HandleResult(string result);
        private HandleResult handle;
        CookieContainer cc = null;

        public void StartPost(string uri, string post, HandleResult handle)
        {
            string result = string.Empty;
            this.handle = handle;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(uri));
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            if (cc == null)
            {
                httpWebRequest.CookieContainer = new CookieContainer();
                if (localSettings.Values.ContainsKey("cookieValue"))
                {
                    Uri cookieuri = new Uri(uri);
                    httpWebRequest.CookieContainer.SetCookies(cookieuri, localSettings.Values["cookieValue"].ToString());
                }
            }
            else
            {
                httpWebRequest.CookieContainer = cc;
            }
            httpWebRequest.BeginGetRequestStream((IAsyncResult streamCallback) =>
            {

                HttpWebRequest webRequest = streamCallback.AsyncState as HttpWebRequest;
                string PostData = post;

                byte[] buffer = Encoding.UTF8.GetBytes(PostData);
                Stream stream = webRequest.EndGetRequestStream(streamCallback);
                stream.Position = 0;
                stream.Write(buffer, 0, buffer.Length);
                stream.Dispose();

                webRequest.BeginGetResponse((IAsyncResult responseCallback) =>
                {
                    try
                    {
                        HttpWebRequest webRequest2 = responseCallback.AsyncState as HttpWebRequest;
                        HttpWebResponse webResponse = (HttpWebResponse)webRequest2.EndGetResponse(responseCallback);
                        foreach (Cookie cook in webResponse.Cookies)
                        {
                            localSettings.Values["cookieValue"] = cook.Value.ToString();
                        }
                        Stream streamResponse = webResponse.GetResponseStream();
                        StreamReader sr = new StreamReader(streamResponse);
                        string str = sr.ReadToEnd();
                        result = str;
                        handle(result);
                    }
                    catch (WebException e)
                    {
                        result = "Error1";
                        handle(result);
                    }
                }, webRequest);
            }, httpWebRequest);
        }
    }
}
