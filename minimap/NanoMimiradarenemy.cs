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
        public Image radarback;
        public Image target;
        public Image targetup;
        public Image targetlow;
        private Transform targettransform;
        private bool findenemy;
        private Vector2 pos;
        // Start is called before the first frame update
        void Start()
        {            
            findenemy = true;
            targettransform = this.transform;
            if (player == null)
            {
                player = GameObject.Find("Nanohands");//找到玩家
            }
        }

        // Update is called once per frame
        void Update()
        {
            float dis = Vector3.Distance(player.transform.position, this.transform.position);
            float dismax = 0;
            float x = (this.transform.position.x - player.transform.position.x) / 100;
            float z = (this.transform.position.z - player.transform.position.z) / 100;
            float y = (this.transform.position.y - player.transform.position.y);
            dismax = dis;
            if (dismax >= 100) dismax = 100;
            if (!nanosuitcore.Lockradar.Value)
            {
                if (player == null)
                {
                    player = GameObject.Find("Nanohands");//找到玩家
                }
                radarback.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, player.transform.eulerAngles.y + 180);
            }
            if (findenemy && dis < 100)
            {
                findenemy = false;
            }
            if (!findenemy)
            {
                if (y > 2.5f)
                {
                    Closeenemy();
                    Closeenemyimage();
                    enemyuesdup.GetComponent<Image>().enabled = true;
                    targetup.GetComponent<Image>().enabled = true;
                    enemyuesdup.GetComponent<RectTransform>().anchoredPosition = new Vector2(-x * 60, -z * 60);
                }
                if (y < -2.5f)
                {
                    Closeenemy();
                    Closeenemyimage();
                    enemyuesdlow.GetComponent<Image>().enabled = true;
                    targetlow.GetComponent<Image>().enabled = true;
                    enemyuesdlow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-x * 60, -z * 60);
                }
                if (y < 2.5f && y > -2.5f)
                {
                    Closeenemy();
                    Closeenemyimage();
                    enemyuesd.GetComponent<Image>().enabled = true;
                    target.GetComponent<Image>().enabled = true;
                    enemyuesd.GetComponent<RectTransform>().anchoredPosition = new Vector2(-x * 60, -z * 60);
                }
            }
            targettransform.position = new Vector3(this.transform.position.x, this.transform.position.y+1.8f, this.transform.position.z); 
            float minX = target.GetPixelAdjustedRect().width / 2;
            float maxX = Screen.width - minX;
            float minY = target.GetPixelAdjustedRect().height / 2;
            float maxY = Screen.height - minY;
            target.GetComponent<RectTransform>().localScale = new Vector3(1.3f - (dismax / 100)*1, 1.3f - (dismax / 100) * 1, 1);
            targetlow.GetComponent<RectTransform>().localScale = new Vector3(1.3f - (dismax / 100) * 1, 1.3f - (dismax / 100) * 1, 1);
            targetup.GetComponent<RectTransform>().localScale = new Vector3(1.3f - (dismax / 100) *1, 1.3f - (dismax / 100) *1, 1);
            pos = Camera.main.WorldToScreenPoint(targettransform.position);
            //pos.x = Camera.main.WorldToScreenPoint(targettransform.position).x * Screen.width / Camera.main.pixelWidth;
            //pos.y = Camera.main.WorldToScreenPoint(targettransform.position).y * Screen.height / Camera.main.pixelHeight;
            if (Vector3.Dot((targettransform.position - nanosuit.marktarget.transform.position), nanosuit.marktarget.transform.forward) < 0)
            {
                if (pos.x < Screen.width / 2)
                {
                    pos.x = maxX;
                }
                else pos.x = minX;
            }
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            target.transform.position = pos;
            targetlow.transform.position = pos;
            targetup.transform.position = pos;
        }
        private void Closeenemy()
        {
            enemyuesd.GetComponent<Image>().enabled = false;
            enemyuesdup.GetComponent<Image>().enabled = false;
            enemyuesdlow.GetComponent<Image>().enabled = false;
        }
        private void Closeenemyimage()
        {
            target.GetComponent<Image>().enabled = false;
            targetup.GetComponent<Image>().enabled = false;
            targetlow.GetComponent<Image>().enabled = false;
        }
    }
}