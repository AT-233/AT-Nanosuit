using Aki.Reflection.Patching;
using Comfort.Common;
using EFT;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using EFT.InventoryLogic;
using HarmonyLib;
using System.Collections;

namespace nanosuit
{
    public class NanoGUI : MonoBehaviour
    {
        public RawImage NormalMode;
        public RawImage AmorMode;
        public RawImage CloakMode;
        public RawImage SpeedMode;
        public RawImage PowerMode;
        public RawImage FlyMode;
        public RawImage EnemyFound;
        public Image basepack;
        public Image NanoHealth;
        public Image NanoHealth1;
        public Image NanoEnergy;
        public Image smg;
        public Image machinegun;
        public Image shotgun;
        public Image assaultRifle;
        public Image marksmanRifle;
        public Image pistol;
        public Image sniperRifle;
        public Image grenadeLauncher;
        public Image noweapon;
        public Image grenade;
        public Image knife;
        public Image food;
        public Image med;
        public Text HealthText;
        public Text Health1Text;
        public Text EnergyText;
        public Text AmmoText;
        public Text LowEnergy;
        public GameObject ammopack;
        private static GameWorld gameWorld;
        public Color EnergyColorbase;
        public static bool befound;
        public static bool ready;
        private float timeFade;
        private float jumpFade;
        private bool isweapon;
        private Weapon NowWeapon;
        private static Player NanoPlayer;
        private Player.FirearmController NowFirearmController;
        private Item NowItemController;
        // Start is called before the first frame update
        void Start()
        {
            gameWorld = Singleton<GameWorld>.Instance;
            befound = false;
            ready = true;
            isweapon = false;
            timeFade = 0f;
            jumpFade = Physics.gravity.y;
            new NanoWaring().Enable();//启动AI警告    
        }

