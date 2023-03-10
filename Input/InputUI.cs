using TMPro;
using UnityEngine;

public class InputUI : MonoBehaviour
{
    [SerializeField] private string _cleverNoInputDeviceText;
    [SerializeField] private int _playerIndex;
    [SerializeField] private TMP_Text _inputText;

    private void OnEnable() => DeviceManager.Instance.OnNewInput += UpdateUI;
    private void OnDisable() => DeviceManager.Instance.OnNewInput -= UpdateUI;

    private void Start() {
        UpdateUI();
    }
    private void UpdateUI() {
        var playerInfo = DeviceManager.Instance.GetPlayerInput(_playerIndex);

        if (playerInfo != null) {
            var controlSchemeText = playerInfo.currentControlScheme;
            _inputText.text = controlSchemeText;
        } else _inputText.text = _cleverNoInputDeviceText;
    }
}
