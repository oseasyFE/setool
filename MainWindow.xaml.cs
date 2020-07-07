using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IWshRuntimeLibrary;
using System.Globalization;
using System.Reflection;
using setool.lib;

namespace setool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Browsers browsersUtil = new Browsers();
        ObservableCollection<EngineModel> gridSource = new ObservableCollection<EngineModel>();
        private WshShell wsh = new WshShellClass();
        private Dictionary<string, IWshShortcut> installedBrowsers = new Dictionary<string, IWshShortcut>();
        private Dictionary<string, string> defaultBrowsers = new Dictionary<string, string>();
        private List<CheckBox> browserRefs = new List<CheckBox>();
        private Dictionary<string, IWshShortcut> selectedBrowsers
        {
            get
            {
                var ret = new Dictionary<string, IWshShortcut>();
                string key;
                foreach(var cb in browserRefs)
                {
                    if (true == cb.IsChecked)
                    {
                        key = cb.Name + ".exe";
                        ret[key] = installedBrowsers[key];
                    }
                }
                return ret;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            detectBrowsers();
        }
        void detectBrowsers()
        {
            Dictionary<string, IWshShortcut> installedBrowsers = browsersUtil.GetInstalledBrowsers();
            foreach (KeyValuePair<string, IWshShortcut> kv in installedBrowsers)
            {
                var cb = new CheckBox();
                cb.Name = kv.Key.Replace(".exe", "");
                cb.Content = browsersUtil.GetBrowserName(kv.Value);
                cb.Margin = new Thickness(0, 0, 5, 0);
                cb.IsChecked = true;
                browserRefs.Add(cb);
                browserPanel.Children.Add(cb);
            }
        }
        void onCheckBrowser(object sender, RoutedEventArgs e)
        {
            CheckBox src = (CheckBox)e.Source;
            Console.WriteLine("checkbox " + src.Content.ToString() + " checked!");
        }
        void onRecordOp(object sender, RoutedEventArgs e)
        {
            string name = ((Button)e.OriginalSource).Name;
            EngineModel model;
            SearchEngineInfoEditor editor;
            DataContext dc = (DataContext)Resources["dataContext"];
            switch (name)
            {
                case "AddRecordBtn":
                    model = new EngineModel("", "", "");
                    editor = new SearchEngineInfoEditor()
                    {
                        Owner = this,
                        MyModel = model,
                        Title = "新增"
                    };
                    if(true == editor.ShowDialog())
                    {
                        dc.Engines.Add(model);
                    }
                    break;
                case "EditRecordBtn":
                    model = (EngineModel)seGrid.SelectedItem;
                    editor = new SearchEngineInfoEditor()
                    {
                        Owner = this,
                        MyModel = model,
                        Title = "编辑"
                    };
                    editor.ShowDialog();
                    seGrid.UnselectAll();
                    setState("SelectedRow", null);
                    break;
                case "RemoveRecordBtn":
                    model = (EngineModel)seGrid.SelectedItem;
                    dc.Engines.Remove(model);
                    setState("SelectedRow", null);
                    break;
            }    
        }
        HashSet<IWshShortcut> findShortcuts(string dir) {
            Regex lnkre = new Regex(@"\.lnk$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            HashSet<IWshShortcut> ret = new HashSet<IWshShortcut>();
            FileSystemObject fs = new FileSystemObjectClass();
            Folder folder = fs.GetFolder(dir);
            foreach(IWshRuntimeLibrary.File file in folder.Files)
            {
                if(file == null) { break;  }
                if(!lnkre.IsMatch(file.Name))
                {
                    continue;
                }
                IWshShortcut shortcut = (IWshShortcut)wsh.CreateShortcut(file.Path);
                //IWshShortcut shortcut = (IWshShortcut)wsh.CreateShortcut(@"C:\Users\mysto\Desktop\v2rayN - 快捷方式.lnk");
                //Console.WriteLine("{0}{1}{2}{3}{4}", shortcut.FullName, shortcut.TargetPath, shortcut.Arguments, shortcut.IconLocation, shortcut.WorkingDirectory);
                ret.Add(shortcut);
            }
            foreach(IWshRuntimeLibrary.Folder item in folder.SubFolders)
            {
                if(item == null)
                {
                    break;
                }
                ret.UnionWith(findShortcuts(item.Path));
            }
            return ret;
        }
        void updateShortcut(IWshShortcut shortcut)
        {
            
        }
        void updateHomePage(EngineModel model)
        {
            // IE 使用修改注册表的方式，key: HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main
            // chromium 内核系列的浏览器使用修改配置文件 Secure Preferences 的方式
            Regex chromeVersionRe = new Regex(@"^\d+\.\d+\.$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (KeyValuePair<string, IWshShortcut> kv in selectedBrowsers)
            {
                if(kv.Key == "iexplore.exe")
                {
                    Console.WriteLine(wsh.RegRead(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main"));
                    //wsh.RegWrite(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main", model.MainPage);
                } else
                {
                    foreach (var fileInfo in Directory.GetParent(kv.Value.TargetPath).GetFiles())
                    {
                        //C:\Users\mysto\AppData\Local\Google\Chrome\User Data\Default\Secure Preferences
                        if (chromeVersionRe.IsMatch(fileInfo.Name))
                        {
                            // chrome
                        }
                    }
                }
            }
        }
        void updateSearchProvider()
        {
            // 更改默认搜索引擎的参数信息
        }
        // 取消所有修改
        private void rcvBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<string, IWshShortcut> kv in selectedBrowsers)
            {
                browsersUtil.SetBrowserArgument(kv.Value, null);
            }
        }
        // 一键执行所有修改
        private void execAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<string, IWshShortcut> kv in selectedBrowsers)
            {
                if(lnkBtn.IsEnabled)
                {
                    browsersUtil.SetBrowserArgument(kv.Value, (EngineModel)lnkCombo.SelectedItem);
                }
                if(mpBtn.IsEnabled)
                {
                    browsersUtil.SetHomepage(kv.Value, (EngineModel)mpCombo.SelectedItem);
                }
                if(seBtn.IsEnabled)
                {
                    browsersUtil.SetSearchEngineProvider(kv.Value, (EngineModel)seCombo.SelectedItem);
                }
            }
        }
        // 添加桌面浏览器链接
        private void lnkBtn_Click(object sender, RoutedEventArgs e)
        {
            EngineModel model = (EngineModel)lnkCombo.SelectedItem;
            foreach(KeyValuePair<string, IWshShortcut> kv in selectedBrowsers)
            {
                browsersUtil.SetBrowserArgument(kv.Value, model);
            }
        }
        // 修改浏览器主页
        private void mpBtn_Click(object sender, RoutedEventArgs e)
        {
            EngineModel model = (EngineModel)mpCombo.SelectedItem;
            foreach (KeyValuePair<string, IWshShortcut> kv in selectedBrowsers)
            {
                browsersUtil.SetHomepage(kv.Value, model);
            }
        }
        // 修改浏览器默认搜索引擎
        private void seBtn_Click(object sender, RoutedEventArgs e)
        {
            EngineModel model = (EngineModel)lnkCombo.SelectedItem;
            foreach (KeyValuePair<string, IWshShortcut> kv in selectedBrowsers)
            {
                browsersUtil.SetSearchEngineProvider(kv.Value, model);
            }
        }
        private void cookieBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<string, IWshShortcut> kv in selectedBrowsers)
            {
                browsersUtil.ClearCache(kv.Value);
            }
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            EngineModel model = (EngineModel)e.Item;
            e.Accepted = model.IsValid();
        }

        private void seGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            setState("SelectedRow", dg.SelectedItem);
        }
        private void setState(string key, object value)
        {
            DataContext dc = (DataContext)Resources["dataContext"];
            Type dcType = dc.GetType();
            PropertyInfo propertyInfo = dcType.GetProperty(key);
            propertyInfo.SetValue(dc, value);
        }

    }
}
