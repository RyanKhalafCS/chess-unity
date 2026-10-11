#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class StartGame : PopupWindowContent
{
    private bool _toggle1 = true;
    private bool _toggle2 = true;
    private bool _toggle3 = true;

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 150);
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.Label("Popup Options Example", EditorStyles.boldLabel);
        _toggle1 = EditorGUILayout.Toggle("Toggle 1", _toggle1);
        _toggle2 = EditorGUILayout.Toggle("Toggle 2", _toggle2);
        _toggle3 = EditorGUILayout.Toggle("Toggle 3", _toggle3);
    }

    public override void OnOpen()
    {
        Debug.Log("Popup opened: " + this);
    }

    public override void OnClose()
    {
        Debug.Log("Popup closed: " + this);
    }
}
#endif
