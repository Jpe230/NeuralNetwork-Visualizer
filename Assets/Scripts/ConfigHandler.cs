using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigHandler : MonoBehaviour
{
   public Text currentValue;
   public Text guessedValue;

   public Text LearningText;
   public Text BatchText;


   public void updateValue(int cValue, int gValue){
       currentValue.text = cValue.ToString();
       guessedValue.text = gValue.ToString();
   }


   public void updateLearningText(float value){
       LearningText.text = "Learning Rate: " + value.ToString("#.###");
   }


    public void updateBatchText(int value){
       BatchText.text = "Tamaño Batch: " + value.ToString();
   }
}
