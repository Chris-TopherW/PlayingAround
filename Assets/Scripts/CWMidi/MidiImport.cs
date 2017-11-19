using System.IO;
using UnityEngine;
using cwMidi;

public class MidiImport : MonoBehaviour {

    public TextAsset file;

    void Start()
    {
        cwMidi.MidiFile midiFile = new cwMidi.MidiFile(file);
        //midiFile.printRawMidiFile();
        midiFile.printCookedMidiFile();
    }
}
