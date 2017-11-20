using System.Collections;
using System.Collections.Generic;
using System;

namespace cwMidi
{
    public class MidiMessage
    {
        private byte[] messageAsBytes;
        private UInt16 numBytes = 0;
        private int bytesInTimeStamp;

        private int PPQ = 960;
        private int timeStamp = 0;
        private int status = 0;
        private int midiEvent = 0;
        private int channel = 0;
        private int controlByte1 = 0;
        private int controlByte2 = 0;

        public MidiMessage(byte[] p_messageAsBytes, int p_bytesInTimeStamp = 1)
        {
            messageAsBytes = p_messageAsBytes;
            numBytes = (ushort)messageAsBytes.Length;

            bytesInTimeStamp = p_bytesInTimeStamp;
            byte[] timeStampRaw = new byte[bytesInTimeStamp];
            for (int i = 0; i < bytesInTimeStamp; i++)
            {
                timeStampRaw[i] = messageAsBytes[i];
            }
            timeStamp = midiHexTimeToNormalTime(timeStampRaw);
            status = messageAsBytes[p_bytesInTimeStamp];
            midiEvent = messageAsBytes[p_bytesInTimeStamp] & 0xF0;
            channel = messageAsBytes[p_bytesInTimeStamp] & 0x0F;
            controlByte1 = messageAsBytes[p_bytesInTimeStamp + 1];
            controlByte2 = messageAsBytes[p_bytesInTimeStamp + 2];
        }

        public MidiMessage(byte stat, byte data1, byte data2)
        {
            status = stat;
            midiEvent = status & 0xF0;
            channel = status & 0x0F;
            controlByte1 = data1;
            controlByte2 = data2;
        }

        public void print()
        {
            UnityEngine.Debug.Log("Time = " + timeStamp +"  Event = " + midiEvent.ToString("X") + "  channel = " + channel.ToString("X") +
                "  control byte 1 = " + controlByte1 + "  control byte 2 = " + controlByte2);
        }

        public byte[] toByteArray()
        {
            return messageAsBytes;
        }

        public void setTimestamp(int p_timestamp)
        {
            timeStamp = p_timestamp;
        }

        public ushort getNumBytes() { return (ushort)messageAsBytes.Length; }
        public int getByteOne() { return controlByte1; }
        public int getByteTwo() { return controlByte2; }
        public int getTimeStamp() { return timeStamp; }
        public int getPPQ() { return PPQ; }
        public int getStatusByte() { return status;  }
        public int getMidiEvent() { return midiEvent; }


        private int midiHexTimeToNormalTime(byte[] n)
        {
            int len = n.Length;
            int t = 0;
            for (int i = 0; i < len - 1; i++)
            {
                t += (n[i] - 128) * (int)UnityEngine.Mathf.Pow(2, 7 * (len - i - 1));
            }
            t += n[len - 1];
            return t;
        }
    }

    //public class NoteOn : MidiMessage
    //{
    //    public byte amplitude;
    //    public byte note;

    //}

    //public class NoteOff : MidiMessage
    //{

    //}

}

