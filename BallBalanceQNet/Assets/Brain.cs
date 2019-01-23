using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Replay
{
    public List<double> states;
    public double reward;

    public Replay(double xr, double ballz, double ballvx, double r)
    {
        states = new List<double>();
        states.Add(xr);
        states.Add(ballz);
        states.Add(ballvx);
        reward = r;
    }
}

public class Brain : MonoBehaviour {

    public GameObject ball;

    ANN ann;

    float reward = 0.0f;
    List<Replay> replayMemory = new List<Replay>();
    int mCapacity = 10000;

    float discount = 0.99f;
    float exploreRate = 100.0f;
    float maxExploreRate = 100.0f;
    float minExploreRate = 0.01f;
    float exploreDecay = 0.0001f;

    Vector3 ballStartPos;
    int failCount = 0;
    float tiltSpeed = 0.5f;


    float timer = 0;
    float maxBalanceTime = 0;

	// Use this for initialization
	void Start () {
        ann = new ANN(3, 2, 1, 6, 0.2f);
        ballStartPos = ball.transform.position;
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

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        List<double> states = new List<double>();
        List<double> qs = new List<double>();

        states.Add(this.transform.rotation.x);
        states.Add(ball.transform.position.z);
        states.Add(ball.GetComponent<Rigidbody>().angularVelocity.x);

        qs = SoftMax(ann.CalcOutput(states));
        double maxQ = qs.Max();
        int maxQIndex = qs.ToList().IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

        if (Random.Range(0, 10000) < exploreRate)
            maxQIndex = Random.Range(0, 2);

        if (maxQIndex == 0)
            this.transform.Rotate(Vector3.right, tiltSpeed * (float)qs[maxQIndex]);
        else if (maxQIndex == 1)
            this.transform.Rotate(Vector3.right, -tiltSpeed * (float)qs[maxQIndex]);

        if (ball.GetComponent<BallState>().dropped)
            reward = -5.0f;
        else
            reward = 0.1f;

        Replay lastMemory = new Replay(this.transform.rotation.x,
                                        ball.transform.position.z,
                                        ball.GetComponent<Rigidbody>().angularVelocity.x,
                                        reward);

        if (replayMemory.Count > mCapacity)
            replayMemory.RemoveAt(0);

        replayMemory.Add(lastMemory);

        if(ball.GetComponent<BallState>().dropped)
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

            ball.GetComponent<BallState>().dropped = false;
            this.transform.rotation = Quaternion.identity;
            ResetBall();
            replayMemory.Clear();
            failCount++;
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("space"))
            ResetBall();
	}
    void ResetBall()
    {
        ball.transform.position = ballStartPos;
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        ball.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
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
