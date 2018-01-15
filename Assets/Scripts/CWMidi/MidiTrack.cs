using System.Collections.Generic;
using System;

namespace cwMidi
{
    public class MidiTrack
    {
        private const int headerLen = 8;
        private byte[] trackHeader;
        //letters MTrk
        private readonly byte[] trkStartMes = { 0x4D, 0x54, 0x72, 0x6B };
        private byte[] trkLenBytes = { 0x00, 0x00, 0x00, 0x00 };
        private ushort trkLen;
        private readonly byte[] trkEndMes = { 0x00, 0xFF, 0x2F, 0x00 };
        private long trackPPQLen = 0;
        public long trackPPQAbsolutePos = 0;
        private int trackNum = -1;

        private List<MidiMessage> trackMessages = new List<MidiMessage>();
        private int numNotes = 0;

        public MidiTrack()
        {
            trackHeader = new byte[headerLen];
            for(int i = 0; i < 4; i++)
                trackHeader[i] = trkStartMes[i];
            for (int i = 4; i < 8; i++)
                trackHeader[i] = trkLenBytes[i - 4];
        }

        public void AddNote(MidiMessage p_message)
        {
            trackMessages.Add(p_message);
            if (p_message == null) UnityEngine.Debug.Log("Message is NULL");
            p_message.setOwnerTrack(this);
            trackPPQLen += p_message.getTimeStamp();
            p_message.setAbsTimestamp(trackPPQLen);
            numNotes++;
            UInt16 numBytesInMes = p_message.getNumBytes();
            trkLen += numBytesInMes;

            trkLenBytes = BitConverter.GetBytes(numBytesInMes);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(trkLenBytes);
        }

        //public void insertNote(MidiMessage p_message, int p_index)
        //{
        //    trackMessages.Insert(p_index, p_message);
        //    numNotes++;
        //    UInt16 numBytesInMes = p_message.getNumBytes();
        //    trkLen += numBytesInMes;

        //    trkLenBytes = BitConverter.GetBytes(numBytesInMes);
        //    if (BitConverter.IsLittleEndian)
        //        Array.Reverse(trkLenBytes);
        //    //need to rearrange abs vals
        //}

        //public void RemoveNote(int p_index)
        //{
        //    trackMessages.RemoveAt(p_index);
        //    numNotes--;
        //    //also need to rearrange absolute values in here!
        //}

        public void clearTrack()
        {
            trackMessages.Clear();
            numNotes = 0;
        }

        public List<MidiMessage> getMessages() { return trackMessages; }

        public byte[] toByteArray()
        {
            List<byte> midiByteArray = new List<byte>();
            for(int i = 0; i < headerLen; i++)
            {
                midiByteArray.Add(trackHeader[i]);
            }
            for(int i = 0; i < trackMessages.Count; i++)
            {
                byte[] rawMessage = trackMessages[i].toByteArray();
                for(int j = 0; j < rawMessage.Length; j++)
                {
                    midiByteArray.Add(rawMessage[j]);
                }
            }
            for(int i = 0; i < trkEndMes.Length; i++)
            {
                midiByteArray.Add(trkEndMes[i]);
            }
            
            return midiByteArray.ToArray();
        }
        
        public MidiMessage getNote(int p_index)
        {
            if (p_index >= numNotes)
            {
                UnityEngine.Debug.Log("<color=red>Error: getNote(arg) must be lower than numNotes - 1</color>");
                return null;
            }   
            else
                return trackMessages[p_index];
        }
        public int getNumNotes() { return numNotes; }
        public int getTrackNum() { return trackNum; }
        public long getTrackPPQReadPos() { return trackPPQAbsolutePos; }
        public void setTrackNum(int p_trackNum) { trackNum = p_trackNum; }
    }
}
