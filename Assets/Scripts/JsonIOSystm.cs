using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
// using UnityEditor.SearchService;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class JsonIOSystm : MonoBehaviour
{
    public void LoadToJson()
    {
        // SceneData data1 = new SceneData();
        // SceneData data2 = new SceneData();
        // data1.Id = "dfahoi";
        // data1.Name = "Scene1";
        // data1.Information = "When you came to the Chang's for the first time...";
        // data1.Plots = new string[]{"Meets", "Greets"};
        // data2.Id = "dfgihasd";
        // data2.Name = "Scene2";
        // data2.Information = "The second time to Chang's wasn't a pleasure experience";
        // data2.Plots = new string[]{"Yells", "Fights"};
        // string filePath = Application.dataPath + "/Resources/JSON/SceneDatas.json";
        // string sceneData1 = JsonUtility.ToJson(data1);
        // string sceneData2 = JsonUtility.ToJson(data2);
        // System.IO.File.WriteAllText(filePath, sceneData1);
        // System.IO.File.WriteAllText(filePath, sceneData2);
        // Debug.Log(filePath);
        // Debug.Log("Data write into SceneDatas.json");
    }

    public SceneData LoadScene(string sceneName)
    {
        string filePath = Application.dataPath + "/Resources/JSON/" + sceneName +  ".json";
        string sceneDataString = System.IO.File.ReadAllText(filePath);
        SceneData data = JsonUtility.FromJson<SceneData>(sceneDataString);
        Debug.Log(sceneName + " data is load.");
        return data;
    }
    public void ReadFromJson()
    {
        // string filePath = Application.dataPath + "/SceneDatas.json";
        // string sceneDatas = System.IO.File.ReadAllText(filePath);
        // SceneData[] scenes = JsonUtility.FromJson<SceneData[]>(sceneDatas);
        // Debug.Log("Scene data loaded.");
    }
}
