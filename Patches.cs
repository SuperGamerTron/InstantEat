using HarmonyLib;
using MuckMod.Patches;

namespace InstantEat.Patches
{
    public partial class MuckPatch
    {
        [HarmonyPrefix, HarmonyPatch(typeof(UseInventory), nameof(UseInventory.Use))]
        public static bool OnUse()
        {
            if (Eat.currentItem && Eat.configInstantEatEnabled.Value) return !(Eat.currentItem.tag == InventoryItem.ItemTag.Food);
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(UseInventory), nameof(UseInventory.SetWeapon))]
        public static void OnSetWeapon(ref InventoryItem item)
        {
            Eat.currentItem = item;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(GameManager), nameof(GameManager.started))]
        public void OnStartGame()
        {
            if (Eat.version != Eat.VERSION) ChatCommand.SendServerMessage("InstantEat has an update available");
        }
    }
}
