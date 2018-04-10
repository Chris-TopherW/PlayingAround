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

//This script allows C++ debug messages to be logged within C#

using AOT;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_EDITOR

namespace CppDebug
{
    public class DebugCPP : MonoBehaviour
    {
        private bool showNativeDebug = false;
        private static bool staticAllowDebug = true;
        void OnEnable()
        {
            RegisterDebugCallback(OnDebugCallback);
            staticAllowDebug = showNativeDebug;
        }

        private void Update()
        {
            staticAllowDebug = showNativeDebug;
        }

        [DllImport("VSTHostUnity", CallingConvention = CallingConvention.Cdecl)]
        static extern void RegisterDebugCallback(debugCallback cb);
        delegate void debugCallback(IntPtr request, int size);
        [MonoPInvokeCallback(typeof(debugCallback))]
        static void OnDebugCallback(IntPtr request, int size)
        {
            if (!staticAllowDebug)
                return;
            string debug_string = Marshal.PtrToStringAnsi(request, size);

            UnityEngine.Debug.Log(debug_string);
        }
    }
}
#endif
