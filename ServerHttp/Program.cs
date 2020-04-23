using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerHttp
{
    class Program
    {
        static void Main(string[] args)
        {
            //   SimpleHTTPServer server = new SimpleHTTPServer("C:/www/ProgSite", 8007);
            //FileStream fout = new FileStream("E:\\");
            Server server = new Server("E:/LoadedFiles", 8009);
            Console.ReadLine();
        }
    }
}
