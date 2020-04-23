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

        async public Task DeleteResource(string uri)
        {
            HttpResponseMessage response = await client.DeleteAsync(uri);
            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException();
            }
        }

        async public Task<int> PostResource(string fileName, HttpContent content)
        {
            int result;
            HttpResponseMessage response = await client.PostAsync(fileName, content);
            try
            {
                string fileID = await response.Content.ReadAsStringAsync();
                result = int.Parse(fileID);
            }
            catch
            {
                result = -1;
            }
            return result;
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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    result = null;
                    //throw new FileNotFoundException();
                }
            }
            return result;
        }
    }
}
