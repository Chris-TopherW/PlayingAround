#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MidiWindow : EditorWindow
{
    public Texture2D noteOffBox;
    public Texture2D noteOnBox;
    public Texture2D noteEndBox;
    public List<List<bool>> gridToggles;

    private bool toggle = true;
    private int midiX = 16;
    private int midiY = 12;
    private bool isNoteEnd = false;
    private float previousWidth = 0;
    private float previousHeight = 0;
    private float editorXOffset = 70.0f;
    private float editorYOffset = 40.0f;

    private List<List<Rect>> gridRects;
    private  GUISkin defaultSkin;
    private GUISkin customSkin;

    [MenuItem ("Window/midiWindow")]
    static void Init ()
    {
        MidiWindow window = (MidiWindow)EditorWindow.GetWindow(typeof(MidiWindow));
        //window.minSize = new Vector2(800, 400);
    }

    private void Awake()
    {
        initialise();
    }

    private void initialise()
    {
        defaultSkin = Resources.Load("defaultSkin") as GUISkin;
        customSkin = Resources.Load("MidiPlayerSkin") as GUISkin;
        noteOffBox = Resources.Load("WhiteSquare") as Texture2D;
        noteOnBox = Resources.Load("BlackSquare") as Texture2D;
        noteEndBox = Resources.Load("SquareEnd") as Texture2D;

        gridToggles = new List<List<bool>>();
        gridRects = new List<List<Rect>>();
        for (int x = 0; x < midiX; x++)
        {
            List<bool> tempListTrigger = new List<bool>();
            List<Rect> tempListRect = new List<Rect>();
            for (int y = 0; y < midiY; y++)
            {
                bool tempBool = false;
                Rect tempRect = new Rect(0, 0, 1, 1);
                tempListTrigger.Add(tempBool);
                tempListRect.Add(tempRect);
            }
            gridToggles.Add(tempListTrigger);
            gridRects.Add(tempListRect);
        }
    }
    
    void OnGUI()
    {
        GUI.skin = customSkin;
        customSkin.toggle.normal.background = noteOffBox;

        if (isNoteEnd)
            customSkin.toggle.onNormal.background = noteEndBox;
        else
            customSkin.toggle.onNormal.background = noteOnBox;

        Rect fullArea = new Rect(0, 0, Screen.width, Screen.height);
        GUI.Box(fullArea, "");
        Rect oneBarRect = new Rect(editorXOffset, editorYOffset, fullArea.width - editorXOffset, fullArea.height - editorYOffset);

        float boxWidth = oneBarRect.width / midiX;
        float boxHeight = 20;
        for (int x = 0; x < midiX; x++)
        {
            for (int y = 0; y < midiY; y++)
            {
                gridRects[x][y] = new Rect(x * boxWidth + editorXOffset, y * boxHeight + editorYOffset, boxWidth, boxHeight);
                gridToggles[x][y] = GUI.Toggle(gridRects[x][y], gridToggles[x][y], "");
            }
        }

        GUI.skin = defaultSkin;
        isNoteEnd = GUI.Toggle(new Rect(0, 0, 20, 20), isNoteEnd, "note end?");

        //GUI.backgroundColor = Color.black;
    }

    private void OnEnable()
    {
        initialise();
        //this stops all settings from disappearing on play
        OnFocus();
    }

    private void OnFocus()
    {
        for (int x = 0; x < midiX; x++)
        {
            for (int y = 0; y < midiY; y++)
            {
                if (EditorPrefs.HasKey("ToggleState_" + x + "_" + y))
                    gridToggles[x][y] = EditorPrefs.GetBool("ToggleState_" + x + "_" + y);
            }
        }
        if (EditorPrefs.HasKey("IsNoteEnd"))
            isNoteEnd = EditorPrefs.GetBool("IsNoteEnd");
    }

    private void OnLostFocus()
    {
        for (int x = 0; x < midiX; x++)
        {
            for (int y = 0; y < midiY; y++)
            { 
                EditorPrefs.SetBool("ToggleState_" + x + "_" + y, gridToggles[x][y]);
            }
        }
        EditorPrefs.SetBool("IsNoteEnd", isNoteEnd);
    }

    private void OnDestroy()
    {
        for (int x = 0; x < midiX; x++)
        {
            for (int y = 0; y < midiY; y++)
            {
                if (EditorPrefs.HasKey("ToggleState_" + x + "_" + y))
                    gridToggles[x][y] = EditorPrefs.GetBool("ToggleState_" + x + "_" + y);
            }
        }
        if (EditorPrefs.HasKey("IsNoteEnd"))
            isNoteEnd = EditorPrefs.GetBool("IsNoteEnd");
    }
}

#endif //UNITY_EDITOR
