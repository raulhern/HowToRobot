using UnityEngine;
using System.Collections;

public class Laser : Skill
{

    // Use this for initialization
    void Start()
    {
        stressCost = 1;
        skType = SkillType.Individual;
        TOTAL_COOLDOWN = 20; // 20s
        cooldown = TOTAL_COOLDOWN;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void action(Student s)
    {
        print("He destruido al estudiante " + s.row + "x" + s.column);
        gameManager.SendMessage("studentDestroyed", s);
        GetComponent<AudioSource>().Play();
        this.toggleCooldown();
    }
}

