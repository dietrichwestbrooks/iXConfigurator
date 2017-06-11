using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    public class ClauseViewModel : ViewModelBase
    {
        private bool _isGrouped;
        private bool _isFirst;
        private ClauseLogicalOperator _logicOperator;
        private string _leftOperand;
        private string _rightOperand;
        private ClauseOperator _operator;

        public ClauseViewModel()
        {
            Operator = ClauseOperator.Equal;
            LogicOperator = ClauseLogicalOperator.And;
        }

        public bool IsGrouped
        {
            get { return _isGrouped; }
            set { SetProperty(ref _isGrouped, value); }
        }

        public bool IsFirst
        {
            get { return _isFirst; }
            set { SetProperty(ref _isFirst, value); }
        }

        public ClauseLogicalOperator LogicOperator
        {
            get { return _logicOperator; }
            set { SetProperty(ref _logicOperator, value); }
        }

        public string LeftOperand
        {
            get { return _leftOperand; }
            set { SetProperty(ref _leftOperand, value?.Trim()); }
        }

        public string RightOperand
        {
            get { return _rightOperand; }
            set { SetProperty(ref _rightOperand, value); }
        }

        public ClauseOperator Operator
        {
            get { return _operator; }
            set { SetProperty(ref _operator, value); }
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(LeftOperand))
            {
                return string.Empty;
            }

            var clause = string.Empty;

            switch (Operator)
            {
                case ClauseOperator.IsTrue:
                    clause =$"%{LeftOperand}%";
                    break;

                case ClauseOperator.IsFalse:
                    clause =$"!%{LeftOperand}%";
                    break;

                case ClauseOperator.IsEmpty:
                    clause =$"%{LeftOperand}% is empty";
                    break;

                case ClauseOperator.IsNotEmpty:
                    clause =$"!(%{LeftOperand}% is empty)";
                    break;

                case ClauseOperator.Equal:
                    clause =$"%{LeftOperand}% = '{RightOperand}'";
                    break;

                case ClauseOperator.NotEqual:
                    clause =$"%{LeftOperand}% != '{RightOperand}'";
                    break;
            }

            return clause;
        }
    }
}
