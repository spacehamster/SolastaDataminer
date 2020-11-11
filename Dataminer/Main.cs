using System;
using UnityEngine;
using UnityModManagerNet;

namespace Dataminer
{
#if DEBUG
    [EnableReloading]
#endif
    static class Main
    {
        static UnityModManager.ModEntry ModEntry;
        public static void Log(string msg)
        {
            ModEntry?.Logger.Log(msg);
        }
        static void Load(UnityModManager.ModEntry modEntry)
        {
            ModEntry = modEntry;
            modEntry.OnGUI = OnGUI;
#if DEBUG
            modEntry.OnUnload = Unload;
#endif
        }
#if DEBUG
        static bool Unload(UnityModManager.ModEntry modEntry)
        {
            return true;
        }
#endif
        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (GUILayout.Button("Export Database"))
            {
                Exporter.Export();
            }
#if DEBUG
            if (GUILayout.Button("Test Export"))
            {
                JsonUtil.Dump(DatabaseRepository.GetDatabase<CharacterClassDefinition>().GetElement("Rogue"),
                    "Dump/RogueTest.json");
            }
#endif
        }
    }
}