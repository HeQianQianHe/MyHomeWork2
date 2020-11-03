using UnityEngine;

namespace CameraShake
{
    public class PerlinShake : ICameraShake
    {
        readonly Params pars;
        readonly Envelope envelope;

        public IAmplitudeController AmplitudeController;

        Vector2[] seeds;
        float time;
        Vector3? sourcePosition;
        float norm;

        public PerlinShake(
            Params parameters,
            float maxAmplitude = 1,
            Vector3? sourcePosition = null,
            bool manualStrengthControl = false)
        {
            pars = parameters;
            envelope = new Envelope(pars.envelope, maxAmplitude,
                manualStrengthControl ?
                    Envelope.EnvelopeControlMode.Manual : Envelope.EnvelopeControlMode.Auto);
            AmplitudeController = envelope;
            this.sourcePosition = sourcePosition;
        }

        public Displacement CurrentDisplacement { get; private set; }
        public bool IsFinished { get; private set; }

        public void Initialize(Vector3 cameraPosition, Quaternion cameraRotation)
        {
            seeds = new Vector2[pars.noiseModes.Length];
            norm = 0;
            for (int i = 0; i < seeds.Length; i++)
            {
                seeds[i] = Random.insideUnitCircle * 20;
                norm += pars.noiseModes[i].amplitude;
            }
        }

        public void Update(float deltaTime, Vector3 cameraPosition, Quaternion cameraRotation)
        {
            if (envelope.IsFinished)
            {
                IsFinished = true;
                return;
            }
            time += deltaTime;
            envelope.Update(deltaTime);

            Displacement disp = Displacement.Zero;
            for (int i = 0; i < pars.noiseModes.Length; i++)
            {
                disp += pars.noiseModes[i].amplitude / norm *
                    SampleNoise(seeds[i], pars.noiseModes[i].freq);
            }

            CurrentDisplacement = envelope.Intensity * Displacement.Scale(disp, pars.strength);
            if (sourcePosition != null)
                CurrentDisplacement *= Attenuator.Strength(pars.attenuation, sourcePosition.Value, cameraPosition);
        }

        private Displacement SampleNoise(Vector2 seed, float freq)
        {
            Vector3 position = new Vector3(
                Mathf.PerlinNoise(seed.x + time * freq, seed.y),
                Mathf.PerlinNoise(seed.x, seed.y + time * freq),
                Mathf.PerlinNoise(seed.x + time * freq, seed.y + time * freq));
            position -= Vector3.one * 0.5f;

            Vector3 rotation = new Vector3(
                Mathf.PerlinNoise(-seed.x - time * freq, -seed.y),
                Mathf.PerlinNoise(-seed.x, -seed.y - time * freq),
                Mathf.PerlinNoise(-seed.x - time * freq, -seed.y - time * freq));
            rotation -= Vector3.one * 0.5f;

            return new Displacement(position, rotation);
        }


        [System.Serializable]
        public class Params
        {
            public Displacement strength = new Displacement(Vector3.zero, new Vector3(2, 2, 0.8f));
            public NoiseMode[] noiseModes = { new NoiseMode(12, 1) };

            public Envelope.EnvelopeParams envelope;
            public Attenuator.StrengthAttenuationParams attenuation;
        }


        [System.Serializable]
        public struct NoiseMode
        {
            public NoiseMode(float freq, float amplitude)
            {
                this.freq = freq;
                this.amplitude = amplitude;
            }

            public float freq;

            [Range(0, 1)]
            public float amplitude;
        }
    }
}
