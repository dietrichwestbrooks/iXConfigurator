using System;
using Wayne.Payment.Products.iXConfigurator.Template;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class OptionViewModel : ElementViewModel
    {
        private Option _option;

        public OptionViewModel(Option option) 
            : base(option)
        {
            _option = option;

            if (option.Key == WellKnownVariableNames.InstallPath || 
                option.Key == WellKnownVariableNames.InstallDrive ||
                option.Key == WellKnownVariableNames.LibraryPath || 
                option.Key == WellKnownVariableNames.TempPath)
            {
                throw new InvalidOperationException($"Invalid key for reserved variable: {Key}");
            }

            Key = _option.Key;
            Label = _option.Label;
        }

        public override void CreateElements()
        {
            base.CreateElements();

            Control = ElementFactory.CreateControl(_option.Key, _option.Label, _option.Required, _option.Control);

            Control.CreateElements();
        }

        public override void BindVariables()
        {
            base.BindVariables();

            Control.BindVariables();
        }

        public override void InitializeVariables()
        {
            base.InitializeVariables();

            Control.InitializeVariables();
        }

        public override bool Validate()
        {
            return Control.Validate();
        }

        public ControlViewModel Control { get; private set; }

        public string Key { get; }

        public string Label { get; }

        public override void Dispose()
        {
            base.Dispose();

            Control.Dispose();
        }
    }
}
