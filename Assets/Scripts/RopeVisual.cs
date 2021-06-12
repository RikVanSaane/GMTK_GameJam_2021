using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeVisual : MonoBehaviour
{
    public int ropeSegments;

    [SerializeField]
    private GameObject playerObject;

    private LineRenderer lineRenderer;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = ropeSegments+1;
        if(playerObject == null)
		{
            playerObject = GameObject.Find("Player");
        }
    }
    void Update()
    {
        for(int i = 0; i <= ropeSegments; i++)
		{
            lineRenderer.SetPosition(i, Vector3.Lerp(transform.position, playerObject.transform.position, (float)i / ropeSegments));
		}

        // lineRenderer.SetPosition(0, transform.position);
        // lineRenderer.SetPosition(1, playerObject.transform.position);
    }
}
