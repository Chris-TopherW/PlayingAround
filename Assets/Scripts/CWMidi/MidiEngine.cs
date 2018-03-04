using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cwMidi;

public class MidiEngine : MonoBehaviour {

    private int midiOutputDevice;
    public int bpm = 120;
    private int previousBpm;

    void Awake () {
        midiOutputDevice = MidiPlayer.Start();
        Metronome.setBPM(bpm);
        previousBpm = bpm;
    }

    private void Start()
    {
        MidiPlayer.resetMidiEventClock();
    }

    void Update()
    {
        MidiPlayer.Update();

        if (bpm != previousBpm)
        {
            Metronome.setBPM(bpm);
            previousBpm = bpm;
        }
    }

    private void OnApplicationQuit() { MidiPlayer.Shutdown(); }
}
