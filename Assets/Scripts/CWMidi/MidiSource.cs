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
    private bool hasPlayed = false;

    public double startTimeOffset = 0.0;

    [Range(0.0f, 1.0f)]
    public float Volume = 1.0f;

    [Range(-127, 127)]
    public int PitchOffset;

    public int bpm = 120;
    private int previousBpm;

    private void Awake()
    {
        Metronome.setBPM(bpm);
        previousBpm = bpm;
        midiFile = new cwMidi.MidiFile(MidiClip);
        midiTrack = new MidiTrack();
        if (Midi.debugLevel > 3) midiFile.printCookedMidiFile();
    }

    void Start () {

        if(PlayOnAwake) Play(); 

    }

    private void Update()
    {
        if(bpm != previousBpm)
        {
            Metronome.setBPM(bpm);
            previousBpm = bpm;
        }

        if(!hasPlayed && Time.time > 3.0f)
        {
            Play();
            hasPlayed = true;
        }
    }

    private void Play()
    {
        startTimeOffset = AudioSettings.dspTime;
        MidiPlayer.PlayTrack(midiFile.getMidiTrack(1), this);
    }
}
