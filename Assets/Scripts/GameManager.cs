using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TRKGeneric;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public int currentLevel = 1;

    private void OnLevelWasLoaded()
    {
        GetComponent<Animator>().Play("FadeIn");
    }
    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void StartFadeToNextLevel()
    {
        GetComponent<Animator>().Play("FadeOut");
    }
    public void LoadNextLevel()
    {
        SceneManager.LoadScene("level" + currentLevel);
    }
}
