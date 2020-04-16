using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;

namespace ClientHttp
{
    public class Client
    {
        HttpClient client;

        public Client()
        {
            client = new HttpClient();
        }

        string GetBaseName(string fileName)
        {
            return Path.GetFileName(fileName);
        }
        public HttpContent LoadFile(string fileName, long MaxFileSize = long.MaxValue)
        {
            ByteArrayContent content = null;
            FileStream fin;
            try
            {
                fin = new FileStream(fileName, FileMode.Open);
            }
            catch
            {
                return null;
            }
            int length = (int)fin.Length;
            fileName = GetBaseName(fileName);
            byte[] buffer = new byte[length + fileName.Length + 1];
            Encoding.ASCII.GetBytes(fileName).CopyTo(buffer, 0);
            buffer[fileName.Length] = 0;
            try
            {
                fin.Read(buffer, fileName.Length + 1, length);
                content = new ByteArrayContent(buffer);
            }
            catch
            {
                content = null;
            }
            finally
            {
                fin.Close();
            }
            return content;
        }

        public HttpContent GetContent(string source)
        {
            StringContent httpContent = new StringContent(source);
            return httpContent;
        }


        async public Task DeleteResource(string uri)
        {
            HttpResponseMessage response = await client.DeleteAsync(uri);
            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException();
            }
        }

        async public Task PostResource(string uri, HttpContent content)
        {
           HttpResponseMessage response = await client.PostAsync(uri, content);
        }

        async public Task<Dictionary<string, string>> GetResourceInf(string uri)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("info", "size");
            request.Headers.Add("info", "name");
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string[] headers = (string[])response.Headers.GetValues("info");
                result.Add("size", headers[0]);
                result.Add("name", headers[1]);
            }
            else
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new FileNotFoundException();    
            return result;
        }
         
        async public Task<byte[]> GetResource(string uri)
        {
            byte[] result = null;
            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new FileNotFoundException();
                }
               
            }
            return result;
        }
    }
}
