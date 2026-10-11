#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class EditorWindowWithPopup : EditorWindow
{
    // Add menu item
    [MenuItem("Examples/Popup Example")]
    public static void Init()
    {
        EditorWindow window = EditorWindow.CreateInstance<EditorWindowWithPopup>();
        //window.maxSize.y = 20;
        //window.maxSize = new Vector2(100, 50); // Set the window size
        const int width = 200;
        const int height = 200;
        var x = (Screen.currentResolution.width - width) / 2;
        var y = (Screen.currentResolution.height - height) / 2;
        window.position = new Rect(x, y, width, height);//
        window.Show();
    }

    private Rect _buttonRect;
    private void OnGUI()
    {
        {
            GUILayout.Label("Go first, second, or AI vs AI?", EditorStyles.boldLabel);
            if (GUILayout.Button("First", GUILayout.Width(200)))
            {
                //PopupWindow.Show(buttonRect, new StartGame());
                ////PlayerManager.SetPlayers(Player.Color.White);
                this.Close();
                //PopupWindow.Destroy(this);//.close()?
                
            }else if (GUILayout.Button("Second", GUILayout.Width(200)))
            {
                //PopupWindow.Show(buttonRect, new StartGame());
                ////PlayerManager.SetPlayers(Player.Color.Black);
                this.Close();
                
            }else if (GUILayout.Button("AI vs AI", GUILayout.Width(200)))
            {
                //PopupWindow.Show(buttonRect, new StartGame());
                Debug.Log("AI vs AI GAME");
            }
            if (Event.current.type == EventType.Repaint)
                _buttonRect = GUILayoutUtility.GetLastRect();
        }
    }
}
#endif
