using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cwMidi;

//Midi note including length for markov model analysis

public class MarkovNote : MidiMessage{

    public long length;

	public MarkovNote(byte stat, byte data1, byte data2, long length = -1)
    {
        messageAsBytes = new byte[4];
        messageAsBytes[0] = 0x00;
        messageAsBytes[1] = stat;
        messageAsBytes[2] = data1;
        messageAsBytes[3] = data2;

        status = stat;
        midiEvent = status & 0xF0;
        channel = status & 0x0F;
        controlByte1 = data1;
        controlByte2 = data2;
    }

    public MarkovNote(MidiMessage _mes, long length = -1)
    {
        messageAsBytes = new byte[4];
        messageAsBytes = _mes.getMessageAsBytes();

        status = _mes.getStatusByte();
        midiEvent = status & 0xF0;
        channel = status & 0x0F;
        controlByte1 = _mes.getByteOne();
        controlByte2 = _mes.getByteTwo();
    }
}
