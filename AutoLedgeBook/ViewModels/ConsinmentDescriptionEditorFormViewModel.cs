using System;
using System.Windows.Forms;

using AutoLedgeBook.Data.Abstractions;



namespace AutoLedgeBook.ViewModels
{
    public class ConsinmentDescriptionEditorFormViewModel : ViewModel
    {
        private readonly IConsinmentNote _consinment;
        private readonly Form _callerForm;

        private IConsinmentDescription _consinmentDescription => _consinment.Description;

        public ConsinmentDescriptionEditorFormViewModel(Form callerForm, IConsinmentNote consinment)
        {
            _callerForm = callerForm ?? throw new ArgumentNullException(nameof(callerForm));
            _consinment = consinment ?? throw new ArgumentNullException(nameof(consinment));
        }

        public string Number => _consinment.Number;
        

        public string Destination
        {
            get => _consinmentDescription.Destination;
            set => _consinmentDescription.Destination = value;
        }

        public string ConsinmentType
        {
            get => _consinmentDescription.Type;
            set => _consinmentDescription.Type = value;
        }

        public int PersonsCount
        {
            get => _consinmentDescription.PersonsCount;
            set => _consinmentDescription.PersonsCount = value;
        }

        public void Confirm()
        {
            _callerForm.DialogResult = DialogResult.OK;
            _callerForm.Close();
        }
    }
}
