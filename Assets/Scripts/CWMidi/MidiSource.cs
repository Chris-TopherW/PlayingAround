using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cwMidi;

public class MidiSource : MonoBehaviour {

    public TextAsset MidiClip;
    private cwMidi.MidiFile midiFile;
    private MidiTrack midiTrack;
    
    public bool Mute;
    public bool PlayOnAwake;
    public bool Loop;
    public bool ForceToChannel;
    public int Channel;

    [HideInInspector]
    public double startTimeOffset = 0.0;

    [Range(0.0f, 1.0f)]
    public float Volume = 1.0f;

    [Range(-127, 127)]
    public int PitchOffset;

    private void Awake()
    {
        midiFile = new cwMidi.MidiFile(MidiClip);
        midiTrack = new MidiTrack();
        if (Midi.debugLevel > 3) midiFile.printCookedMidiFile();
    }

    void Start () {
        if (Channel < 1 || Channel > 16)
        {
            Debug.Log("<color=red>Error:</color> Channels must be between 1 and 16. Auto set to 1");
            Channel = 1;
        }
            
        if (PlayOnAwake) Play(); 

    }

    private void Play()
    {
        startTimeOffset = AudioSettings.dspTime * 1000 - MidiPlayer.getMetronomeStartTime(); //why the metronome offset?
        Debug.Log("Offset: " + startTimeOffset);
        Debug.Log("start time in ms: " + MidiPlayer.getMetronomeStartTime());
        MidiPlayer.PlayTrack(midiFile.getMidiTrack(1), this);
        MidiPlayer.reorderQueue();
    }

    private void Update()
    {

    }
}
