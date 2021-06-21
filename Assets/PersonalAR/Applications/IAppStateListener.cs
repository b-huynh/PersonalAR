public interface IAppStateListener
{
    void AppStart();
    void RenderStateChanged(bool newValue);
    void AppQuit();

    void OnActivityStart(ActivityEventData eventData);
    void OnActivityStop(ActivityEventData eventData);
}