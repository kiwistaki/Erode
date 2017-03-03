using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class Utils
    {
        public static float getRealDeltaTime()
        {
            return Time.deltaTime / (Time.timeScale == 0 ? 1 : Time.timeScale);
        } 
    }
}
