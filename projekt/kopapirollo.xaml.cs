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
using static projekt.MainWindow;

namespace projekt
{
    /// <summary>
    /// Interaction logic for kopapirollo.xaml sigma balls nigger
    /// </summary>
    public partial class kopapirollo : Window
    {

        private static readonly HttpClient httpClient = new HttpClient();
        private int wins = 0;
        private int losses = 0;
        private int draws = 0;

        public kopapirollo()
        {
            InitializeComponent();
            LoadStats();
        }

        private void AddAuthHeader()
        {
            // Töröljük a korábbi x-auth-token fejléceket, majd hozzáadjuk a jelenlegit
            if (httpClient.DefaultRequestHeaders.Contains("x-auth-token"))
                httpClient.DefaultRequestHeaders.Remove("x-auth-token");

            httpClient.DefaultRequestHeaders.Add("x-auth-token", SessionManager.Token);
        }


        private async void LoadStats()
        {
            try
            {
                AddAuthHeader();
                string username = SessionManager.Username;
                var response = await httpClient.GetAsync($"http://localhost:3959/api/history/{username}");
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    dynamic stats = JsonConvert.DeserializeObject(responseJson);
                    wins = stats.user.wins;
                    losses = stats.user.loses;
                    draws = stats.user.draws;
                    UpdateStatsUI();
                }
                else
                {
                    MessageBox.Show("Hiba történt a statisztikák lekérésekor!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a statisztikák betöltésekor: " + ex.Message);
            }
        }


        private async void ChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            string playerChoice = button.Tag.ToString();

            // Gombok letiltása a játék közben
            RockButton.IsEnabled = false;
            PaperButton.IsEnabled = false;
            ScissorsButton.IsEnabled = false;
            ResultTextBlock.Text = "A bot gondolkodik...";

            // Rövid késleltetés a bot "gondolkodásához"
            await Task.Delay(1000);

            try
            {
                AddAuthHeader();
                var playData = new { selectedItem = playerChoice };
                var json = JsonConvert.SerializeObject(playData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("http://localhost:3959/api/play", content);
                var responseJson = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseJson);

                // Gombok újra engedélyezése
                RockButton.IsEnabled = true;
                PaperButton.IsEnabled = true;
                ScissorsButton.IsEnabled = true;

                if (result.success != true)
                {
                    ResultTextBlock.Text = "Hiba történt!";
                    return;
                }

                string botItem = result.botItem;
                if (result.playerWin == true)
                {
                    wins++;
                    ResultTextBlock.Text = $"Nyertél! A bot választott: {botItem}";
                }
                else if (result.botWin == true)
                {
                    losses++;
                    ResultTextBlock.Text = $"Vesztettél! A bot választott: {botItem}";
                }
                else
                {
                    draws++;
                    ResultTextBlock.Text = $"Döntetlen! A bot választott: {botItem}";
                }
                UpdateStatsUI();
                await SaveStats();
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = "Hiba a játék során!";
                MessageBox.Show("Hiba: " + ex.Message);
            }
        }



        private void UpdateStatsUI()
        {
            WinsTextBlock.Text = wins.ToString();
            LossesTextBlock.Text = losses.ToString();
            DrawsTextBlock.Text = draws.ToString();
        }

        private async Task SaveStats()
        {
            try
            {
                AddAuthHeader();
                string username = SessionManager.Username;
                var statsData = new { username, wins, draws, loses = losses };
                var json = JsonConvert.SerializeObject(statsData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await httpClient.PutAsync("http://localhost:3959/api/save", content);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba az adatok mentésekor: " + ex.Message);
            }
        }


        private async void ResetStatsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddAuthHeader();
                string username = SessionManager.Username;
                var data = new { username };
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("http://localhost:3959/api/resetStats", content);
                if (response.IsSuccessStatusCode)
                {
                    wins = 0;
                    losses = 0;
                    draws = 0;
                    UpdateStatsUI();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a statisztikák törlésekor: " + ex.Message);
            }
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        
    }
}
