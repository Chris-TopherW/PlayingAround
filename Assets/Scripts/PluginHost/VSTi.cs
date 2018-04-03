using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace pluginHost
{
    public class VSTi : MonoBehaviour
    {
        //public 
        public string pluginPath;
        private int thisVSTIndex = 0;

        [Space]
        //////////////////////  params  //////////////////////
        public int numParams;
        [Range(0.0f, 1.0f)]
        public float[] parameters;
        private float[] previousParams;
        public string[] paramNames;

        //////////////////////  audio io  //////////////////////
        private float[][] audioThroughArray;
        private int numPluginOutputs;

        ////////////////////// interop //////////////////////
        private int messagePtrSize;
        private IntPtr messageAsVoidPtr;
        private bool ready = false;
        private bool pluginFailedToLoad = false;

        void Awake()
        {
            if (ready)
                return;

            pluginPath = "C:\\Users\\chriswratt\\Documents\\UnityProjects\\UnityMidiLib\\VSTHostUnity\\VSTHostUnity\\TAL-NoiseMaker-64.dll";
            thisVSTIndex = loadInstrument(pluginPath);
            if (thisVSTIndex == -1)
            {
                Debug.Log("Error, VST has failed to load. Unsupported file path");
                pluginFailedToLoad = true;
                return;
            }
            setupParams();
            setupIO();

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

            IntPtr outputVoidPtr = HostDllCpp.processInstAudio(thisVSTIndex, pluggoHost.blockSize, channels);
            Marshal.Copy(outputVoidPtr, data, 0, pluggoHost.blockSize * channels);
        }

        void setupIO()
        {
            ////////////////////// alloc space for audio io //////////////////////
            numPluginOutputs = HostDllCpp.getNumPluginOutputs(thisVSTIndex);
            audioThroughArray = new float[numPluginOutputs][];
            for (int i = 0; i < numPluginOutputs; i++)
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

        public int loadInstrument(string path)
        {
            IntPtr intPtr_aux = Marshal.StringToHGlobalAnsi(path);
            int effectIndex = HostDllCpp.loadInstrument(intPtr_aux);
            Marshal.FreeHGlobal(intPtr_aux);
            return effectIndex;
        }

        public void OnApplicationQuit()
        {
            Marshal.FreeHGlobal(messageAsVoidPtr);
            ready = false;
        }
    }
}
