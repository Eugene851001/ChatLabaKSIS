using SerializeHandler;
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
        HttpListener listener;

        int port;
        bool IsListening;

        FilesHandler filesHandler;

        delegate void RequestHandler(HttpListenerContext context);

        Dictionary<string, RequestHandler> requestHandlers;

        public Server(string path, int port)
        {
            filesHandler = new FilesHandler(new SerializerXml(), path);
            if (File.Exists("filesTable.txt"))
                filesHandler.LoadInfo("filesTable.txt");
            requestHandlers = new Dictionary<string, RequestHandler>();
            requestHandlers.Add("GET", HandleGetRequest);
            requestHandlers.Add("POST", HandlePostRequest);
            requestHandlers.Add("DELETE", HandleDeleteRequest);
            listener = new HttpListener();
            this.port = port;
            IsListening = false;
            Thread threadListen = new Thread(Listen);
            threadListen.IsBackground = true;
            threadListen.Start();
        }

        void HandlePostRequest(HttpListenerContext context)
        {
            Stream input = context.Request.InputStream;
            byte[] buffer = new byte[context.Request.ContentLength64];
            input.Read(buffer, 0, (int)context.Request.ContentLength64);
            string fileName = Path.GetFileName(context.Request.Url.LocalPath);
            Console.WriteLine("Post file name: " + fileName);
            int id;
            if ((id = filesHandler.AddFile(fileName, buffer)) == -1)
            {
                context.Response.OutputStream.Write(Encoding.ASCII.GetBytes(id.ToString()), 0, 
                    Encoding.ASCII.GetBytes(id.ToString()).Length);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                id = filesHandler.GetFileID(fileName);
                Console.WriteLine("Files id: " + id.ToString());
                context.Response.OutputStream.Write(Encoding.ASCII.GetBytes(id.ToString()), 0, 
                    Encoding.ASCII.GetBytes(id.ToString()).Length);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                filesHandler.SaveInfo("filesTable.txt");
            }
             context.Response.OutputStream.Close();
        }



        void HandleGetRequest(HttpListenerContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            
            //     string path = savePath + Path.GetFileName(context.Request.Url.LocalPath);
            int fileID = 0;
            try
            {
                fileID = int.Parse(Path.GetFileName(context.Request.Url.LocalPath));
            }
            catch
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.OutputStream.Close();
                return;
            }
            Console.WriteLine(fileID);
            if (context.Request.Headers.Get("info") != null)
            {
                string info = context.Request.Headers.Get("info");
                Console.WriteLine("Handle info");
                context.Response.AddHeader("info", filesHandler.GetFileSize(fileID).ToString());
                context.Response.AppendHeader("info", filesHandler.GetUserFileName(fileID));
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("Try to load content...");
                byte[] buffer = filesHandler.GetFileContent(fileID);
                if (buffer != null)
                {
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    Console.WriteLine("Content for response loaded");
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    Console.WriteLine("Content not found");
                }
            }
            context.Response.OutputStream.Close();
        }

        int getFileIDFromRequest(HttpListenerContext context)
        {
            int result = 0;
            try
            {
                result = int.Parse(Path.GetFileName(context.Request.Url.LocalPath));
            }
            catch
            {
                result = -1;
            }
            return result;
        }

        void HandleDeleteRequest(HttpListenerContext context)
        {
            Console.WriteLine("Handle delete");
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            int id = getFileIDFromRequest(context);
            if(id == -1)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.OutputStream.Close();
                return;
            }
            try
            {
                filesHandler.DeleteFile(id);
                filesHandler.SaveInfo("filesTable.txt");
            }
            catch(FileNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            context.Response.OutputStream.Close();
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
