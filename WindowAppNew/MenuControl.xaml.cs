﻿using System;
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

namespace WindowAppNew
{
    /// <summary>
    /// Логика взаимодействия для MenuControl.xaml
    /// </summary>
    public partial class MenuControl : UserControl
    {
        private string filename;

        public MenuControl()
        {
            InitializeComponent();
            filename = "";
        }

        public MenuControl(string filename)
        {
            InitializeComponent();
            this.filename = filename;
        }
    }
}
