using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

using MetroFramework.Forms;
using MetroFramework.Controls;

using AutoLedgeBook.Utils;
using AutoLedgeBook.ViewModels;

namespace AutoLedgeBook.Forms;

public partial class MainForm : MetroForm
{
    private readonly MainFormViewModel _viewModel;

    public MainForm()
    {
        InitializeComponent();

        Icon = Resources.Garbage;

        CheckForIllegalCrossThreadCalls = false;

        InitializeBindings(_viewModel = new(this, Logging.Logger.Instance));

#if DEBUG
        System.Threading.Thread debugFormThread = new(() =>
        {
            new DebugForm().ShowDialog();
        });
        debugFormThread.Start();
#endif
    }

    private void InitializeBindings(MainFormViewModel viewModel)
    {
        this.Load += (_, __) => viewModel.FormLoad();
        this.Bind(c => c.Visible, viewModel, vm => vm.FormVisible);
        this.MouseClick += MainForm_MouseClick;

        viewModel.PropertyChanged += (object? s, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(_viewModel.FormVisible))
            {
                if (_viewModel.FormVisible)
                    this.Show();
                else
                    this.Hide();
            }
        };

        ledgeBookTypeLabel.Bind(c => c.Text, viewModel, vm => vm.BookType).Format += (s, e) =>
        {
            if (e.Value is ConsinmentsBookType cbt)
            {
                e.Value = cbt switch
                {
                    ConsinmentsBookType.Canteen => "Книга столовой",
                    ConsinmentsBookType.Ledge => "Книга склада",
                    _ => "unknown book type"
                };
            }
            else if (e.Value is null)
            {
                e.Value = "{null}";
            }
        };

        

        ledgeBookMonthLabel.Bind(c => c.Text, viewModel, vm => vm.BookMonthString); 

        selectMatchesButton.Bind(c => c.Enabled, viewModel, vm => vm.CanSelectMatches);
        selectMatchesButton.Click += (_, __) => viewModel.SelectMatches();

        matchesPanel.DragEnter += MatchesPanel_DragEnter;
        matchesPanel.DragDrop += MatchesPanel_DragDrop;
        
        matchesFileNameLabel.Bind(c => c.Text, viewModel, vm => vm.MatchesFileName);

        addConsinmentButton.Click += (_, __) => viewModel.AddConsinment();
        addConsinmentButton.Bind(c => c.Enabled, viewModel, vm => vm.CanAddConsinment);

        consinmentsToAddGrid.Bind(c => c.DataSource, viewModel, vm => vm.ConsinmentsToAdd);
        consinmentsToAddGrid.CellFormatting += ConsinmentsToAddGrid_CellFormatting;
        consinmentsToAddGrid.MouseClick += ConsinmentsToAddGrid_MouseClick;
        consinmentsToAddGrid.RowPrePaint += ConsinmentsToAddGrid_RowPrePaint;
        consinmentsToAddGrid.DataError += _viewModel.GridViewDataError;

        fillConsinmentsProgressBar.Bind(c => c.Value, viewModel, vm => vm.FillBooksProgress);
    }

    private void MainForm_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
            return;

        metroContextMenu1.Items.Clear();

#if DEBUG
        // TODO: Добавить иконку для редактора сопоставлений в контекстное меню
        metroContextMenu1.Items.Add("Редактор сопоставлений", null, (_, __) => _viewModel.OpenMatchesForm());
        metroContextMenu1.Show(MousePosition.X, MousePosition.Y);
#endif
    }

    private void ConsinmentsToAddGrid_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        Models.ConsinmentToAddModel? consinmentModel = _viewModel.ConsinmentsToAdd[e.RowIndex];

        MetroGrid metroGrid = (MetroGrid)sender!;
        Color backColor;
        Color selectionBackColor;
        if (consinmentModel.ErrorOccured)
        {
            backColor = Color.Red;
            selectionBackColor = Color.LightPink;
        }
        else
        {
            backColor = metroGrid.DefaultCellStyle.BackColor;
            selectionBackColor = metroGrid.DefaultCellStyle.SelectionBackColor;
        }

        DataGridViewRow? row = metroGrid.Rows[e.RowIndex];
        row.DefaultCellStyle.BackColor = backColor;
        row.DefaultCellStyle.SelectionBackColor = selectionBackColor;
    }

    private void MatchesPanel_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data!.GetDataPresent(DataFormats.FileDrop, false) && _viewModel.CanSelectMatches)
        {
            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (fileNames.Length == 1)
            {
                if (_viewModel.CheckMatchesPath(fileNames[0]))
                {
                    _viewModel.SelectMatches(fileNames[0]);
                    return;
                }
            }
        }
        e.Effect = DragDropEffects.None;
    }

    private void MatchesPanel_DragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data!.GetDataPresent(DataFormats.FileDrop, false) && _viewModel.CanSelectMatches)
        {
            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (fileNames.Length == 1 && _viewModel.CheckMatchesPath(fileNames[0]))
            {
                e.Effect = DragDropEffects.Link;
                return;
            }
        }

        e.Effect = DragDropEffects.None;
    }

    private void ConsinmentsToAddGrid_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
            return;

        metroContextMenu1.Items.Clear();

        if (_viewModel.CanFillBook)
            metroContextMenu1.Items.Add("Заполнить все", null, (_, __) => _viewModel.FillBook());
        
        if (_viewModel.ConsinmentsToAdd.Count > 0)
            metroContextMenu1.Items.Add("Очистить все", null, (_, __) => _viewModel.ClearAllConsinments());
        
        if (metroContextMenu1.Items.Count > 0)
            metroContextMenu1.Show(MousePosition);
    }

    private void ConsinmentsToAddGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.Value is double doubleValue && Math.Ceiling(doubleValue) != doubleValue)
            e.Value = doubleValue.ToString("f2");
    }
}