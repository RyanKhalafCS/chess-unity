using UnityEngine;

public class ExceptionHandler : Singleton<ExceptionHandler>
{
    public const int SEVERE_EXCEPTION = 1001;




    public static void ForceQuit()
    {
        #if UNITY_EDITOR
        // Exit Play Mode in Editor
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Quit the built application
        Application.Quit();
        #endif
    }
}