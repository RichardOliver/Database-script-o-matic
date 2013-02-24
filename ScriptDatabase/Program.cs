using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace ScriptDatabase
{
    class Program
    {
        static void Main(string[] args)
        {

            var databaseScripter = new DatabaseScripter("");
            databaseScripter.Script();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }



    }
}
