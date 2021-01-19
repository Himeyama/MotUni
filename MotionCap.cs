using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
// using System.Text.Json;

public class MotionCap : MonoBehaviour{
    public string filePath;
    public int numberOfMakers;
    public float samplingFrequency;
    public GameObject marker;
    float[] posData;
    GameObject markersp;
    GameObject[] markers;
    GameObject timeTextG;
    Text timeText;

    void Start(){
        timeTextG = GameObject.Find("Time");
        timeText = timeTextG.GetComponent<Text>();

        markersp = new GameObject("マーカーたち");
        markers = new GameObject[numberOfMakers];
        for(int i = 0; i < numberOfMakers; i++){
            markers[i] = (GameObject)Instantiate(marker, markersp.transform);
            markers[i].transform.position = Vector3.zero;
            markers[i].name = $"マーカー{i+1}";
        }

        byte[] buf;
        using(FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read)){
            buf = new byte[fs.Length];
            fs.Read(buf, 0, buf.Length);
        }
        posData = new float[buf.Length / 4];
        // Debug.Log(buf.Length);
        for(int i = 0; i < posData.Length; i++)
            posData[i] = BitConverter.ToSingle(buf, i*4);
    }

    Vector3 getPos(int i, int j){
        return new Vector3(
            posData[3*numberOfMakers*i + 3*j] / 1000,
            posData[3*numberOfMakers*i + 3*j + 1] / 1000,
            posData[3*numberOfMakers*i + 3*j + 2] / 1000
        );
    }

    void FixedUpdate(){
        int sample = (int)(Time.time * samplingFrequency);
        sample = sample % (posData.Length / (numberOfMakers * 3));
        float t = sample / samplingFrequency;
        // Debug.Log(t);
        timeText.text = $"Time: {t:F02} s";
        for(int i = 0; i < numberOfMakers; i++){
            markers[i].transform.position = getPos(sample, i);
        }
    }
}
