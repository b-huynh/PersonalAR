using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface employed by any scripts whose behaviours rely on an anchored object.
public interface IAnchorable
{
    // The name of the anchor this object is associated with.
    // string AnchorName { get; }
    // void SetAnchor(AnchorableObject anchor);

    AnchorableObject Anchor { get; set; }

    bool Anchored { get; }
}
