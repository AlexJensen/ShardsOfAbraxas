using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.UI.Managers
{
    class UIManager : MonoBehaviour, IUIManager
    {
        #region Fields
        [SerializeField] private List<GameObject> zones;

        public float minOpacity = 0.1f;
        public float maxOpacity = 1f;
        public float flickerSpeed = 1f;

        private float flickerTimer;
        private bool increasing = true;

        #endregion

        #region Properties
        public float CurrentOpacity { get; private set; }
        #endregion

        #region Methods
        public void DisplayZone(GameObject zone)
        {
            foreach (var z in zones)
            {
                if (z == zone)
                {
                    z.SetActive(!z.activeSelf);
                }
                else
                {
                    z.SetActive(false);
                }
            }
        }

        private void Update()
        {
            // Update the timer based on the current flicker speed
            if (increasing)
            {
                flickerTimer += Time.deltaTime / flickerSpeed;
                if (flickerTimer >= 1f)
                {
                    flickerTimer = 1f;
                    increasing = false;
                }
            }
            else
            {
                flickerTimer -= Time.deltaTime / flickerSpeed;
                if (flickerTimer <= 0f)
                {
                    flickerTimer = 0f;
                    increasing = true;
                }
            }

            // Calculate the current opacity
            CurrentOpacity = Mathf.Lerp(minOpacity, maxOpacity, flickerTimer);
        }
        #endregion

    }
}
