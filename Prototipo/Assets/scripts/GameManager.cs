using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

// public class haber si memuero
public class GameManager : MonoBehaviour {

    // GameObjects array
    GameObject[] skillGo; // Array a rellenar con skills para poder pasarlos a lista
    GameObject[] studentGo; // Lo mismo, pero con estudiantes
    GameObject[] taskGo;

    // Lists
    List<Skill> skills = new List<Skill>();  
    /*
        Esta lista es pública para poder añadir a mano las habilidades de este nivel
        Una vez completado el juego se debería rellenar con la información del menú
    */
    List<Student> students = new List<Student>();
    List<Task> tasks = new List<Task>();

    // Iterators and selectors
    Skill activeSkill;
    int actualTask;

    // counters
    float totalLevelTime;
    public float stress;
    float taskCompletion;
    float totalTaskDuration;

    // Multipliers
    float taskMultiplier = 1;
    float stressMultiplier = 0;

    public Text timerText;
    public Text taskText;
    public GameObject stressBar;
    public Text taskCompletionText;

    //Time for the class to end, is public to change it from the editor
    public int totalTime; //we could put it in the level .txt file as well



    //Counters
    float totalStudentsDisturbing = 0;



    //For the level .txt file reading

    public TextAsset levelsFile;

    string[] lines;
    int lineCounter;
    public string level;

    string[] elements;
    float timesinceStart;

    char delimiter = '-';

    string actualConflict;
    

    int minutesSinceStart, secondsSinceStart,timer1,timer2;

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
        //taskGo = GameObject.FindGameObjectsWithTag("Task");
        //foreach (GameObject ts in taskGo)
        //{
        //    tasks.Add(ts.GetComponent<Task>());
        //}

