using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class Skill : MonoBehaviour, IPointerDownHandler
{

    public int stressCost;
    public enum SkillType { Individual,Unstress,Massive};
    public SkillType skType;
    public bool onCooldown = false;
    public int cooldown;
    protected int TOTAL_COOLDOWN;
    protected GameObject gameManager;
    int actualStress;

    Skill radialLoad;
    Image butComp;


    Image _image;

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


    public void OnPointerDown(PointerEventData eventData)
    {
        
        actualStress = gameManager.GetComponent<GameManager>().stressBars;

        print("Actual stress" + actualStress + " and stress cost is " + stressCost);
        if (!onCooldown && actualStress >= stressCost&& GetComponent<Button>().IsInteractable())
        {
            if (GetComponent<Button>().IsInteractable() == true)
            {
                SetInteractable(false);
            }
            else //Else make it interactable
            {
                SetInteractable(true);
            }
            GameObject manager = GameObject.FindGameObjectWithTag("Manager");
            if (manager == null)
            {
                print("no hay manager :(");
            }
            manager.SendMessage("skillClicked", this);

        }
        else
        {
            print("NO YOU CAN'T");
        }
    }

    //void OnMouseDown()
    //{

    //    actualStress = gameManager.GetComponent<GameManager>().stressBars;

    //    print("Actual stress" +actualStress+" and stress cost is "+ stressCost );
    //    if (!onCooldown && actualStress>=stressCost)
    //    {
    //        setColor(Color.red);
    //        GameObject manager = GameObject.FindGameObjectWithTag("Manager");
    //        if (manager == null)
    //        {
    //            print("no hay manager :(");
    //        }
    //        manager.SendMessage("skillClicked", this);
    //    }else
    //    {
    //        print("NO YOU CAN'T");
    //    }
    //}

     public void setColor(Color color)
    {
        var _renderer = GetComponent<SpriteRenderer>();
        _image = GetComponent<Image>();
        if (_renderer != null || _image != null)
        {
            _image.color = color;
            //_renderer.color = color;
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
        this.SetInteractable(true);
    }



    public void toggleCooldown()
    {
        if(onCooldown)
        {
            this.onCooldown = false;
            //this.setColor(Color.white);
            
            SetInteractable(true);
            this.cooldown = TOTAL_COOLDOWN;
        } else
        {
            
            radialLoad = Instantiate(this,this.transform.parent);
            radialLoad.tag = "Untagged";
            butComp = radialLoad.GetComponent<Image>();
            butComp.fillAmount = 1f;
            butComp.color = Color.black;
            this.onCooldown = true;
        }
    }

    public void decreaseCooldownBar()
    {
        print("El cooldown de esta skill es " + cooldown + " de un total de " + TOTAL_COOLDOWN);
        float percentage = (float)(cooldown) / (float)TOTAL_COOLDOWN;
        print("Entonces el fillamount es " +percentage );
        butComp.fillAmount = percentage;
        if (butComp.fillAmount <=0) { 
            print("destroying");
            Destroy(radialLoad.gameObject);//por que no se destruye
        }
    }

    public void SetInteractable(bool interectable)
    {

        GetComponent<Button>().interactable = interectable;

    }

}
