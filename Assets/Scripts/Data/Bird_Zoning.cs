using DataBank;
using Niantic.Lightship.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bird_Zoning : MonoBehaviour
{
    [SerializeField] private LightshipMapView LMV;
    //List<string> Bird_Entities = new List<string>();
    //public string Birds = "";
    [SerializeField] private TMP_Text Birds;
    void Start()
    {
        Populate_List();
    }

    public void Populate_List()
    {
        Birds.text = "";
        BirdDb mBirdDb = new BirdDb();
        Debug.Log("Lat: " + 21.31624 + "\nLng: " + -157.858102);
        System.Data.IDataReader reader 
            = mBirdDb.Close_Birds((float)21.31624, (float)-157.858102);

        int fieldCount = reader.FieldCount;
        //List<BirdEntity> myList = new List<BirdEntity>();
        while (reader.Read())
        {
            //lat, lng, species
            Debug.Log(reader[0].ToString() + " " + reader[1].ToString() + " " + reader[1].ToString());
            Birds.text += reader[2].ToString();
            //BirdEntity entity = new BirdEntity(reader[0].ToString(),
            //                                            reader[1].ToString(),
            //                                            reader[2].ToString());
            //Debug.Log(entity._Species);
            //myList.Add(entity);
        }
    }

    IEnumerator Bird_Location_Ping()
    {
        yield return new WaitForSeconds(1);
        Populate_List();
    }
}
