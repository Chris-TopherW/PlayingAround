using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cwMidi;

namespace cwMarkov
{
    public class MarkovSortedTrack
    {
        public List<MarkovNote> notesWithLengths;
        public MarkovSortedTrack(MidiTrack p_track)
        {
            notesWithLengths = new List<MarkovNote>();
            for (int i = 0; i < p_track.getNumNotes(); i++)
            {
                //if note on
                if (p_track.getNote(i).getStatusByte() == 0x90)
                {
                    MarkovNote _note = new MarkovNote(p_track.getNote(i));
                    int ppqSinceNoteOn = 0;
                    //find coupled note off
                    for (int j = i + 1; j < p_track.getNumNotes(); j++)
                    {
                        ppqSinceNoteOn += p_track.getNote(j).getPPQ();
                        if (p_track.getNote(j).getByteOne() == _note.getByteOne() /* is same note */ &&
                            (p_track.getNote(j).getByteTwo() == 0x00 || p_track.getNote(j).getStatusByte() == 0x80) /* is note off */)
                        {
                            _note.length = ppqSinceNoteOn;
                            j = p_track.getNumNotes(); //force break for loop
                        }
                    }
                    if (_note.length == -1) Debug.Log("<color=red>Error- no note off</color>");
                    notesWithLengths.Add(_note);
                }
            }
        }
    }

    //why is this a class?
    //public class PlayScheduledMarkov
    //{
    //    public PlayScheduledMarkov(MarkovNote _note)
    //    {
    //        MidiPlayer.PlayScheduled(_note);
    //        MidiMessage _noteOff = new MidiMessage(0x80, (byte)_note.getByteOne(), (byte)_note.getByteTwo());
    //        _noteOff.setTimestamp(_note.length);
    //        //this will make que out of order!! , is this a problem? - yes, it will also change ppq track position...
    //        MidiPlayer.PlayScheduled(_noteOff);
    //    }
    //}
}
