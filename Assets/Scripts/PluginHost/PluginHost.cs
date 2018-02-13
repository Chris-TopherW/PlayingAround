using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class PluginHost : MonoBehaviour
{
    // The imported function
    [DllImport("VSTHostUnity", EntryPoint = "TestSort")]
    public static extern void TestSort(int[] a, int length);
    [DllImport("VSTHostUnity", EntryPoint = "loadPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void loadPlugin(string filepath);
    [DllImport("VSTHostUnity", EntryPoint = "loadPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void processAudio(/*AEffect *plugin, */float[][] inputs, float[][] outputs,
        long numFrames);

    private int bufferSize;
    //private float[] squareWave;
    //private float[][] inputArray;
    //public int[] a;
    //private int iterator = 0;
    //private float[][] outputArray;


    public double frequency = 30;
    public double gain = 0.05;
    private double increment;
    private double phase;
    private double sampling_frequency = 48000;

    void Start()
    {
        //TestSort(a, a.Length);
        loadPlugin(Application.dataPath + "/Assets/Data/JuceDemoPlugin.dll");
        
        int __placeholder;
        AudioSettings.GetDSPBufferSize(out bufferSize, out __placeholder);
        //squareWave = new float[bufferSize];
        //inputArray = new float[1][];
        //inputArray[0] = squareWave;
        //outputArray = new float[1][];
        //outputArray[0] = new float[bufferSize];
    }

    //processAudio(inputArray, outputArray, bufferSize);
    //data = outputArray[0];


    void OnAudioFilterRead(float[] data, int channels)
    {
        // update increment in case frequency has changed
        increment = frequency * 2 * Math.PI / sampling_frequency;
        for (var i = 0; i < data.Length; i = i + channels)
        {
            phase = phase + increment;
            // this is where we copy audio data to make them “available” to Unity
            data[i] = (float)(gain * Math.Sin(phase));
            // if we have stereo, we copy the mono data to each channel
            if (channels == 2) data[i + 1] = data[i];
            if (phase > 2 * Math.PI) phase = 0;
        }
    }
}
