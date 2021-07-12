using UnityEngine;
using UnityEditor;

public class ApplicationsWindow : EditorWindow
{
    private AppState[] apps;

    [MenuItem("Window/Applications Window")]
    public static void Init()
    {
        EditorWindow.GetWindow(typeof(ApplicationsWindow));
    }

    public void Awake()
    {

    }

    void OnGUI()
    {
        // EditorGUILayout.LabelField(apps.Length.ToString());
        // GUILayout.BeginVertical();
        // foreach(AppState app in apps)
        // {
        //     var objectEditor = Editor.CreateEditor(app);
        //     objectEditor.DrawDefaultInspector();
        // }
        // GUILayout.EndVertical();

        string[] results = AssetDatabase.FindAssets("t:AppState");
        GUILayout.BeginHorizontal();
        foreach(string guid in results)
        {
            GUILayout.BeginScrollView(Vector2.zero, GUILayout.Width(300), GUILayout.Height(300));
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AppState app = (AppState)AssetDatabase.LoadAssetAtPath<AppState>(assetPath);
            // AppStateInspector appEditor = (AppStateInspector)Editor.CreateEditor(app);
            var objectEditor = Editor.CreateEditor(app);
            objectEditor.OnInspectorGUI();
            // EditorGUILayout.LabelField(assetPath);
            GUILayout.EndScrollView();
        }
        GUILayout.EndHorizontal();
    }
}
