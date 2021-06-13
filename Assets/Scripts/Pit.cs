using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour
{
    private bool isDead;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead) return;

        //If the collidee position is within the collider, die
        if (GetComponent<Collider2D>().OverlapPoint(collision.gameObject.transform.position))
        {
            //If bear/player just fucking kill them
            if (collision.gameObject.CompareTag("Player"))
            {
                isDead = true;
                collision.gameObject.GetComponent<PlayerMovement>().Die();
                GetComponent<AudioSource>().Play();
            }
            else if (collision.gameObject.CompareTag("Bear"))
            {
                isDead = true;
                collision.gameObject.GetComponent<BearMovement>().Die();
                GetComponent<AudioSource>().Play();
            }
        }
    }
}
