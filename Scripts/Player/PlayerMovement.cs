using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D m_playerRigidBody2d;
    private FixedJoystick m_leftJoystick;
    private FixedJoystick m_rightJoystick;
    public bool m_leftJoystickAvailable = false;
    public bool m_rightJoystickAvailable = false;
    [SerializeField] private float m_movementSpeed = 1f;

    private void Awake()
    {
        m_playerRigidBody2d = GetComponent<Rigidbody2D>();

        if (m_leftJoystickAvailable) m_leftJoystick = GameObject.Find("LeftJoystickFixed").GetComponent<FixedJoystick>();
        if (m_rightJoystickAvailable) m_rightJoystick = GameObject.Find("RightJoystickFixed").GetComponent<FixedJoystick>();
    }

    private void Update()
    {

        if (Mathf.Abs(m_rightJoystick.Horizontal) + Mathf.Abs(m_rightJoystick.Vertical) > 0.95f)
        {
            float an = Mathf.Atan2(m_rightJoystick.Vertical, m_rightJoystick.Horizontal) * (180 / Mathf.PI);
            if (Mathf.Abs(m_playerRigidBody2d.transform.rotation.z - an) > 2f)
            {
                m_playerRigidBody2d.transform.rotation = Quaternion.Euler(0, 0, an);
            }
        }


    }
    private void FixedUpdate()
    {
        Vector3 currPosition = m_playerRigidBody2d.transform.position;
        if (Input.GetKey(KeyCode.W))
        {
            currPosition.y += m_movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            currPosition.y -= m_movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            currPosition.x -= m_movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            currPosition.x += m_movementSpeed * Time.deltaTime;
        }
        m_playerRigidBody2d.position = currPosition;


        if (m_leftJoystickAvailable)
        {
            m_playerRigidBody2d.velocity = new Vector2(m_leftJoystick.Horizontal, m_leftJoystick.Vertical) * m_movementSpeed;
        }
    }
}
