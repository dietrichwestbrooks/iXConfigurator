using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces
{
    public interface IVariableStore : INotifyPropertyChanged, INotifyCollectionChanged, IEnumerable<KeyValuePair<string, object>>
    {
        void AddVariable(string key, object value);

        void RemoveVariable(string key);

        object GetVariableValue(string key);

        void SetVariableValue(string key, object value);

        object this[string key] { get; set; }

        bool TryGetVariableValue<TValue>(string key, out TValue value);

        bool HasVariable(string key);

        void ClearVariables();

        IEnumerable<string> GetExpressionVariableKeys(string expression);

        string ExpandVariables(string value);

        bool TryEvaluateExpression<TValue>(string expression, out TValue value);

        bool Save();
    }
}
