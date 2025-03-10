using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;
using UnityEditor;
using Unity.VisualScripting;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;

namespace DataBank
{
    public class SqliteHelper
    {
        private const string CodistanTag = "Codistan: SqliteHelper:\t";

        private const string database_name = "madhunt_db2.bytes";

        public string db_connection_string;
        public IDbConnection db_connection;

        public SqliteHelper()
        {
            //URI=file:.../AlohaBirdsGame/Assets
            TextAsset dbAsset = Resources.Load<TextAsset>("madhunt_db2");
            if(dbAsset == null)
            {
                Debug.LogError("Database file not found in Resources!");
                return;
            }

            try
            {
                string path = Path.Combine(Application.persistentDataPath, "madhunt_db2.bytes");
                File.WriteAllBytes(path, dbAsset.bytes);
            }
            catch (System.Exception ex)
            {
                string ErrorMessages = "File Write Error\n" + ex.Message;
            }
            

            db_connection_string = "URI=file:" + Application.persistentDataPath + "/" + database_name;
            Debug.Log("db_connection_string" + db_connection_string);
            db_connection = new SqliteConnection(db_connection_string);
            db_connection.Open();
        }

        ~SqliteHelper()
        {
            db_connection.Close();
            Debug.Log("database closed");
        }

        //vitual functions
        public virtual IDataReader getDataById(int id)
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        public virtual IDataReader getDataByString(string str)
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        public virtual void deleteDataById(int id)
        {
            Debug.Log(CodistanTag + "This function is not implemented");
            throw null;
        }

		public virtual void deleteDataByString(string id)
        {
            Debug.Log(CodistanTag + "This function is not implemented");
            throw null;
        }

        public virtual IDataReader getAllData()
        {
            Debug.Log(CodistanTag + "This function is not implemented");
            throw null;
        }

        public virtual void deleteAllData()
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        public virtual IDataReader getNumOfRows()
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        //helper functions
        public IDbCommand getDbCommand()
        {
            return db_connection.CreateCommand();
        }

        public IDataReader getAllData(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + table_name + " LIMIT 10";
            IDataReader reader = dbcmd.ExecuteReader();
            return reader;
        }

        public IDataReader getCloseBirds(string table_name, float lat, float lng)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            //1km = lat 0.01, 1km = 0.01 lng
            //pos + 1 > lat > pos -1
            //pos + 1 > lng > pos -1
                //select* from parsed_birds
                //where
                //(Latitude <= 21.28514725 AND Latitude >= 21.28514725)
                //AND
                //(Longitude <= -158.0597058 AND Longitude >= -158.0597058)
            var latUpper = lat + 0.01;
            var latLower = lat - 0.01;
            var lngUpper = lng + 0.01;
            var lngLower = lng - 0.01;
            dbcmd.CommandText =
                "SELECT * FROM " + table_name + 
                " WHERE (Latitude <= " + latUpper + " AND Latitude >= " + latLower + ") " +
                "AND (Longitude <= " + lngUpper + " AND Longitude >= " + lngLower + ") LIMIT 1";
            IDataReader reader = dbcmd.ExecuteReader();
            return reader;
        }

        public void deleteAllData(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText = "DROP TABLE IF EXISTS " + table_name;
            dbcmd.ExecuteNonQuery();
        }

        public IDataReader getNumOfRows(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText =
                "SELECT COALESCE(MAX(id)+1, 0) FROM " + table_name;
            IDataReader reader = dbcmd.ExecuteReader();
            return reader;
        }

		public void close (){
			db_connection.Close ();
            Debug.Log("database closed");
		}
    }
}