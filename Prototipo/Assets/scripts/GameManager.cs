using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Xml;

using UnityEngine.SceneManagement;

// public class haber si memuero
public class GameManager : MonoBehaviour {

    // GameObjects array
    GameObject[] skillGo; // Array a rellenar con skills para poder pasarlos a lista
    GameObject[] studentGo; // Lo mismo, pero con estudiantes


    public GameObject killedSprite;


    // Lists
    List<Skill> skills = new List<Skill>();  
    /*
        Esta lista es pública para poder añadir a mano las habilidades de este nivel
        Una vez completado el juego se debería rellenar con la información del menú
    */
	public Student[,] students;
    ArrayList listeningStudents;    // Lista de estudiantes atendiendo
    ArrayList disturbingStudents;   // Lista de estudiantes molestando
																  
    List<Task> tasks = new List<Task>();

    // Iterators and selectors
    Skill activeSkill;
    int actualTask;

    // counters
    float totalLevelTime;
    public float stress;
    float taskCompletion;
    float totalTaskDuration;
	
	
    public static int rows;
    public static int columns;

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



    //For the level .xml file reading

     public string level;
    public TextAsset levelsFile;
    
    public XmlDocument readerXML=new XmlDocument();
    List<string> conflicts=new List<string>();
    int lineCounter;

    string[] elements=new string[3];
    float timesinceStart;

    char delimiter = '-';



    //conflict activated
    string actualConflict;

    //Mode of the task
    string taskType;

    int minutesSinceStart, secondsSinceStart,timer1,timer2;

    // Use this for initialization
    void Awake () {
	 // En este caso, hay que llenar columns y rows a pincho, pero deberían leerse del archivo
        rows = 4;
        columns = 3;
        students = new Student[rows, columns];																							  
        // Cargar habilidades y estudiantes
        skillGo = GameObject.FindGameObjectsWithTag("Skill");
        foreach (GameObject sk in skillGo) {
            skills.Add(sk.GetComponent<Skill>());
        }
        studentGo = GameObject.FindGameObjectsWithTag("Students");
		
		
		listeningStudents = new ArrayList();
        disturbingStudents = new ArrayList();									
        foreach (GameObject st in studentGo)
        {
            Student s = st.GetComponent<Student>();
			students[s.row, s.column] = s;
            listeningStudents.Add(s);							  
        }

        stress = 0;
        actualTask = 1;
	}

