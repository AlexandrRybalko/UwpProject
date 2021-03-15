﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Storage;

namespace DAL
{
    public class Repository
    {
        public static void InitializeDatabase()
        {
            var a = ApplicationData.Current.LocalFolder.CreateFileAsync("CamerasDB.db", CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "CamerasDB.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Cameras (Id INTEGER PRIMARY KEY, " +
                    "Ip_Address NVARCHAR(2048) NOT NULL, Country NVARCHAR(2048) NOT NULL, City NVARCHAR(2048) NOT NULL, Image_Type NVARCHAR(2048) NOT NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }

        public void AddCamera(Camera camera)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "CamerasDB.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = "INSERT INTO Cameras VALUES (NULL, @Ip_Address, @Country, @City, @Image_Type);";
                insertCommand.Parameters.AddWithValue("@Ip_Address", camera.IpAddress);
                insertCommand.Parameters.AddWithValue("@Country", camera.Country);
                insertCommand.Parameters.AddWithValue("@City", camera.City);
                insertCommand.Parameters.AddWithValue("@Image_Type", camera.ImageType);

                insertCommand.ExecuteReader();

                db.Close();
            }
        }

        public void AddRange(List<Camera> cameras)
        {
            foreach (var camera in cameras)
            {
                AddCamera(camera);
            }
        }

        public ObservableCollection<Camera> GetCameras()
        {
            ObservableCollection<Camera> entries = new ObservableCollection<Camera>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "CamerasDB.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT * from Cameras", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Camera entity = new Camera();
                    entity.Id = query.GetInt32(0);
                    entity.IpAddress = query.GetString(1);
                    entity.Country = query.GetString(2);
                    entity.City = query.GetString(3);
                    entity.ImageType = query.GetString(4);

                    entries.Add(entity);
                }

                db.Close();
            }

            return entries;
        }

        public Camera GetCamera(int id)
        {
            Camera entity = new Camera();
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "CamerasDB.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ($"SELECT * from Cameras where Id={id}", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                if (query.Read())
                {
                    entity.Id = query.GetInt32(0);
                    entity.IpAddress = query.GetString(1);
                    entity.Country = query.GetString(2);
                    entity.City = query.GetString(3);
                    entity.ImageType = query.GetString(4);
                }

                db.Close();
            }

            return entity;
        }
    }
}
