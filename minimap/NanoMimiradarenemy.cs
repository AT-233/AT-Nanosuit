using System;
using System.IO;
using EFT;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace nanosuit
{
    public class NanoMimiradarenemy : MonoBehaviour
    {
        private GameObject player;
        public Image enemyuesd;
        public Image enemyuesdup;
        public Image enemyuesdlow;
        private bool findenemy;
        // Start is called before the first frame update
        void Start()
        {
            findenemy = true;
            if (player == null)
            {
                player = GameObject.Find("Nanohands");//找到玩家
            }
        }

        // Update is called once per frame
        void Update()
        {           
            float dis = Vector3.Distance(player.transform.position, this.transform.position);
            float x = (this.transform.position.x - player.transform.position.x) / 100;
            float z = (this.transform.position.z - player.transform.position.z) / 100;
            float y = (this.transform.position.y - player.transform.position.y);
            if (findenemy && dis < 100 && findenemy)
            {
                findenemy = false;
  
            }
            if (!findenemy)
            {               
                if (y > 2.5f)
                {
                    Closeenemy();
                    enemyuesdup.GetComponent<Image>().enabled = true;
                    enemyuesdup.GetComponent<RectTransform>().anchoredPosition = new Vector2(-x * 60, -z * 60);
                }
                if (y < -2.5f)
                {
                    Closeenemy();
                    enemyuesdlow.GetComponent<Image>().enabled = true;
                    enemyuesdlow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-x * 60, -z * 60);
                }
                if (y < 2.5f && y>-2.5f)
                {
                    Closeenemy();
                    enemyuesd.GetComponent<Image>().enabled = true;
                    enemyuesd.GetComponent<RectTransform>().anchoredPosition = new Vector2(-x * 60, -z * 60);
                }

            }
        }
        private void Closeenemy()
        {
            enemyuesd.GetComponent<Image>().enabled = false;
            enemyuesdup.GetComponent<Image>().enabled = false;
            enemyuesdlow.GetComponent<Image>().enabled = false;
        }
    }
}