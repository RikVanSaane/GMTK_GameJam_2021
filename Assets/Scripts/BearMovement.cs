using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearMovement : MonoBehaviour
{
    [SerializeField] private float wanderSpeed = 1f;
    [SerializeField] private float distractionSpeed = 3f;
    [SerializeField] private float distractionRange = 5f;
    [SerializeField] private float sniffRange = 0.5f;

    private GameObject player;
    private Rigidbody2D rb;
    private BearState bearState;

    private Vector2 lastScarePosition;
    private Vector2 lastDistractedPosition;
    private enum BearState
    {
        Idle, BeingPulled, Distracted, Scared
    }

    private void Start()
    {
        sniffRange *= sniffRange; //For use in sqrMagnitude (micro optimization)
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();

        ReturnToIdle();
    }
    private void Update()
    {
        CheckForDistractions();
        switch (bearState)
        {
            case BearState.Idle: Idle(); break;
            case BearState.BeingPulled: BeingPulled(); break;
            case BearState.Distracted: ChaseDistraction(); break;
            case BearState.Scared: RunAway(); break;
        }
    }
    private void Idle()
    {
        //Unused as we use IdlingLoop now

        //Alternate between actions:
        //Take small nap
        //Wander to position
        //sniff player
        //switch (Random.Range(0, 3))
        //{
        //    case 1: //nap
        //}

        //Every x seconds Pick random position in player range to move to

    }
    private void BeingPulled()
    {
        //Stop idling, and move with player
        //StopAllCoroutines();
    }
    private void ChaseDistraction()
    {
        //Run towards thing that attracted bear
        rb.MovePosition(Vector2.MoveTowards(transform.position, lastDistractedPosition, Time.deltaTime * distractionSpeed));

        //If in range do action (eat fish, scratch back)


        //If fish is eatn
        //ReturnToIdle();
    }

    private void ReturnToIdle()
    {
        bearState = BearState.Idle;
        StartCoroutine(IdlingLoop());
    }
    private IEnumerator IdlingLoop()
    {
        while (true)
        {
            //Decide thing to do
            switch (Random.Range(0, 3))
            {
                case 0: yield return SniffPlayer();  break;
                case 1: yield return TakeNap();  break;
                case 2: yield return WanderAround();  break;
            }
        }
    }
    private IEnumerator SniffPlayer()
    {
        //Move towards player untill in sniffrange
        while((player.transform.position - transform.position).sqrMagnitude > sniffRange)
        {
            //TODO if we add pathfinding use it here
            rb.MovePosition(Vector2.MoveTowards(transform.position, player.transform.position, Time.deltaTime * wanderSpeed));
            yield return null;
        }

        //Sniff for a while
        yield return new WaitForSeconds(2f);

        yield break;
    }
    private IEnumerator TakeNap()
    {
        //Yawn

        //Take nap

        yield break;
    }
    private IEnumerator WanderAround()
    {
        //Find random position in player range

        //Move towards position untill reached

        yield break;
    }

    private void RunAway()
    {
        //Run away from thing that scared bear untill no longer in scare range of object + small scare timer

    }
    private void CheckForDistractions()
    {
        //Overlap circle with layermask for BearInterests layer (this layer contains scares and distractions
        //Distractions have priorities
        //Scares take priority
        LayerMask layerMask = LayerMask.GetMask("BearInterests");
        Collider2D[] interests = Physics2D.OverlapCircleAll(transform.position, distractionRange, layerMask);

        if (interests.Length == 0) return;

        BearInterest mostInteresting = null;
        foreach(Collider2D interest in interests)
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
            //If has no mostInteresting yet
            if (!mostInteresting)
            {
                mostInteresting = bearInterest;
                lastDistractedPosition = interest.ClosestPoint(transform.position);
            }

            //If current point is more interesting than previous
            if (bearInterest.interest > mostInteresting.interest)
            {
                mostInteresting = bearInterest;
                lastDistractedPosition = interest.ClosestPoint(transform.position);
            }
        }
        //Stop the IdlingLoop
        StopAllCoroutines();
        bearState = BearState.Distracted;

        //If distraction check if player has any fish left, if not game over because cant lure away

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distractionRange);
    }
}
