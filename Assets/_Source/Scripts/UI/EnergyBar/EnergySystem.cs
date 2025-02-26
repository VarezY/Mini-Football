using System;
using System.Collections;
using MiniFootball.Agent;
using UnityEngine;

namespace MiniFootball.UI.NewEnergyBar
{
    [System.Serializable]
    public class EnergySystem
    {
        [Header("Energy Settings")]
        [SerializeField] private AgentType agentType;
        [SerializeField] private float currentEnergy = 0f;
        [SerializeField] private float maxEnergy = 6f;
        
        [Header("Recharge Settings")]
        [SerializeField] private bool autoRecharge = true;
        [SerializeField] private float rechargeRate = .5f; // Units per second
        [SerializeField] private float rechargeDelay = 0f; // Seconds to wait before recharging
        
        private Coroutine _rechargeCoroutine = null;
        private MonoBehaviour _owner; // Reference to the MonoBehaviour to start coroutines
        
        public void Initialize(MonoBehaviour ownerBehaviour)
        {
            _owner = ownerBehaviour;
        }
        
        public void Cleanup()
        {
            // Stop any running coroutines
            if (_rechargeCoroutine == null || !_owner) return;
            
            _owner.StopCoroutine(_rechargeCoroutine);
            _rechargeCoroutine = null;
        }
        
        // Property to get and set energy with event triggering
        public float CurrentEnergy
        {
            get => currentEnergy;
            set
            {
                if (Mathf.Approximately(currentEnergy, value)) return;
                // Immediately update with clamping
                currentEnergy = Mathf.Clamp(value, 0, maxEnergy);
                    
                // Notify listeners of the change
                InGameManager.instance.InGameEvents.EnergyChanged(agentType, currentEnergy);
                    
                // Check if we need to start/stop recharging
                CheckRechargeState();
            }
        }
        
        public float MaxEnergy => maxEnergy;

        public void CheckRechargeState()
        {
            if (currentEnergy < maxEnergy && autoRecharge && _owner)
            {
                // Start recharging if not already
                if (_rechargeCoroutine == null)
                {
                    _rechargeCoroutine = _owner.StartCoroutine(RechargeCoroutine());
                }
            }
            else if (currentEnergy >= maxEnergy && _rechargeCoroutine != null && _owner)
            {
                // Stop recharging if at max
                _owner.StopCoroutine(_rechargeCoroutine);
                _rechargeCoroutine = null;
            }
        }
        
        private IEnumerator RechargeCoroutine()
        {
            // Initial delay before recharging starts
            if (rechargeDelay > 0)
            {
                yield return new WaitForSeconds(rechargeDelay);
            }
            
            // Continue recharging until max or disabled
            while (currentEnergy < maxEnergy)
            {
                // Calculate how much energy to add this frame
                float energyToAdd = rechargeRate * Time.deltaTime;
                
                // Check if this would exceed max energy
                if (currentEnergy + energyToAdd >= maxEnergy)
                {
                    // Set to exact max value and update
                    float oldValue = currentEnergy;
                    currentEnergy = maxEnergy;
                    
                    // Only invoke if value changed
                    if (!Mathf.Approximately(oldValue, currentEnergy))
                        InGameManager.instance.InGameEvents.EnergyChanged(agentType, currentEnergy);
                    
                    // Exit the loop
                    break;
                }
                
                // Update energy without triggering the property
                float previousValue = currentEnergy;
                currentEnergy += energyToAdd;
                
                // Notify listeners of the change
                InGameManager.instance.InGameEvents.EnergyChanged(agentType, currentEnergy);
                
                // Wait for next frame
                yield return null;
            }
            
            // Clear the coroutine reference when done
            _rechargeCoroutine = null;
        }
        
        // Public methods to manipulate energy
        public void SetEnergy(float value)
        {
            currentEnergy = value;
        }
        
        public void IncreaseEnergy(float amount)
        {
            currentEnergy += amount;
        }
        
        public void DecreaseEnergy(float amount)
        {
            currentEnergy -= amount;
            
            // Ensure recharging starts after decreasing energy
            CheckRechargeState();
        }
        
        // Method to manually start recharging
        public void StartRecharging()
        {
            if (currentEnergy < maxEnergy && _rechargeCoroutine == null && _owner)
            {
                _rechargeCoroutine = _owner.StartCoroutine(RechargeCoroutine());
            }
        }
        
        // Method to manually stop recharging
        public void StopRecharging()
        {
            if (_rechargeCoroutine == null || !_owner) return;
            _owner.StopCoroutine(_rechargeCoroutine);
            _rechargeCoroutine = null;
        }
        
        // Method to restart the recharge process
        public void RestartRecharge()
        {
            // Stop any existing recharge coroutine
            if (_rechargeCoroutine != null && _owner)
            {
                _owner.StopCoroutine(_rechargeCoroutine);
                _rechargeCoroutine = null;
            }

            currentEnergy = 0;
            // Only start recharging if we're not at max energy and auto-recharge is enabled
            if (currentEnergy < maxEnergy && autoRecharge && _owner)
            {
                _rechargeCoroutine = _owner.StartCoroutine(RechargeCoroutine());
            }
        }
    }
}