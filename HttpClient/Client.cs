using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
        public HttpContent LoadFile(string fileName)
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

       async public Task PostResource(string uri, HttpContent content)
        {
           HttpResponseMessage response = await client.PostAsync(uri, content);
        }
        

        async public Task<string> GetResource(string uri)
        {
            string result = "";
            HttpResponseMessage response = await client.GetAsync(uri);
            if(response.StatusCode  == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsStringAsync();
            }
            return result;
        }
    }
}
