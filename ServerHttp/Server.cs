using System;
using System.Web.Http;
using System.Net;
using System.Threading;
using System.IO;
using System.Text;

namespace ServerHttp
{
    class Server
    {
        HttpServer server;
        HttpListener listener;
        int port;
        bool IsListening;
        string savePath;

        int fileCounter;

        public Server(string path, int port)
        {
            savePath = path;
            server = new HttpServer();
            listener = new HttpListener();
            this.port = port;
            fileCounter = 0;
            IsListening = false;
            Thread threadListen = new Thread(Listen);
            threadListen.IsBackground = true;
            threadListen.Start();
        }

        bool CreateFile(string fileName, byte[] content, int offset = 0)
        {
            bool result = true;
            FileStream fout;
            try
            {
                fout = new FileStream( savePath + "\\" + fileName, FileMode.Create);
            }
            catch
            {
                return false;
            }
            try
            {
                fout.Write(content, offset, content.Length - offset);
            }
            catch
            {
                result = false;
            }
            finally
            {
                fout.Close();
            }
            return result;
        }

        string GetFileName(byte[] buffer)
        {
            int i;
            for (i = 0; i < buffer.Length && buffer[i] != 0; i++)
                ;
            if (i != buffer.Length)
                return Encoding.ASCII.GetString(buffer, 0, i);
            else
                return "Unknown";
        }

        void HandlePostRequest(HttpListenerContext context)
        {
            Stream input = context.Request.InputStream;
            byte[] buffer = new byte[context.Request.ContentLength64];
            input.Read(buffer, 0, (int)context.Request.ContentLength64);
            string fileName = GetFileName(buffer);
            CreateFile(fileName, buffer, fileName.Length + 1);
            Console.WriteLine(Encoding.ASCII.GetString(buffer));
            input.Close();
            Console.WriteLine("Handle post");
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.OutputStream.Write(Encoding.ASCII.
                GetBytes(fileCounter.ToString()), 0, fileCounter.ToString().Length);
            context.Response.OutputStream.Close();
            Console.WriteLine((int)context.Request.ContentLength64);
        }

        byte[] GetFileContent(string fileName)
        {
            byte[] buffer = null;
            FileStream fin;
            try
            {
                fin = new FileStream(fileName, FileMode.Open);
            }
            catch
            {
                return null;
            }
            try
            {
                buffer = new byte[fin.Length];
                fin.Read(buffer, 0, (int)fin.Length);
            }
            catch
            {
                buffer = null;
            }
            finally
            {
                fin.Close();
            }
            return buffer;
        }

        void HandleGetRequest(HttpListenerContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            Console.WriteLine(context.Request.Url.LocalPath);
            byte[] buffer = GetFileContent(Path.GetFileName(context.Request.Url.LocalPath));
            if (buffer != null)
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            else
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.OutputStream.Close();
            Console.WriteLine("Handle get");
        }

        void HandleRequest(HttpListenerContext context)
        {
            string method = context.Request.HttpMethod;
            Console.WriteLine(method);
            if(method == "GET")
            {
                HandleGetRequest(context);
            } 
            else if(method == "POST")
            {
                HandlePostRequest(context);
            }
        }

        public void Listen()
        {
            listener.Prefixes.Add("http://*:" + port.ToString() + "/");
            listener.Start();
            IsListening = true;
            while(IsListening)
            { 
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    HandleRequest(context);
                }
                catch
                {

                }
            }
        }
    }
}
