using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSDRP.Configuration;
using Discord;
using SocketSaber.EventModels;
using static StandardLevelGameplayManager;
using static UnityEngine.UI.Image;

namespace BSDRP.Utils {
    internal class DRPUtils {
        static ActivityTimestamps timestampsLatest;
        static SongStartEM songDataLatest;

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
            SongStartDRP(eventa, new ActivityTimestamps());
        }
        internal static void SongStartDRP(SongStartEM eventa, ActivityTimestamps? time = null) {
            if (PluginConfig.Instance.DRPEnabled) {
                songDataLatest = eventa;

                var now = (long)(DateTime.UtcNow - Plugin.origin).TotalSeconds;
                var subname = eventa.songSubName;
                string difficulty = eventa.mapDifficulty.ToString().Replace("Plus", "+");
                var mode = eventa.mode
                .Replace("OneSaber", "One Saber")
                .Replace("NoArrows", "No Arrows")
                .Replace("90Degree", "90 Degree")
                .Replace("360Degree", "360 Degree") + (eventa.gameplayModifiers.zenMode ? " Zen" : "");

                if (subname != "") {
                    if (!subname.StartsWith(" (")) {
                        if (!subname.StartsWith("(")) subname = "(" + eventa.songSubName;
                        subname = " " + subname;
                    }
                    if (!subname.EndsWith(")")) {
                        subname += ")";
                    }
                }

                var placeholders = new Dictionary<string, string>() {
                    { "mode", mode },
                    { "subname", subname },
                    { "difficulty", difficulty },
                    { "phraseMapBy", (!eventa.mapGameLevelID.StartsWith("custom_level_")) ? "offical map" : "map by " + eventa.mapAuthor },
                    { "mapBy", (!eventa.mapGameLevelID.StartsWith("custom_level_")) ? "offical map" : "custom map" },
                    { "phraseSongAuthor", (eventa.songAuthorName == "") ? "" : (eventa.songAuthorName + " - ") },
                    { "songAuthor", eventa.songAuthorName },
                    { "songName", eventa.songName },
                    { "mapNJS", eventa.mapNJS.ToString() },
                    { "mapAuthor", eventa.mapAuthor },
                    { "songBPM", eventa.songBPM.ToString() },
                };

                var state = "Playing on {difficulty} in {mode} mode on {phraseMapBy}.".Format(placeholders);
                var details = "{phraseSongAuthor}{songName} {subname}".Format(placeholders);

                if (time == null) {
                    if (timestampsLatest.Equals(default(ActivityTimestamps)))
                    timestampsLatest = new ActivityTimestamps {
                        Start = now,
                        End = (long)(now + eventa.duration)
                    };
                } else if (!time.Value.Equals(default(ActivityTimestamps))) {
                    timestampsLatest = time.Value;
                }

                Plugin.DiscordO.GetActivityManager().UpdateActivity(new Activity {
                    Assets = new ActivityAssets {
                        LargeImage = eventa.mapBeatSaverCoverLink ?? GetStandardCover(eventa.mapGameLevelID),
                        LargeText = $"{eventa.songName} - {difficulty}",
                        SmallImage = "beat_saber_logo",
                        SmallText = mode
                    },
                    Timestamps = timestampsLatest,
                    Details = details,
                    State = state,
                    ApplicationId = 956250915378192454,
                    Type = ActivityType.Playing
                }, (a) => { if (a == Result.Ok) Plugin.Log.Notice($"Updated DRP: SongStart --- {details}"); });
            }
        }
        internal static void PauseDRP() {
            Plugin.Log.Notice("Paused");
            var len = timestampsLatest.End - timestampsLatest.Start;
            var elapsed = timestampsLatest.End - (long)(DateTime.UtcNow - Plugin.origin).TotalSeconds;
            if (elapsed > 0 && len > elapsed) {
                timestampsLatest.Start = elapsed;
            }
            SongStartDRP(songDataLatest, default(ActivityTimestamps));
        }
        internal static void ResumeDRP() {
            Plugin.Log.Notice("Resumed");
            SongStartDRP(songDataLatest, timestampsLatest);
        }
    }
}
