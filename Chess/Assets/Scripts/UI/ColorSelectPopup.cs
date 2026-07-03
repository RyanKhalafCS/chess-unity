using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private Button whiteButton;
    [SerializeField] private Button blackButton;

    private Action<bool> onResult;

    private void Awake()
    {
        whiteButton.onClick.AddListener(OnWhiteClicked);
        blackButton.onClick.AddListener(OnBlackClicked);

        gameObject.SetActive(false);
    }

    public void Show(string message, Action<bool> callback)
    {
        messageText.text = message;
        onResult = callback;

        gameObject.SetActive(true);
    }

    private void OnWhiteClicked()
    {
        onResult?.Invoke(true);
        gameObject.SetActive(false);
    }

    private void OnBlackClicked()
    {
        onResult?.Invoke(false);
        gameObject.SetActive(false);
    }
}
//using System;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;


//public class ColorSelectPopup : MonoBehaviour
//{
//    [SerializeField] private TextMeshProUGUI messageText;

//    [SerializeField] private Button whiteButton;
//    [SerializeField] private Button blackButton;

//    private Action<bool> onResult;

//    private void Awake()
//    {
//        whiteButton.onClick.AddListener(OnYesClicked);
//        blackButton.onClick.AddListener(OnNoClicked);
//        Debug.Log("Awake");
//        gameObject.SetActive(false);
//    }

//    private void Start()
//    {
//        Debug.Log("start");

//    }

//    public void Show(string message, Action<bool> callback)
//    {
//        Debug.Log(message);
//        messageText.text = message;

//        onResult = callback;

//        gameObject.SetActive(true);
//    }

//    private void OnYesClicked()
//    {
//        onResult?.Invoke(true);
//        gameObject.SetActive(false);
//    }

//    private void OnNoClicked()
//    {
//        onResult?.Invoke(false);
//        gameObject.SetActive(false);
//    }
//}
