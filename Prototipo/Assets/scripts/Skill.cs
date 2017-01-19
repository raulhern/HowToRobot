using UnityEngine;
using System.Collections;

public abstract class Skill : MonoBehaviour {

    public int stressCost;
    public enum SkillType { Individual,Unstress,Massive};
    public SkillType skType;
    public int pos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public virtual void Action()
    {

    }

    public virtual void Action(Student s)
    {

    }

    void OnMouseDown()
    {
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        GameObject manager = GameObject.FindGameObjectWithTag("Manager");
        if (manager == null)
        {
            print("no hay manager :(");
        }
        manager.SendMessage("skillClicked", this);
    }
}
