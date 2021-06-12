using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour
{
    [SerializeField] private Sprite disarmedSprite;

    private bool isDisarmed;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDisarmed) return;

        //If fish get disarmed
        if (collision.gameObject.CompareTag("Fish"))
        {
            collision.gameObject.GetComponent<BearInterest>().StopAllCoroutines();
            collision.gameObject.GetComponent<BearInterest>().isInActive = true;
            isDisarmed = true;
            //TODO play disarm sound/anim
            GetComponent<SpriteRenderer>().sprite = disarmedSprite;
        }

        //If bear/player just fucking kill them
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().Die();
        } else if (collision.gameObject.CompareTag("Bear"))
        {
            collision.gameObject.GetComponent<BearMovement>().Die();
        }
    }
}
