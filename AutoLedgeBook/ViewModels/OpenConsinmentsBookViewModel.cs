namespace AutoLedgeBook.ViewModels;

using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using AutoLedgeBook.Models;
using AutoLedgeBook.Logging;
using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Data.CanteenBook;
using AutoLedgeBook.Data.StorageBook;

using Newtonsoft.Json;

public class OpenConsinmentsBookViewModel : ViewModel
{
    private ConsinmentsBookType _bookType = ConsinmentsBookType.Ledge;
    
    private Task<IConsinmentsBook>? _loadBookTask;
    private int _loadingProgressValue = 0;
    private bool _loadBookEnabled = true;

    private string _logMessage = string.Empty;
    private Color _logMessageColor;

    private BindingList<RecentConsinmentsBookModel> _recentFiles = new BindingList<RecentConsinmentsBookModel>();

    private readonly ILogger _logger = Logger.Instance;
    private readonly Form _callerForm;


    public OpenConsinmentsBookViewModel(Form callerForm)
    {
        _callerForm = callerForm;
    }


    public IConsinmentsBook? SelectedConsinmentsBook { get; private set; }

    public ConsinmentsBookType BookType
    {
        get => _bookType;
        set
        {
            if (!Enum.IsDefined(value))
                throw new InvalidOperationException();

            if (_bookType != value)
                ChangeProperty(ref _bookType, value);
        }
    }

    public bool CanteenButtonSelected
    {
        get => BookType == ConsinmentsBookType.Canteen;
        set
        {
            BookType = ConsinmentsBookType.Canteen;
        }
    }

    public bool LedgeButtonSelected
    {
        get => BookType == ConsinmentsBookType.Ledge;
        set
        {
            BookType = ConsinmentsBookType.Ledge;
        }
    }

    public string LogMessage
    {
        get => _logMessage;
        set => ChangeProperty(ref _logMessage, value);
    }

    public Color LogMessageColor
    {
        get => _logMessageColor;
        private set => ChangeProperty(ref _logMessageColor, value);
    }

    public int LoadingProgressValue
    {
        get => _loadingProgressValue;
        set => ChangeProperty(ref _loadingProgressValue, value);
    }

    public bool LoadBookEnabled
    {
        get => _loadBookEnabled && CanOpenBook;
        private set => ChangeProperty(ref _loadBookEnabled, value);
    }

    public bool CanOpenBook => _loadBookTask is null || _loadBookTask.IsCompleted;

    public BindingList<RecentConsinmentsBookModel> RecentFiles
    {
        get => _recentFiles;
        private set
        {
            if (!ReferenceEquals(_recentFiles, value))
            {
                ChangeProperty(ref _recentFiles, value);
            }
        }
    }



    public void Load()
    {
        var recentFiles = LoadRecentFiles();
        recentFiles.Sort((a, b) =>
        {
            return a.LastOpened > b.LastOpened ? 1 : -1;
        });

        RecentFiles = new BindingList<RecentConsinmentsBookModel>(recentFiles.Select(b => new RecentConsinmentsBookModel(b)).ToList());
    }

    public void OpenConsinmentsBook()
    {
        if (!_loadBookTask?.IsCompleted ?? false)
            return;

        string filePath;

        using (OpenFileDialog openFileDialog = new())
        {
            string bookName = GetConsinmentsBookName(BookType);

            openFileDialog.Title = bookName;
            openFileDialog.Filter = $"{bookName}|*.xlsx";
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            filePath = openFileDialog.FileName;
        }

        OpenConsinmentsBook(filePath, BookType);
    }

    public void OpenRecentFile(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= RecentFiles.Count)
            return;

