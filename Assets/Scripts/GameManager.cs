using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TRKGeneric;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public int currentLevel;

    private void OnLevelWasLoaded()
    {
        GetComponent<Animator>().Play("FadeIn");
    }
    protected override void Init()
    {
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
        if (currentLevel == 15) LoadMainMenu();
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
