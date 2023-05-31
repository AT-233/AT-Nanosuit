
using Comfort.Common;
using EFT;
using UnityEngine;

namespace nanosuit
{
    
    public class gausslink : MonoBehaviour
    {
        private static GameWorld gameWorld;
        private GameObject gaussmod;
        public Animation gaussani;
        //public static bool Entermap() => Singleton<GameWorld>.Instantiated && gameWorld.AllPlayers != null && gameWorld.AllPlayers.Count > 1;
        // Start is called before the first frame update
        void Start()
        {
            //gameWorld = Singleton<GameWorld>.Instance;
            //gaussmod = GameObject.Find("gaussbase");
            //gaussani = gaussmod.GetComponent<Animation>();
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetMouseButtonDown(0) && gaussani != null)
            //{
            //    gaussmod.GetComponent<Animation>().Play();
            //    gaussani[gaussani.clip.name].time = 0;
            //    gaussani[gaussani.clip.name].speed = 1;
            //    gaussani.Play(gaussani.clip.name);
            //}
        }
    }
}
