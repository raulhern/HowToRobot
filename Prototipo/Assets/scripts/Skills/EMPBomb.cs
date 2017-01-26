using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPBomb : Skill {

	// Use this for initialization
	void Start () {
        stressCost = 4;
        skType = SkillType.Massive;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void action(List<Student> s)
    {
        print("bombaso");
        foreach(Student student in s)
        {
            print("estudiante " + student.row + "x" + student.column + " estuneao");
        }
    }
}
