using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cwMidi
{
    public static class Metronome
    {

        private static int BPM = 120;
        private static double metronomeStartTime = 0.0;

        public static double ppqToMs(int p_timestamp)
        {
            double msPerBeat = (60.0 / BPM) * 1000.0;
            double ppqPerMs = msPerBeat / 960.0;
            return ppqPerMs * p_timestamp;
        }

        public static void startMetro(int p_BPM)
        {
            BPM = p_BPM;
            metronomeStartTime = AudioSettings.dspTime;
        }

        public static double getMetroStartTime() { return metronomeStartTime; }
    }

}

