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
    //[DllImport("VSTHostUnity", EntryPoint = "processAudio", CallingConvention = CallingConvention.Cdecl)]
    //public static extern void processAudio(/*AEffect *plugin, */float[][] inputs, float[][] outputs,
    //    long numFrames);
    [DllImport("VSTHostUnity", EntryPoint = "processAudio", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr processAudio(IntPtr input, long numFrames);
    [DllImport("VSTHostUnity", EntryPoint = "setNumChannels", CallingConvention = CallingConvention.Cdecl)]
    public static extern void setNumChannels(int p_numChannels);
    [DllImport("VSTHostUnity", EntryPoint = "setBlockSize", CallingConvention = CallingConvention.Cdecl)]
    public static extern void setBlockSize(int p_blocksize);
    [DllImport("VSTHostUnity", EntryPoint = "initializeIO", CallingConvention = CallingConvention.Cdecl)]
    public static extern void initializeIO();
    [DllImport("VSTHostUnity", EntryPoint = "configurePluginCallbacks", CallingConvention = CallingConvention.Cdecl)]
    public static extern int configurePluginCallbacks(/*AEffect *plugin*/);
    [DllImport("VSTHostUnity", EntryPoint = "startPlugin", CallingConvention = CallingConvention.Cdecl)]
    public static extern void startPlugin(/*AEffect *plugin*/);
    [DllImport("VSTHostUnity", EntryPoint = "cDebug", CallingConvention = CallingConvention.Cdecl)]
    public static extern String cDebug();


    private float[][] inputArray;
    float[] tempOutArr;
    //public int[] a;
    //private int iterator = 0;
    private float[][] outputArray;
    public float[] squareWave;
    private int blockSize = 1024;


    public double frequency = 100;
    public double gain = 0.5;
    private double increment;
    private double phase;
    private double sampling_frequency = 48000;

    void Start()
    {
        //TestSort(a, a.Length);
        
        setNumChannels(2);
        setBlockSize(blockSize);
        loadPlugin(Application.dataPath + "/Assets/Data/JuceDemoPlugin.dll");
        //configurePluginCallbacks();
        
        initializeIO();

        tempOutArr = new float[1024];

        inputArray = new float[2][];
        inputArray[0] = new float[blockSize];
        inputArray[1] = new float[blockSize];
        outputArray = new float[2][];
        outputArray[0] = new float[blockSize];
        outputArray[1] = new float[blockSize];
        
    }


    private void Update()
    {
        //String debug = cDebug();
        //if (debug != "no message")
        //{
            //Debug.Log("c debug = " + debug);
        //}
    }


    void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2 * Math.PI / sampling_frequency;
        int j = 0;
        for (var i = 0; i < data.Length; i += channels)
        {
            phase = phase + increment;
            inputArray[0][j] = (float)(gain * Math.Sin(phase));
            //data[i] = inputArray[0][j];
            if (channels == 2)
            {
                inputArray[1][j] = inputArray[0][j];
                //data[i + 1] = inputArray[1][j];
            }
            if (phase > 2 * Math.PI) phase = 0;
            j++;
        }

        int size = Marshal.SizeOf(tempOutArr[0]) * tempOutArr.Length;
        IntPtr tempPtr = Marshal.AllocHGlobal(size);
        Marshal.Copy(inputArray[0], 0, tempPtr, 1024);
        IntPtr buf = processAudio(tempPtr, 1024);
        Marshal.Copy(buf, tempOutArr, 0, 1024);

        j = 0;
        for (int i = 0; i < data.Length; i += channels)
        {
            data[i] = tempOutArr[j];
            if (channels == 2)
            {
                data[i+1] = data[i];
            }
            j++;
        }
    }

    float[] procAudio(float[] input, long p_numFrames)
    {
        for(int i = 0; i < p_numFrames; i++)
        {
            tempOutArr[i] = input[i] * 0.5f;
        }
        return tempOutArr;
    }
}
