using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    private AudioMixer _mixer;

    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _ambientSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private void Awake() {
        if (_mixer == null) {
            _mixer = Resources.Load<AudioMixer>("Mixer/Master");
        }
    }
    public void UpdateMasterVolume() {
        _mixer.SetFloat("MasterVolume", Mathf.Log(_masterSlider.value) * 20);
    }
    public void UpdateAmbientVolume() {
        _mixer.SetFloat("AmbientVolume", Mathf.Log(_ambientSlider.value) * 20);
    }
    public void UpdateMusicVolume() {
        _mixer.SetFloat("MusicVolume", Mathf.Log(_musicSlider.value) * 20);
    }
    public void UpdateSfxVolume() {
        _mixer.SetFloat("SfxVolume", Mathf.Log(_sfxSlider.value) * 20);
    }
    
    
}
