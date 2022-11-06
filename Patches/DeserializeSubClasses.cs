using System.Collections.Generic;
using AtO_Loader.Patches.DataLoader;
using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(Globals), "CreateCardClones")]
public class DeserializeSubClasses
{
    [HarmonyPostfix]
    public static void LoadCharacterData(Dictionary<string, SubClassData> ____SubClassSource)
    {
        var subClassDatas = new SubClassDataLoader(____SubClassSource).LoadData();
        foreach (var subClassData in subClassDatas)
        {
            ____SubClassSource[subClassData.Key] = subClassData.Value;
        }
    }
}