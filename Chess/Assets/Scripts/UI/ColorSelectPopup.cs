using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ColorSelectPopup : MonoBehaviour
{
    [FormerlySerializedAs("messageText")] [SerializeField] private TextMeshProUGUI _messageText;

    [FormerlySerializedAs("whiteButton")] [SerializeField] private Button _whiteButton;
    [FormerlySerializedAs("blackButton")] [SerializeField] private Button _blackButton;

    private Action<bool> _onResult;

    private void Awake()
    {
        _whiteButton.onClick.AddListener(OnWhiteClicked);
        _blackButton.onClick.AddListener(OnBlackClicked);

        gameObject.SetActive(false);
    }

    public void Show(string message, Action<bool> callback)
    {
        _messageText.text = message;
        _onResult = callback;

        gameObject.SetActive(true);
    }

    private void OnWhiteClicked()
    {
        _onResult?.Invoke(true);
        gameObject.SetActive(false);
    }

    private void OnBlackClicked()
    {
        _onResult?.Invoke(false);
        gameObject.SetActive(false);
    }
}
