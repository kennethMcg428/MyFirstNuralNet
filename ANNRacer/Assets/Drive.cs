using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Drive : MonoBehaviour {

    public float speed = 50.0F;
    public float rotationSpeed = 100.0F;
    public float visableDistance = 200.0f;
    List<string> colledctedTrainData = new List<string>();
    StreamWriter tdf;

    private void Start()
    {
        string path = Application.dataPath + "/trainingData.txt";
        tdf = File.CreateText(path);
    }

    private void OnApplicationQuit()
    {
        foreach(string td in colledctedTrainData)
        {
            tdf.WriteLine(td);
        }
        tdf.Close();
    }

    float Round(float x)
    {
        return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2.0f;
    }

    void Update()
    {
        float translationInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");
        float translation = Time.deltaTime * speed * translationInput;
        float rotation = Time.deltaTime * rotationSpeed * rotationInput;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        Debug.DrawRay(transform.position, this.transform.forward * visableDistance, Color.red);
        Debug.DrawRay(transform.position, this.transform.right * visableDistance, Color.red);

        //raycasts
        RaycastHit hit;
        float fDist = 0, rDist = 0, lDist = 0, r45Dist = 0, l45Dist = 0;

        //forward
        if(Physics.Raycast(transform.position,this.transform.forward,out hit, visableDistance))
        {
            fDist = 1 - Round(hit.distance / visableDistance);
        }
        //right
        if (Physics.Raycast(transform.position, this.transform.right, out hit, visableDistance))
        {
            rDist = 1 - Round(hit.distance / visableDistance);
        }
        //left
        if (Physics.Raycast(transform.position, -this.transform.right, out hit, visableDistance))
        {
            lDist = 1 - Round(hit.distance / visableDistance);
        }
        //right 45
        if (Physics.Raycast(transform.position,
            Quaternion.AngleAxis(45, Vector3.up) * this.transform.right, out hit, visableDistance))
        {
            r45Dist = 1 - Round(hit.distance / visableDistance);
        }
        //left 45
        if (Physics.Raycast(transform.position,
            Quaternion.AngleAxis(45, Vector3.up) * -this.transform.right, out hit, visableDistance))
        {
            l45Dist = 1 - Round(hit.distance / visableDistance);
        }

        string td = fDist + "," + rDist + "," + lDist + "," + r45Dist + "," 
            + l45Dist + "," + Round(translationInput) + "," + Round(rotationInput);

        if(!colledctedTrainData.Contains(td))
        {
            colledctedTrainData.Add(td);
        }
        
    }
}
