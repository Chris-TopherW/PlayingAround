using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class PluginHost : MonoBehaviour
{
    [DllImport("VSTHostUnity", EntryPoint = "loadPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void loadPlugin(/*string filepath*/);
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
    [DllImport("VSTHostUnity", EntryPoint = "shutdown", CallingConvention = CallingConvention.Cdecl)]
    public static extern void shutdown();
    [DllImport("VSTHostUnity", EntryPoint = "start", CallingConvention = CallingConvention.Cdecl)]
    public static extern void start();
    [DllImport("VSTHostUnity", EntryPoint = "getNumParams", CallingConvention = CallingConvention.Cdecl)]
    public static extern int getNumParams();
    [DllImport("VSTHostUnity", EntryPoint = "setParam", CallingConvention = CallingConvention.Cdecl)]
    public static extern void setParam(int paramIndex, float p_value);
    [DllImport("VSTHostUnity", EntryPoint = "processBlock", CallingConvention = CallingConvention.Cdecl)]
    public static extern void processBlock();

    public double frequency = 100;
    public double gain = 0.5;
    public int numParams;
    public float[] parameters;

    private float[][] inputArray;
    private float[][] outputArray;
    private int blockSize = 1024;
    private double increment = 0.0;
    private double phase;
    private double sampling_frequency = 48000;

    private int audioPtrSize;
    private IntPtr inputArrayAsVoidPtr;
    private int messagePtrSize;
    private IntPtr messageAsVoidPtr;
    private char[] debugString;
    private bool ready = false;

    [Range(-1.1f, 1.1f)]
    private double sampleAudioOut;

    private void Awake()
    {
        setNumChannels(2);
        setBlockSize(blockSize);
        initializeIO();
        loadPlugin();
        configurePluginCallbacks();
        startPlugin();
        
        numParams = getNumParams();
        //updateParameters();
        ready = true;
    }

    void Start()
    {
        //processBlock(); //this is a debug- will crash if C's processReplacing stuffs up
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
        
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if(!ready)
            return;

        int j = 0;

        //sine wave
        //increment = frequency * 2 * Math.PI / sampling_frequency;
        //for (var i = 0; i < data.Length; i += channels)
        //{
        //    phase = phase + increment;
        //    inputArray[0][j] = (float)(gain * Math.Sin(phase));
        //    if (channels == 2)
        //    {
        //        inputArray[1][j] = inputArray[0][j];
        //    }
        //    if (phase > 2 * Math.PI) phase = 0;
        //    j++;
        //}

        //clicks every second
        for (int i = 2; i < data.Length; i += channels)
        { 
            inputArray[0][j] = 0.0f;
            inputArray[0][j + 1] = 0.0f;
            j++;
            increment++;
        }
        if (increment > 44100)
        {
            inputArray[0][0] = 1.0f;
            inputArray[0][1] = 1.0f;
            increment -= 44100;
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

    //private void updateParameters()
    //{
    //    for (int i = 0; i < numParams; i++)
    //    {
    //        setParam(i, parameters[i]);
    //    }
    //}
}
