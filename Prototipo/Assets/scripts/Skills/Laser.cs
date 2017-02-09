using UnityEngine;
using System.Collections;

public class Laser : Skill
{

    // Use this for initialization
    void Start()
    {
        stressCost = 1;
        skType = SkillType.Individual;
        TOTAL_COOLDOWN = 1250; // 20s
        cooldown = TOTAL_COOLDOWN;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void action(Student s)
    {
        gameManager.SendMessage("studentDestroyed", s);
        Destroy(s.transform.gameObject,1f);
        print("Debería estar destruido");
        this.toggleCooldown();
    }
}

