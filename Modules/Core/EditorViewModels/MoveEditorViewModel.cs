using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class MoveEditorViewModel : TaskEditorViewModel
    {
        private string _from;
        private string _to;
        private bool _overwrite;

        public MoveEditorViewModel()
        {
            Title = "Move Files Task";

            From = "%libraryPath%";
            To = "%installPath%";

            Errors.Add(nameof(From), new List<string>());
            Errors.Add(nameof(To), new List<string>());
        }

        public string From
        {
            get { return _from; }
            set { SetProperty(ref _from, value, () => OnFromChanged(value)); }
        }

        public string To
        {
            get { return _to; }
            set { SetProperty(ref _to, value, () => OnToChanged(value)); }
        }

        public bool Overwrite
        {
            get { return _overwrite; }
            set { SetProperty(ref _overwrite, value); }
        }

        protected override void OnValidate(string propertyName)
        {
            if (Equals(propertyName, nameof(From)))
            {
                if (string.IsNullOrWhiteSpace(From))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
            else if (Equals(propertyName, nameof(To)))
            {
                if (string.IsNullOrWhiteSpace(To))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<move {GetCondition()} {GetOverwrite()}>\n{GetFrom()}{GetTo()}</move>"
                .FormattedXml();
        }

        protected string GetOverwrite()
        {
            return _overwrite ? "overwrite=\"true\"" : string.Empty;
        }

        protected string GetFrom()
        {
            return $"\t<from>{From}</from>\n";
        }

        protected string GetTo()
        {
            return $"\t<to>{To}</to>\n";
        }

        private void OnFromChanged(string from)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<ExpandableEnum>(from) == ExpandableEnum.Build)
                {
                    var expandable = OpenExpandableBuilder(true);
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => From = expandable));
                }
            }
            catch (InvalidOperationException)
            {
                // Do nothing
            }
        }

        private void OnToChanged(string to)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<ExpandableEnum>(to) == ExpandableEnum.Build)
                {
                    var expandable = OpenExpandableBuilder(true);
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => To = expandable));
                }
            }
            catch (InvalidOperationException)
            {
                // Do nothing
            }
        }
    }
}
