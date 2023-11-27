using System.Collections.Generic;

using MetroFramework.Forms;

using AutoLedgeBook.ViewModels;
using AutoLedgeBook.Utils;

namespace AutoLedgeBook.Forms
{
    public partial class ChooseConsinmentNoteForm<TConsinment> : MetroForm 
        where TConsinment : Data.Abstractions.IReadOnlyConsinmentNote
    {
        public static TConsinment? ShowDialog(IReadOnlyCollection<TConsinment> consinments)
        {
            var form = new ChooseConsinmentNoteForm<TConsinment>(consinments);
            if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return default(TConsinment);
            return form.SelectedConsinment;
        }
        
        
        private readonly ChooseConsinmentNoteViewModel<TConsinment> _viewModel;

        public ChooseConsinmentNoteForm(IReadOnlyCollection<TConsinment> consinments)
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            InitializeBindings(_viewModel = new(this, consinments));
        }

        public TConsinment? SelectedConsinment => _viewModel.ResultSelectedConsinment;

        private void InitializeBindings(ChooseConsinmentNoteViewModel<TConsinment> viewModel)
        {
            consinmentsGrid.Bind(c => c.DataSource, viewModel, vm => vm.Consinments);
            
            loadConsinmentsProgressSpinner.Bind(c => c.Visible, viewModel, vm => vm.LoadingProgress);

            Load += (_, __) => viewModel.FormLoaded();
        }

        private void consinmentsGrid_CellDoubleClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            int selectedRowIndex = e.RowIndex;
            if (selectedRowIndex < 0)
                return;

            _viewModel.SelectConsinment(_viewModel.Consinments[selectedRowIndex]);
        }
    }
}
