using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Student : MonoBehaviour {

    public int row;
    public int column;
    bool individualSkillClicked = false;
    public bool disturbActivated;


    public bool activated = true;
    public bool target = false;
    public int cooldown=0;

    public ArrayList links;
    public bool instigator = false;

    Dictionary<string, AudioClip> audioClips;

    // Use this for initialization
    void Start () {
        links = new ArrayList();
        audioClips = new Dictionary<string, AudioClip>();
        audioClips.Add("chat_A", Resources.Load<AudioClip>("sfx/students/chat_A"));
        audioClips.Add("chat_B", Resources.Load<AudioClip>("sfx/students/chat_B"));
        audioClips.Add("dance_disco", Resources.Load<AudioClip>("sfx/students/dance_disco"));
        audioClips.Add("dance_salsa", Resources.Load<AudioClip>("sfx/students/dance_salsa"));
        audioClips.Add("draw", Resources.Load<AudioClip>("sfx/students/draw"));
        audioClips.Add("emails", Resources.Load<AudioClip>("sfx/students/emails"));
        audioClips.Add("funnyNoises_A", Resources.Load<AudioClip>("sfx/students/funnyNoises_A"));
        audioClips.Add("funnyNoises_B", Resources.Load<AudioClip>("sfx/students/funnyNoises_B"));
        audioClips.Add("FunnyNoises_C", Resources.Load<AudioClip>("sfx/students/FunnyNoises_C"));
        audioClips.Add("internet_long", Resources.Load<AudioClip>("sfx/students/internet_long"));
        audioClips.Add("internet_short", Resources.Load<AudioClip>("sfx/students/internet_short"));
        audioClips.Add("jokes_A", Resources.Load<AudioClip>("sfx/students/jokes_A"));
        audioClips.Add("jokes_B", Resources.Load<AudioClip>("sfx/students/jokes_B"));
        audioClips.Add("music_A", Resources.Load<AudioClip>("sfx/students/music_A"));
        audioClips.Add("music_B", Resources.Load<AudioClip>("sfx/students/music_B"));
        audioClips.Add("play_long", Resources.Load<AudioClip>("sfx/students/play_long"));
        audioClips.Add("play_short", Resources.Load<AudioClip>("sfx/students/play_short"));
        audioClips.Add("sleep", Resources.Load<AudioClip>("sfx/students/sleep"));
        audioClips.Add("windowArgue", Resources.Load<AudioClip>("sfx/students/windowArgue"));
    }
	
	// Update is called once per frame
	void Update () {
	

	}

    public void disturb(string conflict)
    {
        // parseo
        if(disturbActivated)
        {
            switch(conflict)
            {
                case "gaming":
                    this.GetComponent<Animator>().Play("student_play", -1, 0f);
                    this.GetComponent<AudioSource>().clip = audioClips["play_long"];
                    break;
                case "screensaver":
                    this.GetComponent<Animator>().Play("student_sleeping", -1, 0f);
                    this.GetComponent<AudioSource>().clip = audioClips["sleep"];
                    break;
                case "internet":
                    this.GetComponent<Animator>().Play("student_netsurfing", -1, 0f);
                    this.GetComponent<AudioSource>().clip = audioClips["internet_long"];
                    break;
                case "mailing":
                    this.GetComponent<Animator>().Play("student_emailing", -1, 0f);
                    this.GetComponent<AudioSource>().clip = audioClips["emails"];
                    break;
                case "annoyingNoises":
                    this.GetComponent<Animator>().Play("student_noises", -1, 0f);
                    this.GetComponent<AudioSource>().clip = audioClips["FunnyNoises_C"];
                    break;
                case "dancing":
                    this.GetComponent<Animator>().Play("student_dancing", -1, 0f);
                    this.GetComponent<AudioSource>().clip = audioClips["dance_salsa"];
                    break;
            }
        }
    }

    public IEnumerator glow()
    {
        individualSkillClicked = true;
        // Rutina de Resaltar estudiante
        while(true)
        {
            for (float f = 1f; f >= 0; f -= 0.01f)
            {
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = f;
                GetComponent<SpriteRenderer>().color = c;
                yield return null;
            }/*
            for (float f = 0f; f >= 1f; f += 0.01f)
            {
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = f;
                GetComponent<SpriteRenderer>().color = c;

            }*/
            yield return null;
        }
    }

    public void unGlow()
    {
        StopCoroutine("glow");
        Color tmp = GetComponent<SpriteRenderer>().color;
        tmp.a = 1f;
        GetComponent<SpriteRenderer>().color = tmp;
    }

    void OnMouseDown()
    {
        if(individualSkillClicked)
        {
            GameObject manager = GameObject.FindGameObjectWithTag("Manager");
            if (manager == null)
            {
                print("no hay manager :(");
            }
            manager.SendMessage("studentClicked", this);

            //adaptar al tipo de skill que le ha tocado
            
        }
    }

    public void setColor(Color color)
    {
        var _renderer = GetComponent<SpriteRenderer>();
        if (_renderer != null)
        {
            _renderer.color = color;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.name == "laserChalk_0" && this.target == true)
        {
            print("Le toca");
            GameObject chalk = col.gameObject;
            chalk.GetComponent<AudioSource>().Play();
            chalk.GetComponent<SpriteRenderer>().enabled = false;
            chalk.GetComponent<Collider2D>().enabled = false;
            setDisturbing(false);
            target = false;
        }
    }

    public void setDisturbing(bool disturbing)
    {
        if(disturbing)
        {
            disturbActivated = true;
        } else
        {
            this.GetComponent<Animator>().Play("student_idle", -1, 0f);
            this.GetComponent<AudioSource>().Stop();
            disturbActivated = false;
        }
    }

    public void setConnected(bool conected)
    {
        if(!conected)
        {
            activated = false;
            cooldown = 5;
            this.GetComponent<Animator>().Play("student_shocked", -1, 0f);
            this.GetComponent<AudioSource>().Stop();
        } else
        {
            activated = true;
            this.GetComponent<Animator>().Play("student_idle", -1, 0f);
            this.GetComponent<AudioSource>().Stop();
        }
    }

    public void link(Student s)
    {
        this.links.Add(s);
        s.links.Add(this);
    }
    public void link(Student a, Student b, Student c)
    {
        // links a
        this.links.Add(a);
        this.links.Add(b);
        this.links.Add(c);

        // links b
        a.links.Add(this);
        a.links.Add(b);
        a.links.Add(c);

        // links c
        b.links.Add(this);
        b.links.Add(a);
        b.links.Add(c);

        // links d
        c.links.Add(this);
        c.links.Add(a);
        c.links.Add(b);
    }
}
