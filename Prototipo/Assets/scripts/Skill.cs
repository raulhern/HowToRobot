using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Skill : MonoBehaviour {

    public int stressCost;
    public enum SkillType { Individual,Unstress,Massive};
    public SkillType skType;
    public int pos; // No sé por qué tengo esto
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

    public virtual void action(List<Student> s)
    {

    }

    void OnMouseDown()
    {
        setColor(Color.red);
        GameObject manager = GameObject.FindGameObjectWithTag("Manager");
        if (manager == null)
        {
            print("no hay manager :(");
        }
        manager.SendMessage("skillClicked", this);
    }

    private void setColor(Color color)
    {
        var _renderer = GetComponent<SpriteRenderer>();
        if (_renderer != null)
        {
            _renderer.color = color;
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

}
