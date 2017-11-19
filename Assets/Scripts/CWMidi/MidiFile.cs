using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace cwMidi
{
    public class MidiFile : MidiHeader
    {
        Dictionary<byte[], string> MessType;

        protected List<byte> midiByteArray;
        protected byte[] readFile;
        private List<MidiTrack> midiTracks;

        private ushort bpm = 0;
        private ushort trackType = 0;
        private ushort numTracks = 0;

        public MidiFile(UnityEngine.TextAsset p_file)
        {
            readFile = p_file.bytes;
            midiTracks = new List<MidiTrack>();
            ushort readPos = headerSize; // start from pos 14 in midi array- after header

            setMidiType(readFile[9]);
            setNumTracks(readFile[11]);

            for(int i = 0; i < getNumTracks(); i++)
            {
                readPos += 4; // 4 bytes for first half of track header
                readPos = readTrack(readPos);
            }
        }

        private ushort readTrack(ushort p_readPos)
        {
            byte[] trackSizeRaw = new byte[4];
            for(int i = 0; i < 4; i++)
            {
                trackSizeRaw[i] = readFile[p_readPos];
                p_readPos++;
            }
            if (BitConverter.IsLittleEndian)
                Array.Reverse(trackSizeRaw);
            ushort trackSize = BitConverter.ToUInt16(trackSizeRaw, 0);

            MidiTrack track = new MidiTrack();

            byte runningStatus = 0x00;
            bool canUseRunningStatus = false;
            //add all notes here. 4 is num bytes in track end message.
            
            while (p_readPos < (trackSize + headerSize + 8))
            {
                while(p_readPos + 2 < (trackSize + headerSize + 8))
                {
                    if (readFile[p_readPos] == 0x00 && readFile[p_readPos + 1] == 0xff)
                    {
                        p_readPos += 2;
                        switch (readFile[p_readPos])
                        {
                            case 0x00:
                                p_readPos += 2;
                                //00 FF 00 02 seqNum
                                //sequenceNum
                                break;
                            case 0x01:
                                //textEvent
                                p_readPos++;
                                p_readPos += (ushort)(readFile[p_readPos]); //len byte and text length
                                break;
                            case 0x02:
                                //copyrightMes
                                p_readPos++;
                                p_readPos += (ushort)(readFile[p_readPos]); //len byte and text length
                                break;
                            case 0x03:
                                //trackName
                                p_readPos++;
                                p_readPos += (ushort)(readFile[p_readPos]); //len byte and text length
                                break;
                            case 0x04:
                                //instName
                                p_readPos++;
                                p_readPos += (ushort)(readFile[p_readPos]); //len byte and text length
                                break;
                            case 0x05:
                                //lyrics
                                p_readPos++;
                                p_readPos += (ushort)(readFile[p_readPos]); //len byte and text length
                                break;
                            case 0x06:
                                //textMarker
                                p_readPos++;
                                p_readPos += (ushort)(readFile[p_readPos]); //len byte and text length
                                break;
                            case 0x07:
                                //cuePoint
                                p_readPos++;
                                p_readPos += (ushort)(readFile[p_readPos]); //len byte and text length
                                break;
                            case 0x20:
                                //midiChannelPrefix
                                p_readPos += 2;
                                break;
                            case 0x2f:
                                //endOfTrack
                                p_readPos++;
                                break;
                            case 0x51:
                                //setTempo
                                p_readPos += 4;
                                break;
                            case 0x54:
                                //SMPTEOffset
                                p_readPos += 6;
                                break;
                            case 0x58:
                                //timeSignature
                                p_readPos += 5;
                                break;
                            case 0x59:
                                //keySignature
                                p_readPos += 3;
                                break;
                        }
                        p_readPos++; //move to next read pos
                    }
                    else break;
                }
                
                if(p_readPos < (trackSize + headerSize + 8))
                {
                    List<byte> rawMessage = new List<byte>();
                    int numBytesTimestamp = 1;

                    //check for large timestamps
                    while (readFile[p_readPos] > 0x7F)
                    {
                        rawMessage.Add(readFile[p_readPos++]);
                        numBytesTimestamp++;
                    }

                    rawMessage.Add(readFile[p_readPos++]); //time stamp size will always be >= 1

                    //check if status byte or ctl byte is ignored
                    if ((readFile[p_readPos] & 0xF0) >= 0x80 && (readFile[p_readPos] & 0xF0) <= 0xE0)
                    {
                        canUseRunningStatus = true;
                        runningStatus = readFile[p_readPos]; //store status for later
                        rawMessage.Add(readFile[p_readPos++]); // add status byte- event type and channel
                        rawMessage.Add(readFile[p_readPos++]); // add note byte
                        rawMessage.Add(readFile[p_readPos++]); //add velocity byte
                    }
                    else if ((readFile[p_readPos] & 0xF0) == 0xF0)
                    {
                        canUseRunningStatus = false;
                        rawMessage.Add(readFile[p_readPos++]); // add status byte
                        rawMessage.Add(0x00); // add ctl byte- 0 as it's not used
                        rawMessage.Add(readFile[p_readPos++]); //add velocity byte
                    }
                    else if ((readFile[p_readPos] & 0xF0) <= 0x70 && canUseRunningStatus)
                    {
                        rawMessage.Add(runningStatus); // add running status message
                        rawMessage.Add(readFile[p_readPos++]); // add note byte
                        rawMessage.Add(readFile[p_readPos++]); //add velocity byte
                    }

                    MidiMessage mes = new MidiMessage(rawMessage.ToArray(), numBytesTimestamp);
                    track.AddNote(mes);
                }
                    
            }
            addTrack(track);
            return p_readPos;
        }
       
        public void addTrack(MidiTrack p_track)
        {
            midiTracks.Add(p_track);
            if (getNumTracks() > 1) setMidiType(1);
        }

        public byte[] midiFileAsByteArray()
        {
            List<byte> dataList = new List<byte>();
            for(int i = 0; i < headerFile.Length; i++)
            {
                dataList.Add(headerFile[i]);
            }
            //loop through all tracks and extract byte data
            for(int i = 0; i < midiTracks.Capacity; i++)
            {
                byte[] trackData = midiTracks[i].toByteArray();
                for(int j = 0; j < trackData.Length; j++)
                {
                    dataList.Add(trackData[j]);
                }
            }
            return dataList.ToArray();
        }

        public List<MidiTrack> getMidiTracks()
        {
            return midiTracks;
        }

        public MidiTrack getMidiTrack(int p_index)
        {
            return midiTracks[p_index];
        }

        public void printRawMidiFile()
        {
            for (int i = 0; i < readFile.Length; i++)
            {
                UnityEngine.Debug.Log("midi message: " + readFile[i].ToString("X"));
            }
        }

        public void printCookedMidiFile()
        {
            UnityEngine.Debug.Log("BPM: " + getBpm());
            UnityEngine.Debug.Log("midiType: " + getMidiType());
            UnityEngine.Debug.Log("numTracks: " + getNumTracks());
            //int i = 0;
            foreach (MidiTrack mTrk in midiTracks)
            {
                foreach (MidiMessage mes in mTrk.getMessages())
                {
                    mes.print();
                }
            }
        }
    }
}
