using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cwMidi;
using System.Linq;

namespace cwMarkov
{
    public class TransitionMatrix
    {
        private List<TransitionNode> nodeMatrix;
        private int previousWritePos = 0; //in nodeMatrix vector
        private TransitionNode previousReadNode = null;
        private MarkovSortedTrack sortedTrack;

        public TransitionMatrix(MidiTrack _track)
        {
            nodeMatrix = new List<TransitionNode>();
            sortedTrack = new MarkovSortedTrack(_track);
            TransitionNode startNode = new TransitionNode(new MarkovNote(0x00, 0x00, 0x00));
            nodeMatrix.Add(startNode);
            previousReadNode = nodeMatrix[0];
            previousWritePos = 0; //pos 0 in array- ie start of phrase
        }
        //each node- check it doesn't exist already
        public void addNote(MarkovNote p_mes)
        {
            //Debug.Log("Add note : " + p_mes.getByteOne());
            for (int i = 0; i < nodeMatrix.Count; i++)
            {
                if (p_mes.getMessageAsBytes().SequenceEqual(nodeMatrix[i].getMessageAsBytes()) && p_mes.length == nodeMatrix[i].getNoteLen())
                {
                    nodeMatrix[previousWritePos].addWeight(nodeMatrix[i]);
                    previousWritePos = i;
                    return;
                }
            }
            TransitionNode node = new TransitionNode(p_mes);
            nodeMatrix[previousWritePos].addWeight(node);
            nodeMatrix.Add(node);
            previousWritePos = nodeMatrix.Count - 1;
        }

        //analyze transition matrix and generate notes
        //how to deal with note offs? - add to an array?
        public MarkovNote getNextNote()
        {
            TransitionNode _node;
            _node = previousReadNode.getNextNote();
            if (_node != null)
            {
                previousReadNode = _node;
                return _node.message;
            }
            else
            {
                Debug.Log("<color=red>Error:</color> non next note");
                return null;
            }
        }
    }
}
