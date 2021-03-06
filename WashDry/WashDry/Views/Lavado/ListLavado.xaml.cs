﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WashDry.Models.ApiModels;
using WashDry.Models.DbModels;
using WashDry.SQLiteDb;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WashDry.Views.Lavado
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListLavado : ContentPage
    {
        public ListLavado()
        {
            InitializeComponent();
         }
        protected override  void OnAppearing()
        {
             
            _ = GetSolicitudesWeb();
        }



        private async void ListLavados_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var content = e.Item as Solicitudes;
            await Navigation.PushAsync(new InfoLavado(Int32.Parse(content.id_solicitud)));
        }

        public User user;
        public UserDataBase userDataBase;

        public async Task GetSolicitudesWeb()
        {
            CatorMain.IsVisible = true;
            CatorMain.IsRunning = true;

            try
            {

                userDataBase = new UserDataBase();
                var user_exist = userDataBase.GetMembers().ToList();
                HttpClient client = new HttpClient();
                var uri = "http://www.washdryapp.com/app/public/solicitud/lista_all/" + user_exist[0].id;


            
                var responseMsg = await client.GetAsync(uri);



                switch (responseMsg.StatusCode)
                {
                    case System.Net.HttpStatusCode.InternalServerError:
                        Console.WriteLine("----------------------------------------------_____:Here status 500");

                        //xlabel.Text = "error 500";
                        // Cator.IsVisible = false;
                        break;


                    case System.Net.HttpStatusCode.OK:
                        Console.WriteLine("----------------------------------------------_____:Here status 200");

                        // ylabel.Text = "Ultimas noticas de proyectos";


                        // var json_ = JsonConvert.DeserializeObject<List<VisitasMod>>(xjson);

                        string xjson = await responseMsg.Content.ReadAsStringAsync();
                        //DireccApiCall

                        HttpContent contentD = responseMsg.Content;
                       var xjsonD = await contentD.ReadAsStringAsync();


                        if (xjsonD.Count()<=0)
                        {
                            lblMainlavados.Text = "Usted no tiene lavados aun";
                        }
                        else
                        {
                            var json_d = JsonConvert.DeserializeObject<List<Solicitudes>>(xjsonD);
                            ListLavados.ItemsSource = json_d;
                        }


                

                        break;

                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("", "" + ex.ToString(), "ok");
                //  Cator.IsVisible = false;

                //  CatorT.Text = "Ha habido un error";
                return;
            }

            CatorMain.IsVisible = false;
            CatorMain.IsRunning = false;
        }

       
    }
}