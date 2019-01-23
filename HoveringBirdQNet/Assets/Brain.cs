using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Replay
{
    public List<double> states;
    public double reward;

    public Replay(double ypos, double yvel, double r)
    {
        states = new List<double>();
        states.Add(ypos);
        states.Add(yvel);
        reward = r;
    }
}

public class Brain : MonoBehaviour {

    public GameObject bird;

    ANN ann;

    float reward = 0.0f;
    List<Replay> replayMemory = new List<Replay>();
    int mCapacity = 10000;

    float discount = 0.99f;
    float exploreRate = 100.0f;
    float maxExploreRate = 100.0f;
    float minExploreRate = 0.01f;
    float exploreDecay = 0.0001f;
    bool hitWall = false;

    Vector3 birdStartPos;
    int failCount = 0;
    float Speed = 0.5f;


    float timer = 0;
    float maxBalanceTime = 0;

	// Use this for initialization
	void Start () {
        ann = new ANN(2, 2, 1, 4, 0.2f);
        birdStartPos = this.transform.position;
        Time.timeScale = 5.0f;
	}

    GUIStyle guiStyle = new GUIStyle();
    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 600, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 500, 30), "Fails:" + failCount, guiStyle);
        GUI.Label(new Rect(10, 45, 500, 30), "Decay Rate:" + exploreRate, guiStyle);
        GUI.Label(new Rect(10, 65, 500, 30), "Last Best Balance:" + maxBalanceTime, guiStyle);
        GUI.Label(new Rect(10, 85, 500, 30), "This Balance:" + timer, guiStyle);
        GUI.EndGroup();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "wall")
        {
            hitWall = true;
            ReserBird();
        }
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        List<double> states = new List<double>();
        List<double> qs = new List<double>();

        states.Add(this.transform.rotation.x);
        states.Add(bird.transform.position.z);
        states.Add(bird.GetComponent<Rigidbody>().angularVelocity.x);

        qs = SoftMax(ann.CalcOutput(states));
        double maxQ = qs.Max();
        int maxQIndex = qs.ToList().IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

        if (Random.Range(0, 10000) < exploreRate)
            maxQIndex = Random.Range(0, 2);

        if (maxQIndex == 0)
            this.GetComponent<Rigidbody2D>().velocity.y += Speed * (float)qs[maxQIndex];
        else if (maxQIndex == 1)
            this.transform.Rotate(0, 0.0001f * -Speed * (float)qs[maxQIndex], 0);

        if (hitWall)
            reward = -5.0f;
        else
            reward = 0.1f;

        Replay lastMemory = new Replay(this.transform.position.y,
                                        bird.GetComponent<Rigidbody2D>().velocity.y,
                                        reward);

        if (replayMemory.Count > mCapacity)
            replayMemory.RemoveAt(0);

        replayMemory.Add(lastMemory);

        if(hitWall)
        {
            for (int i = replayMemory.Count - 1; i >= 0; i--)
            {
                List<double> toutputOld = new List<double>();
                List<double> toutputNew = new List<double>();
                toutputOld = SoftMax(ann.CalcOutput(replayMemory[i].states));

                double maxQOld = toutputOld.Max();
                int action = toutputOld.ToList().IndexOf(maxQOld);

                double feedback;
                if (i == replayMemory.Count - 1 || replayMemory[i].reward == -1)
                    feedback = replayMemory[i].reward;
                else
                {
                    toutputNew = SoftMax(ann.CalcOutput(replayMemory[i + 1].states));
                    maxQ = toutputNew.Max();
                    feedback = (replayMemory[i].reward * discount * maxQ);
                }

                toutputOld[action] = feedback;
                ann.Train(replayMemory[i].states, toutputOld);
            }

            if(timer > maxBalanceTime)
            {
                maxBalanceTime = timer;
            }

            timer = 0;

            hitWall = false;
            this.transform.rotation = Quaternion.identity;
            ReserBird();
            replayMemory.Clear();
            failCount++;
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("space"))
            ReserBird();
	}
    void ReserBird()
    {
        bird.transform.position = birdStartPos;
        bird.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        bird.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
    }

    List<double> SoftMax(List<double> values)
    {
        double max = values.Max();

        float scale = 0.0f;
        for (int i = 0; i < values.Count; ++i)
            scale += Mathf.Exp((float)(values[i] - max));

        List<double> result = new List<double>();
        for (int i = 0; i < values.Count; ++i)
            result.Add(Mathf.Exp((float)(values[i] - max)) / scale);

        return result;
    }
}
