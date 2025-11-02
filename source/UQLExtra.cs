using BepInEx;
using System.Security;
using BepInEx.Logging;

[module: UnverifiableCode]

namespace UQLExtra;

[BepInPlugin("uql.extra", "Extra Parameters", "1.0.10")]
public partial class UQLExtra : BaseUnityPlugin
{
    internal static ManualLogSource LoggerInstance;

    internal static PluginInfo info;

    private bool _initialized;

    public void OnEnable()
    {
        info = Info;
        LoggerInstance = Logger;
        On.RainWorld.OnModsInit += RainWorldOnModsInit;
        LoggerInstance.LogInfo("SHADOW UQLEXTRA MONEY GANG WE LOVE PREPARING YOUR PARAMETERS");
    }

    private void RainWorldOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        if (_initialized) return;

        _initialized = true;
        Hooks.ApplyInit();
    }
}