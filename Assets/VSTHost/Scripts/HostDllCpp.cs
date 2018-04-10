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

using System.Runtime.InteropServices;
using System;
using CppDebug;

namespace pluginHost
{
    public static class HostDllCpp
    {
        ///////////////////////////////////////////Loading///////////////////////////////////////////

        const string cppDllName = "VSTHostUnity";
        [DllImport(cppDllName, EntryPoint = "initHost", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void initHost();

        [DllImport(cppDllName, EntryPoint = "loadEffect", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int loadEffect(IntPtr path);

        [DllImport(cppDllName, EntryPoint = "loadInstrument", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int loadInstrument(IntPtr path);

        ///////////////////////////////////////////Parameters///////////////////////////////////////////

        [DllImport(cppDllName, EntryPoint = "setSampleRate", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int setSampleRate(long p_sampleRate);

        [DllImport(cppDllName, EntryPoint = "setHostBlockSize", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int setHostBlockSize(int p_blocksize);

        [DllImport(cppDllName, EntryPoint = "getNumParams", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getNumParams(int vstIndex);

        [DllImport(cppDllName, EntryPoint = "getParam", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern float getParam(int vstIndex, int paramIndex);

        [DllImport(cppDllName, EntryPoint = "getParamName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getParamName(int vstIndex, int paramIndex);

        [DllImport(cppDllName, EntryPoint = "setParam", CallingConvention = CallingConvention.Cdecl)]
        public static extern int setParam(int vstIndex, int paramIndex, float p_value);

        ///////////////////////////////////////////IO/////////////////////////////////////////

        [DllImport(cppDllName, EntryPoint = "processFxAudio", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr processFxAudio(int vstIndex, IntPtr audioThrough, long numFrames, int numChannels);

        [DllImport(cppDllName, EntryPoint = "processInstAudio", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr processInstAudio(int vstIndex, long numFrames, int numChannels);

        [DllImport(cppDllName, EntryPoint = "getNumPluginInputs", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getNumPluginInputs(int vstIndex);

        [DllImport(cppDllName, EntryPoint = "getNumPluginOutputs", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getNumPluginOutputs(int vstIndex);

        //////////////////////////////////////clean up///////////////////////////////////////

        [DllImport(cppDllName, EntryPoint = "clearVSTs", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clearVSTs();
    }
}