        // Update is called once per frame
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
                        //Console.WriteLine(NanoPlayer.MovementContext.MaxSpeed);
                        MovementState.G = new Vector3(0, -6, 0);
                        isweapon = true;
                    }
                    if (NowItemController.Template._parent == "543be6564bdc2df4348b4568")
                    {
                        isweapon = false;
                        Closeui();
                        AmmoText.GetComponent<Text>().text = "⊙ˍ⊙";
                        grenade.GetComponent<Image>().enabled = true;
                    }
                    if (NowItemController.Template.UnlootableFromSlot == "Scabbard")
                    {
                        isweapon = false;
                        Closeui();
                        AmmoText.GetComponent<Text>().text = "＾U＾";
                        knife.GetComponent<Image>().enabled = true;
                    }
                    if (NowItemController.Template._parent == "5448e8d04bdc2ddf718b4569" || NowItemController.Template._parent == "5448e8d64bdc2dce718b4568")
                    {
                        isweapon = false;
                        Closeui();
                        AmmoText.GetComponent<Text>().text = "￣ω￣";
                        food.GetComponent<Image>().enabled = true;
                    }
                    if (NowItemController.Template._parent == "5448f39d4bdc2d0a728b4568" || NowItemController.Template._parent == "5448f3a64bdc2d60728b456a" || NowItemController.Template._parent == "5448f3a14bdc2d27728b4569" || NowItemController.Template._parent == "5448f3ac4bdc2dce718b4569")
                    {
                        isweapon = false;
                        Closeui();
                        AmmoText.GetComponent<Text>().text = "╯▽╰";
                        med.GetComponent<Image>().enabled = true;

                    }
                }
                if (isweapon)
                {
                    if (NowWeapon.WeapClass == "smg")
                    {
                        Closeui();
                        smg.GetComponent<Image>().enabled = true;
                    }
                    if (NowWeapon.WeapClass == "machinegun")
                    {
                        Closeui();
                        machinegun.GetComponent<Image>().enabled = true;
                    }
                    if (NowWeapon.WeapClass == "shotgun")
                    {
                        Closeui();
                        shotgun.GetComponent<Image>().enabled = true;
                    }
                    if (NowWeapon.WeapClass == "assaultRifle" || NowWeapon.WeapClass == "assaultCarbine")
                    {
                        Closeui();
                        assaultRifle.GetComponent<Image>().enabled = true;
                    }
                    if (NowWeapon.WeapClass == "marksmanRifle")
                    {
                        Closeui();
                        marksmanRifle.GetComponent<Image>().enabled = true;
                    }
                    if (NowWeapon.WeapClass == "pistol")
                    {
                        Closeui();
                        pistol.GetComponent<Image>().enabled = true;
                    }
                    if (NowWeapon.WeapClass == "sniperRifle")
                    {
                        Closeui();
                        sniperRifle.GetComponent<Image>().enabled = true;
                    }
                    if (NowWeapon.WeapClass == "grenadeLauncher")
                    {
                        Closeui();
                        grenadeLauncher.GetComponent<Image>().enabled = true;
                    }
                }
                if (NowWeapon != null && NowWeapon.GetCurrentMagazine() != null && isweapon)
                {
                    //int maxMagazineCount = NowWeapon.GetMaxMagazineCount();
                    int count = NowWeapon.GetCurrentMagazineCount() + NowWeapon.ChamberAmmoCount;
                    AmmoText.GetComponent<Text>().text = count.ToString();
                }
                if (NowWeapon != null && NowWeapon.GetCurrentMagazine() == null && isweapon)
                {
                    int count = NowWeapon.ChamberAmmoCount;
                    AmmoText.GetComponent<Text>().text = count.ToString();
                }
                if (NowItemController == null)
                {
                    AmmoText.GetComponent<Text>().text = "C♂ME";
                    Closeui();
                    noweapon.GetComponent<Image>().enabled = true;
                }
            }
            else
            {
                AmmoText.GetComponent<Text>().text = "NULL";
                Closeui();
                noweapon.GetComponent<Image>().enabled = true;
                ammopack.SetActive(false);
            } 
            if (NormalMode != null)
            {
                basepack.GetComponent<RectTransform>().anchoredPosition = new Vector3(nanosuitcore.Xzhou.Value, nanosuitcore.Yzhou.Value, 0);
                if (nanosuit.isfly)
                {
                    FlyMode.GetComponent<RawImage>().enabled = true;
                }
                else
                {
                    FlyMode.GetComponent<RawImage>().enabled = false;
                }

                if (nanosuit.isarmor)
                {
                    Closemode();
                    AmorMode.GetComponent<RawImage>().enabled = true;
                }
                if (nanosuit.isstealth)
                {
                    Closemode();
                    CloakMode.GetComponent<RawImage>().enabled = true;
                }
                if (nanosuit.ispower)
                {
                    Closemode();
                    PowerMode.GetComponent<RawImage>().enabled = true;
                }
                if (!nanosuit.isstealth && !nanosuit.isarmor && !nanosuit.ispower)
                {
                    Closemode();
                    NormalMode.GetComponent<RawImage>().enabled = true;
                }
            }
            else Console.WriteLine("没识别");

            if (EnergyText != null)
            {
                EnergyText.GetComponent<Text>().text = nanosuit.nowenergy.ToString();
                NanoEnergy.GetComponent<Image>().fillAmount = nanosuit.maxenergy / 100;
                if(NanoEnergy.GetComponent<Image>().fillAmount<=0.2)
                {
                    LowEnergy.GetComponent<Text>().enabled = true;
                } 
                else
                {
                    LowEnergy.GetComponent<Text>().enabled = false;
                }
                var HeadHP = gameWorld.MainPlayer.ActiveHealthController.GetBodyPartHealth(EBodyPart.Head, true);
                var ChestHP = gameWorld.MainPlayer.ActiveHealthController.GetBodyPartHealth(EBodyPart.Chest, true);
                var maxhealthhead = HeadHP.Maximum;
                var maxhealthchest = ChestHP.Maximum;
                var nowhealthhead = HeadHP.Current * 100;
                var nowhealthchest = ChestHP.Current * 100;
                int realhealthhead = (int)(nowhealthhead / maxhealthhead);
                int realhealthchest = (int)(nowhealthchest / maxhealthchest);
                HealthText.GetComponent<Text>().text = realhealthhead.ToString();
                NanoHealth.GetComponent<Image>().fillAmount = (nowhealthhead / maxhealthhead) / 100;
                Health1Text.GetComponent<Text>().text = realhealthchest.ToString();
                NanoHealth1.GetComponent<Image>().fillAmount = (nowhealthchest / maxhealthchest) / 100;
            }
            else Console.WriteLine("没文字");
            if (NanoHealth.fillAmount <= 0.2)
            {
                NanoHealth.GetComponent<Image>().color = Color.red;
            }
            else
            {
                NanoHealth.GetComponent<Image>().color = Color.white;
            }
            if (NanoHealth1.fillAmount <= 0.2)
            {
                NanoHealth1.GetComponent<Image>().color = Color.red;
            }
            else
            {
                NanoHealth1.GetComponent<Image>().color = Color.white;
            }
            if (nanosuit.maxenergy <= 20)
            {
                NanoEnergy.GetComponent<Image>().color = Color.red;
            }
            else
            {
                NanoEnergy.GetComponent<Image>().color = EnergyColorbase;
            }
            if (befound && ready && EnemyFound!=null)
            {
                EnemyFound.GetComponent<RawImage>().enabled = true;
                ready = false;
                befound = false;
                if (!nanosuit.isstealth && !nanosuit.isarmor && !nanosuit.ispower)
                {
                    nanosuit.isautoarmor = true;
                }
                //Console.WriteLine("被锁定");
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
            if (Input.GetKeyDown(KeyCode.Space) && nanosuit.ispower)
            {
                if(Physics.gravity.y == jumpFade)
                {
                    StartCoroutine("PowerJump");
                    PowerMode.GetComponent<AudioSource>().volume = nanosuitcore.nanovolume.Value;
                    PowerMode.GetComponent<AudioSource>().Play();
                    nanosuit.maxenergy -= 15;
                }   
            }
        }
        private void Closeui()
        {
            smg.GetComponent<Image>().enabled = false;
            machinegun.GetComponent<Image>().enabled = false;
            shotgun.GetComponent<Image>().enabled = false;
            assaultRifle.GetComponent<Image>().enabled = false;
            marksmanRifle.GetComponent<Image>().enabled = false;
            pistol.GetComponent<Image>().enabled = false;
            sniperRifle.GetComponent<Image>().enabled = false;
            grenadeLauncher.GetComponent<Image>().enabled = false;
            assaultRifle.GetComponent<Image>().enabled = false;
            noweapon.GetComponent<Image>().enabled = false;
            grenade.GetComponent<Image>().enabled = false;
            knife.GetComponent<Image>().enabled = false;
            food.GetComponent<Image>().enabled = false;
            med.GetComponent<Image>().enabled = false;
        }
        private void Closemode()
        {
            NormalMode.GetComponent<RawImage>().enabled = false;
            AmorMode.GetComponent<RawImage>().enabled = false;
            CloakMode.GetComponent<RawImage>().enabled = false;
            SpeedMode.GetComponent<RawImage>().enabled = false;
            PowerMode.GetComponent<RawImage>().enabled = false;
        }
        IEnumerator PowerJump()
        {
            float Nowgravity;
            Nowgravity = 0f;
            while (Nowgravity > jumpFade)
            {
                Nowgravity -= 0.09f;
                if(Nowgravity <= jumpFade)
                {
                    Nowgravity = jumpFade;
                }
                Physics.gravity = new Vector3(0, Nowgravity, 0);
                yield return 0;
            }
        }
    }   
    public class NanoWaring : ModulePatch //来自山姆特警的第六感
    {
        private static float CoolTime;
        protected override MethodBase GetTargetMethod()
        {
            var t = typeof(BotMemoryClass).GetProperty("GoalEnemy").PropertyType;
            return t.GetMethod("SetVisible");
        }
        [PatchPostfix]
        private static void PatchPostfix(object __instance, bool value, bool ___bool_0)
        {
            if (!NanoGUI.ready || Time.time < CoolTime) return;
            var person = (IAIDetails)__instance.GetType().GetProperty("Person").GetValue(__instance);
            if (!value || !person.GetPlayer.IsYourPlayer || !___bool_0) return;
            //if (person.GetPlayer.IsYourPlayer) 
            NanoGUI.befound = true;           
            CoolTime = Time.time + 3f;
        }
    }
    //[HarmonyPatch(typeof(Player), "MaxSpeed", MethodType.Setter)]
    //class NanoSpeedPatch
    //{
    //    public static bool Prefix(ref float MaxSpeed)
    //    {
    //        MaxSpeed = 100f;
    //        return false; //拦截原方法，直接使用我们给出的结果
    //    }
    //}
}
