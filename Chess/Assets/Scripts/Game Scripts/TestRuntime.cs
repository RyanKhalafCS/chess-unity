
/// <summary>
/// Small runtime flag used to switch the GameManager into test mode before
/// Unity calls Start/Awake on scene objects. Tests should set
/// TestRuntime.IsTesting = true before loading a scene.
/// </summary>
public static class TestRuntime
{
    public static bool IsTesting = false;
}
