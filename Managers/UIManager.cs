using TMPro;
using UnityEngine;
public class UIManager : MonoBehaviour {

    [SerializeField] private TMP_InputField scenetext;
    public void LoadGame() {
        if (DeviceManager.Instance.GetPlayerCount() != 2)
            return;

        DeviceManager.Instance.DisableLeaving();
        SceneManager.Instance.LoadScene("Copy LevelScene");
    }

    public void LoadCustomScene() {
        SceneManager.Instance.LoadScene(scenetext.text);
        Debug.Log("Tried Loading scene " + scenetext.text);
    }
    public void QuitGame() {
        Application.Quit();
    }
}