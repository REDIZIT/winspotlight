using Winspotlight.Models;

namespace Winspotlight.Plugins.Embedded.MutePlugin
{
    public class SearchMuteItem : SearchItem
    {
        public int processId;
        public string processName;
        public bool IsMuted 
        {
            get { return _isMuted; }
            set { _isMuted = value; Refresh(); }
        }
        private bool _isMuted;

        public SearchMuteItem(string displaySubName, int processId, string processName)
        {
            this.displaySubName = displaySubName;
            this.processId = processId;
            this.processName = processName;

            bool? muted = WindowsSoundAPI.GetApplicationMute(processId);
            if (muted != null) IsMuted = muted.Value;
        }

        public override void Execute(bool runAsAdministrator)
        {
            // Toggle mute

            bool? muted = WindowsSoundAPI.GetApplicationMute(processId);
            if (muted == null) return;

            WindowsSoundAPI.SetApplicationMute(processId, !muted.Value);

            IsMuted = !IsMuted;
        }

        private void Refresh()
        {
            displayName = (IsMuted ? "Unmute " : "Mute ") + processName;
            iconBitmap = IsMuted ? MutePluginCore.unmuteIcon : MutePluginCore.muteIcon;
        }
    }
}
