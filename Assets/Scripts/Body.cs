using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SolarSystem
{
    public class Body : MonoBehaviour
    {
        [Min(0f)]
        public float OrbitRadius;
        public string StringFormat;
        public string OrbitFormat;
        public float OrbitModifier;
        [Min(1f)]
        public float Mass;
        [Range(0f, 1f)]
        public float Eccentricity;
        [Min(0f)]
        public float Period;
        [Range(0f, 90f)]
        public float EclipticAngle;
        public bool Counterclockwise;

        private const int Segments = 100;
        private Vector3 _oldPoint;
        private float _alpha;
        private Body _parent;
        private LinkedList<Body> _satellites = new LinkedList<Body>();

        private void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                AddSatellite(transform.GetChild(i).GetComponent<Body>());
            }            
        }
        public void AddSatellite(Body body)
        {
            body._parent = this;
            _satellites.AddLast(body);
        }
        public void Rotate(float speedModifier)
        {
            if (_parent != null)
            {
                transform.position = CalculateOrbit(_alpha, OrbitRadius);
                _alpha = _alpha + (Counterclockwise ? 1 : -1) * (365f / Period * speedModifier);
            }

            foreach (Body body in _satellites)
            {
                body.Rotate(speedModifier);
            }
        }
        private void OnDrawGizmos()
        {
            Vector3 position = transform.position + Vector3.up * 5f;
            Handles.Label(position, name, EditorStyles.whiteLargeLabel);

            if (!Application.isPlaying) return;

            if (_satellites.Count > 0)
            {
                foreach (Body body in _satellites)
                {
                    body.OnDrawGizmos();
                }
            }

            if (_parent == null) return;

            float alpha = 20f;
            for (int i = 0; i < Segments; i++)
            {
                Vector3 newPoint = CalculateOrbit(alpha, OrbitRadius);

                Handles.DrawDottedLine(_oldPoint, newPoint, 5f);
                _oldPoint = newPoint;
                alpha += 360f / Segments;
            }
        }
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || _parent == null) return;

            Debug.DrawLine(transform.position, _parent.transform.position, Color.yellow);

            float range = Vector3.Distance(transform.position, _parent.transform.position) * OrbitModifier;
            Handles.Label(Vector3.Lerp(transform.position, _parent.transform.position, 0.5f), string.Format(StringFormat, range.ToString(OrbitFormat)));
        }
        private Vector3 CalculateOrbit(float alpha, float radius)
        {
            if (alpha == 0f) return transform.position;

            Vector3 parentPos = _parent.transform.position;
            float x = parentPos.x + radius * Mathf.Cos(alpha * Mathf.Deg2Rad);
            float z = parentPos.z + radius * Mathf.Sin(alpha * Mathf.Deg2Rad) * (1 - Eccentricity);
            float y = parentPos.y + radius * Mathf.Sin(alpha * Mathf.Deg2Rad) * Mathf.Sin(EclipticAngle * Mathf.Deg2Rad);
            return new Vector3(x, y, z);
        }
    }
}