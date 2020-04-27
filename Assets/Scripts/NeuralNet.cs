using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNet : MonoBehaviour
{
    public Layer[] layers;
    public NodeHandler[] InputNodes;


    public void startNeuralNetwork(Monitor monitor){

       foreach(var l in layers){
           l.Initialize(monitor);
       }

    }

    public float[] FeedForward(float[] inputs){

        for(int i = 0; i < inputs.Length; i++){
            InputNodes[i].output = inputs[i];
           
        }
        layers[0].FeedForward(InputNodes);

        for(int i = 1; i < layers.Length; i++){
            layers[i].FeedForward(layers[i-1].CurrentNodes);
        }

        List<float> values = new List<float>();

        foreach(var node in layers[layers.Length-1].CurrentNodes){
            values.Add(node.output);
        }

        return values.ToArray();

    }

    public void UpdateColors(){
        foreach(var i in InputNodes){
            i.UpdateColor();
        }

        foreach(var i in layers){
            i.UpdateColors();
        }
    }

    public void BackProp(float[] expected){

        for(int i = layers.Length -1; i >= 0; i--){
            if(i == layers.Length-1)
                layers[i].BackPropOutput(expected);
            else
                layers[i].BackPropHidden(layers[i+1].CurrentNodes);
        }

    }

    public void UpdateNetwork(){

        for(int i = 0; i < layers.Length; i++){
            layers[i].UpdateWeightsBias();
        }
        
    }
}
