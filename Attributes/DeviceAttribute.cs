using System;

namespace AutomationControls.Attributes
{
    public class DeviceAttribute : Attribute
    {


        private string _menuItemName;
        public string menuItemName
        {
            get { return _menuItemName; }
            set
            {
                _menuItemName = value;
            }
        }

        public DeviceAttribute(string menuItemName)
        {
            this._menuItemName = menuItemName;
        }
    }
}
