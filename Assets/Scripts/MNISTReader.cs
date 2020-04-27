using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MNISTReader : MonoBehaviour
{
    public float[] values;
    public float[][] trainingData;
    public float[] trainingLables;
    public float[][] testingData;
    public float[] testingLables;

    

    public void Awake(){

        trainingLables = new float[60000];
        trainingData = new float[60000][];

        testingLables = new float[10000];
        testingData = new float[10000][];

        for(int i = 0; i < trainingData.Length; i++)
            trainingData[i] = new float[28*28];

        for(int i = 0; i < testingData.Length; i++)
            testingData[i] = new float[28*28];

        LoadData("train-labels-idx1-ubyte","train-images-idx3-ubyte", trainingData, trainingLables);
        LoadData("t10k-labels-idx1-ubyte", "t10k-images-idx3-ubyte", testingData, testingLables);

    }

    IEnumerator ReadFileAsync(string fileLable, string fileData, float[][] data, float[] lables)
    {

        UnityWebRequest request = UnityWebRequest.Get(fileLable);
        yield return request.SendWebRequest();

         if(request.isNetworkError || request.isHttpError) {
            Debug.Log(request.error);
        }

        UnityWebRequest request2 = UnityWebRequest.Get(fileData);
        yield return request2.SendWebRequest();
      
        var a = request.downloadHandler.data;
        var b = request2.downloadHandler.data;
        
        MemoryStream ms = new MemoryStream(a);
        MemoryStream ms2 = new MemoryStream(b);

        BinaryReader bL = new BinaryReader(ms);
        BinaryReader bI = new BinaryReader(ms2);

        int magic1 = bI.ReadInt32();
        int numImages = bI.ReadInt32(); 
        int numRows = bI.ReadInt32(); 
        int numCols = bI.ReadInt32(); 

        int magic2 = bL.ReadInt32(); 
        int numLabels = bL.ReadInt32(); 

        byte[][] pixels = new byte[28][];
        for (int i = 0; i < pixels.Length; ++i)
          pixels[i] = new byte[28];

        for(int i = 0; i < lables.Length; ++i){
            for(int j = 0; j < 28; ++j){
                for(int k = 0; k < 28; ++k){
                    byte bq = bI.ReadByte();
                    pixels[j][k] = bq;
                    data[i][(28*j)+k] = pixels[j][k];
                }
            }
            byte lbl = bL.ReadByte();
            lables[i] = lbl;
        }

        
    }

    void ReadFile(string fileLable, string fileData, float[][] data, float[] lables)
    {

        FileStream ms = new FileStream(fileLable, FileMode.Open);
        FileStream ms2 = new FileStream(fileData, FileMode.Open);

        BinaryReader bL = new BinaryReader(ms);
        BinaryReader bI = new BinaryReader(ms2);

        int magic1 = bI.ReadInt32();
        int numImages = bI.ReadInt32(); 
        int numRows = bI.ReadInt32(); 
        int numCols = bI.ReadInt32(); 

        int magic2 = bL.ReadInt32(); 
        int numLabels = bL.ReadInt32(); 

        byte[][] pixels = new byte[28][];
        for (int i = 0; i < pixels.Length; ++i)
          pixels[i] = new byte[28];

        for(int i = 0; i < lables.Length; ++i){
            for(int j = 0; j < 28; ++j){
                for(int k = 0; k < 28; ++k){
                    byte bq = bI.ReadByte();
                    pixels[j][k] = bq;
                    data[i][(28*j)+k] = pixels[j][k];
                }
            }
            byte lbl = bL.ReadByte();
            lables[i] = lbl;
        }

        
    }

    void LoadData(string fileLable, string fileData, float[][] data, float[] lables){



        var string1 = Path.Combine(Application.streamingAssetsPath, fileLable);
        var string2 = Path.Combine(Application.streamingAssetsPath, fileData);

        //var test = Resources.Load<TextAsset>(fileLable);

        ReadFile(string1, string2, data, lables);


        //StartCoroutine(ReadFileAsync(string1, string2, data, lables));

        

    }



}
