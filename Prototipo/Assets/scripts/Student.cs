using UnityEngine;
using System.Collections;

public class Student : MonoBehaviour {

    public int row;
    public int column;
    bool individualSkillClicked = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator glow()
    {
        individualSkillClicked = true;
        // Rutina de Resaltar estudiante
        while(true)
        {
            for (float f = 1f; f >= 0; f -= 0.01f)
            {
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = f;
                GetComponent<SpriteRenderer>().color = c;
                yield return null;
            }/*
            for (float f = 0f; f >= 1f; f += 0.01f)
            {
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = f;
                GetComponent<SpriteRenderer>().color = c;

            }*/
            yield return null;
        }
    }

    public void unGlow()
    {
        StopCoroutine("glow");
    }

    void OnMouseDown()
    {
        if(individualSkillClicked)
        {
            GameObject manager = GameObject.FindGameObjectWithTag("Manager");
            if (manager == null)
            {
                print("no hay manager :(");
            }
            manager.SendMessage("studentClicked", this);
        }
    }
}
