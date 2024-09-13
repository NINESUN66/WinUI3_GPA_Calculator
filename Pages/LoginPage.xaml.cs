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
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace t.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private void AccountBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {

            //sender.SelectionStart = sender.Text.Length;
            //// ����һ���µ��ַ�����ֻ��������
            //string newText = new string(sender.Text.Where(Char.IsDigit).ToArray());
            //// �����µ��ı�����
            //sender.Text = newText;
            //sender.SelectionStart = sender.Text.Length;
        }


        private bool CheckAccountPassword()
        {
            return Account.Text.Length != 0
                 && Account.Text.All(char.IsDigit)
                 && Password.Password.Length != 0;
        }

        private async Task ShowWrongAccountAsync()
        {
            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "�˺Ż�������������",
                Content = "�����˺Ż�����",
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary
            };

            await dialog.ShowAsync();
        }

        private async Task ShowSucceedGetScoreAsync()
        {
            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "�ɼ���ȡ�ɹ�",
                Content = "ǰȥ�鿴��ǰ�ɼ�",
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary
            };

            ContentDialogResult result = await dialog.ShowAsync();

            //if (result == ContentDialogResult.Primary)
            //{
            //    var mainWindow = MainWindow.GetMainWindow();

            //    if (mainWindow != null)
            //    {
            //        var navView = mainWindow.navView;
            //        navView.SelectedItem = navView.MenuItems
            //            .OfType<NavigationViewItem>()
            //            .FirstOrDefault(item => item.Tag.ToString() == "GradePage");
            //    }
            //}
        }

        private async Task PostScoreRequestAsync(string account, string password)
        {
            try
            {
                // ��������� URL ������
                string url = Shared.Instance.requestURL;
                var data = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", account),
                    new KeyValuePair<string, string>("password", password)
                });

                // ���� POST ����
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PostAsync(url, data);
                    string responseText = await response.Content.ReadAsStringAsync();

                    if (responseText.Length == 0)
                    {
                        await ShowWrongAccountAsync();
                        LoginLoading.IsActive = false;
                        return;
                    }
                    // �����������Ӧ����
                    await File.WriteAllTextAsync($"{Shared.Instance.basePath}GradesOriginal\\Grade{Account.Text}.txt", responseText, System.Text.Encoding.UTF8);

                    // ���� Grade.txt �ļ�
                    // await Launcher.LaunchFileAsync(await Windows.Storage.StorageFile.GetFileFromPathAsync($"{Shared.Instance.basePath}Grade.txt"));

                    // �ɹ���ȡ�ɼ���Ĵ���
                    Shared.Instance.nowAccount = Account.Text;
                    await ShowSucceedGetScoreAsync();
                    Shared.Instance.isFirstTimeShowScore = true;
                }
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "����ʧ��",
                    Content = $"��������: {ex.Message}",
                    PrimaryButtonText = "OK"
                };

                await errorDialog.ShowAsync();

                LoginLoading.IsActive = false;
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginLoading.IsActive = true;

            if (!CheckAccountPassword())
            {
                await ShowWrongAccountAsync();
            }
            else
            {
                await PostScoreRequestAsync(Account.Text, Password.Password);
            }

            LoginLoading.IsActive = false;
        }
    }

    public class Shared
    {
        // ��ֻ̬���ֶΣ����浥��ʵ��
        private static readonly Shared instance = new Shared();

        // ˽�й��캯������ֹ�ⲿʵ����
        private Shared()
        {
            LoadConfiguration();

            nowAccount = null;
            isFirstTimeShowScore = false;
        }

        // ������̬���ԣ����ص���ʵ��
        public static Shared Instance
        {
            get
            {
                return instance;
            }
        }

        public string nowAccount { get; set; }
        public bool isFirstTimeShowScore { get; set; }
        public string basePath { get; } = "E:\\vs_project\\t\\";
        public string requestURL { get; set; }
        public List<string> blackList { get; set; } = new List<string>();

        private string jsonFilePath => "E:\\vs_project\\t\\config.json";
        //private string jsonFilePath => "config.json";

        private void LoadConfiguration()
        {
            if (File.Exists(jsonFilePath))
            {
                try
                {
                    var jsonString = File.ReadAllText(jsonFilePath);
                    var config = JsonSerializer.Deserialize<Configuration>(jsonString);

                    if (config != null)
                    {
                        requestURL = config.requestURL;
                        blackList = config.blackList;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading configuration: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Configuration file not found.");
            }
        }

        private class Configuration
        {
            public string requestURL { get; set; }
            public List<string> blackList { get; set; }
        }
    }

}
