using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cwMidi
{
    public static class MidiPlayer
    {
        private static double metronomeStartTime = 0.0;
        public static int deviceNum = 0;
        private static double previousEventMS = 0.0;

        public static int Start()
        {
            return PortMidi.main_test_output();
        }

        public static void PlayScheduled(MidiMessage p_message, double p_time)
        {
            double theTime = AudioSettings.dspTime;

            PortMidi.midiEvent(p_message.getStatusByte(), p_message.getByteOne(), p_message.getByteTwo(), 
                             (int)((p_time - theTime) * 1000.0));
            previousEventMS = (p_time - theTime) * 1000.0;
        }

        //this should be wrapped up in score playback section?
        public static void PlayScheduled(MidiMessage p_message)
        {
            int ppqTime = p_message.getTimeStamp();
            double msOffset = Metronome.ppqToMs(ppqTime);
            PortMidi.midiEvent(p_message.getStatusByte(), p_message.getByteOne(), p_message.getByteTwo(),
                             (int)(previousEventMS + msOffset)); //this is wrong logic- will work for midi file but nothing else...
            previousEventMS = previousEventMS + msOffset; //set for next note
        }

        public static void resetMidiEventClock()
        {
            previousEventMS = 0.0;
        }

        public static void SetBPM(int p_BPM)
        {

        }

        public static void SetPPQ(int p_PPQ)
        {

        }

        public static void StartMetronome(int p_BPM)
        {
            metronomeStartTime = AudioSettings.dspTime;
        }

        public static void Stop()
        {

        }

        public static void Stop(int p_delay)
        {

        }

        public static void Clear()
        {

        }

        public static int Shutdown()
        {
            PortMidi.shutdown();
            PortMidi.Pm_Terminate();
            return 1;
        }
    }

}
