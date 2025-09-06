
# Extra Parameters: a Rain World utility mod

---

### This mod provides various extra parameters to `modinfo.json`

`exclude_steam` - globs to exclude when uploading the mod to Steam (globs starting with `!` will be necessarily included); either a list/string or a dictionary with keys `globs` (list/string), (optional) `delete_temp` (bool), and (optional) `keep_dependency` (bool).

- `delete_temp` will determine whether or not to delete the temporary upload directory. By default, it is FALSE.
- `keep_dependency` will determine whether to keep Extra Parameters as a dependency if it's no longer required in the Steam upload. By default, it is FALSE.

Steam Workshop Item: [3557598109](https://steamcommunity.com/sharedfiles/filedetails/?id=3557598109)