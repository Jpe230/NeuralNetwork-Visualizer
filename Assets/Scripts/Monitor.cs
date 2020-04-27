using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Monitor : MonoBehaviour
{

    public int batchSize = 400;
    public float[] values;
    public float LearningRate = .5f;
    public NeuralNet net;

    public MNISTReader reader;
    public int currentGuess;
    public int guessed;

    int currentBatch = 0;

    public float[][] inputData;
    public float[][] expectedData;

    public ConfigHandler config;
    public NeuronTextHandler textHandler;

    public GameObject ProgressParent;
    public Text progressText;
    public Slider progressSlider;


    public GameObject hideParent;

    public void RestartScene(){
        SceneManager.LoadScene(0);
    }

    public void StartNeuralNetwork()
    {
        net.startNeuralNetwork(this);

        initValues();

        SetValues();
        setSliders();
    }

    public void ChangeBatchSize(Slider slider){
        int i = RoundNum((int)slider.value, 10);
        batchSize = i;
        slider.value = i;
        setSliders();

    }

    public void ChangeLearningRate(Slider slider){
        LearningRate = slider.value;
        setSliders();
    }

    public void SetValues(){
        config.updateValue(currentGuess, guessed);
    }

    public void setSliders(){
       
        config.updateLearningText(LearningRate);
        config.updateBatchText(batchSize);
    }


    int RoundNum(int num, int step)
    {
        if (num >= 0)
            return ((num + (step / 2)) / step) * step;
        else
            return ((num - (step / 2)) / step) * step;
    }

    

    void initValues(){
        inputData = new float[batchSize][];
        expectedData = new float[batchSize][];

        for(int i = 0; i < inputData.Length; i++){
            inputData[i] = new float[28*28];
            expectedData[i] = new float[10];
        }
    }


    public void TrainThread(){
        cBatch = 0;
        currentBatch = 0;
        //startBatch = true;
        hideParent.SetActive(false);
        ProgressParent.SetActive(true);
        StartCoroutine(TrainModel());
    }
    public bool startBatch = false;
    public int cBatch = 0;


    //Main Thread (Use for WebGL)
    /*public void Update(){
        if(startBatch){
            int i = cBatch;
            for(int j = 0; j < batchSize; j++){
                
                int gIndex = i * batchSize + j;

                if(gIndex >= reader.trainingData.Length) break;
                
                if(j >= inputData.Length){
                    Debug.Log(j + ": Max, " + inputData.Length);
                    continue;
                }
                inputData[j] = reader.trainingData[gIndex];

                float[] _m = new float[10];
                int index = (int)reader.trainingLables[gIndex];

                currentGuess = index;

                _m[index] = 1;
                expectedData[j] = _m;
                
                var v = net.FeedForward(inputData[j]);

                GetCurrentGuess(v);

                SetValues();
                net.BackProp(expectedData[j]);
                net.UpdateNetwork();
                net.UpdateColors();

                currentBatch++;
            }
            cBatch++;
            if(cBatch >= 60000/batchSize){
                startBatch = false;
                hideParent.SetActive(true);
            }
        }
    }*/

    public void Update(){
        if(startBatch){
            SetValues();
            startBatch = false;
        }
    }

    public void Train(){
        
        for(int i = 0; i < 60000/batchSize; i++){
           for(int j = 0; j < batchSize; j++){
                
                currentBatch++;
                if((i*batchSize)+j >= reader.trainingData.Length) continue;
                if(j >= inputData.Length) continue;

                inputData[j] = reader.trainingData[(i*batchSize)+j];
                float[] _m = new float[10];
                
                _m[(int)reader.trainingLables[(i*batchSize)+j]] = 1;
                currentGuess = (int)reader.trainingLables[(i*batchSize)+j];
                expectedData[j] = _m;
                
                var v = net.FeedForward(inputData[j]);

                GetCurrentGuess(v);

                net.BackProp(expectedData[j]);
                net.UpdateNetwork();
                net.UpdateColors();
                
                //SetValues();
                startBatch = true;
                
            }
        }
       
    }

    IEnumerator TrainModel()
	{
		
		bool done = false;
		new Thread(() => {
			Train();
            done = true;
		}).Start();
 
		
		while (!done)
		{
            //Debug.Log((currentBatch/600).ToString("0.0") + "%");
            progressText.text = "Progreso: " + (currentBatch/600f).ToString("0.0") + "%";
            progressSlider.value = (currentBatch/600f);
			yield return null;
		}

        net.UpdateColors();
        setSliders();
        hideParent.SetActive(true);
        ProgressParent.SetActive(false);
 
		
	}

    public void Test(){

        int randomInt = Random.Range(1, 10000);
        currentGuess = (int) reader.testingLables[randomInt];
        values = net.FeedForward(reader.testingData[randomInt]);
        GetCurrentGuess(values);
        SetValues();
        net.UpdateColors();


    }


    public void GetCurrentGuess(float[] v){
        float max = 0;
        for(int i = 0; i < 10; i++){
            max = Mathf.Max(max, v[i]);
        }

        for(int i = 0; i < 10; i++){
            if(max == v[i]){
                guessed = i;
            }
        }
    }

    public void UpdateTextNode(NodeHandler node){
        textHandler.SetText(node);
    }
}
