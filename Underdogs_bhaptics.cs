using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using Il2Cpp;

[assembly: MelonInfo(typeof(Underdogs_bhaptics.Underdogs_bhaptics), "Underdogs_bhaptics", "1.0.0", "Florian Fahrenberger")]
[assembly: MelonGame("One Hamsa", "UNDERDOGS")]

namespace Underdogs_bhaptics
{
    public class Underdogs_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr = null!;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        [HarmonyPatch(typeof(PlayerMech), "ShakeCockpit", new Type[] { typeof(float), typeof(float) })]
        public class bhaptics_ShakeCockpit
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerMech __instance)
            {
                if (!__instance.hasHealth) return;
                tactsuitVr.LOG("ShakeCockpit");
                tactsuitVr.PlaybackHaptics("BellyRumble");
            }
        }

        [HarmonyPatch(typeof(PlayerMech), "DetachLeft", new Type[] {  })]
        public class bhaptics_DetachLeft
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerMech __instance)
            {
                if (!__instance.hasHealth) return;
                tactsuitVr.LOG("DetachLeft");
                tactsuitVr.PlaybackHaptics("Recoil_L");
            }
        }

        [HarmonyPatch(typeof(PlayerMech), "DetachRight", new Type[] { })]
        public class bhaptics_DetachRight
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerMech __instance)
            {
                if (!__instance.hasHealth) return;
                tactsuitVr.LOG("DetachRight");
                tactsuitVr.PlaybackHaptics("Recoil_R");
            }
        }

        [HarmonyPatch(typeof(PlayerMech), "KillPlayerMech", new Type[] { })]
        public class bhaptics_KillPlayerMech
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerMech __instance)
            {
                tactsuitVr.LOG("KillMech");
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(PlayerMechHealth), "onCockpitDamage", new Type[] { })]
        public class bhaptics_onCockpitDamage
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerMechHealth __instance)
            {
                tactsuitVr.LOG("CockpitDamage");
                tactsuitVr.PlaybackHaptics("BellyRumble");
            }
        }

        [HarmonyPatch(typeof(PlayerPunchBehavior), "add_OnPunchSucceeded")]
        public class bhaptics_add_OnPunchSucceeded
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerPunchBehavior __instance)
            {
                tactsuitVr.LOG("PunchSucceeded");
                bool isRight = (__instance.mechArm.side == VRSide.Right);
                tactsuitVr.Recoil("Punch", isRight);
            }
        }


    }
}