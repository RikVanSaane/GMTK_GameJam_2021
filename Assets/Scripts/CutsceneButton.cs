using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneButton : MonoBehaviour
{
    public void NextScene()
    {
        GameManager.Instance.LoadNextCutScene();
    }
    public void FallIntoWell()
    {
        GameManager.Instance.FallIntoWell();
    }
}
