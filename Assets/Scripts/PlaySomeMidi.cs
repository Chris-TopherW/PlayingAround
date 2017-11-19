using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cwMidi;

public class PlaySomeMidi : MonoBehaviour {

    public TextAsset file;
    private cwMidi.MidiFile midiFile;

    private void Awake()
    {
        midiFile = new cwMidi.MidiFile(file);
        midiFile.printCookedMidiFile();
    }

    void Start()
    {
        Debug.Log("Midi output device: " + MidiPlayer.Start());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playRandomNotePhrase();
            //playFile();
            //MidiMessage mes = new MidiMessage(0x90, (byte)(60 + Random.Range(0, 12)), 100);
            //mes.setTimestamp(0);
            //MidiPlayer.PlayScheduled(mes);
        }
    }

    private void OnApplicationQuit()
    {
        MidiPlayer.Shutdown();
    }

    private void playRandomNotePhrase()
    {
        MidiPlayer.resetMidiEventClock(); //this bypasses ppq offset for first note

        MidiMessage mes = new MidiMessage(0x90, (byte)(60 + Random.Range(0, 12)), 100);
        mes.setTimestamp(0);
        MidiPlayer.PlayScheduled(mes);

        for (int i = 0; i < 15; i++)
        {
            mes = new MidiMessage(0x90, (byte)(60 + Random.Range(0, 12)), 100);
            mes.setTimestamp(960);
            MidiPlayer.PlayScheduled(mes);
        }
    }

    private void playFile()
    {
        //this will not work entirely- note offs!
        MidiPlayer.resetMidiEventClock();
        for(int tracks = 0; tracks < midiFile.getNumTracks(); tracks++)
        {
            for (int i = 0; i < midiFile.getMidiTrack(tracks).getNumNotes(); i++)
            {
                MidiMessage mes = midiFile.getMidiTrack(tracks).getNote(i);
                if(mes.getMidiEvent() == 0x90);
                    MidiPlayer.PlayScheduled(mes);
            }
        }
    }
}
