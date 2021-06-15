using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    private bool isFading;
    private bool isStartupFade = true;
    private bool isReloadFade;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bear"))
        {
            if (isFading) return; //so if bear and player enter hatch you dont skip a level
            if (collision.gameObject.CompareTag("Bear"))
            {
                collision.gameObject.GetComponent<BearMovement>().Die();
            }
            isFading = true;
            GetComponent<AudioSource>().Play();
            GameManager.Instance.currentLevel++;
            //"Temp" fix to skip main menu stuff, if is in level3, load level 4
            int levelIndex = int.Parse(SceneManager.GetActiveScene().name.Remove(0, 5));
            GameManager.Instance.currentLevel = levelIndex + 1;
            GameManager.Instance.StartFadeToNextLevel();
        }
    }
    public void ReloadLevel()
    {
        isFading = true;
        int levelIndex = int.Parse(SceneManager.GetActiveScene().name.Remove(0, 5));
        GameManager.Instance.currentLevel = levelIndex;
        GameManager.Instance.StartFadeToNextLevel();
    }
}
