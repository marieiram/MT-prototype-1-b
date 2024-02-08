using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Clayxels;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Rittai : Agent
{
    public ClayContainer oya;
    public ClayObject[] objects;
    public Transform[] initialpositions;
    public Vector3[] towardspositions;
    public Vector3[] towardsscale;
    public ClayObject[] initialcolors;
    public Color[] towardscolor;
    public int[] Emotions;
    public int currentEmotion;
    private Vector3 range = new Vector3(2.0f, 2.0f, 2.0f);
    public CaptureAndClassify classifier;
    public GameObject classi;


    ///to restart
    public Transform[] saveinitialpositions;
    public ClayObject[] saveinitialcolors;


    // Start is called before the first frame update
    void Start()
    {
        classifier = classi.GetComponent<CaptureAndClassify>();
        for (int i = 0; i < initialpositions.Length; i++)
        {

            saveinitialcolors[i] = Instantiate(initialcolors[i], new Vector3(0, 0, 0), Quaternion.identity);
            saveinitialpositions[i] = initialpositions[i];

        }
    }

    public override void Initialize()
    {
        //saveinitialpositions = initialpositions;
        //saveinitialcolors = initialcolors;
        //for (int i = 0; i < initialpositions.Length; i++)
        //{
        //    saveinitialcolors[i] = initialcolors[i];
        //    saveinitialpositions[i] = initialpositions[i];
        //}
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("Start////////////////////////////////////////////////////////////////////////////////////");
        //initialpositions = saveinitialpositions;
        //initialcolors = saveinitialcolors;
        for (int i = 0; i < initialpositions.Length; i++)
        {
            initialcolors[i].color = new Vector4(1.0f,1.0f,1.0f,1.0f);
            initialpositions[i].position = Vector3.MoveTowards(initialpositions[i].position, saveinitialpositions[i].position, 0.0f);
            initialpositions[i].localScale = Vector3.MoveTowards(initialpositions[i].localScale, saveinitialpositions[i].localScale, 0.0f);

        }
     }

    public override void CollectObservations(VectorSensor sensor)
    {
        for (int i = 0; i < initialpositions.Length; i++)
        {
            sensor.AddObservation(initialpositions[i].position); //3 * 4
            sensor.AddObservation(initialpositions[i].localScale); //3 * 4
            sensor.AddObservation(initialcolors[i].color.r); // 3 * 4
            sensor.AddObservation(initialcolors[i].color.g);
            sensor.AddObservation(initialcolors[i].color.b);
        }

        sensor.AddObservation(currentEmotion);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var count = 0;
        var steps = Time.deltaTime * 1.0f;

        for (int i = 0; i < initialpositions.Length; i++)
        {
            towardspositions[i] = new Vector3(actions.ContinuousActions[count], actions.ContinuousActions[count+1], actions.ContinuousActions[count+2]);
            count += 3;
            initialpositions[i].position = Vector3.MoveTowards(initialpositions[i].position, towardspositions[i], steps);
            var distance = Vector3.Distance(initialpositions[i].position, range);
            if(distance <= 1.0f)
            {
                Debug.Log("END///////");
                EndEpisode();
            }
        }

        for (int i = 0; i < initialpositions.Length; i++)
        {
            towardsscale[i] = new Vector3(actions.ContinuousActions[count], actions.ContinuousActions[count + 1], actions.ContinuousActions[count + 2]);
            count += 3;
            initialpositions[i].localScale = Vector3.MoveTowards(initialpositions[i].localScale, towardsscale[i], steps);
            var distance = Vector3.Distance(initialpositions[i].localScale, range);
            if (distance <= 1.0f)
            {
                Debug.Log("END///////");
                EndEpisode();
            }
        }

        for (int i = 0; i < initialpositions.Length; i++)
        {
            towardscolor[i] = new Vector4(actions.ContinuousActions[count], actions.ContinuousActions[count + 1], actions.ContinuousActions[count + 2], actions.ContinuousActions[count + 3]);
            count += 4;
            initialcolors[i].color = Vector4.MoveTowards(initialcolors[i].color, towardscolor[i], steps);
        }

        //Debug.Log(actions.DiscreteActions[0]);

        classifier.Capture();
        currentEmotion = classifier.emotion;

        if (currentEmotion == actions.DiscreteActions[0])
        {
            AddReward(0.5f);
            Debug.Log("getReward");
            EndEpisode();

        }
        
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown("space"))
    //    {
    //        freeze("test");
    //    }


    //    var steps = Time.deltaTime * 1.0f;

    //    for(int i=0; i<initialpositions.Length; i++)
    //    {
    //        //create parameter
    //        towardspositions[i] = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
    //        towardsscale[i] = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
    //        towardscolor[i] = new Vector4(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));


            
    //        //update parameter
    //        initialpositions[i].position = Vector3.MoveTowards(initialpositions[i].position, towardspositions[i], steps);
    //        initialpositions[i].localScale = Vector3.MoveTowards(initialpositions[i].localScale, towardsscale[i], steps);
    //        //initialpositions[i].localScale = Vector3.MoveTowards(initialpositions[i].localScale, towardsscale[i], steps);
    //        initialcolors[i].color = Vector4.MoveTowards(initialcolors[i].color, towardsscale[i], steps);

    //    }
    //}

    public void freeze(string name)
    {
        Mesh test;
        test = oya.generateMesh(100, true, true, 100, true);
        AssetDatabase.CreateAsset(test, "Assets/Frozen/" + name + ".mesh");
        AssetDatabase.SaveAssets();

        //oya.freezeContainersHierarchyToMesh();
        //oya.defrostContainersHierarchy();


    }
}
