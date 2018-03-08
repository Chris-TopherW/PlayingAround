using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSTEffect : MonoBehaviour {

    //////////////////////  params  //////////////////////
    public int numParams;
    [Range(0.0f, 1.0f)]
    public float[] parameters;
    private float[] previousParams;
    public string[] paramNames;

    void Start () {
		
	}
	
	void Update () {
		
	}
}
