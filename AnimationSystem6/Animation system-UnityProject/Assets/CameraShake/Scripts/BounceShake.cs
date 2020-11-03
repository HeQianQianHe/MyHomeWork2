using UnityEngine;

namespace CameraShake
{
    public class BounceShake : ICameraShake
    {
        readonly Params pars;
        readonly AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        readonly Vector3? sourcePosition = null;

        float attenuation = 1;
        Displacement direction;
        Displacement previousWaypoint;
        Displacement currentWaypoint;
        int bounceIndex;
        float t;

        public BounceShake(Params parameters, Vector3? sourcePosition = null)
        {
            this.sourcePosition = sourcePosition;
            pars = parameters;
            Displacement rnd = Displacement.InsideUnitSpheres();
            direction = Displacement.Scale(rnd, pars.axesMultiplier).Normalized;
        }

        public BounceShake(Params parameters, Displacement initialDirection, Vector3? sourcePosition = null)
        {
            this.sourcePosition = sourcePosition;
            pars = parameters;
            direction = Displacement.Scale(initialDirection, pars.axesMultiplier).Normalized;
        }

        public Displacement CurrentDisplacement { get; private set; }
        public bool IsFinished { get; private set; }
        public void Initialize(Vector3 cameraPosition, Quaternion cameraRotation)
        {
            attenuation = sourcePosition == null ?
                1 : Attenuator.Strength(pars.attenuation, sourcePosition.Value, cameraPosition);
            currentWaypoint = attenuation * direction.ScaledBy(pars.positionStrength, pars.rotationStrength);
        }

        public void Update(float deltaTime, Vector3 cameraPosition, Quaternion cameraRotation)
        {
            if (t < 1)
            {

                t += deltaTime * pars.freq;
                if (pars.freq == 0) t = 1;

                CurrentDisplacement = Displacement.Lerp(previousWaypoint, currentWaypoint,
                    moveCurve.Evaluate(t));
            }
            else
            {
                t = 0;
                CurrentDisplacement = currentWaypoint;
                previousWaypoint = currentWaypoint;
                bounceIndex++;
                if (bounceIndex > pars.numBounces)
                {
                    IsFinished = true;
                    return;
                }

                Displacement rnd = Displacement.InsideUnitSpheres();
                direction = -direction
                    + pars.randomness * Displacement.Scale(rnd, pars.axesMultiplier).Normalized;
                direction = direction.Normalized;
                float decayValue = 1 - (float)bounceIndex / pars.numBounces;
                currentWaypoint = decayValue * decayValue * attenuation
                    * direction.ScaledBy(pars.positionStrength, pars.rotationStrength);
            }
        }

        [System.Serializable]
        public class Params
        {

            public float positionStrength = 0.05f;

            public float rotationStrength = 0.1f;

            public Displacement axesMultiplier = new Displacement(Vector2.one, Vector3.forward);

            public float freq = 25;


            public int numBounces = 5;

            public float randomness = 0.5f;

            public Attenuator.StrengthAttenuationParams attenuation;
        }
    }
}
