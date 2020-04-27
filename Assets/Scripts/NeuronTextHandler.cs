using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeuronTextHandler : MonoBehaviour
{
    public Text bias;
    public Text error;
    public Text errorDelta;
    public Text output;

    public void SetText(NodeHandler node){
        bias.text = node.bias.ToString("0.00000");
        error.text = node.error.ToString("0.00000");
        errorDelta.text = node.errorDelta.ToString("0.00000");
        output.text = node.output.ToString("0.00000");
    }
}
