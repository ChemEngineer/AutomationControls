using System.Collections;
using System.Collections.ObjectModel;


namespace AutomationControls.Interfaces
{
    public interface ISerializationSurrogate : IProvideUserControls, IProfileName
    {
        string path { get; set; }
        object getData { get; }
        void setData(object obj);
        IList getList { get; }
        IDictionary getListList { get; }
        bool ContainsProfile(string profileName);
        void AddProfile(string profileName);
        void DeleteProfile(string profileName);
        void LoadProfile(string profileName);
        ObservableCollection<string> ProfileNames { get; }
        string Serialize();
        string Deserialize();
    }


}
