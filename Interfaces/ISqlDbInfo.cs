using System;

namespace AutomationControls.Interfaces
{
    public interface ISqlDbInfo
    {
        DateTime lastUpdated { get; set; }
        string key { get; set; }
    }
}
