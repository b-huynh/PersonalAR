using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.WindowsMR;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.MixedReality.Toolkit.Extensions
{

	[MixedRealityExtensionService(SupportedPlatforms.WindowsUniversal|SupportedPlatforms.WindowsEditor)]
	public class AnchorService : BaseExtensionService, IAnchorService, IMixedRealityExtensionService
	{
		//data collection
		public static List<ObjectPosition> objectPositions = new List<ObjectPosition>();
		/*
			Anchor Store Management
		*/

		public int AnchorCount
		{
			get
			{
				if (AnchoredObjects == null)
				{
					return 0;
				}
				else
				{
					return AnchoredObjects.Count;
				}
			}
			private set {}
		}

		public IList<string> AnchorNames
		{
			get
			{
				if (AnchoredObjects == null)
				{
					return new List<string>();
				}
				else
				{
					return new List<string>(_anchoredObjects.Keys);
				}
			}
			private set {}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	
		private XRAnchorSubsystem _anchorPointSubsystem;
		public XRAnchorSubsystem AnchorPointsSubsystem
		{
			get { return _anchorPointSubsystem; }
			private set
			{
				_anchorPointSubsystem = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnchorPointsSubsystem)));
			}
		}

		private XRAnchorStore _anchorStore;
		public XRAnchorStore AnchorStore
		{
			get { return _anchorStore; }
			private set
			{
				_anchorStore = value;

				if (AnchorPointsSubsystem == null)
				{
					// ARDebug.Log($"[{GetType()}] Reference Point Subsystem not initialized.");
					ARDebug.Log($"Reference Point Subsystem not initialized.");
				}

				if (AnchorStore != null)
				{
					// ARDebug.Log($"[{GetType()}] Anchor store initialized.");
					ARDebug.Log($"Anchor store initialized.");
				}
	
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnchorStore)));
			}
		}

		public bool AnchorStoreInitialized
		{
			get => AnchorPointsSubsystem != null && AnchorStore != null;
			private set {}
		}

		private Dictionary<string, AnchorableObject> _anchoredObjects;
		public Dictionary<string, AnchorableObject> AnchoredObjects
		{
			get => _anchoredObjects;
			private set => _anchoredObjects = value;
		}

		public event Action<AnchorableObject> OnRegistered;
		public event Action<string> OnRemoved;
