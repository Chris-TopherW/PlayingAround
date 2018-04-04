using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace pluginHost
{
    [ExecuteInEditMode]
    public class VSTEffect : MonoBehaviour
    {
        //important stuff 
        public string pluginPath;
        private int thisVSTIndex = 0;
        [Space]

        [Header("Midi")]
        public bool midiInput = false;
        public int midiChannel = 1;

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
            pluginPath = "D:\\UnityProjects\\usingExternalCpp\\VSTHostUnity\\VSTHostUnity\\TAL-Reverb-2.dll";
            //pluginPath = "D:\\UnityProjects\\usingExternalCpp\\VSTHostUnity\\VSTHostUnity\\TAL-NoiseMaker-64.dll";

            thisVSTIndex = loadEffect(pluginPath);
            if(thisVSTIndex == -1)
            {
                Debug.Log("Error, VST has failed to load. Unsupported file path");
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
            }
        }

        void setupParams()
        {
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
            IntPtr p_paramName = HostDllCpp.getParamName(thisVSTIndex, paramIndex);
            return Marshal.PtrToStringAnsi(p_paramName);
        }

        public int loadEffect(string path)
        {
            IntPtr intPtr_aux = Marshal.StringToHGlobalAnsi(path);
            int effectIndex = HostDllCpp.loadEffect(intPtr_aux);
            Marshal.FreeHGlobal(intPtr_aux);
            return effectIndex;
        }

        public void OnApplicationQuit()
        {
            Marshal.FreeHGlobal(inputArrayAsVoidPtr);
            Marshal.FreeHGlobal(messageAsVoidPtr);
            ready = false;
        }
    }
}
