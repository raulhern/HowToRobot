using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    GameObject[] skillGo;
    List<Skill> skills = new List<Skill>();
    /*
        Esta lista es pública para poder añadir a mano las habilidades de este nivel
        Una vez completado el juego se debería rellenar con la información del menú
    */
    public List<Student> students = new List<Student>();


	// Use this for initialization
	void Start () {
        skillGo = GameObject.FindGameObjectsWithTag("Skill");
        foreach (GameObject sk in skillGo) {
            skills.Add(sk.GetComponent<Skill>());
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void skillClicked(Skill skill)
    {
        foreach(Skill sk in skills)
        {
            if(sk.pos != skill.pos)
            {
                sk.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        switch(skill.skType)
        {
            case Skill.SkillType.Individual: {
                    break;
                }
            case Skill.SkillType.Unstress: {
                    break;
                }
            case Skill.SkillType.Massive: {
                    break;
                }
        }
    }
}
