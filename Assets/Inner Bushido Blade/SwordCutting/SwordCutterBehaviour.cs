using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DynamicMeshCutter;
using Random = UnityEngine.Random;

public class SwordCutterBehaviour : CutterBehaviour
{
    public bool m_EnteredTarget = false;

    public bool m_DebugMode = false;

    public AudioClip m_MissSound;
    public AudioClip m_HitSound;

    public Plane m_EnteredPlane;
    public Plane m_ExitedPlane;
    public Plane m_CutPlane;

    public float m_PlaneNormalOfChangeDuringCut = 0.0f; //90 degrees would be a perpindicular change in cut
    public GameObject m_VisualEnteredPlane;
    public GameObject m_VisualExitedPlane;
    public GameObject m_VisualCutPlane;
    public bool m_EnteredPlaneIsValid = false;
    public bool m_ExitedPlaneIsValid = false;

    public float m_TimeToAchieveCut = 0.2f;
    public Transform m_SwordTip;
    public float m_MinimumCutSpeed = 5.0f;

    public Vector3 m_SwordTipVelocity;

    private Rigidbody m_rigidbody;
    private AudioSource m_audiosource;

    private Coroutine m_CurrentCoroutine;
    private MeshTarget m_CurrentTarget;

    private Vector3 m_EntryPoint;
    private Vector3 m_ExitPoint;
    private Vector3 m_VectorBetweenCuts;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_audiosource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var meshTarget = other.gameObject.GetComponentInParent<MeshTarget>();
        var ragdoll = other.gameObject.GetComponentInParent<DynamicRagdoll>();

        // Get the contact point
        m_EntryPoint = other.ClosestPoint(m_SwordTip.position);

