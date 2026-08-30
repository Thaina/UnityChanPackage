using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace UnityChan {
[RequireComponent(typeof(Light2D))]
public class RandomLight2DMover : MonoBehaviour {
    void Start() {
        // Initialize targets
        m_targetPos = GetRandomPointInArea();
        m_targetZRot = GetRandomZRotation();
        m_nextPosChangeTime = Time.time + Random.Range(m_minTargetInterval, m_maxTargetInterval);
        m_nextRotChangeTime = Time.time + m_rotationChangeInterval;
    }

    void Update() {
        // Move towards target position
        Vector3 currentPos = transform.position;
        Vector3 targetWorld = new Vector3(m_targetPos.x, m_targetPos.y, currentPos.z);
        transform.position = Vector3.MoveTowards(currentPos, targetWorld, m_moveSpeed * Time.deltaTime);

        // If close enough or time to change, pick a new target position
        if (Vector2.Distance(new Vector2(currentPos.x, currentPos.y), m_targetPos) <= m_targetReachThreshold ||
            Time.time >= m_nextPosChangeTime)
        {
            m_targetPos = GetRandomPointInArea() + Random.insideUnitCircle * m_targetJitter;
            m_nextPosChangeTime = Time.time + Random.Range(m_minTargetInterval, m_maxTargetInterval);
        }

        // Handle rotation
        if (m_rotate)
        {
            float currentZ = transform.eulerAngles.z;
            float newZ = Mathf.MoveTowardsAngle(currentZ, m_targetZRot, m_rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, newZ);

            if (Time.time >= m_nextRotChangeTime)
            {
                m_targetZRot = GetRandomZRotation();
                m_nextRotChangeTime = Time.time + m_rotationChangeInterval;
            }
        }
    }

    private Vector2 GetRandomPointInArea() {
        // Interpret m_area.x/y as the center, and m_area.width/height as full extents
        Vector2 center = new Vector2(m_area.x, m_area.y);
        float halfW = m_area.width * 0.5f;
        float halfH = m_area.height * 0.5f;

        // Sample within the centered rectangle
        float localX = Random.Range(center.x - halfW, center.x + halfW);
        float localY = Random.Range(center.y - halfH, center.y + halfH);

        if (m_useLocalSpace)
        {
            // Treat center as local coordinates relative to this transform
            return transform.TransformPoint(new Vector2(localX, localY));
        }
        else
        {
            // World-space directly
            return new Vector2(localX, localY);
        }
    }

    private float GetRandomZRotation() {
        return Random.Range(0f, 360f);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;

        Vector2 center = new Vector2(m_area.x, m_area.y);
        float halfW = m_area.width * 0.5f;
        float halfH = m_area.height * 0.5f;

        Vector2 bl = new Vector2(center.x - halfW, center.y - halfH);
        Vector2 br = new Vector2(center.x + halfW, center.y - halfH);
        Vector2 tr = new Vector2(center.x + halfW, center.y + halfH);
        Vector2 tl = new Vector2(center.x - halfW, center.y + halfH);

        if (m_useLocalSpace)
        {
            Vector3 a = transform.TransformPoint(bl);
            Vector3 b = transform.TransformPoint(br);
            Vector3 c = transform.TransformPoint(tr);
            Vector3 d = transform.TransformPoint(tl);
            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }
        else
        {
            float z = transform.position.z;
            Vector3 a = new Vector3(bl.x, bl.y, z);
            Vector3 b = new Vector3(br.x, br.y, z);
            Vector3 c = new Vector3(tr.x, tr.y, z);
            Vector3 d = new Vector3(tl.x, tl.y, z);
            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }
    }

    private void OnValidate() {
        m_spotLight = GetComponent<Light2D>();
    }

    //----------------------------------------------------------------------------------------------------------------------    

    [HideInInspector] [SerializeField] private Light2D m_spotLight; // Assign your 2D Spot Light (Light2D with LightType = Spot)

    [Header("Bounds (Local or World)")] [SerializeField]
    private Rect m_area = new Rect(-5f, -3f, 10f, 6f); // x,y = bottom-left; width, height

    [SerializeField] private bool m_useLocalSpace = false; // If true, area is relative to this transform

    [Header("Movement")] [SerializeField] private float m_moveSpeed = 3f; // Units per second
    [SerializeField] private float m_targetReachThreshold = 0.05f;
    [SerializeField] private Vector2 m_targetJitter = new Vector2(0.2f, 0.2f); // Adds slight randomness near target

    [Header("Rotation")] [SerializeField] private bool m_rotate = true;
    [SerializeField] private float m_rotationSpeed = 90f; // Degrees per second towards target rotation
    [SerializeField] private float m_rotationChangeInterval = 2f; // Seconds between new target rotations

    [Header("Target Change")] [SerializeField]
    private float m_minTargetInterval = 1.5f; // Minimum seconds before picking new position

    [SerializeField] private float m_maxTargetInterval = 4f; // Maximum seconds before picking new position

    // Runtime state (not shown in Inspector)
    private Vector2 m_targetPos;
    private float m_targetZRot;
    private float m_nextPosChangeTime;
    private float m_nextRotChangeTime;
}
} //end namespace UnityChan