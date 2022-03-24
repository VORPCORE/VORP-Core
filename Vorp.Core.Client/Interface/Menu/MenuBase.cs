using System.Runtime.Serialization;

namespace Vorp.Core.Client.Interface.Menu
{
    [DataContract]
    internal class MenuBase
    {
        [DataMember(Name = "title")]
        public string Title;

        [DataMember(Name = "subtitle")]
        public string SubTitle;

        [DataMember(Name = "options")]
        public List<MenuOption> Options { get; private set; } = new();

        [DataMember(Name = "updated")]
        public long Updated => GetGameTimer();

        public void AddOption(MenuOption menuOptions)
        {
            menuOptions.Order = Options.Count;
            Options.Add(menuOptions);
        }
    }

    [DataContract]
    internal class MenuOption
    {
        [DataMember(Name = "order")]
        public int Order;

        [DataMember(Name = "value")]
        public int Value;

        [DataMember(Name = "label")]
        public string Label;

        [DataMember(Name = "rightLabel")]
        public string RightLabel;

        [DataMember(Name = "subtitle")]
        public string SubTitle;

        [DataMember(Name = "type")]
        public string Type;

        [DataMember(Name = "description")]
        public string Description;

        [DataMember(Name = "endpoint")]
        public string Endpoint;

        [DataMember(Name = "focusEndpoint")]
        public string FocusEndpoint;

        [DataMember(Name = "listMin")]
        public int ListMin = 1;

        [DataMember(Name = "listMax")]
        public int ListMax;

        [DataMember(Name = "listLabel")]
        public string ListLabel;

        [DataMember(Name = "options")]
        public List<MenuOption> Options { get; private set; } = new();

        public void AddOption(MenuOption menuOptions)
        {
            menuOptions.Order = Options.Count;
            Options.Add(menuOptions);
        }

        public static MenuOption MenuOptionDivider()
        {
            MenuOption menuOptions = new();
            menuOptions.Type = "spacer";
            return menuOptions;
        }

        public static MenuOption MenuOptionList(string label, string description, string endpoint, List<long> list, long currentValue, string focusEndpoint = "")
        {
            MenuOption menuOptions = new();
            menuOptions.Type = "list";
            menuOptions.Label = label;
            menuOptions.Endpoint = endpoint;
            menuOptions.Description = description;
            menuOptions.ListMin = 0;
            menuOptions.ListMax = list.Count;
            menuOptions.Value = list.IndexOf(currentValue);
            menuOptions.FocusEndpoint = focusEndpoint;
            return menuOptions;
        }

        public static MenuOption MenuOptionMenu(string label, string subtitle, string description, string focusEndpoint = "")
        {
            MenuOption menuOptions = new();
            menuOptions.Type = "menu";
            menuOptions.Label = label;
            menuOptions.SubTitle = subtitle;
            menuOptions.Description = description;
            menuOptions.FocusEndpoint = focusEndpoint;
            return menuOptions;
        }

        public static MenuOption MenuOptionButton(string label, string description, string endpoint, string rightLabel = "", string focusEndpoint = "")
        {
            MenuOption menuOptions = new();
            menuOptions.Type = "button";
            menuOptions.Label = label;
            menuOptions.RightLabel = rightLabel;
            menuOptions.Endpoint = endpoint;
            menuOptions.Description = description;
            menuOptions.FocusEndpoint = focusEndpoint;
            return menuOptions;
        }
    }
}
