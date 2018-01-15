using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cwMidi;
using System.Linq;

namespace cwMarkov
{
    public class TransitionNode
    {
        public MarkovNote message;
        private List<TransitionWeight> transitionWeights;
        private int allTransitionsWeight = 0;

        public TransitionNode(MarkovNote p_mes)
        {
            message = p_mes;
            transitionWeights = new List<TransitionWeight>();
        }
        public void addWeight(TransitionNode p_node)
        {
            //Debug.Log("Add weight num : " + transitionWeights.Count + " to node");
            for (int i = 0; i < transitionWeights.Count; i++)
            {
                if (p_node.message.getMessageAsBytes().SequenceEqual(transitionWeights[i].getMessageAsBytes()))
                {
                    transitionWeights[i].weight++; //add to probability if weight node already exists
                    allTransitionsWeight++;
                    return;
                }
            }
            //else create new weighting and attach to exiting node ref
            TransitionWeight connectorWeight = new TransitionWeight(p_node);
            connectorWeight.weight++;
            allTransitionsWeight++;
            transitionWeights.Add(connectorWeight);
        }

        public TransitionNode getNextNote()
        {
            if (transitionWeights.Count == 0)
            {
                Debug.Log("<color=red>Error:</color> no weights in array");
                return null;
            }

            int randomPos = Random.Range(0, allTransitionsWeight);

            int accumulator = 0;
            for (int i = 0; i < transitionWeights.Count; i++)
            {
                if(Midi.debugLevel > 2)
                    Debug.Log("Number of possible transitions: " + transitionWeights.Count
                        + "\t\tNum Weights to chose from: " + allTransitionsWeight + "\nrand num : " + randomPos);

                for (int j = 0; j < transitionWeights[i].weight; j++)
                {
                    if (Midi.debugLevel > 2)
                        Debug.Log("<color=green>transitionWeights[" + i + "].weight = " + transitionWeights[i].weight
                        + "\nMidi mess is : " + transitionWeights[i].node.message.getByteOne().ToString("X") + "</color>");
                    if (randomPos == j + accumulator)
                    {
                        if (Midi.debugLevel > 2)
                            UnityEngine.Debug.Log(
                            "<color=blue>Event = " + transitionWeights[i].node.message.getStatusByte().ToString("X") + "\ncontrol byte 1 = " +
                            transitionWeights[i].node.message.getByteOne().ToString("X") + "\t\tcontrol byte 2 = "
                            + transitionWeights[i].node.message.getByteTwo().ToString("X") + "</color>");

                        return transitionWeights[i].node;
                    }
                }
                accumulator += transitionWeights[i].weight;
            }
            Debug.Log("<color=red>Error, matrix overflow</color>" + "\nrandomPos = " + randomPos + "\t\tAll transitions weight = " + allTransitionsWeight);
            return null;
        }

        public byte[] getMessageAsBytes() { return message.getMessageAsBytes(); }
        public long getNoteLen() { return message.length; }
    }

    public class TransitionWeight
    {
        public TransitionNode node;
        public int weight = 0;

        public TransitionWeight(TransitionNode p_node)
        {
            node = p_node;
        }

        public byte[] getMessageAsBytes()
        {
            return node.message.getMessageAsBytes();
        }
    }
}
