using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Layer : MonoBehaviour
{
  
    public NodeHandler[] InputNodes;
    public NodeHandler[] CurrentNodes;
    public Transform prevLayer;
    public Color startColor;
    public Color endColor;

    public float LearningRate = 0.01f;

    public Monitor monitor;


    public void Initialize(Monitor monitor)
    {

        int size = transform.childCount;
        CurrentNodes = new NodeHandler[size];
        int ind = 0;

        foreach(Transform child in transform){
            CurrentNodes[ind] = child.GetComponent<NodeHandler>();
            CurrentNodes[ind].initCube();
            ind++;
        }

        size = prevLayer.childCount;
        InputNodes = new NodeHandler[size];
        ind = 0;

        foreach(Transform child in prevLayer){
            InputNodes[ind] = child.GetComponent<NodeHandler>();
            //InputNodes[ind].initCube();
            ind++;
        }

        this.monitor = monitor;
        foreach(var node in CurrentNodes)
        {
            node.weights = new float[InputNodes.Length];
          
            for(int i = 0; i < node.weights.Length; i++)
            {
                node.weights[i] = UnityEngine.Random.Range(-1f, 1.0f);
            }

            node.bias = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    public NodeHandler[] FeedForward(NodeHandler[] InputNodes)
    {
        for (int i = 0; i < CurrentNodes.Length; i++)
        {
            CurrentNodes[i].output = CurrentNodes[i].bias;
            for (int j = 0; j < InputNodes.Length; j++)
            {
                CurrentNodes[i].output += InputNodes[j].output *
                                        CurrentNodes[i].weights[j];
                
            }
            CurrentNodes[i].output = sigmoid(CurrentNodes[i].output);

        }

        return CurrentNodes;
    }


    public void BackPropOutput(float[] expected)
    {
        for(int i = 0; i < CurrentNodes.Length; i++)
        {
            CurrentNodes[i].error = expected[i] - CurrentNodes[i].output;
            CurrentNodes[i].errorDelta = CurrentNodes[i].error *
                                    sigmoidDelta(CurrentNodes[i].output);
        }
    }


    public void BackPropHidden(NodeHandler[] forward)
    {
        for(int i = 0; i < CurrentNodes.Length; i++) {

            CurrentNodes[i].error = 0;

            for(int j = 0; j < forward.Length; j++)
            {
                CurrentNodes[i].error += forward[j].errorDelta
                                            * forward[j].weights[i];
            }

             CurrentNodes[i].errorDelta = CurrentNodes[i].error *
                                    sigmoidDelta(CurrentNodes[i].output);
            
        }
    }


    public void UpdateWeightsBias()
    {
        for(int i = 0; i < CurrentNodes.Length; i++)
        {
            for (int j = 0; j < InputNodes.Length; j++)
            {
                CurrentNodes[i].weights[j] += (CurrentNodes[i].errorDelta * monitor.LearningRate * InputNodes[j].output);
            }

            CurrentNodes[i].bias += monitor.LearningRate * CurrentNodes[i].errorDelta;
            
        }
    }



    public void MakeConnections(NodeHandler node)
    {
        float min = 0f;
        float max = 0f;

        int index = 0;

        if(monitor == null) return;
        monitor.UpdateTextNode(node);

        Vector3 offset = new Vector3(0,.5f,0);
        foreach(var input in InputNodes)
        {
            var i = input.gameObject.GetComponent<LineRenderer>();
            
            min = Mathf.Min(min, node.weights[index]);
            max = Mathf.Max(max, node.weights[index]);

            i.SetPosition(0, input.transform.position + offset);
            i.SetPosition(1, node.transform.position - offset);

            index++;
        }

        for(int j = 0; j < node.weights.Length; j++){
            float w = node.weights[j];

            var i = InputNodes[j].gameObject.GetComponent<LineRenderer>();

            var wc = map(w, min, max, 0.001f, 1f);
            w = map(w, min, max, 0.055f, .07f);

            var color = Color.Lerp(startColor, endColor, wc);

            i.startColor = color;
            i.endColor = color;

            i.startWidth = w;
            i.endWidth = w;
        }
    }
    public void RemoveConnections()
    {
       
        foreach (var input in InputNodes)
        {
            if(input == null) return;
            var i = input.gameObject.GetComponent<LineRenderer>();
            
            i.startWidth = .07f;
            i.endWidth = .07f;

            i.SetPosition(0, input.transform.position);
            i.SetPosition(1, input.transform.position);
        }
    }
    public float sigmoid(float x)
    {
        return (1f / (1f + Mathf.Exp(-x)));
    }

    public float sigmoidDelta(float x)
    {
        return sigmoid(x) * (1f - sigmoid(x));
    }

    private float map(float value, float fromLow, float fromHigh, float toLow, float toHigh) 
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }


    public void UpdateColors(){
         foreach(var i in CurrentNodes){
            i.UpdateColor();
        }
    }
}
