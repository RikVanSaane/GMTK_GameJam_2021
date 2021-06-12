using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearMovement : MonoBehaviour
{
    public float distractionRange = 5f;
    public BearState bearState;

    [SerializeField] private float wanderSpeed = 1f;
    [SerializeField] private float distractionSpeed = 3f;
    [SerializeField] private float visionCone = 180f;
    [SerializeField] private float sniffRange = 0.5f;

    //Component references
    private GameObject player;
    private Rigidbody2D rb;
    private Collider2D bearCollider;
    private Animator bearAnimator;

    //Movement variables
    private Vector2 lastScarePosition;
    private Vector2 lastDistractedPosition;
    private Collider2D distractionCollider;
    private Collider2D oldDistractionCollider;
    private Quaternion bearRotation;

    public enum BearState
    {
        Idle, BeingPulled, Distracted, Scared
    }

    private void Start()
    {
        sniffRange *= sniffRange; //For use in sqrMagnitude (micro optimization)
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        bearCollider = GetComponent<Collider2D>();
        bearAnimator = GetComponent<Animator>();

        ReturnToIdle();
    }
    private void Update()
    {
        CheckForDistractions();
        switch (bearState)
        {
            case BearState.Distracted: ChaseDistraction(); break;
            case BearState.Scared: RunAway(); break;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Check if hit something that should kill it
        if (collision.gameObject.layer == LayerMask.GetMask("Walls"))
        {
        }
        if (bearState == BearState.Scared && !collision.gameObject.CompareTag("Crate"))
        {
            //TODO play bump sound/anim maybe wait untill anim is done before it starts tracking again
            ReturnToIdle();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fish"))
        {
            if (collision.gameObject.GetComponent<BearInterest>().isInActive) return;
            StartCoroutine(EatFish(collision.gameObject));
        }
    }
    public void Die()
    {
        //TODO play anim/sound and call GameManager.GameOver()
        bearState = BearState.Idle;
        GameObject.Find("FinishPoint").GetComponent<FinishPoint>().ReloadLevel();
    }
    public void GetBearSprayed(Vector2 direction)
    {
        StopAllCoroutines();
        bearState = BearState.Scared;
        direction = (Vector2)transform.position + direction * 100;
        lastScarePosition = direction;
        //transform.rotation = Quaternion.Euler(0, 0, GetAngleFromVector(direction));
        float angle = Vector3.Angle(direction, transform.right);
        float sign = Mathf.Sign(Vector3.Dot(transform.forward, Vector3.Cross(direction, transform.right)));
        float signedAngle = angle * sign;
        float lookAngle = (signedAngle + 180) % 360;
        int index = Mathf.RoundToInt(lookAngle / 90);
        if (index == 4) index = 0;
        bearAnimator.Play("Walk" + index);
    }
    private IEnumerator EatFish(GameObject fish)
    {
        //TODO play munching sounds
        bearAnimator.Play("Idle");
        Destroy(fish);
        yield return new WaitForSeconds(0.5f);
        ReturnToIdle();
        yield break;
    }
    private void ChaseDistraction()
    {
        //Run towards thing that attracted bear

        //TODO if we add pathfinding use it here
        if (!bearCollider.IsTouching(distractionCollider))
        {
            rb.MovePosition(Vector2.MoveTowards(transform.position, lastDistractedPosition, Time.deltaTime * distractionSpeed));
        }
        else
        {
            bearAnimator.SetTrigger("Eating");
        }

        //If in range do action (eat fish, scratch back)
        //Need to know wether its a thrown fish or Honey/Fish pond

        //If fish is eaten
        //ReturnToIdle();
    }

    private void ReturnToIdle()
    {
        bearState = BearState.Idle;
        bearAnimator.Play("Idle");
        //StartCoroutine(idlingRoutine);
    }

    private void RunAway()
    {
        //Run away from thing that scared bear untill no longer in scare range of object + small scare timer
        rb.MovePosition(Vector2.MoveTowards(transform.position, lastScarePosition, Time.deltaTime * distractionSpeed));
    }
    private void CheckForDistractions()
    {
        if (bearState == BearState.Scared) return;
        //Overlap circle with layermask for BearInterests layer (this layer contains scares and distractions
        //Distractions have priorities
        //Scares take priority
        LayerMask layerMask = LayerMask.GetMask("BearInterests");
        Collider2D[] interests = Physics2D.OverlapCircleAll(transform.position, distractionRange, layerMask);
        //List<Collider2D> interests = new List<Collider2D>();
        //int raysToCast = Mathf.CeilToInt(visionCone / 10);
        //float angleStepSize = visionCone / raysToCast;
        //for (int i = 0; i < raysToCast + 1; i++)
        //{
        //    float localAngle = angleStepSize * i;
        //    float worldSpaceAngle = -GetAngleFromVector(transform.right) + 90;
        //    localAngle = worldSpaceAngle + localAngle - visionCone / 2;
        //    Vector2 rayDirection = VecFromAngle(localAngle);
        //    RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, distractionRange, layerMask);
        //    if (hit)
        //    {
        //        interests.Add(hit.collider);
        //    }
        //}      

        BearInterest mostInteresting = null;
        foreach (Collider2D interest in interests)
        {
            //Gets the BearInterest component, which holds the value of how interesting the object is
            BearInterest bearInterest;
            if (!interest.gameObject.TryGetComponent<BearInterest>(out bearInterest))
            {
                Debug.LogError("Didn't attach BearInterest script to " + interest.gameObject.name);
            }

            //Scary stuff takes priority, so get scared and exit function
            if (bearInterest.isScary)
            {
                bearState = BearState.Scared;
                lastScarePosition = interest.ClosestPoint(transform.position);
                StopAllCoroutines();
                return;
            }

            //Ignore if is in air
            if (bearInterest.isInActive) continue;

            //If has no mostInteresting yet
            if (!mostInteresting)
            {
                mostInteresting = bearInterest;
                lastDistractedPosition = interest.ClosestPoint(transform.position);
                distractionCollider = interest;
            }

            //If current point is more interesting than previous
            if (bearInterest.interest > mostInteresting.interest)
            {
                mostInteresting = bearInterest;
                lastDistractedPosition = interest.ClosestPoint(transform.position);
                distractionCollider = interest;
            }
        }
        layerMask = LayerMask.GetMask("FishLayer");
        interests = Physics2D.OverlapCircleAll(transform.position, 100, layerMask);
        foreach (Collider2D interest in interests)
        {
            //Gets the BearInterest component, which holds the value of how interesting the object is
            BearInterest bearInterest;
            if (!interest.gameObject.TryGetComponent<BearInterest>(out bearInterest))
            {
                Debug.LogError("Didn't attach BearInterest script to " + interest.gameObject.name);
            }
            //Ignore if is in air
            if (bearInterest.isInActive) continue;

            //If has no mostInteresting yet
            if (!mostInteresting)
            {
                mostInteresting = bearInterest;
                lastDistractedPosition = interest.ClosestPoint(transform.position);
                distractionCollider = interest;
            }

            //If current point is more interesting than previous
            if (bearInterest.interest > mostInteresting.interest)
            {
                mostInteresting = bearInterest;
                lastDistractedPosition = interest.ClosestPoint(transform.position);
                distractionCollider = interest;
            }
        }

        //If found no interests 
        if (interests.Length == 0)
        {
            if (!(bearState == BearState.Idle)) ReturnToIdle();
            return;
        }

        if (mostInteresting == null)
        {
            if (!(bearState == BearState.Idle)) ReturnToIdle();
            return;
        }
        bearState = BearState.Distracted;

        //If found new distraction
        if(oldDistractionCollider != distractionCollider)
        {

            float angle = Vector3.Angle((lastDistractedPosition - (Vector2)transform.position), transform.right);
            float sign = Mathf.Sign(Vector3.Dot(transform.forward, Vector3.Cross((lastDistractedPosition - (Vector2)transform.position), transform.right)));
            float signedAngle = angle * sign;
            float lookAngle = (signedAngle + 180) % 360;
            int index = Mathf.RoundToInt(lookAngle / 90);
            if (index == 4) index = 0;
            bearAnimator.Play("Walk" + index);
            oldDistractionCollider = distractionCollider;
        }

        //TODO If distraction check if player has any fish left, if not game over because cant lure away
    }

    private float GetAngleFromVector(Vector2 deltaVector)
    {
        return Mathf.Atan2(deltaVector.y, deltaVector.x) * Mathf.Rad2Deg;
    }

    private static Vector2 VecFromAngle(float angle)
    {
        return new Vector2(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distractionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sniffRange / sniffRange);

        //int raysToCast = Mathf.CeilToInt(visionCone / 10);
        //float angleStepSize = visionCone / raysToCast;
        //for (int i = 0; i < raysToCast + 1; i++)
        //{
        //    float localAngle = angleStepSize * i;
        //    float worldSpaceAngle = -GetAngleFromVector(transform.right)+90;
        //    localAngle = worldSpaceAngle + localAngle - visionCone / 2;
        //    Vector2 rayDirection = VecFromAngle(localAngle);
        //    Gizmos.DrawLine(transform.position, (Vector2)transform.position + rayDirection * distractionRange);
        //}
    }
}
