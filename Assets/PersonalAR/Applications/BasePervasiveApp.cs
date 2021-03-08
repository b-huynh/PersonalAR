using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Recaug;

using Microsoft.MixedReality.Toolkit.Utilities;

public static class PervasiveAppRegistry
{
    private static Dictionary<string, BasePervasiveApp> registry = new Dictionary<string, BasePervasiveApp>();

    public static IList<BasePervasiveApp> GetAllApps()
    {
        return registry.Values.ToList();
    }

    public static IList<PervasiveAppInfo> GetAllAppInfos()
    {
        return GetAllApps().Select(x => x.appInfo).ToList();
    }

    public static bool AddApp<T>(T appInstance) where T : BasePervasiveApp
    {
        if (appInstance == null || string.IsNullOrEmpty(appInstance.appId) ||
            string.IsNullOrEmpty(appInstance.appInfo.name))
        {
            // Adding a null instance is not supported.
            return false;
        }

        // T existingApp;
        // if (TryGetApp<T>(out existingApp, appInstance.appInfo.name))
        // {
        //     // App already exists
        //     return false;
        // }

        if (!registry.ContainsKey(appInstance.appId))
        {
            // Can't add duplicate app guid
            registry.Add(appInstance.appId, appInstance);
            return true;
        }

        return false;
    }

    // Try to get app based on the name. Returns first instance of app with that name.
    // if name is null or empty, app returns first instance of app type T.
    public static bool TryGetApp<T>(out T outInstance, string name = null) where T : BasePervasiveApp
    {
        outInstance = null;

        if (registry.ContainsKey(name))
        {
            outInstance = (T)registry[name];
            return true;
        }

        if (string.IsNullOrEmpty(name))
        {
            foreach(var app in registry.Values)
            {
                if (typeof(T).IsAssignableFrom(app.GetType()))
                {
                    outInstance = (T)app;
                    return true;
                }
            }
        }

        return false;
    }    
}

[System.Serializable]
public struct PervasiveAppInfo
{
    // public int id;
    public string name;
    public string description;
    public Material logo;
}

public class BasePervasiveApp : MonoBehaviour
{
    public string appId { get; private set; }
    
    [Header("Pervasive App Settings")]
    public PervasiveAppInfo appInfo;
    public bool Rendering;

    private Transform contentRoot;

    private List<IAppEntity> _appEntities;

    public System.Action<bool> OnRenderStateChanged;

    private void SetDefaultAppInfo()
    {
        appInfo.name = "DefaultAppName";
        appInfo.description = "Default application description";
        // appInfo.logo = new Material(Shader.Find("DefaultLogo"));
    }

    void Reset()
    {
        SetDefaultAppInfo();

        // Test that Content layer is there. If not, create one.
        if (transform.Find("ContentRoot") == null)
        {
            GameObject empty = new GameObject("ContentRoot");
            empty.transform.parent = this.transform;
        }
    }

    protected virtual void Awake()
    {
        contentRoot = transform.Find("ContentRoot");

        _appEntities = new List<IAppEntity>();

        // Add new app to app registry.
        appId = System.Guid.NewGuid().ToString();
        PervasiveAppRegistry.AddApp(this);
    }

    // Start is called before the first frame update
    void Start()
    {   
    }

    void Update()
    {
    }

    void LateUpdate()
    {
        // var renderers = contentRoot.GetComponentsInChildren<Renderer>();
        // var colliders = contentRoot.GetComponentsInChildren<Collider>();
        // var canvases = contentRoot.GetComponentsInChildren<Canvas>();
        // var mrtkLineRenderers = contentRoot.GetComponentsInChildren<MixedRealityLineRenderer>();
        // foreach (var r in renderers) { r.enabled = Rendering; }
        // foreach (var c in colliders) { c.enabled = Rendering; }
        // foreach (var c in canvases) { c.enabled = Rendering; }
        // foreach (var r in mrtkLineRenderers) { r.enabled = Rendering; }
    }

    public GameObject InstantiateInApp(GameObject original)
    {
        GameObject clone = GameObject.Instantiate(original, contentRoot.transform);
        return clone;
    }

    public void SetRenderState(bool value)
    {
        if (Rendering != value)
        {
            Rendering = value;
            OnRenderStateChanged?.Invoke(Rendering);
        }
    }

    public void ToggleRenderState()
    {
        SetRenderState(!Rendering);
    }
}
