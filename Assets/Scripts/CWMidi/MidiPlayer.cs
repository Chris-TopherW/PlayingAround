using System.Collections.Generic;
using UnityEngine;

namespace cwMidi
{
    public static class MidiPlayer
    {
        private static double metronomeStartTimeMs = 0.0;
        public static int deviceNum = 0;
        private static double previousEventMS = 0.0;
        public static List<MidiMessage> messOutBuff;
        private static double updateLookAhead = 1000; //ms
        private static bool hasStarted = false;

        public static int Start()
        {
            if (!hasStarted)
            {
                hasStarted = true;
                messOutBuff = new List<MidiMessage>();
                //startTime = AudioSettings.dspTime;
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
            messOutBuff.Add(p_message);
            if(p_message.getOwnerTrack() != null)
                p_message.getOwnerTrack().trackPPQAbsolutePos = p_message.getAbsTimeStamp(); //this sets write head for ppq
        }

        public static void PlayTrack(MidiTrack p_track, MidiSource p_source)
        {
            //resetMidiEventClock();
            p_track.trackPPQAbsolutePos = 0; //problem if playing the same track twice... Maybe should store in MidiSource obj
            long accumulatedTrackLenPPQ = 0;
            for (int _notes = 0; _notes < p_track.getNumNotes(); _notes++)
            {
                //hmm, not quite...
                MidiMessage nextNote = p_track.getNote(_notes);
                PlayNext(nextNote, p_source);
                accumulatedTrackLenPPQ += nextNote.getPPQ();
            }
            //if (p_source.Loop)
            //{
            //    for (int _notes = 0; _notes < p_track.getNumNotes(); _notes++)
            //    {
            //        MidiMessage nextNote = p_track.getNote(_notes).copy();
            //        nextNote.setAbsTimestamp(nextNote.getAbsTimeStamp() + accumulatedTrackLen); //here's the problem! changing this messes with everything
            //       PlayNext(nextNote, p_source);
            //    }
            //}
        }

        public static void Update()
        {
            double currentTime = (AudioSettings.dspTime) * 1000.0;
            if(messOutBuff.Count > 0)
            {
                MidiMessage temporaryMessage = messOutBuff[0];
                double msUntilEvent = temporaryMessage.noteSource.startTimeOffset + Metronome.ppqToMs(temporaryMessage.getAbsTimeStamp());

                //this while accounts for multiple notes at once
                while (msUntilEvent < updateLookAhead && messOutBuff.Count > 0)
                {
                    if (Midi.debugLevel > 0) Debug.Log("Absolute timestamp = " + temporaryMessage.getAbsTimeStamp());
                    long msOffset = (long)(msUntilEvent);
                    ///*if (Midi.debugLevel > 0) */UnityEngine.Debug.Log("Event time: " + msOffset);
                    if (msOffset < 0)
                    {
                        Debug.Log("<color=red>Error: negative event time offset : </color>" + msOffset + ", setting time to 0");
                        Debug.Log("source start time offset : " + temporaryMessage.noteSource.startTimeOffset);
                        Debug.Log("MS time of note relative to track start : " + Metronome.ppqToMs(temporaryMessage.getAbsTimeStamp()));
                        Debug.Log("Current time : " + currentTime);
                        Debug.Log("metro start time : " + metronomeStartTimeMs);
                        msOffset = 0;
                    }
                    
                    MidiMessage p_message = messOutBuff[0];
                    messOutBuff.RemoveAt(0);
                    int statusByte;
                    int amplitude;

                    if (p_message.noteSource.ForceToChannel)
                    {
                        statusByte = p_message.getStatusByte() & 0xF0;
                        statusByte = statusByte += p_message.noteSource.Channel - 1;
                    }
                        
                    else 
                        statusByte = p_message.getStatusByte(); 

                    if (p_message.noteSource.Mute) 
                        amplitude = 0; 
                    else 
                        amplitude = (int)(p_message.getByteTwo() * p_message.getGain()); 

                    PortMidi.midiEvent(statusByte, p_message.getByteOne() + p_message.noteSource.PitchOffset, amplitude, (int)(msOffset));

                    if (messOutBuff.Count > 0)
                    {
                        temporaryMessage = messOutBuff[0];
                        msUntilEvent =  temporaryMessage.noteSource.startTimeOffset +  Metronome.ppqToMs(temporaryMessage.getAbsTimeStamp()) - currentTime;
                    }
                }
            }
        }

        public static void resetMidiEventClock()
        {
            metronomeStartTimeMs = (AudioSettings.dspTime) * 1000; //m/s since start of program- provides offset
        }

        public static void SetBPM(int p_BPM)
        {
            Metronome.setBPM(p_BPM);
        }

        public static void StartMetronome(int p_BPM)
        {
            Metronome.setBPM(p_BPM);
            metronomeStartTimeMs = AudioSettings.dspTime * 1000;
        }

        public static double getMetronomeStartTime() { return metronomeStartTimeMs; }
        public static void setMetronomeStartTimeMs(double p_time) { metronomeStartTimeMs = p_time; }

        public static void reorderQueue()
        {
            messOutBuff.Sort((a, b) => { return a.getAbsTimeStamp().CompareTo(b.getAbsTimeStamp()); });
        }

        public static int Shutdown()
        {
            PortMidi.shutdown();
            PortMidi.Pm_Terminate();
            //metronomeStartTimeMs = 0;
            return 1;
        }
    }
}
