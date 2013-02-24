using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if (database.Name == "AdventureWorks2008")
                {
                    ScriptDatabase(database);
                }
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
                if (IsSystemObject(obj)) continue;
                Console.WriteLine("scripting {0}: {1}", type, obj.Name);

                var fileName = Path.Combine(scriptDirectory, obj.Name + ".sql");
                var script = _scripter.Script(new Urn[] { obj.Urn });
                File.WriteAllLines(fileName, script.Cast<string>());
            }              
        }

        public static bool IsSystemObject(object objectToCheck)
        {
            const string propertyName = "IsSystemObject";
            var type = objectToCheck.GetType();

            var isSystemObject = type.GetProperty(propertyName);

            if (isSystemObject == null) return false;

            return isSystemObject.GetValue(objectToCheck) as bool? == true;
        }
    }
}
