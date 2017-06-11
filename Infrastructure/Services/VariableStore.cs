using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Practices.ServiceLocation;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Services
{
    public sealed class VariableStore : DynamicObject, IVariableStore
    {
        private IApplicationLogger _logger;

        private Dictionary<string, object> _variables = new Dictionary<string, object>();

        private static Regex _variableNameRestriction = new Regex("%([a-zA-Z_$][a-zA-Z_$0-9]*)%");

        public VariableStore(IApplicationLogger logger)
        {
            _logger = logger;
        }

        #region IVariableStore

        public bool HasVariable(string key)
        {
            return _variables.ContainsKey(key);
        }

        public void AddVariable(string key, object value)
        {
            if (_variables.ContainsKey(key))
            {
                SetVariableValue(key, value);
                return;
            }

            _variables.Add(key, value);

            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, key);

            RaisePropertyChanged(key);
        }

        public void RemoveVariable(string key)
        {
            if (!_variables.ContainsKey(key))
            {
                return;
            }

            _variables.Remove(key);

            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, key);
        }

        public object GetVariableValue(string key)
        {
            if (!_variables.ContainsKey(key))
            {
                throw new KeyNotFoundException(key);
            }

            return _variables[key];
        }

        public void SetVariableValue(string key, object value)
        {
            if (!_variables.ContainsKey(key))
            {
                throw new KeyNotFoundException(key);
            }

            if (Equals(_variables[key], value))
            {
                return;
            }

            _variables[key] = value;

            RaisePropertyChanged(key);
        }

        public object this[string key]
        {
            get { return GetVariableValue(key); }
            set { AddVariable(key, value); }
        }

        public bool TryGetVariableValue<TValue>(string key, out TValue value)
        {
            value = default(TValue);

            try
            {
                var result = GetVariableValue(key);
                value = ConverterHelper.Convert<TValue>(result);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);

                // Use default value
                return false;
            }

            return true;
        }

        public void ClearVariables()
        {
            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, _variables.Keys.ToArray());

            _variables.Clear();
        }

        public string ExpandVariables(string value)
        {
            var matches = _variableNameRestriction.Matches(value);

            if (matches.Count <= 0)
                return value;

            foreach (var key in matches.OfType<Match>().Select(match => match.Groups[1].Value))
            {
                object result;

                if (TryGetVariableValue(key, out result))
                {
                    var resultStr = result?.ToString() ?? string.Empty;

                    if (result is bool)
                    {
                        resultStr = resultStr.ToLower();
                    }

                    value = value.Replace($"%{key}%", resultStr);
                }
            }

            return value;
        }

        public IEnumerable<string> GetExpressionVariableKeys(string expression)
        {
            var matches = _variableNameRestriction.Matches(expression);

            if (matches.Count <= 0)
                return Enumerable.Empty<string>();

            return matches.OfType<Match>().Select(match => match.Groups[1].Value).ToList();
        }

        public bool TryEvaluateExpression<TValue>(string expression, out TValue value)
        {
            value = default(TValue);

            try
            {
                var parsed = ParseExpression(expression);

                var exp = parsed.Item1;
                var p = parsed.Item2.Select(v => Expression.Parameter(v.Value.GetType(), v.Key));
                var e = DynamicExpression.ParseLambda(p.ToArray(), null, exp);

                var result = e.Compile().DynamicInvoke(parsed.Item2.Select(v => v.Value).ToArray());

                value = ConverterHelper.Convert<TValue>(result);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);

                // Use default value
                return false;
            }

            return true;
        }

        public bool Save()
        {
            var serializer = new XmlSerializer(typeof(VariableStore));

            string xml;

            using (var memorySteam = new MemoryStream())
            using (var writer = new XmlTextWriter(memorySteam, Encoding.UTF8))
            {
                serializer.Serialize(writer, this);
                memorySteam.Position = 0;
                using (var reader = new StreamReader(memorySteam))
                {
                    xml = reader.ReadToEnd();
                    reader.Close();
                }
                writer.Close();
            }

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "variables.xml");

            var fileService = ServiceLocator.Current.GetInstance<IFileService>();

            fileService.SaveTextFileAsync(path, xml);

            return true;
        } 

        #endregion

        #region DynamicObject

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _variables.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            if (!HasVariable(binder.Name))
            {
                return false;
            }

            result = GetVariableValue(binder.Name);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {

            if (!HasVariable(binder.Name))
            {
                return false;
            }

            SetVariableValue(binder.Name, value);

            return true;
        } 

        #endregion

        #region INotifyPropertyChange

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChanged(NotifyCollectionChangedAction action, params string[] propertyNames)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, propertyNames));
        }

        #endregion

        #region IEnumerable<string, object>

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _variables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_variables).GetEnumerator();
        } 

        #endregion

        private Tuple<string, IDictionary<string, object>> ParseExpression(string expression)
        {
            var parameters = new Dictionary<string, object>();

            var rgx = new Regex(@"\s+and\s+", RegexOptions.IgnoreCase);
            expression = rgx.Replace(expression, " && ");

            rgx = new Regex(@"\s+or\s+", RegexOptions.IgnoreCase);
            expression = rgx.Replace(expression, " || ");

            rgx = new Regex(@"'");
            expression = rgx.Replace(expression, "\"");

            rgx = new Regex(@"\s+=\s+");
            expression = rgx.Replace(expression, " == ");

            rgx = new Regex(@"(%[a-zA-Z_$][a-zA-Z_$0-9]*%)\s+is\s+empty", RegexOptions.IgnoreCase);
            expression = rgx.Replace(expression, "string.IsNullOrEmpty($1)");

            var matches = _variableNameRestriction.Matches(expression);

            if (matches.Count > 0)
            {
                foreach (var match in matches.OfType<Match>())
                {
                    var key = match.Groups[1].Value;

                    object value;

                    if (TryGetVariableValue(key, out value))
                    {
                        var identifier = $"@{key}";

                        expression = expression.Replace($"%{key}%", identifier);

                        if (!parameters.ContainsKey(identifier))
                        {
                            bool boolVal;

                            if (bool.TryParse(value?.ToString(), out boolVal))
                            {
                                parameters.Add(identifier, boolVal);
                            }
                            else
                            {
                                parameters.Add(identifier, value);
                            }
                        }
                    }
                }
            }

            return new Tuple<string, IDictionary<string, object>>(expression, parameters);
        }
    }
}
