using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cwMidi
{
    public static class Metronome
    {

        private static double BPM = 120;
        private static double metronomeStartTime = 0.0;

        public static double ppqToMs(long p_timestamp)
        {
            double msPerBeat = (60.0 / BPM) * 1000.0;
            double msPerPPQ = msPerBeat / 960.0;
            return msPerPPQ * p_timestamp;
        }

        private static double msToPPQ(double p_ms)
        {
            double msPerBeat = (60.0 / BPM) * 1000.0;
            double msPerPPQ = msPerBeat / 960.0;
            return p_ms / msPerPPQ;
        }

        public static long msToSamps(double p_ms)
        {
            return (long)((p_ms / 1000.0) * AudioSettings.outputSampleRate);
        }

        public static void startMetro(int p_BPM)
        {
            BPM = p_BPM;
            metronomeStartTime = AudioSettings.dspTime;
        }

        public static double ppqDspTime()
        {
            return msToPPQ(AudioSettings.dspTime - metronomeStartTime);
        }

        public static double getMetroStartTime() { return metronomeStartTime; }
        public static void setBPM(double p_BPM) { BPM = p_BPM; }
    }
}
