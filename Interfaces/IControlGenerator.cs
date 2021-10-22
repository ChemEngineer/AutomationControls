using AutomationControls.Codex.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationControls.Interfaces
{
    public interface IControlGenerator
    {
        string GenerateAll(CodexData data);
        string GenerateTextBox(PropertiesData data);
        string GenerateLabel(PropertiesData data);
        string GenerateCheckBox(PropertiesData data);
        string GenerateDateTimePicker(PropertiesData data);
        string GenerateRadioButton(PropertiesData data);
        string GenerateDropDownList(PropertiesData data);
        string GenerateDropDownListFromEnum(PropertiesData data);
    }
}
