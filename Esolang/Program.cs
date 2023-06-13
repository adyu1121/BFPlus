using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace Esolang
{
    

    class MainClass
    {
        
        public static void Main(string[] argv)
        {
            StreamReader sr = new StreamReader(@"./Code.txt");
            Interpreter interpreter = new Interpreter(sr);
            interpreter.RunCode();
        }
    }
    
}
    
