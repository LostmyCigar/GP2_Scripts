using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private AudioMixer _mixer;
    private Sound _sound;
    private AudioSource _source;
    public Sound Sound {
        get => _sound;
        set {
            if (_source == null) {
                _source = GetComponent<AudioSource>();
            }
            if (_mixer == null) {
                _mixer = Resources.Load<AudioMixer>("Mixer/Master");
            }
            _source.clip = value.Clip;
            _source.volume = value.Volume;
            _source.pitch = value.Pitch;
            _source.panStereo = value.StereoPan;
            _source.loop = value.Loop;
            _source.spatialBlend = value.SpatialBlend;
            _source.outputAudioMixerGroup = _mixer.FindMatchingGroups(value.Type.ToString())[0];
            
            _sound = value;
        }
    }
    public void Play() {
        if (_source != null) {
            _source.Play();
        }
    }
}
