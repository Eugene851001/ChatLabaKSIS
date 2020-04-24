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
            Server server = new Server("E:/LoadedFiles", 8009);
            Console.ReadLine();
        }
    }
}
