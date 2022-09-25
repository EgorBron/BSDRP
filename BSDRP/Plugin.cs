using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;
using SocketSaber;
using Discord;
using System.Threading;
using BS_Utils;
using BS_Utils.Utilities;
using Config = IPA.Config.Config;
using BSDRP.Configuration;
using IPA.Config.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace BSDRP {
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal static Discord.Discord DiscordO { get; private set; }
        private long launched;
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        internal string GetStandardCover(string levelid) {
            return "";
        }

        internal void MenuDRP() {
            DiscordO.GetActivityManager().UpdateActivity(new Activity {
                Timestamps = new ActivityTimestamps {
                    Start = launched,
                    End = 0L
                },
                Assets = new ActivityAssets {
                    LargeImage = "mapselect_md"
                },
                Details = "In menu"
            }, (a) => { });
        }

        [Init]
        public void Init(IPALogger logger, Config conf) {
            Instance = this;
            Log = logger;
            Log.Info("BSDRP initialized.");

            PluginConfig.Instance = conf.Generated<PluginConfig>();
            Log.Debug("BSDRP config loaded");

            try {
                DiscordO = new Discord.Discord(PluginConfig.Instance.TargetDiscordAppID, (ulong)CreateFlags.Default);
            } catch (ResultException) { }
            if (DiscordO == null) { Log.Critical("Discord isn't initialized!"); return; }

            var activitymgr = DiscordO.GetActivityManager();
            SockSEventation.SongStartEvent += (eventa) => {
                if (PluginConfig.Instance.DRPEnabled) {
                    var now = (long)(DateTime.UtcNow - origin).TotalSeconds;
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

                    activitymgr.UpdateActivity(new Activity {
                        Assets = new ActivityAssets {
                            LargeImage = eventa.mapBeatSaverCoverLink??GetStandardCover(eventa.mapGameLevelID),
                            LargeText = $"{eventa.songName} - {difficulty} ({mode})",
                            SmallImage = "beat_saber_logo",
                            SmallText = $"With BSDRP and love"
                        },
                        Timestamps = new ActivityTimestamps {
                            Start = now,
                            End = (long)(now + eventa.duration)
                        },
                        Details = details,
                        State = state,
                        ApplicationId = 956250915378192454,
                        Type = ActivityType.Playing
                    }, (a) => { if (a == Result.Ok) Log.Notice($"Updated DRP: SongStart --- {details}"); });
                }
            };
            SockSEventation.SongEndEvent += (_) => {
                MenuDRP();
            };
            BSEvents.menuSceneLoaded += MenuDRP;
        }

        [OnStart]
        public void OnApplicationStart() {
            new GameObject("BSDRPController").AddComponent<BSDRPController>();
            launched = (long)(DateTime.UtcNow - origin).TotalSeconds;
            MenuDRP();
        }

        [OnExit]
        public void OnApplicationQuit() {
            DiscordO.Dispose();
        }
    }
}
