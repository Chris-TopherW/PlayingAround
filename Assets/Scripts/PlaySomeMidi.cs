using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cwMidi;

public class PlaySomeMidi : MonoBehaviour {

    public TextAsset file;
    private cwMidi.MidiFile midiFile;
    private MidiTrack midiTrack;
    int midiOutputDevice;

    private void Awake()
    {
        midiFile = new cwMidi.MidiFile(file);
        midiTrack = new MidiTrack();
        midiOutputDevice = MidiPlayer.Start();
        if(Midi.debugLevel > 3) midiFile.printCookedMidiFile();
    }

    void Start()
    {
        if(Midi.debugLevel > 0) Debug.Log("Midi output device: " + midiOutputDevice);
        MidiPlayer.resetMidiEventClock();
        MidiPlayer.PlayTrack(midiFile.getMidiTrack(1));
       
        //MidiMessage _on = new MidiMessage(0x90, 0x60, 0x90);
        //MidiMessage _off = new MidiMessage(0x80, 0x60, 0x00);
        //_on.setAbsTimestamp(90);
        //_off.setAbsTimestamp(960);

        //MidiPlayer.PlayNext(_on);
        //MidiPlayer.PlayNext(_off);
    }

    void Update()
    {
        MidiPlayer.Update();
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    MidiPlayer.PlayTrack(midiFile.getMidiTrack(1));
        //    if(Midi.debugLevel > 5) Debug.Log("Space");
        //}
            
    }

    private void OnApplicationQuit() { MidiPlayer.Shutdown(); }
}
