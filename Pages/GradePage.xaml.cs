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
using Microsoft.UI.Xaml.Shapes;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace t.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GradePage : Page
    {
        public GradePage()
        {
            this.InitializeComponent();
            ShowCourse();
        }

        public ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();

        public static List<Course> coursesList = new List<Course>();

        public void AddCourse(Course newCourse)
        {
            Courses.Add(newCourse);
        }

        static bool ContainsBlacklistedWord(string line, List<string> blacklist)
        {
            foreach (var word in blacklist)
            {
                if (line.Contains(word))
                {
                    return true;
                }
            }
            return false;
        }
        public void ProcessFile(string inputPath, string outputPath, List<string> blacklist)
        {
            List<string> lines = new List<string>();

            using (var reader = new StreamReader(inputPath, System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            int firstSkip = 2;
            bool skipLine = false;

            using (var writer = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8))
            {
                int tempNum = 1;
                foreach (var l in lines)
                {
                    if (tempNum <= firstSkip)
                    {
                        tempNum++;
                        continue;
                    }

                    if (skipLine)
                    {
                        skipLine = false;
                        continue;
                    }

                    if (string.IsNullOrEmpty(l))
                    {
                        skipLine = true;
                        continue;
                    }

                    if (ContainsBlacklistedWord(l, blacklist))
                    {
                        writer.WriteLine($"#{l}");
                    }
                    else
                    {
                        writer.WriteLine(l);
                        var parts = l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        coursesList.Add(new Course(
                            parts[0],
                            parts[1],
                            int.Parse(parts[2]),
                            int.Parse(parts[3]),
                            double.Parse(double.Parse(parts[4]).ToString("F8")),
                            0
                        ));
                    }
                }
            }
        }
        public void CalculateGPA(List<Course> courses)
        {
            double GPsum = 0;
            double times = 0;

            foreach (var course in courses)
            {
                GPsum += course.GradePoint * course.Credits;
                times += course.Credits;
            }

            GPABox.Text = (GPsum / times).ToString("F2");

            foreach (var course in courses)
            {
                double affect = (GPsum / times) - ((GPsum - course.GradePoint * course.Credits) / (times - course.Credits));
                course.GradeEffect = affect;
            }
        }
        private async Task SaveProcessedCourse(List<Course> courseList)
        {
            string processedFileSavePath = $"{Shared.Instance.basePath}GradesProcessed\\Grade-{Shared.Instance.nowAccount}.txt";

            using (StreamWriter writer = new StreamWriter(processedFileSavePath, false))
            {
                foreach (var course in courseList)
                {
                    string line = $"{course.CourseName},{course.CourseCategory},{course.Credits},{course.Grade},{course.GradePoint},{course.GradeEffect}";
                    await writer.WriteLineAsync(line);
                }
            }
        }
        private async void ShowCourse()
        {
            coursesList.Clear();
            string txtPath = $"{Shared.Instance.basePath}GradesOriginal\\Grade{Shared.Instance.nowAccount}.txt";

            if (Shared.Instance.isFirstTimeShowScore)
            {
                ProcessFile(txtPath, txtPath, Shared.Instance.blackList);
                CalculateGPA(coursesList);
                Task task = SaveProcessedCourse(coursesList);
                Shared.Instance.isFirstTimeShowScore = false;
            }

            foreach (var course in coursesList)
            {
                AddCourse(course);
            }

            NowAccountBox.Text = Shared.Instance.nowAccount;
        }

        public class Course(string name, string category, int credits, double grade, double point, double effect)
        {
            public string CourseName { get; set; } = name;
            public string CourseCategory { get; set; } = category;
            public int Credits { get; set; } = credits;
            public double Grade { get; set; } = grade;
            public double GradePoint { get; set; } = point;
            public double GradeEffect { get; set; } = effect;
        }

    }
}
