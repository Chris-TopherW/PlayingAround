using System.Runtime.InteropServices;
using System;

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
