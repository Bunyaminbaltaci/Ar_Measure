using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(ARRaycastManager))]
public class Measurement_controler : MonoBehaviour
{
    [SerializeField]
    private GameObject measurementPoint;
    [SerializeField]
    private float measurementFactor=39.37f;
    [SerializeField]
    private Vector3 offsetmeasurement = Vector3.zero;
    [SerializeField]
    private GameObject Panel_UI;   
    [SerializeField]
    private Button Tamam;
    [SerializeField]
    private TextMeshPro distanceText;
    [SerializeField]
    private ARCameraManager arcameramanager ;
    [SerializeField]
    private LineRenderer MeasureLine;
    [SerializeField]
    private ARRaycastManager arRaycastManeger;
    private GameObject startpoint;
    private GameObject endpoint;
    private Vector2 TouchPosition=default;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    void Awake()
    {     
        startpoint= Instantiate(measurementPoint,Vector3.zero,Quaternion.identity);
        endpoint= Instantiate(measurementPoint,Vector3.zero,Quaternion.identity);    
        startpoint.SetActive(false);
        endpoint.SetActive(false);
      
        Tamam.onClick.AddListener(Dismiss);
        
    }
    // UI panelini kapatmak için
    private void Dismiss() => Panel_UI.SetActive(false);

    // nokta kontrolü
    private void OnEnable()
    {
        if (measurementPoint==null)
        {
            Debug.LogError("olcmek istenen noktalar eklenmedi");
            enabled = false;
        }

    }
    void Update()
    {
        // ilk noktayı belirlemek için 
        if (Input.touchCount>0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase==TouchPhase.Began)
            {
                TouchPosition = touch.position;
                if (arRaycastManeger.Raycast(TouchPosition,hits,UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds))
                {
                   
                    startpoint.SetActive(true);
                    Pose Hitpose = hits[0].pose;
                    startpoint.transform.SetPositionAndRotation(Hitpose.position,Hitpose.rotation);

                }
            }
            // ikinci nokta için
            if (touch.phase == TouchPhase.Moved)
            {
                TouchPosition = touch.position;
                
                if (arRaycastManeger.Raycast(TouchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds))
                {
                    MeasureLine.gameObject.SetActive(true);
                    endpoint.SetActive(true);

                    Pose hitPose = hits[0].pose;
                    endpoint.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
                }
            }
        }
        // 2 noktada aktif hale gelmiş ise aradaki çizgiyi ekliyor text'imizide son nokta üzerine ilerletiyor
        if (startpoint.activeSelf && endpoint.activeSelf)
        {
            distanceText.transform.position = endpoint.transform.position + offsetmeasurement;
            distanceText.transform.position = new Vector3(distanceText.transform.position.x, distanceText.transform.position.y+0.015f, distanceText.transform.position.z);
            distanceText.transform.rotation = endpoint.transform.rotation;
            MeasureLine.SetPosition(0,startpoint.transform.position);
            MeasureLine.SetPosition(1,endpoint.transform.position);
            distanceText.text=$"distance: {((Vector3.Distance(startpoint.transform.position, endpoint.transform.position) * measurementFactor)*2.54f).ToString("F2") } cm";
            // kullanılan F2 değerin noktadan osnra kaç basamak yazacağını belirtmek için 
        }
    }




     
}
