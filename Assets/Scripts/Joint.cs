using Assets.Scripts;
using System;
using UnityEngine;

public class Joint : MonoBehaviour
{
    [SerializeField] public LimbsEnum limb;
    [SerializeField] public float parentMagnitude;
    [SerializeField] public int order;
    [SerializeField] public Joint parentJoint;
    [SerializeField] public float lenghtToParent;
    Rigidbody rigidbody;
    public bool IsKinematic => rigidbody ? rigidbody.isKinematic : true;

    public void SetLimb(int i)
    {
        order = i;
        limb = (LimbsEnum)Enum.ToObject(typeof(LimbsEnum), i);
    }
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (parentJoint)
        {
            parentMagnitude = (this.transform.position - parentJoint.transform.position).magnitude;
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        print("joint triger "+name);
        if (col.tag == "death")
        {
            if (rigidbody)
                rigidbody.velocity = rigidbody.velocity + ( Vector3.up * 5);
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        print("joint collision " + name);
    }
    public void Kinematic(bool v)
    {
        if(rigidbody)
        {
            rigidbody.isKinematic = v;
            rigidbody.position += Vector3.up; ;
        }
    }
}
