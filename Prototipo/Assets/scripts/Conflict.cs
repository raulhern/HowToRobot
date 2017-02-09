using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

public class Conflict : MonoBehaviour {

    //public float stressInc;
    //public float taskRed;

    string[] conflictList;
    
    int lineCounter;


    public TextAsset conflictsFile;


    public Conflict(string type, string name)
    {

    }

	// Use this for initialization
	void Start () {

        conflictList = System.IO.File.ReadAllLines(@"Assets/Resources/conflicts/conflictsList.txt");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

     public string getTypeConflict(string nameConflict)
    {
        lineCounter = 0;
        char delimiter = '-';
        
        foreach(string line in conflictList)
        {
            //
            print("Conflicts length: " + conflictList.Length+ " is "+line);
            

            string[] elements = line.Split(delimiter);


            //
            print(elements[0] + "-" + elements[1]);
            if (elements[0] == nameConflict)
            {
                return elements[1];//return if it's individual, dual or massive
            }

            lineCounter++;

        }
        return null;
    }
}
