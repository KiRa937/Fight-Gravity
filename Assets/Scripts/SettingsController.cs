using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Slider musicVolumeSlider;
    public Slider soundVolumeSlider;

	public AudioMixer mixer;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {

		musicVolumeSlider.onValueChanged.AddListener(delegate {OnMusicVolumeChange();});
		soundVolumeSlider.onValueChanged.AddListener(delegate {OnSoundVolumeChange();});

    }

    public void OnMusicVolumeChange()
    {
		//musicSource.volume = musicVolumeSlider.value;
		mixer.SetFloat("musicVol", musicVolumeSlider.value);
    }

    public void OnSoundVolumeChange()
    {
		mixer.SetFloat("sfxVol", soundVolumeSlider.value);
    }
}
