using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserChalk : Skill {

	// Use this for initialization
	void Start () {
        stressCost = 2;
        skType = SkillType.Individual;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void action(Student s)
    {
        print("le he tirado una tiza al estudiante " + s.row + "x" + s.column);
        // Hacer que el estudiante reaccione: s.atender();
    }
}
