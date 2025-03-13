using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;


namespace projekt
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public Window1()
        {
            InitializeComponent();
        }

        public async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = UsernameTextBox.Text.Trim();
                string password = PasswordBox.Password.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Kérlek, töltsd ki az összes mezőt!");
                    return;
                }

                var registerData = new { username, password };
                var json = JsonConvert.SerializeObject(registerData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("http://localhost:3959/api/register", content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Hiba történt a regisztráció során!");
                    return;
                }
                var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                dynamic result = JsonConvert.DeserializeObject(responseJson);
                if (result.success == true)
                {
                    MainWindow mainW = new MainWindow();
                    mainW.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Hiba történt a regisztráció során!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt: " + ex.Message);
            }
        }

        private async Task AutoLogin(string username, string password)
        {
            var loginData = new { username, password };
            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("http://localhost:3959/api/login", content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Regisztráció sikeres, de bejelentkezési hiba!");
                    return;
                }
                var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                dynamic result = JsonConvert.DeserializeObject(responseJson);
                if (result.success == true)
                {
                    SessionManager.Username = username;
                    SessionManager.Token = result.token;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainWindow gameWindow = new MainWindow();
                        gameWindow.Show();
                        this.Close();
                    });
                }
                else
                {
                    MessageBox.Show("Hiba történt a bejelentkezés során!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt: " + ex.Message);
            }
        }
    }
}
