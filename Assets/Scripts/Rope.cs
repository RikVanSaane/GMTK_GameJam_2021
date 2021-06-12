using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rope : MonoBehaviour
{
    public Rigidbody2D hook;
    public GameObject linkPrefab;
    public int numberOfLinks = 7;
    public float distanceBwLinks = 1.2f;
    public GameObject player;

    private LineRenderer lineRenderer;
    private List<GameObject> links = new List<GameObject>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numberOfLinks + 1;

        links.Add(hook.gameObject);

        lineRenderer.SetPosition(0, hook.transform.position);

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        GenerateRope();
    }

    void GenerateRope()
	{
        Rigidbody2D previoudRB = hook;
        for(int i = 0; i < numberOfLinks; i++)
		{
            GameObject link = Instantiate(linkPrefab, transform);
            HingeJoint2D joint = link.GetComponent<HingeJoint2D>();
            joint.connectedAnchor = new Vector2(0, -distanceBwLinks);
            joint.connectedBody = previoudRB;

            links.Add(link);

            lineRenderer.SetPosition(i+1, link.transform.position);

            if (i < numberOfLinks - 1)
                previoudRB = link.GetComponent<Rigidbody2D>();
            else
                player.GetComponent<PlayerRope>().ConnectPlayerRope(link.GetComponent<Rigidbody2D>());
		}        
	}

	private void Update()
	{
		for(int i = 0; i < numberOfLinks+1; i++)
		{
            lineRenderer.SetPosition(i, links[i].transform.position);
		}
	}
}
