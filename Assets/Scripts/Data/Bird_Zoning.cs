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
    [SerializeField] private TMP_Text Birds;
    [SerializeField] private TMP_Text Spawned;
    [SerializeField] private GameObject Nearby_Birds;
    [SerializeField] private List<string> Bird_Names;
    void Start()
    {
        StartCoroutine(Bird_Location_Ping(0));
    }

    public void Populate_List()
    {
        BirdDb mBirdDb = new BirdDb();

        #if UNITY_ANDROID && !UNITY_EDITOR
            Vector2 playerLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            System.Data.IDataReader reader
                = mBirdDb.Close_Birds(playerLocation.x, playerLocation.y);
        #endif

        #if UNITY_EDITOR
            System.Data.IDataReader reader
                = mBirdDb.Close_Birds((float)21.31624, (float)-157.858102);
        #endif

        //int fieldCount = reader.FieldCount;
        while (reader.Read())
        {
            //lat, lng, species
            //Debug.Log(reader[0].ToString() + " " + reader[1].ToString() + " " + reader[1].ToString());
            Birds.text = "Lat: " + reader[0].ToString() + " Lng: " + reader[1].ToString() + " \nBirds: " + reader[2].ToString();
            Debug.Log("Lat: " + reader[0].ToString() + " Lng: " + reader[1].ToString() + " Birds: " + reader[2].ToString());
            string[] words = reader[2].ToString().Split(", ", StringSplitOptions.RemoveEmptyEntries);
            foreach(var bird in words)
            {
                Bird_Names.Add(bird);
            }
        }
        mBirdDb.close();

        StartCoroutine(Spawn_Viable_Bird(0));
    }

    IEnumerator Spawn_Viable_Bird(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        var randAmt = UnityEngine.Random.Range(1, 3);
        Spawned.text = "Spawned: ";
        for(int i = 0; i < randAmt; i++)
        {
            var randomNum = UnityEngine.Random.Range(0, Bird_Names.Capacity);
            Spawned.text += Bird_Names[randomNum] + " ";
        }

        Bird_Names.Clear();

        StartCoroutine(Fade_Text(Spawned));
    }

    IEnumerator Fade_Text(TMP_Text text)
    {
        text.color = new Color(255, 255, 255, 1);
        yield return new WaitForSeconds(2f);
        for(float i = 1; 0 <= i; i =- 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
            text.color = new Color(255, 255, 255, i);
        }
        text.text = "";

    }

    IEnumerator Bird_Location_Ping(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        Populate_List();
        StartCoroutine(Bird_Location_Ping(30));
    }

    public void Show_Birds()
    {
        Nearby_Birds.SetActive(!Nearby_Birds.activeSelf);
    }
}
