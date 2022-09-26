using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSDRP.Configuration;
using Discord;
using SocketSaber.EventModels;
using static UnityEngine.UI.Image;

namespace BSDRP {
    internal class DRPUtils {
        internal static string GetStandardCover(string levelid) {
            return "";
        }

        internal static void MenuDRP() {
            if (PluginConfig.Instance.DRPEnabled)
                Plugin.DiscordO.GetActivityManager().UpdateActivity(new Activity {
                    Timestamps = new ActivityTimestamps {
                        Start = Plugin.launched,
                        End = 0L
                    },
                    Assets = new ActivityAssets {
                        LargeImage = "mapselect_md"
                    },
                    Details = "In menu"
                }, (a) => { });
        }
        internal static void SongStartDRP(SongStartEM eventa) {
            if (PluginConfig.Instance.DRPEnabled) {
                var now = (long)(DateTime.UtcNow - Plugin.origin).TotalSeconds;
                var subname = eventa.songSubName;
                string difficulty = eventa.mapDifficulty.ToString().Replace("Plus", "+");
                var mode = eventa.mode
                .Replace("OneSaber", "One Saber")
                .Replace("NoArrows", "No Arrows")
                .Replace("90Degree", "90 Degree")
                .Replace("360Degree", "360 Degree") + (eventa.gameplayModifiers.zenMode ? " Zen" : "");

                var state = $"Playing on {difficulty} in {mode} mode on {((!eventa.mapGameLevelID.StartsWith("custom_level_")) ? "offical map." : "map by " + eventa.mapAuthor + ".")}";

                if (subname != "") {
                    if (!subname.StartsWith(" (")) {
                        if (!subname.StartsWith("(")) subname = "(" + eventa.songSubName;
                        subname = " " + subname;
                    }
                    if (!subname.EndsWith(")")) {
                        subname += ")";
                    }
                }
                var details = $"{((eventa.songAuthorName == "") ? "" : (eventa.songAuthorName + " - "))}{eventa.songName} {subname}";

                Plugin.DiscordO.GetActivityManager().UpdateActivity(new Activity {
                    Assets = new ActivityAssets {
                        LargeImage = eventa.mapBeatSaverCoverLink ?? GetStandardCover(eventa.mapGameLevelID),
                        LargeText = $"{eventa.songName} - {difficulty}",
                        SmallImage = "beat_saber_logo",
                        SmallText = mode
                    },
                    Timestamps = new ActivityTimestamps {
                        Start = now,
                        End = (long)(now + eventa.duration)
                    },
                    Details = details,
                    State = state,
                    ApplicationId = 956250915378192454,
                    Type = ActivityType.Playing
                }, (a) => { if (a == Result.Ok) Plugin.Log.Notice($"Updated DRP: SongStart --- {details}"); });
            }
        }
    }
}
