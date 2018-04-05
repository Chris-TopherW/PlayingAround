using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace pluginHost
{
    [ExecuteInEditMode]
    public class VSTEffect : MonoBehaviour
    {
        //important stuff 
        // public string pluginPath = "D:\\UnityProjects\\usingExternalCpp\\VSTHostUnity\\VSTHostUnity\\TAL-Reverb-2.dll";
        public string pluginPath = "C:\\Users\\chriswratt\\Documents\\UnityProjects\\UnityMidiLib\\VSTHostUnity\\VSTHostUnity\\TAL-Reverb-2.dll";
        private int thisVSTIndex = 0;
        public bool MonoOutput = false;
        //[Space]

        //[Header("Midi")]
        //public bool midiInput = false;
        //public int midiChannel = 1;

        [Space]

        //////////////////////  params  //////////////////////
        [Header("Parameters")]
        public int numParams;
        [Range(0.0f, 1.0f)]
        public float[] parameters;
        private float[] previousParams;
        public string[] paramNames;

        //////////////////////  audio io  //////////////////////
        private float[][] audioThroughArray;
        private int numPluginInputs;
        private int numPluginOutputs;

        ////////////////////// interop //////////////////////
        private int audioPtrSize;
        private IntPtr inputArrayAsVoidPtr;
        private int messagePtrSize;
        private IntPtr messageAsVoidPtr;
        private bool ready = false;
        private bool pluginFailedToLoad = false;

        void Awake()
        {
            if (ready)
                return;

            //pluginPath = "C:\\Users\\chriswratt\\Documents\\UnityProjects\\UnityMidiLib\\VSTHostUnity\\VSTHostUnity\\TAL-Reverb-2.dll";
            //pluginPath = "D:\\UnityProjects\\usingExternalCpp\\VSTHostUnity\\VSTHostUnity\\TAL-NoiseMaker-64.dll";

            thisVSTIndex = loadEffect(pluginPath);
            if(thisVSTIndex == -1)
            {
                Debug.Log("Error, VST has failed to load. Unsupported file path or format");
                pluginFailedToLoad = true;
                return;
            }
            setupParams();
            setupIO();

            audioPtrSize = Marshal.SizeOf(audioThroughArray[0][0]) * audioThroughArray[0].Length * audioThroughArray.Length;
            inputArrayAsVoidPtr = Marshal.AllocHGlobal(audioPtrSize);
            messagePtrSize = 8 * 256;
            messageAsVoidPtr = Marshal.AllocHGlobal(messagePtrSize);

            ready = true;
        }

        void Update()
        {
            if (pluginFailedToLoad) return;
                
            for (int i = 0; i < numParams; i++)
            {
                if (previousParams[i] != parameters[i])
                {
                    HostDllCpp.setParam(thisVSTIndex, i, parameters[i]);
                    previousParams[i] = parameters[i];
                }
            }
        }
        
        void OnAudioFilterRead(float[] data, int channels)
        {
            if (!ready) return;
            if (pluginFailedToLoad) return;

            Marshal.Copy(data, 0, inputArrayAsVoidPtr, pluggoHost.blockSize * channels);
            IntPtr outputVoidPtr = HostDllCpp.processFxAudio(thisVSTIndex, inputArrayAsVoidPtr, pluggoHost.blockSize, channels);
            Marshal.Copy(outputVoidPtr, data, 0, pluggoHost.blockSize * channels);
            if(MonoOutput && channels == 2)
            {
                for(int i = 0; i < data.Length; i+= 2)
                {
                    data[i + 1] = data[i];
                }
            }
        }

        void setupIO()
        {
            ////////////////////// alloc space for audio io //////////////////////
            numPluginInputs = HostDllCpp.getNumPluginInputs(thisVSTIndex);
            numPluginOutputs = HostDllCpp.getNumPluginOutputs(thisVSTIndex);
            audioThroughArray = new float[numPluginInputs][];
            for (int i = 0; i < numPluginInputs; i++)
            {
                audioThroughArray[i] = new float[pluggoHost.blockSize];
            }

            if (HostDllCpp.getNumPluginInputs(thisVSTIndex) != HostDllCpp.getNumPluginOutputs(thisVSTIndex))
            {
                Debug.Log("Warning, plugin inputs does not equal plugin outputs");
                Debug.Log("Num plugin ins = " + HostDllCpp.getNumPluginInputs(thisVSTIndex));
                Debug.Log("Num plugin outs = " + HostDllCpp.getNumPluginOutputs(thisVSTIndex));
            }
        }

        void setupParams()
        {
            if (pluginFailedToLoad) return;

            numParams = HostDllCpp.getNumParams(thisVSTIndex);
            parameters = new float[numParams];
            previousParams = new float[numParams];
            paramNames = new string[numParams];
            for (int i = 0; i < numParams; i++)
            {
                parameters[i] = HostDllCpp.getParam(thisVSTIndex, i);
                previousParams[i] = parameters[i];
                paramNames[i] = getParameterName(i);
            }
        }

        public string getParameterName(int paramIndex)
        {
            if (pluginFailedToLoad) return "";

            IntPtr p_paramName = HostDllCpp.getParamName(thisVSTIndex, paramIndex);
            return Marshal.PtrToStringAnsi(p_paramName);
        }

        public int loadEffect(string path)
        {
            if (pluginFailedToLoad) return 0;

            IntPtr intPtr_aux = Marshal.StringToHGlobalAnsi(path);
            int effectIndex = HostDllCpp.loadEffect(intPtr_aux);
            Marshal.FreeHGlobal(intPtr_aux);
            return effectIndex;
        }

        public void OnApplicationQuit()
        {
            if (pluginFailedToLoad) return;

            Marshal.FreeHGlobal(inputArrayAsVoidPtr);
            Marshal.FreeHGlobal(messageAsVoidPtr);
            ready = false;
        }
    }
}
