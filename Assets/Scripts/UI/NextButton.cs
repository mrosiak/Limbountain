using UnityEngine;

public class NextButton : MonoBehaviour
{
    SpriteRenderer sprite;
    [Range(0, 1)] [SerializeField] public float dim = 0.5f;
    [SerializeField] public bool NextLevel;
    [SerializeField] public bool RestartLevel;
    PlayerDemo demo;
    PlayerDemo Demo
    {
        get
        {
            if (!demo)
            {
                demo = FindObjectOfType<PlayerDemo>();
            }
            return demo;
        }
    }

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    void OnMouseUp()
    {
        if (NextLevel && Demo.isWin)
        {
            SceneLoader.LoadNextSceneStatic();
        }
        else if (RestartLevel || Demo.isDead)
        {
            SceneLoader.RestartSceneStatic();
        }
        else
        {
            Demo.nextButton = this;
            Demo.calculateTurn = true;
        }
    }
    void OnMouseEnter()
    {
        Dim(dim);
    }

    void OnMouseExit()
    {
        Dim(1);
    }
    private void Dim(float d)
    {
        Color tmp = sprite.color;
        tmp.a = d;
        sprite.color = tmp;
    }
}
