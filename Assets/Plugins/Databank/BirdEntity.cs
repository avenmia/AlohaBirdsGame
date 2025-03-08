using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBank
{
    public class BirdEntity
    {
        public string _Lat;
        public string _Lng;
        public string _Species;

        public BirdEntity(string Lat, string Lng, string Species)
        {
            _Lat = Lat;
            _Lng = Lng;
            _Species = Species;
        }

        //public LocationEntity(string id, String type, string Lat, string Lng, string dateCreated)
        //{
        //    _id = id;
        //    _type = type;
        //    _Lat = Lat;
        //    _Lng = Lng;
        //    _dateCreated = dateCreated;
        //}

        //public static LocationEntity getFakeLocation()
        //{
        //    return new LocationEntity("0", "Test_Type", "0.0", "0.0");
        //}
    }
}
