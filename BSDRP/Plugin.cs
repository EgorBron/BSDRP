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

namespace BSDRP {
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal static Discord.Discord Discord { get; private set; }

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger) {
            Instance = this;
            Log = logger;
            Log.Info("BSDRP initialized.");
            Discord = new Discord.Discord(956250915378192454, (ulong)CreateFlags.Default);
            var activitymgr = Discord.GetActivityManager();
            SockSEventation.SongStartEvent += (eventa) => {
                activitymgr.UpdateActivity(new Activity {
                    Assets = new ActivityAssets { LargeImage = eventa.mapBeatSaverCoverLink },
                    Details = eventa.songName,
                    State = eventa.songAuthorName,
                    ApplicationId = 956250915378192454,
                    Type = ActivityType.Playing
                }, (a) => { Log.Notice(a.ToString()); });
                Log.Notice("AOK");
            };
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart() {
            Log.Info("OnApplicationStart");
            new GameObject("BSDRPController").AddComponent<BSDRPController>();
        }

        [OnExit]
        public void OnApplicationQuit() {
            Log.Debug("OnApplicationQuit");
            Discord.Dispose();
        }
    }
}
