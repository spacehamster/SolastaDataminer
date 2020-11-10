using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Dataminer
{
    internal class Exporter
    {
        internal static void Export()
        {
            Main.Log("Dumping database");
            if(!Directory.Exists("Dump"))
            {
                Directory.CreateDirectory("Dump");
            }
            var databases = (Dictionary<Type, object>)AccessTools.Field(typeof(DatabaseRepository), "databases")
                .GetValue(null);
            using (var sw = new StreamWriter("Dump/Types.txt"))
            {
                foreach(var type in databases.Keys)
                {
                    sw.WriteLine($"{type.FullName}");
                }
            }
            using (var sw = new StreamWriter("Dump/Assets.txt"))
            {
                sw.WriteLine("{0}\t{1}\t{2}",
                    "Name", "Type", "GUID");
                foreach (object db in databases.Values)
                {
                    var elements = (IList)AccessTools.Method(db.GetType(), "GetAllElements")
                        .Invoke(db, new object[] { });
                    foreach(BaseDefinition value in elements)
                    {
                        sw.WriteLine("{0}\t{1}\t{2}",
                            value.Name, value.GetType().FullName, value.GUID);
                    }
                }
            }

        }
    }
}