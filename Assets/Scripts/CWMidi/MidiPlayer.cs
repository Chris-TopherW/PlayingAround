using System.Collections.Generic;
using UnityEngine;

namespace cwMidi
{
    public static class MidiPlayer
    {
        private static double metronomeStartTime = 0.0;
        public static int deviceNum = 0;
        private static double previousEventMS = 0.0;
        public static Queue<MidiMessage> mesOutBuff; //swap this for normal list 
        private static double updateLookAhead = 1000; //ms
        private static double startTime = 0.0;
        private static bool hasStarted = false;

        public static int Start()
        {
            if (!hasStarted)
            {
                hasStarted = true;
                mesOutBuff = new Queue<MidiMessage>();
                startTime = AudioSettings.dspTime;
                return PortMidi.main_test_output();
            }
            else
                return -1;
        }

        public static void PlayScheduled(MidiMessage p_message, double p_time)
        {
            if (p_time < AudioSettings.dspTime)
            {
                Debug.Log("<color=yellow>Warning:</color> time must not be in past!");
                return;
            }
            double theTime = AudioSettings.dspTime;

            PortMidi.midiEvent(p_message.getStatusByte(), p_message.getByteOne(), p_message.getByteTwo(), 
                             (int)((p_time - theTime) * 1000.0));
            previousEventMS = (p_time - theTime) * 1000.0;
        }

        public static void Play(MidiMessage p_message)
        {
            if(Midi.debugLevel > 4) Debug.Log("Add note to play " + p_message.getByteOne()); 

            PortMidi.midiEvent(p_message.getStatusByte(), p_message.getByteOne(), p_message.getByteTwo(), 0);
        }

        public static void PlayTrackNext(MidiTrack p_track, MidiSource p_source)
        {
            for(int _notes = 0; _notes < p_track.getNumNotes(); _notes++)
            {
                PlayNext(p_track.getNote(_notes), p_source);
            }
        }

        public static void PlayNext(MidiMessage p_message, MidiSource p_source)
        {
            if (Midi.debugLevel > 4) Debug.Log("Add note to play " + p_message.getByteOne() + " " + p_message.getByteTwo() + " at time: " + p_message.getAbsTimeStamp());
            p_message.noteSource = p_source;
            mesOutBuff.Enqueue(p_message);
            if(p_message.getOwnerTrack() != null)
                p_message.getOwnerTrack().trackPPQAbsolutePos = p_message.getAbsTimeStamp(); //this sets write head for ppq
        }

        public static void PlayTrack(MidiTrack p_track, MidiSource p_source)
        {
            if (Midi.debugLevel > 3) Debug.Log("Play Midi track");

            resetMidiEventClock();
            p_track.trackPPQAbsolutePos = 0; //problem if playing the same track twice... Maybe should store in MidiSource obj
            //long accumulatedTrackLen = 0;
            for (int _notes = 0; _notes < p_track.getNumNotes(); _notes++)
            {
                //hmm, not quite...
                MidiMessage nextNote = p_track.getNote(_notes);
                PlayNext(nextNote, p_source);
               // accumulatedTrackLen += nextNote.getPPQ();
            }
            //if(p_source.Loop)
            //{
            //    for (int _notes = 0; _notes < p_track.getNumNotes(); _notes++)
            //    {
            //        MidiMessage nextNote = p_track.getNote(_notes);
            //        //nextNote.setAbsTimestamp(nextNote.getAbsTimeStamp() + accumulatedTrackLen); //here's the problem! changing this messes with everything
            //        //PlayNext(nextNote, p_source);
            //    }
            //}
        }

        public static void Update()
        {
            double currentTime = (AudioSettings.dspTime - startTime) * 1000.0;
            if(mesOutBuff.Count > 0)
            {
                MidiMessage temporaryMessage = mesOutBuff.Peek();
                //double msUntilEvent =  Metronome.ppqToMs(temporaryMessage.getAbsTimeStamp()) - (currentTime - metronomeStartTime);
                double msUntilEvent = temporaryMessage.noteSource.startTimeOffset + Metronome.ppqToMs(temporaryMessage.getAbsTimeStamp()) - currentTime - metronomeStartTime;


                while (msUntilEvent < updateLookAhead + temporaryMessage.noteSource.startTimeOffset && mesOutBuff.Count > 0)
                {
                    Debug.Log("Absolute timestamp = " + temporaryMessage.getAbsTimeStamp());
                    long msOffset = (long)(msUntilEvent);
                    UnityEngine.Debug.Log("Event time: " + msOffset);
                    if (msOffset < 0)
                    {
                        msOffset = 0;
                        Debug.Log("<color=red>Error: negative event time offset</color>");
                    }
                    
                    MidiMessage p_message = mesOutBuff.Dequeue();
                    int statusByte;
                    int amplitude;

                    if (p_message.noteSource.ForceToChannel) 
                        statusByte = p_message.getStatusByte() | p_message.noteSource.Channel; 
                    else 
                        statusByte = p_message.getStatusByte(); 

                    if (p_message.noteSource.Mute) 
                        amplitude = 0; 
                    else 
                        amplitude = (int)(p_message.getByteTwo() * p_message.getGain()); 

                    PortMidi.midiEvent(statusByte, p_message.getByteOne() + p_message.noteSource.PitchOffset, amplitude, (int)(msOffset));

                    if (mesOutBuff.Count > 0)
                    {
                        temporaryMessage = mesOutBuff.Peek(); //check next element for timestamp pos
                        msUntilEvent = Metronome.ppqToMs(temporaryMessage.getAbsTimeStamp()) - (currentTime - metronomeStartTime);
                    }
                }
            }
        }

        public static void resetMidiEventClock()
        {
            metronomeStartTime = (AudioSettings.dspTime - startTime) * 1000; //m/s since start of program- provides offset
        }

        public static void SetBPM(int p_BPM)
        {
            Metronome.setBPM(p_BPM);
        }

    //public static void SetPPQ(int p_PPQ)
    //{

    //}

    public static void StartMetronome(int p_BPM)
        {
            Metronome.setBPM(p_BPM);
            metronomeStartTime = AudioSettings.dspTime;
        }

        //public static void Stop()
        //{

        //}

        //public static void Stop(int p_delay)
        //{

        //}

        //public static void Clear()
        //{

        //}

        public static int Shutdown()
        {
            PortMidi.shutdown();
            PortMidi.Pm_Terminate();
            startTime = 0;
            metronomeStartTime = 0;
            return 1;
        }
    }
}
