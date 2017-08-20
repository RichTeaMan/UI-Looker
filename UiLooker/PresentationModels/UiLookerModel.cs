﻿using System;
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
