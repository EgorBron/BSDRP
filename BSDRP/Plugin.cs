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
using BSDRP.Utils;

namespace BSDRP {
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal static Discord.Discord DiscordO { get; private set; }
        internal static long launched;
        internal static DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        

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
            SockSEventation.SongStartEvent += DRPUtils.SongStartDRP;
            SockSEventation.SongEndEvent += (_) => {
                DRPUtils.MenuDRP();
            };
            BSEvents.menuSceneLoaded += DRPUtils.MenuDRP;
            // that isn't work in any case
            //SockSEventation.SongPauseEvent += DRPUtils.PauseDRP;
            //SockSEventation.SongResumeEvent += DRPUtils.ResumeDRP;
            //BSEvents.songPaused += DRPUtils.PauseDRP;
            //BSEvents.songUnpaused += DRPUtils.ResumeDRP;
        }

        [OnStart]
        public void OnApplicationStart() {
            new GameObject("BSDRPController").AddComponent<BSDRPController>();
            launched = (long)(DateTime.UtcNow - origin).TotalSeconds;
            DRPUtils.MenuDRP();
        }

        [OnExit]
        public void OnApplicationQuit() {
            DiscordO.Dispose();
        }
    }
}
