using System.Collections.Generic;
using Verlet;

namespace Assets.Scripts
{
   public static class ExtentionMethods
    {
        public static int ToInt(this LimbsEnum limb)
        {
            return (int)limb;
        }
        public static Node Get(this List<Node> particles, LimbsEnum limb)
        {
            return particles[(int)limb];
        }
    }
}