        if (meshTarget != null)
        {
            EnteredTarget(meshTarget);
            //VibrationManager.instance.VibrateController(0.3f, 0.3f, 0.4f, OVRInput.Controller.LTouch);
            //VibrationManager.instance.VibrateController(0.3f, 0.3f, 0.4f, OVRInput.Controller.RTouch);
        }
        else if (ragdoll != null)
        {
            meshTarget = ragdoll.gameObject.GetComponentInChildren<MeshTarget>();

            if (meshTarget != null)
            {
                EnteredTarget(meshTarget);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("ONTRIGGEREXIT CALLED");

        var meshTarget = other.gameObject.GetComponentInParent<MeshTarget>();
        var ragdoll = other.gameObject.GetComponentInParent<DynamicRagdoll>();

        // Get the contact point
        m_ExitPoint = other.ClosestPoint(m_SwordTip.position);

        if (meshTarget != null)
        {

            LeavingTarget(meshTarget);
        }
        else if (ragdoll != null)
        {
            meshTarget = ragdoll.gameObject.GetComponentInChildren<MeshTarget>();

            if (meshTarget != null)
            {
                LeavingTarget(meshTarget);
            }
        }
    }

    private void EnteredTarget(MeshTarget meshTarget)
    {
        // Entered a new target
        if (!m_EnteredTarget && m_CurrentCoroutine == null)
        {
            ResetVariablesForNewCut(meshTarget);
        }
        // Time must have expired on previous cut or something went wrong
        else if (m_EnteredTarget && m_CurrentCoroutine == null)
        {
            ResetVariablesForNewCut(meshTarget);
        }
        // Already have an active cut, so do nothing
        else if (m_EnteredTarget && m_CurrentCoroutine != null)
        {
            Debug.Log("ACTIVE CUT DETECTED");
        }
        // Probably already have a cut going through still
        else if (!m_EnteredTarget && m_CurrentCoroutine != null)
        {
            m_EnteredTarget = true;
        }
        else
        {
            ResetVariablesForNewCut(meshTarget);
        }
    }

    private void ResetVariablesForNewCut(MeshTarget meshTarget)
    {
        m_EnteredTarget = true;
        m_CurrentCoroutine = StartCoroutine(ResetEnteredTime());
        //m_EnteredRotation = transform.rotation;
        m_CurrentTarget = meshTarget;
        m_EnteredPlane = new Plane(transform.forward, transform.position);
        m_EnteredPlaneIsValid = true;

        // Show visual plane of cut
        if (m_DebugMode)
        {
            m_VisualEnteredPlane.SetActive(true);
            m_VisualEnteredPlane.transform.position = m_EntryPoint;
            m_VisualEnteredPlane.transform.LookAt(m_VisualEnteredPlane.transform.position + m_EnteredPlane.normal);
        }
        else
        {
            m_VisualEnteredPlane.SetActive(false);
        }
    }

    IEnumerator ResetEnteredTime()
    {
        yield return new WaitForSeconds(m_TimeToAchieveCut);
        m_CurrentCoroutine = null;
        m_CurrentTarget = null;
    }

    private void LeavingTarget(MeshTarget meshTarget)
    {
        // We entered the target and are within the time frame of the cut
        if(m_EnteredTarget && m_CurrentCoroutine != null && m_CurrentTarget == meshTarget)
        {
            m_SwordTipVelocity = m_rigidbody.GetPointVelocity(m_SwordTip.position);
            Debug.Log("RIGID BODY VELOCITY: " + m_SwordTipVelocity.magnitude);

            if(m_SwordTipVelocity.magnitude >= m_MinimumCutSpeed)
            {
                // Cut Mesh
                CutMesh(meshTarget);
                //m_CurrentCoroutine = null;
                //m_CurrentTarget = null;

                //m_audiosource.PlayOneShot(m_HitSound);
            }
            else
            {
                m_audiosource.PlayOneShot(m_MissSound, 0.5f);
                Debug.Log("Failed cut... Not enough Velocity in the cut!");
            }
        }

        m_ExitedPlane = new Plane(transform.forward, transform.position);
        m_ExitedPlaneIsValid = true;

        // Show visual plane of cut
        if (m_DebugMode)
        {
            m_VisualExitedPlane.SetActive(true);
            m_VisualExitedPlane.transform.position = m_ExitPoint;
            m_VisualExitedPlane.transform.LookAt(m_VisualExitedPlane.transform.position + m_ExitedPlane.normal);
        }

        CheckPlanesOfCutsAndOutputData();


        m_EnteredTarget = false;
        //m_CurrentCoroutine = null;
        //m_CurrentTarget = null;
    }

    private void CheckPlanesOfCutsAndOutputData()
    {
        if (m_EnteredPlaneIsValid && m_ExitedPlaneIsValid)
        {
            Quaternion enteredRotation = Quaternion.LookRotation(m_EnteredPlane.normal, transform.up);
            Quaternion exitedRotation = Quaternion.LookRotation(m_ExitedPlane.normal, transform.up);
            m_PlaneNormalOfChangeDuringCut = Quaternion.Angle(enteredRotation, exitedRotation);

            Debug.Log("Successfully Cut through something and outputting data...");
            Debug.Log("Angle of change between in/out cut was: " + m_PlaneNormalOfChangeDuringCut);

        }

        m_EnteredPlaneIsValid = false;
        m_ExitedPlaneIsValid = false;
    }

    private void CutMesh(MeshTarget meshTarget)
    {
        m_VectorBetweenCuts = m_EntryPoint - m_ExitPoint;
        m_VectorBetweenCuts.Normalize();

        Debug.DrawLine(m_EntryPoint, m_ExitPoint, Color.blue, 3);

        Debug.DrawRay(m_ExitPoint, m_VectorBetweenCuts, Color.magenta, 3);
        Vector3 crossOfZ = Vector3.Cross(transform.forward, m_VectorBetweenCuts).normalized;
        Debug.DrawRay(m_ExitPoint, crossOfZ, Color.magenta, 3);
        Debug.DrawRay(m_ExitPoint, Vector3.Cross(m_VectorBetweenCuts, crossOfZ), Color.red, 5);

        m_CutPlane = new Plane(Vector3.Cross(m_VectorBetweenCuts, crossOfZ), m_ExitPoint);

        // Show visual plane of cut
        if (m_DebugMode)
        {
            m_VisualCutPlane.SetActive(true);
            m_VisualCutPlane.transform.position = m_ExitPoint;
            m_VisualCutPlane.transform.LookAt(m_VisualCutPlane.transform.position + m_CutPlane.normal);
        }


        Debug.Log("CUT MESH CALLED");
        Cut(meshTarget, transform.position, transform.forward, null, OnCreated);
        // Cut(meshTarget, m_ExitPoint, Vector3.Cross(m_VectorBetweenCuts, crossOfZ), OnCutWithSword, OnCreated);
    }

    void OnCutWithSword(bool success, Info info)
    {
        List<MeshTarget> meshes = null;
        // var breakage = info.MeshTarget.GetComponent<EnemyBreakage>();

        if (success)
        {
            m_CurrentCoroutine = null;
            m_CurrentTarget = null;
            m_audiosource.PlayOneShot(m_HitSound);

            // if (breakage != null)
            // {
            //     meshes = breakage.m_MeshesToCut;
            //     breakage.InstantiateScore(100 - Mathf.FloorToInt(m_PlaneNormalOfChangeDuringCut), info.MeshTarget.transform.position);
            // }

            // Cut All attached Meshes first
            if (meshes != null)
            {
                foreach (var mesh in meshes)
                {
                    Debug.Log("CUT MESH CALLED on MESHES");
                    Cut(mesh, transform.position, transform.forward, null, OnCreated);
                }
            }
        }
    }

    void OnCreated(Info info, MeshCreationData cData)
    {
        MeshCreation.TranslateCreatedObjects(info, cData.CreatedObjects, cData.CreatedTargets, Separation);

        foreach(var obj in cData.CreatedObjects)
        {
            var destroyScript = obj.AddComponent<DestroyAfterTime>();
            destroyScript.Initialize(2);

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddRelativeTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Random.Range(2f, 10f));
            }
        }
    }

}
