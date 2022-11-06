using System.Collections.Generic;
using AtO_Loader.Patches.DataLoader;
using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Globals), "CreateCardClones")]
public class DeserializeSubClasses
{
    [HarmonyPostfix]
    public static void LoadCharacterData(Dictionary<string, SubClassData> ____SubClass)
    {
        // sub classes are too annoying to not modify directly.
        _ = new SubClassDataLoader(____SubClass).LoadData();
    }
}