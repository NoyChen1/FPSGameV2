using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerBottle : MonoBehaviour
{

    public List<Rigidbody> allParts = new List<Rigidbody>();

    public void Explosion()
    {
        foreach(Rigidbody b in allParts)
        {
            b.isKinematic = false;
        }
    }
}
