using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiLooker.PresentationModels
{
    public class UiLookerModel : INotifyPropertyChanged
    {
        private ElementTreeView _uiElementTree;

        private ElementTreeView _selectedUiElement;

        private IReadOnlyList<UiPattern> _supportedPatterns;

        public ElementTreeView UiElementTree
        {
            get { return _uiElementTree; }
            set
            {
                _uiElementTree = value;
                OnPropertyChanged(nameof(UiElementTree));
            }
        }

        public ElementTreeView SelectedUiElement
        {
            get { return _selectedUiElement; }
            set
            {
                _selectedUiElement = value;
                OnPropertyChanged(nameof(SelectedUiElement));
            }
        }

        public IReadOnlyList<UiPattern> SupportedPatterns
        {
            get { return _supportedPatterns; }
            set
            {
                _supportedPatterns = value;
                OnPropertyChanged(nameof(SupportedPatterns));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

        }

    }
}
