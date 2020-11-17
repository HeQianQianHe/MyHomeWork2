using UnityEngine;
using Unity.Mathematics;
using Klak.Math;
using Noise = Klak.Math.NoiseHelper;

namespace Puppet
{
    public class Dancer : MonoBehaviour
    {
        [SerializeField] float _footDistance = 0.3f;
        [SerializeField] float _stepFrequency = 2;
        [SerializeField] float _stepHeight = 0.3f;
        [SerializeField] float _stepAngle = 90;
        [SerializeField] float _maxDistance = 2;

        [SerializeField] float _hipHeight = 0.9f;
        [SerializeField] float _hipPositionNoise = 0.1f;
        [SerializeField] float _hipRotationNoise = 30;

        [SerializeField] float _spineBend = -8;
        [SerializeField] Vector3 _spineRotationNoise = new Vector3(30, 20, 20);

        [SerializeField] Vector3 _handPosition = new Vector3(0.3f, 0.3f, -0.2f);
        [SerializeField] Vector3 _handPositionNoise = new Vector3(0.3f, 0.3f, 0.3f);

        [SerializeField] float _headMove = 3;

        [SerializeField] float _noiseFrequency = 1.1f;
        [SerializeField] uint _randomSeed = 123;

        public float footDistance {
            get { return _footDistance; }
            set { _footDistance = value; }
        }

        public float stepFrequency {
            get { return _stepFrequency; }
            set { _stepFrequency = value; }
        }

        public float stepHeight {
            get { return _stepHeight; }
            set { _stepHeight = value; }
        }

        public float stepAngle {
            get { return _stepAngle; }
            set { _stepAngle = value; }
        }

        public float maxDistance {
            get { return _maxDistance; }
            set { _maxDistance = value; }
        }


        public float hipHeight {
            get { return _hipHeight; }
            set { _hipHeight = value; }
        }

        public float hipPositionNoise {
            get { return _hipPositionNoise; }
            set { _hipPositionNoise = value; }
        }

        public float hipRotationNoise {
            get { return _hipRotationNoise; }
            set { _hipRotationNoise = value; }
        }



        public float spineBend {
            get { return _spineBend; }
            set { _spineBend = value; }
        }

        public Vector3 spineRotationNoise {
            get { return _spineRotationNoise; }
            set { _spineRotationNoise = value; }
        }


        public Vector3 handPosition {
            get { return _handPosition; }
            set { _handPosition = value; }
        }

        public Vector3 handPositionNoise {
            get { return _handPositionNoise; }
            set { _handPositionNoise = value; }
        }



        public float headMove {
            get { return _headMove; }
            set { _headMove = value; }
        }



        public float noiseFrequency {
            get { return _noiseFrequency; }
            set { _noiseFrequency = value; }
        }

        public uint randomSeed {
            get { return _randomSeed; }
            set { _randomSeed = value; }
        }


        Animator _animator;

        XXHash _hash;
        float2 _noise;

        Vector3[] _feet = new Vector3[2];

        float _step;
        
        float _stepSign = 1;

        Matrix4x4 _chestMatrix, _chestMatrixInv;

        static Vector3 SetY(Vector3 v, float y)
        {
            v.y = y;
            return v;
        }

        static float SmoothStep(float x)
        {
            return x * x * (3 - 2 * x);
        }

        int StepCount { get { return Mathf.FloorToInt(_step); } }


        float StepTime { get { return _step - Mathf.Floor(_step); } }


        uint StepSeed { get { return (uint)StepCount * 100; } }


        bool PivotIsLeft { get { return (StepCount & 1) == 0; } }


        float StepAngle { get {
            return _hash.Float(0.5f, 1.0f, StepSeed + 1) * _stepAngle * _stepSign;
        } }


        Quaternion StepRotationFull { get {
            return Quaternion.AngleAxis(StepAngle, Vector3.up);
        } }


        Quaternion StepRotation { get {
            return Quaternion.AngleAxis(StepAngle * StepTime, Vector3.up);
        } }


        Vector3 FootBias { get {
            return Vector3.up * _animator.leftFeetBottomHeight;
        } }


        Vector3 GetFootPosition(int index)
        {
            var thisFoot = _feet[index];
            var thatFoot = _feet[(index + 1) & 1];


            if (PivotIsLeft ^ (index == 1)) return thisFoot + FootBias;


            var rp = StepRotation * (thisFoot - thatFoot);


            var up = Mathf.Sin(SmoothStep(StepTime) * Mathf.PI) * _stepHeight;

            return thatFoot + rp + up * Vector3.up + FootBias;
        }

        Vector3 LeftFootPosition { get { return GetFootPosition(0); } }
        Vector3 RightFootPosition { get { return GetFootPosition(1); } }


