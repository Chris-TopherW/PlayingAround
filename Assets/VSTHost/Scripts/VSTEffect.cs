//Copyright 2018 Chris Wratt and Victoria University of Wellington
//This library is free software : you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.
//
// see <http://www.gnu.org/licenses/> for a full copy of the license.
//Also note that this library CANNOT be used in commercial applications 
//due to restrictions within steinberg's VST license.

using UnityEngine;
using System.Runtime.InteropServices;
using System;
using CppDebug;

namespace pluginHost
{
    //[ExecuteInEditMode]
    public class VSTEffect : MonoBehaviour
    {
        //important stuff 
        [Header("Alpha build: Windows")]
        [Header("Only supports 64bit VST2 effects (not VSTi)")]
        [Space]
        public string pluginPath = "";
        public bool MonoOutput = false;
        private int thisVSTIndex = 0;
        [Space]

        //////////////////////  params  //////////////////////
        [Header("Parameters")]
        public int numParams;
        [Range(0.0f, 1.0f)]
        public float[] parameters;
        public string[] paramNames;
        private float[] previousParams;

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

            if (pluginPath == "")
                pluginPath = Application.dataPath + "\\VSTHost\\VSTPlugins\\TAL-Reverb-2";

            thisVSTIndex = loadEffect(pluginPath);
            if(thisVSTIndex == -1)
            {
                Debug.Log("Error, VST has failed to load. Unsupported file path or format");
                pluginFailedToLoad = true;
                return;
            }
            setupParams();
            setupIO();
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

            //alloc unmanaged memory
            audioPtrSize = Marshal.SizeOf(audioThroughArray[0][0]) * audioThroughArray[0].Length * audioThroughArray.Length;
            inputArrayAsVoidPtr = Marshal.AllocHGlobal(audioPtrSize);
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
            //alloc unmanaged memory
            messagePtrSize = 8 * 256;
            messageAsVoidPtr = Marshal.AllocHGlobal(messagePtrSize);
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
