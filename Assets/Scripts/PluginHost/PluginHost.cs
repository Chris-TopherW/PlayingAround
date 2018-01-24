using UnityEngine;
using System.Runtime.InteropServices;

public class PluginHost : MonoBehaviour
{
    // The imported function
    [DllImport("VSTHostUnity", EntryPoint = "TestSort")]
    public static extern void TestSort(int[] a, int length);

    public int[] a;

    void Start()
    {
        TestSort(a, a.Length);
    }
}