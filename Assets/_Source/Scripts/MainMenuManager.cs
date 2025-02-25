using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniFootball
{
    public class MainMenuManager : MonoBehaviour
    {
        public void ChangeSceneToGame()
        {
            SceneManager.LoadScene("_Source/Scene/Prototype/Basic Gameplay with Cinemachine Opening");
        }
    }
}