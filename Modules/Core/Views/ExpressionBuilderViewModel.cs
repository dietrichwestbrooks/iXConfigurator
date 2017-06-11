using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    public class ExpressionBuilderViewModel : BuilderViewModelBase
    {
        public ExpressionBuilderViewModel(IApplicationLogger logger, ITemplateManager templateManager) 
            : base(logger, templateManager)
        {
            Title = "Expression Builder";

            AddClauseCommand = new DelegateCommand<ClauseViewModel>(OnAddClause, clause => true);
            RemoveClauseCommand = new DelegateCommand<ClauseViewModel>(OnRemoveClause, clause => true);

            AddClause(0);
        }

        public ICommand AddClauseCommand { get; }

        public ICommand RemoveClauseCommand { get; }

        public ObservableCollection<ClauseViewModel> Clauses { get; } = new ObservableCollection<ClauseViewModel>();

        private void OnRemoveClause(ClauseViewModel clause)
        {
            clause.PropertyChanged -= OnClauseChanged;
            Clauses.Remove(clause);

            foreach (var c in Clauses)
            {
                c.IsFirst = Clauses.IndexOf(c) == 0;
                c.IsGrouped = false;
            }
        }

        private void OnAddClause(ClauseViewModel clause)
        {
            var index = Clauses.IndexOf(clause);
            AddClause(index + 1);
        }

        private void AddClause(int index)
        {
            var newClause = new ClauseViewModel();

            newClause.PropertyChanged += OnClauseChanged;

            Clauses.Insert(index, newClause);

            foreach (var c in Clauses)
            {
                c.IsFirst = Clauses.IndexOf(c) == 0;
                c.IsGrouped = false;
            }
        }

        private void OnClauseChanged(object sender, PropertyChangedEventArgs e)
        {
             BuildExpression();
        }

        private void BuildExpression()
        {
            ResultBuilder.Clear();

            var validClauses = Clauses.Where(clause => !string.IsNullOrWhiteSpace(clause.LeftOperand)).ToList();

            if (!validClauses.Any())
            {
                return;
            }

            if (validClauses.Any(c => !c.IsGrouped))
            {
                var first = validClauses.First(c => !c.IsGrouped);

                ResultBuilder.Append(first);

                foreach (var clause in validClauses.Where(c => !c.IsGrouped).Skip(1))
                {
                    ResultBuilder.Append($" {clause.LogicOperator} {clause}");
                }
            }

            if (validClauses.Any(c => c.IsGrouped))
            {
                var first = validClauses.First(c => c.IsGrouped);

                if (validClauses.All(c => c.IsGrouped))
                {
                    ResultBuilder.Append($"{first}");
                }
                else
                {
                    ResultBuilder.Append($" {first.LogicOperator} ({first}");
                }

                foreach (var clause in validClauses.Where(c => c.IsGrouped).Skip(1))
                {
                    ResultBuilder.Append($" {clause.LogicOperator} {clause}");
                }

                if (!validClauses.All(c => c.IsGrouped))
                {
                    ResultBuilder.Append(")");
                }
            }
        }
    }
}
