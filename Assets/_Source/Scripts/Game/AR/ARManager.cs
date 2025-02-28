using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;

namespace MiniFootball.Game.AR
{
    public class ARManager : MonoBehaviour
    {
        [Header("AR Configuration")]
        public ARSession session;
        public ARPlaneManager planeManager;
        public Material planeMaterial;
        public float aRScale;

        [Header("AR UI")]
        public GameObject aRButton;
        public GameObject visualizerButton;

        [Header("Game Objects")] 
        public GameObject gameBoard;
        public UISwitcher.UISwitcher aRToggle;
        public UISwitcher.UISwitcher visualizationToggle;

        private InGameManager _gameManager;
        public bool inAR { get; private set; } = false;

        private void OnEnable()
        {
            StartCoroutine(CheckSupport());
        }

        private void Start()
        {
            _gameManager = InGameManager.instance;
            aRToggle.OnValueChanged += CheckCameraPermissions;
            visualizationToggle.OnValueChanged += ToggleVisualizer;
        }

        private void OnDestroy()
        {
            aRToggle.OnValueChanged -= CheckCameraPermissions;
            visualizationToggle.OnValueChanged -= ToggleVisualizer;
        }

        private void ToggleVisualizer(bool isVisible)
        {
            if (planeManager.planePrefab.TryGetComponent(out ARPlaneMeshVisualizer visualizer))
                visualizer.enabled = isVisible;
            
            foreach (ARPlane plane in planeManager.trackables)
            {
                if (plane.TryGetComponent(out ARPlaneMeshVisualizer planeVisualizer))
                    planeVisualizer.enabled = isVisible;
            }
        }

        private IEnumerator CheckSupport()
        {
            yield return ARSession.CheckAvailability();

            if (ARSession.state == ARSessionState.NeedsInstall)
            {
                yield return ARSession.Install();
            }
            
            if (ARSession.state == ARSessionState.Ready)
            {
                aRButton.SetActive(true);
            }
            else
            {
                switch (ARSession.state)
                {
                    case ARSessionState.Unsupported:
                        aRButton.SetActive(false);
                        break;
                }
            }
        }

        private void CheckCameraPermissions(bool toggleAR)
        {
            inAR = toggleAR;
            if (toggleAR)
            {
                StartCoroutine(CheckARCameraPermissions());
            }
            else
            {
                visualizerButton.SetActive(false);
                _gameManager.InGameEvents.SwitchAR(false);
            }
        }

        private IEnumerator CheckARCameraPermissions()
        {
            Permission.RequestUserPermission(Permission.Camera);
            yield return new WaitForSeconds(0.2f);

            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                aRToggle.isOn = false;
                inAR = false;
                yield break;
            }
            
            // Handle denied permission
            session.enabled = true;
            planeManager.gameObject.SetActive(true);
            visualizerButton.SetActive(true);
            gameBoard.SetActive(false);
            _gameManager.InGameEvents.SwitchAR(true);
        }
    
    }
}