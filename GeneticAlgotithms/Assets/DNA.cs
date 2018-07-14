using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA : MonoBehaviour {

    public Vector3 ColorGene;
    public float r;
    public float g;
    public float b;
    bool Dead = false;
    public float timeToDie;
    SpriteRenderer sRenderer;
    Collider2D sCollider;

    // Use this for initialization
    void Start () {
        sRenderer = GetComponent<SpriteRenderer>();
        sCollider = GetComponent<Collider2D>();
        sRenderer.color = new Color(ColorGene.x,ColorGene.y,ColorGene.z);
	}

    private void OnMouseDown()
    {
        Dead = true;
        timeToDie = PopulationManager.elasped;
        Debug.Log("Dead at " + timeToDie);
        sRenderer.enabled = false;
        sCollider.enabled = false;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
