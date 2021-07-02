using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using InstantEat.Patches;
using System;
using System.Collections;
using System.Net;
using UnityEngine;

namespace InstantEat
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess("Muck.exe")]
    public class InstantEat : BaseUnityPlugin
    {
        public static ConfigEntry<bool> configInstantEatEnabled;

        public static InventoryItem currentItem;
        public static ParticleSystem.EmissionModule eatingEmission;

        public static string version = new WebClient().DownloadString(new Uri("https://raw.githubusercontent.com/SuperGamerTron/InstantEat/main/version.txt"));

        public const string
            GUID = "InstantEat",
            NAME = "InstantEat",
            VERSION = "1.0.0";

        public void Awake()
        {
            configInstantEatEnabled = Config.Bind("General", "Enabled", true, "Whether or not InstantEat is enabled");
            Harmony.CreateAndPatchAll(typeof(MuckPatch));
            Logger.LogMessage("Loaded InstantEat");
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentItem != null && !InventoryUI.Instance.gameObject.activeInHierarchy)
                {
                    if (configInstantEatEnabled.Value && currentItem.tag == InventoryItem.ItemTag.Food) FinishEating();
                }
            }
        }

        public void FinishEating()
        {
            UseInventory.Instance.eatSfx.Stop();
            StartCoroutine(Play(UseInventory.Instance.eatSfx, 0.45f));
            eatingEmission = UseInventory.Instance.eatingParticles.emission;
            eatingEmission.enabled = false;
            PlayerStatus.Instance.Eat(currentItem);
            ClientSend.AnimationUpdate(OnlinePlayer.SharedAnimation.Eat, false);
            Hotbar.Instance.UseItem(1);
        }

        public IEnumerator Play(AudioSource audioSource, float seconds)
        {
            audioSource.Play();
            yield return new WaitForSeconds(seconds);
            audioSource.Stop();
        }
    }
}
