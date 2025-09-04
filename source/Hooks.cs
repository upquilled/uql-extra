using UQLExtra.Parameters;
using System.IO;
using Steamworks;
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
                if (deleteAfter)
                    try
                    {
                        RelativePath.CoroutineRunner.Instance.StartCoroutine(ExcludeSteam.DeleteAfterUploadFinishes(self, tempDir));
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"[{UQLExtra.info.Metadata.Name}] Failed to start cleanup coroutine for {tempDir}: {ex}");
                    }


                return result;
            };
        }
    }
}
