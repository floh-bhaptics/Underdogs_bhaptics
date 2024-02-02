using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using Il2Cpp;

[assembly: MelonInfo(typeof(Underdogs_bhaptics.Underdogs_bhaptics), "Underdogs_bhaptics", "1.1.0", "Florian Fahrenberger")]
[assembly: MelonGame("One Hamsa", "UNDERDOGS")]

namespace Underdogs_bhaptics
{
    public class Underdogs_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr = null!;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
        }

        [HarmonyPatch(typeof(PlayerMech), "ShakeCockpit", new Type[] { typeof(float), typeof(float) })]
        public class bhaptics_ShakeCockpit
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerMech __instance)
            {
                tactsuitVr.PlaybackHaptics("MechDamage");
            }
        }

        [HarmonyPatch(typeof(PlayerMech), "DetachLeft", new Type[] {  })]
        public class bhaptics_DetachLeft
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerMech __instance)
            {
                tactsuitVr.PlaybackHaptics("DetachArm_L");
            }
        }

        [HarmonyPatch(typeof(PlayerMech), "DetachRight", new Type[] { })]
        public class bhaptics_DetachRight
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerMech __instance)
            {
                tactsuitVr.PlaybackHaptics("DetachArm_R");
            }
        }

        [HarmonyPatch(typeof(PlayerMech), "KillPlayerMech", new Type[] { })]
        public class bhaptics_KillPlayerMech
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerMech __instance)
            {
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(PlayerPunchBehavior), "OnPunchResolved", new Type[] { typeof(EntityInteraction) } )]
        public class bhaptics_OnPunchResolved
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerPunchBehavior __instance, EntityInteraction punchInteraction)
            {
                if (!punchInteraction.hasImpact) return;
                if (__instance.mechArm.side == VRSide.Right) tactsuitVr.PlaybackHaptics("Punch_R");
                else tactsuitVr.PlaybackHaptics("Punch_L");
            }
        }

        [HarmonyPatch(typeof(BashGuardEntity), "OnBashResolved", new Type[] { typeof(EntityInteraction) })]
        public class bhaptics_OnBashResolved
        {
            [HarmonyPostfix]
            public static void Postfix(BashGuardEntity __instance, EntityInteraction bashInteraction)
            {
                if (bashInteraction.totalHealthDamage > 0f) tactsuitVr.PlaybackHaptics("Bash");
                if (bashInteraction.hasImpact) tactsuitVr.PlaybackHaptics("Bash");
                if (bashInteraction.result.totalImpact > 0f) tactsuitVr.PlaybackHaptics("Bash");
            }
        }

        [HarmonyPatch(typeof(PlayerMech), "OnChassisHealthChange", new Type[] { typeof(IHealthComponent), typeof(int), typeof(IEntityBehavior) })]
        public class bhaptics_HealthChanged
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerMech __instance, IHealthComponent healthcomponent)
            {
                if (healthcomponent.health <= healthcomponent.maxHealth * 0.25f) tactsuitVr.StartHeartBeat();
                else tactsuitVr.StopHeartBeat();
                if (healthcomponent.health == 0) tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(Bomb), "Explode", new Type[] { })]
        public class bhaptics_BombBExplode
        {
            [HarmonyPostfix]
            public static void Postfix(Bomb __instance)
            {
                tactsuitVr.PlaybackHaptics("Explosion");
            }
        }


    }
}