﻿using System;
using System.Diagnostics;
using BepInEx.Logging;
using DLCLib;
using DLCLib.Campaigns;
using DLCLib.DLC;
using DLCLib.HUD;
using DLCQuestipelago.Archipelago;
using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace DLCQuestipelago.Items
{
    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch(nameof(Inventory.Coins), MethodType.Getter)]
    public static class InventoryCoinsGetPatch
    {
        private static ManualLogSource _log;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(ManualLogSource log, ArchipelagoClient archipelago)
        {
            _log = log;
            _archipelago = archipelago;
        }

        //public override void OnPickup(Player player)
        private static bool Prefix(Inventory __instance, ref int __result)
        {
            try
            {
                if (!Plugin.Instance.IsInGame || _archipelago == null)
                {
                    return true; // run original logic;
                }

                if (_archipelago.SlotData.Coinsanity == Coinsanity.None)
                {
                    return true; // run original logic
                }

                var currentCoins = CoinsanityUtils.GetCurrentCoins(_archipelago);
                __result = (int)Math.Floor(currentCoins); // + 4;
                // _coinDisplay.HandleCoinChanged(currentCoins);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _log.LogError($"Failed in {nameof(InventoryCoinsGetPatch)}.{nameof(Prefix)}:\n\t{ex}");
                Debugger.Break();
                return true; // run original logic
            }
        }
    }
}
