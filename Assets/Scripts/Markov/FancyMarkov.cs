using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cwMarkov;
using cwMidi;

public class FancyMarkov : MonoBehaviour {

    private TransitionMatrix matrix;
    private TextAsset file;
    private cwMidi.MidiFile midiFile;
    private MidiTrack midiTrack;
    int midiOutputDevice;
    private MidiSource midiSource;

    private void Awake()
    {
        file = GetComponent<MidiSource>().MidiClip;
        midiFile = new cwMidi.MidiFile(file);
        midiTrack = new MidiTrack();
        midiOutputDevice = MidiPlayer.Start();
        if (Midi.debugLevel > 3) midiFile.printCookedMidiFile();
        matrix = new TransitionMatrix(midiFile.getMidiTrack(1));
        midiSource = GetComponent<MidiSource>();
    }

    void Start () {

        Debug.Log("Midi output device: " + midiOutputDevice);
        MidiMessage mes;
        MidiMessage _off;
        int timestamp = 0;
        GetComponent<MidiSource>().startTimeOffset = AudioSettings.dspTime * 1000;

        for (int i = 0; i < 5; i++)
        {
            mes = matrix.getNextNote();
            mes.setAbsTimestamp(timestamp);
            _off = new MidiMessage(0x80, (byte)mes.getByteOne(), 0x00);
            _off.setAbsTimestamp(timestamp + 250);

            MidiPlayer.PlayNext(mes, midiSource);
            MidiPlayer.PlayNext(_off, midiSource);
            timestamp += 500;
        }
        MidiPlayer.reorderQueue();
    }
}
