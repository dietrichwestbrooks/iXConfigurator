﻿using System;
using System.Collections.Specialized;
using System.Windows;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Behaviors
{
    public class DialogActivationBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        /// <summary>
        /// The key of this behavior
        /// </summary>
        public const string BehaviorKey = "DialogActivation";

        private IWindow _contentDialog;
        private NavigationParameters _navigationParameters;

        /// <summary>
        /// Gets or sets the <see cref="DependencyObject"/> that the <see cref="IRegion"/> is attached to.
        /// </summary>
        /// <value>A <see cref="DependencyObject"/> that the <see cref="IRegion"/> is attached to.
        /// This is usually a <see cref="FrameworkElement"/> that is part of the tree.</value>
        public DependencyObject HostControl { get; set; }

        /// <summary>
        /// Performs the logic after the behavior has been attached.
        /// </summary>
        protected override void OnAttach()
        {
            Region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
            Region.NavigationService.Navigating += NavigationServiceOnNavigating;
        }

        /// <summary>
        /// Override this method to create an instance of the <see cref="IWindow"/> that 
        /// will be shown when a view is activated.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="IWindow"/> that will be shown when a 
        /// view is activated on the target <see cref="IRegion"/>.
        /// </returns>
        protected IWindow CreateWindow()
        {
            return new WindowWrapper();
        }

        private void NavigationServiceOnNavigating(object sender, RegionNavigationEventArgs e)
        {
            _navigationParameters = e.NavigationContext.Parameters;
        }

        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                CloseContentDialog();
                PrepareContentDialog(e.NewItems[0]);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                CloseContentDialog();
            }
        }

        private Style GetStyleForView()
        {
            return HostControl.GetValue(RegionPopupBehaviors.ContainerWindowStyleProperty) as Style;
        }

        private void PrepareContentDialog(object view)
        {
            var frameworkElement = view as FrameworkElement;

            if (frameworkElement != null)
            {
                var acceptParameters = frameworkElement.DataContext as IAcceptNavigationParameters;

                acceptParameters?.AcceptNavigationParameters(_navigationParameters);
            }

            _contentDialog = CreateWindow();
            _contentDialog.Content = view;
            _contentDialog.Owner = HostControl;
            _contentDialog.Closed += ContentDialogClosed;
            _contentDialog.Style = GetStyleForView();
            _contentDialog.Show();
        }

        private void CloseContentDialog()
        {
            if (_contentDialog != null)
            {
                _contentDialog.Closed -= ContentDialogClosed;
                _contentDialog.Close();
                _contentDialog.Content = null;
                _contentDialog.Owner = null;
            }
        }

        private void ContentDialogClosed(object sender, System.EventArgs e)
        {
            Region.Remove(_contentDialog.Content);
            CloseContentDialog();
        }
    }
}
