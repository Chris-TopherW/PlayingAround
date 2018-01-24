using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cwMidi;

public class MidiEngine : MonoBehaviour {

    private int midiOutputDevice;

    void Awake () {
        midiOutputDevice = MidiPlayer.Start();
    }

    void Update()
    {
        MidiPlayer.Update();
    }

    private void OnApplicationQuit() { MidiPlayer.Shutdown(); }
}
