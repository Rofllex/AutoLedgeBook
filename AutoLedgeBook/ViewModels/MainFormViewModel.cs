using System;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using AutoLedgeBook.Forms;
using AutoLedgeBook.Models;
using AutoLedgeBook.Logging;
using AutoLedgeBook.Utils.Matches;
using AutoLedgeBook.Data.CanteenBook;
using AutoLedgeBook.Data.Abstractions;

namespace AutoLedgeBook.ViewModels;

public class MainFormViewModel : ViewModel
{
    private enum MainFormState
    {
        WaitConsinments,
        FillBook,
    }

    private bool _formVisible = false;

    private IConsinmentsBook? _consinmentsBook;

    private string _bookMonthString = string.Empty;

    private ConsinmentsBookType _bookType;

    private int _fillBooksProgress = 0;

    private Task? _fillBookTask = null;

    private string _matchesFileName = "Наименование файла";

    private IMatchesCollection<string, string>? _matches;
    private IMatchesCollection<string, string>? Matches
    {
        get => _matches;
        set => ChangeProperty(ref _matches, value);

    }

    private readonly Form _callerForm;

    private readonly ILogger _logger;

    public MainFormViewModel(Form callerForm, ILogger logger)
    {
        Debug.Assert(callerForm is not null);
        _callerForm = callerForm;

        Debug.Assert(logger is not null);
        _logger = logger;
    }

    /// <summary>
    ///     Флаг указывающий, должен ли пользователь видеть форму в данный момент.
    /// </summary>
    public bool FormVisible
    {
        get => _formVisible;
        private set => ChangeProperty(ref _formVisible, value);
    }

    /// <summary>
    ///     Флаг указывающий что на данный момент книга заполняется.
    /// </summary>
    public bool CanFillBook => _matches is not null
                               && ConsinmentsToAdd.Count > 0
                               && !AlreadyFilling;

    /// <summary>
    ///     Флаг указывающий что на данный момент идёт заполнение книги.
    /// </summary>
    public bool AlreadyFilling => _fillBookTask is not null && !_fillBookTask.IsCompleted;

    public bool CanAddConsinment => _matches is not null && !AlreadyFilling;


    public string BookMonthString
    {
        get => _bookMonthString;
        private set => ChangeProperty(ref _bookMonthString, value);
    }

    public ConsinmentsBookType BookType
    {
        get => _bookType;
        private set => ChangeProperty(ref _bookType, value);
    }

    /// <summary>
    ///     Если можно выбрать сопоставления.
    /// </summary>
    public bool CanSelectMatches => _matches is null || ConsinmentsToAdd.Count == 0;

    /// <summary>
    ///     Название файла с сопоставлениями
    /// </summary>
    public string MatchesFileName
    {
        get => _matchesFileName;
        private set => ChangeProperty(ref _matchesFileName, value);
    }

    /// <summary>
    ///     Накладные к добавлению
    /// </summary>
    public BindingList<ConsinmentToAddModel> ConsinmentsToAdd { get; } = new();

    /// <summary>
    ///     Минимальное значение для заполнения прогресс бара
    /// </summary>
    public int FillBooksProgressMinValue => 0;

    /// <summary>
    ///     Максимальное значение для заполнения прогресс браа
    /// </summary>
    public int FillBooksProgressMaxValue => 100;

    /// <summary>
    ///     Текущее значение прогресс бара заполнения накладных.
    /// </summary>
    public int FillBooksProgress
    {
        get => _fillBooksProgress;
        private set => ChangeProperty(ref _fillBooksProgress, value);
    }

    public bool CanOpenMatchesForm => Matches is IMatchesList<string, string>;


    public void GridViewDataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        var l = _logger;

        l.Error($"Произошла ошибка при интерпретировании данных в таблице grid view! " +
            $"Ex: {e.ThrowException}" +
            $"Col index: {e.ColumnIndex}" +
            $"Row index: {e.RowIndex}" +
            $"Context: {e.Context}");

