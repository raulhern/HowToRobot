using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Count1010 : Skill {

	// Use this for initialization
	void Start () {
        stressCost = 2;
        skType = SkillType.Unstress;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void action()
    {
        print("reducir 2 de estrés");
        // Comenzar corrutina de relajarse: gameManager.startCoroutine("Relax", CuantoRelajaEsto);
    }
}
