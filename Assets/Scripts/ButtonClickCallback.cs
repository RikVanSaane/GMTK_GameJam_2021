using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickCallback : MonoBehaviour
{
    public enum ButtonScene
	{
        Play,
        Menu,
        Credits,
        Options
	}

    public ButtonScene buttonScene;
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(OnButtonPressed);
    }

    void OnButtonPressed()
	{
        if (buttonScene == ButtonScene.Menu)
            GameManager.Instance.LoadMainMenu();
        else if (buttonScene == ButtonScene.Credits)
            GameManager.Instance.LoadCreditsScene();
        else if (buttonScene == ButtonScene.Options)
            GameManager.Instance.LoadOptionsScene();
        else if (buttonScene == ButtonScene.Play)
            GameManager.Instance.LoadNextCutScene();

    }
}
