using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{

    int DNALength = 5;
    public DNA dna;
    public GameObject eyes;
    bool seeUpWall = false;
    bool seeDownWall = false;
    bool seeTop = false;
    bool seeBottom = false;
    bool alive = true;
    private Vector3 startingPosition;
    public float distanceTraveled = 0;
    public float timeAlive;
    public int crash;
    Rigidbody2D rb;
    // public GameObject ethanPrefab;
    //  GameObject ethan;

    private void Start()
    {
        
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
        //1 upWall
        //2 downWall
        //3 normal upward
        dna = new DNA(DNALength, 200);
        this.transform.Translate(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
        rb = this.GetComponent<Rigidbody2D>();
        timeAlive = 0;
        alive = true;
        startingPosition = this.transform.position;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
       
        if(other.gameObject.tag == "dead")
        {
            alive = false;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.tag == "top"||
            other.gameObject.tag == "bottom" ||
            other.gameObject.tag == "upWall" ||
            other.gameObject.tag == "downWall")
        {
            crash++;
        }
    }

    private void Update()
    {
        if (!alive) return;

        seeUpWall = false;
        seeDownWall = false;
        seeTop = false;
        seeBottom = false;
        RaycastHit2D hit = Physics2D.Raycast(eyes.transform.position, eyes.transform.forward, 1.0f);

        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 1.0f, Color.red);
        Debug.DrawRay(eyes.transform.position, eyes.transform.up * 1.0f, Color.red);
        Debug.DrawRay(eyes.transform.position, -eyes.transform.up * 1.0f, Color.red);

        if(hit.collider!= null)
        {
            if(hit.collider.gameObject.tag == "upWall")
            {
                seeUpWall = true;
            }
            else if(hit.collider.gameObject.tag == "downWall")
            {
                seeDownWall = true;
            }
        }
        hit = Physics2D.Raycast(eyes.transform.position, eyes.transform.up, 1.0f);
        
        if(hit.collider!= null)
        {
            if(hit.collider.gameObject.tag == "top")
            {
                seeTop = true;
            }

        }
        hit = Physics2D.Raycast(eyes.transform.position, -eyes.transform.up, 1.0f);

        if(hit.collider!=null)
        {
            if(hit.collider.gameObject.tag == "bottom")
            {
                seeBottom = true;
            }
        }
        timeAlive = PopulationManager.elapsed;
    }

    private void FixedUpdate()
    {
        if (!alive) return;

        //read DNA
        float upForce = 0;
        float forwardForce = 1.0f;

        if(seeUpWall)
        {
            upForce = dna.GetGene(0);
        }
        else if (seeDownWall)
        {
            upForce = dna.GetGene(1);
        }
        else if (seeTop)
        {
            upForce = dna.GetGene(2);
        }
        else if (seeBottom)
        {
            upForce = dna.GetGene(3);
        }
        else
        {
            upForce = dna.GetGene(4);
        }

        rb.AddForce(this.transform.right * forwardForce);
        rb.AddForce(this.transform.up * upForce * 0.1f);
        distanceTraveled = Vector3.Distance(startingPosition, this.transform.position);
    }

}
