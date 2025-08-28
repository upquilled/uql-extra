using UQLExtra.Params;
using System.IO;
using System.Threading.Tasks;
using System;

namespace UQLExtra
{
    public static class Hooks
    {
        public static void ApplyInit()
        {
            HookExcludeSteam();
        }

        private static void HookExcludeSteam()
        {
            On.RainWorldSteamManager.UploadWorkshopMod += (orig, self, mod, unlisted) =>
            {
                ExcludeSteam.PrepareUpload(mod, out string tempDir, out bool deleteAfter);
                bool result = orig(self, mod, unlisted);
                if (deleteAfter) Task.Run(async () =>
                    {
                        await Task.Delay(ExcludeSteam.deletionDelay);
                        try
                        {
                            if (Directory.Exists(tempDir))
                                Directory.Delete(tempDir, true);
                            UnityEngine.Debug.Log($"[{UQLExtra.info.Metadata.Name}] Temporary directory {tempDir} has been deleted.");
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogError($"[{UQLExtra.info.Metadata.Name}] Failed to delete temp directory {tempDir}: {ex.Message}");
                        }
                    });
                return result;
            };
        }
    }
}
