using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cwMarkov;
using cwMidi;

public class FancyMarkov : MonoBehaviour {

    private TransitionMatrix matrix;
    public TextAsset file;
    private cwMidi.MidiFile midiFile;
    private MidiTrack midiTrack;
    int midiOutputDevice;

    private void Awake()
    {
        midiFile = new cwMidi.MidiFile(file);
        midiTrack = new MidiTrack();
        midiOutputDevice = MidiPlayer.Start();
        if (Midi.debugLevel > 3) midiFile.printCookedMidiFile();
        matrix = new TransitionMatrix(midiFile.getMidiTrack(1));
    }

    void Start () {

        Debug.Log("Midi output device: " + midiOutputDevice);
        //PortMidi.midiEvent(0x90, 60, 0x1F, 0);
        //PortMidi.midiEvent(0x90, 60, 0x00, 1000);
        //MidiPlayer.resetMidiEventClock();
        //int timeStamp = 0;
        ////for (int i = 0; i < 5; i++)
        ////{
        //    MarkovNote _note = matrix.getNextNote();
        //    _note.setAbsTimestamp(timeStamp = 960);
        //    MidiMessage _mes = _note;
        //    MidiMessage _off = new MidiMessage(0x80, (byte)_note.getByteOne(), 0x00);
        //    _off.setAbsTimestamp(timeStamp = 100);
        //    midiTrack.AddNote(_note);
        //    midiTrack.AddNote(_off);
        //}
        MidiPlayer.resetMidiEventClock();
        MidiMessage mes;
        MidiMessage _off;
        int timestamp = 0;

        for(int i = 0; i < 5; i++)
        {
            mes = matrix.getNextNote();
            mes.setAbsTimestamp(timestamp);
            _off = new MidiMessage(0x80, (byte)mes.getByteOne(), 0x00);
            _off.setAbsTimestamp(timestamp + 250);

            MidiPlayer.PlayNext(mes);
            MidiPlayer.PlayNext(_off);
            timestamp += 500;
        }
        


        //MidiPlayer.PlayTrack(midiTrack);

        //MidiPlayer.PlayTrack(midiFile.getMidiTrack(1));

        //MidiMessage _on = new MidiMessage(0x90, 0x60, 0x90);
        //MidiMessage _off = new MidiMessage(0x80, 0x60, 0x00);
        //_on.setAbsTimestamp(90);
        //_off.setAbsTimestamp(960);

        //MidiPlayer.PlayNext(_on);
        //MidiPlayer.PlayNext(_off);

        // MidiPlayer.PlayScheduled(_on, 1000.0);
        //MidiPlayer.PlayScheduled(_off, 2000.0);

        //for (int i = 0; i < 20; i++)
        //{
        //    MarkovNote _note = matrix.getNextNote();
        //    MidiMessage onMess = _note;
        //    MidiMessage offMess = new MidiMessage(0x80, (byte)onMess.getByteOne(), 0x00);

        //    onMess.setAbsTimestamp(onTimestamp += 960);
        //    MidiPlayer.PlayNext(onMess);
        //    //Debug.Log("On timestamp: " + onTimestamp + " _note.length = " + _note.length);
        //    offMess.setAbsTimestamp(onTimestamp += 960);
        //    MidiPlayer.PlayNext(offMess);
        //}
    }

	void Update () {
        MidiPlayer.Update();
    }

    private void OnApplicationQuit() { MidiPlayer.Shutdown(); }
}
