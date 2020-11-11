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
                foreach (IEnumerable<BaseDefinition> db in databases.Values)
                {
                    foreach (BaseDefinition value in db)
                    {
                        sw.WriteLine("{0}\t{1}\t{2}",
                            value.Name, value.GetType().FullName, value.GUID);
                    }
                }
            }
            foreach (IEnumerable<BaseDefinition> db in databases.Values)
            {
                foreach (BaseDefinition value in db)
                {
                    JsonUtil.Dump(value, $"Dump/{value.GetType().Name}/{value.name}.{value.GUID}.json");
                }
            }
        }
    }
}