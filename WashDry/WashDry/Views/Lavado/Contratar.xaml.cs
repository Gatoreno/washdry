﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WashDry.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WashDry.Views.Lavado
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Contratar : ContentPage
    {
        public Contratar()
        {
            InitializeComponent();
            BindingContext = new TimerModel();

           
        }
 
    }
}