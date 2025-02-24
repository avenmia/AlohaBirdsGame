using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;

public class DbTestBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //Fetch All Data
        BirdDb mBirdDb = new BirdDb();
        System.Data.IDataReader reader = mBirdDb.getAllData();

        int fieldCount = reader.FieldCount;
        List<BirdEntity> myList = new List<BirdEntity>();
        while (reader.Read())
        {
            BirdEntity entity = new BirdEntity(reader[0].ToString(),
                                                        reader[1].ToString(),
                                                        reader[2].ToString());
            Debug.Log(entity._Species);
            myList.Add(entity);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
