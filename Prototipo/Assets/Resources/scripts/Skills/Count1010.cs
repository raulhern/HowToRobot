using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Count1010 : Skill {
    
	// Use this for initialization
	void Start () {
        stressCost = 2;
        skType = SkillType.Unstress;
        TOTAL_COOLDOWN = 5; // 5s
        cooldown = TOTAL_COOLDOWN;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void action()
    {
        print("reducir 2 de estrés");
        if (gameManager.GetComponent<GameManager>().stress - 40 < 0)
        {
            gameManager.GetComponent<GameManager>().stress = 0;
            gameManager.GetComponent<GameManager>().setStress(0);
        } else
        {
            gameManager.GetComponent<GameManager>().stress -= 40;
            gameManager.GetComponent<GameManager>().setStress((int)gameManager.GetComponent<GameManager>().stress/2);
        }
        GetComponent<AudioSource>().Play();
        // Comenzar corrutina de relajarse: gameManager.startCoroutine("Relax", CuantoRelajaEsto);
        this.toggleCooldown();
    }
}
