using System;
using System.IO;
using System.Text.RegularExpressions;
using Kittehface.Build.Json;
using UnityEngine;

namespace UQLExtra;

public static class RelativePath
{
    public static string GetRelativePath(string basePath, string fullPath)
    {
        Uri baseUri = new Uri(AppendDirectorySeparatorChar(basePath));
        Uri fullUri = new Uri(fullPath);
        return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString().Replace(Path.DirectorySeparatorChar, '/'));
    }

    private static string AppendDirectorySeparatorChar(string path)
    {
        if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            return path + Path.DirectorySeparatorChar;
        return path;
    }
    public static bool MatchesGlob(string filePath, string glob)
    {
        var regex = "^" + Regex.Escape(glob)
            .Replace(@"\*\*", ".*")
            .Replace(@"\*", @"[^/]*")
            .Replace(@"\?", ".") + "$";
        return Regex.IsMatch(filePath, regex, RegexOptions.IgnoreCase);
    }

    public static string FixRelaxedCommas(string rawJson)
    {
        return Regex.Replace(
            rawJson,
            @"(""[^""]*?""\s*:\s*(?:\[[^\]]*\]|\{[^\}]*\}|[^,\[\{\}\n]+?))(?=\r?\n\s*"")",
            "$1,"
        );
    }

    public static bool TryGetValue(JsonObject json, string key, out object? value)
    {
        if (json.HasProperty(key))
        {
            value = json[key];
            return true;
        }
        value = null;
        return false;
    }


    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("UQLExtra_CoroutineRunner");
                    _instance = go.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
    }

}