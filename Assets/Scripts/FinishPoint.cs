using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    private bool isStartupFade = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bear"))
        {
            //TODO Screenwipe
            GameManager.Instance.currentLevel++;
            GetComponent<Animator>().Play("FadeOut");
        }
    }
    private void LoadNextLevel()
    {
        if (isStartupFade)
        {
            isStartupFade = false;
            return;
        }
        GameManager.Instance.LoadNextLevel();
    }
}