        RecentConsinmentsBook recentFile = RecentFiles[selectedIndex].Origin;
        OpenConsinmentsBook(recentFile);
    }

    public void OnCloseForm()
    {
        if (SelectedConsinmentsBook is not null)
        {
            _callerForm.DialogResult = DialogResult.OK;
            SaveRecentFiles(RecentFiles.Select(rf => rf.Origin));
        }
    }



    private Task<IConsinmentsBook> LoadConsinmentsBook(string filePath, ConsinmentsBookType bookType)
    {
        return Task.Run(() =>
        {
            var xlAppContext = Data.Excel.LowLevel.ExcelApplicationContext.SingleInstance;
            return bookType switch
            {
                ConsinmentsBookType.Canteen => (IConsinmentsBook)ExcelCanteenBook.FromFile(filePath, xlAppContext),
                ConsinmentsBookType.Ledge => ExcelStorageBook.FromFile(filePath, xlAppContext),
                _ => throw new InvalidOperationException()
            };
        });
    }

    private void OpenConsinmentsBook(RecentConsinmentsBook recentFile) => OpenConsinmentsBook(recentFile.FilePath, recentFile.BookType);

    private void OpenConsinmentsBook(string filePath, ConsinmentsBookType bookType)
    {
        if (!_loadBookTask?.IsCompleted ?? false)
            return;
        SetLogMessage("Загрузка книги", Color.Black);
        _logger.Info($"Загрузка книги [{_bookType}] \"{filePath}\"");
        _loadBookTask = LoadConsinmentsBook(filePath, _bookType);

        _loadBookTask.ContinueWith(t =>
        {
            if (t.IsCompletedSuccessfully)
            {
                _logger.Info("Успешная загрузка");
                AddRecentFile(BookType, filePath);


                SetLogMessage("Успешная загрузка", Color.Green);
                SelectedConsinmentsBook = t.Result;
                LoadBookEnabled = false;
                _ = FillFullProgressBar();
            }
            else
            {
                _logger.Error("Возникла ошибка при загрузке.");
                _logger.Error(t.Exception);

                MessageBox.Show(_callerForm, "Ошибка при загрузке книги", "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetLogMessage("Ошибка при загрузке книги", Color.Red);
                LoadingProgressValue = 0;
            }

            InvokePropertyChanged(nameof(CanOpenBook));
            InvokePropertyChanged(nameof(SelectedConsinmentsBook));
        });

        InvokePropertyChanged(nameof(CanOpenBook));
        InvokePropertyChanged(nameof(SelectedConsinmentsBook));

        CancellationTokenSource progressCancellationSource = new();

        Task.Run(() =>
        {
            while (LoadingProgressValue < 50)
            {
                LoadingProgressValue += 1;
                Task.Delay(50).Wait();
            }
        }, progressCancellationSource.Token);
    }

    private async Task FillFullProgressBar()
    {
        while (LoadingProgressValue < 100)
        {
            LoadingProgressValue += 1;
            await Task.Delay(TimeSpan.FromMilliseconds(10));
        }
    }

    private void SetLogMessage(string text, Color messageColor)
    {
        LogMessage = text;
        LogMessageColor = messageColor;
    }

    private readonly string RecentFilesCacheFilePath = Path.Combine(Program.CacheDirectoryPath, "recentConsinmentsBook.data");

    private List<RecentConsinmentsBook> LoadRecentFiles()
    {
        if (!File.Exists(RecentFilesCacheFilePath))
            return new List<RecentConsinmentsBook>();

        try
        {
            string cacheData;

            using (StreamReader sr = File.OpenText(RecentFilesCacheFilePath))
            {
                cacheData = sr.ReadToEnd();
                sr.Close();
            }

            return JsonConvert.DeserializeObject<List<RecentConsinmentsBook>>(cacheData)!;
        }
        catch (Exception ex)
        {
            Logger.Instance.Error("Ошибка загрузки недавно открытых файлов");
            Logger.Instance.Error(ex);
            return new List<RecentConsinmentsBook>();
        }
    }

    private void SaveRecentFiles(IEnumerable<RecentConsinmentsBook> recentFilesCollection)
    {
        if (File.Exists(RecentFilesCacheFilePath))
            File.Delete(RecentFilesCacheFilePath);

        List<RecentConsinmentsBook> recentFiles = new List<RecentConsinmentsBook>(recentFilesCollection);
        if (recentFiles.Count == 0)
            return;

        string cacheDirectory = Path.GetDirectoryName(RecentFilesCacheFilePath)!;
        if (!Directory.Exists(cacheDirectory))
            Directory.CreateDirectory(cacheDirectory);

        using (StreamWriter sw = File.CreateText(RecentFilesCacheFilePath))
        {
            sw.Write(JsonConvert.SerializeObject(recentFiles));
            sw.Close();
        }
    }

    private void AddRecentFile(ConsinmentsBookType bookType, string filePath)
    {
        RecentConsinmentsBookModel? recentModel = RecentFiles.FirstOrDefault(m => m.Origin.FilePath == filePath);
        if (recentModel is not null)
        {
            RecentConsinmentsBook recentFile = recentModel.Origin;
            recentFile.LastOpened = DateTime.Now;
            recentFile.BookType = bookType;

            RecentFiles.Remove(recentModel);
        }
        else
        {
            RecentConsinmentsBook recentFile = new RecentConsinmentsBook(DateTime.Now, filePath, bookType);
            recentModel = new RecentConsinmentsBookModel(recentFile);
        }

        RecentFiles.Insert(0, recentModel);
    }

    private string GetConsinmentsBookName(ConsinmentsBookType bookType) => bookType switch
    {
        ConsinmentsBookType.Canteen => "Книга столовой",
        ConsinmentsBookType.Ledge => "Книга склада",
        _ => throw new InvalidProgramException()
    };
}
