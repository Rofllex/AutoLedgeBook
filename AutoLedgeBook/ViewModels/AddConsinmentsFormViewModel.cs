using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;

using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.Excel.LowLevel;
using AutoLedgeBook.Data.ExcelConsinments;
using AutoLedgeBook.Data.ExcelConsinments.Health;
using AutoLedgeBook.Data.ExcelConsinments.Guard;

namespace AutoLedgeBook.ViewModels
{
    public class AddConsinmentsFormViewModel : ViewModel
    {
        private ConsinmentType _selectedConsinmentType = ConsinmentType.Buffet;
        private IReadOnlyConsinmentsBook? _selectedConsinmentsBook;

        private IList _availableDates = new ArrayList();
        private DateOnly? _selectedDate;
        private string _consinmentFileName = string.Empty;

        private bool _selectConsinmentFileEnabled = true;

        private readonly DateOnly[]? _dateRestrictions;
        private readonly Form _callerForm;
        private bool _canConfirm = false;

        private IReadOnlyConsinmentNote? _selectedConsinmentNote;
        
        private int _loadProgressBarValue = 0;


        public AddConsinmentsFormViewModel(Form form, DateOnly[]? dateRestrictions = null)
        {
            _dateRestrictions = dateRestrictions;
            _callerForm = form;

            AvailableConsinmentTypes = new BindingList<ConsinmentType>(Enum.GetValues<ConsinmentType>().ToList());
        }

        public IReadOnlyConsinmentNote? SelectedConsinmentsNote
        {
            get => _selectedConsinmentNote;
            set
            {
                if (value != _selectedConsinmentNote)
                {
                    ChangeProperty(ref _selectedConsinmentNote, value);

                    InvokePropertyChanged(nameof(SelectedConsinmentPersonsCount));
                    InvokePropertyChanged(nameof(SelectedConsinmentTypeString));
                    InvokePropertyChanged(nameof(SelectedConsinmentTotalPcs));
                    InvokePropertyChanged(nameof(SelectedConsinmentTotalWeight));
                }
            }
        }

        public ConsinmentType SelectedConsinmentType
        {
            get => _selectedConsinmentType;
            set
            {
                if (_selectedConsinmentType != value)
                    ChangeProperty(ref _selectedConsinmentType, value);
            }
        }

        public int LoadProgressBarValue
        {
            get => _loadProgressBarValue;
            set => ChangeProperty(ref _loadProgressBarValue, value);
        }

        public IList AvailableDates
        {
            get => _availableDates;
            set => ChangeProperty(ref _availableDates, value);
        }

        public DateOnly? SelectedDate
        {
            get => _selectedDate;
            set
            {
                ChangeProperty(ref _selectedDate, value);

                if (_selectedConsinmentsBook is not null && value.HasValue)
                    SelectedConsinmentsNote = _selectedConsinmentsBook.GetConsinmentsByDate(value.Value).First();
                else
                    SelectedConsinmentsNote = null;
            }
        }

        public string ConsinmentFileName
        {
            get => _consinmentFileName;
            set => ChangeProperty(ref _consinmentFileName, value);
        }

        public bool CanSelectConsinmentFile
        {
            get => _selectConsinmentFileEnabled;
            private set => ChangeProperty(ref _selectConsinmentFileEnabled, value);
        }

        public bool CanConfirm
        {
            get => _canConfirm;
            set => ChangeProperty(ref _canConfirm, value);
        }

        public double SelectedConsinmentTotalWeight
            => SelectedConsinmentsNote?.GetTotalProductsWeight() ?? default;

        public double SelectedConsinmentTotalPcs
            => SelectedConsinmentsNote?.GetTotalProductsPcs() ?? default;

        public int SelectedConsinmentPersonsCount
            => SelectedConsinmentsNote?.Description.PersonsCount ?? 0;

        public string SelectedConsinmentTypeString
            => SelectedConsinmentsNote?.Description.Type ?? string.Empty;

        public BindingList<ConsinmentType> AvailableConsinmentTypes { get; }

