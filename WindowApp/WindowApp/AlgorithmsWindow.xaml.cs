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

namespace WindowApp
{
    /// <summary>
    /// Логика взаимодействия для AlgorithmsWindow.xaml
    /// </summary>
    public partial class AlgorithmsWindow : Window
    {
        private int[] numbers;

        public AlgorithmsWindow()
        {
            InitializeComponent();
        }


        private void clickButton(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public int[] getInformation()
        {
            numbers = new int[3];
            Array.Clear(numbers, 0, numbers.Length);
            if (checkBox1.IsChecked == true)
                numbers[0] = 1;
            if (checkBox2.IsChecked == true)
                numbers[1] = 1;
            if (checkBox3.IsChecked == true)
                numbers[2] = 1;
            if (numbers[0] == 0 && numbers[1] == 0 && numbers[2] == 0)
            {
                MessageBox.Show("Don't choose algorithms");
            }
            return numbers;
        }
    }
}
