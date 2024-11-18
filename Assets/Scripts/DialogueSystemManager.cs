using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using Mono.Cecil.Cil;
using TMPro;
using Unity.VisualScripting;


// using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
public class DialogueSystemManager : MonoBehaviour
{
    private GameController gamecontroller;
    public GameObject chatBox;
    public bool isOption;
    public GameObject sceneCount;
    private TMP_Text chatboxText;
    private TMP_Text sceneCountText;
    public GameObject DialogueSystem;
    public int currentPlot = 0;
    public int PlotVolume;
    public GameObject NextButton;
    public GameObject OptionBoxes;
    public GameObject OptionA;
    public GameObject OptionB;
    public GameObject TBCScreen;
    public GameObject IntersceneScreen;
    public GameObject Test;
    public GameObject TimeShiftScreen;
    public GameObject TimeShiftT;
    public int timeshiftLink;
    public GameObject CloudBubbleT;
    private TMP_Text OptionAText;
    private TMP_Text OptionBText;
    private int OptionALink;
    private int OptionBLink;
    private string OptionADeal = "";
    private string OptionBDeal = "";
    private int nextLink = -1;
    private TMP_Text TestText;
    private TMP_Text TimeShiftText;
    private TMP_Text CloudBubbleText;
    private List<int> hiddenPlots = new List<int>();
    private List<string> visitedScenes = new List<string>();
    // Start is called before the first frame update
    void Awake()
    {
        chatboxText = chatBox.GetComponent<TMP_Text>();
        sceneCountText = sceneCount.GetComponent<TMP_Text>();
        gamecontroller = GameObject.Find("Game Controller").GetComponent<GameController>();
        DialogueSystem.gameObject.SetActive(false);
        OptionAText = OptionA.GetComponent<TMP_Text>();
        OptionBText = OptionB.GetComponent<TMP_Text>();
        TestText = Test.GetComponent<TMP_Text>();
        TimeShiftText = TimeShiftT.GetComponent<TMP_Text>();
        CloudBubbleText = CloudBubbleT.GetComponent<TMP_Text>();
        WriteDiagesis();
    }

    // Update is called once per frame
    void Update()
    {
        Testing();
        if(gamecontroller.isSearchingScene)
        {
            IntersceneScreen.SetActive(true);
        }else{
            IntersceneScreen.SetActive(false);
        } 
        if(gamecontroller.isDialogue)
        {
            DialogueUpdate();
            DialogueSystem.SetActive(true);
        }else{
            DialogueSystem.SetActive(false);
        } 
        if(gamecontroller.isGamestart&&gamecontroller.isDialogue)
        {
            var scene = gamecontroller.currentScene;
            var sceneNameSplit = scene.Split("_");
            sceneCountText.text = ""+sceneNameSplit[1];
            ControlIntersceneScreen();
        }     
    }
    void DialogueUpdate()
    {
        
    }
    string[] ReadPlot(int plotNumber)
    {
        var sceneName = gamecontroller.currentScene;
        string filePath = Application.persistentDataPath + "/" + sceneName +  ".json";
        string sceneDataString = System.IO.File.ReadAllText(filePath);
        SceneData data = JsonUtility.FromJson<SceneData>(sceneDataString);
        // Debug.Log(sceneName + " data is load.");
        string[] plotline = new string[]{};
        PlotVolume = data.Plots.Length;
        if(PlotVolume>plotNumber)
        {
            plotline = data.Plots[plotNumber].Split("/");
        }
        return plotline;
    }

