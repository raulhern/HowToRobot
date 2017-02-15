using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Skill : MonoBehaviour {

    public int stressCost;
    public enum SkillType { Individual,Unstress,Massive};
    public SkillType skType;
    public bool onCooldown = false;
    public int cooldown;
    protected int TOTAL_COOLDOWN;
    protected GameObject gameManager;

    // Use this for initialization
    void Start () {
	}
	
    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("Manager");
    }

	// Update is called once per frame
	void Update () {
	
	}

    public virtual void action()
    {

    }

    public virtual void action(Student s)
    {

    }

    public virtual void action(Student[,]s)
    {

    }

    void OnMouseDown()
    {

        int actualStress= gameManager.GetComponent<GameManager>().stressBars;
        print("Actual stress" +actualStress+" and stress cost is "+ stressCost );
        if (!onCooldown && actualStress>=stressCost)
        {
            setColor(Color.red);
            GameObject manager = GameObject.FindGameObjectWithTag("Manager");
            if (manager == null)
            {
                print("no hay manager :(");
            }
            manager.SendMessage("skillClicked", this);
        }else
        {
            print("NO YOU CAN'T");
        }
    }

    private void setColor(Color color)
    {
        var _renderer = GetComponent<SpriteRenderer>();
        if (_renderer != null)
        {
            _renderer.color = color;
        } else
        {
            print("no hay renderer :(");
        }
    }

    // Podría quedar más bonito con una corrutina glow
    public void highlight()
    {
        this.setColor(Color.red);
    }

    public void unHighlight()
    {
        this.setColor(Color.white);
    }

    public void toggleCooldown()
    {
        if(onCooldown)
        {
            this.onCooldown = false;
            this.setColor(Color.white);
            this.cooldown = TOTAL_COOLDOWN;
        } else
        {
            this.onCooldown = true;
            this.setColor(Color.black);
        }
    }

}
