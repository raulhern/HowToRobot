using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPBomb : Skill {

	// Use this for initialization
	void Start () {
        stressCost = 4;
        skType = SkillType.Massive;
        TOTAL_COOLDOWN = 30; // 30s
        cooldown = TOTAL_COOLDOWN;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void action(Student[,] s)
    {
        print("bombaso");
        foreach(Student student in s)
        {
            print("estudiante " + student.row + "x" + student.column + " estuneao");
        }
        // Animación
        GameObject bomb = GameObject.FindGameObjectWithTag("Bomb");
        bomb.GetComponent<SpriteRenderer>().enabled = true;
        bomb.GetComponent<Animator>().Play("empBomb", -1, 0f);

        // Audio
        GetComponent<AudioSource>().Play();

        this.toggleCooldown();
    }
}
