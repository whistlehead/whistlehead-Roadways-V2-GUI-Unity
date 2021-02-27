using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderObjectRemover : MonoBehaviour
{
    public GameObject g1;
    public GameObject g2;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(g1);
        Destroy(g2);
    }
}
