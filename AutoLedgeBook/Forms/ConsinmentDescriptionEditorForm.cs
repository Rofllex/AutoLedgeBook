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

using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.ViewModels;
using AutoLedgeBook.Utils;

namespace AutoLedgeBook.Forms
{
    public partial class ConsinmentDescriptionEditorForm : MetroForm
    {
        public static void ShowDialog(IConsinmentNote consinment)
        {
            using ConsinmentDescriptionEditorForm form = new(consinment);
            form.ShowDialog();
        }

        private readonly ConsinmentDescriptionEditorFormViewModel _viewModel;

        public ConsinmentDescriptionEditorForm(IConsinmentNote consinment)
        {
            InitializeComponent();
            InitializeBindings(_viewModel = new(this, consinment));
        }

        private void InitializeBindings(ConsinmentDescriptionEditorFormViewModel viewModel)
        {
            consinmentNumberTextBox.Bind(c => c.Text, viewModel, vm => vm.Number);

            consinmentTypeTextBox.Bind(c => c.Text, viewModel, vm => vm.ConsinmentType);

            consinmentPersonsCountTextBox.Bind(c => c.Text, viewModel, vm => vm.PersonsCount);

            consinmentDestinationTextBox.Bind(c => c.Text, viewModel, vm => vm.Destination);

            confirmButton.Click += (_, __) => _viewModel.Confirm();
        }
    }
}
