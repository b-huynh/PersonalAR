using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetEarthMainActivity : BaseAppActivity
{
    [Range(0.1f, 3.0f)]
    public float launchDistance;

    [SerializeField] private GameObject entityToLaunch;
    [SerializeField] private GameObject locationMarker;
    [SerializeField] private List<CityCoords> cities = new List<CityCoords>();
    protected GameObject cachedEntity;
    //protected GameObject marker;
    private List<GameObject> markerList = new List<GameObject>();
    private float radius = 20; 
    //private float latitude = 51.5072f; // lat
    //private float longitude = -0.1275f; // long

    [Header("Experiment Parameters")]
    [SerializeField] private RandomPinCodes pinCodes;

    void Reset()
    {
        launchDistance = 1.0f;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // TODO: This is awful. Don't poll every frame and don't hardcode child hierarchy.
        for(int i = 0; i < markerList.Count; ++i)
        {
            markerList[i].transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshPro>().text = cities[i].getDescription();
        }
    }

    void OnDestroy()
    {
        Destroy(cachedEntity);
    }

    public override void StartActivity(ExecutionContext executionContext)
    {
        if (initialized == false)
        {
            // Calculate a launch point set distance away from users current forward position.
            Vector3 forwardDelta = Camera.main.transform.forward * launchDistance;
            Vector3 launchPoint = Camera.main.transform.position + forwardDelta;
            launchPoint.y = Mathf.Max(0.35f, launchPoint.y);

            cachedEntity = GameObject.Instantiate(entityToLaunch, this.transform);
            //marker = GameObject.Instantiate(locationMarker, this.transform);
            cachedEntity.transform.SetPositionAndRotation(launchPoint, Quaternion.identity);
            //marker.transform.parent = cachedEntity.transform;

            for (int i = 0; i < cities.Count; i++)
            {
                GameObject marker = GameObject.Instantiate(locationMarker, this.transform);
                markerList.Add(marker);
                marker.transform.parent = cachedEntity.transform;

                CityCoords city = cities[i];
                string name = city.getCityName();
                float latitude = city.getLatitude();
                float longitude = city.getLongitude();
                marker.transform.Rotate(0, -1 * longitude, -1 * latitude , Space.Self);
                float latitudeRadians = Mathf.PI * latitude / 180;
                float longitudeRadians = Mathf.PI * longitude / 180;

                // adjust position by radians	
                latitudeRadians -= 1.570795765134f; // subtract 90 degrees (in radians)
                                                    // and switch z and y (since z is forward)
                float xPos = (radius) * Mathf.Sin(latitudeRadians) * Mathf.Cos(longitudeRadians);
                float zPos = (radius) * Mathf.Sin(latitudeRadians) * Mathf.Sin(longitudeRadians);
                float yPos = (radius) * Mathf.Cos(latitudeRadians);

                //Vector3 markerPosition = new Vector3(launchPoint.x + xPos, launchPoint.y + yPos, launchPoint.z + zPos);
                Vector3 markerPosition = new Vector3(xPos, yPos, zPos);
                // move marker to position
                marker.transform.localPosition = markerPosition;
                //marker.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
                TextMeshPro cityText = marker.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshPro>();
                cityText.text = city.getDescription();
                // cityText.text = name + "\n" + latitude + ", " + longitude;

                // Add code piece
                if (pinCodes != null)
                {
                    // var assignableObjects = new List<CityCoords>() {cities[i]};
                    // var assignment = pinCodes.AssignCodePieces(assignableObjects, 2);
                    // var assignment = pinCodes.GetAssignment(assignableObjects, 2);
                    city.InitWithCodes(pinCodes);
                    cityText.text = city.getDescription();
                    // var codePiece = assignment[cities[i]];
                    // cityText.text += $"\n (CODE) {codePiece.Label}-{codePiece.Value}";
                }
            }

            initialized = true;
        }

        markerList.ForEach(marker => marker.SetActive(true));
        cachedEntity.SetActive(true);
    }
    public override void StopActivity(ExecutionContext executionContext)
    {
        markerList.ForEach(marker => marker.SetActive(false));
        cachedEntity.SetActive(false);

        // for (int i = 0; i < markerList.Count; i++)
        // {
        //     Destroy(markerList[i]);
        // }
        // if (cachedEntity != null) 
        // {
        //     Destroy(cachedEntity);
        //     cachedEntity = null;
        // }
    }

    [Serializable]
    class CityCoords
    {
        [SerializeField] private string cityName;
        [SerializeField] private string description;
        [SerializeField] private float latitude;
        [SerializeField] private float longitude;

        // For tracking code pieces assigned to this object
        private RandomPinCodes codeSet;
        private CodePiece codePiece;

        public CityCoords(string cityName, float latitude, float longitude)
        {
            this.cityName = cityName;
            this.latitude = latitude;
            this.longitude = longitude;
            this.description = cityName + "\n" + latitude + ", " + longitude;
        }

        ~CityCoords()
        {
            if (codePiece != null)
            {
                codePiece.OnCodeEntryComplete.RemoveListener(this.OnCodeEntryComplete);
            }
        }

        public string getCityName()
        {
            return this.cityName;
        }

        public string getDescription()
        {
            return this.description;
        }

        public float getLatitude()
        {
            return this.latitude;
        }

        public float getLongitude()
        {
            return this.longitude;
        }

        public void InitWithCodes(RandomPinCodes codes)
        {
            codeSet = codes;
            codePiece = codeSet.GetAssignment(this, 2);
            this.description = cityName + "\n" + latitude + ", " + longitude;
            this.description += $"\n (CODE) {codePiece.Label}-{codePiece.Value}";

            codePiece.OnCodeEntryComplete.AddListener(this.OnCodeEntryComplete);
        }

        private void OnCodeEntryComplete()
        {
            codePiece.OnCodeEntryComplete.RemoveListener(this.OnCodeEntryComplete);
            // Debug.Log("Planet Earth code entry complete");
            InitWithCodes(codeSet);
        }
    }

 }
