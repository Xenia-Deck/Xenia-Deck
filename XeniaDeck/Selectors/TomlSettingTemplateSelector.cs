using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using XeniaDeck.Core.ViewModels;

namespace XeniaDeck.Selectors
{
    internal partial class TomlSettingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? BooleanTemplate { get; set; }
        public DataTemplate? LongTemplate { get; set; }
        public DataTemplate? DoubleTemplate { get; set; }
        public DataTemplate? StringTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is TomlSetting tomlSetting)
            {
                if (tomlSetting.Value is bool)
                {
                    return BooleanTemplate ?? throw new InvalidOperationException();
                }
                if (tomlSetting.Value is long)
                {
                    return LongTemplate ?? throw new InvalidOperationException();
                }
                if (tomlSetting.Value is double)
                {
                    return DoubleTemplate ?? throw new InvalidOperationException();
                }
                else
                {
                    return StringTemplate ?? throw new InvalidOperationException();
                }
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}
