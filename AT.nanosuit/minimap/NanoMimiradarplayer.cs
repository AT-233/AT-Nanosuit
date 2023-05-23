using System;
using EFT;
using UnityEngine;
using UnityEngine.UI;

namespace nanosuit
{
    public class NanoMimiradarplayer : MonoBehaviour
    {
        private GameObject player;
        // Start is called before the first frame update
        void Start()
        {            
        }

        // Update is called once per frame
        void Update()
        {
            if(nanosuitcore.Lockradar.Value)
            {
                if (player == null)
                {
                    player = GameObject.Find("Nanohands");//ÕÒµ½Íæ¼Ò
                }
                this.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, -player.transform.eulerAngles.y + 180);
            }           
        }
    }
}
