using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Xml;

using UnityEngine.SceneManagement;

// public class haber si memuero
public class GameManager : MonoBehaviour
{
    
    public Canvas pauseMenu;

    public Toggle musicToggle;
    // GameObjects array
    GameObject[] skillGo; // Array a rellenar con skills para poder pasarlos a lista
    GameObject[] studentGo; // Lo mismo, pero con estudiantes


    public GameObject killedSprite,timeUpSprite;


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
    public int stressBars;
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

    public XmlDocument readerXML = new XmlDocument();
    List<string> conflicts = new List<string>();
    int lineCounter;

    string[] elements = new string[3];
    float timesinceStart;

    char delimiter = '-';



    //conflict activated
    string actualConflict;

    //Mode of the task
    string taskType;
    int timeLeft;
    string timeText, timeLeftText;
    int minutesSinceStart, secondsSinceStart, minutesTimeLeft, secondsTimeLeft, timer1, timer2;


    // Stress Bar SpriteSheet
    Sprite[] stressBarSprites;

    string time;
    bool ending;


    //GameOver parameters
    string deadby;

    // Use this for initialization
    void Awake()
    {
        // En este caso, hay que llenar columns y rows a pincho, pero deberían leerse del archivo
        rows = 4;
        columns = 3;
        students = new Student[rows, columns];
        // Cargar habilidades y estudiantes
        skillGo = GameObject.FindGameObjectsWithTag("Skill");
        foreach (GameObject sk in skillGo)
        {
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

        readTasks();

        setTask(actualTask);
        updateTime();

        timer1 = (int)Time.time;
	        stressBarSprites = Resources.LoadAll<Sprite>("sprites/stressBar_spriteSheet");
																				  
    }



    // Update is called once per frame
    void Update()
    {
        // Cuenta atrás



        setMusic();


        timer2 = timer1;
        timer1 = (int)Time.time;
        if (timer1 > timer2) //We do all the stuff  each second
        {

            print("Quedan " + totalTime + " segundos");
             timeLeft = totalTime--;
			
			
            //secondsTimeLeft = (int)timeLeft % 60;
            //minutesTimeLeft = (int)timeLeft / 60;
            //timeLeftText = minutesTimeLeft + ":" + secondsTimeLeft.ToString("00");
            //timerText.text = timeLeftText;

            if (timeLeft <= 0)
            {
                print("END OF THE CLASS");
                deadby = "timeup";
                StartCoroutine(gameOver(deadby));
                
                //yield return(gameOver(deadby));
                
            }
            else if (deadby == "explosion")
            {
                StartCoroutine(gameOver(deadby));
            }
            else
            {
                updateTime();
                totalStudentsDisturbing = disturbingStudents.Count;
				if(totalStudentsDisturbing == 0) // Control cada segundo de que la clase vaya bien
                {
                    stressMultiplier = 0;
                    taskMultiplier = 1;
                }																				  
                print("there are " + totalStudentsDisturbing + " students disturbing");

                // Stress increasing
                stress += 1 * stressMultiplier * 10;//we us the *10 to increase it faster since we synchronized with time instead of updates
				if(stress>100){
                    print("THASMORIO");
                    deadby = "explosion";
                    stress = 100;
                    StartCoroutine(gameOver(deadby));
				}
                setStress(stress / 2);


                //Decrease cooldown students
                foreach (Student s in students)
                {
                    if(s!=null)
                    {
                        if (s.cooldown > 1)
                        {
                            s.cooldown--;
                        }
                        else if (s.cooldown == 1)
                        {
                            s.cooldown--;
                            s.setConnected(true);
                            listeningStudents.Add(s);
                        }
                    }
                    

                }

                //Decrease cooldown skills

                foreach (Skill sk in skills)
                {


                    if (stressBars < sk.stressCost && !sk.onCooldown)
                        sk.setColor(Color.gray);
                    else if (stressBars >= sk.stressCost && !sk.onCooldown)
                        sk.setColor(Color.white);
                    


                    // print(sk.name + " tiene " + sk.cooldown + " de cooldown");
                    if (sk.cooldown > 1 && sk.onCooldown)
                    {
                        sk.cooldown--;
                        sk.decreaseCooldownBar();

                    }
                    else if (sk.cooldown == 1 && sk.onCooldown)
                    {
                        sk.cooldown--;

                        sk.decreaseCooldownBar();
                        sk.toggleCooldown();
                    }
                }


                switch (taskType)
                {
                    case "Talk":
                        //Increasing task each seconds (keeping in mind the multiplier), we have to alsokeep in mind the ones destroyed


                        taskCompletion = taskCompletion - (1 * taskMultiplier);//every second we 

                        break;
                    case "Work":
                        if (totalStudentsDisturbing == 0)//If there is someone disturnbing, keep in mind that they could be destroyed will not affect
                        {

                            taskMultiplier = 1;
                            taskCompletion = taskCompletion - (1 * taskMultiplier);//every second we 

                        }
                        else
                        {
                            //Increasing task each seconds (keeping in mind the multiplier)
                            taskMultiplier = 0; 
                            taskCompletion = taskCompletion - (1 * taskMultiplier);

                        }
                        break;
                    case "Blackboard":
                        //Increasing task each seconds (keeping in mind the multiplier), we have to alsokeep in mind the ones destroyed


                        taskCompletion = taskCompletion - (1 * taskMultiplier);//for now we do it as in Talk tasks, later we have to adapt it
                        break;
                    default:
                        print("ERROR");
                        break;
                }




                 //Si la tarea se completa
                if (taskCompletion <= 0)
                {
                    actualTask++;
                    setTask(actualTask);

                }
                // Incremento de tarea
                float percentage = (100 * (totalTaskDuration - taskCompletion)) / totalTaskDuration;

                taskCompletionText.text = ((int)percentage).ToString() + "%";
                print("Task " + tasks[actualTask].title + " needs " + taskCompletion + " more seconds to finish");


                //activate conflicts
                if ((lineCounter < conflicts.Count)) //change to foreach (string row in lines)
                {
                    timesinceStart++;
                    secondsSinceStart = (int)timesinceStart % 60;
                    minutesSinceStart = (int)timesinceStart / 60;
                    timeText = minutesSinceStart + ":" + secondsSinceStart.ToString("00");

                    print(timeText);
                    elements = conflicts[lineCounter].Split(delimiter);

                    if ((conflicts[lineCounter] != "") && (elements[0] == timeText))
                    {

                        //We activate the conflict with the same name as elements[1]
                        //
                        print(elements[1] + " conflict has been activated...");
                        print(elements[1].GetType());

                        actualConflict = GetComponent<Conflict>().getTypeConflict(elements[1]);



                        activateConflict(actualConflict,elements[1]);

                        print("Time: " + elements[0]);
                        print("Type: " + elements[1]);

                        lineCounter++;
                    }
                }
            }
        }



    }

	
	   void skillClicked(Skill skill)
    {
        if(activeSkill == skill)
        {
            skill.unHighlight();
            unHighlightStudents();
            activeSkill = null;
        }
        activeSkill = skill; // Para poder hacer las acciones individuales
        activeSkill.highlight();
        foreach (Skill sk in skills) // Desactivar habilidades inactivas
        {
            if (sk != activeSkill && !sk.onCooldown)
            {
                sk.unHighlight();
            }
        }
        // Distinguir entre habilidad de relax, masiva e individual
        switch (skill.skType)
        {
            case Skill.SkillType.Individual:
                {
                    highlightStudents();
                    break;
                }
            case Skill.SkillType.Unstress:
                {
                    activeSkill.action();

                    unHighlightStudents();
                    break;
                }
            case Skill.SkillType.Massive:
                {
                    activeSkill.action(students);
                    unHighlightStudents();
                    foreach (Student s in students)
                    {
                        if (s != null)
                        {
                            s.setColor(Color.white);

                            //we put it again in the listening students list
                            disturbingStudents.Remove(s);
                            if (!s.activated)
                            {
                                listeningStudents.Remove(s);
                            }
                            s.links.Clear();
                        }
                    }
                    taskMultiplier = 1;
                    stressMultiplier = 0;
                    break;
                }
        }
    }

    void studentClicked(Student s)
    {
							  
        if(activeSkill != null)
        {
            float newMultiplier = 0.1f;
								   

            activeSkill.action(s);
            activeSkill = null;
            unHighlightStudents();
            s.setColor(Color.white);

            //we put it again in the listening students list
            disturbingStudents.Remove(s);
            if (s.activated) // Controlamos que no le hayamos apagado
            {
                listeningStudents.Add(s);
            }

            // CONDICIONALES DE ENGANCHE
            if (s.links.Count == 1) // Solo tiene un compañero (DUAL O MASIVO Y SOLO QUEDAN DOS)
            {
                Student companion = (Student)s.links[0];
                companion.setDisturbing(false);
                disturbingStudents.Remove(companion);
                listeningStudents.Add(companion);
                companion.links.Clear();

                newMultiplier = 0.3f;
            }
            else if (s.links.Count > 1 && s.instigator) // HEMOS PILLAO AL MALO
            {
                foreach (Student student in s.links)
                {
                    student.setDisturbing(false);
                    disturbingStudents.Remove(student);
                    listeningStudents.Add(student);
                    student.links.Clear();
                    newMultiplier = 0.75f;
                }
                s.instigator = false;
            }
            else if (s.links.Count > 1 && !s.instigator) // LA SIGUEN LIANDO
            {
                foreach (Student student in s.links)
                {
                    student.links.Remove(s);
                }
            }

            s.links.Clear();

            if (stressMultiplier >= 0.1f)
            {
                stressMultiplier -= newMultiplier;
            }
            if (taskMultiplier <= 0.9f)
            {
                taskMultiplier += newMultiplier;
            }
        }
    }



    void activateConflict(string typeConflict, string conflict)
    {
        int randIndex;
        switch (typeConflict)
        {
            case "individual":


                print("And is type individual");

                if (listeningStudents.Count > 0)
                {
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student1 = (Student)listeningStudents[randIndex];
                    setDisturbingStudent(student1, conflict);
                    student1.GetComponent<AudioSource>().Play();
                    changeMultiplier(0.1f);
                }
                
										 
									   

                break;
            case "dual":


                print("And is type dual");

                if (listeningStudents.Count > 1)

                {
                    // Falta conectarlos entre ellos
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student1 = (Student)listeningStudents[randIndex];
                    setDisturbingStudent(student1, conflict);
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student2 = (Student)listeningStudents[randIndex];
                    setDisturbingStudent(student2, conflict);






                    student1.link(student2);
                    student1.GetComponent<AudioSource>().Play();
                    changeMultiplier(0.3f);
                }

										 
									   

                break;
            case "massive":

                print("And is type massive");

                if (listeningStudents.Count > 3)


                {
                    // Falta conectarlos entre ellos
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student1 = (Student)listeningStudents[randIndex];
                    student1.instigator = true; // EL MALOSO
                    setDisturbingStudent(student1, conflict);
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student2 = (Student)listeningStudents[randIndex];
                    setDisturbingStudent(student2, conflict);
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student3 = (Student)listeningStudents[randIndex];
                    setDisturbingStudent(student3, conflict);
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student4 = (Student)listeningStudents[randIndex];
                    setDisturbingStudent(student4, conflict);



                    student1.link(student2, student3, student4);
                    student1.GetComponent<AudioSource>().Play();
                    changeMultiplier(0.75f);
                }
                
										 
									   

                break;
            default:
                break;
        }
    }


    void highlightStudents()
    {

        foreach (Student s in students)
        {
            if(s!=null)
            {
                s.StartCoroutine("glow");
            }
        }
    }

    void studentDestroyed(Student s)
    {
        //si estaba molestando, deberemos quitarlo de la lista
        listeningStudents.Remove(s);
        disturbingStudents.Remove(s);
										 
												
		 
										
		 
						
        students[s.row, s.column] = null;
        Destroy(s.gameObject);
        // Quitar puntos


    }

    void unHighlightStudents()
    {
        foreach (Student s in students)
        {
            if(s != null)
            {
                s.unGlow();
            }
        }
    }



    //REVISAR. VALE LA PENA TENER ESTOS VOID?
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

    void changeMultiplier(float multiplier)
    {
        if(taskMultiplier - multiplier <= 0)
        {
            taskMultiplier = 0;
        } else
        {
            taskMultiplier -= multiplier;
        }
        if (stressMultiplier + multiplier >= 1)
        {
            stressMultiplier = 1;
        }
        else
        {
            stressMultiplier += multiplier;
        }
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

        readerXML.Load(@"Assets/Resources/levels/" + level + ".xml");
        XmlNodeList nodesDocument = readerXML.DocumentElement.SelectNodes("/Level/Tasks/Task");

        foreach (XmlNode node in nodesDocument)//reading from the XML
        {

            elements[0] = node.SelectSingleNode("Type").InnerText;
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

    public void setStress(float stressLevel)
    {
        stressBars = (int)(stressLevel/10);//It's used by skill class
        stressBar.GetComponent<SpriteRenderer>().sprite = stressBarSprites[(int)stressLevel];
    }

	    private void setDisturbingStudent(Student s, string conflict)
    {
        s.setDisturbing(true);
        s.disturb(conflict);
							  
        disturbingStudents.Add(s);
        listeningStudents.Remove(s);
    }


     IEnumerator gameOver (string deadby)
    {
        switch (deadby){
            case "timeup":
                timeUpSprite.SetActive(true);
                while (!Input.GetMouseButtonDown(0))
                    yield return StartCoroutine(WaitForKeyDown(Input.GetMouseButtonDown(0)));

                restartGame();
               
                break;
            case "explosion":
                killedSprite.SetActive(true);

                while (!Input.GetMouseButtonDown(0)) {
                    
                    yield return StartCoroutine(WaitForKeyDown(Input.GetMouseButtonDown(0)));
                }

                restartGame();
                break;
            default:
                print("No idea why I'm dead");
                break;
        }
    }

    public void restartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator WaitForKeyDown(bool mousePressed)
    {
 
                yield return null;
    
    }
	
	
	//Show time
	    void updateTime()
    {
        int seconds = (int)timeLeft % 60;
        int minutes = (int)timeLeft / 60;
            time = minutes.ToString("00") + ":" + seconds.ToString("00");
        
        timerText.text = time;
        if(seconds == 10 && !ending && minutes==0)
        {
            ending = true;
            levelEnding();
        }
    }

    void levelEnding()
    {
        StartCoroutine("endingTimer");
        
        GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("music/How To Robot - 10 Seconds");
        GetComponent<AudioSource>().Play();
    }

    public IEnumerator endingTimer()
    {
        while (true)
        {
            int i = 14;
            int adding = 0;
            for (float f = 1f; f >= 0; f -= 0.017f)
            {
                timerText.color =  new Color(f,0,0);
                adding = (int)(f * 10);
                print(adding);
                timerText.fontSize = i + adding;
                yield return null;
            }
        }
    }

    public void exitGame()
    {
        print("SALIR");
        Application.Quit();
    }

    public void TogglePauseMenu()
    {
        // not the optimal way but for the sake of readability
        if (pauseMenu.GetComponentInChildren<Canvas>().enabled)
        {
            pauseMenu.GetComponentInChildren<Canvas>().enabled = false;
            Time.timeScale = 1.0f;
        }
        else
        {
            pauseMenu.GetComponentInChildren<Canvas>().enabled = true;
            Time.timeScale = 0f;
        }

        Debug.Log("GAMEMANAGER:: TimeScale: " + Time.timeScale);
    }

    public void setMusic()
    {

        if (musicToggle.isOn)
        {

            this.GetComponent<AudioSource>().mute = false;
        }
        else
        {

            this.GetComponent<AudioSource>().mute = true;
        }
    }

}
