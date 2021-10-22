using System;
using System.Linq;
using System.Windows.Markup;

namespace AutomationControls.WPF_Helpers
{

    //<ComboBox DisplayMemberPath="DisplayName"
    //    ItemsSource="{persons:EnumToItemsSource {x:Type enums:States}}"
    //    SelectedValue="{Binding Path=WhereEverYouWant}"
    //    SelectedValuePath="Value" />

    public class EnumToItemsSource : MarkupExtension
    {
        private readonly Type _type;

        public EnumToItemsSource(Type type)
        {
            _type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(_type)
                .Cast<object>()
                .Select(e => new { Value = (int)e, DisplayName = e.ToString() });
        }
    }
}
