using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject paddle;
    float paddleMinY = 8.8f;
    float paddleMaxY = 17.4f;
    public float numSaved = 0;
    public float numMissed = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w") && paddle.transform.position.y < paddleMaxY)
        {
            paddle.transform.position = new Vector3(paddle.transform.position.x,
                                                          paddle.transform.position.y + 0.2f,
                                                          paddle.transform.position.z);
        }
        else if (Input.GetKey("s") && paddle.transform.position.y > paddleMinY)
        {
            paddle.transform.position = new Vector3(paddle.transform.position.x,
                                                          paddle.transform.position.y - 0.2f,
                                                          paddle.transform.position.z);
        }
        else
        { paddle.transform.position = paddle.transform.position; }
    }
}
