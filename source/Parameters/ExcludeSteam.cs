using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Kittehface.Build.Json;
using System.Collections;
using UnityEngine;

namespace UQLExtra.Parameters
{
    public static class ExcludeSteam
    {
        public const float pollingDelay = 0.5f;
        public static void PrepareUpload(ModManager.Mod mod, out string tempDir, out bool deleteAfter)
        {
            string sourceDir = mod.path;
            tempDir = GetUniqueTempFolder(sourceDir);

            Directory.CreateDirectory(tempDir);

            (List<string>, List<string>) patterns = GetPatterns(mod);

            List<string> includePatterns = patterns.Item1;
            List<string> excludePatterns = patterns.Item2;

            foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string relative = RelativePath.GetRelativePath(sourceDir, file);
                if (includePatterns.Any(glob => RelativePath.MatchesGlob(relative, glob)) || !excludePatterns.Any(glob => RelativePath.MatchesGlob(relative, glob)))
                {
                    string dest = Path.Combine(tempDir, relative);
                    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                    File.Copy(file, dest);
                }
            }

            deleteAfter = false;
            bool keepDependency = false;

            // Modify modinfo.json if uql.extra contains only exclude_steam
            string modInfoPath = Path.Combine(tempDir, "modinfo.json");
            if (File.Exists(modInfoPath))
            {
                try
                {
                    string jsonText = RelativePath.FixRelaxedCommas(File.ReadAllText(modInfoPath));
                    var modInfo = JsonObject.FromJsonString(jsonText);

                    if (RelativePath.TryGetValue(modInfo, "uql.extra", out var extraToken) && extraToken is JsonObject extra)
                    {
                        if (extra["exclude_steam"] is JsonObject excludeSteam)
                        {
                            deleteAfter = (excludeSteam["delete_temp"] as bool?) ?? false;
                            keepDependency = (excludeSteam["keep_dependency"] as bool?) ?? false;
                        }

                        if (extra[0] == null || (extra[1] == null && extra.HasProperty("exclude_steam")))
                            removeDependency(modInfo, keepDependency, mod.id);


                        File.WriteAllText(modInfoPath, JsonObject.ToJsonString(modInfo));
                    }
                    else removeDependency(modInfo, keepDependency, mod.id);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning($"[{UQLExtra.info.Metadata.Name}] Failed to sanitize uql.extra from {modInfoPath}: {ex.Message}");
                }
            }

            mod.path = tempDir;
        }


        private static string GetUniqueTempFolder(string sourceDir)
        {
            string baseTemp = sourceDir + "_";
            string tempDir = baseTemp;

            while (Directory.Exists(tempDir))
            {
                tempDir += "_";
            }

            return tempDir;
        }

        private static (List<string>, List<string>) GetPatterns(ModManager.Mod mod)
        {
            List<string> exclude = new();
            List<string> include = new();

            void add(string pattern)
            {
                if (pattern.StartsWith("!")) {
                    if (pattern.Length > 1)
                        include.Add(pattern.Substring(1));
                } else if (pattern.Length > 0) exclude.Add(pattern);
            }

            string modInfoPath = Path.Combine(mod.path, "modinfo.json");
            if (!File.Exists(modInfoPath))
                return (include,exclude);

            try
            {
                string jsonText = RelativePath.FixRelaxedCommas(File.ReadAllText(modInfoPath));
                var modInfo = JsonObject.FromJsonString(jsonText);

                if (RelativePath.TryGetValue(modInfo, "uql.extra", out var extraToken))
                {
                    var extra = extraToken as JsonObject;
                    if (extra == null) return (include,exclude);

                    if (extra["exclude_steam"] is JsonObject excludeSteam && RelativePath.TryGetValue(excludeSteam, "globs", out var steamToken) || RelativePath.TryGetValue(extra, "exclude_steam", out steamToken))
                    {
                        if (steamToken is JsonArray arrayToken)
                        {
                            for (int i = 0; i < arrayToken.Length; i++)
                                if (arrayToken[i] is string pattern) exclude.Add(pattern);
                        }

                        else if (steamToken is string pattern)
                            add(pattern);
                    }

                    int totalCount = exclude.Count + include.Count;

                    if (totalCount > 0)
                    {
                        UnityEngine.Debug.Log($"[{UQLExtra.info.Metadata.Name}] Loaded {totalCount} steam exclude patterns for mod {mod.id}");
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"[{UQLExtra.info.Metadata.Name}] Failed to read exclude_steam from {modInfoPath}: {ex.Message}");
            }

            return (include,exclude);
        }

        internal static IEnumerator DeleteAfterUploadFinishes(RainWorldSteamManager manager, string tempDir)
        {
            while (manager.updateItemCallback.IsActive())
                yield return null;

            yield return new WaitForSeconds(pollingDelay);

            try
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                    UnityEngine.Debug.Log($"[{UQLExtra.info.Metadata.Name}] Temporary folder deleted: {tempDir}");
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[{UQLExtra.info.Metadata.Name}] Failed to delete temp folder: {ex.Message}");
            }
        }

        private static void removeDependency(JsonObject modInfo, bool keepDependency, string id)
        {
            if (!keepDependency)
            {
                if (RelativePath.TryGetValue(modInfo, "requirements", out var reqToken) && reqToken is JsonArray reqArray)
                {
                    JsonArray newArray = new JsonArray();
                    JsonArray newNameArray = new JsonArray();
                    int i;
                    int b = 0;
                    JsonArray? reqNameArray = modInfo["requirements_names"] as JsonArray;

                    for (i = 0; reqArray[i] != null; i++)
                    {
                        if (reqArray[i] as string == "uql.extra")
                        {
                            b++;
                            continue;
                        }
                        newArray.Add(reqArray[i]);
                        if (reqNameArray != null)
                            newNameArray.Add(reqNameArray[i]);
                    }
                    modInfo["requirements"] = newArray;
                    if (reqNameArray != null) modInfo["requirements_names"] = newNameArray;
                }

                UnityEngine.Debug.Log($"[{UQLExtra.info.Metadata.Name}] Removed uql.extra dependency from modinfo for {id} because no relevant parameters were found in modinfo,json");
            }
        }
    }
}
