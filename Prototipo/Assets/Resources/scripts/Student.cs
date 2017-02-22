using UnityEngine;
using System.Collections;

public class Student : MonoBehaviour {

    public int row;
    public int column;
    bool individualSkillClicked = false;
    public bool disturbActivated;


    public bool activated = true;
    public bool target = false;
    public int cooldown=0;

	// Use this for initialization
	void Start () {
	
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
                    break;
                case "screensaver":
                    this.GetComponent<Animator>().Play("student_sleeping", -1, 0f);
                    break;
                case "internet":
                    this.GetComponent<Animator>().Play("student_netsurfing", -1, 0f);
                    break;
                case "mailing":
                    this.GetComponent<Animator>().Play("student_emailing", -1, 0f);
                    break;
                case "annoyingNoises":
                    this.GetComponent<Animator>().Play("student_noises", -1, 0f);
                    break;
                case "dancing":
                    this.GetComponent<Animator>().Play("student_dancing", -1, 0f);
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
            toggleDisturb(false);
        }
    }

    public void toggleDisturb(bool disturbing)
    {
        if(disturbing)
        {
            disturbActivated = true;
        } else
        {
            this.GetComponent<Animator>().Play("student_idle", -1, 0f);
            disturbActivated = false;
        }
    }

    public void toggleConnected()
    {
        if(activated)
        {
            activated = false;
            cooldown = 5;
            this.GetComponent<Animator>().Play("student_shocked", -1, 0f);
        } else
        {
            activated = true;
            this.GetComponent<Animator>().Play("student_idle", -1, 0f);
        }
    }
}
