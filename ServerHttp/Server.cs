using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Http;

namespace ServerHttp
{
    class Server
    {
        HttpServer server;
        HttpListener listener;

        const int MaxClientAmount = 10;

        int port;
        bool IsListening;
        string savePath;

        int fileCounter;

        delegate void RequestHandler(HttpListenerContext context);

        Dictionary<string, RequestHandler> requestHandlers;

        public Server(string path, int port)
        {
            savePath = path;
            requestHandlers = new Dictionary<string, RequestHandler>();
            requestHandlers.Add("GET", HandleGetRequest);
            requestHandlers.Add("POST", HandlePostRequest);
            requestHandlers.Add("DELETE", HandleDeleteRequest);
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
                fout = new FileStream(savePath + "\\" + fileName, FileMode.Create);
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
            if (File.Exists(fileName))
            {
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
            else
            {
        //        context.Response.StatusCode = (int)HttpStatusCode.
            }
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

        long GetFileSize(string fileName)
        {
            long length = 0;
            FileStream fin = null;
            try
            {
                fin = new FileStream(fileName, FileMode.Open);
            }
            catch
            {
                return 0;
            }
            try
            {
                length = fin.Length;
            }
            catch
            {
                length = 0;
            }
            finally
            {
                fin.Close();
            }
            return length;
        }

        void HandleGetRequest(HttpListenerContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            Console.WriteLine(context.Request.Url.LocalPath);
            byte[] buffer = null;
            if (context.Request.Headers.Get("info") != null)
            {
                string info = context.Request.Headers.Get("info");
                Console.WriteLine("Handle info");
                context.Response.AddHeader("info", GetFileSize(Path.
                    GetFileName(context.Request.Url.LocalPath)).ToString());
                context.Response.AppendHeader("info", context.Request.Url.LocalPath);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                buffer = GetFileContent(Path.GetFileName(context.Request.Url.LocalPath));
                if (buffer != null)
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                else
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            context.Response.OutputStream.Close();
            Console.WriteLine("Handle get");
        }

        void HandleDeleteRequest(HttpListenerContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            string fileName = Path.GetFileName(context.Request.Url.LocalPath);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            context.Response.OutputStream.Close();
            Console.WriteLine("Handle delete");
        }

        void HandleRequest(HttpListenerContext context)
        {
            string method = context.Request.HttpMethod;
            Console.WriteLine(method);
            if(requestHandlers.ContainsKey(method))
            {
                requestHandlers[method](context);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }


        public void Listen()
        {
            listener.Prefixes.Add("http://*:" + port.ToString() + "/");
            listener.Start();
            IsListening = true;
            while (IsListening)
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
