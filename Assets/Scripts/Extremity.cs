public class Extremity : Joint
{
    public Grip GripInRange;
    public LimbHealth health;

    protected override void Update()
    {
        base.Update();
    }

    internal void TakeEnergy(GripStat gripStat, float divider)
    {
        health.TakeEnergy(gripStat, divider);
    }

    internal void PreviewEnergy(GripStat gripStat, float divider)
    {
        health.PreviewEnergy(gripStat, divider);
    }
    internal void ResetPreviewEnergy()
    {
        health.SetPreviewHealhtBar(0);
    }

    internal bool IsOverStreched()
    {
        float max = lenghtToParent + parentJoint.lenghtToParent + 0.2f;
        float currentLenght = parentMagnitude + parentJoint.parentMagnitude;
        return currentLenght > max;
    }
    internal bool IsImpossibleGrip()
    {
        float max1 = lenghtToParent * 2;
        float max2 = parentJoint.lenghtToParent * 2;
        return max1 < parentMagnitude || max2 < parentJoint.parentMagnitude || health.previewHealth <= 0 || health.health <= 0; ;
    }
}
