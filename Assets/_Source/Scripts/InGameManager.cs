using MiniFootball.Game;
using MiniFootball.UI;
using UnityEngine;

namespace MiniFootball
{
    public class InGameManager : MonoBehaviour
    {
        public static InGameManager instance { get; private set; }

        public InGameEvents InGameEvents;
        public UIManager uiManager { get; private set; }
        public MatchManager matchManager { get; private set; }

        private void Awake()
        {
            if (instance && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            InGameEvents = new InGameEvents();

            uiManager = GetComponentInChildren<UIManager>();
            matchManager = GetComponentInChildren<MatchManager>();
        }

        [ContextMenu("Start Game")]
        public void StartGame()
        {
            InGameEvents.StartGame();
        }
    }
}
