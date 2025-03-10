using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace projekt
{
    public partial class MainWindow : Window
    {
        private readonly LoginService _loginService = new LoginService();

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

        private void btnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void btnClose_Click(object sender, RoutedEventArgs e) => ApplyCloseAnimation();

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            var success = await _loginService.LoginAsync(username, password);

            if (success)
            {
                MessageBox.Show("Sikeres bejelentkezés!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                // Itt átnavigálhatsz egy másik ablakra
                new kopapirollo().Show();
                this.Close();
            }
            else
            {
                StatusText.Text = "Hibás bejelentkezési adatok!";
            }

            
            
            
            //check fuggveny --> Backend-ben van e ilyen acc ha es jo e a jelszo



        
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
          

       

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
