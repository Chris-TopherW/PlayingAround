//////////////////////// Unity plugin host for vst 2. Chris Wratt 2018 //////////////////////

//using UnityEngine;
//using System.Runtime.InteropServices;
//using System;

////#if UNITY_EDITOR
////[ExecuteInEditMode]
////#endif
//public class PluginHost : MonoBehaviour
//{
//    public string pluginPath;

//    //////////////////////  params  //////////////////////
//    public int numParams;
//    [Range(0.0f, 1.0f)]
//    public float[] parameters;
//    private float[] previousParams;
//    public string[] paramNames;

//    //////////////////////  audio io  //////////////////////
//    private float[][] inputArray;
//    private float[][] outputArray;
//    private int numPluginInputs;
//    private int numPluginOutputs;
//    private int blockSize = 0;
//    private long sampleRate = 0;

//    ////////////////////// interop //////////////////////
//    private int audioPtrSize;
//    private IntPtr inputArrayAsVoidPtr;
//    private int messagePtrSize;
//    private IntPtr messageAsVoidPtr;
//    private bool ready = false;

//    private void Awake()
//    {
//        ////////////////////// setup io //////////////////////
//        int _numBuff;
//        AudioSettings.GetDSPBufferSize(out blockSize, out _numBuff);
//        Debug.Log("block size = " + blockSize);
//        sampleRate = AudioSettings.outputSampleRate;

//        ////////////////////// interface with plugin and alloc memory //////////////////////
//        loadPlugin();
//        setupParams();
//        setupIO();

//        audioPtrSize = Marshal.SizeOf(inputArray[0][0]) * inputArray[0].Length;
//        inputArrayAsVoidPtr = Marshal.AllocHGlobal(audioPtrSize);
//        messagePtrSize = 8 * 256;
//        messageAsVoidPtr = Marshal.AllocHGlobal(messagePtrSize);

//        //make sure that audio thread does not try to process audio before initialisation complete
//        ready = true;
//    }

//    private void Update()
//    {
//        for (int i = 0; i < numParams; i++)
//        {
//            if (previousParams[i] != parameters[i])
//            {
//                HostDll.setParam(i, parameters[i]);
//                previousParams[i] = parameters[i];
//            }
//        }
//    }

//    void OnAudioFilterRead(float[] data, int channels)
//    {

//        ////send audio to and from C using marshal for unmanaged code
//        //send audio to c in interlaced form with numChan attached?
//        Marshal.Copy(data, 0, inputArrayAsVoidPtr, blockSize * channels);
//        IntPtr outputVoidPtr = HostDll.processAudio(inputArrayAsVoidPtr, blockSize, channels);
//        Marshal.Copy(outputVoidPtr, data, 0, blockSize * channels);
//    }

//    private void OnApplicationQuit()
//    {
//        if(!Application.isEditor)
//            HostDll.shutdown();
//    }

//    void loadPlugin()
//    {
//        HostDll.setBlockSize(blockSize);
//        HostDll.loadPlugin();
//        HostDll.configurePluginCallbacks();
//        HostDll.initializeIO();
//        HostDll.startPlugin();
//    }

//    void setupParams()
//    {
//        numParams = HostDll.getNumParams();
//        parameters = new float[numParams];
//        previousParams = new float[numParams];
//        paramNames = new string[numParams];
//        for (int i = 0; i < numParams; i++)
//        {
//            parameters[i] = HostDll.getParam(i);
//            previousParams[i] = parameters[i];
//            paramNames[i] = getParameterName(i);
//        }
//    }

//    public string getParameterName(int index)
//    {
//        IntPtr p_paramName = HostDll.getParamName(index);
//        return Marshal.PtrToStringAnsi(p_paramName);
//    }

//    void setupIO()
//    {
//        ////////////////////// declare memory for audio io //////////////////////
//        numPluginInputs = HostDll.getNumPluginInputs();
//        numPluginOutputs = HostDll.getNumPluginOutputs();
//        inputArray = new float[numPluginInputs][];
//        for (int i = 0; i < numPluginInputs; i++)
//        {
//            inputArray[i] = new float[blockSize];
//        }
//        outputArray = new float[numPluginOutputs][];
//        for (int i = 0; i < numPluginOutputs; i++)
//        {
//            outputArray[i] = new float[blockSize];
//        }

//        if (HostDll.getNumPluginInputs() != HostDll.getNumPluginOutputs())
//        {
//            Debug.Log("Warning, plugin inputs does not equal plugin outputs");
//        }
//    }
//}
