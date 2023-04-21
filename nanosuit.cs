using System;
using System.IO;
using System.Reflection;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EFT;
using HarmonyLib;
using UnityEngine;
using Aki.Reflection.Patching;
using Object = UnityEngine.Object;
using Comfort.Common;
using System.Collections;

namespace nanosuit
{
    [BepInPlugin("AT.nanosuit", "AT.纳米生化装Nanosuit", "1.0.9")]
    public class nanosuitcore : BaseUnityPlugin
    {
        // 窗口开关
        private static ManualLogSource logger;
        public static ConfigEntry<int> armorcost;
        public static ConfigEntry<int> armordefense;
        public static ConfigEntry<int> stealthcost;
        public static ConfigEntry<int> flycost;
        public static ConfigEntry<int> nanocharg;
        public static ConfigEntry<int> nanochargdelay;
        public static ConfigEntry<int> nanovisioncost;
        public static ConfigEntry<int> FixDelay;
        public static ConfigEntry<bool> flycore;
        public static ConfigEntry<bool> NanoSystemOnline;
        public static ConfigEntry<bool> curemodeOnline;
        public static ConfigEntry<string> nanoup;
        public static ConfigEntry<string> language;
        public static ConfigEntry<KeyCode> ARMOR;
        public static ConfigEntry<KeyCode> STEALTH;
        public static ConfigEntry<KeyCode> NANOVISION;
        public static ConfigEntry<float> nanovolume;
        public static ConfigEntry<float> Xzhou;
        public static ConfigEntry<float> Yzhou;
        public static ConfigEntry<float> ArmorhudScale;
        public static ConfigEntry<float> ArmorhudAlpha;       
        public static ConfigEntry<float> FixIntensity;
        public static string[] nanouplist = { "生存强化(Enhanced Survival)", "装甲强化(Enhanced Armor)", "隐身强化(Enhanced Stealth)", "充能强化(Enhanced Charge)", "推进器强化(Enhanced Propeller)", "纳米视野强化(Enhanced Nanovision)" };
        public static string[] languagelist = { "中文", "English"};
        public void Awake()
        {
            var harmony = new Harmony("nanosuit");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            logger = Logger;
            Logger.LogInfo($"nanosuit: Loading");
            language = Config.Bind<string>("语言设置(Language Setting)", "能量条HUD语言(Language of energy HUD)", nanouplist[0],
                new ConfigDescription("选择能量条HUD显示文字语言，重启游戏后生效(Select language of energy HUD.It takes effect after you restart the game)", new AcceptableValueList<string>(languagelist)));
            armorcost = Config.Bind<int>("装甲模式配置(Armor Settings)", "装甲消耗(Armor Cost)", 2, "装甲模式下能量自然消耗每秒的值(Natural energy cost per second in Armor mode)");
            armordefense = Config.Bind<int>("装甲模式配置(Armor Settings)", "装甲防御力(Armor Defense)", 45, "装甲模式下受击，子弹穿透力超过这个数值则会对能量造成5点消耗，反之造成1点能量消耗(When hit in Armor mode, bullet penetration above this value costs 5 energy and vice versa costs 1 energy)");
            ArmorhudScale = Config.Bind("装甲模式配置(Armor Settings)", "HUD缩放(HUD Scale)", 1f, new ConfigDescription("装甲模式下的HUD缩放(The HUD Scale in Armor mode)", new AcceptableValueRange<float>(0.8f, 3f)));
            ArmorhudAlpha = Config.Bind("装甲模式配置(Armor Settings)", "HUD透明度(HUD Alpha)", 0.3f, new ConfigDescription("装甲模式下的HUD透明度(The HUD Alpha in Armor mode)", new AcceptableValueRange<float>(0f, 0.5f)));
            stealthcost = Config.Bind<int>("隐身模式配置(Stealth Settings)", "隐身消耗(Stealth Cost)", 5, "隐身模式下能量自然消耗每秒的值(Natural energy cost per second in Stealth mode)");
            nanocharg = Config.Bind<int>("充能配置(Charge Settings)", "充能速率(Charging Speed)", 25, "纳米服每秒充能速率(Nanosuit charge rate per second)");
            nanochargdelay = Config.Bind<int>("充能配置(Charge Settings)", "充能延迟(Charging Delay)", 2, "纳米服关闭功能后延迟几秒后开始充能(Turn off the function after a delay of a few seconds to start charging)");
            Xzhou = Config.Bind("充能配置(Charge Settings)", "能量条X轴位置(Energy HUD X-axis position)", 0f, new ConfigDescription("能量条在界面的左右位置(Energy HUD on the left and right positions of the interface)", new AcceptableValueRange<float>(-0.1f, 0.35f)));
            Yzhou = Config.Bind("充能配置(Charge Settings)", "能量条Y轴位置(Energy HUD Y-axis position)", 0f, new ConfigDescription("能量条在界面的上下位置(Energy HUD on the upper and lower positions of the interface)", new AcceptableValueRange<float>(-0.08f, 0.25f)));
            nanoup = Config.Bind<string>("强化方案选择(Enhanced Choose)", "选择后将无法更改其他参数(Other parameters cannot be changed after selection)", nanouplist[0], 
                new ConfigDescription("下列强化方案中选择一个(Choose one of the following reinforcement programs)\n默认生存强化：穿戴者受伤后给予60秒内止疼和每三秒全部位回复15生命值(Default Enhanced Survival:Gives the wearer pain relief for 60 seconds after injury and 15 health for all positions every 3 seconds)", new AcceptableValueList<string>(nanouplist)));
            ARMOR = Config.Bind<KeyCode>("按键设置(KeyCode Settings)", "装甲按键(Armor KeyCode)", KeyCode.Q, "装甲模式启动按钮(Armor mode start button)");
            STEALTH = Config.Bind<KeyCode>("按键设置(KeyCode Settings)", "隐身按键(Stealth KeyCode)", KeyCode.E, "隐身模式启动按钮(Stealth mode start button)");
            NANOVISION = Config.Bind<KeyCode>("按键设置(KeyCode Settings)", "纳米视野按键(Nanovision KeyCode)", KeyCode.H, "纳米视野启动按钮(Nanovision  start button)");
            flycore = Config.Bind<bool>("垂直推进器设置(Propeller Settings)", "是否启动垂直喷射器(Whether to start the Propeller)", false, "启动后跳跃时长按空格可消耗能量垂直起飞(After starting the jump, press space for a long time to consume energy for vertical takeoff)");
            flycost = Config.Bind<int>("垂直推进器设置(Propeller Settings)", "推进器消耗(Propeller Cost)", 15, "飞行时每秒能量消耗值(Natural energy cost per second in Propeller mode)");
            nanovisioncost = Config.Bind<int>("纳米视野设置(Nanovision Settings)", "纳米视野消耗(Nanovision Cost)", 8, "纳米视野开启后每秒能量消耗值(Natural energy cost per second in Nanovision mode)");
            nanovolume = Config.Bind("纳米系统设置(Nanosystem Settings)", "设置音量(Volume Settings)", 1f, new ConfigDescription("纳米服各个模式启动时的音效音量(The sound volume of each mode on the Nano Suit)", new AcceptableValueRange<float>(0f, 1f)));
            NanoSystemOnline = Config.Bind<bool>("纳米系统设置(Nanosystem Settings)", "纳米系统是否启动(NanoSystem online or not)", true, "纳米机器，小子！启动纳米系统(Nanomachine,son!Start-up NanoSystem)");
            curemodeOnline = Config.Bind<bool>("自动修复系统设置(Automatic Treatment System Settings)", "自动修复系统是否启动(Automatic treatment system online or not)", true, "你为什么还不死？选择生存强化后受伤10秒后修复全部黑色部位，去除所有负面效果，缓慢回复所有生命值(Why don't you die?After choosing Enhanced Survival,remove all negative states, repair black areas, and restore all health after 10 seconds of damage)");
            FixIntensity = Config.Bind("自动修复系统设置(Automatic Treatment System Settings)", "修复速度(Treatment Rate)", 0.1f, new ConfigDescription("触发自动修复后的修复速率(Rate of Treatment after Automatic Treatment is triggered)", new AcceptableValueRange<float>(0f, 1f)));
            FixDelay = Config.Bind<int>("自动修复系统设置(Automatic Treatment System Settings)", "修复延迟(Treatment Delay)", 10, "收到伤害后延迟几秒后开始自动修复(After receiving damage delay of a few seconds, Automatic Treatment begins)");
        }
        void Update()
        {

        }
        [HarmonyPatch(typeof(GClass2623), "HitCollider", MethodType.Getter)]
        public class GClass2623_HitCollider_PatchBulletClass_Damage_Patch
        {

