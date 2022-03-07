using System.Runtime.Serialization;

namespace Vorp.Core.Client.Interface.Menu
{
    [DataContract]
    internal class MenuBase
    {
        [DataMember(Name = "title")]
        public string Title;

        [DataMember(Name = "options")]
        public List<MenuOptions> Options { get; private set; } = new();

        public void AddOption(MenuOptions menuOptions)
        {
            menuOptions.Order = Options.Count;
            Options.Add(menuOptions);
        }
    }

    [DataContract]
    internal class MenuOptions
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

        [DataMember(Name = "listMin")]
        public int ListMin;

        [DataMember(Name = "listMax")]
        public int ListMax;

        [DataMember(Name = "listLabel")]
        public string ListLabel;

        [DataMember(Name = "options")]
        public List<MenuOptions> Options { get; private set; } = new();

        public void AddOption(MenuOptions menuOptions)
        {
            menuOptions.Order = Options.Count;
            Options.Add(menuOptions);
        }
    }
}