    // NextPlot function: read and decode the json file.
    private void NextPlot()
    {
        string[] plotline = ReadPlot(currentPlot);
        if(plotline.Length == 0){
                //scene ended
                EndScene();
                // IntersceneScreen.SetActive(true);
        }else if((plotline[0] == "linear")&&(plotline.Length == 2))
        {
            chatboxText.text = plotline[1];
            isOption = false;
        }else if((plotline[0] == "linear")&&(plotline.Length == 3))
        {
            chatboxText.text = plotline[1];
            nextLink = Convert.ToInt32(plotline[2]);
            isOption = false;
        }else if((plotline[0] == "optional")&&(plotline.Length == 6))
        {
            chatboxText.text = plotline[1];
            OptionAText.text = plotline[2];
            OptionALink = Convert.ToInt32(plotline[3]);
            OptionBText.text = plotline[4];
            OptionBLink = Convert.ToInt32(plotline[5]);
            hiddenPlots.Add(OptionALink);
            hiddenPlots.Add(OptionBLink);
            isOption = true;
        }else if((plotline[0] == "optional")&&(plotline.Length == 7))
        {
            chatboxText.text = plotline[1];
            OptionAText.text = plotline[2];
            OptionBDeal = plotline[3];
            OptionBText.text = plotline[4];
            OptionADeal = plotline[5];
            OptionALink = Convert.ToInt32(plotline[6]);
            OptionBLink = Convert.ToInt32(plotline[6]);
            hiddenPlots.Add(OptionALink);
            hiddenPlots.Add(OptionBLink);
            isOption = true;
        }else if((plotline[0] == "instruction")&&(plotline.Length == 4))
        {
            chatboxText.text = plotline[1];
            isOption = false;
            gamecontroller.nextScene = plotline[2];
            CloudBubbleText .text = plotline[3];

        }else if((plotline[0] == "timeshift")&&(plotline.Length==3))
        {
            TimeShiftText.text = plotline[1];
            StartCoroutine(TimeShiftScreenDisplayCo());
            timeshiftLink = Convert.ToInt32(plotline[2]);
        }else if((plotline[0] == "toinstruction")&&(plotline.Length==2))
        {
            EndScene();
            StartScene(plotline[1]);
        }else if((plotline[0] == "miracle")&&(plotline.Length==3))
        {
            chatboxText.text = plotline[1];
            isOption = false;
            gamecontroller.nextScene = plotline[2];
            CloudBubbleText .text = plotline[3];
        }
        if(isOption)
        {
            OptionBoxes.SetActive(true);
            NextButton.SetActive(false);
        }else{
            OptionBoxes.SetActive(false);
            NextButton.SetActive(true);
        }    
    }
    void ControlValue(string code)
    {
        string[] lines = code.Split("_");
        if(lines.Length==3){
            if(lines[0] == "clue"){
                if(lines[1]=="+")
                {
                    gamecontroller.clue += Convert.ToInt32(lines[2]);
                }else{
                    gamecontroller.clue -= Convert.ToInt32(lines[2]);
                }
            }

        }
    }

    public void ConsecutivePlot(){
        if(gamecontroller.isGamestart && gamecontroller.isDialogue && currentPlot<PlotVolume)
        {   if(nextLink!=-1 && nextLink < PlotVolume)
            {
                currentPlot = nextLink;
                nextLink = -1;
            }else{
                while(true)
                {
                    currentPlot +=1;
                    bool isHidden = false;
                    foreach(var plot in hiddenPlots)
                    {
                        if(currentPlot==plot)
                        {
                            isHidden = true;
                            break;
                        }
                    }
                    if(!isHidden)
                    {break;}
                }
            }
        }
        NextPlot();
    }

    void ControlIntersceneScreen()
    {
        if(gamecontroller.isSearchingScene)
        {
            IntersceneScreen.SetActive(true);
        }else{
            IntersceneScreen.SetActive(false);
        }
        NextPlot();
    }

