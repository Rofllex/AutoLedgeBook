
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

using AutoLedgeBook.ViewModels;
using AutoLedgeBook.Data.Abstractions;
using AutoLedgeBook.Utils.Matches;
using AutoLedgeBook.Utils;
using AutoLedgeBook.Utils.Extensions;

namespace AutoLedgeBook.Forms
{
    public partial class MatchesEditorForm : MetroForm
    {
        private readonly MatchesEditorFormViewModel _viewModel;

        public MatchesEditorForm(IMatchesList<string,string> matchesList, string[] bookProductNames)
        {
            InitializeComponent();
            //InitializeBindings(_viewModel = new(matchesList, ));

            _viewModel = new MatchesEditorFormViewModel(matchesList, bookProductNames);
            InitializeBindings(_viewModel);
        }

        private void InitializeBindings(MatchesEditorFormViewModel viewModel)
        {
            viewModel.DestinationProducts.ListChanged += DestinationProducts_ListChanged;
            
        }

        private void DestinationProducts_ListChanged(object? sender, ListChangedEventArgs e)
        {
            
        }
    }
}
