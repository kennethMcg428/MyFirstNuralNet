using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour {

    Vector3 ballStartPosition;
    Rigidbody2D rb;
    float speed = 400;
    public AudioSource blip;
    public AudioSource blop;
    public int resets = 0;

    // Use this for initialization
    void Start() {
        rb = this.GetComponent<Rigidbody2D>();
        ballStartPosition = this.transform.position;
        ResetBall();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "tops")
            blop.Play();
        else if (other.gameObject.tag == "backwall")
        {
            if (other.gameObject.GetComponent<BackWall>().Gaurd.GetComponent<PlayerController>() != null)
            { other.gameObject.GetComponent<BackWall>().Gaurd.GetComponent<PlayerController>().numMissed += 1; }
            else if(other.gameObject.GetComponent<BackWall>().Gaurd.GetComponent<PlayerController>() == null)
            { other.gameObject.GetComponent<BackWall>().GaurdBrain.GetComponent<Brain>().numMissed += 1; }

            ResetBall();
        }
        else
        {
            if (other.gameObject.tag == "Player")
            {
                if (other.gameObject.GetComponent<PlayerController>() != null)
                    other.gameObject.GetComponent<PlayerController>().numSaved += 1;
                else
                    GameObject.Find("Brain").GetComponent<Brain>().numSaved += 1;
            }
            blip.Play();
        }
    }

    public void ResetBall()
    {
        resets++;
        this.transform.position = ballStartPosition;
        rb.velocity = Vector3.zero;
        Vector3 dir = new Vector3(Random.Range(100, 300), Random.Range(-100, 100), 0).normalized;
        if(resets%2 == 0)
            rb.AddForce(dir * speed);
        else
            rb.AddForce(dir * -speed);
    }

    // Update is called once per frame
    void Update () {
		if(Input.GetKeyDown("space"))
        {
            ResetBall();
        }
	}
}