            public static void Postfix(GClass2623 __instance, ref Collider __result)//350是2611,351-353是2620, 355是2623
            {
                var ammo = __instance.Ammo as BulletClass;//获取击中子弹的数据
                var hitman = __instance.Player;//获取是谁射的子弹
                if (__result != null)
                {
                    var armorhitScript = __result.GetComponentInParent<armorhit>();
                    if (armorhitScript != null)
                    {
                        armorhitScript.youarehit(true);
                        if (ammo.ammoType != "Buckshot")
                        {
                            armorhitScript.armorhit_method(ammo.Damage, ammo.PenetrationPower, hitman.IsYourPlayer);
                        }
                    }
                }
            }
        }
    }
    public class nanosuit : MonoBehaviour
    {
        public AudioClip[] audios;
        public KeyCode ARMOR;
        public KeyCode STEALTH;
        public KeyCode NANOVISION;
        public GameObject Hand;
        private static GameWorld gameWorld;
        private Animation armorhudani;
        public Material stealthmaterial;
        public Material armormaterial;
        public Material armormaterial02;
        public Material basematerial;
        public Material basematerial02;
        Material[] newmaterial;
        [ColorUsageAttribute(true, true)]
        public Color armorupcolor;
        [ColorUsageAttribute(true, true)]
        public Color chargupcolor;
        [ColorUsageAttribute(true, true)]
        public Color chargupbasecolor;
        private Color cloakColor = Color.white;
        private Color ArmorColor = Color.white;
        public static float maxenergy = 100;
        private int armorvoice;
        private int stealthvoice;
        private int nanomode;
        private int armorbasecost=2;
        private int stealthbasecost=10;
        private int nanochargbase = 25;
        private int nanodelaychargbase = 2;
        private int flybasecost = 15;
        private int nanovisionbasecost = 8;      
        public static int armorpowerbase = 45;
        private GameObject nanoarmor;
        private GameObject nanoarmorhud;
        private GameObject nanoenergyhud;
        private GameObject nanovisionhud;
        private GameObject nanoarmarmor;
        private GameObject flymode;
        private GameObject marktarget;
        private GameObject[] AItarget;
        private float energytimer = 0;
        private float timer = 0;
        private float MAXHP = 0;
        private float NOWHP = 0;
        public static float fixdelaytime = 0;      
        bool isshengcun = false;
        bool ischarging = false;
        bool isenergyempty = false;
        bool isenergy = false;
        public static bool isarmor = false;
        public static bool isstealth = false;
        public static bool isnanovision = false;
        public static bool isarmorhit = false;
        public static bool isarmorhitdanger = false;
        public static bool startfixdebuff = false;
        public static bool iscureyourself = false;
        bool isplayaudio = false;
        bool isclick = false;
        bool isfly = false;    
        private bool[] AIhealth;
        public enum Clickcount
        {
            firsttime,
            secondtime,
            zerotime
        }
        Clickcount First = Clickcount.zerotime;
        public static Object armorPrefab { get; private set; }
        public static Object stealthPrefab { get; private set; }
        public static Object armorhudPrefab { get; private set; }
        public static Object energyhudPrefab { get; private set; }
        public static Object enenergyhudPrefab { get; private set; }
        public static Object nanovisionhudPrefab { get; private set; }
        public static Object armarmorPrefab { get; private set; }
        public static Object flymodePrefab { get; private set; }
        public static Object nanotargetPrefab { get; private set; }
        // Start is called before the first frame update
        private bool Entermap() => Singleton<GameWorld>.Instantiated && gameWorld.AllPlayers != null && gameWorld.AllPlayers.Count > 0 && gameWorld.AllPlayers[0] is Player ? true : false;
        void Start()
        {                
            armorvoice = 0;
            stealthvoice = 2;
            energytimer = 0;
            isclick = false;
            isfly = false;
            AItarget = new GameObject[999];
            AIhealth = new bool[999];
            newmaterial = new Material[Hand.GetComponent<SkinnedMeshRenderer>().materials.Length];//替换材质只能这样搞
            for (int i = 0; i < newmaterial.Length; i++)//获取物体全部材质
            {
                newmaterial[i] = Hand.GetComponent<SkinnedMeshRenderer>().materials[i];
            }
            newmaterial[0] = basematerial;//设置默认材质
            newmaterial[1] = basematerial02;//设置默认材质
            if (armorPrefab == null)//加载装甲和隐身模块音效
            {
                String filename = Path.Combine(Environment.CurrentDirectory, "BepInEx/plugins/atmod/nanosuitcore");
                if (!File.Exists(filename))
                    return;

                var nanosuitBundle = AssetBundle.LoadFromFile(filename);
                if (nanosuitBundle == null)
                    return;
                armorPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/Armorsound.prefab");
                stealthPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/Stealthsound.prefab");
                armorhudPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/HUD/ArmorHUD.prefab");
                nanovisionhudPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/HUD/NanovisionHUD.prefab");
                energyhudPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/HUD/EnergyHUD.prefab");
                enenergyhudPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/HUD/EnergyHUD(en).prefab");
                armarmorPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/Armarmor.prefab");
                flymodePrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/flymode.prefab");
                nanotargetPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/HUD/Nanotarget.prefab");
                Console.WriteLine("纳米服功能模块加载完毕");
            }
        }

