using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    private ARTrackedImageManager trackedImages;
    public GameObject[] ArPrefabs;
    public GameObject Arrow;
    public GameObject arCamera;
    public GameObject gc;
    private GameController gameController;
    private DialogueSystemManager dialoguemanager;

    List<GameObject> ARObjects = new List<GameObject>();

    
    void Awake()
    {
        trackedImages = GetComponent<ARTrackedImageManager>();
        gameController = gc.GetComponent<GameController>();
        dialoguemanager = gc.GetComponent<DialogueSystemManager>();
    }

    void OnEnable()
    {
        trackedImages.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImages.trackedImagesChanged -= OnTrackedImagesChanged;
    }


    // Event Handler
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        //Create object based on image tracked
        foreach (var trackedImage in eventArgs.added)
        {
            foreach (var arPrefab in ArPrefabs)
            {
                if(trackedImage.referenceImage.name == arPrefab.name)
                {
                    StartCoroutine(StartCoutine(trackedImage, arPrefab));
                }
            }
        }
        
        //Update tracking position
        foreach (var trackedImage in eventArgs.updated)
        {
            float distance = Vector3.Distance(arCamera.gameObject.transform.position, trackedImage.gameObject.transform.position);
            if(gameController.isSearchingScene && ((trackedImage.trackingState == TrackingState.Tracking)||(trackedImage.trackingState == TrackingState.Limited)))
            {
                if(!dialoguemanager.visited(trackedImage.referenceImage.name))
                {
                    dialoguemanager.StartScene(trackedImage.referenceImage.name);
                }
            }
            foreach (var gameObject in ARObjects)
            {
                if(gameObject.name == trackedImage.referenceImage.name)
                {
                    Debug.Log(gameController.currentScene+": "+gameObject.name);
                    if(gameController.currentScene==gameObject.name)
                    {
                        gameObject.SetActive(true);
                    }else{
                        // gameObject.SetActive(false);
                    }
                }
            }
        }

         foreach (var removedImage in eventArgs.removed)
        {
        // Handle removed event
              foreach (var gameObject in ARObjects)
            {
                if(gameObject.name == removedImage.referenceImage.name)
                {
                    // Destroy(gameObject);
                    ARObjects.Remove(gameObject);
                    Destroy(gameObject);
                }
            }
            
        }
        
    }
     IEnumerator StartCoutine(ARTrackedImage trackedImage, GameObject arPrefab)
    {
        yield return null;
        var newPrefab = Instantiate(arPrefab, trackedImage.transform);
        newPrefab.SetActive(true);
        ARObjects.Add(newPrefab);
        var newArrow = Instantiate(Arrow);
        var arrowTracker = newArrow.GetComponent<ArrowTracker>();
        arrowTracker.trackedImage = trackedImage;

    }
}