using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class GameController : MonoBehaviour
{
    public bool isGamestart;
    public bool isDetectingPlane;
    public bool isDialogue;
    public bool isSearchingScene;
    public bool isInstruction;
    public MiraclePhase miraclePhase = MiraclePhase.End;
    public enum MiraclePhase{
        End,
        Searching,
        Active
    };
    public GameObject[] miraclePrefabs;
    public string[] miracle;
    public GameObject[] existingMiracles;
    public GameObject gameRegular;
    public string currentScene = "";
    public string nextScene = "";
    public GameObject startScreen;
    public ARSessionOrigin arOrigin;
    public GameObject arCamera;
    private ARPlaneManager planeManager;
    private ARRaycastManager raycastManager;
    private ARTrackedImageManager trackedImageManager;
    public GameObject[] LionPrefab;
    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private ImageTracker imageTracker;
    private AudioSource backgroundMusic1;
    public GameObject SplashEffect;
    private float offset;
    public int clue=0;

    // Start is called before the first frame update
    void Start()
    {
        isInstruction = false;
        isGamestart = false;
        isDialogue = false;
        isSearchingScene = false;
        planeManager = arOrigin.GetComponent<ARPlaneManager>();
        raycastManager = arOrigin.GetComponent<ARRaycastManager>();
        trackedImageManager = arOrigin.GetComponent<ARTrackedImageManager>();
        imageTracker = arOrigin.GetComponent<ImageTracker>();
        planeManager.enabled = false;
        raycastManager.enabled = false;
        trackedImageManager.enabled = false;
        imageTracker.enabled = false;
        backgroundMusic1 = arCamera.GetComponent<AudioSource>();
        backgroundMusic1.mute = true;
        gameRegular.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isGamestart && miraclePhase==MiraclePhase.Searching)
        {
            if(isDetectingPlane)
            {
                if(miracle.Length!=0)
                {
                    //disable the plane and the plane manager
                    bool isRayCollision = raycastManager.Raycast(new Vector2(Screen.width/2.0f, Screen.height/2.0f), raycastHits, TrackableType.PlaneWithinPolygon);
                    if(isRayCollision)
                    {
                        foreach(var m in miracle)
                        {
                             GameObject _object = Instantiate(GetMiracle(m));
                            _object.transform.position = raycastHits[0].pose.position;
                            existingMiracles.Append(_object);
                            StartCoroutine(MiraclePrecedureCo(m));
                        }
                       
                    }
                    foreach(var plane in planeManager.trackables)
                    {
                        plane.gameObject.SetActive(false);
                    }
                    planeManager.enabled = false;
                    
                }            
            }else{//detecting plane
                isDetectingPlane = true;
                planeManager.enabled = true;
                planeManager.detectionMode = PlaneDetectionMode.Horizontal;
            }

        }
    }

IEnumerator MiraclePrecedureCo(string m)
{
    yield return new WaitForSeconds(5);
    if(existingMiracles.Length!=0)
    {
        for(int i=0; i < miracle.Length; i++)
        {
            GameObject dm = existingMiracles[i];
            Destroy(dm.gameObject);
        }
    }
    miraclePhase = MiraclePhase.End;
}
    GameObject GetMiracle(string miracleName)
    {   GameObject empty = null;
        foreach(var miracle in miraclePrefabs)
        {
            if (miracle.gameObject.name==miracleName)
            {
                return miracle;
            }
        }
        return empty;
    }
    public void StartGame()
    {
        startScreen.SetActive(false);
        trackedImageManager.enabled = true;
        raycastManager.enabled = true;
        imageTracker.enabled = true;
        isGamestart = true;
        isInstruction = true;
        isDialogue = true;
        currentScene = "Scene_A";
        isSearchingScene = false;
        backgroundMusic1.mute = false;
        gameRegular.SetActive(true);
        SpwanSplashEffect();
    }
    public void SpwanSplashEffect()
    {
        Instantiate(SplashEffect, arCamera.transform.position + Vector3.back*offset, arCamera.transform.rotation);
    }
    public void StartMiracle()
    {

    }
}
