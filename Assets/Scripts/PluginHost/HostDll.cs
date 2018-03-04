using System.Runtime.InteropServices;
using System;

public static class HostDll {

    [DllImport("VSTHostUnity", EntryPoint = "loadPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void loadPlugin(/*string filepath*/);
    [DllImport("VSTHostUnity", EntryPoint = "processAudio", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr processAudio(IntPtr input, long numFrames, int numChannels);
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
    public static extern IntPtr getParamName(int index);
    [DllImport("VSTHostUnity", EntryPoint = "getNumPluginOutputs", CallingConvention = CallingConvention.Cdecl)]
    public static extern int getNumPluginOutputs();
    [DllImport("VSTHostUnity", EntryPoint = "getNumPluginInputs", CallingConvention = CallingConvention.Cdecl)]
    public static extern int getNumPluginInputs();
}
