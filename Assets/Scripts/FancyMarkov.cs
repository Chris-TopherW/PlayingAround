using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cwMarkov;
using cwMidi;

public class FancyMarkov : MonoBehaviour {

    private TransitionMatrix matrix;
    public TextAsset file;
    private MidiFile midiFile;
    private MidiTrack newTrack;

    // Use this for initialization
    void Start () {

        newTrack = new MidiTrack();
        midiFile = new MidiFile(file);
        matrix = new TransitionMatrix(midiFile.getMidiTrack(1));

        //here I will loop through and generate randomised notes
        MidiPlayer.resetMidiEventClock();
        for (int i = 0; i < 20; i++)
        {
            //MidiMessage _mes = new MidiMessage(0x90, (byte)(0x48 + i), 0x48);
            MarkovNote _note = matrix.getNextNote();
            MidiMessage onMess = _note;
            MidiMessage offMess = new MidiMessage(0x80, (byte)onMess.getByteOne(), 0x00);
            //MidiMessage _mes = new MidiMessage(0x90, 0x48, 0x48);

            int onTimestamp = 960;
            onMess.setTimestamp(onTimestamp);
            offMess.setTimestamp(_note.length);
            MidiPlayer.PlayNext(onMess);
            MidiPlayer.PlayNext(offMess);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
