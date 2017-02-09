using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserChalk : Skill {

	// Use this for initialization
	void Start () {
        stressCost = 2;
        skType = SkillType.Individual;
        TOTAL_COOLDOWN = 937; // 15s
        cooldown = TOTAL_COOLDOWN;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void action(Student s)
    {
        print("le he tirado una tiza al estudiante " + s.row + "x" + s.column);
        s.disturbActivated = false;
        // Esto, no? Dejar de molestar because yes
        this.toggleCooldown();
    }
}
