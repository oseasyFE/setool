using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace setool
{
    public class EngineModel : INotifyPropertyChanged
    {
        private string _name;
        private string _main_page;
        private string _lnk_page;

        public EngineModel(string name, string mainPage, string lnkPage)
        {
            this._name = name;
            this._main_page = mainPage;
            this._lnk_page = lnkPage;
        }
        public string Name {
            get { return _name;  }
            set {
                if(!_name.Equals(value))
                {
                    _name = value;
                    onPropertyChange("Name");
                }
            }
        }
        public string MainPage
        {
            get { return _main_page; }
            set
            {
                if (!_main_page.Equals(value))
                {
                    _main_page = value;
                    onPropertyChange("MainPage");
                }
            }
        }
        public string LnkPage
        {
            get { return _lnk_page; }
            set
            {
                if (!_lnk_page.Equals(value))
                {
                    _lnk_page = value;
                    onPropertyChange("LnkPage");
                }
            }
        }
        public bool isEditing { get; set; }
        public bool selected { get; set; }

        public new string ToString => _name;

        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChange(string propName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        internal bool IsValid()
        {
            try
            {
                new Uri(_main_page);
                new Uri(_lnk_page);
                return _name.Length > 0 && _main_page.Length > 0 && _lnk_page.Length > 0;
            } catch(UriFormatException e)
            {
                return false;
            }
            
        }
    }

}
