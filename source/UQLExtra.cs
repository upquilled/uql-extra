using BepInEx;
using System.Security;
using BepInEx.Logging;
using System;

[module: UnverifiableCode]

namespace UQLExtra;

[BepInPlugin("uql.extra", "Extra Parameters", "1.0.15")]
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

    public static void LInfo(string message)
    {
        UnityEngine.Debug.Log($"[Info   :{info.Metadata.Name}] " + message);
    }

    public static void LWarn(string message, Exception ex)
    {
        UnityEngine.Debug.LogWarning($"[Warning:{info.Metadata.Name}] {message}: {ex}");
    }

    public static void LWarn(string message)
    {
        UnityEngine.Debug.LogWarning($"[Warning:{info.Metadata.Name}] {message}");
    }

    public static void LError(string message, Exception ex)
    {
        UnityEngine.Debug.LogError($"[Error  :{info.Metadata.Name}] {message}: {ex}");
    }

    public static void LError(string message)
    {
        UnityEngine.Debug.LogError($"[Error  :{info.Metadata.Name}] {message}");
    }
}