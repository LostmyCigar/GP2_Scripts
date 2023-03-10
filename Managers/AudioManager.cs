using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class AudioManager : GenericSingleton<AudioManager> {
    public Sound[] Sounds;
    private ObjectPool<AudioPlayer> _pool;
    public AudioPlayer AudioPlayerPrefab;
    private Vector3 _spawnPos;
    private Sound _sound;
    private static Dictionary<Sound, float> _soundTimerDictionary;
    private void Awake() {
        _pool = new ObjectPool<AudioPlayer>(
            OnObjectCreate, OnObjectGet, OnObjectRelease, OnObjectDestroy, false, 10, 100);
        
        Sounds = Resources.LoadAll<Sound>("Sounds");
        _soundTimerDictionary = new Dictionary<Sound, float>();
        
        foreach (Sound sound in Sounds) {
            if (sound.RepeatingWithTimer)
                _soundTimerDictionary[sound] = sound.Timer;
        }
    }
    public void Play(string clipName, Vector3 position) {
        _spawnPos = position;
        _sound = Array.Find(Sounds, sound => sound.name == clipName);
        if (CanPlaySound(_sound)) {
            var soundTemp = _pool.Get();
            soundTemp.Play();
            StartCoroutine(ReturnSound(soundTemp.Sound.Clip.length, soundTemp));
        }
    }
    private IEnumerator ReturnSound(float time, AudioPlayer player) {
        yield return new WaitForSeconds(time);
        _pool.Release(player);
    }
    public void Play(string clipName) {
        Sound sound = Array.Find(Sounds, sound => sound.name == clipName.ToUpper());
        if (CanPlaySound(sound)) 
            sound.Source.Play();
    }
    private AudioPlayer OnObjectCreate() {
        var tempObject = Instantiate(AudioPlayerPrefab, _spawnPos, Quaternion.identity).GetComponent<AudioPlayer>();
        tempObject.Sound = _sound;
        return tempObject;
    }
    private void OnObjectGet(AudioPlayer audioObj) {
        audioObj.transform.position = _spawnPos;
        audioObj.Sound = _sound;
        audioObj.gameObject.SetActive(true);
    }
    private void OnObjectRelease(AudioPlayer audioObj) {
        audioObj.gameObject.SetActive(false);
    }
    private void OnObjectDestroy(AudioPlayer audioObj) {
        Destroy(audioObj.gameObject);
    }
    private static bool CanPlaySound(Sound sound) {
        switch (sound.RepeatingWithTimer) {
            default:
                return true;
            case true:
                if (_soundTimerDictionary.ContainsKey(sound)) {
                    float lastTimePlayed = _soundTimerDictionary[sound];
                    float timerMax = sound.Timer;
                    if (lastTimePlayed + timerMax < Time.time) {
                        _soundTimerDictionary[sound] = Time.time;
                        return true;
                    }else {
                        return false;
                    } 
                }
                break;
        }
        return false;
    }
}
    