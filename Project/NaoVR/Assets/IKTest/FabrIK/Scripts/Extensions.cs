using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.IKTest.FabrIK.Scripts
{
    public static class Extensions
    {
        public static Vector3 RemoveX(this Vector3 vector)
        {
            return new Vector3(0, vector.y, vector.z);
        }
        public static Vector3 RemoveY(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        }
        public static Vector3 RemoveZ(this Vector3 vector)
        {
            return new Vector3(vector.x, vector.y, 0);
        }
    }
}
