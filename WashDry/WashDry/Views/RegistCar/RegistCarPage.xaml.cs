﻿using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Net.Http;
using Spillman.Xamarin.Forms.ColorPicker;
using System.Net.Http.Headers;
using WashDry.SQLiteDb;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using WashDry.Models.ApiModels;

namespace WashDry.Views.RegistCar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistCarPage : ContentPage
    {
        public RegistCarPage()
        {
            InitializeComponent();

        }

        private Color editedColor;

        public Color EditedColor
        {
            get => editedColor;
            set { editedColor = value; OnPropertyChanged(); }
        }
        public UserDataBase userDataBase;
        public string imagen_name;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, true);

            userDataBase = new UserDataBase();
            var user_exist = userDataBase.GetMembers().ToList();

            idx = user_exist[0].id.ToString();
        }

        private MediaFile _image;
        private string idx;
        private async void AgregarAutobtn_Clicked(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(Marca_.Text))
            {
                Marca_.Focus();
            } else if (string.IsNullOrEmpty(Placas.Text)){
                Placas.Focus();
            }
            else if (string.IsNullOrEmpty(Modelo.Text)){
                Modelo.Focus();
            }

            Cator.IsRunning = true;
            Cator.IsVisible = true;
            lblmodelo.IsVisible = false;
            btnCamara.IsVisible = false;
            btnGal.IsVisible = false;
            imgx.IsVisible = false;
            Marca_.IsVisible = false;
            Modelo.IsVisible = false;
            Placas.IsVisible = false;
            Ann_.IsVisible = false;
            lblamio.IsVisible = false;
            lblmarca.IsVisible = false;
            colorx.IsVisible = false;
            lblPlacas.IsVisible = false;
       
            lblError.Text = "Agregando su auto";

            try
            {
                var color = colorx.BackgroundColor;
                var color2 = colorx.ViewModel.Color;
                var colorhex = colorx.ViewModel.Hex;
                var color22 = colorx.ViewModel.HueColor;
                var modelox = Modelo.Text.ToString();
                var marcax = Marca_.Text.ToString();
                var annx = Ann_.Date.Year.ToString();
                var placasx = Placas.Text.ToString();
                
                var current = Connectivity.NetworkAccess;
                if (current != NetworkAccess.Internet)
                {
                    await DisplayAlert("Sin linea", "Active sus datos o su wifi para agregar un auto.", "OK");
                }

                StringContent hex = new StringContent(colorhex.ToString());
                StringContent id_usuario = new StringContent(idx);
                StringContent placas = new StringContent(placasx);
                StringContent modelo = new StringContent(modelox);
                StringContent ann = new StringContent(annx);
                StringContent marca = new StringContent(marcax);


                var content1 = new MultipartFormDataContent();
                content1.Add(new StreamContent(_image.GetStream()), "\"file\"", $"\"{_image.Path}\"");

                var httpClient1 = new System.Net.Http.HttpClient();
                httpClient1.BaseAddress = new Uri("http://www.washdryapp.com");
                var url1 = "http://www.washdryapp.com/oficial/ImagenesPerfil.php";
                var responseMsg1 = await httpClient1.PostAsync(url1, content1);
                var remotePath = await responseMsg1.Content.ReadAsStringAsync();
                imagen_name = remotePath;
                //*************
                var value_check = new Dictionary<string, string>
                { 
                    { "id_usuario", idx },
                        { "placas", placasx},
                        { "modelo", modelox},                   
                        { "ann", annx },
                        { "marca", marcax },
                        { "color", colorhex },
                    { "image",  imagen_name}
                };
            var httpClient = new System.Net.Http.HttpClient();                
                var url = "http://www.washdryapp.com/app/public/auto/guardar";
                var content = new FormUrlEncodedContent(value_check);

                var responseMsg = await httpClient.PostAsync(url, content);
              
                switch (responseMsg.StatusCode)
                {

                    case System.Net.HttpStatusCode.InternalServerError: 
                        string xjsonerror = await responseMsg.Content.ReadAsStringAsync();
                        await DisplayAlert("error", "error status 500 InternalServerError", "ok");
                        btnCamara.IsVisible = true;
                        btnGal.IsVisible = true;
                        lblmarca.IsVisible = true;
                        lblmodelo.IsVisible = true;
                        imgx.IsVisible = true;
                        Marca_.IsVisible = true;
                        Modelo.IsVisible = true;
                        Placas.IsVisible = true;
                        lblamio.IsVisible = true;
                        Ann_.IsVisible = true;
                        lblPlacas.IsVisible = true;
                        colorx.IsVisible = true;
                        lblError.Text = "hubo un error status 500 ";
                        Cator.IsRunning = false;
                        Cator.IsVisible = false;
                        break;
                    case System.Net.HttpStatusCode.BadRequest:
                        lblError.Text = "hubo un error status 400 ";
                        await DisplayAlert("error", "error status 400 Unauthorized", "ok");
                        Cator.IsRunning = false;
                        Cator.IsVisible = false;
                        break;

                    case System.Net.HttpStatusCode.Forbidden:
                        await DisplayAlert("error", "error status 403  ", "ok");
                        lblError.Text = "hubo un error status 403 "; Cator.IsRunning = false;
                        Cator.IsVisible = false;
                        break;

                    case System.Net.HttpStatusCode.NotFound:
                        await DisplayAlert("error", "error status 404  ", "ok");
                        lblError.Text = "hubo un error status 404 "; Cator.IsRunning = false;
                        Cator.IsVisible = false;
                        break;

                    case System.Net.HttpStatusCode.OK:

                        Cator.IsRunning = false;
                        Cator.IsVisible = false;
                        string xjson =  await responseMsg.Content.ReadAsStringAsync();
                        var mess = JsonConvert.DeserializeObject<AutoRegistResp200>(xjson);
                        await DisplayAlert("Exito", " " + mess.message + " ", "ok");

                        btnCamara.IsVisible = true;
                        btnGal.IsVisible = true;
                        lblmarca.IsVisible = true;
                        lblmodelo.IsVisible = true;
                        imgx.IsVisible = true;
                        Marca_.IsVisible = true;
                        Modelo.IsVisible = true;
                        Placas.IsVisible = true;
                        lblamio.IsVisible = true;
                        Ann_.IsVisible = true;
                        colorx.IsVisible = true;
                        await Navigation.PopToRootAsync();

                        break;


                    case System.Net.HttpStatusCode.RequestEntityTooLarge:
                        await DisplayAlert("error", "error status 413  ", "ok");
                        lblError.Text = "hubo un error status 413 ";
                        Cator.IsRunning = false;
                        Cator.IsVisible = false;
                        break;
                    case System.Net.HttpStatusCode.RequestTimeout:
                        await DisplayAlert("error", "error status 408  ", "ok");
                        lblError.Text = "hubo un error status 408 ";
                        Cator.IsRunning = false;
                        Cator.IsVisible = false;
                        break;



                    case System.Net.HttpStatusCode.Unauthorized:
                        await DisplayAlert("error", "yeah status 401 Unauthorized", "ok");
                        Cator.IsRunning = false;
                        Cator.IsVisible = false;
                        break;

                }
            }
            catch (Exception ex)
            {

                await DisplayAlert("Error", "Error : "+ex.ToString(), "OK");
                Cator.IsRunning = false;
                Cator.IsVisible = false;
            }
  
        }

        string result;

        private async void btnCamara_Clicked(object sender, EventArgs e)
        {

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera soportada.", "OK");
                return;
            }
            _image = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "auto_" + idx,
                Name = "auto.jpg"
            });
            if (_image == null)
                return;
             var xlocal = _image.Path;
           
            imgx.Source = ImageSource.FromStream(() => {
                return _image.GetStream();  });

            //_image.Path
         // _ = b64img();


        }

        private async Task b64img() {
            try
            {
                Cator.IsVisible = true;
                Cator.IsRunning = true;
                byte[] b =  System.IO.File.ReadAllBytes(_image.Path);
                String s = Convert.ToBase64String(b);
                b64.Text = s;

                colorx.IsVisible = false;
                btnCamara.IsVisible = false;
                btnGal.IsVisible = false;

                

            }
            catch (Exception ex)
            {

               await DisplayAlert("error", ex.ToString(), "ok");
            }


            Cator.IsVisible = false;
            Cator.IsRunning = false;
        }

        private async void btnGal_Clicked(object sender, EventArgs e)
        {

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("No Galeria", ":( No camera soportada.", "OK");
                return;
            }

            _image = await CrossMedia.Current.PickPhotoAsync();

            if (_image == null)
            {
                return;
            }
            imgx.Source = ImageSource.FromStream(() => {

                return _image.GetStream();


            });

            //
           
            // end try
        }
    }
}