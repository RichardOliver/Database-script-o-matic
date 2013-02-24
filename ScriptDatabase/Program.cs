using System;

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
