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
        static StreamReader sr;
        public static void Main(string[] argv)
        {
            using (FileStream fs = new FileStream(@"./Code.txt", FileMode.Open, FileAccess.Read))
            {
                Interpreter interpreter = new Interpreter(fs);
                interpreter.RunCode();
            }
            


        }
    }
    
}
    
