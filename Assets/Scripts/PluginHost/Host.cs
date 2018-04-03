using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pluginHost
{
    public static class pluggoHost
    {
        public static int blockSize;
        public static long sampleRate;
        //first is key, second is value
        public static Dictionary<string, int> pluginIndexDict;

        public static void init()
        {
            pluginIndexDict = new Dictionary<string, int>();
            HostDllCpp.initHost();

            ////////////////////// setup io //////////////////////
            int _numBuff;
            AudioSettings.GetDSPBufferSize(out blockSize, out _numBuff);
            //Debug.Log("block size = " + blockSize);
            sampleRate = AudioSettings.outputSampleRate;
            HostDllCpp.setHostBlockSize(blockSize);
            HostDllCpp.setSampleRate(sampleRate);
        }
    }

    public class Host : MonoBehaviour
    {
        private void Awake()
        {
            pluggoHost.init();
        }
    }
}
