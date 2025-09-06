using System;
using System.Collections.Generic;
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

    public static string GlobToRegex(string glob)
    {

        glob = Regex.Replace(glob, @"(\\(?!\*)|/)+", "/");

        if (glob.Length > 0 && glob[0] == '/') glob = glob.Substring(1);

        string regex = "^" + Regex.Escape(glob)
            .Replace("\u0000", "")
            .Replace("\uFFFF", "")
            .Replace(@"\\\\", "\uFFFF")
            .Replace(@"\\\*", "\u0000")
            .Replace(@"\*\*/", "(.*/)?")
            .Replace(@"\*\*", ".*")
            .Replace(@"\*", @"[^/]*")
            .Replace("\u0000", @"\*")
            .Replace(@"\\\?", "\u0000")
            .Replace(@"\?", "[^/]")
            .Replace("\u0000", @"\?")
            .Replace('\uFFFF', '\\') + "$";

        return regex;
    }

    public static void RegisterGlob(string glob, List<string> normal, List<string> inverted)
    {
        if (glob.Length == 0) return;

        bool isInverted = false;
        if (glob[0] == '!')
        {
            if (glob.Length == 1) return;
            isInverted = true;
            glob = glob.Substring(1);
        }

        glob = GlobToRegex(glob);

        if (glob.Length == 0) return;

        if (isInverted) inverted.Add(glob);
        else normal.Add(glob);
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