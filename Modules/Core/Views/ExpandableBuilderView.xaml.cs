using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    /// <summary>
    /// Interaction logic for ExpandableBuilderView.xaml
    /// </summary>
    public partial class ExpandableBuilderView
    {
        public ExpandableBuilderView()
        {
            InitializeComponent();
        }

        private void ConfiguratorVariables_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            var key = ConfiguratorVariables.SelectedItem as string;

            if (key == null)
            {
                return;
            }

            var expression = $"%{key}%";

            var index = Result.CaretIndex;

            if (!string.IsNullOrEmpty(Result.SelectedText))
            {
                Result.SelectedText = expression;
            }
            else
            {
                Result.Text = Result.Text.Insert(Result.CaretIndex, expression);
            }

            Result.CaretIndex = index + expression.Length;

            Dispatcher.BeginInvoke(new Action(() => Result.Focus()), DispatcherPriority.ApplicationIdle);
        }

        private void EnvironmentVariables_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            var key = EnvironmentVariables.SelectedItem as string;

            if (key == null)
            {
                return;
            }

            var expression = $"%{key}%";

            var index = Result.CaretIndex;

            if (!string.IsNullOrEmpty(Result.SelectedText))
            {
                Result.SelectedText = expression;
            }
            else
            {
                Result.Text = Result.Text.Insert(Result.CaretIndex, expression);
            }

            Result.CaretIndex = index + expression.Length;

            Dispatcher.BeginInvoke(new Action(() => Result.Focus()), DispatcherPriority.ApplicationIdle);
        }
    }
}
