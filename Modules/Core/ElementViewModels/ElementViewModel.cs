using System;
using Microsoft.Practices.Unity;
using Prism.Events;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public abstract class ElementViewModel : ViewModelBase, IDisposable
    {
        private BindableVariableExpression<bool> _isVisibleProperty;
        private string _visible;
        private bool _isVisible;
        private bool _isActive;

        protected ElementViewModel()
        {
            _isVisibleProperty = new BindableVariableExpression<bool>(this, nameof(IsVisible));
            _visible = "true";
        }

        protected ElementViewModel(Element element)
            : this()
        {
            _visible = element?.Visible ?? "true";
        }

        public virtual void CreateElements()
        {
            
        }

        public virtual void BindVariables()
        {
            _isVisibleProperty.Bind(_visible);
        }

        public virtual void InitializeVariables()
        {

        }

        public virtual bool Validate()
        {
            return true;
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value, () => OnIsVisbleChanged(value)); }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        [Dependency]
        protected IEventAggregator Events { get; set; }

        [Dependency]
        protected IApplicationLogger Logger { get; set; }

        [Dependency]
        protected IVariableStore Variables { get; set; }

        [Dependency]
        protected IElementViewModelFactory ElementFactory { get; set; }

        [Dependency]
        protected ITaskCommandFactory CommandFactory { get; set; }

        protected virtual void OnIsVisbleChanged(bool isVisible)
        {
            IsActive = isVisible;
        }

        public virtual void Dispose()
        {
            _isVisibleProperty.Dispose();
        }
    }
}
