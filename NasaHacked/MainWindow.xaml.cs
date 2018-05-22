using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace NasaHacked
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime? selectedDate;
        public MainWindow()
        {
            InitializeComponent();
            selectedDate = DateTime.Now;
            progress.IsIndeterminate = false;
            imag.Visibility = Visibility.Collapsed;
        }

        private async void DownloadImageClick(object sender, RoutedEventArgs e)
        {
            progress.IsIndeterminate = true;
            var obj = await DownloadJson(selectedDate);
            var ms = await DownloadImage(obj);
            if (obj.media_type == "image")
            {
                

                var bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                ShowImage(bi);
                textBlockDisc.Text = obj.explanation;
            }

            else
            {
                Process.Start(obj.url);
            }


            progress.IsIndeterminate = false;
            progress.Visibility = Visibility.Collapsed;
            

        }


        private Task<RootObject> DownloadJson(DateTime? data)
        {
            return Task.Run(() =>
            {
                string valueJson;
                //data = DateTime.ParseExact(data.Value.Date.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                using (var webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    valueJson = webClient.DownloadString("https://api.nasa.gov/planetary/apod?api_key=cjEJ2OhMsDkKdg0xZ3kMlkc4W0ebgknXlnDrIZFD" + "&" +"date="+ data.Value.Date.ToString("yyyy-MM-dd"));
                }
                RootObject obj = JsonConvert.DeserializeObject<RootObject>(valueJson);
                return obj;
                
            });
            //LayoutRoot.Background = imb;
        }

        private Task<MemoryStream> DownloadImage(RootObject obj)
        {
            return Task.Run(() =>
            {
                // var c = new WebClient();
                using (var c = new WebClient())
                {
                    var bytes = c.DownloadData(obj.url);
                    var ms = new MemoryStream(bytes);
                    return ms;
                }
            });
        }



        private void ShowImage(BitmapImage biImage)
        {
            imag.Visibility= Visibility.Visible;
            imag.Source = biImage;
        }

        private void DateChange(object sender, SelectionChangedEventArgs e)
        {
            selectedDate = date.SelectedDate;
        }
    }
}