        stress = 0;
        actualTask = 1;
	}

    void Start()
    {
        totalLevelTime = totalTime;

        //lines = System.IO.File.ReadAllLines(@"Assets/Resources/levels/" + level + ".txt");
        lines = levelsFile.text.Split('\n');
        foreach (string s in lines)
            print(s);

        readTasks();


        
        setTask(actualTask);
        timer1 = (int)Time.realtimeSinceStartup;
    }



    // Update is called once per frame
    void Update () {
        // Cuenta atrás
        totalLevelTime -= Time.deltaTime;
        int timeLeft = (int)totalLevelTime;
        timerText.text = timeLeft.ToString();
        
        // Aumento de estrés
        stress += 1 * stressMultiplier;

        // Cambio de barra de estrés
        if(stress > 20 && stress <= 40)
        {
            setStress(1);
        } else if (stress > 40 && stress <= 60)
        {
            setStress(2);
        } else if (stress > 60 && stress <= 80)
        {
            setStress(3);
        } else if (stress > 80 && stress <= 100)
        {
            setStress(4);
        } else if (stress > 100)
        {
            setStress(5);
            print("THASMORIO");
            // PUN
        }


        //Decrease cooldown students
        foreach(Student s in students)
        {
            if (s.cooldown > 1) {
                s.cooldown--;
                
            }else if (s.cooldown == 1)
            {
                s.cooldown--;
                s.activated = true;
            }
            
        }

        //Decrease cooldown skills




        //Increasing task each seconds (keeping in mind the multiplier)
        timer2 = timer1;
        timer1 = (int)Time.realtimeSinceStartup;
        if (timer1 > timer2)
        {
            taskCompletion=taskCompletion-(1*taskMultiplier);//every second we 
        }


        // Incremento de tarea
        //taskCompletion -= 1 * taskMultiplier;
        float percentage = (100 * (totalTaskDuration - taskCompletion)) / totalTaskDuration;
        taskCompletionText.text = ((int)percentage).ToString() + "%";
        print("Task "+tasks[actualTask].title+ " needs "+ taskCompletion+ " more seconds to finish");
        //Si la tarea se completa
        if (taskCompletion <= 0)
        {
            actualTask++;
            setTask(actualTask);
        }

        //activate conflicts
        if ((lineCounter < lines.Length)) //change to foreach (string row in lines)
        {
            timesinceStart = Time.realtimeSinceStartup;
            secondsSinceStart = (int)timesinceStart % 60;
            minutesSinceStart = (int)timesinceStart / 60;
            string time = minutesSinceStart + ":" + secondsSinceStart.ToString("00");

            print(time);
            elements = lines[lineCounter].Split(delimiter);

            if ((lines[lineCounter] != "") && (elements[0] == time))
            {

                //We activate the conflict with the same name as elements[1]
                //
                print(elements[1] + " conflict has been activated...");

                actualConflict = GetComponent<Conflict>().getTypeConflict(elements[1]);



                activateConflict(actualConflict);

                print("Time: " + elements[0]);
                print("Type: " + elements[1]);

                lineCounter++;
            }
        }


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
                    unHighlightStudents();
                    break;
                }
            case Skill.SkillType.Massive: {
                    activeSkill.action(students);
                    activeSkill.unHighlight();
                    unHighlightStudents();
                    foreach(Student s in students)
                    {
                        s.setColor(Color.white);
                        s.disturbActivated = false;
                    }
                    taskMultiplier = 1;
                    stressMultiplier = 0;
                    break;
                }
        }
    }

    void studentClicked(Student s)
    {
        activeSkill.action(s);
        activeSkill.unHighlight();
        activeSkill = null;
        unHighlightStudents();
        s.setColor(Color.white);
        s.disturbActivated = false;
        if(stressMultiplier >= 0.1f)
        {
            stressMultiplier -= 0.1f;
        }
        if (taskMultiplier <= 0.9f)
        {
            taskMultiplier += 0.1f;
        }
        
    }



    void activateConflict(string typeConflict)
    {
        switch (typeConflict)
        {
            case "individual":

                //
                print("And is type individual");
                foreach(Student s in students)
                {
                    if (s.disturbActivated == false)
                    {
                        s.disturbActivated = true;
                        s.setColor(Color.red);
                        totalStudentsDisturbing++;
                        stressMultiplier += 0.1f;
                        taskMultiplier -= 0.1f;
                        break;
                    }
                }
               
                break;
            case "dual":

                //
                print("And is type dual");

                int i = 0;
                foreach (Student s in students)
                {
                    if (s.disturbActivated == false)
                    {
                        s.disturbActivated = true;
                        s.setColor(Color.red);
                        totalStudentsDisturbing++;
                        taskMultiplier -= 0.1f;
                        stressMultiplier += 0.1f;
                        i++;
                        if (i == 2)
                            break;
                    }
                }
                break;
            case "massive":
                //
                print("And is type massive");


                int i2 = 0;
                foreach (Student s in students)
                {
                    if (s.disturbActivated == false)
                    {
                     //implementar liante y asociados
                        s.disturbActivated = true;
                        s.setColor(Color.red);
                        totalStudentsDisturbing++;
                        taskMultiplier -= 0.1f;
                        stressMultiplier += 0.1f;
                        i2++;
                        if (i2 == 4)
                            break;
                    }
                }

                break;
            default:
                break;
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

    void unHighlightStudents()
    {
        foreach (Student s in students)
        {
            s.unGlow();
        }
    }

    // Task special methods
    void startTalkTask()
    {
        taskMultiplier = 1;
        // poner de cara a la clase y a los alumnos a atender
    }

    void startWorkTask()
    {
        taskMultiplier = 0;
        // poner de cara a la clase y a los alumnos a escribir
    }

    void startBlackboardTask()
    {
        taskMultiplier = 1;
        // escribir en la pizarra y a los alumnos a atender
    }

    void conflictStarted(float multiplier)
    {
        taskMultiplier -= multiplier;
        stressMultiplier += multiplier;
    }

    void setTask(int iterator)
    {
        taskCompletion = tasks[iterator].duration;
        totalTaskDuration = taskCompletion;
        taskText.text = tasks[iterator].title;
    }



    //Task reader
    void readTasks()
    {
        if (lines[lineCounter] == "--\n")
        {
            print("WOAH MAN");
        }

        while (lines[lineCounter] != "--")
        {
            print(lines[lineCounter]);

            elements = lines[lineCounter].Split(delimiter);
            print(lineCounter);
            //
            print("Type: " + elements[0]);
            print("Title: " + elements[1]);
            print("Duration: " + elements[2]);

            //Creates a task, the time it keeps to complete it (elements[2]) we define it in each level .txt file or we keep all of them in a DB?
            tasks.Add(new Task(elements[0], elements[1], float.Parse(elements[2]))); //We add all the tasks of the file in a list
            
            

            lineCounter++;
        }
        lineCounter++;

        print("--STARTING WALKTHROUGH--");
    }

    public void setStress(int stressLevel)
    {
        stressBar.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/stressBar_" + stressLevel);
    }
}