    public void ChooseA()
    {
        if(gamecontroller.isGamestart && gamecontroller.isDialogue)
        {
            if(OptionALink<PlotVolume)
            {
                currentPlot = OptionALink;
            }
            if(OptionADeal!="")
            {
                ControlValue(OptionADeal);
                OptionADeal = "";
            }
        }
        NextPlot();
    }
    public void ChooseB()
    {
        if(gamecontroller.isGamestart && gamecontroller.isDialogue)
        {
            if(OptionBLink<PlotVolume)
            {
                currentPlot = OptionBLink;
            }
            if(OptionBDeal!="")
            {
                ControlValue(OptionBDeal);
                OptionBDeal = "";
            }
        }
        NextPlot();
    }
    IEnumerator TimeShiftScreenDisplayCo()
    {
        TimeShiftScreen.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        TimeShiftScreen.SetActive(false);
        currentPlot = timeshiftLink;
        NextPlot();
        
        // ConsecutivePlot();
    }
    void WriteDiagesis()
    {
        var filePath = Application.persistentDataPath;
        Diagesis diagesis = new Diagesis();
        diagesis.Id = "0000";
        diagesis.Name = "Chang's Story";
        diagesis.scenes = new string[]{
            "Scene1", "Scene2", "Scene3"
        };
        string diagesisString = JsonUtility.ToJson(diagesis);
        System.IO.File.WriteAllText(filePath+ "/Diagesis.json", diagesisString);
        //create a list of scene datas
        List<SceneData> scenes = new List<SceneData>(); 
        SceneData sceneA = new SceneData();
        SceneData scene1 = new SceneData();
        SceneData sceneB = new SceneData();
        SceneData scene2 = new SceneData();
        SceneData sceneC = new SceneData();
        SceneData scene3 = new SceneData();
        SceneData sceneD = new SceneData();
        SceneData scene4 = new SceneData();
        // add all scenes to the List
        scenes.Add(sceneA); 
        scenes.Add(scene1); 
        scenes.Add(sceneB); 
        scenes.Add(scene2);
        scenes.Add(sceneC); 
        scenes.Add(scene3);
        scenes.Add(sceneD); 
        scenes.Add(scene4);
        // initialize scene datas.
        sceneA.Id = "000A"; 
        scene1.Id = "0001"; 
        sceneB.Id = "000B"; 
        scene2.Id = "0002";
        sceneC.Id = "000C"; 
        scene3.Id = "0003";
        sceneD.Id = "000D"; 
        scene4.Id = "0004"; 
        sceneA.Name = "Scene_A";
        scene1.Name = "Scene_1";
        sceneB.Name = "Scene_B";
        scene2.Name = "Scene_2";
        sceneC.Name = "Scene_C";
        scene3.Name = "Scene_3";
        sceneD.Name = "Scene_D";
        scene4.Name = "Scene_4";
        sceneA.Information = "City Life.";
        scene1.Information = "When you came to the Chang's for the first time...";
        sceneB.Information = "Loss...";
        scene2.Information = "The second time to Chang's wasn't a pleasure experience";
        sceneC.Information = "Traged";
        scene3.Information = "The Finale";
        sceneA.Plots = new string[]{
            "linear/This is your day-to-day life, hustle and bustle.",
            "instruction/What is this?/Scene_1/Walk around and check what's going wrong..."
        };
        scene1.Plots = new string[]{
            //0
            "optional/But where is it here? let me just look around/look left/1/look right/2",  
            //1
            "linear/On my left, it's boudless tea planation.../3",    
            //2
            "linear/On my right, it's blood-red soil...",   
            //3
            "linear/A figure appears ahead and interupts me.", 
            //4
            "linear/He looks like a country squire.", 
            //5
            "linear/Country squire: \"It's already the end of the year. When will your family repay the money you owe me?\"",
            //6
            "linear/I: \"I'm terribly sorry. We will pay the debt as soon as possible\"",
            //7
            "timeshift/A Few Days Later.../8",
            //8
            "linear/It's the country squire again...",
            //9
            "linear/Country squire: \"No more delay!\"",
            //10
            "linear/I: \"Yes…Please wait for another ten days.\"",
            //11
            "timeshift/A Few Days Later.../12",
            //12
            "linear/Country squire: \"I will retrieve my money in five days. I promise.\"",
            //13
            "optional/I: \"...\"/Hurry into the city to sell the tea/14/It's useless. It's impossible to pay it back/15",
            //14
            "linear/You pay what you owe, but your family is as poor as a church mouse.",
            //15
            "linear/Five days later, a gang of robbers ransack your house./16",
            //16
            "linear/You're too poor to live on.",
            //17
            "toinstruction/Scene_B"
        };
        sceneB.Plots = new string[]{
            "instruction/My head is aching! there is a strange sound/Scene_2/Find the source of the sound..."
        };
         scene2.Plots = new string[]{
            "linear/A figure appears ahead.",
            "linear/The girl is my little sister.",
            "linear/My sister: \"Mom's medicine has run out. We can't afford new ones.\"",
            "linear/I: \"I will go to the school to beg for my salary...\"",
            "toinstruction/Scene_C"
        };
        sceneC.Plots = new string[]{
            "linear/What's happening?",
            "instruction/Mom's ill?/Scene_3/Keep on finding the sound source..."
        };
         scene3.Plots = new string[]{
            
            "linear/My sister: \"Brother, we can't afford to bury our mother…\"",
            "liniear/I: \"...\"",
            "linear/I:\"Ask our younger brother to drop out of school so we can save on tuition.\"",
            "toinstruction/Scene_D"
        };
        sceneD.Plots = new string[]{
            //0
            "linear/What should I do?",
            //1
            "linear/Suddenly, I hear the gossip: \"The Changs' people have arrived…they bought tea and exported it overseas, making a fortune…\"",
            //2
            "optional/Facing this dilemma, I choose:/To avoid risk, I'd better stay in the home/3/It's an opportunity/4",
            //3
            "linear/You are forced to leave your homeland to find a way to survive.",
            //4
            "linear/You leave home and go to Shanxi.",
            //5
            "instruction/Scene_4",
        };
        scene4.Plots = new string[]{
            //0
            "linear/The grand door of the Mansion of Changs is in front of you.",
            //1
            "linear/The servant leads you to meet the master.",
            //2
            "linear/The master: \"Nice to meet you, guest from afar. Please tell me the reason for the kind visit.\"",
            //3
            "optional/I: \"  … \"/Honestly tell him your purpose/4/Keep silent about your real purpose and flatter him/5",
            //4
            "linear/He praises your kindness and intelligence, inviting you to stay in the mansion and promising to help you.",
            //5
            "linear/ Not satisfied with your response, he still invites you to stay for a while.",
            //6
            "timeshift/Days pass by. I don't have a chance to meet him again./7",
            //7
            "linear/I ask the servant to express gratitude and desire to visit him.",
            //8
            "linear/The servant informs you that the master is unwell and cannot meet you.",
            //9
            "timeshift/I feel anxious and unsettled. The livelihood of your family is unresolved./10",
            //10
            "timeshift/You cannot give up./11",
            //11
            "TBC"
        };
        foreach(var scene in scenes)
        {
            string sceneString = JsonUtility.ToJson(scene);
            System.IO.File.WriteAllText(filePath + "/" +scene.Name +".json", sceneString);
        }
        Debug.Log("Scene written. into " + filePath);
    }

