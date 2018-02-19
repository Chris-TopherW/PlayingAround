using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class PluginHost : MonoBehaviour
{
    // The imported function
    [DllImport("VSTHostUnity", EntryPoint = "TestSort")]
    public static extern void TestSort(int[] a, int length);
    [DllImport("VSTHostUnity", EntryPoint = "loadPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void loadPlugin(/*string filepath*/);
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
    [DllImport("VSTHostUnity", EntryPoint = "cDebugDelegate", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern String cDebugDelegate();

    [DllImport("VSTHostUnity", EntryPoint = "shutdown", CallingConvention = CallingConvention.Cdecl)]
    public static extern void shutdown();
    [DllImport("VSTHostUnity", EntryPoint = "start", CallingConvention = CallingConvention.Cdecl)]
    public static extern void start();


    private float[][] inputArray;
    //public int[] a;
    private float[][] outputArray;
    public float[] squareWave;
    private int blockSize = 1024;

    public double frequency = 100;
    public double gain = 0.5;
    private double increment;
    private double phase;
    private double sampling_frequency = 48000;

    private int audioPtrSize;
    private IntPtr inputArrayAsVoidPtr;
    private int messagePtrSize;
    private IntPtr messageAsVoidPtr;
    private char[] debugString;

    //[Range(-5.0f, 5.0f)]
    private double sampleAudioOut;

    void Start()
    {
        //TestSort(a, a.Length);

        start();
        setNumChannels(2);
        setBlockSize(blockSize);
        //loadPlugin(Application.dataPath + "/Assets/Data/JuceDemoPlugin.dll");
        initializeIO();
        //sending char array is probably going weeeeird- had code for now
        loadPlugin(/*Application.dataPath + "/Assets/Data/Reverb.dll"*/);
        //configurePluginCallbacks();
        configurePluginCallbacks();
        startPlugin();

        inputArray = new float[2][];
        inputArray[0] = new float[blockSize];
        inputArray[1] = new float[blockSize];
        outputArray = new float[2][];
        outputArray[0] = new float[blockSize];
        outputArray[1] = new float[blockSize];

        audioPtrSize = Marshal.SizeOf(inputArray[0][0]) * inputArray[0].Length;
        inputArrayAsVoidPtr = Marshal.AllocHGlobal(audioPtrSize);
        messagePtrSize = 8 * 256;
        messageAsVoidPtr = Marshal.AllocHGlobal(messagePtrSize);
        debugString = new char[256];
    }

    //need to update  dll end.
    private void Update()
    {
        String debugMes = cDebugDelegate();
        if (debugMes != "no message")
        {
            Debug.Log(debugMes);
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        //generate sine wave test tone
        increment = frequency * 2 * Math.PI / sampling_frequency;
        int j = 0;
        for (var i = 0; i < data.Length; i += channels)
        {
            phase = phase + increment;
            inputArray[0][j] = (float)(gain * Math.Sin(phase));
            if (channels == 2)
            {
                inputArray[1][j] = inputArray[0][j];
            }
            if (phase > 2 * Math.PI) phase = 0;
            j++;
        }

        //send audio to and from C using marshal for unmanaged code
        Marshal.Copy(inputArray[0], 0, inputArrayAsVoidPtr, 1024);
        IntPtr outputVoidPtr = processAudio(inputArrayAsVoidPtr, 1024);
        Marshal.Copy(outputVoidPtr, inputArray[0], 0, 1024);

        //copy buffer to data output
        j = 0;
        for (int i = 0; i < data.Length; i += channels)
        {
            data[i] = inputArray[0][j];
            if (channels == 2)
            {
                data[i+1] = data[i];
            }
            j++;
        }
    }
    private void OnApplicationQuit()
    {
        //in editor dll loads into memory when scene is opened and unloads when unity closes... this is still a hack :(
        if(!Application.isEditor)
            shutdown();
    }
}
