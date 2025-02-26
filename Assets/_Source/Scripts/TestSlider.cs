using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MiniFootball
{
    // [ExecuteInEditMode]
    public class TestSlider : MonoBehaviour
    {
        public Image energyFillImage;
        public float energyFillPercent;
        
        [System.Serializable]
        public class EnergyThresholdEvent
        {
            public float threshold;
            public UnityEvent onThresholdReached;
            public UnityEvent onThresholdDecrease;
            public bool isAboveThreshold = false;
        }
    
        [Header("Energy Threshold Events")]
        [SerializeField, Range(0, 10)] private int numberOfParts = 4;
        [SerializeField] private List<EnergyThresholdEvent> thresholdEvents = new List<EnergyThresholdEvent>();

        private void OnValidate()
        {
            UpdateThresholdEvents();
        }

        private void UpdateThresholdEvents()
        {
            if (energyFillImage)
            {
                energyFillImage.fillAmount = energyFillPercent / numberOfParts;
            }
            
            // Only rebuild if the number of parts changed
            if (thresholdEvents.Count == numberOfParts) return;
            thresholdEvents.Clear();
            
            // Create events for each threshold
            for (int i = 1; i <= numberOfParts; i++)
            {
                float threshold = (float)i;
                var a = new EnergyThresholdEvent
                {
                    threshold = threshold,
                    onThresholdReached = new UnityEvent(),
                    onThresholdDecrease = new UnityEvent(),
                    isAboveThreshold = false
                };
                a.onThresholdReached.AddListener(DebugTestSlider);
                a.onThresholdDecrease.AddListener(DebugTestSliderLow);
                thresholdEvents.Add(a);
            }
            
        }

        private void Update()
        {
            foreach (var thresholdEvent in thresholdEvents)
            {
                // Check if we're crossing the threshold in the increasing direction
                if (energyFillPercent >= thresholdEvent.threshold && !thresholdEvent.isAboveThreshold)
                {
                    thresholdEvent.isAboveThreshold = true;
                    thresholdEvent.onThresholdReached.Invoke();
                    
                    // For debugging in the editor
                    Debug.Log($"Energy threshold reached: {thresholdEvent.threshold}");
                }
                // Check if we're crossing the threshold in the decreasing direction
                else if (energyFillPercent < thresholdEvent.threshold && thresholdEvent.isAboveThreshold)
                {
                    thresholdEvent.isAboveThreshold = false;
                    thresholdEvent.onThresholdDecrease.Invoke();
                    
                    // For debugging in the editor
                    Debug.Log($"Energy decreased below threshold: {thresholdEvent.threshold}");
                }
            }
        }

        private void DebugTestSlider()
        {
            Debug.Log($"Reach threshold");
        }
        
        private void DebugTestSliderLow()
        {
            Debug.Log($"Down threshold");
        }
    }
}