     public void InitializeDiagesis()
    {
        // Diagesis diagesis = new Diagesis();
        // diagesis.Id = "0001";
        // diagesis.Name = "Chang's Story";
        // diagesis.scenes = new string[]{"Scene1", "Scene2", "Scene3"};
        string filePath = Application.persistentDataPath + "/Diagesis.json";
        // string diagesisString = JsonUtility.ToJson(diagesis);
        // System.IO.File.WriteAllText(filePath, diagesisString);
        string diagesisString = System.IO.File.ReadAllText(filePath);
        Diagesis diagesis = JsonUtility.FromJson<Diagesis>(diagesisString);
        // foreach (var scene in diagesis.scenes)
        // {
        //     SceneData sceneData = LoadScene(scene);
        //     Debug.Log(sceneData);
        // }
        isOption = false;
        // gamecontroller.isSearchingScene = true;
        // gamecontroller.isDialogue = false;
        // StartScene("Scene1");
    }
    void EndScene()
    {
        gamecontroller.isDialogue = false;
        gamecontroller.isSearchingScene = true;
        gamecontroller.currentScene = "";
        currentPlot = 0;
        gamecontroller.SpwanSplashEffect();
    }
    public void StartScene(string sceneName)
    {
        gamecontroller.currentScene = sceneName;
        gamecontroller.isSearchingScene = false;
        gamecontroller.isDialogue = true;
        hiddenPlots.Clear();
        NextPlot();
        Debug.Log("start: " + sceneName);
        gamecontroller.SpwanSplashEffect();
        visitedScenes.Add(sceneName);
    }
    void Testing()
    {
        var firstline = "Dialogue: " + gamecontroller.isDialogue;
        var secondline = "\nInterScene: " + gamecontroller.isSearchingScene;
        var thirdline = "\nScene: " + gamecontroller.currentScene;
        var forthline = "\nPlot: " + currentPlot + "/" + PlotVolume;
        var fifthline = "\nclue: " + gamecontroller.clue;
        var sixthline = "\nOA Link: " + OptionALink + "OB Link: " + OptionBLink;
        var seventhline = "\nOA Deal: " + OptionADeal + "OB Deal: " + OptionBDeal;
        var eighthline = "\nnextlink: "+ nextLink;
        TestText.text =  firstline+ secondline + thirdline  + forthline + fifthline + sixthline + seventhline + eighthline;
    }
    public bool visited(string sceneName)
    {
        foreach(var scene in visitedScenes)
        {
            if(scene == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}