        if (e.ThrowException)
            l.Error(e.Exception);

    }

    public void FormLoad()
    {
        using OpenConsinmentsBookForm openConsinmentsForm = new();
        if (openConsinmentsForm.ShowDialog() == DialogResult.OK)
        {
            _consinmentsBook = openConsinmentsForm.SelectedConsinmentsBook!;

            BookType = openConsinmentsForm.SelectedBookType;
            BookMonthString = _consinmentsBook.Dates.First().ToString("MMMM");
            FormVisible = true;
        }
        else
            Application.Exit();
    }

    public void AddConsinment()
    {
        try
        {
            _logger.Info($"{nameof(MainFormViewModel)}.{nameof(AddConsinment)}");

            using AddConsinmentForm consinmentsForm = new(_consinmentsBook!.Dates.ToArray());
            if (consinmentsForm.ShowDialog() == DialogResult.OK)
            {
                IReadOnlyConsinmentNote sourceConsinmentNote = consinmentsForm.SelectedConsinmentsNote!;
                Debug.Assert(sourceConsinmentNote is not null);

                if (ConsinmentsToAdd.FirstOrDefault(c => c.ConsinmentNumber == sourceConsinmentNote.Number) is not null)
                {
                    MessageBox.Show($"Накладная с номером \"{sourceConsinmentNote.Number}\" уже есть в коллекции к добавлению...");
                    return;
                }

                string[] destinationProductNames = _consinmentsBook!.GetProductNames();
                string[] sourceProductNames = sourceConsinmentNote.Products.GetProductNames();

                try
                {
                    _ = DefaultProductMatcher.Match(destinationProductNames, sourceProductNames, Matches!);
                }
                catch (AggregateException ae)
                {
                    StringBuilder errorMessageBuilder = new();
                    ae.Handle(ex =>
                    {
                        if (ex is not CompareMatchException cme)
                            return false;
                        errorMessageBuilder.AppendLine($"   -{cme.Message}");
                        return true;
                    });

                    MessageBox.Show("Ошибка сопоставления\n" + errorMessageBuilder, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _logger.Warning("Ошибка сопоставления...\n" + errorMessageBuilder);

                    return;
                }
                catch (Exception ex)
                {
                    _logger.Error("Произошла неизвестная ошибка при сопоставлении");
                    _logger.Error(ex);

                    MessageBox.Show("Неизвестная ошибка\n" + ex);
                    return;
                }

                IConsinmentsCollection dailyConsinmentsCollection = _consinmentsBook.GetConsinmentsByDate(sourceConsinmentNote.Day);

                IConsinmentNote? destinationConsinment = CreateConsinment(dailyConsinmentsCollection, sourceConsinmentNote.Number, sourceConsinmentNote.Description.Type, sourceConsinmentNote.Description.Destination, sourceConsinmentNote.Description.PersonsCount, false);

                if (destinationConsinment is null)
                {
                    return;
                }

                ConsinmentToAddModel consinmentToAddModel = new(destinationConsinment, sourceConsinmentNote);
                ConsinmentsToAdd.Add(consinmentToAddModel);
            }
            else
            {
                _logger.Info("Отмена выбора файла с накладными");
            }
        }
        finally
        {
            InvokePropertyChanged(nameof(CanAddConsinment));
        }
    }

    public void SelectMatches()
    {
        if (!CanSelectMatches)
            return;

        using OpenFileDialog openFileDialog = new()
        {
            Filter = "Сопоставления|*.xlsx",
            Title = "Выбор сопоставлений",
            Multiselect = false,
            CheckFileExists = true,
            CheckPathExists = true,
            ReadOnlyChecked = true
        };

        if (openFileDialog.ShowDialog() != DialogResult.OK)
            return;

        SelectMatches(openFileDialog.FileName);
    }

    public void SelectMatches(string matchesFilePath)
    {
        try
        {
            Matches = AutoLedgeBook.Matches.ExcelProductMatchCollection.FromFile(matchesFilePath);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось загрузить сопоставления\n" + ex.ToString());
            return;
        }

        MatchesFileName = System.IO.Path.GetFileName(matchesFilePath);
    }

    public bool CheckMatchesPath(string matchesFilePath)
    {
        if (string.IsNullOrWhiteSpace(matchesFilePath))
            return false;

        string fileExtension = System.IO.Path.GetExtension(matchesFilePath)!;
        return fileExtension.IndexOf("xl") > -1;
    }

    public void FillBook()
    {
        Debug.Assert(_matches is not null);

        _fillBookTask = Task.Run(() =>
        {
            double totalConsinments = ConsinmentsToAdd.Count;
            double filledCount = 0;

            bool successFilled = false;

            for (var i = 0; i < ConsinmentsToAdd.Count; i++)
            {
                ConsinmentToAddModel currentModel = ConsinmentsToAdd[i];

                try
                {
                    FillConsinment(
                        destinationConsinment: currentModel.OriginalConsinment,
                        sourceConsinment: currentModel.SourceConsinment,
                        matches: _matches!);
                    successFilled = true;
                    ConsinmentsToAdd.Remove(currentModel);
                    i--;
                }
                catch (Exception ex)
                {
                    currentModel.ErrorOccured = true;
                    currentModel.ErrorMessage = ex.ToString();
                    continue;
                }
                finally
                {
                    filledCount++;

                    double filledPercentage = filledCount / totalConsinments;
                    FillBooksProgress = Convert.ToInt32(filledPercentage);
                }
            }


            FillBooksProgress = 100;

            if (successFilled && _consinmentsBook is ISaveable saveableBook
                && MessageBox.Show("Сохранить книгу?", "Подтверждение действия", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    saveableBook.Save();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения книги...\n" + ex);
                }
            }
        });


        _fillBookTask.ContinueWith(_ =>
        {
            _fillBookTask = null;
            InvokePropertyChanged(nameof(CanAddConsinment));
        });
    }

    public void ClearAllConsinments() => ConsinmentsToAdd.Clear();

    public void OpenMatchesForm()
    {
        if (CanOpenMatchesForm && Matches is IMatchesList<string, string> matchesList)
        {
            Debug.Assert(_consinmentsBook is not null, "Попытка открытия редактора сопоставлений, когда экземпляр книги был Null!");

            using MatchesEditorForm matchesEditor = new(matchesList, _consinmentsBook.GetProductNames());
            matchesEditor.ShowDialog();
        }
    }

    private IConsinmentNote? CreateConsinment(IConsinmentsCollection consinmentsCollection, string number, string type, string destination, int personsCount, bool postEditConsinmentInfoByUser = true)
    {
        IConsinmentNote buildedConsinment;

        if (consinmentsCollection is ExcelCanteenConsinmentsCollection canteenConsinments)
        {
            _logger.Info("Коллекция накладных определена как книга столовой.");

            if (consinmentsCollection.Contains(number))
            {
                _logger.Info($"Накладная с номером \"{number}\" уже присутствует в книге. Спрашиваю пользователя о перезаписи.");
                if (MessageBox.Show($"Накладная с номером \"{number}\" уже присутствует. Перезаписать?", "Подтверждение действия", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                {
                    _logger.Info($"Пользователь отказался.");
                    return default;
                }

                _logger.Info($"Пользователь согласился. Начинаю удаление старой накладной");
                consinmentsCollection.Delete(number);
                _logger.Info($"Накладная перезаписана");
            }

            ExcelCanteenConsinment? buildedCanteenConsinment = BuildCanteenConsinment(canteenConsinments, number, type, destination, personsCount);
            if (buildedCanteenConsinment is null)
                return default;
            
            buildedConsinment = buildedCanteenConsinment;
        }
        else if (consinmentsCollection.Contains(number))
        {
            if (MessageBox.Show($"Накладная с номером \"{number}\" уже присутствует. Перезаписать?", "Подтверждение действия", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return default;

            consinmentsCollection.Delete(number);
            buildedConsinment = consinmentsCollection.Add(number);
        }
        else if (((IReadOnlyCollection<IConsinmentNote>)consinmentsCollection).Count == consinmentsCollection.MaxCount)
        {
            IConsinmentNote? consinmentToOverwrite = ChooseConsinmentNoteForm<IConsinmentNote>.ShowDialog(consinmentsCollection);
            if (consinmentToOverwrite is null)
                return default;

            buildedConsinment = consinmentToOverwrite;
        }
        else
        {
            buildedConsinment = consinmentsCollection.Add(number);
        }

        buildedConsinment.Description.Type = type;
        buildedConsinment.Description.Destination = destination;
        buildedConsinment.Description.PersonsCount = personsCount;

        if (postEditConsinmentInfoByUser)
            ConsinmentDescriptionEditorForm.ShowDialog(buildedConsinment);

        return buildedConsinment;
    }

    private IConsinmentNote? CreateConsinment(IConsinmentsCollection consinmentsCollection, string number, IReadOnlyConsinmentDescription otherDescription) => CreateConsinment(consinmentsCollection, number, otherDescription.Type, otherDescription.Destination, otherDescription.PersonsCount);

    private ExcelCanteenConsinment? BuildCanteenConsinment(ExcelCanteenConsinmentsCollection canteenConsinments, string number, string type, string destination, int personsCount)
    {
        ExcelCanteenConsinment? selectedCanteenConsinment = null;

        _callerForm.Invoke(() =>
        {
            selectedCanteenConsinment = ChooseConsinmentNoteForm<IConsinmentNote>.ShowDialog(canteenConsinments) as ExcelCanteenConsinment;
        });


        if (selectedCanteenConsinment is null)
            return default;

        if (!selectedCanteenConsinment.CanBuild)
            selectedCanteenConsinment.Delete();

        return selectedCanteenConsinment.GetBuilder()
                                        .SetConsinmentNumber(number)
                                        .SetType(type)
                                        .SetDestination(destination)
                                        .SetPersonsCount(personsCount)
                                        .Build();
    }

    private void FillConsinment(IConsinmentNote destinationConsinment, IReadOnlyConsinmentNote sourceConsinment, IMatchesCollection<string, string> matches)
    {
        IMatchesCollection<IAccountingProduct, IReadOnlyAccountingProduct> matchedProducs = DefaultProductMatcher.Match(destinationConsinment.Products, sourceConsinment.Products, matches);
        foreach (var match in matchedProducs)
            match.Key.Value = match.Values.Sum(c => c.Value);
    }
}
