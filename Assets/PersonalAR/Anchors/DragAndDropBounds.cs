using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
public class DragAndDropBounds : BaseInputHandler, IMixedRealityPointerHandler
{
    [SerializeField]
    [Tooltip("Whether input events should be marked as used after handling so other handlers in the same game object ignore them")]
    private bool MarkEventsAsUsed = false;

    public BaseEntityEvent OnEntityDropped = new BaseEntityEvent();

    // For determining drag & drop state.
    private bool _entityInBounds;
    private bool EntityInBounds
    {
        get => _entityInBounds;
        set
        {
            _entityInBounds = value;
            GetComponent<MeshRenderer>().enabled = _entityInBounds;
        }
    }
    private Vector3 _pointerPosition;
    private BaseEntity _draggedEntity;

    #region MonoBehaviour Implementation

    void Reset()
    {
        SetInitialComponentValues();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetInitialComponentValues();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void SetInitialComponentValues()
    {
        GetComponent<Collider>().isTrigger = true;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        IsFocusRequired = false;
    }

    void OnTriggerStay(Collider other)
    {
        Bounds bounds = GetComponent<Collider>().bounds;
        if (bounds.Contains(_pointerPosition))
        {
            BaseEntity otherEntity = other.GetComponentInParent<BaseEntity>();
            if (otherEntity != null)
            {
                EntityInBounds = true;
                _draggedEntity = other.GetComponentInParent<BaseEntity>();
            }
        }
        else
        {
            EntityInBounds = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        EntityInBounds = false;
    }

    #endregion MonoBehaviour Implementation

    #region InputSystemGlobalHandlerListener Implementation

    protected override void RegisterHandlers()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);
    }

    protected override void UnregisterHandlers()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
    }

    #endregion InputSystemGlobalHandlerListener Implementation

    #region IMixedRealityPointerHandler

    void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData) {}

    void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
    {
        if (!eventData.used)
        {
            if (EntityInBounds)
            {
                OnEntityDropped.Invoke(_draggedEntity);
                if (MarkEventsAsUsed)
                {
                    eventData.Use();
                }
            }
        }
    }

    void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) {}

    void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        if (!eventData.used)
        {
            if (eventData.Pointer.BaseCursor != null)
            {
                _pointerPosition = eventData.Pointer.BaseCursor.Position;
                if (MarkEventsAsUsed)
                {
                    eventData.Use();
                }
            }
        }
    }

    #endregion IMixedRealityPointerHandler

    public void TestOnEntityDropped(BaseEntity entity)
    {
        ARDebug.Log($"Dropped entity {entity.name} to bounds {this.name}");
    }


    // private void DropEntity(BaseEntity entity)
    // {
    //     if (EntityInBounds)
    //     {
    //         Anchorable anchorableTarget = entity.GetComponent<Anchorable>();
    //         if (anchorableTarget != null)
    //         {
    //             anchorableTarget.Anchor = this.GetComponentInParent<AnchorableObject>();

    //             anchorableTarget.transform.parent = _appContentParent.transform;
    //             anchorableTarget.transform.position = Vector3.zero;
    //             anchorableTarget.transform.rotation = Quaternion.identity;

    //             _appContentParent.UpdateCollection();

    //             EntityInBounds = false;
    //         }
    //     }
    // }
}
