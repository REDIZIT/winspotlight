namespace Winspotlight
{
    public partial class SettingsWindow
    {
        private class IntervalSelectionModel
        {
            public string DisplayText { get; set; }
            public int Minutes { get; set; }

            public IntervalSelectionModel(string displayText, int minutes)
            {
                DisplayText = displayText;
                Minutes = minutes;
            }

            public override string ToString()
            {
                return DisplayText;
            }
        }
    }
}
