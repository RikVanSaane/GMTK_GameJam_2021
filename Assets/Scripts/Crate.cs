using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Crate : MonoBehaviour
{
    [SerializeField] private Sprite brokenSprite;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Test if is bear
        if (!collision.gameObject.CompareTag("Bear")) return;
        //Test if bear is scared
        if (collision.gameObject.GetComponent<BearMovement>().bearState != BearMovement.BearState.Scared) return;

        //TODO play break anim/sound

        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = brokenSprite;
        GetComponent<ShadowCaster2D>().enabled = false;
        GetComponent<AudioSource>().Play();
    }
}
