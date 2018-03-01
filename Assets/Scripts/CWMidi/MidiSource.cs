using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using cwMidi;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MidiSource : MonoBehaviour {

    public TextAsset MidiClip;
    private cwMidi.MidiFile midiFile;
    private MidiTrack midiTrack;
    private long trackPPQAbsolutePos = 0;

    public bool Mute;
    public bool PlayOnAwake;
    public bool Loop;
    public bool ForceToChannel;
    public int Channel = 1;

    [HideInInspector]
    public double startTimeOffset = 0.0;

    public float volume;

    [Range(-127, 127)]
    public int PitchOffset;

    private bool tempHasPlayed = false;

    public string midiInput;
    public string midiOutput;

    public MyPlayerEditor editor;

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
        startTimeOffset = AudioSettings.dspTime * 1000; /*- MidiPlayer.getMetronomeStartTime()*/ //why the metronome offset?
        //Debug.Log("Offset: " + startTimeOffset);
        //Debug.Log("start time in ms: " + MidiPlayer.getMetronomeStartTime());
        MidiPlayer.PlayTrack(midiFile.getMidiTrack(1), this);
        MidiPlayer.reorderQueue();
    }

    private void Update()
    {
        if(Loop)
        if (AudioSettings.dspTime * 1000 > startTimeOffset + Metronome.ppqToMs(midiFile.getMidiTrack(1).getTrackPPQLen()))
        {
            startTimeOffset += Metronome.ppqToMs(midiFile.getMidiTrack(1).getTrackPPQLen());
            Play();
        }
    }

    public long getTrackPPQAbsolutePos()
    {
        return trackPPQAbsolutePos;
    }

    public void setTrackPPQAbsolutePos(long p_pos)
    {
        trackPPQAbsolutePos = p_pos;
    }
}

// Custom Editor using SerializedProperties.
// Automatic handling of multi-object editing, undo, and prefab overrides.
[CustomEditor(typeof(MidiSource))]
[CanEditMultipleObjects]
public class MyPlayerEditor : Editor
{
    static string deviceNamePlaceholder;
    SerializedProperty midiInputProp;
    string[] midiInputChoices = new[] { "Up", "Down", "Left", "Blah" };
    int midiInputIndex = 0;

    SerializedProperty midiOutputProp;
    string[] midiOutputChoices = new[] { "Up", "Down", "Left", "Right" };
    int midiOutputIndex = 0;

    MidiSource script;

    public int channel;


    void OnEnable()
    {
        deviceNamePlaceholder = PortMidi.getDeviceName(0);
        script = (MidiSource)target;
        script.editor = this;

        midiInputProp = serializedObject.FindProperty("midiInput");
        midiInputIndex = Array.IndexOf(midiInputChoices, midiInputProp.stringValue);

        midiOutputProp = serializedObject.FindProperty("midiOutput");
        midiOutputIndex = Array.IndexOf(midiOutputChoices, midiOutputProp.stringValue);

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        
        midiInputIndex = EditorGUILayout.Popup("Midi Input", midiInputIndex, midiInputChoices);
        if (midiInputIndex < 0)
            midiInputIndex = 0;
        midiInputProp.stringValue = midiInputChoices[midiInputIndex];

        midiOutputIndex = EditorGUILayout.Popup("Midi Output", midiOutputIndex, midiOutputChoices);
        if (midiOutputIndex < 0)
            midiOutputIndex = 0;
        midiOutputProp.stringValue = midiOutputChoices[midiOutputIndex];

        script.Mute = EditorGUILayout.Toggle("Mute", script.Mute);
        script.PlayOnAwake = EditorGUILayout.Toggle("PlayOnAwake", script.PlayOnAwake);
        script.Loop = EditorGUILayout.Toggle("Loop", script.Loop);
        script.ForceToChannel = EditorGUILayout.Toggle("ForceToChannel", script.ForceToChannel);
        script.Channel = EditorGUILayout.IntField("Channel", script.Channel);
        script.volume = EditorGUILayout.Slider("Volume", script.volume, 0.0f, 1.0f);


        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }
}