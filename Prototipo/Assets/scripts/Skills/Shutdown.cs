using UnityEngine;
using System.Collections;

public class Shutdown : Skill {

    // Use this for initialization
    void Start () {
        stressCost = 1;
        skType = SkillType.Individual;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void action (Student s)
    {
        print("I've shut down student " + s.row + "x" + s.column);
    }
}
