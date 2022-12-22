
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using SocketSaber.Utils;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BSDRP.Configuration {
    internal enum TimestampMode {
        Disabled,
        Static,
        ChangeOnMap
    }
    internal class PluginConfig {
        public static PluginConfig Instance { get; set; }
        public virtual long TargetDiscordAppID { get; set; } = 956250915378192454;
        public virtual bool DRPEnabled { get; set; } = true;
        public virtual DictStrO CoverOverrides { get; set; } = new DictStrO();
        public virtual string State { get; set; } = "Playing on {difficulty} in {mode} mode on {phraseMapBy}.";
        public virtual string Details { get; set; } = "{phraseSongAuthor}{songName} {subname}";
        public virtual string SmallImageText { get; set; } = "{mode}";
        public virtual string LargeImageText { get; set; } = "{songName} - {difficulty}";
        public virtual TimestampMode TimestampMode { get; set; } = TimestampMode.ChangeOnMap;


        public virtual void OnReload() {
            // Do stuff after config is read from disk.
        }

        public virtual void Changed() {
            // Do stuff when the config is changed.
        }
    }
}
