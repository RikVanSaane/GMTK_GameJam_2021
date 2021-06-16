using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VolumeSliders : MonoBehaviour
{
    [SerializeField] private GameObject volumeSliders;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    private void Start()
    {
        musicSlider.value = GameManager.Instance.musicVolume;
        sfxSlider.value = GameManager.Instance.sfxVolume;
    }
    public void RestartLevel()
    {
        int levelIndex = int.Parse(SceneManager.GetActiveScene().name.Remove(0, 5));
        GameManager.Instance.currentLevel = levelIndex;
        GameManager.Instance.StartFadeToNextLevel();
    }
    public void ToggleVolumeSliders()
    {
        volumeSliders.SetActive(!volumeSliders.activeSelf);
        Time.timeScale = volumeSliders.activeSelf == true ? 0 : 1;
    }
    public void ChangeMusicVolume(float value)
    {
        GameManager.Instance.SetMusicVolume(value);
    }
    public void ChangeSFXVolume(float value)
    {
        GameManager.Instance.SetSFXVolume(value);
    }
}
