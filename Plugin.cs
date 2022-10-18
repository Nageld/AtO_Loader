using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace AtO_Loader;

[BepInPlugin(modGUID, modName, ModVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string modGUID = "Book.CardLoader";
    private const string modName = "AtO_Loader";
    private const string ModVersion = "0.0.0.1";
    private readonly Harmony harmony = new Harmony(modGUID);

    private void Awake()
    {
        harmony.PatchAll(typeof(create));
        // harmony.PatchAll(typeof(Unlocked));
        harmony.PatchAll(typeof(KeyNotePrint));
    }

    // [HarmonyPatch(typeof(PlayerManager), "IsCardUnlocked")]
    // class Unlocked
    // {
    //   [HarmonyPrefix]
    //   static bool setpatch(ref bool  __result)
    //   {
    //     __result = true;
    //     return false;
    //   }
    // }


    [HarmonyPatch(typeof(Globals), "GetKeyNotesData")]
    class KeyNotePrint
    {
        [HarmonyPrefix]
        static void setpatch(ref string id)
        {
            if (id == null)
            {
                id = "vanish";
                System.Console.WriteLine($"Property set incorrectly on a card");
            }
        }
    }

    [HarmonyPatch(typeof(Globals), "CreateCardClones")]
    class create
    {
        [HarmonyPrefix]
        static void setpatch(Dictionary<string, CardData> ____CardsSource, ref string ___cardsText)
        {
            foreach (string fileName in Directory.GetFiles(@"BepInEx\plugins\cards\", "*.json"))
            {
                string[] fileLines = File.ReadAllLines(fileName);
                string data = String.Concat(fileLines);
                CardData newcard = ScriptableObject.CreateInstance<CardData>();
                JsonUtility.FromJsonOverwrite(data, newcard);
                newcard.Id = newcard.Id.ToLower();
                ____CardsSource.Add(newcard.Id, newcard);
                ___cardsText = string.Concat(new string[]
                {
                    ___cardsText,
                    "c_",
                    newcard.Id,
                    "_name=",
                    Functions.NormalizeTextForArchive(newcard.CardName),
                    "\n"
                });
            }
        }
    }
}