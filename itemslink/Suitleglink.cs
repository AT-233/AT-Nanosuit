
using System.Collections;
using UnityEngine;

namespace nanosuit
{
    public class Suitleglink : MonoBehaviour
    {
        public Material legstealthmaterial;
        public Material legarmormaterial;
        public Material basematerial;
        private Material[] suitlegmaterial;
        private bool iscloak = false;
        private Color cloakColor = Color.white;
        void Start()
        {
            suitlegmaterial = new Material[this.GetComponent<SkinnedMeshRenderer>().materials.Length];
            for (int i = 0; i < suitlegmaterial.Length; i++)//获取物体全部材质
            {
                suitlegmaterial[i] = this.GetComponent<SkinnedMeshRenderer>().materials[i];
            }
            suitlegmaterial[0] = basematerial;//设置默认材质
            iscloak = false;
        }
        void Update()
        {
            if (nanosuit.isarmor)
            {
                if (suitlegmaterial[0] != legarmormaterial)
                {
                    suitlegmaterial[0] = legarmormaterial;
                    this.GetComponent<SkinnedMeshRenderer>().materials = suitlegmaterial;
                    iscloak = false;
                }
            }
            if (nanosuit.isstealth)
            {
                if (suitlegmaterial[0] != legstealthmaterial)
                {
                    suitlegmaterial[0] = legstealthmaterial;
                    this.GetComponent<SkinnedMeshRenderer>().materials = suitlegmaterial;
                    StartCoroutine("CloakSuitleg");
                    iscloak = true;
                }
            }
            if (!nanosuit.isstealth && !nanosuit.isarmor && !iscloak)
            {
                if (suitlegmaterial[0] != basematerial)
                {
                    suitlegmaterial[0] = basematerial;
                    this.GetComponent<SkinnedMeshRenderer>().materials = suitlegmaterial;
                }
            }
            if (!nanosuit.isstealth && !nanosuit.isarmor && iscloak)
            {
                if (suitlegmaterial[0] != basematerial)
                {
                    StartCoroutine("DeCloakSuitleg");
                }
            }
        }
        IEnumerator CloakSuitleg()
        {
            float cloakFade = 0.9f;
            cloakColor.r = cloakFade;
            cloakColor.g = cloakFade;
            cloakColor.b = cloakFade;
            legstealthmaterial.SetColor("_Tint", cloakColor);

            while (cloakFade >= 0.5f)
            {

                cloakFade -= 0.025f;
                cloakColor.r = cloakFade;
                cloakColor.g = cloakFade;
                cloakColor.b = cloakFade;
                legstealthmaterial.SetColor("_Tint", cloakColor);
                yield return 0;

            }

        }
        IEnumerator DeCloakSuitleg()
        {
            float cloakFade = 0.5f;
            cloakColor.r = cloakFade;
            cloakColor.g = cloakFade;
            cloakColor.b = cloakFade;
            legstealthmaterial.SetColor("_Tint", cloakColor);

            while (cloakFade <= 0.9f)
            {

                cloakFade += 0.04f;
                cloakColor.r = cloakFade;
                cloakColor.g = cloakFade;
                cloakColor.b = cloakFade;
                legstealthmaterial.SetColor("_Tint", cloakColor);
                yield return 0;

                if (cloakFade >= 0.9f)
                {
                    iscloak = false;
                }
            }
        }
    }
}
