
# Extra Parameters: a Rain World utility mod

---

### This mod provides various extra parameters to `modinfo.json`

`exclude_steam` - globs to exclude when uploading the mod to Steam (globs starting with `!` will be necessarily included); either a list/string or a dictionary with keys `globs` (list/string), (optional) `dry_run` (bool), and (optional) `keep_dependency` (bool).

- `dry_run` , if enabled, will prevent the mod from being uploaded, and instead will generate a directory with files filtered exactly as they would be for the Steam Workshop upload. Use this for debugging your globs. By default, it is FALSE.
- `keep_dependency` will determine whether to keep Extra Parameters as a dependency. Enable this if your mod directly patches Extra Paraemters. By default, it is FALSE.

Steam Workshop Item: [3557598109](https://steamcommunity.com/sharedfiles/filedetails/?id=3557598109)