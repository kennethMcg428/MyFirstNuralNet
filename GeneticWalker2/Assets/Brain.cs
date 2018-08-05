using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour {

    int DNALength = 2;
    public float timeAlive;
    public float timeWalking;
    public DNA dna;
    public GameObject eyes;
    bool alive = true;
    bool seeGround = true;

    public GameObject ethanPrefab;
    GameObject ethan;

    private void OnDestroy()
    {
        Destroy(ethan);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "dead")
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
        dna = new DNA(DNALength, 3);
        timeAlive = 0;
        alive = true;
        ethan = Instantiate(ethanPrefab, this.transform.position, this.transform.rotation);
        ethan.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().target = this.transform;
    }
    private void Update () {
        if (!alive) return;

        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 10, Color.red, 10);
        seeGround = false;
        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10, out hit))
        {
            if(hit.collider.gameObject.tag == "platform")
            {
                seeGround = true;
            }
        }
        timeAlive = PopulationManager.elapsed;

        // read DNA
        float Rotate = 0;
        float Move = 0;
        if(seeGround)
        {
            //make v relative to character and always move forward
            if (dna.GetGene(0) == 0) { Move = 1; timeWalking += 1; }
            else if (dna.GetGene(0) == 1) Rotate = -90;
            else if (dna.GetGene(0) == 2) Rotate = 90;
        }
        else
        {
            if (dna.GetGene(1) == 0) { Move = 1; timeWalking += 1; }
            else if (dna.GetGene(1) == 1) Rotate = -90;
            else if (dna.GetGene(1) == 2) Rotate = 90;
        }

        this.transform.Translate(0, 0, Move * 0.1f);
        this.transform.Rotate(0, Rotate, 0);
	}

    // Use this for initialization
    void Start () {
		
	}
	
	
	
}
