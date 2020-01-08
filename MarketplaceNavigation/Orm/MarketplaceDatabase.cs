using System;
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

using MarketplaceNavigation.Models;
using Android.Util;

namespace MarketplaceNavigation.Orm
{
    public class MarketplaceDatabase
    {
        private const string tag = "MarkerPlaceDb";

        private const int FLOORS_COUNT = 10;
        private const int PAVILIONS_COUNT = 10;


        private SQLiteAsyncConnection db;
        private string dbPath = System.IO.Path
            .Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Database");
        public MarketplaceDatabase()
        {
            System.IO.Directory.CreateDirectory(dbPath);
            string dbName = System.IO.Path.Combine(dbPath, "Database.db");
            db = new SQLiteAsyncConnection(dbName);

            DropDb();

            db.CreateTableAsync<User>().Wait();
            db.CreateTableAsync<Floor>().Wait();
            db.CreateTableAsync<Pavilion>().Wait();
            db.CreateTableAsync<Shop>().Wait();

            FillDb();

            User Admin = new User();
            Admin.AccessLevel = AccessLevels.Administrator;
            Admin.Name = "Admin";
            Admin.Password = "Admin";
            db.InsertAsync(Admin);

            this.DAO = new DbHelper(db);
            Log.Info("Db", "Database created");
        }
        public DbHelper DAO { get; private set; }
        
        private void DropDb()
        {
            db.DropTableAsync<User>().Wait();
            db.DropTableAsync<Shop>().Wait();
            db.DropTableAsync<Pavilion>().Wait();
            db.DropTableAsync<Floor>().Wait();
        }

        private void FillDb()
        {
            if (db.Table<Floor>().CountAsync().Result == 0)
                for (int i = 1; i < FLOORS_COUNT; i++)
                {
                    var floor = new Floor();
                    floor.Number = i;
                    floor.Size = 10;
                    floor.EmptySize = PAVILIONS_COUNT;
                    db.InsertAsync(floor).Wait();
                    Log.Info(tag, $"Floor #{i} created");

                    for (int j = 1; j <= PAVILIONS_COUNT; j++)
                    {
                        var pavilion = new Pavilion();
                        pavilion.Size = 1;
                        pavilion.IsEmpty = true;
                        pavilion.FloorId = i;
                        pavilion.Number = j + (100 * i);

                        db.InsertAsync(pavilion).Wait();
                        Log.Info("Db", $"Pavilion #{pavilion.Number} created");
                    }
                }
            else
                Log.Info(tag, "Floor there");
        }
    }
}