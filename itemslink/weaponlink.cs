
using System.Collections;
using UnityEngine;

namespace nanosuit
{
    public class weaponlink : MonoBehaviour
    {
        public Material stealthmaterial;
        public Material basematerial;
        Material[] weaponmaterial;
        private bool iscloak = false;
        private Color cloakColor = Color.white;
        void Start()
        {
            weaponmaterial = new Material[this.GetComponent<MeshRenderer>().materials.Length];
            for (int i = 0; i < weaponmaterial.Length; i++)//获取物体全部材质
            {
                weaponmaterial[i] = this.GetComponent<MeshRenderer>().materials[i];
            }
            weaponmaterial[0] = basematerial;//设置默认材质
        }
        void Update()
        {
            if (nanosuit.isstealth && !iscloak)
            {
                weaponmaterial[0] = stealthmaterial;
                this.GetComponent<MeshRenderer>().materials = weaponmaterial;
                StartCoroutine("CloakWeapon");
                iscloak = true;
            }
            if (!nanosuit.isstealth && !iscloak)
            {
                weaponmaterial[0] = basematerial;
                this.GetComponent<MeshRenderer>().materials = weaponmaterial;
            }
            if (!nanosuit.isstealth && iscloak)
            {
                StartCoroutine("DeCloakWeapon");
            }
        }
        IEnumerator CloakWeapon()
        {
            float cloakFade = 0.9f;
            cloakColor.r = cloakFade;
            cloakColor.g = cloakFade;
            cloakColor.b = cloakFade;
            stealthmaterial.SetColor("_Tint", cloakColor);

            while (cloakFade >= 0.5f)
            {

                cloakFade -= 0.025f;
                cloakColor.r = cloakFade;
                cloakColor.g = cloakFade;
                cloakColor.b = cloakFade;
                stealthmaterial.SetColor("_Tint", cloakColor);
                yield return 0;

            }

        }
        IEnumerator DeCloakWeapon()
        {
            float cloakFade = 0.5f;
            cloakColor.r = cloakFade;
            cloakColor.g = cloakFade;
            cloakColor.b = cloakFade;
            stealthmaterial.SetColor("_Tint", cloakColor);

            while (cloakFade <= 0.9f)
            {

                cloakFade += 0.04f;
                cloakColor.r = cloakFade;
                cloakColor.g = cloakFade;
                cloakColor.b = cloakFade;
                stealthmaterial.SetColor("_Tint", cloakColor);
                yield return 0;

                if (cloakFade >= 0.9f)
                {
                    iscloak = false;
                }
            }
        }
    }
}
