﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using WashDry.Droid;
using WashDry.Interface;
using Xamarin.Forms;

[assembly: Dependency(typeof(Android_SQLite))]

namespace WashDry.Droid
{
    public class Android_SQLite : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var dbName = "User.sqlite";
            var dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            var path = System.IO.Path.Combine(dbPath, dbName);
            var conn = new SQLite.SQLiteConnection(path);


            return conn;
        }
    }
}