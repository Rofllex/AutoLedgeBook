using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

using AutoLedgeBook.Data.Abstractions;

using AutoLedgeBook.Utils.Matches;

namespace AutoLedgeBook.ViewModels;

public class MatchesEditorFormViewModel : ViewModel
{
    private readonly IMatchesList<string, string> _matches;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="matches">Список сопоставлений для редактирования</param>
    /// <param name="destinationProducts">Продукты в книге накладных</param>
    /// <param name="sourceProducts">Продукты в накладной</param>
    public MatchesEditorFormViewModel(IMatchesList<string, string> matchesList, string[] bookProductNames)
    {
        _matches = matchesList;
        
        DestinationProducts = new BindingList<string>(bookProductNames);
    }

    public BindingList<string> DestinationProducts { get; init; }


    public bool CanAddProductsFromConsinment { get; } = true;


    


    public void AddProductsFromConsinment()
    {
        if (!CanAddProductsFromConsinment)
            return;

        string consinmentFilePath;

        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Файл с накладной|*.xlsx";
            openFileDialog.CheckFileExists = openFileDialog.CheckPathExists = true;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                consinmentFilePath = openFileDialog.FileName;
            }
            else
            {
                return;
            }
        }

    }

}