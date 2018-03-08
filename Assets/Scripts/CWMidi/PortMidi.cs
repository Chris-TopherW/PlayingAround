using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

//this talks directly to C DLL. For some reason it slows things down on build a lot...

namespace cwMidi
{
    public static class PortMidi
    {

        [DllImport("portmidi", EntryPoint = "Pm_Initialize")]
        public static extern int Pm_Initialize();

        [DllImport("portmidi", EntryPoint = "Pm_Terminate")]
        public static extern int Pm_Terminate();

        [DllImport("portmidi", EntryPoint = "Pm_CountDevices")]
        public static extern int Pm_CountDevices();

        [DllImport("portmidi", EntryPoint = "main_test_output")]
        public static extern int main_test_output();

        [DllImport("portmidi", EntryPoint = "midiEvent")]
        public static extern void midiEvent(int status, int mess1, int mess2, int delayMs);

        [DllImport("portmidi", EntryPoint = "getNamePointer")]
        public static extern IntPtr getNamePointer(int index);

        [DllImport("portmidi", EntryPoint = "getNumDevices")]
        public static extern int getNumDevices();

        [DllImport("portmidi", EntryPoint = "shutdown")]
        public static extern void shutdown();


        public static string getDeviceName(int index)
        {
            return "Device Name here";
        }
    }    
}
