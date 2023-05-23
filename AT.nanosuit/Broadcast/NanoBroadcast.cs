using System.Reflection;
using Aki.Reflection.Patching;
using EFT;
using UnityEngine;

namespace nanosuit
{
    public class NanoBroadcast : MonoBehaviour
    {
        public AudioClip[] Bt7274Audio;
        public static bool killone;
        public static bool isready;
        private int voicechoose = 0;
        private float readytime = 0;
        private bool isbroad;
        // Start is called before the first frame update
        void Start()
        {
            isbroad = true;            
        }

        // Update is called once per frame
        void Update()
        {
            if (nanosuitcore.BroadcastOnline.Value && isbroad)
            {
                new NanoSystemPatch().Enable();
                voicechoose = 0;
                killone = false;
                isready = true;
                isbroad = false;
                readytime = 0;
            }
            if (!nanosuitcore.BroadcastOnline.Value && !isbroad)
            {
                new NanoSystemPatch().Disable();
                voicechoose = 0;
                killone = false;
                isready = true;
                isbroad = true;
                readytime = 0;
            }
            if (killone && isready)
            {
                this.GetComponent<AudioSource>().volume = nanosuitcore.nanovolume.Value;
                this.GetComponent<AudioSource>().clip = Bt7274Audio[voicechoose];
                this.GetComponent<AudioSource>().Play();
                killone = false;
                isready = false;
                voicechoose ++;
                if (voicechoose == 4)
                { 
                    voicechoose = 0; 
                }
            }
            if (killone && !isready)
            {
                killone = false;
            }
            if (!isready)
            {
                readytime += Time.deltaTime;
                if(readytime > 3)
                {
                    isready = true;
                    readytime = 0;
                }
            }
        }      
    }
    public class NanoSystemPatch : ModulePatch
    {
        //搜索EFT空间下的Player类里的OnBeenKilledByAggressor
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("OnBeenKilledByAggressor", BindingFlags.Instance | BindingFlags.NonPublic);
        }
        [PatchPostfix]
        private static void PatchPostFix(ref Player __instance, Player aggressor)
        {
            if (aggressor.IsYourPlayer)
            {
                NanoBroadcast.killone = true;
            }
            if (nanosuitcore.nanoup.Value == "能量吸收(Energy absorption)")
            {
                nanosuit.maxenergy+=30;
                if(nanosuit.maxenergy>=100) nanosuit.maxenergy = 100;
            }
        }
    }
    
}
