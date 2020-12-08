using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartGame.KartSystems
{
    public class JoystickInput : MonoBehaviour, IInput
    {
        public Joystick joystick;

        public int Acceleration
        {
            get { return m_Acceleration; }
        }
        public int Steering
        {
            get { return m_Steering; }
        }

        int m_Acceleration;
        int m_Steering;

        // Update is called once per frame
        void Update()
        {
            m_Acceleration = (int)joystick.Vertical;
            m_Steering = (int)joystick.Horizontal;
        }
    }
}