        public void SelectConsinment()
        {
            if (!CanSelectConsinmentFile)
                return;

            using OpenFileDialog openFileDialog = new()
            {
                Title = "Выбор накладных",
                CheckFileExists = true,
                CheckPathExists = true,
                ReadOnlyChecked = true,
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            SelectConsinment(openFileDialog.FileName);
        }

        public void SelectConsinment(string filePath)
        {
            if (!CanSelectConsinmentFile)
                return;

            CancellationTokenSource loadProgressBarTaskCts = new();
            Task loadProgressValueTask = LoadProgressBarTask(loadProgressBarTaskCts.Token);
            
            Task.Run(() =>
            {
                return BuildConsinmentNote(filePath, SelectedConsinmentType, ExcelApplicationContext.SingleInstance);
            }).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    if (t.Exception!.InnerExceptions.Count == 1)
                        MessageBox.Show("Ошибка при загрузке накладных\n" + t.Exception!.InnerExceptions[0], "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("Ошибка при загрузке накладных\n" + t.Exception!, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    loadProgressBarTaskCts.Cancel();
                    LoadProgressBarValue = 0;
                    return;
                }

                loadProgressBarTaskCts.Cancel();
               
                LoadProgressBarValue = 100;

                _selectedConsinmentsBook = t.Result;

                ConsinmentFileName = System.IO.Path.GetFileName(filePath);
                CanSelectConsinmentFile = false;

                DateOnly[] availableDates;
                if (_dateRestrictions is null || _dateRestrictions.Length == 0)
                    availableDates = _selectedConsinmentsBook.Dates.ToArray();
                else
                    availableDates = _selectedConsinmentsBook.Dates.Intersect(_dateRestrictions).ToArray();

                if (availableDates.Length == 0)
                {
                    MessageBox.Show("Выбранная книга накладных не сопоставима по дате");
                    _selectedConsinmentsBook.Dispose();
                    CanSelectConsinmentFile = true;
                    return;
                }

                ArrayList availableDatesList = new ArrayList(_selectedConsinmentsBook.Dates.Count);
                foreach (DateOnly availableDate in availableDates)
                    availableDatesList.Add(availableDate);
                AvailableDates = availableDatesList;
                SelectedDate = _selectedConsinmentsBook.Dates.Max();
                CanConfirm = true;
                CanSelectConsinmentFile = false;
            });
        }

        public void ConfirmButtonClicked()
        {
            if (!SelectedDate.HasValue)
            {
                MessageBox.Show($"Ошибка приложения...\n{ this.GetType().FullName}.{nameof(ConfirmButtonClicked)} не выбрана дата...");
                return;
            }

            if (_selectedConsinmentsBook is null)
            {
                MessageBox.Show("Ошибка приложения... \n - Не загружена книга накладных");
                return;
            }

            this.SelectedConsinmentsNote = _selectedConsinmentsBook.GetConsinmentsByDate(SelectedDate!.Value).First();

            _callerForm.DialogResult = DialogResult.OK;
            _callerForm.Close();
        }

        public string FormatConsinmentType(ConsinmentType? consType)
        {
            if (consType is null)
                return string.Empty;
#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
            return consType switch
            {
                ConsinmentType.Buffet => "ШС",
                ConsinmentType.Health => "Госпиталь, мед. рота.",
                ConsinmentType.SUC => "СУЦ",
                ConsinmentType.Guard => "Караул",
                _ => consType.ToString()
            };
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
        }

        private IReadOnlyConsinmentsBook BuildConsinmentNote(string filePath, ConsinmentType consinmentType, ExcelApplicationContext xlAppContext)
        {
            return consinmentType switch
            {
                ConsinmentType.Buffet => ExcelBuffetDocumentConsinmentBook.FromFile(filePath, ref xlAppContext),
                ConsinmentType.Health => ExcelHealthDocumentConsinmentBook.FromFile(filePath, xlAppContext),
                ConsinmentType.SUC => ExcelSUCDocumentConsinmentBook.FromFile(filePath, ref xlAppContext),
                ConsinmentType.Guard => ExcelGuardConsinmentsBook.FromFile(filePath, xlAppContext),
                _ => throw new InvalidOperationException($"Тип накладной {consinmentType} неизвестен для фабрики..")
            };
        }

        private async Task LoadProgressBarTask(CancellationToken ct)
        {

            while (LoadProgressBarValue < 80 && !ct.IsCancellationRequested)
            {
                LoadProgressBarValue += 1;

                try
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(25), ct);
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerExceptions.Count == 1 && ae.InnerExceptions[0] is TaskCanceledException)
                        continue;
                }
                catch (TaskCanceledException)
                {

                }
            }
        }
    }
}
