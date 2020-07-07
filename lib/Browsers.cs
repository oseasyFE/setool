using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace setool.lib
{
    enum BrowserType
    {
        IntenetExplore,
        MSEdge,
        Chrome,
        Chrome360,
        ChromeLiebao,
        ChromeSougou,
        NONE
    }
    class Browsers
    {
        private WshShell wsh = new WshShellClass();
        private Dictionary<string, string> defaultBrowsers = new Dictionary<string, string>();

        public Browsers()
        {
            defaultBrowsers.Add("chrome.exe", "谷歌浏览器");
            defaultBrowsers.Add("360chrome.exe", "360极速浏览器");
            defaultBrowsers.Add("sogouExplorer.exe", "搜狗浏览器");
            defaultBrowsers.Add("liebao.exe", "猎豹浏览器");
            defaultBrowsers.Add("iexplore.exe", "IE浏览器");
            defaultBrowsers.Add("msedge.exe", "Microsoft Edge");
        }
        public BrowserType GetBrowserType(IWshShortcut shortcut)
        {
            return GetBrowserTypes(shortcut.TargetPath);
        }
        public BrowserType GetBrowserTypes(string path)
        {
            string[] parts = path.Split('\\');
            string filename = parts[parts.Length - 1];
            BrowserType type;
            switch(filename)
            {
                case "chrome.exe":
                    type = BrowserType.Chrome;
                    break;
                case "msedge.exe":
                    type = BrowserType.MSEdge;
                    break;
                case "iexplore.exe":
                    type = BrowserType.IntenetExplore;
                    break;
                case "360chrome.exe":
                    type = BrowserType.Chrome360;
                    break;
                case "sogouExplorer.exe":
                    type = BrowserType.ChromeSougou;
                    break;
                case "liebao.exe":
                    type = BrowserType.ChromeLiebao;
                    break;
                default:
                    type = BrowserType.NONE;
                    break;
            }
            return type;
        }
        public string GetBrowserName(IWshShortcut shortcut)
        {
            return GetBrowserName(GetBrowserType(shortcut));
        }
        public string GetBrowserName(BrowserType type)
        {
            string text = "";
            switch(type)
            {
                case BrowserType.Chrome:
                    text = "谷歌浏览器";
                    break;
                case BrowserType.Chrome360:
                    text = "360极速浏览器";
                    break;
                case BrowserType.ChromeLiebao:
                    text = "猎豹浏览器";
                    break;
                case BrowserType.ChromeSougou:
                    text = "搜狗浏览器";
                    break;
                case BrowserType.IntenetExplore:
                    text = "IE浏览器";
                    break;
                case BrowserType.MSEdge:
                    text = "Microsoft Edge";
                    break;
            }
            return text;
        }

        public void SetBrowserArgument(IWshShortcut shortcut, EngineModel model)
        {
            if(model == null)
            {
                shortcut.Arguments = "";
            }
            else
            {
                shortcut.Arguments = model.LnkPage;
            }
            shortcut.Save();
        }
        public void SetHomepage(IWshShortcut shortcut, EngineModel model)
        {
            string file = shortcut.TargetPath;
        }
        public void ClearCache(IWshShortcut shortcut)
        {
            BrowserType type = GetBrowserType(shortcut);
            string[] cacheDir = null;
            string homePath = Environment.GetEnvironmentVariable("USERPROFILE");
            switch (type)
            {
                case BrowserType.Chrome:
                    cacheDir = new string[] { homePath + @"\AppData\Local\Google\Chrome\User Data\Default\" };
                    break;
                case BrowserType.IntenetExplore:
                    cacheDir = new string[]{
                        homePath + @"\AppData\Local\Microsoft\Intern~1",
                        homePath + @"\AppData\Local\Microsoft\Windows\History",
                        homePath + @"\AppData\Local\Microsoft\Windows\Tempor~1",
                        homePath + @"\AppData\Roaming\Microsoft\Windows\Cookies"
                    };
                    break;
                case BrowserType.MSEdge:
                    cacheDir = new string[] { homePath + @"\AppData\Local\Microsoft\Edge\User Data\Default\" };
                    break;
                case BrowserType.Chrome360:
                case BrowserType.ChromeLiebao:
                case BrowserType.ChromeSougou:
                case BrowserType.NONE:
                default:
                    cacheDir = new string[] { };
                    break;
            }
            //string GooglePath = homePath + @"\AppData\Local\Google\Chrome\User Data\Default\";
            //string MozilaPath = homePath + @"\AppData\Roaming\Mozilla\Firefox\";
            //string Opera1 = homePath + @"\AppData\Local\Opera\Opera";
            //string Opera2 = homePath + @"\AppData\Roaming\Opera\Opera";
            //string Safari1 = homePath + @"\AppData\Local\Apple Computer\Safari";
            //string Safari2 = homePath + @"\AppData\Roaming\Apple Computer\Safari";
            //string IE1 = homePath + @"\AppData\Local\Microsoft\Intern~1";
            //string IE2 = homePath + @"\AppData\Local\Microsoft\Windows\History";
            //string IE3 = homePath + @"\AppData\Local\Microsoft\Windows\Tempor~1";
            //string IE4 = homePath + @"\AppData\Roaming\Microsoft\Windows\Cookies";
            //string Flash = homePath + @"\AppData\Roaming\Macromedia\Flashp~1";
        }
        public void SetSearchEngineProvider(IWshShortcut shortcut, EngineModel model)
        {
            //
        }
        public Dictionary<string, IWshShortcut> GetInstalledBrowsers()
        {
            Dictionary<string, IWshShortcut> installedBrowsers = new Dictionary<string, IWshShortcut>();
            HashSet<string> folders = new HashSet<string>();
            folders.Add(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            folders.Add(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
            folders.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));
            folders.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
            HashSet<IWshShortcut> shortcuts = new HashSet<IWshShortcut>();
            foreach (var item in folders)
            {
                shortcuts.UnionWith(findShortcuts(item));
            }

            foreach (IWshShortcut lnk in shortcuts)
            {
                string filepath = lnk.TargetPath;
                string[] parts = filepath.Split("\\");
                string basename = parts[parts.Length - 1];
                if (defaultBrowsers.ContainsKey(basename) && !installedBrowsers.ContainsKey(basename))
                {
                    installedBrowsers.Add(basename, lnk);
                }
            }
            return installedBrowsers;
        }
        private HashSet<IWshShortcut> findShortcuts(string dir)
        {
            Regex lnkre = new Regex(@"\.lnk$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            HashSet<IWshShortcut> ret = new HashSet<IWshShortcut>();
            FileSystemObject fs = new FileSystemObjectClass();
            Folder folder = fs.GetFolder(dir);
            foreach (IWshRuntimeLibrary.File file in folder.Files)
            {
                if (file == null) { break; }
                if (!lnkre.IsMatch(file.Name))
                {
                    continue;
                }
                IWshShortcut shortcut = (IWshShortcut)wsh.CreateShortcut(file.Path);
                //IWshShortcut shortcut = (IWshShortcut)wsh.CreateShortcut(@"C:\Users\mysto\Desktop\v2rayN - 快捷方式.lnk");
                //Console.WriteLine("{0}{1}{2}{3}{4}", shortcut.FullName, shortcut.TargetPath, shortcut.Arguments, shortcut.IconLocation, shortcut.WorkingDirectory);
                ret.Add(shortcut);
            }
            foreach (IWshRuntimeLibrary.Folder item in folder.SubFolders)
            {
                if (item == null)
                {
                    break;
                }
                ret.UnionWith(findShortcuts(item.Path));
            }
            return ret;
        }

    }
}
