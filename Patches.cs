using HarmonyLib;
using UnityEngine;

namespace InstantEat.Patches
{
    public class MuckPatch
    {
        [HarmonyPrefix, HarmonyPatch(typeof(UseInventory), nameof(UseInventory.Use))]
        public static bool OnUse()
        {
            if (InstantEat.currentItem && InstantEat.configInstantEatEnabled.Value) return !(InstantEat.currentItem.tag == InventoryItem.ItemTag.Food);
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(UseInventory), nameof(UseInventory.SetWeapon))]
        public static void OnSetWeapon(ref InventoryItem item)
        {
            InstantEat.currentItem = item;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(ChatBox), nameof(ChatBox.Instance.ChatCommand))]
        public static void OnChatCommand(ref string message)
        {
            if (message == "/instanteat")
            {
               InstantEat.configInstantEatEnabled.Value = !InstantEat.configInstantEatEnabled.Value;
                SendServerMessage($"Instant Eat {(InstantEat.configInstantEatEnabled.Value ? "Enabled" : "Disabled")}");
            }
        }

        public static void SendServerMessage(string message)
        {
            ChatBox instance = ChatBox.Instance;
            instance.typing = false;
            message = TrimMessage(message);
            if (message == "")
            {
                return;
            }
            if (message.ToCharArray()[0] == '/')
            {
                instance.ChatCommand(message);
                return;
            }
            foreach (string input in instance.profanityList.text.Split(new char[] { '\n' }))
            {
                message = message.Replace(input, "muck");
            }
            instance.AppendMessage(0, message, "Server");
            ClientSend.SendChatMessage(message);
            ClearMessage();
        }

        private static void ClearMessage()
        {
            ChatBox.Instance.inputField.text = "";
            ChatBox.Instance.inputField.interactable = false;
        }

        public static string TrimMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return "";
            }
            return message.Substring(0, Mathf.Min(message.Length, 120));
        }
    }
}
