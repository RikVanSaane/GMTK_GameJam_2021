using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TRKGeneric;
using UnityEngine.SceneManagement;
using FMODUnity;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip mainMenu;
    [SerializeField] private AudioClip ingame;
    [SerializeField] private AudioClip endgame;
    public int currentLevel;

    private void OnLevelWasLoaded()
    {
        GetComponent<Animator>().Play("FadeIn");
    }
    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);
        GetComponent<StudioEventEmitter>().Play();
    }
    public void FallIntoWell()
    {
        GetComponent<AudioSource>().Play();
        LoadNextCutScene();
    }
    public void LoadNextCutScene()
    {
        currentLevel++;
        if(currentLevel == 3)
        {
            //musicSource.clip = ingame;
            //musicSource.Play();
            GetComponent<StudioEventEmitter>().SetParameter("state", 1);
        }
        
        StartFadeToNextLevel();
    }
    public void StartFadeToNextLevel()
    {
        if (currentLevel == 14)
        {
            //musicSource.clip = endgame;
            //musicSource.Play();
            GetComponent<StudioEventEmitter>().SetParameter("state", 1.1f);
        }
        else if (currentLevel == 16)
        {
            GetComponent<StudioEventEmitter>().SetParameter("state", 1.5f);
        }
        GetComponent<Animator>().Play("FadeOut");
    }
    public void LoadNextLevel()
    {
        if (currentLevel == 17)
        {
            currentLevel = 0;
            //musicSource.clip = mainMenu;
            //musicSource.Play();
            //GetComponent<StudioEventEmitter>().Play();
            GetComponent<StudioEventEmitter>().SetParameter("state", 0);
            SceneManager.LoadScene("MainMenu");
            return;
        }
        SceneManager.LoadScene("level" + currentLevel);
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
