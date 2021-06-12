using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TRKGeneric;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public int currentLevel = 1;

    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void LoadNextLevel()
    {
        SceneManager.LoadScene("level" + currentLevel);
    }
}
