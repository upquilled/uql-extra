using UQLExtra.Parameters;
using System;
using System.IO;
using System.Reflection;

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
                ExcludeSteam.PrepareUpload(mod, out string tempDir, out bool dryRun);

                if (!dryRun)
                {
                    bool result = orig(self, mod, unlisted);

                    try
                    {
                        RelativePath.CoroutineRunner.Instance.StartCoroutine(ExcludeSteam.DeleteAfterUploadFinishes(self, tempDir));
                    }
                    catch (Exception ex)
                    {
                        UQLExtra.LError($"Failed to start cleanup coroutine for {tempDir}", ex);
                    }
                    return result;
                }
                var flags = BindingFlags.Instance
                          | BindingFlags.Public
                          | BindingFlags.NonPublic;

                var initUpload = typeof(RainWorldSteamManager).GetMethod("InitUpload", flags);
                var isCurrentlyUploading = typeof(RainWorldSteamManager).GetField("isCurrentlyUploading", flags);
                var lastUploadFail = typeof(RainWorldSteamManager).GetField("lastUploadFail", flags);
                
                if (!(bool)initUpload.Invoke(self, null)) return false;
                isCurrentlyUploading.SetValue(self, false);
                lastUploadFail.SetValue(self, $"exclude_steam debug directory has been created at {tempDir.Replace("\\", "/")}");
                return false;
            };
        }
    }
}
