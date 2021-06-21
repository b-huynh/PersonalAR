using UnityEngine;

public abstract class BaseAppActivity : MonoBehaviour
{
    [Header("App Activity")]
    [ReadOnly]
    public AppState appState;
    // public abstract void Launch(AnchorableObject anchor = null);

    public abstract void StartActivity(ExecutionContext executionContext);
    public abstract void StopActivity(ExecutionContext executionContext);
}   