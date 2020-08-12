using System.Collections.Generic;
using UnityEngine;

namespace Verlet
{

    public class Node {
        public List<Edge> Connection { get { return connection; } }

        public Vector3 position;
        public Vector3 prev;
        public Vector3 lockPosition;
        public bool lockInGrip;
        public Joint model;
        List<Edge> connection;
        public string Name => model.name;
        public Node(Vector3 p, Joint model)
        {
            this.model = model;
            position = prev = p;
            connection = new List<Edge>();
        }

        public void Step()
        {
            if (!lockInGrip)
            {
                var v = position - prev;
                var next = position + v;
                prev = position;
                position = next;
            }
        }

        public void Connect(Edge e)
        {
            connection.Add(e);
        }

        internal void LockToJoint()
        {
            if (!model.IsKinematic)
            {
                lockPosition = model.transform.position;
                lockInGrip = true;
            }
        }

        internal void AdjusTLockedPosition()
        {
            if (lockInGrip)
            {
                position = lockPosition;
            }
        }
    }

}


