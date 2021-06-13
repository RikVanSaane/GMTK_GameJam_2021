using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeVisual : MonoBehaviour
{
    public int ropeSegments;
    public AnimationCurve ropeCurve;
    public float ropeOffset = 0.1f;

    [SerializeField]
    private GameObject playerObject;
    private GameObject bearObject;
    static float t = 0.0f;
    private bool reverseAnim = false;

    private float bearRadius;
    private float playerRadius;

    private LineRenderer lineRenderer;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = ropeSegments+1;
        if(playerObject == null)
		{
            playerObject = GameObject.Find("Player");
        }
        if(bearObject == null)
		{
            bearObject = GameObject.Find("Bear");
		}

        //playerRadius = playerObject.GetComponent<CircleCollider2D>().radius;
        //bearRadius = playerRadius * 3; //bear no longer has circle collider so had to change this
        playerRadius = 0.1f;
        bearRadius = 0.1f;
    }
    void Update()
    {
        for (int i = 0; i <= ropeSegments; i++)
        {
            float distance = Vector2.Distance(transform.position, playerObject.transform.position);
            Vector3 diff = transform.position - playerObject.transform.position;
            Vector2 temp = Vector3.Cross(transform.position - playerObject.transform.position, Vector3.forward);
            float animCurveValue = ropeCurve.Evaluate((float)i / ropeSegments);

            // if (reverseAnim)
                //animCurveValue = -animCurveValue;
            t += 0.1f * Time.deltaTime;
            // lineRenderer.SetPosition(i, Vector2.Lerp(transform.position, playerObject.transform.position, (float)i / ropeSegments) + temp * Mathf.Lerp(animCurveValue, -animCurveValue, t) * ropeOffset);
            lineRenderer.SetPosition(i, Vector2.Lerp(transform.position - diff.normalized * bearRadius, playerObject.transform.position + diff.normalized * playerRadius, (float)i / ropeSegments) + temp * animCurveValue * (1 / (distance * distance * distance)) * ropeOffset);
            if (t > 1.0f)
			{
                reverseAnim = !reverseAnim;
                t = 0.0f;
			}
            
        }
    }
}
