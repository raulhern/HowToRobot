using UnityEngine;
using System.Collections;

public class Laser : Skill
{

    LineRenderer line;
    GameObject teacherHand;
    GameObject patata;
    bool LASER = false;
    Transform pos;

    // Use this for initialization
    void Start()
    {
        stressCost = 1;
        skType = SkillType.Individual;
        TOTAL_COOLDOWN = 20; // 20s
        cooldown = TOTAL_COOLDOWN;
        line = GetComponent<LineRenderer>();
        teacherHand = GameObject.FindGameObjectWithTag("Teacherhand");
    }

    // Update is called once per frame
    void Update()
    {
        if(LASER)
        {
            line.SetPosition(0, teacherHand.transform.position);
            line.SetPosition(1, pos.position);
        }
    }

    public override void action(Student s)
    {
        print("He destruido al estudiante " + s.row + "x" + s.column);
        GetComponent<AudioSource>().Play();

        patata = GameObject.FindGameObjectWithTag("Patata");
        line.enabled = true;
        LASER = true;
        pos = patata.transform;

        s.setConnected(false);
        gameManager.SendMessage("studentDestroyed", s);
        this.toggleCooldown();
    }
}

