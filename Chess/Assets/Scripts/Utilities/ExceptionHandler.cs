using UnityEngine;

public class ExceptionHandler : Singleton<ExceptionHandler>
{
    public const int SEVERE_EXCEPTION = 1001;
    public const int GAMEOBJECT_NOT_FOUND_EXCEPTION = 1005;
    public const int COMPONENT_NOT_FOUND_EXCEPTION = 1006;

    //add param to detect exception type. need object not found exception.
    public static void PrintException(int ex)
    {
        Debug.Log("SEVERE EXCEPTION:");//add stack trace
        switch (ex)
        {
            case GAMEOBJECT_NOT_FOUND_EXCEPTION:
                Debug.Log("Game object not found!");
                break;
            case COMPONENT_NOT_FOUND_EXCEPTION:
                Debug.Log("Component could not be found!");
                break;
            default:
                //Specific message will be printed from function that threw exception.
                break;
        }

    }

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