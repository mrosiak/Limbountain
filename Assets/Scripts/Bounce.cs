using UnityEngine;

public class Bounce : MonoBehaviour
{
    BoxCollider boxCollider;
    PlayerDemo player;
    private void Start()
    {
        player = FindObjectOfType<PlayerDemo>();
        boxCollider = GetComponent<BoxCollider>();
    }
    private void OnCollisionEnter(Collision col)
    {
        print("mat " + col.transform.name);
        if (col.transform.tag == "death")
        {

        }
    }
    private void OnTriggerEnter(Collider col)
    {
        print("triggerx mat " + col.name);
        Joint j = col.GetComponent<Joint>();
        if (j)
        {
            boxCollider.isTrigger = false;
            player.SwitchOffKinematic();
        }
    }
}
