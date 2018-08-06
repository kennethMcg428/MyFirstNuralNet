using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{

    int DNALength = 2;
    public float timeAlive;
    public float timeWalking;
    public DNA dna;
    public GameObject eyes;
    bool alive = true;
    bool seeGround = true;
    bool seeWall = true;
    public float distanceTraveled = 0;
    private Vector3 startingPosition;
    // public GameObject ethanPrefab;
    //  GameObject ethan;

    private void Start()
    {
        startingPosition = gameObject.transform.position;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "dead")
        {
            alive = false;
        }
    }

    public void Init()
    {
        //initialize DNA
        //0 forward
        //1 left
        //2 right
        dna = new DNA(DNALength, 360);
        timeAlive = 0;
        alive = true;
    }
    private void Update()
    {
        if (!alive) return;
        UpdateDistanceTraveled();
        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 0.5f, Color.red, 10);
        seeWall = false;
        seeGround = false;
        RaycastHit hit;
        if (Physics.SphereCast(eyes.transform.position,0.1f, eyes.transform.forward, out hit, 0.5f))
        {
            if(hit.collider.gameObject.tag == "wall")
            {
                seeWall = true;
            }
        }
        timeAlive = PopulationManager.elapsed;

        // read DNA
        float Rotate = 0;
        float Move = dna.GetGene(0);

        if (seeWall)
        {
            Rotate = dna.GetGene(1);
        }
       
        this.transform.Translate(0, 0, Move * 0.0005f);
        this.transform.Rotate(0, Rotate, 0);
    }

    private void UpdateDistanceTraveled()
    {
        distanceTraveled = (startingPosition - gameObject.transform.position).magnitude;
    }

}
