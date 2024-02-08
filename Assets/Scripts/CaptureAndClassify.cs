using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureAndClassify : MonoBehaviour
{
    [Header("Detected Emotion")]
    [Tooltip("Material and Image for detection and detected emotion")]
    public GameObject screen;
    public Material mat; 
    public Texture2D textemotion;
    public GameObject classifier;
    public int emotion;
    Renderer m_Renderer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            
            Debug.Log("space key was pressed");
            Capture();
        }
    }

    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();
        textemotion = ScreenCapture.CaptureScreenshotAsTexture();
        m_Renderer = screen.GetComponent<Renderer>();
        m_Renderer.material.SetTexture("_MainTex", textemotion);
        classifier.GetComponent<Classifier>().imageTexture = textemotion;

        // do something with texture
        // cleanup
        //Object.Destroy(texture);
    }


    public void Capture()
    {
        //capture screen
        //Set screen as texture
      //  StartCoroutine(RecordFrame());
       emotion = classifier.GetComponent<Classifier>().emotion;
        Debug.Log(emotion);
       
    }

    
}
