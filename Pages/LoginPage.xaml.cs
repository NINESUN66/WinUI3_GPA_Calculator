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
            //// 创建一个新的字符串，只包含数字
            //string newText = new string(sender.Text.Where(Char.IsDigit).ToArray());
            //// 设置新的文本内容
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
                Title = "账号或密码输入有误！",
                Content = "请检查账号或密码",
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
                Title = "成绩获取成功",
                Content = "前去查看当前成绩",
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
                // 设置请求的 URL 和数据
                string url = Shared.Instance.requestURL;
                var data = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", account),
                    new KeyValuePair<string, string>("password", password)
                });

                // 发送 POST 请求
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
                    // 输出并保存响应内容
                    await File.WriteAllTextAsync($"{Shared.Instance.basePath}GradesOriginal\\Grade{Account.Text}.txt", responseText, System.Text.Encoding.UTF8);

                    // 弹出 Grade.txt 文件
                    // await Launcher.LaunchFileAsync(await Windows.Storage.StorageFile.GetFileFromPathAsync($"{Shared.Instance.basePath}Grade.txt"));

                    // 成功获取成绩后的处理
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
                    Title = "请求失败",
                    Content = $"发生错误: {ex.Message}",
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
        // 静态只读字段，保存单例实例
        private static readonly Shared instance = new Shared();

        // 私有构造函数，防止外部实例化
        private Shared()
        {
            LoadConfiguration();

            nowAccount = null;
            isFirstTimeShowScore = false;
        }

        // 公共静态属性，返回单例实例
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