    void Start()
    {
        totalLevelTime = totalTime;

        

        readerXML.Load(@"Assets/Resources/levels/" + level + ".xml");

       

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
            killedSprite.SetActive(true);
            if (Input.GetMouseButtonDown(0))

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

		foreach(Skill sk in skills)
        {
            // print(sk.name + " tiene " + sk.cooldown + " de cooldown");
            if (sk.cooldown > 1 && sk.onCooldown)
            {
                sk.cooldown--;

            }
            else if (sk.cooldown == 1 && sk.onCooldown)
            {
                sk.cooldown--;
                sk.toggleCooldown();
            }
        }


        switch (taskType)
        {
            case "Talk":
                //Increasing task each seconds (keeping in mind the multiplier), we have to alsokeep in mind the ones destroyed
                timer2 = timer1;
                timer1 = (int)Time.realtimeSinceStartup;
                if (timer1 > timer2)
                {
                    taskCompletion = taskCompletion - (1 * taskMultiplier);//every second we 
                }
                break;
            case "Work":
                if (totalStudentsDisturbing > 0)//If there is someone disturnbing, keep in mind that they could be destroyed will not affect
                {
                    taskMultiplier = 0;
                    timer2 = timer1;
                    timer1 = (int)Time.realtimeSinceStartup;
                    if (timer1 > timer2)
                    {
                        taskCompletion = taskCompletion - (1 * taskMultiplier);//every second we 
                    }
                }
                else
                {
                    //Increasing task each seconds (keeping in mind the multiplier)
                    timer2 = timer1;
                    timer1 = (int)Time.realtimeSinceStartup;
                    if (timer1 > timer2)
                    {
                        taskCompletion = taskCompletion - (1 * taskMultiplier);//every second we 
                    }
                }
                break;
            case "Blackboard":
                break;
            default:
                print("ERROR");
                break;
        }
        
        


        // Incremento de tarea
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
        if ((lineCounter < conflicts.Count)) //change to foreach (string row in lines)
        {
            timesinceStart = Time.realtimeSinceStartup;
            secondsSinceStart = (int)timesinceStart % 60;
            minutesSinceStart = (int)timesinceStart / 60;
            string time = minutesSinceStart + ":" + secondsSinceStart.ToString("00");

            print(time);
            elements = conflicts[lineCounter].Split(delimiter);

            if ((conflicts[lineCounter] != "") && (elements[0] == time))
            {

                //We activate the conflict with the same name as elements[1]
                //
                print(elements[1] + " conflict has been activated...");
                print(elements[1].GetType());

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
                    
                    unHighlightStudents();
                    break;
                }
            case Skill.SkillType.Massive: {
                    activeSkill.action(students);
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
        int randIndex;
        switch (typeConflict)
        {
            case "individual":

				  
                print("And is type individual");

                if(listeningStudents.Count > 0)
                {
                    randIndex = Random.Range(0, listeningStudents.Count);
                    setDisturbingStudent(randIndex);
												  
											  
												  
												 
																	  
														 
							  
					 
                }

                stressMultiplier += 0.1f;
                taskMultiplier -= 0.1f;
               
                break;
            case "dual":

				  
                print("And is type dual");

                if (listeningStudents.Count > 1)
											   
                {
                    // Falta conectarlos entre ellos
                    randIndex = Random.Range(0, listeningStudents.Count);
                    setDisturbingStudent(randIndex);
                    randIndex = Random.Range(0, listeningStudents.Count);
                    setDisturbingStudent(randIndex);
											   
												 
							
								   
								  
					 
                }

                stressMultiplier += 0.3f;
                taskMultiplier -= 0.3f;

                break;
            case "massive":
              
                print("And is type massive");

                if (listeningStudents.Count > 3)
						   
											   
                {
                    // Falta conectarlos entre ellos
                    randIndex = Random.Range(0, listeningStudents.Count);
                    setDisturbingStudent(randIndex);
                    randIndex = Random.Range(0, listeningStudents.Count);
                    setDisturbingStudent(randIndex);
                    randIndex = Random.Range(0, listeningStudents.Count);
                    setDisturbingStudent(randIndex);
                    randIndex = Random.Range(0, listeningStudents.Count);
                    setDisturbingStudent(randIndex);
									
								  
					 
                }

                stressMultiplier += 0.3f;
                taskMultiplier -= 0.3f;

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
        students[s.row, s.column] = null;
											 
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

        taskType = tasks[iterator].tkType.ToString();//define the kind of task we are currently doing
        
    }



    //Task reader
    void readTasks()
    {
        print("EMPECEMOS CON EL XML");
        XmlNodeList nodesDocument = readerXML.DocumentElement.SelectNodes("/Level/Tasks/Task");
        
        foreach(XmlNode node in nodesDocument)//reading from the XML
        {
            
            elements[0]= node.SelectSingleNode("Type").InnerText;
            elements[1] = node.SelectSingleNode("Title").InnerText;
            elements[2] = node.SelectSingleNode("Duration").InnerText;
            print("Type: " + elements[0]);
            print("Title: " + elements[1]);
            print("Duration: " + elements[2]);
            tasks.Add(new Task(elements[0], elements[1], float.Parse(elements[2])));

        }

        nodesDocument = readerXML.DocumentElement.SelectNodes("/Level/Walkthrough/Event");

        print("Y AHORA LOS CONFLICTOS WOP WOP");
        print(nodesDocument.Count);
        foreach (XmlNode node in nodesDocument)//reading from the XML
        {
            print("Time: " + node.SelectSingleNode("Time").InnerText);
            print("Title: " + node.SelectSingleNode("Type").InnerText);
            conflicts.Add(node.SelectSingleNode("Time").InnerText + "-" + node.SelectSingleNode("Type").InnerText);
            
        }

        print("Hay " + conflicts.Count + " conflictos");



            print("--STARTING WALKTHROUGH--");
    }

    public void setStress(int stressLevel)
    {
        stressBar.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/stressBar_" + stressLevel);
    }
	private void setDisturbingStudent(int randIndex)
    {
        Student s = (Student)listeningStudents[randIndex];
        s.disturbActivated = true;
        s.setColor(Color.red);
        disturbingStudents.Add(s);
        listeningStudents.Remove(s);
    }											  
}
