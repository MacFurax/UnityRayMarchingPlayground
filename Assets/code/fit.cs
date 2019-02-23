using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class fit : MonoBehaviour
{
    public bool fitToViewPort = false;

    // Start is called before the first frame update
    void Start()
    {
        FitToViewPort();
    }

    private void Awake()
    {
        FitToViewPort();
    }

    void FitToViewPort()
    {
        Camera camera = Camera.main;

        Vector3 upperRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        Vector3 lowerLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));

        Vector3 size = GetComponent<Renderer>().bounds.size;
        
        float viewPortwidth = upperRight.x - lowerLeft.x;
        
        float ratio = viewPortwidth / size.x;

        Debug.Log("Plan width " + size.x + " Viewport width " + viewPortwidth + " => Scale " + ratio);

        transform.localScale = new Vector3(ratio, 1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (fitToViewPort)
        {
            fitToViewPort = false;
            FitToViewPort();
        }
#endif
    }

    void OnDrawGizmosSelected()
    {
        
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position- new Vector3(1.0f,0,0), 0.1F);

        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(p, 0.1F);


    }
}
