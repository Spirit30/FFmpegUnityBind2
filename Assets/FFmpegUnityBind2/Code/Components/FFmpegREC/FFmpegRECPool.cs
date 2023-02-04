using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FFmpegUnityBind2.Components
{
    class FFmpegRECPool : MonoBehaviour
    {
        [SerializeField]
        List<FFmpegREC> recorders = null;

        public FFmpegREC GetWhichIdle()
        {
            var result = recorders.FirstOrDefault(r => r && r.State == FFmpegRECState.Idle);

            if (result)
            {
                return result;
            }

            result = Instantiate(recorders.First());
            recorders.Add(result);
            return result;
        }
    }
}