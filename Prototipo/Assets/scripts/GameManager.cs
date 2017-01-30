using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// public class haber si memuero
public class GameManager : MonoBehaviour {

    GameObject[] skillGo; // Array a rellenar con skills para poder pasarlos a lista
    GameObject[] studentGo; // Lo mismo, pero con estudiantes
    List<Skill> skills = new List<Skill>();  
    /*
        Esta lista es pública para poder añadir a mano las habilidades de este nivel
        Una vez completado el juego se debería rellenar con la información del menú
    */
    List<Student> students = new List<Student>();
    public Text timer;
    Skill activeSkill;
    float totalLevelTime;


	// Use this for initialization
	void Awake () {
        // Cargar habilidades y estudiantes
        skillGo = GameObject.FindGameObjectsWithTag("Skill");
        foreach (GameObject sk in skillGo) {
            skills.Add(sk.GetComponent<Skill>());
        }
        studentGo = GameObject.FindGameObjectsWithTag("Students");
        foreach (GameObject st in studentGo)
        {
            students.Add(st.GetComponent<Student>());
        }
	}

    void Start()
    {
        totalLevelTime = 120;
    }
	
	// Update is called once per frame
	void Update () {
        totalLevelTime -= Time.deltaTime;
        int timeLeft = (int)totalLevelTime;
        timer.text = timeLeft.ToString();
    }

    void skillClicked(Skill skill)
    {
        if(activeSkill != null)
        {
            activeSkill.unHighlight();
        }
        activeSkill = skill; // Para poder hacer las acciones individuales
        activeSkill.highlight();

        // Distinguir entre habilidad de relax, masiva e individual
        switch(skill.skType)
        {
            case Skill.SkillType.Individual: {
                    highlightStudents();
                    break;
                }
            case Skill.SkillType.Unstress: {
                    activeSkill.action();
                    activeSkill.unHighlight();
                    foreach(Student s in students)
                    {
                        s.unGlow();
                    }
                    break;
                }
            case Skill.SkillType.Massive: {
                    activeSkill.action(students);
                    activeSkill.unHighlight();
                    foreach (Student s in students)
                    {
                        s.unGlow();
                    }
                    break;
                }
        }
    }

    void studentClicked(Student s)
    {
        activeSkill.action(s);
        activeSkill.unHighlight();
        activeSkill = null;
        if(s != null)
        {
            s.unGlow();
        }
    }

    void highlightStudents()
    {
        foreach(Student s in students)
        {
            s.StartCoroutine("glow");
        }
    }

    void studentDestroyed(Student s)
    {
        // Quitar puntos
        students.Remove(s);
    }
}
