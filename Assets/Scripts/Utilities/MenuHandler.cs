using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
	[SerializeField] GameObject mainMenu = default;
	[SerializeField] GameObject optionsMenu = default;
	[SerializeField] Slider sfxVolumeSlider = default;
	[SerializeField] Slider musicVolumeSlider = default;


	bool isOptionsMenu = false;
	SettingsHolder settings;

	private void Start()
	{
		settings = FindObjectOfType<SettingsHolder>();
		optionsMenu.SetActive(false);
		mainMenu.SetActive(true);
		AddListeners();
		ResetParameters();
	}

	private void AddListeners()
	{
		sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
		musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
	}

	private void ResetParameters()
	{
		sfxVolumeSlider.value = settings.GetSFXVolume();
		musicVolumeSlider.value = settings.GetMusicVolume();
	}

	public void GoToOptionsMenu()
	{
		if (!isOptionsMenu)
		{
			optionsMenu.SetActive(true);
			mainMenu.SetActive(false);
			isOptionsMenu = true;
		}
		else
		{
			optionsMenu.SetActive(false);
			mainMenu.SetActive(true);
			isOptionsMenu = false;
		}
	}

	public void SetSFXVolume(float value)
	{
		settings.SetSFXVolume(value);
	}

	public void SetMusicVolume(float value)
	{
		settings.SetMusicVolume(value);
	}
}