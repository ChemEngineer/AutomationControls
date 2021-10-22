using AutomationControls.Serialization;
using System;
using System.Collections;

namespace AutomationControls.Interfaces
{
    public interface IProfileName
    {
        string profileName { get; set; }
    }

    public interface ISupportProfiles
    {
        //string currentProfile { get; set; }
        IList getList { get; }
        Object getData { get; }
        void AddProfile(string profileName);
        void LoadProfile(string profileName);
        void DeleteProfile(string profileName);
        void ChangeProfileName(string profileName);
        string Serialize(SerializationType type = SerializationType.JSON);
        bool Deserialize(SerializationType type = SerializationType.JSON);
        global::System.Collections.Generic.List<string> GetProfileNames();
        event EventHandler AddRemove;
        event EventHandler ProfileChanged;
    }
}
