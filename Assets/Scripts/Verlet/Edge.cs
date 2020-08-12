using UnityEngine;

namespace Verlet
{

    public class Edge {

        public float Length { get { return length; } }
        public float Tiredness { get { return tiredness; } }
        Node a, b;
        float length;
        float tiredness;

        public Edge(Node a, Node b)
        {
            this.a = a;
            this.b = b;
            this.length = (a.position - b.position).magnitude;
        }

        public Edge(Node a, Node b, float len)
        {
            this.a = a;
            this.b = b;
            this.length = len;
        }

        public Node Other(Node p)
        {
            if(a == p)
            {
                return b;
            } else
            {
                return a;
            }
        }

        internal Color GetColour()
        {
            var magnitude = (a.position - b.position).magnitude;
            if (magnitude > length+0.2f)
            {
                if (magnitude > length * 2)
                {
                    return Color.black;
                }
                return Color.red;
            }
            return Color.white;
        }
    }

}


