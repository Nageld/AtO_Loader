﻿using HarmonyLib;

namespace AtO_Loader.Patches;

[HarmonyPatch(typeof(SteamManager), "SetScore")]
public class SetScore
{
    [HarmonyPrefix]
    static bool SetPatch()
    {
        return false;
    }
}