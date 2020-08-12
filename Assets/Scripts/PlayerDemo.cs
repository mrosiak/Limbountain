using Assets.Scripts;
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verlet;
public class PlayerDemo : PlayerBase
{
    [SerializeField] int iterations = 12;

    [SerializeField] bool useGravity = true;
    [SerializeField] float gravity = 1f;
    [SerializeField] int activeJoint = 5;
    [SerializeField] List<Joint> Joints;
    [SerializeField] ParticleSystem winVfx;
    [SerializeField] AudioClip winSfx;
    [SerializeField] ParticleSystem deathVfx;
    [SerializeField] AudioClip deathSfx;
    VerletSimulator simulator;
    public List<Node> particles;
    [SerializeField] public float FreezZ = -0.2f;
    [SerializeField] public Grip LeftHandGrip;
    [SerializeField] public Grip RightHandGrip;
    [SerializeField] public Grip LeftFootGrip;
    [SerializeField] public Grip RightFootGrip;
    [SerializeField] public bool calculateTurn;
    [SerializeField] public bool isDead;
    [SerializeField] public bool isWin;
    [SerializeField] TextMesh InfoMessage;
    [SerializeField] TextMesh Score;
    [SerializeField] TextMesh Height;
    [SerializeField] int messageInSeconds = 1;
    public string InfoMessageText;
    CinemachineVirtualCamera vcam;
    public List<Grip> touchedGrips = new List<Grip>();
    bool displayingInfo = true;
    float startPoint;
    public NextButton nextButton;
    void Start()
    {
        vcam = FindObjectOfType<CinemachineVirtualCamera>();
        particles = new List<Node>();
        for (int i = 0; i < 10; i++)
        {
            var p = new Node(Vector3.down * i, Joints[i]);
            particles.Add(p);
            Joints[i].SetLimb(i);
        };
        particles[(int)LimbsEnum.Hips].lockInGrip = true;
        particles[(int)LimbsEnum.Hips].lockPosition = particles[(int)LimbsEnum.Hips].model.transform.position;
        ConnectHumanJoints();
        startPoint = Joints[(int)LimbsEnum.Hips].transform.position.y;
        SetHeight();
        simulator = new VerletSimulator(particles);
        simulator.freezeZ = FreezZ;
        PreviewEnergy();
        InfoMessageText = "Go Up";
        StartCoroutine(InfoMessageDisplay());
    }

    private IEnumerator InfoMessageDisplay()
    {
        while (true)
        {
            if (InfoMessage.text != InfoMessageText)
            {
                InfoMessage.text = InfoMessageText;
                if (isDead || isWin)
                {
                    InfoMessageText = "Press Next or Reset button";

                }
                else
                {
                    InfoMessageText = "Go Up";
                }
            }
            yield return new WaitForSeconds(messageInSeconds);
        }
    }

    private void ConnectHumanJoints()
    {
        Connect(LimbsEnum.Head, LimbsEnum.LeftElbow);
        Connect(LimbsEnum.LeftElbow, LimbsEnum.LeftHand);
        if (LeftHandGrip)
        {
            LeftHandGrip.Lock(particles.Get(LimbsEnum.LeftHand));
        }
        Connect(LimbsEnum.Head, LimbsEnum.RightElbow);
        Connect(LimbsEnum.RightElbow, LimbsEnum.RightHand);
        if (RightHandGrip)
        {
            RightHandGrip.Lock(particles.Get(LimbsEnum.RightHand));
        }
        Connect(LimbsEnum.Head, LimbsEnum.Hips);
        Connect(LimbsEnum.Hips, LimbsEnum.LeftKnee);
        Connect(LimbsEnum.LeftKnee, LimbsEnum.LeftFoot);
        if (LeftFootGrip)
        {
            LeftFootGrip.Lock(particles.Get(LimbsEnum.LeftFoot));
        }
        Connect(LimbsEnum.Hips, LimbsEnum.RightKnee);
        Connect(LimbsEnum.RightKnee, LimbsEnum.RightFoot);
        if (RightFootGrip)
        {
            RightFootGrip.Lock(particles.Get(LimbsEnum.RightFoot));
        }
    }
    void Connect(LimbsEnum limbA, LimbsEnum limbB)
    {
        var a = particles.Get(limbA);
        var b = particles.Get(limbB);
        Joint jointB = b.model.GetComponent<Joint>();
        jointB.parentJoint = a.model.GetComponent<Joint>();
        var e = new Edge(a, b, jointB.lenghtToParent);
        a.Connect(e);
        b.Connect(e);
    }

