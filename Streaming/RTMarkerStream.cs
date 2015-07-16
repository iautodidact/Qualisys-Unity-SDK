﻿// Unity SDK for Qualisys Track Manager. Copyright 2015 Qualisys AB
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using QTMRealTimeSDK;

namespace QualisysRealTime.Unity
{
    public class RTMarkerStream : MonoBehaviour
    {
        private List<LabeledMarker> markerData;
        private RTClient rtClient;
        private GameObject markerRoot;
        private List<GameObject> markers;

        public bool visibleMarkers = true;

        public float markerScale = 0.01f;

        private bool streaming = false;

        // Use this for initialization
        void Start()
        {
            rtClient = RTClient.GetInstance();
            markers = new List<GameObject>();
            markerRoot = this.gameObject;
        }


        private void InitiateMarkers()
        {
            foreach (var marker in markers)
            {
                UnityEngine.GameObject.Destroy(marker);
            }

            markers.Clear();
            markerData = rtClient.Markers;

            for (int i = 0; i < markerData.Count; i++)
            {
                GameObject newMarker = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newMarker.name = markerData[i].Label;
                newMarker.transform.parent = markerRoot.transform;
                newMarker.transform.localScale = Vector3.one * markerScale;
                newMarker.SetActive(false);
                markers.Add(newMarker);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (rtClient.GetStreamingStatus() && !streaming)
            {
                InitiateMarkers();
                streaming = true;
            }
            if (!rtClient.GetStreamingStatus() && streaming)
            {
                streaming = false;
                InitiateMarkers();
            }

            markerData = rtClient.Markers;

            if (markerData == null && markerData.Count == 0)
                return;

            if (markers.Count != markerData.Count)
            {
                InitiateMarkers();
            }

            for (int i = 0; i < markerData.Count; i++)
            {
                if (markerData[i].Position.magnitude > 0)
                {
                    markers[i].name = markerData[i].Label;
                    markers[i].GetComponent<Renderer>().material.color = markerData[i].Color;
                    markers[i].transform.localPosition = markerData[i].Position;
                    markers[i].SetActive(true);
                    markers[i].GetComponent<Renderer>().enabled = visibleMarkers;
                    markers[i].transform.localScale = Vector3.one * markerScale;
                }
                else
                {
                    //hide markers if we cant find them.
                    markers[i].SetActive(false);
                }
            }
        }
    }
}