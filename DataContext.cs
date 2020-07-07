using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace setool
{
    class DataContext : INotifyPropertyChanged
    {
        private EngineModel _shortcut;
        private EngineModel _homepage;
        private EngineModel _defaultEngine;
        private EngineList _engines;
        private EngineModel _selectedRow;

        public DataContext()
        {
            _engines = new EngineList();
            _engines.CollectionChanged += onEnginesChange;
            _engines.Add(new EngineModel(
                "360",
                "https://www.so.com/?src=lm&ls=sm2363376&lm_extend=ctype:31",
                "https://www.so.com/s?ie={inputEncoding}&fr=lm&ls=sm2363376&lm_extend=ctype:31&src=home_lm&q=%s"
            ));
            _engines.Add(new EngineModel(
                "百度",
                "https://www.baidu.com/index.php?tn=02049043_70_pg&ch=1",
                "http://www.baidu.com/s?tn=02049043_70_pg&ch=1&ie={inputEncoding}&wd=%s"
            ));
        }

        public EngineModel Shortcut { get => _shortcut; set
            {
                if (_shortcut != value)
                {
                    _shortcut = value;
                    onPropertyChange("Shortcut");
                    onPropertyChange("HasShortcut");
                }
            }
        }
        public EngineModel Homepage { get => _homepage; set
            {
                if(_homepage != value)
                {
                    _homepage = value;
                    onPropertyChange("Homepage");
                    onPropertyChange("HasHomepage");
                }
            }
        }
        public EngineModel DefaultEngine { get => _defaultEngine; set
            {
                if(_defaultEngine != value)
                {
                    _defaultEngine = value;
                    onPropertyChange("DefaultEngine");
                    onPropertyChange("HasDefaultEngine");
                }
            }
        }
        public EngineList Engines { get => _engines; }
        public bool HasShortcut { get => _shortcut != null; }
        public bool HasHomepage { get => _homepage != null; }
        public bool HasDefaultEngine { get => _defaultEngine != null; }
        public bool IsAllSelected { get => HasDefaultEngine && HasHomepage && HasShortcut; }
        public EngineModel SelectedRow { get => _selectedRow; set
            {
                if(_selectedRow != value)
                {
                    _selectedRow = value;
                    onPropertyChange("SelectedRow");
                    onPropertyChange("HasRowSelected");
                }
            }
        }
        public bool HasRowSelected { get => _selectedRow != null; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void onPropertyChange(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void onEnginesChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(_engines.Count == 0)
            {
                _shortcut = _homepage = _defaultEngine = null;
                onPropertyChange("Shortcut");
                onPropertyChange("HasShortcut");
                onPropertyChange("Homepage");
                onPropertyChange("HasHomepage");
                onPropertyChange("DefaultEngine");
                onPropertyChange("HasDefaultEngine");
            }
            else
            {
                EngineModel selected = _engines[0];
                if (_shortcut == null)
                {
                    _shortcut = selected;
                    onPropertyChange("Shortcut");
                    onPropertyChange("HasShortcut");
                }
                if(_homepage == null)
                {
                    _homepage = selected;
                    onPropertyChange("Homepage");
                    onPropertyChange("HasHomepage");
                }
                if(_defaultEngine == null)
                {
                    _defaultEngine = selected;
                    onPropertyChange("DefaultEngine");
                    onPropertyChange("HasDefaultEngine");
                }
            }
            
        }
    }
}
