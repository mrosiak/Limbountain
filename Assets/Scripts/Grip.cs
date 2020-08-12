using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;
using Verlet;

public class Grip : MonoBehaviour
{
    public Extremity inRangeExtremity;
    [SerializeField] public GripStat gripStat;
    [SerializeField] bool shine;
    [SerializeField] public bool topGrip;
    [SerializeField] int frame = 10;
    MeshRenderer mr;
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        SetStats();
        if (topGrip)
        {
            StartCoroutine(ColourSwap());
        }
    }

    private IEnumerator ColourSwap()
    {
        int f = 0;
        while (true)
        {
            if (f++ > frame)
            {
                f = 0;
                SetStats();
            }
            yield return null;
        }
    }

    private void SetStats()
    {
        if (gripStat)
        {
            var gripModel = transform.GetChild(0);
            GripHelper.SetColour(gripModel, gripStat);
        }
    }

    void Update()
    {
        mr.enabled = shine;
    }
    private void OnTriggerEnter(Collider col)
    {
        var extremity = col.gameObject.GetComponent<Extremity>();
        if (extremity)
        {
            Lock(extremity);
        }
    }
    private void OnTriggerExit(Collider col)
    {
        var extremity = col.gameObject.GetComponent<Extremity>();
        if (extremity && extremity == inRangeExtremity)
        {
            ReleaseGrip();
        }
    }

    private void ReleaseGrip()
    {
        inRangeExtremity.GripInRange = null;
        inRangeExtremity = null;
        shine = false;
    }

    internal void Lock(Extremity extremity)
    {
        extremity.GripInRange = this;
        inRangeExtremity = extremity;
        shine = true;
    }
    internal void Lock(Node particle)
    {
        particle.lockInGrip = true;
        particle.lockPosition = this.transform.position;
        Lock(particle.model.GetComponent<Extremity>());
    }

    internal void TakeEnergy(float divider)
    {
        if (!inRangeExtremity) return;
        inRangeExtremity.TakeEnergy(gripStat, divider);
    }

    internal void PreviewEnergy(float divider)
    {
        if (!inRangeExtremity) return;
        inRangeExtremity.PreviewEnergy(gripStat, divider);
    }

    internal bool IsImpossiblegrip()
    {
        if (inRangeExtremity && inRangeExtremity.IsImpossibleGrip())
        {
            inRangeExtremity.TakeEnergy(gripStat, 1);
            ReleaseGrip();
            return true;
        }

        return false;
    }

    internal bool IsOverStreched()
    {
        return inRangeExtremity && inRangeExtremity.IsOverStreched();
    }
}
