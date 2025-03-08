using DataBank;
using Niantic.Lightship.Maps;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class Bird_Zoning : MonoBehaviour
{
    [SerializeField] private LightshipMapView LMV;
    //List<string> Bird_Entities = new List<string>();
    [SerializeField] private TMP_Text Birds;
    [SerializeField] private List<string> Bird_Names;
    void Start()
    {
        StartCoroutine(Bird_Location_Ping(0));
    }

    public void Populate_List()
    {
        BirdDb mBirdDb = new BirdDb();
        //Debug.Log("Lat: " + 21.31624 + "\nLng: " + -157.858102);
        Vector2 playerLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        //System.Data.IDataReader reader 
        //    = mBirdDb.Close_Birds(playerLocation.x, playerLocation.y);
        System.Data.IDataReader reader
            = mBirdDb.Close_Birds((float)21.31624, (float)-157.858102);

        int fieldCount = reader.FieldCount;
        while (reader.Read())
        {
            //lat, lng, species
            //Debug.Log(reader[0].ToString() + " " + reader[1].ToString() + " " + reader[1].ToString());
            Birds.text += reader[2].ToString();
            string[] words = reader[2].ToString().Split(", ", StringSplitOptions.RemoveEmptyEntries);
            foreach(var bird in words)
            {
                //once we have a dictionary of all birds we have only put in viable bird names
                Bird_Names.Add(bird);
            }
        }
    }

    IEnumerator Spawn_Viable_Bird(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        var randomNum = UnityEngine.Random.Range(0, Bird_Names.Capacity);
        //Instantiate by string
    }

    IEnumerator Bird_Location_Ping(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        Populate_List();
        StartCoroutine(Bird_Location_Ping(30));
    }
}
