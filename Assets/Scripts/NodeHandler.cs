using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeHandler: MonoBehaviour
{
    Layer layer;
    public float output;
    public float[] weights;
    public float bias;
    public float error;
    public float errorDelta;
    public Material material;
    public Color color;

    #region 3dEngine

    bool changeColor = false;
    bool isOver;
    private void Awake()
    {
        layer = GetComponentInParent<Layer>();
        material = this.gameObject.GetComponent<Renderer>().material;
    }

    public void initCube()
    {
        layer = GetComponentInParent<Layer>();
        material = this.gameObject.GetComponent<Renderer>().material;
    }
    void OnMouseOver()
    {
        if (isOver) return;
        isOver = true;
        
        this.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
        if (layer != null) layer.MakeConnections(this);
        
    }

    void OnMouseExit()
    {
        this.transform.localScale = new Vector3(1f, 1f, 1f);
        isOver = false;
        
        if (layer != null)layer.RemoveConnections();
        
    }


    public void Update(){
        if(changeColor){
            material.color = color;
            changeColor = false;
        }
    }


    public void UpdateColor(){
        color = new Color(output, output, output, 1);
        changeColor = true;
    }
    #endregion


}
