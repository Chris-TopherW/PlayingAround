using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cwMidi;

public class PlaySomeMidi : MonoBehaviour {

    public TextAsset file;
    private cwMidi.MidiFile midiFile;
    int midiOutputDevice;

    private void Awake()
    {
        midiFile = new cwMidi.MidiFile(file);
        midiOutputDevice = MidiPlayer.Start();
        //midiFile.printCookedMidiFile();
    }

    void Start()
    {
        Debug.Log("Midi output device: " + midiOutputDevice);
    }

    void Update()
    {
        MidiPlayer.Update();
        if (Input.GetKeyDown(KeyCode.Space))
            MidiPlayer.PlayTrack(midiFile.getMidiTrack(1));
    }

    private void OnApplicationQuit() { MidiPlayer.Shutdown(); }
}
