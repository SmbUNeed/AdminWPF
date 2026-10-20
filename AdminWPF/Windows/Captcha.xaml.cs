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
using System.Windows.Shapes;

namespace AdminWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для Captcha.xaml
    /// </summary>
    public partial class Captcha : Window
    {
        Random random = new Random();
        string capcha;
        public Captcha()
        {
            InitializeComponent();
            UpdateCapcha();
        }

        public void UpdateCapcha(){
            var alph = "1234567890qwertyuiopasdfghjklzxcvbnm";
            string res = "";
            for (int i = 0; i < 6; i++) {
                int index = random.Next(0, alph.Length);
                res += alph[index];
            }
            capcha = res;
            CapchaTB.Text = res;
            CapchaEnter.Text = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (capcha == CapchaEnter.Text.ToLower()) {
                DialogResult = true;
                Close();
            }
            else {
                ErrorText.Text = "Ошибка!";
                UpdateCapcha();
            }
        }
    }
}
