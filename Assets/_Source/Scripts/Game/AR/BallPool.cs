using System;
using UnityEngine;

namespace MiniFootball.Game.AR
{
    public class BallPool : MonoBehaviour
    {
        private void Start()
        {
            Invoke(nameof(DisableBall), 15);
        }

        private void DisableBall()
        {
            this.gameObject.SetActive(false);
        }
    }
}