using UnityEngine;

namespace nanosuit
{
    public class armorhit : MonoBehaviour
    {
        private float costenergy;
        private float getenergy;
        public int modekey;
        public void cloakhit_method(bool IsYourPlayer)
        {
            getenergy = nanosuit.maxenergy;
            if (modekey == 2)
            {
                if (IsYourPlayer)
                {
                    getenergy = 0;
                }
                else
                {
                    costenergy = 15;
                    getenergy = getenergy - costenergy;
                }
            }
            nanosuit.maxenergy = getenergy;
        }
    }
}
