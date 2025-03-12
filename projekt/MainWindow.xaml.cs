using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows.Media.Animation;

namespace projekt
{
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
                ApplyOpacityAnimation(0.8, 1.0, 0.3);
            }
        }

        public void btnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        public void btnClose_Click(object sender, RoutedEventArgs e) => ApplyCloseAnimation();

        public async void btnLogin_Click(object sender, RoutedEventArgs e)
        {


            HttpClient client = new HttpClient();
            try
            {
                string url = "http//127.0.0.1:3595";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string stringResponse = await response.Content.ReadAsStringAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }


        private void ApplyOpacityAnimation(double from, double to, double duration)
        {
            BeginAnimation(OpacityProperty, new DoubleAnimation(from, to, TimeSpan.FromSeconds(duration)));
        }

        private void ApplyCloseAnimation()
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            fadeOut.Completed += (s, e) => Application.Current.Shutdown();
            BeginAnimation(OpacityProperty, fadeOut);
        }
          

       

        
    }
}
