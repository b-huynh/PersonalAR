using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Recaug
{
    // Defines the geometry of an object.
    public class ObjectGeometry
    {
        public CircularBuffer<PredPoint3D> points;
        // Unity geometry components like meshes and colliders.
        public List<GameObject> gameObjects;

        public Vector3 extentsMin;
        public Vector3 extentsMax;
        public Vector3 extentsCenter;

        public ObjectGeometry(int capacity)
        {
            points = new CircularBuffer<PredPoint3D>(capacity);
            gameObjects = new List<GameObject>(capacity);
        }
    }
    
    // ObjectRegistration contains all the info needed to represent a real object.
    public class ObjectRegistration : MonoBehaviour
    {
        public string className;
        public float confidence { get; private set; }

        public Vector3 position
        {
            get { return gameObject.transform.position; }
            set { gameObject.transform.position = value; }
        }

        public ObjectGeometry geometry;

        public void Start()
        {
            // this.geometry = new ObjectGeometry(Config.System.GeometryCapacity);
            this.geometry = new ObjectGeometry(60);
        }

        public void Update()
        {

        }

        public void Init(string className, float confidence, Vector3 position)
        {
            this.className = className;
            this.confidence = confidence;
            this.position = position;

            
            // GetComponentInChildren<UIFadable>().OnFadeFocusEnter += delegate {
            //     StatsTracker.Instance.LogHoverOn(className);
            // };

            // GetComponentInChildren<UIFadable>().OnFadeFocusExit += delegate {
            //     StatsTracker.Instance.LogHoverOff(className);
            // };
        }

        public void Destroy()
        {
            GameObject.Destroy(gameObject);
        }

        public void SetConfidence(float val)
        {
            confidence = val;
        }
    }

} // namespace Recaug