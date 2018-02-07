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
        Debug.Log("Start @ AudioSettings.dspTime = " + AudioSettings.dspTime);
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
