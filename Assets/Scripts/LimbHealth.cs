using UnityEngine;

public class LimbHealth : MonoBehaviour
{
    [SerializeField] public bool horizontal;
    [SerializeField] public float health = 100;
    [SerializeField] public float previewHealth = 100;
    TextMesh label;
    SpriteRenderer healthBar;
    SpriteRenderer previewHealthBar;
    TextMesh Label
    {
        get
        {
            if (!label)
            {
                label = GetComponentInChildren<TextMesh>();
            }
            return label;
        }
    }
    SpriteRenderer HealthBar
    {
        get
        {
            if (!healthBar)
            {
                healthBar = transform.Find("healthBar").GetComponent<SpriteRenderer>();
            }
            return healthBar;
        }
    }
    SpriteRenderer PreviewHealthBar
    {
        get
        {
            if (!previewHealthBar)
            {
                previewHealthBar = transform.Find("previewHealthBar").GetComponent<SpriteRenderer>();
            }
            return previewHealthBar;
        }
    }
    void Start()
    {;
        SetLabel(health);
    }

    internal void TakeEnergy(GripStat gripStat, float divider)
    {
        var energy = GetEffort(gripStat, divider);
        health -= energy;
        previewHealth = health;
        SetHealhtBar();
    }


    internal void PreviewEnergy(GripStat gripStat, float divider)
    {
        var energy = GetEffort(gripStat, divider);
        SetPreviewHealhtBar(energy);
    }
    private float GetEffort(GripStat gripStat, float divider)
    {
        if (divider == 0)
        {
            divider++;
        }
        return (Mathf.Pow(gripStat.dificulty, 2) + 1) / divider;
    }

    void SetLabel(float h)
    {
        int a = (int)h;
        if (a < 0)
        {
            a = 0;
        }
        Label.text = string.Format("{0}%", a);
    }
    public void SetHealhtBar()
    {
        PreviewHealthBar.transform.localScale = GetScale(health);
        HealthBar.transform.localScale = GetScale(health);
        SetLabel(health);
    }
    public void SetPreviewHealhtBar(float preview)
    {
        previewHealth = health - preview;
        PreviewHealthBar.transform.localScale = GetScale(previewHealth);
        SetLabel(health - preview);
    }
    Vector3 GetScale(float h)
    {
        var energy = h / 100f;
        if (energy < 0)
        {
            energy = 0;
        }
        return horizontal ? new Vector3(energy, 1, 1) : new Vector3(1, energy, 1);
    }
}
