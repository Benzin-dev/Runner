using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] List<Rigidbody> rigidbodies;
    public bool isHit = false;

    private Animator animator;

    private void Awake()
    {
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        GetChildList();
    }

    private void GetChildList()
    {
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (null == child)
                continue;

            if (child.GetComponent<Rigidbody>())
            {
                rigidbodies.Add(child.GetComponent<Rigidbody>());
            }       
        }
    }


    public void DoRagdoll(bool isRagdoll)
    {
        animator.enabled = !isRagdoll;
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
        isHit = isRagdoll;
        //Physics.IgnoreLayerCollision(6, 0, isRagdoll);
    }
}
