using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace setool
{
    /// <summary>
    /// SearchEngineInfoEditor.xaml 的交互逻辑
    /// </summary>
    public partial class SearchEngineInfoEditor : Window
    {
        private EngineModel myModel;
        public SearchEngineInfoEditor()
        {
            InitializeComponent();
        }

        public new EngineModel MyModel { get => myModel; set
            {
                myModel = value;
                seName.Text = value.Name;
                seHomePage.Text = value.MainPage;
                seLinkPage.Text = value.LnkPage;
            }
        }

        private void StackPanel_Click(object sender, RoutedEventArgs e)
        {
            Button src = (Button)e.Source;
            if(src.Name == "ok")
            {
                myModel.Name = seName.Text;
                myModel.MainPage = seHomePage.Text;
                myModel.LnkPage = seLinkPage.Text;
                DialogResult = true;
            } else
            {
                DialogResult = false;
            }
        }

        private void field_TextChanged(object sender, TextChangedEventArgs e)
        {
            EngineModel tmp = new EngineModel(seName.Text, seHomePage.Text, seLinkPage.Text);
            ok.IsEnabled = tmp.IsValid();
        }
    }
}
