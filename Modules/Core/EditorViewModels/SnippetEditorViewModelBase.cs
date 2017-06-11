using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public abstract class SnippetEditorViewModelBase : ViewModelBase, INotifyDataErrorInfo
    {
        private string _snippet;

        [Dependency]
        protected IRegionManager RegionManager { get; set; }

        protected static Regex KeyNameConstraint { get; } = new Regex("^[a-zA-Z_$][a-zA-Z_$0-9]*$");

        protected Dictionary<string, List<string>> Errors { get; } = new Dictionary<string, List<string>>();

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName) || !Errors.ContainsKey(propertyName))
            {
                return Enumerable.Empty<string>();
            }

            return Errors[propertyName];
        }

        public string Snippet
        {
            get { return _snippet; }
            set { SetProperty(ref _snippet, value); }
        }

        public bool HasErrors => Errors.Values.SelectMany(errors => errors).Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected string OpenExpressionBuilder()
        {
            var result = new StringBuilder();

            var parameters = new NavigationParameters
                {
                    {"Result", result}
                };

            RegionManager.RequestNavigate(RegionNames.DialogPopupRegion, PopupNames.ExpressionBuilderPopup, parameters);

            return result.ToString();
        }

        protected string OpenExpandableBuilder(bool environment = false)
        {
            var result = new StringBuilder();

            var parameters = new NavigationParameters
                {
                    {"Result", result},
                    {"Environment", environment}
                };

            RegionManager.RequestNavigate(RegionNames.DialogPopupRegion, PopupNames.ExpandableBuilderPopup, parameters);

            return result.ToString();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            OnUpdateSnippet();

            foreach (var errors in Errors.Values)
            {
                errors.Clear();
            }

            OnValidate(args.PropertyName);

            RaiseErrorsChanged(args.PropertyName);
        }

        protected virtual void OnValidate(string propertyName)
        {
        }

        protected virtual void OnUpdateSnippet()
        {
        }

        protected void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected string BooleanToLower(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return string.Empty;
            }

            if (string.Compare(expression, bool.TrueString, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) == 0)
            {
                return bool.TrueString.ToLower();
            }

            if (string.Compare(expression, bool.FalseString, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) == 0)
            {
                return bool.FalseString.ToLower();
            }

            return expression;
        }
    }
}
