using Comfort.Common;
using EFT;
using UnityEngine;

namespace nanosuit
{
    public class NanoMiniMapCompass : MonoBehaviour
    {

        private Transform Target;
        public RectTransform CompassRoot;
        public RectTransform North;
        public RectTransform South;
        public RectTransform East;
        public RectTransform West;
        [HideInInspector] public int Grade;
        private static GameWorld gameWorld;

        private int Rotation;

        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            gameWorld = Singleton<GameWorld>.Instance;
            Target = gameWorld.MainPlayer.Transform.Original;
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            gameWorld = Singleton<GameWorld>.Instance;
            //return always positive
            if (Target != null)
            {
                Rotation = (int)Mathf.Abs(Target.eulerAngles.y);
            }
            //else
            //{
            //    Target = gameWorld.MainPlayer.Transform.Original;
            //    Rotation = (int)Mathf.Abs(m_Transform.eulerAngles.y);
            //}
            Rotation = Rotation % 360;//return to 0 


            Grade = Rotation;
            //opposite angle
            if (Grade > 180)
            {
                Grade = Grade - 360;
            }
            float cm = CompassRoot.sizeDelta.x * 0.5f;
            if (North != null) North.anchoredPosition = new Vector2((cm - (Grade * 2) - cm), 0);
            if (South != null) South.anchoredPosition = new Vector2((cm - Rotation * 2 + 360) - cm, 0);
            if (East != null) East.anchoredPosition = new Vector2((cm - Grade * 2 + 180) - cm, 0);
            if (West != null) West.anchoredPosition = new Vector2((cm - Rotation * 2 + 540) - cm, 0);            
        }

        float angle3602(Vector3 from, Vector3 to, Vector3 right)
        {
            float angle = Vector3.Angle(from, to);
            Vector3 cross = Vector3.Cross(from, to);
            if (cross.y < 0) { angle = -angle; }
            return angle;
        }

        public float Angle360(Vector2 p1, Vector2 p2, Vector2 o = default(Vector2))
        {
            Vector2 v1, v2;
            if (o == default(Vector2))
            {
                v1 = p1.normalized;
                v2 = p2.normalized;
            }
            else
            {
                v1 = (p1 - o).normalized;
                v2 = (p2 - o).normalized;
            }
            float angle = Vector2.Angle(v1, v2);
            return Mathf.Sign(Vector3.Cross(v1, v2).z) < 0 ? (360 - angle) % 360 : angle;
        }

        //private Transform t;
        //private Transform m_Transform
        //{
        //    get
        //    {
        //        if (t == null)
        //        {
        //            t = this.GetComponent<Transform>();
        //        }
        //        return t;
        //    }
        //}
    }
}