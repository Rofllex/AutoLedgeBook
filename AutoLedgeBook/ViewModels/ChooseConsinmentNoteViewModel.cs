using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using AutoLedgeBook.Models;
using AutoLedgeBook.Data.Abstractions;

using MetroFramework.Forms;

namespace AutoLedgeBook.ViewModels;

public class ChooseConsinmentNoteViewModel<TConsinment> : ViewModel
                                                where TConsinment : IReadOnlyConsinmentNote
{
    private ConsinmentModel? _selectedConsinment;
    private readonly MetroForm _callerForm;

    private readonly IReadOnlyCollection<TConsinment> _consinmentsCollection;

    private Task? _loadConsinmentsTask;
    private CancellationTokenSource? _loadConsinmentsCancellationToken;
    private bool _loadingProgress = false;


    public ChooseConsinmentNoteViewModel(MetroForm callerForm, IReadOnlyCollection<TConsinment> consinmentsCollection)
    {
        _callerForm = callerForm;

        Consinments = new();

        _consinmentsCollection = consinmentsCollection;
    }

    public TConsinment? ResultSelectedConsinment
    {
        get
        {
            if (SelectedConsinment is null)
                return default(TConsinment);
            return (TConsinment)SelectedConsinment.Original;
        }
    }

    public BindingList<ConsinmentModel> Consinments { get; private set; }

    public ConsinmentModel? SelectedConsinment
    {
        get => _selectedConsinment;
        set => ChangeProperty(ref _selectedConsinment, value);
    }

    public bool LoadingProgress
    {
        get => _loadingProgress;
        private set => ChangeProperty(ref _loadingProgress, value);
    }

    public void SelectConsinment(ConsinmentModel consinment)
    {
        if (_loadConsinmentsTask != null && !_loadConsinmentsTask.IsCompleted)
        {
            _loadConsinmentsCancellationToken.Cancel();
            _loadConsinmentsTask.Wait();
        }

        SelectedConsinment = consinment;
        
        _callerForm.DialogResult = System.Windows.Forms.DialogResult.OK;
        _callerForm.Close();
    }

    public void FormLoaded()
    {
        _loadConsinmentsCancellationToken = new();
        LoadingProgress = true;

        _loadConsinmentsTask = LoadConsinmentsAsync(Consinments, this._consinmentsCollection, _loadConsinmentsCancellationToken.Token);
        _loadConsinmentsTask.ContinueWith(t => 
        {
            LoadingProgress = false;
        });
    }

    private Task LoadConsinmentsAsync(IList destinationList, IReadOnlyCollection<TConsinment> consinments, CancellationToken ct)
    {
        return Task.Run(() =>
        {
            foreach (TConsinment consinment in consinments)
            {
                if (ct.IsCancellationRequested)
                    return;

                try
                {
                    destinationList.Add(new ConsinmentModel(consinment));
                }
                catch (TaskCanceledException) 
                {
                }
            }

        });
    }
}
