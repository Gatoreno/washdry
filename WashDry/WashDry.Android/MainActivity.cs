﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using WashDry.Models.DbModels;
using WashDry.SQLiteDb;
using Android.Gms.Common;
using Android.Util;
using Firebase.Iid;
using System.Linq;

namespace WashDry.Droid
{
    [Activity(Label = "WashDry", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public User user;
        private UserDataBase userdb;

        const string TAG = "MainActivity";

        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.SetFlags(new[] { "CollectionView_Experimental", "Shell_Experimental" });
            Xamarin.FormsMaps.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());

            //CreateNotificationChannel();

            IsPlayServicesAvailable();
            Log.Debug(TAG, "InstanceID token: " + FirebaseInstanceId.Instance.Token);
            var token = FirebaseInstanceId.Instance.Token;
            //CONSULTAR BD
            userdb = new UserDataBase();
            var userW = new User();
            var user_exista = userdb.GetMembers().ToList();
            var user_exist = userdb.GetMembers();
            int RowCount = 0;
            int usercount = user_exist.Count();
            RowCount = Convert.ToInt32(usercount);
            if (RowCount > 1)
            {
                userdb.DeleteMembers();
                userW.token = token;
                userW.status = 0;
                userdb.AddMember(userW);
            }
            else if (RowCount == 1)
            {
                userdb.UpdateMemberToken(user_exista[0].id, token);
                /*if (token != user_exista[0].token)
                {
                    
                }*/
            }
            else
            {
                if (token == null || token == "")
                {
                    FinishAffinity();
                }
                userW.id_cliente = 0;
                
                userW.name = "";
                userW.nombre = "";
                userW.password = "";
                userW.username = "";
                userW.remember_token = "";
                userW.google_id = "";
                userW.email = "";
                userW.token = token;
                userW.status = 0;
                userdb.AddMember(userW);
            }
        }


        public bool IsPlayServicesAvailable()
        {
            var resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    //msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                }
                else
                {
                    //msgText.Text = "This device is not supported";
                    Finish();
                }

                return false;
            }

            //msgText.Text = "Google Play Services is available.";
            return true;
        }


        //void CreateNotificationChannel()
        //{
        //    if (Build.VERSION.SdkInt < BuildVersionCodes.O)
        //    {
        //        // Notification channels are new in API 26 (and not a part of the
        //        // support library). There is no need to create a notification 
        //        // channel on older versions of Android.
        //        return;
        //    }

        //    var channel = new NotificationChannel(CHANNEL_ID, "FCM Notifications", NotificationImportance.Default)
        //    {
        //        Description = "Firebase Cloud Messages appear in this channel"
        //    };

        //    var notificationManager = (NotificationManager)GetSystemService(NotificationService);
        //    notificationManager.CreateNotificationChannel(channel);
        //}

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }
    }
}   