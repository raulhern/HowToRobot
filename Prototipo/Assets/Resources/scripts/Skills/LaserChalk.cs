﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserChalk : Skill {

    public bool activateMovement = false;
    float speed = 5;
    Transform target;
    GameObject laserChalk;
    GameObject teacherHand;
    // Use this for initialization
    void Start () {
        stressCost = 2;
        skType = SkillType.Individual;
        TOTAL_COOLDOWN = 15; // 15s
        cooldown = TOTAL_COOLDOWN;
        
        activateMovement = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (target == null)
        {
            activateMovement = false;
        }
        if(activateMovement)
        {
            laserChalk.transform.position = Vector3.MoveTowards(laserChalk.transform.position, target.position, speed * Time.deltaTime);
        }
    }

    public override void action(Student s)
    {
        print("le he tirado una tiza al estudiante " + s.row + "x" + s.column);
        s.target = true;

        // Animación

        laserChalk = GameObject.FindGameObjectWithTag("Laserchalk");
        teacherHand = GameObject.FindGameObjectWithTag("Teacherhand");
        laserChalk.transform.position = teacherHand.transform.position;

        target = s.transform;
        laserChalk.GetComponent<SpriteRenderer>().enabled = true;
        laserChalk.GetComponent<Collider2D>().enabled = true;
        activateMovement = true;

        // Audio
        GetComponent<AudioSource>().Play(); // Reproducir sonido de lanzamiento
        
        this.toggleCooldown();
    }
}
