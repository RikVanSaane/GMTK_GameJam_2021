using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        //If the collidee position is within the collider, die
        if (GetComponent<Collider2D>().OverlapPoint(collision.gameObject.transform.position))
        {
            //If bear/player just fucking kill them
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerMovement>().Die();
            }
            else if (collision.gameObject.CompareTag("Bear"))
            {
                collision.gameObject.GetComponent<BearMovement>().Die();
            }
        }
    }
}
