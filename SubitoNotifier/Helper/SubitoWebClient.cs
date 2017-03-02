﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SubitoNotifier.Helper
{
    public class SubitoWebClient : WebClient
    {
        public async Task<string> getLoginResponse(string loginData, Uri uri)
        {
            CookieContainer container;

            var request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = "POST";
            request.ContentType = "application/json";
            var buffer = Encoding.ASCII.GetBytes(loginData);
            request.ContentLength = buffer.Length;
            var requestStream = request.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();

            container = request.CookieContainer = new CookieContainer();

            var response = await request.GetResponseAsync();
            CookieContainer = container;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public SubitoWebClient(CookieContainer container)
        {
            CookieContainer = container;
        }

        public SubitoWebClient()
          : this(new CookieContainer())
        { }

        public CookieContainer CookieContainer { get; private set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            var request = (HttpWebRequest)base.GetWebRequest(uri);
            this.Headers.Add("Accept", "*/*");
            this.Headers.Add("host", "hades.subito.it");
            this.Headers.Add("X-Subito-Channel", "50");
            this.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36");
            this.Headers.Add("Accept-Language", "it-IT;q=1, en-US;q=0.9");
            this.Headers.Add("Accept-Encoding", "gzip, deflate");
            this.Headers.Add("Connection", "keep-alive");
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.CookieContainer = CookieContainer;
            return request;
        }

        public async Task<bool> DeleteRequest(Uri uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "DELETE";
            request.CookieContainer = this.CookieContainer;
            HttpWebResponse response =  (HttpWebResponse) await request.GetResponseAsync();
            if (response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> GetRequest(Uri uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.CookieContainer = this.CookieContainer;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public async Task<string> PostRequest (string message ,Uri uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.CookieContainer = this.CookieContainer;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.ContentType = "application/x-www-form-urlencoded";

            var buffer = Encoding.ASCII.GetBytes(message);
            request.ContentLength = buffer.Length;
            var requestStream = request.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();

            var response = await request.GetResponseAsync();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        //public async Task<string> PostImageRequest(string imageString, int category, Uri uri)
        //{
        //    var request = (HttpWebRequest)WebRequest.Create(uri);
        //    request.Method = "POST";
        //    request.CookieContainer = this.CookieContainer;

        //    var webClient = new WebClient();
        //    string boundary = "__END_OF_PART__";
        //    request.ContentType = "multipart/form-data; boundary=" + boundary;
        //    DateTime date = DateTime.Now;
        //    string package = string.Format("--{0}\r\nAccept-Encoding: gzip\r\nContent-Length: {1}\r\nContent-Type: image/png\r\ncontent-disposition: form-data; name=\"image\"; filename=\"{2}\"\r\ncontent-transfer-encoding: binary\r\n\r\n{3}\r\n", boundary, imageString.Length, "IMG_" + date.ToString("yyyyMMdd_HHmmss") + "_-497335034.png", imageString);
        //    package += string.Format("--{0}\r\nAccept-Encoding: gzip\r\nContent-Length: {1}\r\nContent-Type: text/plain\r\ncontent-disposition: form-data; name=\"category\"\r\ncontent-transfer-encoding: binary\r\n\r\n{2}\r\n--{0}--\r\n", boundary, category.ToString().Length, category);

        //    var buffer = Encoding.ASCII.GetBytes(package);
        //    request.ContentLength = buffer.Length;
        //    var requestStream = request.GetRequestStream();
        //    requestStream.Write(buffer, 0, buffer.Length);
        //    requestStream.Close();

        //    var response = await request.GetResponseAsync();
        //    using (var reader = new StreamReader(response.GetResponseStream()))
        //    {
        //        return reader.ReadToEnd();
        //    }
        //}


        public async Task<string> PostImageRequest(string imageString, int category, Uri uri)
        {
            Uri newUri = new Uri(uri + "?category=" + category);
            var request = (HttpWebRequest)WebRequest.Create(newUri);
            request.Method = "POST";
            request.CookieContainer = this.CookieContainer;

            var webClient = new WebClient();
            request.ContentType = "text/plain;charset=UTF-8";
            DateTime date = DateTime.Now;
            string package = string.Format("data:image/jpeg;base64,"+ imageString);

            var buffer = Encoding.ASCII.GetBytes(package);
            request.ContentLength = buffer.Length;
            var requestStream = request.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();

            var response = await request.GetResponseAsync();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}