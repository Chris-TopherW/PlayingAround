using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TestDll : MonoBehaviour {

    [DllImport("TestDLL", EntryPoint = "TestSort")]
    public static extern void TestSort(int[] a, int length);

    public int[] a;


	void Start () {
        double timee = Time.realtimeSinceStartup;
        TestSort(a, a.Length);
        Debug.Log("Func took " + (Time.realtimeSinceStartup - timee) + " seconds to run");
	}
	
	void Update () {
		
	}
}
