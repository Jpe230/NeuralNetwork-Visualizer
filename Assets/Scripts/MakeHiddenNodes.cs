using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeHiddenNodes : MonoBehaviour
{

    public Layers[] layers;

    public GameObject cubePrefab;

    [System.Serializable]
    public class Layers{
        public Layer layer;
        public Text uiText;
    }
    

    public void UpdateLayer1(Slider slider){
        UpdateLayer(0, (int)slider.value);
    }

     public void UpdateLayer2(Slider slider){
        UpdateLayer(1, (int)slider.value);
    }


    public void Start(){

        UpdateLayer(0, 30);
        UpdateLayer(1, 30);
        
    }


    public void UpdateLayer(int index, int value){
        layers[index].uiText.text = "Número de Neuronas Capa " + (index+1) + ": " + value;


        foreach(Transform child in layers[index].layer.gameObject.transform){
            Destroy(child.gameObject);
        }


        float size = (1.5f * value) / 2f;

        for(int i = 0 ; i < value; i++){
            var neuron = Instantiate(cubePrefab, layers[index].layer.gameObject.transform);
            neuron.transform.localPosition = new Vector3(i * 1.5f - size, 0, 0);
        }
    }

}
