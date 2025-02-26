using System;
using System.Collections.Generic;
using MiniFootball.Agent;
using MiniFootball.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MiniFootball.UI.NewEnergyBar
{
    [System.Serializable]
    public class EnergyThreshold
    {
        public float threshold;
        public UnityEvent onThresholdReached;
        public UnityEvent onThresholdDecrease;
        [HideInInspector] public bool isAboveThreshold = false;
    }
    
    public class EnergyDisplay : MonoBehaviour
    {
        [SerializeField] private AgentType agentType = AgentType.Player;
        [SerializeField] private Color flagColor;
        [SerializeField] private Image energyFillImage;
        [SerializeField] private TMP_Text energyText;
        [SerializeField] private GameObject energyFillPrefab;
        [SerializeField] private InGameManager gameManager;
        
        [Header("Energy Threshold Events")]
        [SerializeField] private int numberOfParts = 4;
        [SerializeField] private List<EnergyThreshold> thresholds = new List<EnergyThreshold>();

        private int _currentEnergy;
        
        private void OnValidate()
        {
            UpdateThresholdList();
        }
        
        private void UpdateThresholdList()
        {
            if (thresholds.Count == numberOfParts) return;
            
            if (!energyFillPrefab) return;
            
            // Preserve existing events if possible
            List<EnergyThreshold> newThresholds = new List<EnergyThreshold>();

            for (int i = 1; i <= numberOfParts; i++)
            {
                float threshold = i;
                
                // Try to find existing threshold at this level
                EnergyThreshold existingThreshold = thresholds.Find(t => Mathf.Approximately(t.threshold, threshold));
                    
                if (existingThreshold != null)
                {
                    newThresholds.Add(existingThreshold);
                }
                else
                {
                    // Create new threshold
                    newThresholds.Add(new EnergyThreshold
                    {
                        threshold = threshold,
                        onThresholdReached = new UnityEvent(),
                        onThresholdDecrease = new UnityEvent(),
                        isAboveThreshold = false
                    });
                }
            }
                
            thresholds = newThresholds;
        }
        
        private void Start()
        {
            InGameManager.instance.InGameEvents.OnEnergyChanged += OnEnergyChanged;

            energyFillImage.color = flagColor;
            Color tempColor = energyFillImage.color;
            tempColor.a = .5f;
            energyFillImage.color = tempColor;
        }
        
        private void OnDestroy()
        {
            InGameManager.instance.InGameEvents.OnEnergyChanged -= OnEnergyChanged;
        }

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < numberOfParts; i++)
            {
                GameObject energyFill = Instantiate(energyFillPrefab, transform);
                Image energyFillTempImage = energyFill.GetComponent<Image>();
                
                energyFillTempImage.fillCenter = false;
                energyFillTempImage.color = flagColor;
                
                thresholds[i].onThresholdReached.AddListener(() =>
                {
                    energyFillTempImage.fillCenter = true;
                    _currentEnergy++;
                    energyText.text = $"{_currentEnergy}";
                });
                thresholds[i].onThresholdDecrease.AddListener(() =>
                {
                    energyFillTempImage.fillCenter = false;
                    _currentEnergy--;
                    energyText.text = $"{_currentEnergy}";
                });
            }
        }

        private void OnEnergyChanged(AgentType type, float newEnergy)
        {
            if (type != agentType) return;
            
            if (energyFillImage)
            {
                energyFillImage.fillAmount = newEnergy / numberOfParts;
            }

            // Check all thresholds
            foreach (EnergyThreshold threshold in thresholds)
            {
                bool wasAboveThreshold = threshold.isAboveThreshold;
                bool isAboveThresholdNow = (newEnergy >= threshold.threshold);

                if (wasAboveThreshold == isAboveThresholdNow) continue;
                
                threshold.isAboveThreshold = isAboveThresholdNow;

                if (isAboveThresholdNow)
                {
                    threshold.onThresholdReached?.Invoke();
                }
                else
                {
                    threshold.onThresholdDecrease?.Invoke();
                }
            }
        }
    }
}