using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace t.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HistoryPage : Page
    {
        // ģ����˺��б�
        private List<(string account, string time)> nowAccounts = new List<(string, string)> { };

        public HistoryPage()
        {
            this.InitializeComponent();
            FreshNowAccounts();
            LoadAccounts();
        }

        private void FreshNowAccounts()
        {
            if (nowAccounts != null) nowAccounts.Clear();
            string folderPath = $"{Shared.Instance.basePath}GradesProcessed";
            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string pattern = @"\d+";
                MatchCollection matches = Regex.Matches(fileName, pattern);

                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        string numbers = match.Value;
                        DateTime lastModified = File.GetLastWriteTime(file);
                        nowAccounts.Add((numbers, lastModified.ToString()));
                    }
                }
            }
        }


        private void LoadAccounts()
        {
            foreach (var (account, time) in nowAccounts)
            {
                // ����һ��Grid������ÿһ��
                var rowGrid = new Grid
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,  // ��ÿһ��ƽ������ҳ��
                    ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // �˺���
                new ColumnDefinition { Width = GridLength.Auto },                      // �߽���
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // ʱ����
                new ColumnDefinition { Width = GridLength.Auto },                      // �߽���
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // ɾ����ť
                new ColumnDefinition { Width = GridLength.Auto },                      // �߽���
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }  // ���水ť
            }
                };

                // �˺��ı�
                var accountText = new TextBlock
                {
                    Text = account,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(10, 0, 10, 0)
                };
                Grid.SetColumn(accountText, 0);

                // �߽���
                var border1 = new Border
                {
                    Width = 1,
                    Background = new SolidColorBrush(Colors.Gray)
                };
                Grid.SetColumn(border1, 1);

                // ʱ���ı�
                var timeText = new TextBlock
                {
                    Text = time,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(10, 0, 10, 0)
                };
                Grid.SetColumn(timeText, 2);

                // �߽���
                var border2 = new Border
                {
                    Width = 1,
                    Background = new SolidColorBrush(Colors.Gray)
                };
                Grid.SetColumn(border2, 3);

                // ɾ����ť
                var deleteButton = new Button
                {
                    Content = "ɾ��",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 100
                };
                deleteButton.Click += (sender, args) => DeleteAccount(account);
                Grid.SetColumn(deleteButton, 4);

                // �߽���
                var border3 = new Border
                {
                    Width = 1,
                    Background = new SolidColorBrush(Colors.Gray)
                };
                Grid.SetColumn(border3, 5);

                // ���水ť
                var saveButton = new Button
                {
                    Content = "����",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 100
                };
                saveButton.Click += (sender, args) => SaveAccountAsync(account);
                Grid.SetColumn(saveButton, 6);

                // �����пؼ���ӵ�Grid
                rowGrid.Children.Add(accountText);
                rowGrid.Children.Add(border1);
                rowGrid.Children.Add(timeText);
                rowGrid.Children.Add(border2);
                rowGrid.Children.Add(deleteButton);
                rowGrid.Children.Add(border3);
                rowGrid.Children.Add(saveButton);

                // ����һ����ӵ�StackPanel
                AccountPanel.Children.Add(rowGrid);
            }
        }

        // ɾ���˺ŵķ���
        private void DeleteAccount(string account)
        {
            // �ڴ˴���ɾ���߼�
            nowAccounts.RemoveAll(a => a.account == account);
            AccountPanel.Children.Clear();
            LoadAccounts();

            string filePath1 = $"{Shared.Instance.basePath}GradesProcessed\\Grade-{account}.txt";
            string filePath2 = $"{Shared.Instance.basePath}GradesOriginal\\Grade{account}.txt";

            if (File.Exists(filePath1))
            {
                File.Delete(filePath1);
            }

            if (File.Exists(filePath2))
            {
                File.Delete(filePath2);
            }
        }

        // �����˺ŵķ���
        private async Task SaveAccountAsync(string account)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "��������",
                Content = "�Ժ���˵",
                PrimaryButtonText = "OK"
            };

            await errorDialog.ShowAsync();
        }
    }
}
