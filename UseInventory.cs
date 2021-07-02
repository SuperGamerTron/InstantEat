using HarmonyLib;

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
    }
}
