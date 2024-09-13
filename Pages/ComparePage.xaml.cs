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
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace t.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class Course(string name, string category, int credits, double grade, double point, double effect)
    {
        public string CourseName { get; set; } = name;
        public string CourseCategory { get; set; } = category;
        public int Credits { get; set; } = credits;
        public double Grade { get; set; } = grade;
        public double GradePoint { get; set; } = point;
        public double GradeEffect { get; set; } = effect;
    }


    public sealed partial class ComparePage : Page
    {
        public List<string> nowAccounts { get; set; } = new List<string>();
        public ObservableCollection<Course> courses1 { get; set; } = [];
        public ObservableCollection<Course> courses2 { get; set; } = [];

        public ComparePage()
        {
            this.InitializeComponent();
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
                        nowAccounts.Add(numbers);
                        Console.WriteLine(numbers);
                    }
                }
            }
        }

        private async Task ShowCourseAsync(int side, string account)
        {
            if (side == 1) courses1.Clear();
            else courses2.Clear();

            string filePath = $"{Shared.Instance.basePath}GradesProcessed\\Grade-{account}.txt"; //     ·  

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] s = line.Split(',');

                    if (s.Length == 6)
                    {
                        try
                        {
                            Course course = new
                            (
                                s[0],
                                s[1],
                                int.Parse(s[2]),
                                double.Parse(s[3]),
                                double.Parse(s[4]),
                                double.Parse(s[5])
                            );
                            Console.WriteLine(course);
                            if (side == 1)
                            {
                                courses1.Add(course);
                            }
                            else
                            {
                                courses2.Add(course);
                            }
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine(line);
                    }
                }
            }
        }

        private async void ShowAccountNotFoundDialog()
        {
            await AccountNotFoundDialog.ShowAsync();
        }

        public void CalculateGPA(int side, ObservableCollection<Course> courses)
        {
            double GPsum = 0; // 绩点和
            double times = 0; // 权重和

            // 计算总绩点和总权重
            foreach (var course in courses)
            {
                GPsum += course.GradePoint * course.Credits;
                times += course.Credits;
            }

            // 输出GPA
            if (side == 1) GPABox1.Text = (GPsum / times).ToString("F2");
            else GPABox2.Text = (GPsum / times).ToString("F2");
        }

        public async void LeftSearchButtonClick(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            FreshNowAccounts();

            var searchAccount = sender.Text;

            if (nowAccounts.Contains(searchAccount))
            {
                await ShowCourseAsync(1, searchAccount);
                CalculateGPA(1, courses1);
            }
            else
            {
                ShowAccountNotFoundDialog();
            }
        }

        public async void RightSearchButtonClick(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            FreshNowAccounts();

            var searchAccount = sender.Text;

            if (nowAccounts.Contains(searchAccount))
            {
                await ShowCourseAsync(2, searchAccount);
                CalculateGPA(2, courses2);
            }
            else
            {
                ShowAccountNotFoundDialog();
            }
        }
    }
}