        Vector3 CurrentStepDestination { get {
            var right = (_feet[1] - _feet[0]).normalized * _footDistance;
            if (PivotIsLeft)
                return _feet[0] + StepRotationFull * right;
            else
                return _feet[1] - StepRotationFull * right;
        } }


        Vector3 BodyPosition {
            get {

                var theta = (StepTime + (PivotIsLeft ? 0 : 1)) * Mathf.PI;
                var right = (1 - Mathf.Sin(theta)) / 2;
                var pos = Vector3.Lerp(LeftFootPosition, RightFootPosition, right);

     
                var y = _hipHeight + Mathf.Cos(StepTime * Mathf.PI * 4) * _stepHeight / 2;
                y += noise.snoise(_noise) * _hipPositionNoise;

                return SetY(pos, y);
            }
        }

        Quaternion BodyRotation {
            get {

                var rot = Quaternion.AngleAxis(-90, Vector3.up);


                var right = SetY(RightFootPosition - LeftFootPosition, 0);

       
                rot *= Quaternion.LookRotation(right.normalized);

                return rot * Noise.Rotation(_noise, math.radians(_hipRotationNoise), 1);
            }
        }

        Quaternion SpineRotation { get {
            var rot = Quaternion.AngleAxis(_spineBend, Vector3.forward);
            rot *= Noise.Rotation(_noise, math.radians(_spineRotationNoise), 2);
            return rot;
        } }
     
        Vector3 GetHandPosition(int index)
        {
            var isLeft = (index == 0);
    
            var pos = _handPosition;
            if (isLeft) pos.x *= -1;

            pos = _animator.bodyRotation * pos + _animator.bodyPosition;

            pos += Vector3.Scale(Noise.Float3(_noise, (uint)(4 + index)), _handPositionNoise);

            pos = _chestMatrixInv * new Vector4(pos.x, pos.y, pos.z, 1);
            pos.y = Mathf.Max(pos.y, 0.2f);
            pos.z = isLeft ? Mathf.Max(pos.z, 0.2f) : Mathf.Min(pos.z, -0.2f);
            pos = _chestMatrix * new Vector4(pos.x, pos.y, pos.z, 1);

            return pos;
        }

        Vector3 LeftHandPosition { get { return GetHandPosition(0); } }
        Vector3 RightHandPosition { get { return GetHandPosition(1); } }

        Vector3 LookAtPosition {
            get {
                var pos = Noise.Float3(_noise, 3) * _headMove;
                pos.z = 2;
                return _animator.bodyPosition + _animator.bodyRotation * pos;
            }
        }


        void OnValidate()
        {
            _footDistance = Mathf.Max(_footDistance, 0.01f);
        }

        void Start()
        {
            _animator = GetComponent<Animator>();

            _hash = new XXHash(_randomSeed);
            _noise = _hash.Float2(-1000, 1000, 0);


            var origin = SetY(transform.position, 0);
            var foot = transform.right * _footDistance / 2;
            _feet[0] = origin - foot;
            _feet[1] = origin + foot;
        }

        void Update()
        {

            _noise.x += _noiseFrequency * Time.deltaTime;

    
            var delta = _stepFrequency * Time.deltaTime;


            var right = (_feet[1] - _feet[0]).normalized * _footDistance;


            if (StepCount == Mathf.FloorToInt(_step + delta))
            {
                
                if (PivotIsLeft)
                    _feet[1] = _feet[0] + right;
                else
                    _feet[0] = _feet[1] - right;

                _step += delta;
            }
            else
            {
         
                _feet[PivotIsLeft ? 1 : 0] = CurrentStepDestination;

   
                _step += delta;

       
                if (_hash.Float(StepSeed) > 0.5f) _stepSign *= -1;

         
                var dist = (CurrentStepDestination - transform.position).magnitude;
                if (dist > _maxDistance)
                {
                
                    _stepSign *= -1;
                    if (dist < (CurrentStepDestination - transform.position).magnitude)
                        _stepSign *= -1;
                }
            }


            var chest = _animator.GetBoneTransform(HumanBodyBones.Chest);
            _chestMatrix = chest.localToWorldMatrix;
            _chestMatrixInv = chest.worldToLocalMatrix;
        }

        void OnAnimatorIK(int layerIndex)
        {
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, LeftFootPosition);
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, RightFootPosition);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

            _animator.bodyPosition = BodyPosition;
            _animator.bodyRotation = BodyRotation;

            var spine = SpineRotation;
            _animator.SetBoneLocalRotation(HumanBodyBones.Spine, spine);
            _animator.SetBoneLocalRotation(HumanBodyBones.Chest, spine);
            _animator.SetBoneLocalRotation(HumanBodyBones.UpperChest, spine);

            _animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandPosition);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandPosition);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);

            _animator.SetLookAtPosition(LookAtPosition);
            _animator.SetLookAtWeight(1);
        }


    }
}
