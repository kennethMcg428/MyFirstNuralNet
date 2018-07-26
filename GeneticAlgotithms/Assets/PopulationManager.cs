using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager : MonoBehaviour
{

    public GameObject personPrefabl;
    public float populationSize = 10;
    List<GameObject> population = new List<GameObject>();
    List<GameObject> oldPopulation = new List<GameObject>();
    public static float elasped = 0;
    int trialTime = 10;
    int Generation = 1;
    int displaysize;
    GUIStyle guiStyle = new GUIStyle();

    private void OnGUI()
    {
        guiStyle.fontSize = 50;
        guiStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 100, 20), "Generation: " + Generation, guiStyle);
        GUI.Label(new Rect(10, 65, 100, 20), "Trial Time: " + (int)elasped, guiStyle);
        GUI.Label(new Rect(10, 110, 100, 20), "pop size " + displaysize, guiStyle);
    }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-9, 9), Random.Range(-4.5f, 4.5f), 0);
            GameObject currPerson = Instantiate(personPrefabl, pos, Quaternion.identity);
            currPerson.GetComponent<DNA>().ColorGene = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            currPerson.GetComponent<DNA>().scaleGene = Random.Range(.15f, .5f);
            population.Add(currPerson);
        }
       // InvokeRepeating("BreedNewPopulation", trialTime, trialTime);
    }

    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        //initialize offspring
        Vector3 pos = new Vector3(Random.Range(-9, 9), Random.Range(-4.5f, 4.5f), 0);
        GameObject offspring = Instantiate(personPrefabl, pos, Quaternion.identity);
        //parent DNA
        DNA dna1 = parent1.GetComponent<DNA>();
        DNA dna2 = parent2.GetComponent<DNA>();
        //swap DNA
        if (Random.Range(0, 1000) > 5)
        {
            offspring.GetComponent<DNA>().ColorGene.x = Random.Range(0, 10) < 5 ? dna1.ColorGene.x : dna2.ColorGene.x;
            offspring.GetComponent<DNA>().ColorGene.y = Random.Range(0, 10) < 5 ? dna1.ColorGene.y : dna2.ColorGene.y;
            offspring.GetComponent<DNA>().ColorGene.z = Random.Range(0, 10) < 5 ? dna1.ColorGene.z : dna2.ColorGene.z;

            offspring.GetComponent<DNA>().scaleGene = Random.Range(0, 10) < 5 ? dna1.scaleGene : dna2.scaleGene;
        }
        //random mutation
        else
        {
            offspring.GetComponent<DNA>().ColorGene.x = Random.Range(0.0f, 1.0f);
            offspring.GetComponent<DNA>().ColorGene.y = Random.Range(0.0f, 1.0f);
            offspring.GetComponent<DNA>().ColorGene.z = Random.Range(0.0f, 1.0f);

            offspring.GetComponent<DNA>().scaleGene = Random.Range(.05f, .4f);
        }


        return offspring;
    }

    void BreedNewPopulation()
    {
        Generation++;
        elasped = 0.0f;
        oldPopulation = population;
        List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<DNA>().timeToDie).ToList();
        displaysize = (int)sortedList.Count;
        population.Clear();
        for (int i = (int) ((sortedList.Count/2)-1); i <= (int) sortedList.Count; i++)
        {
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
        }
          foreach(GameObject pop in sortedList)
        {
            Destroy(pop);
        }
        
    }

    void KillOld(List<GameObject> pop)
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        elasped += Time.deltaTime;
        if (elasped > trialTime)
        {
            BreedNewPopulation();
            //KillOld();
            
        }
      
            
        
    }
}
