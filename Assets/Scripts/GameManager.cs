using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TRKGeneric;
using UnityEngine.SceneManagement;
using FMODUnity;

public class GameManager : MonoSingleton<GameManager>
{
    public int currentLevel;

    private void OnLevelWasLoaded()
    {
        GetComponent<Animator>().Play("FadeIn");
    }
    protected override void Init()
    {
        GetComponent<StudioEventEmitter>().Play();
        DontDestroyOnLoad(gameObject);
    }
    public void FallIntoWell()
    {
        GetComponent<AudioSource>().Play();
        LoadNextCutScene();
    }
    public void LoadNextCutScene()
    {
        currentLevel++;
        StartFadeToNextLevel();
    }
    public void StartFadeToNextLevel()
    {
        GetComponent<Animator>().Play("FadeOut");
    }
    public void LoadNextLevel()
    {
        if (currentLevel == 15)
        {
            currentLevel = 0;
            GetComponent<StudioEventEmitter>().Play();
            SceneManager.LoadScene("MainMenu");
        }
        else SceneManager.LoadScene("level" + currentLevel);
    }

    public void LoadOptionsScene()
	{
        SceneManager.LoadScene("OptionsMenu");
	}

    public void LoadCreditsScene()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    public void LoadMainMenu()
	{
        SceneManager.LoadScene("MainMenu");
	}

    public void LoadSceneWithName(string sceneName)
	{
        SceneManager.LoadScene(sceneName);
    }
}