    void Update()
    {
        if (!calculateTurn)
        {
            PlayModeActions();
        }
        else
        {
            CalculateNextTurn();
        }
        MoveActive();
    }

    private void PlayModeActions()
    {
        if (Input.GetMouseButtonDown(0) && !LockExtremity())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                print(hit.transform.name);

                for (int i = 0; i < Joints.Count; i++)
                {
                    if (Joints[i].gameObject.GetInstanceID() == hit.transform.gameObject.GetInstanceID())
                    {
                        var extremity = Joints[i].GetComponent<Extremity>();
                        if (extremity)
                        {
                            Joints[i].GetComponent<Extremity>().ResetPreviewEnergy();
                            activeJoint = i;
                            ToggleGrip(activeJoint, true);
                            CalculateEnergy(false);
                        }
                        break;
                    }
                }
            }
            PreviewEnergy();
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (activeJoint != 5)
                ResetGrip(activeJoint);
        }
    }

    private void PreviewEnergy()
    {
        CalculateEnergy(false);
    }

    private void CalculateNextTurn()
    {
        if (HasGrip())
        {
            var midPoint = GetMidPoint();
            var hips = particles.Get(LimbsEnum.Hips);
            CheckForGrip();
            if (CheckBalance(hips.lockPosition, midPoint) || HeadHigherThanHands() || StreightLegs())
            {
                calculateTurn = false;
                CalculateEnergy(true);
                AddTouchedGrips();
                IsTop();
                SetHeight();
            }
            else
            {
                hips.lockPosition = Vector3.MoveTowards(particles.Get(LimbsEnum.Hips).position, midPoint, Time.deltaTime);
            }
        }
        else
        {
            calculateTurn = false;
            Die();
        }
    }

    private void SetHeight()
    {
        float newHeight = Camera.main.transform.position.y - startPoint;
        var h = newHeight.ToString("F");
        Height.text = string.Format("Height: {0}", h);
    }

    private void AddTouchedGrips()
    {
        AddTouchedGrips(LeftFootGrip);
        AddTouchedGrips(LeftHandGrip);
        AddTouchedGrips(RightHandGrip);
        AddTouchedGrips(RightFootGrip);
        var score = 0;
        touchedGrips.ForEach(g => score += g.gripStat.dificulty);
        Score.text = "Score: " + score.ToString();
    }

    private void AddTouchedGrips(Grip grip)
    {
        if (grip)
        {
            touchedGrips.Add(grip);
        }
    }

    private void IsTop()
    {
        if (LeftHandGrip && RightHandGrip)
        {
            if (LeftHandGrip.topGrip && RightHandGrip.topGrip)
            {
                var vfx = Instantiate(winVfx, LeftHandGrip.transform.position, Quaternion.identity);
                vfx.Play();
                Destroy(vfx.gameObject, vfx.main.duration);
                AudioSource.PlayClipAtPoint(winSfx, Camera.main.transform.position);
                isWin = true;
                nextButton.NextLevel = true;
                InfoMessageText = "That is a win. You can continue to another level by pressing Next.";
            }
        }
        else if (LeftHandGrip)
        {
            InfoMessageText = "You need two hands on top grip to finish the level";
        }
        else if (RightHandGrip)
        {
            InfoMessageText = "You need two hands on top grip to finish the level";
        }
    }

    private void CheckForGrip()
    {
        if (RightFootGrip && RightFootGrip.IsImpossiblegrip())
        {
            HalfPosition(LimbsEnum.RightFoot);
            HalfPosition(LimbsEnum.RightKnee);
            ResetGrip((int)LimbsEnum.RightFoot);
            RightFootGrip = null;
        }
        if (RightHandGrip && RightHandGrip.IsImpossiblegrip())
        {
            HalfPosition(LimbsEnum.RightHand);
            HalfPosition(LimbsEnum.RightElbow);
            ResetGrip((int)LimbsEnum.RightHand);
            RightHandGrip = null;
        }
        if (LeftFootGrip && LeftFootGrip.IsImpossiblegrip())
        {
            HalfPosition(LimbsEnum.LeftFoot);
            HalfPosition(LimbsEnum.LeftKnee);
            ResetGrip((int)LimbsEnum.LeftFoot);
            LeftFootGrip = null;
        }
        if (LeftHandGrip && LeftHandGrip.IsImpossiblegrip())
        {
            HalfPosition(LimbsEnum.LeftHand);
            HalfPosition(LimbsEnum.LeftElbow);
            ResetGrip((int)LimbsEnum.LeftHand);
            LeftHandGrip = null;
        }
        if (!HasGrip())
        {
            calculateTurn = false;
            Die();
        }
    }

    private void ResetGrip(int limb)
    {
        particles[limb].lockInGrip = false;
        var etremity = Joints[limb].GetComponent<Extremity>();
        if (etremity)
            etremity.ResetPreviewEnergy();
        ToggleGrip(limb, true);
        activeJoint = 5;
        particles[activeJoint].lockInGrip = true;
        PreviewEnergy();
    }

    void HalfPosition(LimbsEnum limb)
    {
        particles.Get(limb).prev = particles.Get(limb).lockPosition = particles.Get(limb).position /= 2;
    }
    private bool CheckBalance(Vector3 lockPosition, Vector3 mid)
    {
        var distance = Vector3.Distance(lockPosition, mid);
        bool balance = distance <= 0.1f;
        if (balance)
        {
            InfoMessageText = "That is perfect balance. Can't go any further up";
        }
        return balance;
    }

    private void CalculateEnergy(bool take)
    {
        TakeEnergy(take, LeftFootGrip, LimbsEnum.LeftFoot);
        TakeEnergy(take, RightFootGrip, LimbsEnum.RightFoot);
        TakeEnergy(take, LeftHandGrip, LimbsEnum.LeftHand);
        TakeEnergy(take, RightHandGrip, LimbsEnum.RightHand);
    }

    private void TakeEnergy(bool take, Grip grip, LimbsEnum limb)
    {
        if (grip)
        {
            Extremity e = Joints[(int)limb].GetComponent<Extremity>();
            if (e)
            {
                grip.inRangeExtremity = e;
            }
            if (take)
            {

                grip.TakeEnergy(GripCount());
            }
            else
            {
                grip.PreviewEnergy(GripCount());
            }
        }
    }

    private void Die()
    {
        InfoMessageText = "You failed. Press Next or Reset.";
        particles.Get(LimbsEnum.Hips).lockInGrip = false;
        vcam.LookAt = Joints[(int)LimbsEnum.Hips].transform;
        vcam.Follow = null;
        var vfx = Instantiate(deathVfx, Joints[(int)LimbsEnum.Hips].transform.position, Quaternion.identity);
        vfx.Play();
        Destroy(vfx.gameObject, vfx.main.duration);
        AudioSource.PlayClipAtPoint(deathSfx, Camera.main.transform.position);
        ResetGrip((int)LimbsEnum.RightFoot);
        ResetGrip((int)LimbsEnum.RightHand);
        ResetGrip((int)LimbsEnum.LeftFoot);
        ResetGrip((int)LimbsEnum.LeftHand);
        particles[(int)LimbsEnum.Hips].lockInGrip = false;
    }


    public void SwitchOffKinematic()
    {
        if (GripCount() == 0)
        {
            Joints.ForEach(j => j.Kinematic(false));
            isDead = true;
        }
    }
    private bool StreightLegs()
    {
        if (RightFootGrip)
        {
            if (RightFootGrip.IsOverStreched())
            {
                InfoMessageText = "Right leg is streched. Can't go any further up";
                print("right foot streched");
                return true;
            }
        }
        if (LeftFootGrip)
        {
            if (LeftFootGrip.IsOverStreched())
            {
                InfoMessageText = "Left leg is streched. Can't go any further up";
                print("left foot streched");
                return true;
            }
        }
        return false;
    }

    private bool HeadHigherThanHands()
    {
        var head = particles.Get(LimbsEnum.Head).position.y;
        var leftHand = particles.Get(LimbsEnum.LeftHand).position.y;
        var rightHand = particles.Get(LimbsEnum.RightHand).position.y;
        bool result = head > leftHand && head > rightHand;
        if (result)
        {
            InfoMessageText = "Head can't be higher than both hands. Can't go any further up";
        }
        return result;
    }

    private Vector3 GetMidPoint()
    {
        Vector3 result = Vector3.zero;
        float gripCount = GripCount();

        float divider = 1 / gripCount;
        if (LeftFootGrip)
        {
            var medium = particles.Get(LimbsEnum.LeftFoot).position * divider;
            result = medium;
        }
        if (RightFootGrip)
        {
            var medium = particles.Get(LimbsEnum.RightFoot).position * divider;
            result = result == Vector3.zero ? medium : result += medium;
        }
        if (LeftHandGrip)
        {
            var medium = particles.Get(LimbsEnum.LeftElbow).position * divider;
            result = result == Vector3.zero ? medium : result += medium;
        }
        if (RightHandGrip)
        {
            var medium = particles.Get(LimbsEnum.RightElbow).position * divider;
            result = result == Vector3.zero ? medium : result += medium;
        }
        return new Vector3(result.x, result.y);
    }
    float GripCount()
    {
        float gripCount = 0;
        if (LeftFootGrip) { gripCount += 1; }
        if (RightFootGrip) { gripCount += 1; }
        if (LeftHandGrip) { gripCount += 1; }
        if (RightHandGrip) { gripCount += 1; }
        return gripCount;
    }

    private void MoveActive()
    {
        var dt = Time.deltaTime;
        foreach (var p in particles)
        {
            if (useGravity)
            {
                var g = Vector3.down * gravity;

                p.position += dt * g;
            }
        }

        simulator.Simulate(iterations, dt);
        if (isDead)
        {
            particles.ForEach(p => p.LockToJoint());
        }
        else
        {
            particles.ForEach(p => p.AdjusTLockedPosition());
        }
        if (activeJoint != 5)
        {
            particles[activeJoint].position = Joints[activeJoint].transform.position = GetWorld();
        }

        for (int n = 0; n < particles.Count; n++)
        {
            Joints[n].transform.position = particles[n].position;
        }
    }
    private bool HasGrip()
    {
        if (((LeftFootGrip || RightFootGrip) && (LeftHandGrip || RightHandGrip)) || LeftHandGrip && RightHandGrip)
        {
            return true;
        }
        return false;
    }

    private bool LockExtremity()
    {
        if (activeJoint != 5)
        {
            Extremity extremity = Joints[activeJoint].GetComponent<Extremity>();
            if (extremity && extremity.GripInRange)
            {
                print("locking " + extremity.name + " to " + extremity.GripInRange.name);
                particles[activeJoint].lockInGrip = true;
                particles[activeJoint].lockPosition = GetWorld();
                if (extremity.IsOverStreched())
                {
                    InfoMessageText = "That grip is not going to work!";
                }
                ToggleGrip(activeJoint);
                activeJoint = 5;
                PreviewEnergy();
                return true;
            }
        }
        return false;
    }

    private void ToggleGrip(int extremity, bool switchOff = false)
    {
        var limb = (LimbsEnum)Enum.ToObject(typeof(LimbsEnum), extremity);
        switch (limb)
        {
            case LimbsEnum.LeftHand:
                {
                    LeftHandGrip = switchOff ? null : Joints[extremity].GetComponent<Extremity>().GripInRange;
                    break;
                }
            case LimbsEnum.RightHand:
                {
                    RightHandGrip = switchOff ? null : Joints[extremity].GetComponent<Extremity>().GripInRange;
                    break;
                }
            case LimbsEnum.LeftFoot:
                {
                    LeftFootGrip = switchOff ? null : Joints[extremity].GetComponent<Extremity>().GripInRange;
                    break;
                }
            case LimbsEnum.RightFoot:
                {
                    RightFootGrip = switchOff ? null : Joints[extremity].GetComponent<Extremity>().GripInRange;
                    break;
                }
            default:
                break;
        }
    }


    protected override void OnRenderObject()
    {
        base.OnRenderObject();

        RenderConnection(particles, Color.white);
    }

    void OnDrawGizmos()
    {
        if (simulator == null) return;
        simulator.DrawGizmos();
    }
    Vector3 GetWorld()
    {
        var mouse = Input.mousePosition;
        var cam = Camera.main;
        return cam.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 30f));
    }
}
