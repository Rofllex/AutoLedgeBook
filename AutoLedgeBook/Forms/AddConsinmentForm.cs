using System;
using System.Windows.Forms;
using System.ComponentModel;

using MetroFramework.Forms;
using MetroFramework.Controls;

using AutoLedgeBook.Utils;
using AutoLedgeBook.ViewModels;
using AutoLedgeBook.Data.Abstractions;

namespace AutoLedgeBook.Forms
{
    public partial class AddConsinmentForm : MetroForm
    {
        private readonly AddConsinmentsFormViewModel _viewModel;

        public AddConsinmentForm(DateOnly[]? dateRestrictions = null)
        {
            InitializeComponent();
                        
            CheckForIllegalCrossThreadCalls = false;
            InitializeBindings(_viewModel = new(this, dateRestrictions));
        }

        

        public IReadOnlyConsinmentNote? SelectedConsinmentsNote => _viewModel.SelectedConsinmentsNote;

        public ConsinmentType SelectedConsinmentType => _viewModel.SelectedConsinmentType;


        private void InitializeBindings(AddConsinmentsFormViewModel viewModel)
        {
            consinmentTypeComboBox.Bind(c => c.DataSource, viewModel, vm => vm.AvailableConsinmentTypes);
            consinmentTypeComboBox.Bind(c => c.SelectedItem, viewModel, vm => vm.SelectedConsinmentType);
            consinmentTypeComboBox.SelectedIndexChanged += (s, e) => 
            {
                MetroComboBox comboBox = (MetroComboBox)s!;
                _viewModel.SelectedConsinmentType = (ConsinmentType)comboBox!.SelectedItem;
            };
            consinmentTypeComboBox.Format += (s, e) => 
            {
                if (e.Value is ConsinmentType consType)
                    e.Value = _viewModel.FormatConsinmentType(consType);
            };                        


            selectConsinmentDateComboBox.Bind(c => c.DataSource, viewModel, vm => vm.AvailableDates);
            selectConsinmentDateComboBox.Bind(c => c.SelectedItem, viewModel, vm => vm.SelectedDate);
            selectConsinmentDateComboBox.SelectedIndexChanged += (sender, e) => 
            {
                MetroComboBox callerComboBox = (MetroComboBox)sender!;
                if (callerComboBox.SelectedIndex > -1)
                    viewModel.SelectedDate = (DateOnly)callerComboBox.SelectedItem;
                else
                    viewModel.SelectedDate = null;
            };

            selectConsinmentFileButton.Click += (_, __) => viewModel.SelectConsinment();
            selectConsinmentFileButton.Bind(c => c.Enabled, viewModel, vm => vm.CanSelectConsinmentFile);

            selectConsinmentFileNameLabel.Bind(c => c.Text, viewModel, vm => vm.ConsinmentFileName);

            confirmConsinmentButton.Bind(c => c.Enabled, viewModel, vm => vm.CanConfirm);
            confirmConsinmentButton.Click += (_, __) => viewModel.ConfirmButtonClicked();

            WeightFormatter(selectedConsinmentSummaryPcsLabel.Bind(c => c.Text, viewModel, vm => vm.SelectedConsinmentTotalPcs));

            WeightFormatter(selectedConsinmentSummaryWeightLabel.Bind(c => c.Text, viewModel, vm => vm.SelectedConsinmentTotalWeight));


            selectedConsinmentTypeLabel.Bind(c => c.Text, viewModel, vm => vm.SelectedConsinmentTypeString);
            //ConsinmentTypeFormatter(selectedConsinmentTypeLabel.Bind(c => c.Text, viewModel, vm => vm.SelectedConsinmentType));

            PersonsCountValueFormatter(selectedConsinmentPersonsCountLabel.Bind(c => c.Text, viewModel, vm => vm.SelectedConsinmentPersonsCount));

            consinmentsBookLoadProgressBar.Bind(c => c.Value, viewModel, vm => vm.LoadProgressBarValue);

            this.DragDrop += AddConsinmentForm_DragDrop;
            this.DragEnter += AddConsinmentForm_DragEnter;
        }

        private void AddConsinmentForm_DragEnter(object? sender, DragEventArgs e)
        {
            if (CheckDrop(e) && _viewModel.CanSelectConsinmentFile)
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }

        private void AddConsinmentForm_DragDrop(object? sender, DragEventArgs e)
        {
            if (CheckDrop(e) && _viewModel.CanSelectConsinmentFile)
            {
                string fileDrop = ((string[])e.Data!.GetData(DataFormats.FileDrop, false))[0];
                _viewModel.SelectConsinment(fileDrop);
            }
        }

        private bool CheckDrop(DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) ?? false)
            {
                string[] fileDrops = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                if (fileDrops.Length == 1 && fileDrops[0].IndexOf(".xl") > -1)
                {
                    e.Effect = DragDropEffects.Link;
                    return true;
                }
            }

            return false;

        }

        private void PersonsCountValueFormatter(Binding bind)
        {
            bind.Format += (sender, e) => 
            {
                int personsCount;

                try
                {
                    personsCount = Convert.ToInt32(e.Value);
                }
                catch { return; }

                e.Value = $"{personsCount} чел.";
            };
        }

        private void WeightFormatter(Binding bind)
        {
            bind.Format += (sender, e) => 
            {
                if (e.Value is double doubleValue && Math.Ceiling(doubleValue) != doubleValue)
                {
                    e.Value = doubleValue.ToString("f4");
                }
            };
        }
    
        private void ConsinmentTypeFormatter(Binding bind)
        {
            bind.Format += (sender, e) => 
            {
                ConsinmentType? consinmentType = e.Value as ConsinmentType?;
                if (consinmentType.HasValue)
                {
                    e.Value = consinmentType switch
                    {
                        ConsinmentType.Buffet => "Шведский стол",
                        ConsinmentType.Health => "Госпиталь, мед. рота",
                        ConsinmentType.SUC => "СУЦ, поля",
                        _ => e.Value
                    };
                }
            };
        }
    }
}
