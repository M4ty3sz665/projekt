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
        private static readonly HttpClient httpClient = new HttpClient();


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
            string username = txtUser.Text.Trim();
            string password = txtPass.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Kérlek, töltsd ki az összes mezőt!");
                return;
            }

            var loginData = new { username, password };
            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Cseréld ki a megfelelő API URL-re, ha szükséges
                var response = await httpClient.PostAsync("http://localhost:3959/api/login", content);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Hibás felhasználónév vagy jelszó!");
                    return;
                }
                var responseJson = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseJson);
                if (result.success == true)
                {
                    // Mentjük a felhasználó nevét és a token értékét
                    SessionManager.Username = username;
                    SessionManager.Token = result.token;

                    // Megnyitjuk a játékablakot
                    kopapirollo gameWindow = new kopapirollo();
                    gameWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Hibás felhasználónév vagy jelszó!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt: " + ex.Message);
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
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Window1 regWindow = new Window1();
            regWindow.Show();
            this.Close();
        }



       

        private void ASD_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
