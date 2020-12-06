using HarmonyLib;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dataminer
{
    internal class Exporter
    {
        internal static void Export()
        {
            Main.Log("Dumping database");
            if (!Directory.Exists("Dump"))
            {
                Directory.CreateDirectory("Dump");
            }
            var databases = (Dictionary<Type, object>)AccessTools
                .Field(typeof(DatabaseRepository), "databases")
                .GetValue(null);
            using (var sw = new StreamWriter("Dump/Types.txt"))
            {
                foreach (var type in databases.Keys)
                {
                    sw.WriteLine($"{type.FullName}");
                }
            }
            using (var sw = new StreamWriter("Dump/Assets.txt"))
            {
                sw.WriteLine("{0}\t{1}\t{2}\t{3}",
                    "Name", "Type", "DatabaseType", "GUID");
                foreach (IEnumerable<BaseDefinition> db in databases.Values)
                {
                    Type dbType = db.GetType().GetGenericArguments()[0];
                    foreach (BaseDefinition value in db)
                    {
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}",
                            value.Name, value.GetType().FullName, dbType.FullName, value.GUID);
                    }
                }
            }
            foreach (IEnumerable<BaseDefinition> db in databases.Values)
            {
                Type dbType = db.GetType().GetGenericArguments()[0];
                bool hasSubtypes = db.Any(x => x.GetType() != dbType);
                foreach (BaseDefinition value in db)
                {
                    var subfolder = value.GetType().Name;
                    if (hasSubtypes) subfolder = $"{dbType.FullName}/{subfolder}";
                    JsonUtil.Dump(value, $"Dump/{subfolder}/{value.name}.{value.GUID}.json");
                }
            }
        }
        internal static void ExportStrings()
        {
            if (!Directory.Exists("Dump"))
            {
                Directory.CreateDirectory("Dump");
            }
            LocalizationManager.InitializeIfNeeded();
            int i = 0;
            foreach(var source in LocalizationManager.Sources)
            {
                var languageIndex = source.GetLanguageIndex(LocalizationManager.CurrentLanguage);
                using (var sw = new StreamWriter($"Dump/LanguageSource{i}_Terms.txt"))
                {
                    foreach (var term in source.mTerms)
                    {
                        sw.WriteLine("{0}\t{1}\t{2}",
                            term.Term,
                            term.TermType,
                            languageIndex < term.Languages.Length ?
                                term.Languages[languageIndex].Replace("\n", @"\n") : "NULL");
                    }
                }
                using (var sw = new StreamWriter($"Dump/LanguageSource{i}_Categories.txt"))
                {
                    var mainCategories = source.GetCategories(OnlyMainCategory: true);
                    var secondaryCategories = source.GetCategories(OnlyMainCategory: false)
                        .Where(c => !mainCategories.Contains(c));
                    foreach(var category in mainCategories)
                    {
                        sw.WriteLine($"{category}\tMain");
                    }
                    foreach (var category in secondaryCategories)
                    {
                        sw.WriteLine($"{category}\tSecondary");
                    }
                }
                i++;
            }
        }
    }
}