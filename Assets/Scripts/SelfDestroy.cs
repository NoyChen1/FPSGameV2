using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float timeForDistruction;

    private void Start()
    {
        StartCoroutine(DestroySelf(timeForDistruction));
    }


    private IEnumerator DestroySelf(float timeForDistruction)
    {
        yield return new WaitForSeconds(timeForDistruction);
        Destroy(gameObject);
    }
}
