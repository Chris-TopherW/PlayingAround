using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class PluginHost : MonoBehaviour
{
    [DllImport("VSTHostUnity", EntryPoint = "loadPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void loadPlugin(/*string filepath*/);
    [DllImport("VSTHostUnity", EntryPoint = "processAudio", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr processAudio(IntPtr input, long numFrames);
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
    [DllImport("VSTHostUnity", EntryPoint = "getNumParams", CallingConvention = CallingConvention.Cdecl)]
    public static extern int getNumParams();
    [DllImport("VSTHostUnity", EntryPoint = "setParam", CallingConvention = CallingConvention.Cdecl)]
    public static extern void setParam(int paramIndex, float p_value);
    [DllImport("VSTHostUnity", EntryPoint = "getParam", CallingConvention = CallingConvention.Cdecl)]
    public static extern float getParam(int index);
    [DllImport("VSTHostUnity", EntryPoint = "getParamName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr getParamName(int index);
    [DllImport("VSTHostUnity", EntryPoint = "getNumPluginOutputs", CallingConvention = CallingConvention.Cdecl)]
    public static extern int getNumPluginOutputs();
    [DllImport("VSTHostUnity", EntryPoint = "getNumPluginInputs", CallingConvention = CallingConvention.Cdecl)]
    public static extern int getNumPluginInputs();

    public double frequency = 100;
    public double gain = 0.5;
    public int numParams;
    [Range(0.0f, 1.0f)]
    public float[] parameters;
    private float[] previousParams;
    public string[] paramNames;

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

    private void Awake()
    {
        Debug.Log("awake called");
        setBlockSize(blockSize);
        loadPlugin();
        configurePluginCallbacks();
        initializeIO();
        startPlugin();

        numParams = getNumParams();
        ready = true;
        parameters = new float[numParams];
        previousParams = new float[numParams];
        paramNames = new string[numParams];

        for (int i = 0; i < numParams; i++)
        {
            parameters[i] = getParam(i);
            previousParams[i] = parameters[i];
            paramNames[i] = getParameterName(i);
        }
        for(int i = 0; i < numParams; i++)
        {
            //Debug.Log("Parameter name at pos " + i + " is: " + getParameterName(i));
        }
    }

    void Start()
    {
        inputArray = new float[getNumPluginInputs()][];
        inputArray[0] = new float[blockSize];
        inputArray[1] = new float[blockSize];
        outputArray = new float[getNumPluginOutputs()][];
        outputArray[0] = new float[blockSize];
        outputArray[1] = new float[blockSize];

        if(getNumPluginInputs() != getNumPluginOutputs())
        {
            Debug.Log("Error, plugin inputs does not equal plugin outputs");
        }

        audioPtrSize = Marshal.SizeOf(inputArray[0][0]) * inputArray[0].Length;
        inputArrayAsVoidPtr = Marshal.AllocHGlobal(audioPtrSize);
        messagePtrSize = 8 * 256;
        messageAsVoidPtr = Marshal.AllocHGlobal(messagePtrSize);
        debugString = new char[256];
    }

    private void Update()
    {
        for (int i = 0; i < numParams; i++)
        {
            if (previousParams[i] != parameters[i])
            {
                setParam(i, parameters[i]);
                previousParams[i] = parameters[i];
            }
        }
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
        Marshal.Copy(inputArray[0], 0, inputArrayAsVoidPtr, blockSize);
        IntPtr outputVoidPtr = processAudio(inputArrayAsVoidPtr, blockSize);
        Marshal.Copy(outputVoidPtr, outputArray[0], 0, blockSize);

        //copy buffer to data output
        j = 0;
        for (int i = 0; i < data.Length; i += channels)
        {
            data[i] = outputArray[0][j];
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

    public string getParameterName(int index)
    {
        IntPtr p_paramName = getParamName(index);
        return Marshal.PtrToStringAnsi(p_paramName);
    }

    //private void updateParameters()
    //{
    //    for (int i = 0; i < numParams; i++)
    //    {
    //        setParam(i, parameters[i]);
    //    }
    //}

}







//[UnityEditor.CustomEditor(typeof(PluginHost))]
//public class InspectorCustomizer : UnityEditor.Editor
//{
//    public void ShowArrayProperty(UnityEditor.SerializedProperty list)
//    {
//        UnityEditor.EditorGUI.indentLevel += 1;
//        for (int i = 0; i < list.arraySize; i++)
//        {
//            UnityEditor.EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new UnityEngine.GUIContent("Bla" + (i + 1).ToString()));
//        }
//        UnityEditor.EditorGUI.indentLevel -= 1;
//    }

//    public override void OnInspectorGUI()
//    {
//        ShowArrayProperty(serializedObject.FindProperty("NameOfListToView"));
//    }
//}

//[System.Serializable]
//public class PluginParam
//{
//    public string name;//your name variable to edit
//    [Range(0.0f, 1.0f)]
//    public float param;//place texture in here
//}

//[System.Serializable]
//public class DragonsClass
//{
//    public string Name;
//    [HideInInspector]
//    public bool bUsed;
//}

//public class NamedArrayAttribute : PropertyAttribute
//{
//    public readonly string[] names;
//    public NamedArrayAttribute(string[] names) { this.names = names; }
//}

//[CustomPropertyDrawer(typeof(NamedArrayAttribute))]
//public class NamedArrayDrawer : PropertyDrawer
//{
//    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
//    {
//        int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
//        EditorGUI.ObjectField(rect, property, new GUIContent(((NamedArrayAttribute)attribute).names[pos]));
//    }
//}