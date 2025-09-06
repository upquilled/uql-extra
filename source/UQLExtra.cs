using BepInEx;
using System.Security;
using System.Security.Permissions;
using BepInEx.Logging;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace UQLExtra;

[BepInPlugin("uql.extra", "Extra Parameters", "1.0.5")]
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