using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainingSet
{
    public double[] input;
    public double output;
}

public class Perceptron : MonoBehaviour {

    public TrainingSet[] ts;
    double[] weights = { 0, 0 };
    double bias = 0;
    double totalError = 0;

    double DotProductBias(double[] v1, double[] v2)
    {
        if (v1 == null || v2 == null)
            return -1;

        if (v1.Length != v2.Length)
            return -1;
        
        double d = 0;
        for(int x = 0; x < v1.Length; x ++)
        {
            d += v1[x] * v2[x];
        }

        d += bias;

        return d;
    }

    double CalcOutput(int i)
    {
        double dp = DotProductBias(weights, ts[i].input);
        if (dp > 0) return (1);
        return (0);
    }

    void InitializeWeights()
    {
        for(int i =0; i < weights.Length; i++)
        {
            weights[i] = Random.Range(-1.0f, 1.0f);
        }
        bias = Random.Range(-1.0f, 1.0f);
    }

    void UpdateWeights(int j)
    {
        double error = ts[j].output - CalcOutput(j);
        totalError += Mathf.Abs((float)error);
        for(int i = 0; i < weights.Length; i++)
        {
            weights[i] = weights[i] + error * ts[j].input[i];
        }
        bias += error;
    }

    double CalcOutput(double i1, double i2)
    {
        double[] inp = new double[] { i1, i2 };
        double dp = DotProductBias(weights, inp);
        if (dp > 0) return (1);
        return (0);
    }

    void Train(int epochs)
    {
        InitializeWeights();

        for(int e = 0; e < epochs; e++)
        {
            totalError = 0;
            for(int t = 0; t < ts.Length; t++)
            {
                UpdateWeights(t);
                Debug.Log("W1: " + (weights[0] + " W2: " + (weights[1]) + " B: " + bias));
            }
            Debug.Log("TOTAL ERRORS: " + totalError);
        }
    }

	// Use this for initialization
	void Start () {
        Train(8);
        Debug.Log("input1: 0 input2: 0 output: " + CalcOutput(0, 0));
        Debug.Log("input1: 1 input2: 0 output: " + CalcOutput(1, 0));
        Debug.Log("input1: 0 input2: 1 output: " + CalcOutput(0, 1));
        Debug.Log("input1: 1 input2: 1 output: " + CalcOutput(1, 1));
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
