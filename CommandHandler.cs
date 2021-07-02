using MuckMod;
using MuckMod.Patches;

namespace InstantEat
{
    public class CommandHandler
    {
        [MuckCommand]
        public static void InstantEat(string[] args)
        {
            Eat.configInstantEatEnabled.Value = !Eat.configInstantEatEnabled.Value;
            ChatCommand.SendServerMessage($"Instant Eat {(Eat.configInstantEatEnabled.Value ? "Enabled" : "Disabled")}");
        }
    }
}