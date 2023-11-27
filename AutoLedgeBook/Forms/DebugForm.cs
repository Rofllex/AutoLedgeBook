using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MetroFramework.Forms;
using MetroFramework.Controls;

using AutoLedgeBook.Utils;
using AutoLedgeBook.Models;
using AutoLedgeBook.Logging;
using AutoLedgeBook.ViewModels;
using AutoLedgeBook.Utils.Extensions;


namespace AutoLedgeBook.Forms
{
    public partial class DebugForm : MetroForm
    {
        private readonly DebugFormViewModel _viewModel;

        public DebugForm()
        {
            InitializeComponent();
            
            InitializeBindings(_viewModel = new());
        }
    
        private void InitializeBindings(DebugFormViewModel viewModel)
        {
            logsListBox.Bind(c => c.Enabled, viewModel, vm => vm.LoggingEnabled);
            logsListBox.Bind(c => c.DataSource, viewModel, vm => vm.Logs);
            logsListBox.Format += LogsListBox_Format;
        }

        
        private void LogsListBox_Format(object? sender, ListControlConvertEventArgs e)
        {
            if (e.Value is LogModel logModel)
            {
                string logTypeString = $"[{ logModel.Original.LogType }]";
                e.Value = $"{logTypeString} {logModel.Original.Message}";
            }
        }

        private void DebugForm_Load(object sender, EventArgs e)
        {
            _viewModel.FormLoaded();
        }
    }
}
