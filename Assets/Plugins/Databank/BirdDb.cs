using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DataBank
{
    public class BirdDb : SqliteHelper
    {
        private const String CodistanTag = "Codistan: BirdDb:\t";

        private const String TABLE_NAME = "parsed_birds";
        private const String KEY_LAT = "Latitude";
        private const String KEY_LNG = "Longitude";
        private const String KEY_SPC = "Species";
        private String[] COLUMNS = new String[] { KEY_LAT, KEY_LNG, KEY_SPC };

        public BirdDb() : base()
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                KEY_LAT + " TEXT, " +
                KEY_LNG + " TEXT, " +
                KEY_SPC + " )";
            dbcmd.ExecuteNonQuery();
        }

        public void addData(BirdEntity bird)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_LAT + ", "
                + KEY_LNG + ", "
                + KEY_SPC + " ) "

                + "VALUES ( '"

                + bird._Lat + "', '"
                + bird._Lng + "', '"
                + bird._Species + "' )";
            dbcmd.ExecuteNonQuery();
        }

        public override IDataReader getDataById(int id)
        {
            return base.getDataById(id);
        }

        public override IDataReader getDataByString(string str)
        {
            Debug.Log(CodistanTag + "Getting Location: " + str);

            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " LIMIT 10";
            return dbcmd.ExecuteReader();
        }

        //public override void deleteDataByString(string id)
        //{
        //    Debug.Log(CodistanTag + "Deleting Location: " + id);

        //    IDbCommand dbcmd = getDbCommand();
        //    dbcmd.CommandText =
        //        "DELETE FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + id + "'";
        //    dbcmd.ExecuteNonQuery();
        //}

        //public override void deleteDataById(int id)
        //{
        //    base.deleteDataById(id);
        //}

        //public override void deleteAllData()
        //{
        //    Debug.Log(CodistanTag + "Deleting Table");

        //    base.deleteAllData(TABLE_NAME);
        //}

        public override IDataReader getAllData()
        {
            return base.getAllData(TABLE_NAME);
        }

        //public IDataReader getNearestLocation(LocationInfo loc)
        //{
        //    Debug.Log(CodistanTag + "Getting nearest centoid from: "
        //        + loc.latitude + ", " + loc.longitude);
        //    IDbCommand dbcmd = getDbCommand();

        //    string query =
        //        "SELECT * FROM "
        //        + TABLE_NAME
        //        + " ORDER BY ABS(" + KEY_LAT + " - " + loc.latitude
        //        + ") + ABS(" + KEY_LNG + " - " + loc.longitude + ") ASC LIMIT 1";

        //    dbcmd.CommandText = query;
        //    return dbcmd.ExecuteReader();
        //}

        //public IDataReader getLatestTimeStamp()
        //{
        //    IDbCommand dbcmd = getDbCommand();
        //    dbcmd.CommandText =
        //        "SELECT * FROM " + TABLE_NAME + " ORDER BY " + KEY_DATE + " DESC LIMIT 1";
        //    return dbcmd.ExecuteReader();
        //}
    }
}
