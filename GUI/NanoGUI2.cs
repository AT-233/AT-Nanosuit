using SPT.Reflection.Patching;
using Comfort.Common;
using EFT;
using EFT.HealthSystem;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using EFT.InventoryLogic;
using HarmonyLib;
using System.Collections;

namespace nanosuit
{
    public class NanoGUI2 : MonoBehaviour
    {
        public AudioClip[] nanoaudios;
        public RawImage AmorMode;
        public RawImage CloakMode;
        public RawImage PowerMode;
        public RawImage EnemyFound;
        public Image basepack;
        public RawImage Energyico;
        public RawImage ico;
        public Text EnergyText;
        public Text LowEnergy;
        public Text ammonow;
        public Text ammomax;
        public Image Energybase;
        public GameObject ammopack;
        public static bool befound2;
        private static GameWorld gameWorld;
        private float timeFade;
        private float jumpFade;
        private float Energyfill;
        public static bool isweapon;
        public static bool ready;
        private static bool firstspeed;
        private static bool firstrunning;
        private Weapon NowWeapon;
        private static Player NanoPlayer;
        private Player.FirearmController NowFirearmController;
        private Item NowItemController;
        void Start()
        {
            gameWorld = Singleton<GameWorld>.Instance;
            timeFade = 0f;
            Energyfill = 1f;
            jumpFade = Physics.gravity.y;
            befound2 = false;
            ready = true;
            isweapon = false;
            firstrunning = false;
            firstspeed = false;
            new NanoWaring().Enable();//启动AI警告
        }
        void Update()
        {
            if (nanosuitcore.AmmoUIOnline.Value)
            {
                ammopack.SetActive(true);
                if (NanoPlayer == null)
                {
                    NanoPlayer = gameWorld.MainPlayer;
                    Console.WriteLine("玩家正常");
                }
                if (isweapon && NanoPlayer != null && NanoPlayer.HandsController != null)
                {
                    NowFirearmController = NanoPlayer.HandsController as Player.FirearmController;
                }
                if (NowFirearmController != null && NanoPlayer != null && NowFirearmController.Item != null)
                {
                    NowWeapon = NowFirearmController.Item;
                }
                if (NanoPlayer != null && NanoPlayer.HandsController.Item != null)
                {
                    NowItemController = NanoPlayer.HandsController.Item;
                }
                if (NowItemController != null && NanoPlayer != null)
                {
                    if (NowItemController.Template.UnlootableFromSlot == "FirstPrimaryWeapon" && NowItemController.Template._parent != "543be6564bdc2df4348b4568")
                    {
                        isweapon = true;
                    }
                    if (NowItemController.Template._parent == "543be6564bdc2df4348b4568"|| 
                        NowItemController.Template.UnlootableFromSlot == "Scabbard"|| 
                        NowItemController.Template._parent == "5448e8d04bdc2ddf718b4569" || 
                        NowItemController.Template._parent == "5448e8d64bdc2dce718b4568"|| 
                        NowItemController.Template._parent == "5448f39d4bdc2d0a728b4568" || 
                        NowItemController.Template._parent == "5448f3a64bdc2d60728b456a" || 
                        NowItemController.Template._parent == "5448f3a14bdc2d27728b4569" || 
                        NowItemController.Template._parent == "5448f3ac4bdc2dce718b4569")
                    {
                        isweapon = false;
                    }
                }
                if (NowWeapon != null && NowWeapon.GetCurrentMagazine() != null && isweapon)
                {
                    int maxMagazineCount = NowWeapon.GetMaxMagazineCount();
                    int count = NowWeapon.GetCurrentMagazineCount() + NowWeapon.ChamberAmmoCount;
                    ammonow.GetComponent<Text>().text = count.ToString();
                    ammomax.GetComponent<Text>().text = maxMagazineCount.ToString();
                }
                if (NowWeapon != null && NowWeapon.GetCurrentMagazine() == null && isweapon)
                {
                    int count = NowWeapon.ChamberAmmoCount;
                    ammonow.GetComponent<Text>().text = count.ToString();
                    ammomax.GetComponent<Text>().text = count.ToString();
                }
                if(!isweapon)
                {
                    ammonow.GetComponent<Text>().text = "000";
                    ammomax.GetComponent<Text>().text = "000";
                }
            }
            else ammopack.SetActive(false);
            if (EnergyText != null)
            {
                EnergyText.GetComponent<Text>().text = nanosuit.nowenergy.ToString();
                if (nanosuit.maxenergy > 95 && nanosuit.maxenergy <= 100) Energyfill = 1f;
                if (nanosuit.maxenergy > 90 && nanosuit.maxenergy <= 95) Energyfill = 0.95f;
                if (nanosuit.maxenergy > 85 && nanosuit.maxenergy <= 90) Energyfill = 0.9f;
                if (nanosuit.maxenergy > 80 && nanosuit.maxenergy <= 85) Energyfill = 0.85f;
                if (nanosuit.maxenergy > 75 && nanosuit.maxenergy <= 80) Energyfill = 0.8f;
                if (nanosuit.maxenergy > 70 && nanosuit.maxenergy <= 75) Energyfill = 0.75f;
                if (nanosuit.maxenergy > 65 && nanosuit.maxenergy <= 70) Energyfill = 0.7f;
                if (nanosuit.maxenergy > 60 && nanosuit.maxenergy <= 65) Energyfill = 0.65f;
                if (nanosuit.maxenergy > 55 && nanosuit.maxenergy <= 60) Energyfill = 0.6f;
                if (nanosuit.maxenergy > 50 && nanosuit.maxenergy <= 55) Energyfill = 0.55f;
                if (nanosuit.maxenergy > 45 && nanosuit.maxenergy <= 50) Energyfill = 0.5f;
                if (nanosuit.maxenergy > 40 && nanosuit.maxenergy <= 45) Energyfill = 0.45f;
                if (nanosuit.maxenergy > 35 && nanosuit.maxenergy <= 40) Energyfill = 0.4f;
                if (nanosuit.maxenergy > 30 && nanosuit.maxenergy <= 35) Energyfill = 0.35f;
                if (nanosuit.maxenergy > 25 && nanosuit.maxenergy <= 30) Energyfill = 0.3f;
                if (nanosuit.maxenergy > 20 && nanosuit.maxenergy <= 25) Energyfill = 0.25f;
                if (nanosuit.maxenergy > 15 && nanosuit.maxenergy <= 20) Energyfill = 0.2f;
                if (nanosuit.maxenergy > 10 && nanosuit.maxenergy <= 15) Energyfill = 0.15f;
                if (nanosuit.maxenergy > 5 && nanosuit.maxenergy <= 10) Energyfill = 0.1f;
                if (nanosuit.maxenergy > 0 && nanosuit.maxenergy <= 5) Energyfill = 0.05f;
                if (nanosuit.maxenergy == 0) Energyfill = 0f;
                Energybase.GetComponent<Image>().fillAmount = Energyfill;
                if (nanosuit.maxenergy <= 20)
                {
                    LowEnergy.GetComponent<Text>().enabled = true;
                    Energybase.GetComponent<Image>().color = Color.red;
                    ico.GetComponent<RawImage>().color = Color.red;
                    Energyico.GetComponent<RawImage>().color = Color.red;
                }
                else
                {
                    LowEnergy.GetComponent<Text>().enabled = false;
                    Energybase.GetComponent<Image>().color = Color.white;
                    ico.GetComponent<RawImage>().color = Color.white;
                    Energyico.GetComponent<RawImage>().color = Color.white;
                }
            }
            if (basepack != null)
            {
                basepack.GetComponent<RectTransform>().anchoredPosition = new Vector3(nanosuitcore.Xzhou.Value, nanosuitcore.Yzhou.Value, 0);
                basepack.GetComponent<RectTransform>().eulerAngles = new Vector3(nanosuitcore.Xzhouxuan.Value, nanosuitcore.Yzhouxuan.Value, nanosuitcore.Zzhouxuan.Value);
                if (nanosuit.isfly|| nanosuit.ispower|| nanosuit.isspeed)
                {
                    PowerMode.GetComponent<RawImage>().enabled = true;
                    Energyico.GetComponent<RawImage>().enabled = true;
                    ico.GetComponent<RawImage>().enabled = true;
                    if (Input.GetKeyDown(KeyCode.Space) && nanosuit.ispower)
                    {
                        if (Physics.gravity.y == jumpFade)
                        {
                            StartCoroutine("PowerJump");
                            PowerMode.GetComponent<AudioSource>().volume = nanosuitcore.nanovolume.Value;
                            PowerMode.GetComponent<AudioSource>().clip = nanoaudios[0];
                            PowerMode.GetComponent<AudioSource>().Play();
                            nanosuit.maxenergy -= nanosuitcore.powerjumpcost.Value;
                        }
                    }
                    if (nanosuit.isspeed)
                    {
                        if (NanoPlayer.IsSprintEnabled && !firstspeed)
                        {
                            PowerMode.GetComponent<AudioSource>().volume = nanosuitcore.nanovolume.Value;
                            PowerMode.GetComponent<AudioSource>().clip = nanoaudios[1];
                            PowerMode.GetComponent<AudioSource>().Play();
                            firstspeed = true;
                        }
                        if (!NanoPlayer.IsSprintEnabled && firstspeed)
                        {
                            firstrunning = true;
                        }
                        if (firstspeed && firstrunning)
                        {
                            firstspeed = false;
                            firstrunning = false;
                        }
                    }
                }
                if (!nanosuit.isfly && !nanosuit.ispower && !nanosuit.isspeed)
                {
                    PowerMode.GetComponent<RawImage>().enabled = false;
                }
                if (nanosuit.isarmor)
                {
                    AmorMode.GetComponent<RawImage>().enabled = true;
                    Energyico.GetComponent<RawImage>().enabled = true;
                    ico.GetComponent<RawImage>().enabled = true;
                }
                else AmorMode.GetComponent<RawImage>().enabled = false;
                if (nanosuit.isstealth)
                {
                    CloakMode.GetComponent<RawImage>().enabled = true;
                    Energyico.GetComponent<RawImage>().enabled = true;
                    ico.GetComponent<RawImage>().enabled = true;
                }
                else CloakMode.GetComponent<RawImage>().enabled = false;
                if (nanosuit.isnanovision)
                {
                    Energyico.GetComponent<RawImage>().enabled = true;
                    ico.GetComponent<RawImage>().enabled = true;
                }
                if (!nanosuit.isfly && !nanosuit.ispower && !nanosuit.isspeed && !nanosuit.isstealth && !nanosuit.isarmor && !nanosuit.isnanovision)
                {
                    Energyico.GetComponent<RawImage>().enabled = false;
                    ico.GetComponent<RawImage>().enabled = false;
                }
            }
            if (befound2 && ready && EnemyFound != null)
            {
                EnemyFound.GetComponent<RawImage>().enabled = true;
                ready = false;
                befound2 = false;
                if (!nanosuit.isstealth && !nanosuit.isarmor && !nanosuit.ispower)
                {
                    nanosuit.isautoarmor = true;
                }
                Console.WriteLine("被锁定");
            }
            if (!ready)
            {
                timeFade += Time.deltaTime;
                if (timeFade >= 3)
                {
                    timeFade = 0;
                    ready = true;
                    EnemyFound.GetComponent<RawImage>().enabled = false;
                }
            }
        }
        IEnumerator PowerJump()
        {
            float Nowgravity;
            Nowgravity = 0f;
            while (Nowgravity > jumpFade)
            {
                Nowgravity -= 0.09f;
                if (Nowgravity <= jumpFade)
                {
                    Nowgravity = jumpFade;
                }
                Physics.gravity = new Vector3(0, Nowgravity, 0);
                yield return 0;
            }
        }
    }   
}
