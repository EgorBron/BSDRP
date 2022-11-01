
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using SocketSaber.Utils;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BSDRP.Configuration {
    internal class PluginConfig {
        public static PluginConfig Instance { get; set; }
        public virtual long TargetDiscordAppID { get; set; } = 956250915378192454;
        public virtual bool DRPEnabled { get; set; } = true;
        public virtual DictStrO CoverOverrides { get; set; } = new DictStrO();

        public virtual void OnReload() {
            // Do stuff after config is read from disk.
        }

        public virtual void Changed() {
            // Do stuff when the config is changed.
        }
    }
}
