using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashEffectDelete : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(DeleteSelf());
    }

    IEnumerator DeleteSelf()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    // Update is called once per frame
}