/*		public event Action<string> ToRemove;*/

		private string _editorAnchorSaveFile = Path.Combine(Application.persistentDataPath, "editor_anchors.json");

		/*
			Mixed Reality Toolkit Service
		*/
		private AnchorServiceProfile anchorServiceProfile;

		public AnchorService(string name,  uint priority,  BaseMixedRealityProfile profile) : base(name, priority, profile) 
		{
			anchorServiceProfile = (AnchorServiceProfile)profile;
		}

		/*
			Service implementation
		*/
		public override void Initialize()
		{
			base.Initialize();
		}

		public override async void Enable()
		{
			base.Enable();

			// Add handler function where when AnchorStore and Reference Point Subsystem are loaded.
			PropertyChanged += OnPropertyChanged;

			// Do service initialization here.
			AnchoredObjects = new Dictionary<string, AnchorableObject>();

			// TODO: Replace w/ ARAnchorManager as directly acessing rp subsystems is deprecated.
#if WINDOWS_UWP
			// Initialized the (1) Reference Point Subsystem and (2) XRAnchorStore
			AnchorPointsSubsystem = CreateReferencePointSubSystem();
			// AnchorStore = await _anchorPointSubsystem.TryGetAnchorStoreAsync();
			AnchorStore = await XRAnchorStore.LoadAsync(AnchorPointsSubsystem);
#else
			// Can only initialize Anchor Subsystems on device.
			AnchorPointsSubsystem = null;
			AnchorStore = null;
#endif

#if UNITY_EDITOR
			Debug.Log($"Editor Anchor File: {_editorAnchorSaveFile}");
#endif
		}

		private bool StartHasRun = false;
		private void Start()
		{
			try
			{
				LoadExistingAnchors();
			}
			catch(Exception ex)
			{
				ARDebug.LogException(ex);
			}
			StartHasRun = true;
		}

		public override void Update()
		{
			base.Update();

			// To be used for communication after scene objects have been initialized, but prior to first frame update.
			if (StartHasRun == false)
			{
				Start();
			}

			// Do service updates here.
		}

        public override void Disable()
        {
			AnchoredObjects.Clear();
            base.Disable();
        }

        public override void Destroy()
        {
			if (AnchorPointsSubsystem != null)
			{
				AnchorPointsSubsystem.Stop();
			}
			if (AnchorStore != null)
			{
				// AnchorStore.Dispose();
				AnchorStore.Clear();
			}

            base.Destroy();
        }

		/*
			Interface Implementation
			data for anchor
		*/
		public bool RegisterAnchor(string name, Vector3 position)
		{
			// Create object model (for debug/visualization purposes)

			GameObject newAnchoredObject = CreateAnchorActor(name);
			newAnchoredObject.transform.position = position;

			var anchor = newAnchoredObject.GetComponent<AnchorableObject>();
			bool returnValue = false;
#if UNITY_EDITOR
			// In editor we ignore the anchor store.
			AnchoredObjects.Add(name, anchor);
			//add anchor for data collection
			ObjectPosition current_data = new ObjectPosition();
			current_data.position = position;
			current_data.objectName = name;
			current_data.unixTime = Utils.UnixTimestampMilliseconds();
			// Debug.Log("SAVE " + name);
			objectPositions.Add(current_data);
			returnValue = true;

			// Experimental write to save file for editor emulation purposes
			File.WriteAllText(_editorAnchorSaveFile, SerializeAnchors());
#endif

#if WINDOWS_UWP
			// Attempt to save anchor at provided location
			if (anchor.SaveAnchor())
			{
				// Add to list tracking anchored game objects
				AnchoredObjects.Add(name, anchor);
				ARDebug.Log($"Saved anchor {name}");
				returnValue = true;

				//add anchor for data collection
				ObjectPosition current_data = new ObjectPosition();
				current_data.position = position;
				current_data.objectName = name;
				current_data.unixTime = Utils.UnixTimestampMilliseconds();
				// Debug.Log("SAVE " + name);
				objectPositions.Add(current_data);
			}
			else
			{
				// Could not save, so destroy the newly created game object
				GameObject.Destroy(newAnchoredObject);
				ARDebug.Log($"Failed to save anchor {name}");
				returnValue = false;
			}
#endif
			// If anchor was succesfully registered, invoke OnRegistered callbacks
			if (returnValue)
			{
				OnRegistered?.Invoke(anchor);
			}
			return returnValue;
		}

		public void UnregisterAnchor(string name)
		{
			//Need to invoke event prior to removing anchor because the event depends on the existence of the anchor
			AnchorableObject anchor = GetAnchor(name);
			RemoveFromDict(anchor);

#if WINDOWS_UWP
			AnchorStore.UnpersistAnchor(name);
			GameObject.Destroy(AnchoredObjects[name].gameObject);
			AnchoredObjects.Remove(name);

			objectPositions.ForEach(delegate(ObjectPosition ob){
				if(ob.objectName == name)
				{
					objectPositions.Remove(ob);
				}
	     	});
#endif

#if UNITY_EDITOR
			GameObject.Destroy(AnchoredObjects[name].gameObject);
			AnchoredObjects.Remove(name);
			// Experimental write to save file for editor emulation purposes
			File.WriteAllText(_editorAnchorSaveFile, SerializeAnchors());
#endif

            OnRemoved?.Invoke(name);

		}

		public bool ContainsAnchor(string name)
		{
			return AnchoredObjects.ContainsKey(name);
		}

		public AnchorableObject GetAnchor(string name)
		{
			AnchorableObject retval = null;
			if (ContainsAnchor(name))
			{
				retval = AnchoredObjects[name];
			}
			return retval;
		}

		public void Clear()
		{
			List<String> anchorNames = new List<String>(AnchoredObjects.Keys);
			foreach(String name in anchorNames)
			{
				UnregisterAnchor(name);
			}

		}

		/*
			Private helpers
		*/
		private XRAnchorSubsystem CreateReferencePointSubSystem()
		{
			List<XRAnchorSubsystemDescriptor> rpSubSystemsDescriptors = new List<XRAnchorSubsystemDescriptor>();
			SubsystemManager.GetSubsystemDescriptors(rpSubSystemsDescriptors);
	
			string descriptors = "";
			foreach (var descriptor in rpSubSystemsDescriptors)
			{
				descriptors += $"{descriptor.id} \r\n";
				// {descriptor.subsystemImplementationType}
			}
	
			ARDebug.Log($"[AnchorService] Found {rpSubSystemsDescriptors.Count} reference point subsystem descriptor:\r{descriptors}");
			Debug.Log($"[AnchorService] Found {rpSubSystemsDescriptors.Count} reference point subsystem descriptor:\r{descriptors}");

			XRAnchorSubsystem rpSubSystem = null;
			if (rpSubSystemsDescriptors.Count > 0)
			{
				rpSubSystem = rpSubSystemsDescriptors[0].Create();
				rpSubSystem.Start();
			}
	
			return rpSubSystem;
		}

		private void OnPropertyChanged(System.Object sender, PropertyChangedEventArgs eventArgs)
		{
			// Load existing anchors once AnchorStore is initialized.
			if (eventArgs.PropertyName == nameof(AnchorStore))
			{
				if (AnchorStore == null)
				{
					// This means we're likely running in editor.
					ARDebug.LogFormat("Cannot load existing anchors, no anchor store found");
					return;
				}
				else
				{
					LoadExistingAnchors();
				}
			}
		}

		//data anchor
		private void LoadExistingAnchors()
		{
#if UNITY_EDITOR
			// Experimental editor emulation of anchor save and load
			string editorAnchorsJson = File.ReadAllText(_editorAnchorSaveFile);
			ARDebug.Log($"Reading anchors from {_editorAnchorSaveFile}");
			DeserializeAnchors(editorAnchorsJson);
#endif

#if WINDOWS_UWP
			// AnchorStore is null when running on start...
			var existingAnchors = AnchorStore.PersistedAnchorNames;
			ARDebug.LogFormat("Found {0} existing anchor store anchors", existingAnchors.Count);

			foreach (string anchorName in existingAnchors)
			{
				GameObject anchorMesh = CreateAnchorActor(anchorName);
				AnchorableObject anchor = anchorMesh.GetComponent<AnchorableObject>();
				
				anchor.OnAnchorLoaded.AddListener(
					delegate 
					{
						ARDebug.LogFormat("Loaded anchor {0}", anchorName);
						AnchoredObjects.Add(anchorName, anchor);
						OnRegistered?.Invoke(anchor);

						//may be a bug
						ObjectPosition current_data = new ObjectPosition();
						current_data.position = anchorMesh.transform.position;
						current_data.objectName = anchorName;
						current_data.unixTime = Utils.UnixTimestampMilliseconds();
						// Debug.Log("Add " + current_data.objectName);
						objectPositions.Add(current_data);
					}
				);
				if (!anchor.LoadAnchor())
				{
					ARDebug.LogFormat("Unable to load anchor {0}", anchorName);
				}
			}
#endif
		}

		private GameObject CreateAnchorActor(string name)
		{
			GameObject newAnchor = GameObject.Instantiate(anchorServiceProfile.AnchorActorPrefab);
			newAnchor.name += $" [{name}]";

			AnchorableObject anchor = newAnchor.GetComponent<AnchorableObject>();
			anchor.WorldAnchorName = name;

			// newAnchor.GetComponent<IAnchorable>().SetAnchor(anchor);
			foreach(IAnchorable anchorView in newAnchor.GetComponentsInChildren<IAnchorable>())
			{
				anchorView.Anchor = anchor;
			}

			newAnchor.SetActive(true);
			return newAnchor;
		}

		private string SerializeAnchors()
		{
			JArray anchoredObjectsJson = new JArray();

			foreach(var kv in AnchoredObjects)
			{
				string anchorName = kv.Key;
				AnchorableObject anchor = kv.Value;
				Pose anchorPose = new Pose(anchor.transform.position, anchor.transform.rotation);
				
				// Create JObject representation of anchor
				JObject anchorJson = new JObject();
				anchorJson.Add("anchorName", anchorName);
				anchorJson.Add("pose", JsonUtility.ToJson(anchorPose));

				// Add to anchor collection JArray
				anchoredObjectsJson.Add(anchorJson);
			}

			return anchoredObjectsJson.ToString();
		}

		private void DeserializeAnchors(string json)
		{
			JArray anchoredObjectsJson = JArray.Parse(json);

			foreach(JObject anchorJson in anchoredObjectsJson)
			{
				string anchorName = (string)anchorJson["anchorName"];
				string serializedPose = (string)anchorJson["pose"];
				Pose anchorPose = JsonUtility.FromJson<Pose>(serializedPose);

				// Create new anchor object
				GameObject newAnchoredObject = CreateAnchorActor(anchorName);
				newAnchoredObject.transform.position = anchorPose.position;
				newAnchoredObject.transform.rotation = anchorPose.rotation;

#if UNITY_EDITOR
				var anchor = newAnchoredObject.GetComponent<AnchorableObject>();
				AnchoredObjects.Add(anchorName, anchor);
				OnRegistered?.Invoke(anchor);

				ObjectPosition current_data = new ObjectPosition();
				current_data.position = anchorPose.position;
				current_data.objectName = anchorName;
				current_data.unixTime = Utils.UnixTimestampMilliseconds();
				// Debug.Log("Add " + current_data.objectName);
				objectPositions.Add(current_data);

#endif
			}
		}

		// Manage anchor handlers.
		public Dictionary<AnchorableObject, HashSet<AppState>> handlers = new Dictionary<AnchorableObject, HashSet<AppState>>();
		public Dictionary<AppState, HashSet<AnchorableObject>> handlersByApp = new Dictionary<AppState, HashSet<AnchorableObject>>();
		public void AddHandler(AnchorableObject anchor, AppState app)
		{
			if (handlers.ContainsKey(anchor) == false)
			{
				handlers.Add(anchor, new HashSet<AppState>());
			}
			handlers[anchor].Add(app);

			if (handlersByApp.ContainsKey(app) == false)
			{
				handlersByApp.Add(app, new HashSet<AnchorableObject>());
			}
			handlersByApp[app].Add(anchor);
		}

		public void RemoveFromDict(AnchorableObject anchor)
		{

			foreach (KeyValuePair<AnchorableObject, HashSet<AppState>> kvp in handlers)
			{
				Debug.Log("kvp: " + kvp.Key + " " + kvp.Value);
				if (kvp.Key == anchor)
				{
					Debug.Log(" == ");
					kvp.Value.Clear();
				}
			}

			foreach (KeyValuePair<AppState, HashSet<AnchorableObject>> kvp in handlersByApp)
            {
				Debug.Log("kvp: " + kvp.Key + " " + kvp.Value);
				if (kvp.Value.Contains(anchor))
                {
					Debug.Log(" Contains ");
					kvp.Value.Remove(anchor);
				}
            }
		}

		public void RemoveHandler()
		{
			Debug.Log("AnchorService RemoveHandler");
		}

		public static List<ObjectPosition> getObjects()
		{
			return objectPositions;
		}

	}
}

[System.Serializable]
public class ObjectPosition
{
	public long unixTime;
	public long placedTime;
	public string objectName;
	public Vector3 position;
}
