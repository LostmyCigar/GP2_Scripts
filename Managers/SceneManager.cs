using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : GenericSingleton<SceneManager> {
    private const float MINIMUMLOADTIME = 3f;
    private bool Loading { get; set; } = false;
    private float _loadTimer;
    private AsyncOperation _sceneLoad;

    private void Awake() {
        DontDestroyOnLoad(this);
    }

    public void LoadScene(string scene) { // input to starT?
        Loading = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScene", LoadSceneMode.Single);
        _sceneLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        _sceneLoad.allowSceneActivation = false;
      
    }
    

    private void Update() { // clumsy to have update in scenemanager
        if (!Loading) return;
        _loadTimer += Time.deltaTime;

        
        if (_sceneLoad.isDone && _loadTimer > MINIMUMLOADTIME && _sceneLoad != null) { // move this? vid input?
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("LoadScene");
            _sceneLoad.allowSceneActivation = true;
            _loadTimer = 0;
            Loading = false;
            _sceneLoad = null;
            
        }
        
        
    }
}