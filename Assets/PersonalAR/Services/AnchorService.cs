using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.WindowsMR;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Recaug;

namespace Microsoft.MixedReality.Toolkit.Extensions
{
	[MixedRealityExtensionService(SupportedPlatforms.WindowsUniversal|SupportedPlatforms.WindowsEditor)]
	public class AnchorService : BaseExtensionService, IAnchorService, IMixedRealityExtensionService
	{
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
	
		private XRReferencePointSubsystem _anchorPointSubsystem;
		public XRReferencePointSubsystem AnchorPointsSubsystem
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
					// Debug.Log($"[{GetType()}] Reference Point Subsystem not initialized.");
					Debug.Log($"Reference Point Subsystem not initialized.");
				}

				if (AnchorStore != null)
				{
					// Debug.Log($"[{GetType()}] Anchor store initialized.");
					Debug.Log($"Anchor store initialized.");
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
		public event Action<AnchorableObject> OnRemoved;

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

			// Initialized the (1) Reference Point Subsystem and (2) XRAnchorStore
			AnchorPointsSubsystem = CreateReferencePointSubSystem();
			AnchorStore = await _anchorPointSubsystem.TryGetAnchorStoreAsync();
		}

		public override void Update()
		{
			base.Update();

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
				AnchorStore.Dispose();
			}

            base.Destroy();
        }

		/*
			Interface Implementation
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
				Debug.Log($"Saved anchor {name}");
				returnValue = true;
			}
			else
			{
				// Could not save, so destroy the newly created game object
				GameObject.Destroy(newAnchoredObject);
				Debug.Log($"Failed to save anchor {name}");
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
			AnchorStore.UnpersistAnchor(name);
			GameObject.Destroy(AnchoredObjects[name].gameObject);
			AnchoredObjects.Remove(name);
#if UNITY_EDITOR
			// Experimental write to save file for editor emulation purposes
			File.WriteAllText(_editorAnchorSaveFile, SerializeAnchors());
#endif
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
			AnchorStore.Clear();
			foreach(var kv in AnchoredObjects)
			{
				GameObject.Destroy(kv.Value.gameObject);
			}
			AnchoredObjects.Clear();
		}

		/*
			Private helpers
		*/
		private XRReferencePointSubsystem CreateReferencePointSubSystem()
		{
			List<XRReferencePointSubsystemDescriptor> rpSubSystemsDescriptors = new List<XRReferencePointSubsystemDescriptor>();
			SubsystemManager.GetSubsystemDescriptors(rpSubSystemsDescriptors);
	
			string descriptors = "";
			foreach (var descriptor in rpSubSystemsDescriptors)
			{
				descriptors += $"{descriptor.id} {descriptor.subsystemImplementationType}\r\n";
			}
	
			Debug.Log($"[{GetType()}] {rpSubSystemsDescriptors.Count} reference point subsystem descriptors:\r\n{descriptors}");
	
			XRReferencePointSubsystem rpSubSystem = null;
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
					Debug.LogFormat("Cannot load existing anchors, no anchor store found");
					return;
				}
				else
				{
					LoadExistingAnchors();
				}
			}
		}

		private void LoadExistingAnchors()
		{
#if UNITY_EDITOR
			// Experimental editor emulation of anchor save and load
			string editorAnchorsJson = File.ReadAllText(_editorAnchorSaveFile);
			Debug.Log($"Reading anchors from {_editorAnchorSaveFile}");
			DeserializeAnchors(editorAnchorsJson);
#endif

			var existingAnchors = AnchorStore.PersistedAnchorNames;
			Debug.LogFormat("Found {0} existing anchor store anchors", existingAnchors.Count);

			foreach (string anchorName in existingAnchors)
			{
				GameObject anchorMesh = CreateAnchorActor(anchorName);
				AnchorableObject anchor = anchorMesh.GetComponent<AnchorableObject>();
				anchor.OnAnchorLoaded.AddListener(
					delegate 
					{
						Debug.LogFormat("Loaded anchor {0}", anchorName);
						AnchoredObjects.Add(anchorName, anchor);
						OnRegistered?.Invoke(anchor);
					}
				);
				if (!anchor.LoadAnchor())
				{
					Debug.LogFormat("Unable to load anchor {0}", anchorName);
				}
			}
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
#endif
			}
		}
	}
}
