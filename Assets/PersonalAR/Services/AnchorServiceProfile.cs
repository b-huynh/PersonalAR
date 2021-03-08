using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Extensions
{
	[MixedRealityServiceProfile(typeof(IAnchorService))]
	[CreateAssetMenu(fileName = "AnchorServiceProfile", menuName = "MixedRealityToolkit/AnchorService Configuration Profile")]
	public class AnchorServiceProfile : BaseMixedRealityProfile
	{
		// Store config data in serialized fields
		public GameObject AnchorActorPrefab => anchorActorPrefab;

        [SerializeField]
        [Tooltip("The default prefab used to indicate anchor location")]
        private GameObject anchorActorPrefab = null;
	}
}