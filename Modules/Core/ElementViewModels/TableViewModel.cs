using System;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using Prism.Commands;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class TableViewModel : ControlViewModel<DataTable>
    {
        private TableControl _control;
        private bool _isEditable;
        private DataRowView _selectedRow;
        private DataView _items;

        public TableViewModel(string key, string label, string required, TableControl control)
            : base(key, label, required, control)
        {
            _control = control;

            _isEditable = _control.Editable.GetValueOrDefault();

            AddCommand = new DelegateCommand(AddRow, () => true);
            RemoveCommand = new DelegateCommand(RemoveRow, () => SelectedRow != null);
        }

        public override void InitializeVariables()
        {
            var dt = new DataTable();

            if (!string.IsNullOrWhiteSpace(_control.Data))
            {
                try
                {
                    // strip out empty lines
                    var lines = _control.Data.Split('\n');
                    var data = lines.Where(l => !string.IsNullOrWhiteSpace(l)).Aggregate((l1, l2) => $"{l1}\n{l2}");

                    using (var parser = new TextFieldParser(new StringReader(data)))
                    {
                        parser.HasFieldsEnclosedInQuotes = true;
                        parser.SetDelimiters(",");

                        var isFirstRow = true;

                        while (!parser.EndOfData)
                        {
                            var fields = parser.ReadFields();

                            if (fields == null)
                            {
                                continue;
                            }

                            if (isFirstRow)
                            {
                                foreach (var field in fields)
                                {
                                    dt.Columns.Add(new DataColumn(field));
                                }

                                isFirstRow = false;
                            }
                            else
                            {
                                dt.Rows.Add(fields.Take(dt.Columns.Count).Cast<object>().ToArray());
                            }
                        }
                    }

                    //foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
                    //{
                    //    var fields = line.Trim().Split(',');

                    //    if (isFirstRow)
                    //    {
                    //        foreach (var field in fields)
                    //        {
                    //            dt.Columns.Add(new DataColumn(field));
                    //        }

                    //        isFirstRow = false;
                    //    }
                    //    else
                    //    {
                    //        dt.Rows.Add(fields.Take(dt.Columns.Count).Cast<object>().ToArray());
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }

            Variables.AddVariable(Key, dt);

            Items = dt.AsDataView();
        }

        public DelegateCommand AddCommand { get; }

        public DelegateCommand RemoveCommand { get; }

        public DataView Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public bool IsEditable
        {
            get { return _isEditable; }
            set { SetProperty(ref _isEditable, value); }
        }

        public DataRowView SelectedRow
        {
            get { return _selectedRow; }
            set { SetProperty(ref _selectedRow, value, () => RemoveCommand?.RaiseCanExecuteChanged()); }
        }

        private void RemoveRow()
        {
            var dr = SelectedRow?.Row;

            if (dr == null)
            {
                return;
            }

            Items.Table.Rows.Remove(dr);
        }

        private void AddRow()
        {
            Items.Table.Rows.Add();
        }

        protected override void OnValueChanged(DataTable newValue)
        {
            Events.GetEvent<DebugOutputEvent>().Publish($"> ixconfig set -Key {Key} -Value [Table...]\n");
            Events.GetEvent<DebugOutputEvent>().Publish($"{Key}=[Table...]\n\n");
        }
    }
}
