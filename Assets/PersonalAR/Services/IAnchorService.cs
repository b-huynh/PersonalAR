using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.WindowsMR;

using Recaug;

namespace Microsoft.MixedReality.Toolkit.Extensions
{
	// public enum AnchorRegistrationPolicy { OVERWRITE, FIRST_IN }

	public interface IAnchorService : IMixedRealityExtensionService
	{
		// Expose service features and abilities here
		// event Action<ObjectRegistration> OnRegistered;
		// event Action<ObjectRegistration> OnRemoved;

		// Raised when AnchorStore and AnchorPointsSubsystem are set.
		event PropertyChangedEventHandler PropertyChanged;

		event Action<AnchorableObject> OnRegistered;
		event Action<AnchorableObject> OnRemoved;

		int AnchorCount { get; }
		IList<string> AnchorNames { get;}
		XRAnchorStore AnchorStore { get; }
		XRReferencePointSubsystem AnchorPointsSubsystem { get; }
		bool AnchorStoreInitialized { get; }

		bool RegisterAnchor(string name, Vector3 position);
		void UnregisterAnchor(string name);
		bool ContainsAnchor(string name);
		AnchorableObject GetAnchor(string name);
		void Clear();
	}
}