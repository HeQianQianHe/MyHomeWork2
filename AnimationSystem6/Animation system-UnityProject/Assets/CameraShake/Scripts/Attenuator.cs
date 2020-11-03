using UnityEngine;

namespace CameraShake
{

    public static class Attenuator
    {

        public static float Strength(StrengthAttenuationParams pars, Vector3 sourcePosition, Vector3 cameraPosition)
        {
            Vector3 vec = cameraPosition - sourcePosition;
            float distance = Vector3.Scale(pars.axesMultiplier, vec).magnitude;
            float strength = Mathf.Clamp01(1 - (distance - pars.clippingDistance) / pars.falloffScale);

            return Power.Evaluate(strength, pars.falloffDegree);
        }

        public static Displacement Direction(Vector3 sourcePosition, Vector3 cameraPosition, Quaternion cameraRotation)
        {
            Displacement direction = Displacement.Zero;
            direction.position = (cameraPosition - sourcePosition).normalized;
            direction.position = Quaternion.Inverse(cameraRotation) * direction.position;

            direction.eulerAngles.x = direction.position.z;
            direction.eulerAngles.y = direction.position.x;
            direction.eulerAngles.z = -direction.position.x;

            return direction;
        }

        [System.Serializable]
        public class StrengthAttenuationParams
        {
  
            public float clippingDistance = 10;

            public float falloffScale = 50;

            public Degree falloffDegree = Degree.Quadratic;

            public Vector3 axesMultiplier = Vector3.one;
        }
    }
}
