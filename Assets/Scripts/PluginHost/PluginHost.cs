using UnityEngine;
using System.Runtime.InteropServices;

public class PluginHost : MonoBehaviour
{
    // The imported function
    [DllImport("VSTHostUnity", EntryPoint = "TestSort")]
    public static extern void TestSort(int[] a, int length);
    [DllImport("VSTHostUnity", EntryPoint = "loadPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void loadPlugin(string filepath);

    public int[] a;

    void Start()
    {
        TestSort(a, a.Length);
        loadPlugin(Application.dataPath + "/Assets/Data/JuceDemoPlugin.dll");
        //loadPlugin(0);
    }
}
