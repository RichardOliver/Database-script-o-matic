using System;
using System.Collections;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

namespace ScriptDatabase
{
    public class DatabaseScripter
    {
        private readonly Server _server;
        private Scripter _scripter;
        private string _basePath;
        public  DatabaseScripter(string name)
        {
            _server = new Server(name);
            _basePath = @"C:\temp\scriptDatabase\";
            GenerateScripter();

 
        }

        private void GenerateScripter()
        {
            _scripter = new Scripter(_server)
                {
                    Options = { 
                                ScriptDrops = false, 
                                ScriptData = false,
                                AllowSystemObjects = false,
                                WithDependencies = false, 
                                Indexes = true, 
                                DriAllConstraints = true
                              }
                }; // Define a Scripter object and set the required scripting options. 
        }

        public void Script()
        {

            Directory.CreateDirectory(_basePath);
            foreach (Database database in _server.Databases)
            {
                ScriptDatabase(database);
            }
        }

            


        private void ScriptDatabase(Database database)
        {
            Console.WriteLine(database.Name);
            var databaseScriptDirectory = Path.Combine(_basePath, database.Name);
            Directory.CreateDirectory(databaseScriptDirectory);

            ScriptDatabaseObjects<Schema>(database.Schemas, "Schemas", databaseScriptDirectory);
            ScriptDatabaseObjects<Table>(database.Tables, "Tables", databaseScriptDirectory);
            ScriptDatabaseObjects<StoredProcedure>(database.StoredProcedures, "StoredProcedures", databaseScriptDirectory);
            ScriptDatabaseObjects<UserDefinedFunction>(database.UserDefinedFunctions, "UserDefinedFunctions", databaseScriptDirectory);
            ScriptDatabaseObjects<View>(database.Views, "Views", databaseScriptDirectory);
        }

        private void ScriptDatabaseObjects<T>(IEnumerable objects, string type, string databasePath) where T : ScriptNameObjectBase
        {
            var scriptDirectory = Path.Combine(databasePath, type);
            Directory.CreateDirectory(scriptDirectory);

            foreach (T obj in objects)
            {
                Console.WriteLine("scripting {0}: {1}", type, obj.Name);

                var fileName = Path.Combine(scriptDirectory, obj.Name + ".sql");
                var script = _scripter.Script(new Urn[] { obj.Urn });
                File.WriteAllLines(fileName, script.Cast<string>());
            }              
        }
    }
}
