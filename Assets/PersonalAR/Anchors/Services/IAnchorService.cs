using Microsoft.MixedReality.OpenXR.ARFoundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
//using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.WindowsMR;

using Microsoft.MixedReality.OpenXR;

namespace Microsoft.MixedReality.Toolkit.Extensions
{
	// public enum AnchorRegistrationPolicy { OVERWRITE, FIRST_IN }

	public interface IAnchorService : IMixedRealityExtensionService
	{
		// Expose service features and abilities here

		// Raised when AnchorStore and AnchorPointsSubsystem are set.
		event PropertyChangedEventHandler PropertyChanged;

		event Action<string> OnBeforeRegistered;
		event Action<AnchorableObject> OnAfterRegistered;
		event Action<AnchorableObject> OnBeforeRemoved;
		event Action<string> OnAfterRemoved;

		int AnchorCount { get; }
		IList<string> AnchorNames { get; }
		XRAnchorStore AnchorStore { get; }
		XRAnchorSubsystem AnchorPointsSubsystem { get; }
		bool AnchorStoreInitialized { get; }

		bool RegisterAnchor(string name, Vector3 position);
		void UnregisterAnchor(string name);
		bool ContainsAnchor(string name);
		AnchorableObject GetAnchor(string name);
		void Clear();

		void AddHandler(AnchorableObject anchor, AppState app);
		void RemoveHandler();
	}
}