using System;
using UnityEngine;

namespace Abraxas.VFX.Controllers
{
    internal class FlickerEffectController
    {
        #region Fields
        public float minOpacity = .1f;
        public float maxOpacity = .5f;
        public float flickerSpeed = 1f;

        private float flickerTimer;
        private bool increasing = true;
        #endregion

        #region Properties
        public float CurrentFlickerOpacity { get; private set; }
        #endregion

        public void Update(float deltaTime)
        {
            // Update the timer based on the current flicker speed
            if (increasing)
            {
                flickerTimer += deltaTime / flickerSpeed;
                if (flickerTimer >= 1f)
                {
                    flickerTimer = 1f;
                    increasing = false;
                }
            }
            else
            {
                flickerTimer -= deltaTime / flickerSpeed;
                if (flickerTimer <= 0f)
                {
                    flickerTimer = 0f;
                    increasing = true;
                }
            }

            // Calculate the current opacity
            CurrentFlickerOpacity = Mathf.Lerp(minOpacity, maxOpacity, flickerTimer);
        }
    }
}