        // Update is called once per frame
        void Update()
        {           
            if (nanosuitcore.NanoSystemOnline.Value)
            {
                this.GetComponent<AudioSource>().volume = nanosuitcore.nanovolume.Value;//纳米系统音量调整
                gameWorld = Singleton<GameWorld>.Instance;
                if (nanosuitcore.nanoup.Value == "生存强化(Enhanced Survival)")
                {
                    if (startfixdebuff && Entermap()&& nanosuitcore.curemodeOnline.Value)
                    {
                        fixdelaytime += Time.deltaTime;
                        var HeadHP = gameWorld.AllPlayers[0].ActiveHealthController.GetBodyPartHealth(EBodyPart.Head, true);
                        var ChestHP = gameWorld.AllPlayers[0].ActiveHealthController.GetBodyPartHealth(EBodyPart.Chest, true);
                        var LeftArmHP = gameWorld.AllPlayers[0].ActiveHealthController.GetBodyPartHealth(EBodyPart.LeftArm, true);
                        var LeftLegHP = gameWorld.AllPlayers[0].ActiveHealthController.GetBodyPartHealth(EBodyPart.LeftLeg, true);
                        var RightArmHP = gameWorld.AllPlayers[0].ActiveHealthController.GetBodyPartHealth(EBodyPart.RightArm, true);
                        var RightLegHP = gameWorld.AllPlayers[0].ActiveHealthController.GetBodyPartHealth(EBodyPart.RightLeg, true);
                        var StomachHP = gameWorld.AllPlayers[0].ActiveHealthController.GetBodyPartHealth(EBodyPart.Stomach, true);
                        if (fixdelaytime >= nanosuitcore.FixDelay.Value && !iscureyourself)
                        {
                            gameWorld.AllPlayers[0].ActiveHealthController.RemoveNegativeEffects(EBodyPart.Common);
                            gameWorld.AllPlayers[0].ActiveHealthController.RestoreBodyPart(EBodyPart.LeftArm, 1f);
                            gameWorld.AllPlayers[0].ActiveHealthController.RestoreBodyPart(EBodyPart.LeftLeg, 1f);
                            gameWorld.AllPlayers[0].ActiveHealthController.RestoreBodyPart(EBodyPart.RightArm, 1f);
                            gameWorld.AllPlayers[0].ActiveHealthController.RestoreBodyPart(EBodyPart.RightLeg, 1f);
                            gameWorld.AllPlayers[0].ActiveHealthController.RestoreBodyPart(EBodyPart.Stomach, 1f);
                            //gameWorld.AllPlayers[0].ActiveHealthController.RestoreFullHealth();                       
                            iscureyourself = true;
                            fixdelaytime = 0;
                            Console.WriteLine("启动修复");
                        }
                        if (iscureyourself)
                        {
                            var nowHPHead = HeadHP.Current;
                            var nowHPChest = ChestHP.Current;
                            var nowHPLeftArm = LeftArmHP.Current;
                            var nowHPLeftLeg = LeftLegHP.Current;
                            var nowHPRightArm = RightArmHP.Current;
                            var nowHPRightLeg = RightLegHP.Current;
                            var nowHPStomach = StomachHP.Current;
                            if (nowHPHead < HeadHP.Maximum)
                            {
                                gameWorld.AllPlayers[0].ActiveHealthController.ChangeHealth(EBodyPart.Head, nanosuitcore.FixIntensity.Value, default);
                                nowHPHead += nanosuitcore.FixIntensity.Value;
                            }
                            if (nowHPChest < ChestHP.Maximum)
                            {
                                gameWorld.AllPlayers[0].ActiveHealthController.ChangeHealth(EBodyPart.Chest, nanosuitcore.FixIntensity.Value, default);
                                nowHPChest += nanosuitcore.FixIntensity.Value;
                            }
                            if (nowHPLeftArm < LeftArmHP.Maximum)
                            {
                                gameWorld.AllPlayers[0].ActiveHealthController.ChangeHealth(EBodyPart.LeftArm, nanosuitcore.FixIntensity.Value, default);
                                nowHPLeftArm += nanosuitcore.FixIntensity.Value;
                            }
                            if (nowHPLeftLeg < LeftLegHP.Maximum)
                            {
                                gameWorld.AllPlayers[0].ActiveHealthController.ChangeHealth(EBodyPart.LeftLeg, nanosuitcore.FixIntensity.Value, default);
                                nowHPLeftLeg += nanosuitcore.FixIntensity.Value;
                            }
                            if (nowHPRightArm < RightArmHP.Maximum)
                            {
                                gameWorld.AllPlayers[0].ActiveHealthController.ChangeHealth(EBodyPart.RightArm, nanosuitcore.FixIntensity.Value, default);
                                nowHPRightArm += nanosuitcore.FixIntensity.Value;
                            }
                            if (nowHPRightLeg < RightLegHP.Maximum)
                            {
                                gameWorld.AllPlayers[0].ActiveHealthController.ChangeHealth(EBodyPart.RightLeg, nanosuitcore.FixIntensity.Value, default);
                                nowHPRightLeg += nanosuitcore.FixIntensity.Value;
                            }
                            if (nowHPStomach < StomachHP.Maximum)
                            {
                                gameWorld.AllPlayers[0].ActiveHealthController.ChangeHealth(EBodyPart.Stomach, nanosuitcore.FixIntensity.Value, default);
                                nowHPStomach += nanosuitcore.FixIntensity.Value;
                            }
                            NOWHP = nowHPStomach + nowHPRightLeg + nowHPRightArm + nowHPLeftLeg + nowHPLeftArm + nowHPChest + nowHPHead;
                            MAXHP = HeadHP.Maximum + ChestHP.Maximum + LeftArmHP.Maximum + LeftLegHP.Maximum + RightArmHP.Maximum + RightLegHP.Maximum + StomachHP.Maximum;
                            if (MAXHP <= NOWHP)
                            {
                                iscureyourself = false;
                                startfixdebuff = false;
                            }
                        }
                    }
                    if (!isshengcun)
                    {
                        new NanosuitPatch().Enable();//启动受击buff
                        new NanoSystemPatch().Enable();
                        isshengcun = true;
                    }
                }
                if (nanosuitcore.nanoup.Value != "生存强化(Enhanced Survival)" && isshengcun == true)
                {
                    new NanosuitPatch().Disable();//关闭受击buff     
                    isshengcun = false;
                }
                if (nanosuitcore.nanoup.Value != "装甲强化(Enhanced Armor)")
                {
                    armorbasecost = nanosuitcore.armorcost.Value;
                    armorpowerbase = nanosuitcore.armordefense.Value;
                }
                else
                {
                    nanosuitcore.armorcost.Value = 1;
                    nanosuitcore.armordefense.Value = 90;
                    nanosuitcore.stealthcost.Value = 5;
                    nanosuitcore.nanocharg.Value = 25;
                    nanosuitcore.nanochargdelay.Value = 2;
                    nanosuitcore.flycost.Value = 15;
                    nanosuitcore.nanovisioncost.Value = 8;
                    nanosuitcore.FixIntensity.Value = 0.1f;
                    nanosuitcore.FixDelay.Value = 10;
                    armorbasecost = 1;
                    armorpowerbase = 90;
                    isshengcun = false;
                    //if (nanoarmorhud != null)
                    //{
                    //MeshRenderer[] armorupMeshRenderer = nanoarmorhud.GetComponentsInChildren<MeshRenderer>();
                    //foreach (MeshRenderer child in armorupMeshRenderer)
                    //{
                    //armorupMeshRenderer[0].materials[0].SetColor("_Edgecolor", armorupcolor);
                    //}
                    //}
                }
                if (nanosuitcore.nanoup.Value != "隐身强化(Enhanced Stealth)")
                {
                    stealthbasecost = nanosuitcore.stealthcost.Value;
                }
                else
                {
                    nanosuitcore.stealthcost.Value = 3;
                    nanosuitcore.armorcost.Value = 2;
                    nanosuitcore.armordefense.Value = 45;
                    nanosuitcore.nanocharg.Value = 25;
                    nanosuitcore.nanochargdelay.Value = 2;
                    nanosuitcore.flycost.Value = 15;
                    nanosuitcore.nanovisioncost.Value = 8;
                    nanosuitcore.FixIntensity.Value = 0.1f;
                    nanosuitcore.FixDelay.Value = 10;
                    stealthbasecost = 3;
                    isshengcun = false;
                }
                if (nanosuitcore.nanoup.Value != "充能强化(Enhanced Charge)")
                {
                    nanochargbase = nanosuitcore.nanocharg.Value;
                    nanodelaychargbase = nanosuitcore.nanochargdelay.Value;
                }
                else
                {
                    nanosuitcore.nanocharg.Value = 50;
                    nanosuitcore.nanochargdelay.Value = 1;
                    nanosuitcore.armorcost.Value = 2;
                    nanosuitcore.armordefense.Value = 45;
                    nanosuitcore.stealthcost.Value = 5;
                    nanosuitcore.flycost.Value = 15;
                    nanosuitcore.nanovisioncost.Value = 8;
                    nanosuitcore.FixIntensity.Value = 0.1f;
                    nanosuitcore.FixDelay.Value = 10;
                    nanodelaychargbase = 1;
                    nanochargbase = 50;
                    isshengcun = false;
                }
                if (nanosuitcore.nanoup.Value != "推进器强化(Enhanced Propeller)")
                {
                    flybasecost = nanosuitcore.flycost.Value;
                }
                else
                {
                    nanosuitcore.nanocharg.Value = 25;
                    nanosuitcore.nanochargdelay.Value = 2;
                    nanosuitcore.armorcost.Value = 2;
                    nanosuitcore.armordefense.Value = 45;
                    nanosuitcore.stealthcost.Value = 5;
                    nanosuitcore.flycost.Value = 5;
                    nanosuitcore.nanovisioncost.Value = 8;
                    nanosuitcore.FixIntensity.Value = 0.1f;
                    nanosuitcore.FixDelay.Value = 10;
                    flybasecost = 5;
                    isshengcun = false;
                }
                if (nanosuitcore.nanoup.Value != "纳米视野强化(Enhanced Nanovision)")
                {
                    nanovisionbasecost = nanosuitcore.nanovisioncost.Value;
                }
                else
                {
                    nanosuitcore.nanocharg.Value = 25;
                    nanosuitcore.nanochargdelay.Value = 2;
                    nanosuitcore.armorcost.Value = 2;
                    nanosuitcore.armordefense.Value = 45;
                    nanosuitcore.stealthcost.Value = 5;
                    nanosuitcore.flycost.Value = 15;
                    nanosuitcore.nanovisioncost.Value = 2;
                    nanosuitcore.FixIntensity.Value = 0.1f;
                    nanosuitcore.FixDelay.Value = 10;
                    nanovisionbasecost = 2;
                    isshengcun = false;
                }
                if (marktarget == null)
                {
                    marktarget = GameObject.Find("FPS Camera");//找到第一人称视角摄像机
                }
                if (nanoenergyhud == null && nanosuitcore.language.Value == "中文")//生成能量条HUD
                {
                    var nanoenergyhudbase = Instantiate(energyhudPrefab, marktarget.transform.position, marktarget.transform.rotation);
                    nanoenergyhud = nanoenergyhudbase as GameObject;
                    nanoenergyhud.transform.parent = marktarget.transform;
                }
                if (nanoenergyhud == null && nanosuitcore.language.Value == "English")//生成能量条HUD
                {
                    var nanoenergyhudbase = Instantiate(enenergyhudPrefab, marktarget.transform.position, marktarget.transform.rotation);
                    nanoenergyhud = nanoenergyhudbase as GameObject;
                    nanoenergyhud.transform.parent = marktarget.transform;
                }
                if (nanoenergyhud != null)//能量条HUD变化，模式显示，低能量警告
                {
                    nanoenergyhud.transform.localPosition = new Vector3(nanosuitcore.Xzhou.Value, nanosuitcore.Yzhou.Value, 0);
                    Transform[] allChildrenTransform = nanoenergyhud.GetComponentsInChildren<Transform>();
                    foreach (Transform child in allChildrenTransform)
                    {
                        allChildrenTransform[1].localScale = new Vector3(maxenergy, 1, 1);
                    }
                    MeshRenderer[] allChildrenMeshRenderer = nanoenergyhud.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer child in allChildrenMeshRenderer)
                    {
                        if (nanosuitcore.nanoup.Value == "充能强化(Enhanced Charge)")
                        {
                            allChildrenMeshRenderer[0].materials[0].SetColor("_Edgecolor", chargupcolor);
                        }
                        else
                        {
                            allChildrenMeshRenderer[0].materials[0].SetColor("_Edgecolor", chargupbasecolor);
                        }
                        if (maxenergy <= 20)
                        {
                            allChildrenMeshRenderer[5].enabled = true;
                        }
                        else
                        {
                            allChildrenMeshRenderer[5].enabled = false;
                        }
                        if (isfly)
                        {
                            allChildrenMeshRenderer[6].enabled = true;
                        }
                        else
                        {
                            allChildrenMeshRenderer[6].enabled = false;
                        }
                        if (isnanovision)
                        {
                            allChildrenMeshRenderer[7].enabled = true;
                        }
                        else
                        {
                            allChildrenMeshRenderer[7].enabled = false;
                        }
                        if (isarmor)
                        {
                            allChildrenMeshRenderer[2].enabled = false;
                            allChildrenMeshRenderer[3].enabled = true;
                            allChildrenMeshRenderer[4].enabled = false;
                        }
                        if (isstealth)
                        {
                            allChildrenMeshRenderer[2].enabled = false;
                            allChildrenMeshRenderer[3].enabled = false;
                            allChildrenMeshRenderer[4].enabled = true;
                        }
                        if (isstealth != true && isarmor != true)
                        {
                            allChildrenMeshRenderer[2].enabled = true;
                            allChildrenMeshRenderer[3].enabled = false;
                            allChildrenMeshRenderer[4].enabled = false;
                        }
                    }
                }
                if (ischarging)//开始充能
                {
                    if (isenergy == false)
                    {
                        energytimer += Time.deltaTime;
                        if (energytimer >= nanodelaychargbase)
                        {
                            energytimer = 0;
                            isenergy = true;
                        }
                    }
                    if (isenergy)
                    {
                        maxenergy += Time.deltaTime * nanochargbase;
                        isplayaudio = false;
                        if (maxenergy >= 100)
                        {
                            maxenergy = 100;
                            ischarging = false;
                            isenergy = false;
                        }
                    }
                }
                if (Input.GetKeyDown(nanosuitcore.ARMOR.Value) && maxenergy > 0)//装甲模式启动音效和关闭音效
                {
                    this.GetComponent<AudioSource>().clip = audios[armorvoice];
                    this.GetComponent<AudioSource>().Play();
                    newmaterial[0] = armormaterial;
                    newmaterial[1] = armormaterial02;
                    Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
                    armorvoice++;
                    stealthvoice = 2;
                    if (armorvoice >= 2)//装甲模式是否关闭
                    {
                        armorvoice = 0;
                        if (nanoarmarmor != null)
                        {
                            Destroy(nanoarmarmor);
                        }
                        if (nanoarmor != null)
                        {
                            Destroy(nanoarmor);
                        }
                        if (armorhudani != null)
                        {
                            armorhudani[armorhudani.clip.name].time = armorhudani[armorhudani.clip.name].length;
                            armorhudani[armorhudani.clip.name].speed = -1;
                            armorhudani.Play(armorhudani.clip.name);
                        }
                        newmaterial[0] = basematerial;
                        newmaterial[1] = basematerial02;
                        Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
                        if (gameWorld.AllPlayers[0].ActiveHealthController.DamageCoeff != 1f && Entermap())
                        {
                            gameWorld.AllPlayers[0].ActiveHealthController.SetDamageCoeff(1f);
                        }
                        isstealth = false;
                        isarmor = false;
                        ischarging = true;
                        isenergy = false;
                        energytimer = 0;
                    }
                    nanomode = armorvoice;
                }
                if (Input.GetKeyDown(nanosuitcore.STEALTH.Value) && maxenergy > 0)//隐身模式启动音效和关闭音效
                {
                    this.GetComponent<AudioSource>().clip = audios[stealthvoice];
                    this.GetComponent<AudioSource>().Play();
                    newmaterial[0] = stealthmaterial;
                    newmaterial[1] = stealthmaterial;
                    Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
                    StartCoroutine("CloakCharacter");
                    stealthvoice++;
                    armorvoice = 0;
                    if (stealthvoice >= 4)//隐身模式是否关闭
                    {
                        stealthvoice = 2;
                        if (nanoarmarmor != null)
                        {
                            Destroy(nanoarmarmor);
                        }
                        if (nanoarmor != null)
                        {
                            Destroy(nanoarmor);
                        }
                        StartCoroutine("DeCloakCharacter");
                        ischarging = true;
                        isstealth = false;
                        isarmor = false;
                        isenergy = false;
                        energytimer = 0;
                    }
                    nanomode = stealthvoice;
                }
                if (Input.GetKeyDown(nanosuitcore.NANOVISION.Value) && maxenergy > 0)//纳米视野是否启动
                {
                    isnanovision = !isnanovision;
                    if (isnanovision)//判断现在是否纳米视野
                    {
                        this.GetComponent<AudioSource>().clip = audios[4];
                        this.GetComponent<AudioSource>().Play();
                        if (nanovisionhud == null)
                        {
                            var nanovisionhudbase = Instantiate(nanovisionhudPrefab, marktarget.transform.position, marktarget.transform.rotation);
                            nanovisionhud = nanovisionhudbase as GameObject;
                            nanovisionhud.transform.parent = marktarget.transform;
                        }
                        if (Entermap() && gameWorld.AllPlayers.Count >= 2)
                        {
                            for (int i = 1; i < gameWorld.AllPlayers.Count; i++)//获取全部AI
                            {
                                Console.WriteLine("读取坐标" + gameWorld.AllPlayers[i].Transform.position);
                                Console.WriteLine(i);
                                AItarget[i] = Instantiate(nanotargetPrefab, gameWorld.AllPlayers[i].Transform.position, gameWorld.AllPlayers[i].Transform.rotation) as GameObject;
                            }
                        }
                    }
                    else
                    {
                        if (nanovisionhud != null)
                        {
                            Destroy(nanovisionhud);
                        }
                        if (AItarget != null)
                        {
                            for (int i = 1; i < 100; i++)//获取全部AI
                            {
                                Destroy(AItarget[i]);
                            }
                        }
                        if (!isstealth || !isarmor || !isfly)
                        {
                            ischarging = true;
                            energytimer = 0;
                        }
                    }
                }
                if (nanomode == 1 && !isenergyempty)//判断现在是否启动装甲模式
                {
                    if (nanoarmor != null)
                    {
                        Destroy(nanoarmor);
                    }
                    var nanoarmorbase = Instantiate(armorPrefab, this.transform.position, this.transform.rotation);
                    nanoarmor = nanoarmorbase as GameObject;
                    nanoarmor.transform.parent = this.transform;
                    var nanoarmarmorbase = Instantiate(armarmorPrefab, marktarget.transform.position, marktarget.transform.rotation);
                    nanoarmarmor = nanoarmarmorbase as GameObject;
                    nanoarmarmor.transform.parent = marktarget.transform;
                    if (nanoarmorhud == null)
                    {
                        var nanoarmorhudbase = Instantiate(armorhudPrefab, marktarget.transform.position, marktarget.transform.rotation);
                        nanoarmorhud = nanoarmorhudbase as GameObject;
                        armorhudani = nanoarmorhud.GetComponent<Animation>();
                        nanoarmorhud.transform.parent = marktarget.transform;
                    }
                    if (armorhudani != null)
                    {
                        armorhudani[armorhudani.clip.name].time = 0;
                        armorhudani[armorhudani.clip.name].speed = 1;
                        armorhudani.Play(armorhudani.clip.name);
                        nanoarmorhud.transform.localScale = new Vector3(nanosuitcore.ArmorhudScale.Value, nanosuitcore.ArmorhudScale.Value, 1);
                        MeshRenderer[] armorhudMeshRenderer = nanoarmorhud.GetComponentsInChildren<MeshRenderer>();
                        foreach (MeshRenderer child in armorhudMeshRenderer)
                        {
                            armorhudMeshRenderer[0].materials[0].SetFloat("_Innerfresnelintensity", nanosuitcore.ArmorhudAlpha.Value);
                        }
                    }
                    nanomode = 0;
                    isstealth = false;
                    isarmor = true;
                }
                if (nanomode == 3 && !isenergyempty)//判断现在是否启动隐身模式
                {
                    if (nanoarmor != null)
                    {
                        Destroy(nanoarmor);
                    }
                    if (nanoarmorhud != null)
                    {
                        Destroy(nanoarmorhud);
                    }
                    if (nanoarmarmor != null)
                    {
                        Destroy(nanoarmarmor);
                    }
                    var nanoarmorbase = Instantiate(stealthPrefab, gameObject.transform.position, transform.rotation);
                    nanoarmor = nanoarmorbase as GameObject;
                    nanoarmor.transform.parent = this.transform;
                    if (gameWorld.AllPlayers[0].ActiveHealthController.DamageCoeff != 1f && Entermap())
                    {
                        gameWorld.AllPlayers[0].ActiveHealthController.SetDamageCoeff(1f);
                    }
                    isstealth = true;
                    isarmor = false;
                    nanomode = 0;
                }
                if (!isclick && !isfly && nanosuitcore.flycore.Value && !isenergyempty)//是否启动飞行模式
                {
                    timer -= Time.deltaTime;
                    if (Input.GetKeyDown(KeyCode.Space) && First == Clickcount.zerotime)
                    {
                        timer = 0.8f;
                        First = Clickcount.firsttime;

                    }
                    if (Input.GetKeyUp(KeyCode.Space) && First == Clickcount.firsttime)
                    {
                        First = Clickcount.secondtime;

                    }
                    if (timer < 0f)
                    {
                        First = Clickcount.zerotime;
                    }
                    if (Input.GetKey(KeyCode.Space) && First == Clickcount.secondtime && timer > 0f && nanosuitcore.flycore.Value)
                    {
                        var floorbase = Instantiate(flymodePrefab, gameObject.transform.position, transform.rotation);
                        flymode = floorbase as GameObject;
                        flymode.transform.parent = this.transform;
                        isclick = true;
                        isfly = true;
                        First = Clickcount.zerotime;
                        //Console.WriteLine(floorobject.transform.parent.localPosition);
                    }
                }
                if (isfly)
                {
                    flymode.transform.Translate(0, 0, 0.1f * Time.deltaTime);
                }
                if (Input.GetKeyUp(KeyCode.Space) && isfly)
                {
                    Destroy(flymode);
                    isfly = false;
                    isclick = false;
                    if (!isstealth || !isarmor || !isnanovision)
                    {
                        ischarging = true;
                        energytimer = 0;
                    }
                }
                if (isenergyempty)//判断能量是否耗尽
                {
                    if (!isplayaudio)
                    {
                        nanoenergyhud.GetComponent<AudioSource>().Play();
                        isplayaudio = true;
                    }
                    maxenergy = 0;
                    armorvoice = 0;
                    stealthvoice = 2;
                    newmaterial[0] = basematerial;
                    newmaterial[1] = basematerial02;
                    Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
                    if (nanoarmor != null)
                    {
                        Destroy(nanoarmor);
                    }
                    if (nanoarmorhud != null)
                    {
                        Destroy(nanoarmorhud);
                    }
                    if (nanoarmarmor != null)
                    {
                        Destroy(nanoarmarmor);
                    }
                    if (flymode != null)
                    {
                        Destroy(flymode);
                    }
                    if (nanovisionhud != null)
                    {
                        Destroy(nanovisionhud);
                    }
                    if (AItarget != null)
                    {
                        for (int i = 1; i < 100; i++)//获取全部AI
                        {
                            if (AItarget[i] != null)
                            {
                                Destroy(AItarget[i]);
                            }
                        }
                    }
                    if (gameWorld.AllPlayers[0].ActiveHealthController.DamageCoeff != 1f && Entermap())
                    {
                        gameWorld.AllPlayers[0].ActiveHealthController.SetDamageCoeff(1f);
                    }
                    isfly = false;
                    isclick = false;
                    isstealth = false;
                    isarmor = false;
                    isnanovision = false;
                    energytimer += Time.deltaTime;
                    if (energytimer >= 1)
                    {
                        energytimer = 0;
                        ischarging = true;
                        isenergy = false;
                        isenergyempty = false;
                    }
                }
                if (isstealth)//隐身模式能量自然消耗
                {
                    maxenergy -= Time.deltaTime * stealthbasecost;
                    ischarging = false;
                    if (maxenergy <= 0)
                    {
                        maxenergy = 0;
                        energytimer = 0;
                        isenergyempty = true;
                    }
                    if (gameWorld.AllPlayers[0].ActiveHealthController.DamageCoeff != 1f && Entermap())
                    {
                        gameWorld.AllPlayers[0].ActiveHealthController.SetDamageCoeff(1f);
                    }
                }
                if (isarmor)//装甲模式能量自然消耗
                {
                    maxenergy -= Time.deltaTime * armorbasecost;
                    ischarging = false;
                    if (maxenergy <= 0)
                    {
                        maxenergy = 0;
                        energytimer = 0;
                        isenergyempty = true;
                    }
                    if (isarmorhit)
                    {
                        StartCoroutine("ArmorHit");
                        isarmorhit = false;
                    }
                    if (isarmorhitdanger)
                    {
                        StartCoroutine("ArmorHitdanger");
                        isarmorhitdanger = false;
                    }
                    if (gameWorld.AllPlayers[0].ActiveHealthController.DamageCoeff != -1f && Entermap())
                    {
                        gameWorld.AllPlayers[0].ActiveHealthController.SetDamageCoeff(-1f);
                    }
                }
                if (isfly)//飞行模式能量自然消耗
                {
                    maxenergy -= Time.deltaTime * flybasecost;
                    ischarging = false;
                    if (maxenergy <= 0)
                    {
                        maxenergy = 0;
                        energytimer = 0;
                        isenergyempty = true;
                    }
                }
                if (isnanovision && Entermap())//纳米视野能量自然消耗
                {
                    maxenergy -= Time.deltaTime * nanovisionbasecost;
                    ischarging = false;
                    if (maxenergy <= 0)
                    {
                        maxenergy = 0;
                        energytimer = 0;
                        isenergyempty = true;
                    }
                    if (!isenergyempty)
                    {
                        if (gameWorld.AllPlayers.Count >= 2)
                        {
                            for (int i = 1; i < gameWorld.AllPlayers.Count; i++)//获取全部AI
                            {
                                AIhealth[i] = gameWorld.AllPlayers[i].HealthController.IsAlive;
                                if (!AIhealth[i])
                                {
                                    Destroy(AItarget[i]);
                                }
                                if (AItarget[i] != null && AIhealth[i])
                                {
                                    AItarget[i].transform.position = gameWorld.AllPlayers[i].Transform.position;
                                }
                            }
                        }
                    }
                }
            }
            if (!nanosuitcore.NanoSystemOnline.Value)
            {
                if (nanoenergyhud != null)
                {
                    Destroy(nanoenergyhud);
                }
            }
        }
        void FixedUpdate()
        {
        }
        IEnumerator CloakCharacter()
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
        IEnumerator DeCloakCharacter()
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
                    newmaterial[0] = basematerial;
                    newmaterial[1] = basematerial02;
                    Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
                }

            }
        }
        IEnumerator ArmorHit()
        {
            float ArmorColorhit = 3f;
            ArmorColor.r = 1f;
            ArmorColor.g = 1f;
            ArmorColor.b = 1f;
            if (nanoarmorhud != null)
            {
                MeshRenderer[] armorhudMeshRenderer = nanoarmorhud.GetComponentsInChildren<MeshRenderer>();               
                foreach (MeshRenderer child in armorhudMeshRenderer)
                {
                    armorhudMeshRenderer[0].materials[0].SetFloat("_Maintextureintensity", ArmorColorhit);
                    armorhudMeshRenderer[0].materials[0].SetColor("_Edgecolor", ArmorColor);
                    while (ArmorColorhit >= nanosuitcore.ArmorhudAlpha.Value)
                    {
                        ArmorColorhit -= 0.5f;
                        armorhudMeshRenderer[0].materials[0].SetFloat("_Maintextureintensity", ArmorColorhit);
                        yield return 0;
                    }
                }
            }           
        }
        IEnumerator ArmorHitdanger()
        {
            float ArmorColorhit = 2.4f;
            float ArmorFade = 0f;
            ArmorColor.r = 1f;
            ArmorColor.g = ArmorFade;
            ArmorColor.b = ArmorFade;
            if (nanoarmorhud != null)
            {
                MeshRenderer[] armorhudMeshRenderer = nanoarmorhud.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer child in armorhudMeshRenderer)
                {
                    armorhudMeshRenderer[0].materials[0].SetFloat("_Maintextureintensity", ArmorColorhit);
                    armorhudMeshRenderer[0].materials[0].SetColor("_Edgecolor", ArmorColor);
                    while (ArmorColorhit >= nanosuitcore.ArmorhudAlpha.Value)
                    {
                        ArmorColorhit -= 0.5f;
                        armorhudMeshRenderer[0].materials[0].SetFloat("_Maintextureintensity", ArmorColorhit);       
                        yield return 0;
                    }
                    while (ArmorFade <= 1f)
                    {
                        ArmorFade += 0.2f;
                        ArmorColor.g = ArmorFade;
                        ArmorColor.b = ArmorFade;
                        armorhudMeshRenderer[0].materials[0].SetColor("_Edgecolor", ArmorColor);
                        yield return 0;
                    }
                }
            }
        }
    }
    public class armorhit : MonoBehaviour
    {
        private float costenergy;
        private float getenergy;
        public int armorpower=45;
        public int modekey;
        public void armorhit_method(int damage, int PenetrationPower,bool IsYourPlayer)
        {
            getenergy = nanosuit.maxenergy;
            if (modekey==1)
            {
                if (PenetrationPower < armorpower)
                {
                    costenergy = 1;
                    getenergy = getenergy - costenergy;
                    nanosuit.isarmorhit = true; 
                }
                else
                {
                    costenergy = 5;
                    getenergy = getenergy - costenergy;
                    nanosuit.isarmorhitdanger = true; 
                }
            }
            if (modekey == 2)
            {
                if (IsYourPlayer)
                {
                    getenergy = 0;
                }
                if (!IsYourPlayer && damage > 0)
                {
                    costenergy = 15;
                    getenergy = getenergy - costenergy;
                }
            }
            nanosuit.maxenergy = getenergy;            
        }
        
        void Update()
        {
            armorpower = nanosuit.armorpowerbase;
        }
        public void youarehit(bool ishit)//装甲模式下被打启动特殊受击音效
        {
            if (ishit && modekey == 1)
            {
                this.GetComponent<AudioSource>().Play();
            }
        }
    }

    public class armorhide : MonoBehaviour
    {
        void FixedUpdate()
        {
            if (Input.GetMouseButtonDown(0))//点击时取消碰撞体积
            {
                this.GetComponent<BoxCollider>().enabled = false;
            }
            if (Input.GetMouseButtonUp(0))//抬起时显示碰撞体积
            {
                this.GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

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

    public class Suitleglink : MonoBehaviour
    {
        public Material legstealthmaterial;
        public Material legarmormaterial;
        public Material basematerial;
        private Material[] suitlegmaterial;
        private bool iscloak=false;
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

    public class NanosuitPatch : ModulePatch
    {
        //搜索EFT空间下的Player类里的ReceiveDamage
        protected override MethodBase GetTargetMethod() => typeof(Player).GetMethod("ReceiveDamage", BindingFlags.Instance | BindingFlags.NonPublic);

        [PatchPostfix]
        static void PostFix(ref Player __instance, EDamageType type)
        {
            if (__instance.IsYourPlayer && (type == EDamageType.Bullet || type == EDamageType.Explosion || type == EDamageType.Sniper || type == EDamageType.Landmine || type == EDamageType.GrenadeFragment || type == EDamageType.Barbed || type == EDamageType.Fall))
            {
                //const float delayTime = 0f;//延迟时间
                //const float workTime = 60f;//持续时间
                //const float residueTime = 0f; //残留时间
                //const float strength = 5f;//强度
                nanosuit.startfixdebuff = true;
                nanosuit.iscureyourself = false;
                nanosuit.fixdelaytime = 0;
                //如果已经有Buff就在原Buff上添加时长
                //if (__instance.ActiveHealthController.BodyPartEffects.Effects[0].Any(v => v.Key == "HealthBoost"))//其它效果在GClass2103（3.5.0客户端）2112（351-353客户端）
                //{
                //ActiveHealthControllerClass.GClass2103 nanoHealthBoost = typeof(ActiveHealthControllerClass).GetMethod("FindActiveEffect", BindingFlags.Instance | BindingFlags.Public).MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("HealthBoost", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head }) as ActiveHealthControllerClass.GClass2103;
                //if (nanoHealthBoost.TimeLeft < 60) nanoHealthBoost.AddWorkTime(60f, false); //350是2091；351-353是2100；355是2103
                //return;
                //}
                if (__instance.ActiveHealthController.BodyPartEffects.Effects[0].Any(v => v.Key == "PainKiller"))
                {
                    ActiveHealthControllerClass.GClass2103 nanoPainKiller = typeof(ActiveHealthControllerClass).GetMethod("FindActiveEffect", BindingFlags.Instance | BindingFlags.Public).MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("PainKiller", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head }) as ActiveHealthControllerClass.GClass2103;
                    if (nanoPainKiller.TimeLeft < 60) nanoPainKiller.AddWorkTime(60f, false);
                    return;
                }

                MethodInfo method = typeof(ActiveHealthControllerClass).GetMethod("method_15", BindingFlags.Instance | BindingFlags.NonPublic);
                method.MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("PainKiller", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head, 0f, 60f, 0f, 1f, null });
                //method.MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("HealthBoost", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head, delayTime, workTime, residueTime, strength, null });
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
                Console.WriteLine("你做掉了一个AI" );
            }
        }
    }
}
