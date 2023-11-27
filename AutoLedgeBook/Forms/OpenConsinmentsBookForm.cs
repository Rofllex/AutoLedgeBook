using System.IO;

using AutoLedgeBook.Utils;
using AutoLedgeBook.Models;
using AutoLedgeBook.ViewModels;
using AutoLedgeBook.Data.Abstractions;
using System.Windows.Forms;

namespace AutoLedgeBook.Forms
{
    public partial class OpenConsinmentsBookForm : MetroFramework.Forms.MetroForm
    {

        private readonly OpenConsinmentsBookViewModel _viewModel;


        public OpenConsinmentsBookForm()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            InitializeBindings(_viewModel = new(this));
        }

        public IConsinmentsBook? SelectedConsinmentsBook => _viewModel.SelectedConsinmentsBook;

        public ConsinmentsBookType SelectedBookType => _viewModel.BookType;



        private void InitializeBindings(OpenConsinmentsBookViewModel viewModel)
        {
            canteenRadioButton.Bind(c => c.Checked, viewModel, vm => vm.CanteenButtonSelected);
            canteenRadioButton.Bind(c => c.Enabled, viewModel, vm => vm.LoadBookEnabled);

            ledgeRadioButton.Bind(c => c.Checked, viewModel, vm => vm.LedgeButtonSelected);
            ledgeRadioButton.Bind(c => c.Enabled, viewModel, vm => vm.LoadBookEnabled);

            canteenRadioButton.CheckedChanged += (_, __) => viewModel.BookType = ConsinmentsBookType.Canteen;
            ledgeRadioButton.CheckedChanged += (_, __) => viewModel.BookType = ConsinmentsBookType.Ledge;

            metroProgressSpinner1.Bind(c => c.Value, viewModel, vm => vm.LoadingProgressValue);
            metroProgressBar1.Bind(c => c.Value, viewModel, vm => vm.LoadingProgressValue);

            logLabel.Bind(c => c.Text, viewModel, vm => vm.LogMessage);
            logLabel.Bind(c => c.ForeColor, viewModel, vm => vm.LogMessageColor);

            openConsinmentsBookButton.Click += (_, __) => viewModel.OpenConsinmentsBook();
            openConsinmentsBookButton.Bind(c => c.Enabled, viewModel, vm => vm.LoadBookEnabled);

            recentFilesListBox.Bind(c => c.DataSource, viewModel, vm => vm.RecentFiles);
            recentFilesListBox.Format += RecentFilesListBox_Format;
            recentFilesListBox.DoubleClick += RecentFilesListBox_DoubleClick;

            FormClosed += (_, __) => viewModel.OnCloseForm();
            this.Load += (_, __) => viewModel.Load();
        }

        private void RecentFilesListBox_DoubleClick(object? sender, System.EventArgs e)
        {
            ListBox? listBox = sender as ListBox;
            if (listBox is null)
                return;

            if (listBox.SelectedIndex > -1)
                _viewModel.OpenRecentFile(listBox.SelectedIndex);
        }

        private void RecentFilesListBox_Format(object? sender, System.Windows.Forms.ListControlConvertEventArgs e)
        {
            if (e.Value is RecentConsinmentsBookModel recentFileModel)
            {
                string fileName = Path.GetFileName(recentFileModel.FilePath);
                e.Value = fileName;
            }
        }
    }
}
