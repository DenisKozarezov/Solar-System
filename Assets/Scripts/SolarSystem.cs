using UnityEngine;

namespace SolarSystem
{
    public class SolarSystem : MonoBehaviour
    {
        public Body Sun;
        [Range(0.01f, 2f)]
        public float SpeedModifier;

        private void Update()
        {
            Sun.Rotate(SpeedModifier);
        }
    }
}