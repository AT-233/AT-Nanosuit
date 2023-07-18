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
using EFT.Interactive;
using Aki.Reflection.Patching;
using Object = UnityEngine.Object;
using Comfort.Common;
using System.Collections;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace nanosuit
{
    [BepInPlugin("AT.nanosuit", "AT.纳米生化装Nanosuit", "2.0.0.0")]
    public class nanosuitcore : BaseUnityPlugin
    {
        // 窗口开关
        private static GameWorld gameWorld;
        private static ManualLogSource logger;
        public static ConfigEntry<int> armorcost;
        public static ConfigEntry<int> armordefenselow;
        public static ConfigEntry<int> armordefensehigh;
        public static ConfigEntry<float> stealthcost;
        public static ConfigEntry<float> stealthcostmove;
        public static ConfigEntry<float> speedcostrun;
        public static ConfigEntry<float> speedupbase;
        public static ConfigEntry<int> flycost;
        public static ConfigEntry<int> fuckdoorcost;
        public static ConfigEntry<int> quickgrenadecost;
        public static ConfigEntry<int> nanocharg;
        public static ConfigEntry<int> nanochargdelay;
        public static ConfigEntry<int> nanovisioncost;
        public static ConfigEntry<int> FixDelay;
        public static ConfigEntry<bool> flycore;
        public static ConfigEntry<bool> NanoSystemOnline;
        public static ConfigEntry<bool> curemodeOnline;
        public static ConfigEntry<bool> BroadcastOnline;
        public static ConfigEntry<bool> AmmoUIOnline;
        public static ConfigEntry<bool> Lockradar;
        public static ConfigEntry<string> nanoup;
        public static ConfigEntry<string> nanovoice;
        //public static ConfigEntry<string> language;
        public static ConfigEntry<KeyCode> ARMOR;
        public static ConfigEntry<KeyCode> STEALTH;
        public static ConfigEntry<KeyCode> NANOVISION;
        public static ConfigEntry<KeyCode> POWER;
        public static ConfigEntry<KeyCode> SPEED;
        public static ConfigEntry<float> powercost;
        public static ConfigEntry<float> powerweapon;
        public static ConfigEntry<float> powerjumpcost;
        public static ConfigEntry<float> speedcost;
        public static ConfigEntry<float> speedratio;
        public static ConfigEntry<float> nanovolume;
        public static ConfigEntry<float> Xzhou;
        public static ConfigEntry<float> Yzhou;
        public static ConfigEntry<float> Xzhouxuan;
        public static ConfigEntry<float> Yzhouxuan;
        public static ConfigEntry<float> Zzhouxuan;
        //public static ConfigEntry<float> ArmorhudScale;
        public static ConfigEntry<float> ArmorhudAlpha;
        public static ConfigEntry<float> FixIntensity;
        public static string[] nanouplist = { "无(Null)", "生存强化(Enhanced Survival)", "自动装甲(Auto Armor)", "能量吸收(Energy absorption)" };
        public static string[] languagelist = { "二代男声(V2.0male)", "一代男声(V1.0male)", "一代女声(V1.0female)" };
        public static BindingFlags BFlags = BindingFlags.NonPublic
                                            | BindingFlags.Public
                                            | BindingFlags.Instance
                                            | BindingFlags.DeclaredOnly
                                            | BindingFlags.Static;
        public void Awake()
        {
            var harmony = new Harmony("nanosuit");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            logger = Logger;
            Logger.LogInfo($"nanosuit: Loading");
            //language = Config.Bind<string>("语言设置(Language Setting)", "能量条HUD语言(Language of energy HUD)", nanouplist[0],
            //    new ConfigDescription("选择能量条HUD显示文字语言，重启游戏后生效(Select language of energy HUD.It takes effect after you restart the game)", new AcceptableValueList<string>(languagelist)));
            armorcost = Config.Bind<int>("装甲模式配置(Armor Settings)", "装甲消耗(Armor Cost)", 2, "装甲模式下能量自然消耗每秒的值(Natural energy cost per second in Armor mode)");
            armordefenselow = Config.Bind<int>("装甲模式配置(Armor Settings)", "抵抗轻伤消耗(Resist Minor Damage Cost)", 2, "装甲模式下受到子弹以及破片伤害消耗的能量(The amount of energy taken by bullets and fragments in Armor mode)");
            armordefensehigh = Config.Bind<int>("装甲模式配置(Armor Settings)", "抵抗重伤消耗(Resist Serious Damage Cost)", 10, "装甲模式下受到爆炸、地雷、狙击手、跌落伤害消耗的能量(The amount of energy taken by Explosion, Sniper, Mine and Fall in Armor mode)");
            //ArmorhudScale = Config.Bind("装甲模式配置(Armor Settings)", "HUD缩放(HUD Scale)", 1f, new ConfigDescription("装甲模式下的HUD缩放(The HUD Scale in Armor mode)", new AcceptableValueRange<float>(0.8f, 3f)));
            ArmorhudAlpha = Config.Bind("装甲模式配置(Armor Settings)", "HUD透明度(HUD Alpha)", 0.3f, new ConfigDescription("装甲模式下的HUD透明度(The HUD Alpha in Armor mode)", new AcceptableValueRange<float>(0f, 1f)));
            stealthcost = Config.Bind<float>("隐身模式配置(Stealth Settings)", "隐身消耗(Stealth Cost)", 1, "隐身模式下能量自然消耗每秒的值(Natural energy cost per second in Stealth mode)");
            stealthcostmove = Config.Bind<float>("隐身模式配置(Stealth Settings)", "移动时隐身消耗(Stealth Cost)", 5, "隐身模式下移动时能量自然消耗每秒的值(Natural energy cost per second in Stealth mode when moving)");
            powercost = Config.Bind<float>("力量模式配置(Power Settings)", "力量消耗(Power Cost)", 0.5f, "力量模式下能量自然消耗每秒的值(Natural energy cost per second in Power mode)");
            fuckdoorcost = Config.Bind<int>("力量模式配置(Power Settings)", "踹门消耗(FuckDoor Cost)", 30, "力量模式下能量自然消耗每秒的值(Energy cost fuck the door in Power mode)");
            powerjumpcost = Config.Bind<float>("力量模式配置(Power Settings)", "力量跳跃消耗(Power Jump Cost)", 15, "力量模式下每次跳跃消耗的能量(Energy cost per jump in Power Mode)");
            powerweapon = Config.Bind("力量模式配置(Power Settings)", "力量后座(Power Recoil)", 0.1f, new ConfigDescription("力量模式下的武器后座力(Weapon recoil in Power mode)", new AcceptableValueRange<float>(0f, 1f)));
            speedcost = Config.Bind<float>("速度模式配置(Speed Settings)", "速度消耗(Speed Cost)", 0.5f, "速度模式下能量自然消耗每秒的值(Natural energy cost per second in Speed mode)");
            speedratio = Config.Bind<float>("速度模式配置(Speed Settings)", "武器使用速度倍率(Weapon use speed Ratio)", 1.8f, "速度模式下武器使用速度倍率(Speed Ratio for weapon use in Speed mode)");
            quickgrenadecost = Config.Bind<int>("速度模式配置(Speed Settings)", "快速丢雷消耗(QuickGrenade Cost)", 3, "速度模式下丢雷一次消耗能量(Energy cost quick throw grenade in Speed mode)");
            speedcostrun = Config.Bind<float>("速度模式配置(Speed Settings)", "奔跑消耗(Running Cost)", 30, "速度模式下奔跑消耗的能量(Energy cost when running in Speed mode)");
            speedupbase = Config.Bind<float>("速度模式配置(Speed Settings)", "移动速度倍率(Moving Ratio)", 1.2f, "速度模式下移动速度倍率(Speed Ratio for moving in Speed mode)");
            nanocharg = Config.Bind<int>("充能配置(Charge Settings)", "充能速率(Charging Speed)", 25, "纳米服每秒充能速率(Nanosuit charge rate per second)");
            nanochargdelay = Config.Bind<int>("充能配置(Charge Settings)", "充能延迟(Charging Delay)", 2, "纳米服关闭功能后延迟几秒后开始充能(Turn off the function after a delay of a few seconds to start charging)");           
            nanoup = Config.Bind<string>("强化方案选择(Enhanced Choose)", "根据你的战斗习惯选择不同的强化方案(Depending on your combat habits, choose different Enhanced programs)", nanouplist[1], 
                new ConfigDescription("下列强化方案中选择一个(Choose one of the following reinforcement programs)\n默认生存强化：穿戴者受伤后，10秒未受攻击后去除全部debuff，缓慢回复全部生命\n自动装甲：被AI锁定自动开启装甲模式\n能量吸收：击杀AI后回复30点能量(Default Enhanced Survival:After the wearer is injured, remove all debuff after 10 seconds without being attacked, and slowly restore all health\nAuto Armor: Automatically activates armor mode when locked by AI\nEnergy Absorption: Regenerate 30 energy after killing an AI)", new AcceptableValueList<string>(nanouplist)));
            ARMOR = Config.Bind<KeyCode>("按键设置(KeyCode Settings)", "装甲按键(Armor KeyCode)", KeyCode.Q, "装甲模式启动按钮(Armor mode start button)");
            STEALTH = Config.Bind<KeyCode>("按键设置(KeyCode Settings)", "隐身按键(Stealth KeyCode)", KeyCode.E, "隐身模式启动按钮(Stealth mode start button)");
            NANOVISION = Config.Bind<KeyCode>("按键设置(KeyCode Settings)", "纳米视野按键(Nanovision KeyCode)", KeyCode.H, "纳米视野启动按钮(Nanovision  start button)");
            POWER = Config.Bind<KeyCode>("按键设置(KeyCode Settings)", "力量模式按键(Power KeyCode)", KeyCode.J, "力量模式启动按钮(Power start button)");
            SPEED = Config.Bind<KeyCode>("按键设置(KeyCode Settings)", "速度模式按键(Speed KeyCode)", KeyCode.K, "速度模式启动按钮(Speed start button)");
            flycore = Config.Bind<bool>("垂直推进器设置(Propeller Settings)", "是否启动垂直喷射器(Whether to start the Propeller)", false, "启动后跳跃时长按空格可消耗能量垂直起飞(After starting the jump, press space for a long time to consume energy for vertical takeoff)");
            flycost = Config.Bind<int>("垂直推进器设置(Propeller Settings)", "推进器消耗(Propeller Cost)", 15, "飞行时每秒能量消耗值(Natural energy cost per second in Propeller mode)");
            nanovisioncost = Config.Bind<int>("纳米视野设置(Nanovision Settings)", "纳米视野消耗(Nanovision Cost)", 5, "纳米视野开启后每秒能量消耗值(Natural energy cost per second in Nanovision mode)");
            Lockradar = Config.Bind<bool>("纳米视野设置(Nanovision Settings)", "锁定雷达视角(Locked radar viewt)", false, "开启后敌方位置随玩家方向改变(The enemy position changes with the player's direction)");
            nanovolume = Config.Bind("纳米系统设置(Nanosystem Settings)", "设置音量(Volume Settings)", 1f, new ConfigDescription("纳米服各个模式启动时的音效音量(The sound volume of each mode on the Nano Suit)", new AcceptableValueRange<float>(0f, 1f)));
            NanoSystemOnline = Config.Bind<bool>("纳米系统设置(Nanosystem Settings)", "纳米系统是否启动(NanoSystem online or not)", true, "纳米机器，小子！启动纳米系统(Nanomachine,son!Start-up NanoSystem)");
            BroadcastOnline = Config.Bind<bool>("纳米系统设置(Nanosystem Settings)", "击杀播报是否启动(Kill broadcast is online or not)", true, "开启后击杀AI播报语音(Turn on kill AI to broadcast voice)");
            AmmoUIOnline = Config.Bind<bool>("纳米系统设置(Nanosystem Settings)", "弹药显示是否启动(Ammunition counter is online or not)", true, "开启后击杀AI播报语音(Turn on Ammunition counter)");
            nanovoice = Config.Bind<string>("纳米系统设置(Nanosystem Settings)", "选择你的纳米服语音音效(Select your nanosuit voice sound effect)", languagelist[0],
                new ConfigDescription("选择你的纳米服语音音效(Select your nanosuit voice sound effect)", new AcceptableValueList<string>(languagelist)));
            Xzhou = Config.Bind("纳米系统设置(Nanosystem Settings)", "能量条X轴位置(Energy HUD X-axis position)", -100f, new ConfigDescription("能量条在界面的左右位置(Energy HUD on the left and right positions of the interface)", new AcceptableValueRange<float>(-700, -100)));
            Yzhou = Config.Bind("纳米系统设置(Nanosystem Settings)", "能量条Y轴位置(Energy HUD Y-axis position)", 30f, new ConfigDescription("能量条在界面的上下位置(Energy HUD on the upper and lower positions of the interface)", new AcceptableValueRange<float>(30, 400)));
            Xzhouxuan = Config.Bind("纳米系统设置(Nanosystem Settings)", "能量条X轴旋转(Energy HUD X-axis rotation)", 20f, new ConfigDescription("能量条在界面的上下位置(Energy HUD spin back and forth of the interface)", new AcceptableValueRange<float>(-40, 40)));
            Yzhouxuan = Config.Bind("纳米系统设置(Nanosystem Settings)", "能量条Y轴旋转(Energy HUD Y-axis rotation)", 30f, new ConfigDescription("能量条在界面的上下位置(Energy HUD spin left and right of the interface)", new AcceptableValueRange<float>(-50, 50)));
            Zzhouxuan = Config.Bind("纳米系统设置(Nanosystem Settings)", "能量条Z轴旋转(Energy HUD Z-axis rotation)", -5f, new ConfigDescription("能量条在界面的上下位置(Energy HUD spin upper and lower of the interface)", new AcceptableValueRange<float>(-25, 25)));
            curemodeOnline = Config.Bind<bool>("自动修复系统设置(Automatic Treatment System Settings)", "自动修复系统是否启动(Automatic treatment system online or not)", true, "你为什么还不死？选择生存强化后受伤10秒后修复全部黑色部位，去除所有负面效果，缓慢回复所有生命值(Why don't you die?After choosing Enhanced Survival,remove all negative states, repair black areas, and restore all health after 10 seconds of damage)");
            FixIntensity = Config.Bind("自动修复系统设置(Automatic Treatment System Settings)", "修复速度(Treatment Rate)", 0.1f, new ConfigDescription("触发自动修复后的修复速率(Rate of Treatment after Automatic Treatment is triggered)", new AcceptableValueRange<float>(0f, 1f)));
            FixDelay = Config.Bind<int>("自动修复系统设置(Automatic Treatment System Settings)", "修复延迟(Treatment Delay)", 10, "收到伤害后延迟几秒后开始自动修复(After receiving damage delay of a few seconds, Automatic Treatment begins)");
        }
        void Start()
        {
            
        }
        void Update()
        {
           
        }
        void LateUpdate()
        {

        }

        [HarmonyPatch(typeof(GClass2624), "HitCollider", MethodType.Getter)]
        public class NanosuitClockMode
        {
            public static void Postfix(GClass2624 __instance, ref Collider __result)//350是2611,351-353是2620, 355-356是2623，357是2624
            {
                var hitman = __instance.Player;//获取是谁射的子弹
                if (__result != null)
                {
                    var cloakhitScript = __result.GetComponentInParent<armorhit>();
                    if (cloakhitScript != null)
                    {
                        cloakhitScript.cloakhit_method(hitman.IsYourPlayer);
                    }
                }
            }
            
        }
    }
    public class nanosuit : MonoBehaviour
    {
        public AudioClip[] audios;
        public AudioClip[] audiosman;
        public AudioClip[] audioswoman;
        public KeyCode ARMOR;
        public KeyCode STEALTH;
        public KeyCode NANOVISION;
        public GameObject Hand;
        public static AssetBundle nanosuitBundle; 
        private static GameWorld gameWorld;
        private Animation armorhudani;
        public Material stealthmaterial;
        public Material armormaterial;
        public Material armormaterial02;
        public Material basematerial;
        public Material basematerial02;
        public Material powermaterial;
        public Material powermaterial02;
        public Material speedmaterial;
        public Material speedmaterial02;
        Material[] newmaterial;
        private Color cloakColor = Color.white;
        private Color ArmorColor = Color.white;
        public static float maxenergy = 100;
        public static float speeduprunbase = 2f;
        public static int nowenergy;
        //private ArmorComponent armor;
        private int armorvoice;
        private int stealthvoice;
        private int powervoice;
        private int speedvoice;
        private int nanomode;
        //private int flybasecost = 15;
        private GameObject clockbase;
        private GameObject nanoarmorhud;
        private GameObject nanoenergyhud;
        private GameObject nanovisionhud;
        //private GameObject nanoarmarmor;
        public static GameObject minimap;
        private GameObject flymode;
        private GameObject marktarget;
        private GameObject fastmemu;
        private GameObject[] AItarget;      
        private float energytimer = 0;
        private float timer = 0;
        private float MAXHP = 0;
        private float NOWHP = 0;
        private float oldknifeskill;
        private float BaseFov;
        private float NowFov;
        private float SetFov;
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
        public static bool youarehit = false;
        public static bool isfly = false;
        public static bool ispower = false;
        public static bool isspeed = false;
        public static bool isautoarmor = false;
        public static bool isfirstarmor = false;
        private bool isplayaudio = false;
        private bool isclick = false;
        private bool islowpower = false;
        private bool isfirstlowpower = false;
        private bool isarmorfov = false;
        private bool[] AIhealth;
        public enum Clickcount
        {
            firsttime,
            secondtime,
            zerotime
        }
        Clickcount First = Clickcount.zerotime;
        public static Object stealthPrefab { get; private set; }
        public static Object armorhudPrefab { get; private set; }
        public static Object energyhudPrefab { get; private set; }
        public static Object enenergyhudPrefab { get; private set; }
        public static Object nanovisionhudPrefab { get; private set; }
        public static Object flymodePrefab { get; private set; }
        public static Object nanotargetPrefab { get; private set; }
        public static Object minimapPrefab { get; private set; }
        public static Object fastmemuPrefab { get; private set; }
        // Start is called before the first frame update
        public static bool Entermap() => Singleton<GameWorld>.Instantiated;
        //public static bool Entermap() => Singleton<GameWorld>.Instantiated && gameWorld.AllPlayers != null && gameWorld.AllPlayers.Count > 0 && gameWorld.MainPlayer.IsYourPlayer;
        void Start()
        {
            BaseFov = CameraClass.Instance.Fov;
            armorvoice = 0;
            stealthvoice = 2;
            powervoice = 5;
            speedvoice = 9;
            energytimer = 0;
            maxenergy = 100;           
            isclick = false;
            isfly = false;
            isarmorfov = false;
            islowpower = false;
            isfirstlowpower = false;
            oldknifeskill = 1;
            isstealth = false;
            isarmor = false;
            ispower = false;
            isspeed = false;
            gameWorld = Singleton<GameWorld>.Instance;
            AItarget = new GameObject[999];
            AIhealth = new bool[999];
            new ArmormodePatch().Enable();
            new FastWeapon().Enable();
            new BoyNextDoor().Enable();
            new FastGrenade().Enable();
            new speedupcore().Enable();
            new speedmode().Enable();
            new speeduplink().Enable();
            newmaterial = new Material[Hand.GetComponent<SkinnedMeshRenderer>().materials.Length];//替换材质只能这样搞
            for (int i = 0; i < newmaterial.Length; i++)//获取物体全部材质
            {
                newmaterial[i] = Hand.GetComponent<SkinnedMeshRenderer>().materials[i];
            }
            newmaterial[0] = basematerial;//设置默认材质
            newmaterial[1] = basematerial02;//设置默认材质
            Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
            if (energyhudPrefab == null)//加载装甲和隐身模块音效
            {
                String nanosuitcore = Path.Combine(Environment.CurrentDirectory, "BepInEx/plugins/atmod/nanosuitcore");
                if (!File.Exists(nanosuitcore))
                    return;
                nanosuitBundle = AssetBundle.LoadFromFile(nanosuitcore);
                if (nanosuitBundle == null)
                    return;
                stealthPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/Stealthsound.prefab");
                armorhudPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/HUD/ArmorHUD.prefab");
                nanovisionhudPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/HUD/NanovisionHUD.prefab");
                energyhudPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/GUI/NanoHUD.prefab");
                enenergyhudPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/HUD/EnergyHUD(en).prefab");
                flymodePrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/flymode.prefab");
                nanotargetPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/HUD/Nanotarget.prefab");
                minimapPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/Minimap/Mimiradar.prefab");
                fastmemuPrefab = nanosuitBundle.LoadAsset("Assets/Nanosuit/GUI/Memu/Nanomemu.prefab");
                Console.WriteLine("纳米服功能模块加载完毕");
            }
        }

        // Update is called once per frame
        void Update()
        {           
            if (nanosuitcore.NanoSystemOnline.Value)
            {
                NowFov = CameraClass.Instance.Fov;
                //FastMemu();
                //killMemu();
                //testmode();
                energywaring();
                nowenergy = (int)maxenergy;
                this.GetComponent<AudioSource>().volume = nanosuitcore.nanovolume.Value;//纳米系统音量调整                               
                if (nanosuitcore.nanoup.Value == "生存强化(Enhanced Survival)")
                {
                    if (startfixdebuff && Entermap()&& nanosuitcore.curemodeOnline.Value)
                    {
                        fixdelaytime += Time.deltaTime;
                        var HeadHP = gameWorld.MainPlayer.ActiveHealthController.GetBodyPartHealth(EBodyPart.Head, true);
                        var ChestHP = gameWorld.MainPlayer.ActiveHealthController.GetBodyPartHealth(EBodyPart.Chest, true);
                        var LeftArmHP = gameWorld.MainPlayer.ActiveHealthController.GetBodyPartHealth(EBodyPart.LeftArm, true);
                        var LeftLegHP = gameWorld.MainPlayer.ActiveHealthController.GetBodyPartHealth(EBodyPart.LeftLeg, true);
                        var RightArmHP = gameWorld.MainPlayer.ActiveHealthController.GetBodyPartHealth(EBodyPart.RightArm, true);
                        var RightLegHP = gameWorld.MainPlayer.ActiveHealthController.GetBodyPartHealth(EBodyPart.RightLeg, true);
                        var StomachHP = gameWorld.MainPlayer.ActiveHealthController.GetBodyPartHealth(EBodyPart.Stomach, true);
                        if (fixdelaytime >= nanosuitcore.FixDelay.Value && !iscureyourself)
                        {
                            gameWorld.MainPlayer.ActiveHealthController.RemoveNegativeEffects(EBodyPart.Common);
                            gameWorld.MainPlayer.ActiveHealthController.RestoreBodyPart(EBodyPart.LeftArm, 1f);
                            gameWorld.MainPlayer.ActiveHealthController.RestoreBodyPart(EBodyPart.LeftLeg, 1f);
                            gameWorld.MainPlayer.ActiveHealthController.RestoreBodyPart(EBodyPart.RightArm, 1f);
                            gameWorld.MainPlayer.ActiveHealthController.RestoreBodyPart(EBodyPart.RightLeg, 1f);
                            gameWorld.MainPlayer.ActiveHealthController.RestoreBodyPart(EBodyPart.Stomach, 1f);                
                            iscureyourself = true;
                            fixdelaytime = 0;
                            Hand.GetComponent<AudioSource>().Play();
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
                                gameWorld.MainPlayer.ActiveHealthController.ChangeHealth(EBodyPart.Head, nanosuitcore.FixIntensity.Value, default);
                                nowHPHead += nanosuitcore.FixIntensity.Value;
                            }
                            if (nowHPChest < ChestHP.Maximum)
                            {
                                gameWorld.MainPlayer.ActiveHealthController.ChangeHealth(EBodyPart.Chest, nanosuitcore.FixIntensity.Value, default);
                                nowHPChest += nanosuitcore.FixIntensity.Value;
                            }
                            if (nowHPLeftArm < LeftArmHP.Maximum)
                            {
                                gameWorld.MainPlayer.ActiveHealthController.ChangeHealth(EBodyPart.LeftArm, nanosuitcore.FixIntensity.Value, default);
                                nowHPLeftArm += nanosuitcore.FixIntensity.Value;
                            }
                            if (nowHPLeftLeg < LeftLegHP.Maximum)
                            {
                                gameWorld.MainPlayer.ActiveHealthController.ChangeHealth(EBodyPart.LeftLeg, nanosuitcore.FixIntensity.Value, default);
                                nowHPLeftLeg += nanosuitcore.FixIntensity.Value;
                            }
                            if (nowHPRightArm < RightArmHP.Maximum)
                            {
                                gameWorld.MainPlayer.ActiveHealthController.ChangeHealth(EBodyPart.RightArm, nanosuitcore.FixIntensity.Value, default);
                                nowHPRightArm += nanosuitcore.FixIntensity.Value;
                            }
                            if (nowHPRightLeg < RightLegHP.Maximum)
                            {
                                gameWorld.MainPlayer.ActiveHealthController.ChangeHealth(EBodyPart.RightLeg, nanosuitcore.FixIntensity.Value, default);
                                nowHPRightLeg += nanosuitcore.FixIntensity.Value;
                            }
                            if (nowHPStomach < StomachHP.Maximum)
                            {
                                gameWorld.MainPlayer.ActiveHealthController.ChangeHealth(EBodyPart.Stomach, nanosuitcore.FixIntensity.Value, default);
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
                       
                        isshengcun = true;
                    }
                }
                if (nanosuitcore.nanoup.Value != "生存强化(Enhanced Survival)" && isshengcun == true)
                {
                    new NanosuitPatch().Disable();//关闭受击buff     
                    isshengcun = false;
                }
                //if (nanosuitcore.nanoup.Value != "装甲强化(Enhanced Armor)")
                //{
                //    armorbasecost = nanosuitcore.armorcost.Value;
                //}
                //else
                //{
                //    CommonSet();
                //    nanosuitcore.armorcost.Value = 1;
                //    nanosuitcore.armordefenselow.Value = 1;
                //    nanosuitcore.armordefensehigh.Value = 5;
                //    armorbasecost = 1;
                //    isshengcun = false;
                //}
                //if (nanosuitcore.nanoup.Value != "隐身强化(Enhanced Stealth)")
                //{
                //    stealthbasecost = nanosuitcore.stealthcost.Value;
                //}
                //else
                //{
                //    CommonSet();
                //    nanosuitcore.stealthcost.Value = 3;
                //    stealthbasecost = 3;
                //    isshengcun = false;
                //}
                //if (nanosuitcore.nanoup.Value != "充能强化(Enhanced Charge)")
                //{
                //    nanochargbase = nanosuitcore.nanocharg.Value;
                //    nanodelaychargbase = nanosuitcore.nanochargdelay.Value;
                //}
                //else
                //{
                //    CommonSet();
                //    nanosuitcore.nanocharg.Value = 50;
                //    nanosuitcore.nanochargdelay.Value = 1;
                //    nanodelaychargbase = 1;
                //    nanochargbase = 50;
                //    isshengcun = false;
                //}
                //if (nanosuitcore.nanoup.Value != "推进器强化(Enhanced Propeller)")
                //{
                //    flybasecost = nanosuitcore.flycost.Value;
                //}
                //else
                //{
                //    CommonSet();
                //    nanosuitcore.flycost.Value = 5;
                //    flybasecost = 5;
                //    isshengcun = false;
                //}
                //if (nanosuitcore.nanoup.Value != "纳米视野强化(Enhanced Nanovision)")
                //{
                //    nanovisionbasecost = nanosuitcore.nanovisioncost.Value;
                //}
                //else
                //{
                //    CommonSet();
                //    nanosuitcore.nanovisioncost.Value = 2;
                //    nanovisionbasecost = 2;
                //    isshengcun = false;
                //}
                if (marktarget == null)
                {
                    marktarget = GameObject.Find("FPS Camera");//找到第一人称视角摄像机
                }
                if (nanoenergyhud == null)//生成能量条HUD
                {
                    var nanoenergyhudbase = Instantiate(energyhudPrefab, marktarget.transform.position, marktarget.transform.rotation);
                    nanoenergyhud = nanoenergyhudbase as GameObject;
                    nanoenergyhud.transform.parent = marktarget.transform;
                }
                if (ischarging)//开始充能
                {
                    if (isenergy == false)
                    {
                        energytimer += Time.deltaTime;
                        if (energytimer >= nanosuitcore.nanochargdelay.Value)
                        {
                            energytimer = 0;
                            isenergy = true;
                        }
                    }
                    if (isenergy)
                    {
                        maxenergy += Time.deltaTime * nanosuitcore.nanocharg.Value;
                        isplayaudio = false;
                        if (maxenergy >= 100)
                        {
                            maxenergy = 100;
                            ischarging = false;
                            isenergy = false;
                        }
                    }
                }
                if (Input.GetKeyDown(nanosuitcore.ARMOR.Value) && maxenergy > 0|| (!isfirstarmor && isautoarmor && nanosuitcore.nanoup.Value == "自动装甲(Auto Armor)"))//装甲模式启动音效和关闭音效
                {                  
                    CameraArmorFov();
                    isfirstarmor = true;
                    Destorynanohud();
                    SearchSpeedUpClose();
                    Nanovoiceset(armorvoice);
                    if (isautoarmor && nanosuitcore.nanoup.Value == "自动装甲(Auto Armor)") 
                    {
                        Nanovoiceset(7);
                    }
                    isautoarmor = false;
                    this.GetComponent<AudioSource>().Play();
                    newmaterial[0] = armormaterial;
                    newmaterial[1] = armormaterial02;
                    Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
                    armorvoice++;
                    stealthvoice = 2;
                    powervoice = 5;
                    speedvoice = 9;
                    if (armorvoice >= 2)//装甲模式是否关闭
                    {
                        armorvoice = 0;
                        if (armorhudani != null)
                        {
                            armorhudani[armorhudani.clip.name].time = armorhudani[armorhudani.clip.name].length;
                            armorhudani[armorhudani.clip.name].speed = -1;
                            armorhudani.Play(armorhudani.clip.name);
                        }
                        if (clockbase != null)
                        {
                            Destroy(clockbase);
                        }
                        newmaterial[0] = basematerial;
                        newmaterial[1] = basematerial02;
                        Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
                        if (gameWorld.MainPlayer.ActiveHealthController.DamageCoeff != 1f && Entermap())
                        {
                            gameWorld.MainPlayer.ActiveHealthController.SetDamageCoeff(1f);
                        }
                        CloseCameraClassEffects();
                        CloseCameraArmorFov();
                        isfirstarmor = false;
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
                    Destorynanohud();
                    CloseCameraArmorFov();
                    SearchSpeedUpClose();
                    Nanovoiceset(stealthvoice);
                    //this.GetComponent<AudioSource>().clip = audios[stealthvoice];
                    this.GetComponent<AudioSource>().Play();
                    newmaterial[0] = stealthmaterial;
                    newmaterial[1] = stealthmaterial;
                    Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
                    StartCoroutine("CloakCharacter");
                    stealthvoice++;
                    armorvoice = 0;
                    powervoice = 5;
                    speedvoice = 9;
                    if (stealthvoice >= 4)//隐身模式是否关闭
                    {
                        if (clockbase != null)
                        {
                            Destroy(clockbase);
                        }
                        stealthvoice = 2;
                        StartCoroutine("DeCloakCharacter");
                        isfirstarmor = false;
                        ischarging = true;
                        isstealth = false;
                        isarmor = false;
                        isenergy = false;
                        energytimer = 0;
                    }
                    nanomode = stealthvoice;
                }
                if (Input.GetKeyDown(nanosuitcore.POWER.Value) && maxenergy > 0)//力量模式启动音效和关闭音效
                {
                    isfirstarmor = false;
                    Destorynanohud();
                    CloseCameraArmorFov();
                    SearchSpeedUpClose();        
                    if (nanovisionhud == null)
                    {
                        var nanovisionhudbase = Instantiate(nanovisionhudPrefab, marktarget.transform.position, marktarget.transform.rotation);
                        nanovisionhud = nanovisionhudbase as GameObject;
                        MeshRenderer[] hudMeshRenderer = nanovisionhud.GetComponentsInChildren<MeshRenderer>();
                        foreach (MeshRenderer child in hudMeshRenderer)
                        {
                            hudMeshRenderer[0].materials[0].SetColor("_Edgecolor", Color.red);
                        }
                        nanovisionhud.transform.parent = marktarget.transform;
                    }
                    Nanovoiceset(powervoice);
                    //this.GetComponent<AudioSource>().clip = audios[powervoice];
                    this.GetComponent<AudioSource>().Play();
                    newmaterial[0] = powermaterial;
                    newmaterial[1] = powermaterial02;
                    Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
                    powervoice++;
                    armorvoice = 0;
                    stealthvoice = 2;
                    speedvoice = 9;
                    if (powervoice >= 7)//力量模式是否关闭
                    {
                        newmaterial[0] = basematerial;
                        newmaterial[1] = basematerial02;
                        Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
                        powervoice = 5;
                        ischarging = true;
                        isstealth = false;
                        isarmor = false;
                        ispower = false;
                        isenergy = false;
                        energytimer = 0;
                        Physics.gravity = new Vector3(0, -9.81f, 0);
                        if (nanovisionhud != null)
                        {
                            Destroy(nanovisionhud);
                        }
                        if (gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value != oldknifeskill)
                        {
                            gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value = oldknifeskill;
                        }
                    }
                    nanomode = powervoice;
                }
                if (Input.GetKeyDown(nanosuitcore.SPEED.Value) && maxenergy > 0)//速度模式启动音效和关闭音效
                {
                    isfirstarmor = false;                       
                    SearchSpeedUp();
                    Destorynanohud();
                    CloseCameraArmorFov();
                    if (nanovisionhud == null)
                    {
                        var nanovisionhudbase = Instantiate(nanovisionhudPrefab, marktarget.transform.position, marktarget.transform.rotation);
                        nanovisionhud = nanovisionhudbase as GameObject;
                        nanovisionhud.transform.parent = marktarget.transform;
                    }
                    Nanovoiceset(speedvoice);
                    //this.GetComponent<AudioSource>().clip = audios[powervoice];
                    this.GetComponent<AudioSource>().Play();
                    newmaterial[0] = speedmaterial;
                    newmaterial[1] = speedmaterial02;
                    Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
                    speedvoice++;
                    powervoice = 5;
                    armorvoice = 0;
                    stealthvoice = 2;
                     
                     
                    if (speedvoice >= 11)//速度模式是否关闭
                    {
                        SearchSpeedUpClose();
                        newmaterial[0] = basematerial;
                        newmaterial[1] = basematerial02;
                        Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
                        speedvoice = 9;
                        ischarging = true;
                        isspeed = false;
                        isstealth = false;
                        isarmor = false;
                        ispower = false;
                        isenergy = false;
                        energytimer = 0;
                        Physics.gravity = new Vector3(0, -9.81f, 0);
                        if (nanovisionhud != null)
                        {
                            Destroy(nanovisionhud);
                        }
                        if (gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value != oldknifeskill)
                        {
                            gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value = oldknifeskill;
                        }
                    }
                    nanomode = speedvoice;
                }
                if (Input.GetKeyDown(nanosuitcore.NANOVISION.Value) && maxenergy > 0)//纳米视野是否启动
                {
                    isnanovision = !isnanovision;
                    if (isnanovision)//判断现在是否纳米视野
                    {
                        Nanovoiceset(4);
                        //this.GetComponent<AudioSource>().clip = audios[4];
                        this.GetComponent<AudioSource>().Play();
                        Destorynanohud();
                        if (nanovisionhud == null)
                        {
                            var nanovisionhudbase = Instantiate(nanovisionhudPrefab, marktarget.transform.position, marktarget.transform.rotation);
                            nanovisionhud = nanovisionhudbase as GameObject;
                            MeshRenderer[] hudMeshRenderer = nanovisionhud.GetComponentsInChildren<MeshRenderer>();
                            foreach (MeshRenderer child in hudMeshRenderer)
                            {
                                hudMeshRenderer[0].materials[0].SetColor("_Edgecolor", Color.white);
                            }
                            nanovisionhud.transform.parent = marktarget.transform;
                        }
                        if (minimap == null)
                        {
                            var minimapbase = Instantiate(minimapPrefab);
                            minimap = minimapbase as GameObject;
                        }
                        if (Entermap() && gameWorld.AllPlayers.Count >= 2)
                        {
                            for (int i = 1; i < gameWorld.AllPlayers.Count; i++)//获取全部AI
                            {
                                //Console.WriteLine("读取坐标" + gameWorld.AllPlayers[i].Transform.position);
                                //Console.WriteLine(i);
                                AItarget[i] = Instantiate(nanotargetPrefab, gameWorld.AllPlayers[i].Transform.position, gameWorld.AllPlayers[i].Transform.rotation) as GameObject;
                            }
                        }
                    }
                    else
                    {
                        Destorynanohud();
                        if (AItarget != null)
                        {
                            for (int i = 1; i < 100; i++)//获取全部AI
                            {
                                Destroy(AItarget[i]);
                            }
                        }
                        if (minimap != null)
                        {
                            Destroy(minimap);
                        }
                        if (!isstealth || !isarmor || !isfly || !ispower)
                        {
                            ischarging = true;
                            energytimer = 0;
                        }
                    }
                }
                if (nanomode == 1 && !isenergyempty)//判断现在是否启动装甲模式
                {
                    if (clockbase != null)
                    {
                        Destroy(clockbase);
                    }
                    if (nanoarmorhud == null)
                    {
                        var nanoarmorhudbase = Instantiate(armorhudPrefab, marktarget.transform.position, marktarget.transform.rotation);
                        nanoarmorhud = nanoarmorhudbase as GameObject;
                        armorhudani = nanoarmorhud.GetComponent<Animation>();
                    }
                    if (armorhudani != null)
                    {
                        armorhudani[armorhudani.clip.name].time = 0;
                        armorhudani[armorhudani.clip.name].speed = 1;
                        armorhudani.Play(armorhudani.clip.name);                       
                        RawImage[] armorhudMeshRenderer = nanoarmorhud.GetComponentsInChildren<RawImage>();
                        foreach (RawImage child in armorhudMeshRenderer)
                        {
                            ArmorColor.a = nanosuitcore.ArmorhudAlpha.Value;
                            armorhudMeshRenderer[0].color = ArmorColor;
                        }
                    }                                     
                    if (CameraClass.Instance != null)
                    {
                        if (CameraClass.Instance.EffectsController != null && CameraClass.Instance.EffectsController.enabled)
                        {
                            CameraClass.Instance.EffectsController.enabled = false;
                        }
                    }
                    if (gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value != oldknifeskill)
                    {
                        gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value = oldknifeskill;
                    }
                    Physics.gravity = new Vector3(0, -9.81f, 0);
                    Powermodeclose();
                    nanomode = 0;
                    isstealth = false;
                    isarmor = true;
                    ispower = false;
                    isspeed = false;
                }
                if (nanomode == 3 && !isenergyempty)//判断现在是否启动隐身模式
                {
                    if (nanoarmorhud != null)
                    {
                        Destroy(nanoarmorhud);
                    }
                    if (clockbase != null)
                    {
                        Destroy(clockbase);
                    }
                    var nanoarmorbase = Instantiate(stealthPrefab, gameObject.transform.position, transform.rotation);
                    clockbase = nanoarmorbase as GameObject;
                    clockbase.transform.parent = this.transform;
                    if (gameWorld.MainPlayer.ActiveHealthController.DamageCoeff != 1f && Entermap())
                    {
                        gameWorld.MainPlayer.ActiveHealthController.SetDamageCoeff(1f);
                    }
                    CloseCameraClassEffects();
                    if (gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value != oldknifeskill)
                    {
                        gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value = oldknifeskill;
                    }
                    Physics.gravity = new Vector3(0, -9.81f, 0);
                    Powermodeclose();
                    isstealth = true;
                    isarmor = false;
                    ispower = false;
                    isspeed = false;
                    nanomode = 0;
                }
                if (nanomode == 6 && !isenergyempty)//判断现在是否启动力量模式
                {         
                    if (nanoarmorhud != null)
                    {
                        Destroy(nanoarmorhud);
                    }
                    if (clockbase != null)
                    {
                        Destroy(clockbase);
                    }
                    if (gameWorld.MainPlayer.ActiveHealthController.DamageCoeff != 1f && Entermap())
                    {
                        gameWorld.MainPlayer.ActiveHealthController.SetDamageCoeff(1f);
                    }
                    CloseCameraClassEffects();
                    CloseCameraArmorFov();
                    oldknifeskill = gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value;
                    if (gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value != 1337f)
                    {
                        gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value = 1337f;  
                    }                    
                    isstealth = false;
                    isarmor = false;
                    isspeed = false;
                    ispower = true;
                    nanomode = 0;
                }
                if (nanomode == 10 && !isenergyempty)//判断现在是否启动速度模式
                {
                    if (nanoarmorhud != null)
                    {
                        Destroy(nanoarmorhud);
                    }
                    if (clockbase != null)
                    {
                        Destroy(clockbase);
                    }
                    if (gameWorld.MainPlayer.ActiveHealthController.DamageCoeff != 1f && Entermap())
                    {
                        gameWorld.MainPlayer.ActiveHealthController.SetDamageCoeff(1f);
                    }
                    if (gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value != oldknifeskill)
                    {
                        gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value = oldknifeskill;
                    }                   
                    CloseCameraClassEffects();
                    CloseCameraArmorFov();
                    Powermodeclose();
                    isstealth = false;
                    isarmor = false;
                    ispower = false;
                    isspeed = true;
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
                    if (!isstealth || !isarmor || !isnanovision || !ispower)
                    {
                        ischarging = true;
                        energytimer = 0;
                    }
                }
                if (isenergyempty)//判断能量是否耗尽
                {
                    if (!isplayaudio)
                    {
                        nanoenergyhud.GetComponent<AudioSource>().volume = nanosuitcore.nanovolume.Value;
                        nanoenergyhud.GetComponent<AudioSource>().Play();
                        isplayaudio = true;
                    }
                    maxenergy = 0;
                    armorvoice = 0;
                    stealthvoice = 2;
                    powervoice = 5;
                    speedvoice = 9;
                    newmaterial[0] = basematerial;
                    newmaterial[1] = basematerial02;
                    Physics.gravity = new Vector3(0, -9.81f, 0);
                    Powermodeclose();
                    if (gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value != oldknifeskill)
                    {
                        gameWorld.MainPlayer.Skills.StrengthBuffMeleePowerInc.Value = oldknifeskill;
                    }
                    Hand.GetComponent<SkinnedMeshRenderer>().materials = newmaterial;
                    if (clockbase != null)
                    {
                        Destroy(clockbase);
                    }
                    if (nanoarmorhud != null)
                    {
                        Destroy(nanoarmorhud);
                    }
                    if (flymode != null)
                    {
                        Destroy(flymode);
                    }
                    if (nanovisionhud != null)
                    {
                        Destroy(nanovisionhud);
                    }
                    if (minimap != null)
                    {
                        Destroy(minimap);
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
                    if (gameWorld.MainPlayer.ActiveHealthController.DamageCoeff != 1f && Entermap())
                    {
                        gameWorld.MainPlayer.ActiveHealthController.SetDamageCoeff(1f);
                    }
                    CloseCameraClassEffects();
                    CloseCameraArmorFov();
                    isfly = false;
                    isclick = false;
                    isstealth = false;
                    isarmor = false;
                    ispower = false;
                    isspeed = false;
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
                if (isarmor)//装甲模式参数设置
                {
                    maxenergy -= Time.deltaTime * nanosuitcore.armorcost.Value;
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
                    if (nanoarmorhud != null && youarehit)
                    {
                        nanoarmorhud.GetComponent<AudioSource>().volume = nanosuitcore.nanovolume.Value;
                        nanoarmorhud.GetComponent<AudioSource>().Play();
                        youarehit = false;
                    }
                    //if (gameWorld.MainPlayer.ActiveHealthController.DamageCoeff != -1f)
                    //{
                    //    gameWorld.MainPlayer.ActiveHealthController.SetDamageCoeff(-1f);
                    //}
                    if (gameWorld.MainPlayer.Physical.Stamina.Current < 75)
                    {
                        gameWorld.MainPlayer.Physical.Stamina.Current = gameWorld.MainPlayer.Physical.Stamina.TotalCapacity.Value;
                    }
                }
                if (isstealth)//隐身模式能量自然消耗
                {
                    cloakcost();
                    ischarging = false;
                    if (maxenergy <= 0)
                    {
                        maxenergy = 0;
                        energytimer = 0;
                        isenergyempty = true;
                    }
                    if (gameWorld.MainPlayer.ActiveHealthController.DamageCoeff != 1f && Entermap())
                    {
                        gameWorld.MainPlayer.ActiveHealthController.SetDamageCoeff(1f);
                    }
                    CloseCameraClassEffects();
                }                
                if(ispower)//力量模式设置
                {
                    ischarging = false;
                    maxenergy -= Time.deltaTime * nanosuitcore.powercost.Value;
                    Powermode();
                    if (maxenergy <= 0)
                    {
                        maxenergy = 0;
                        energytimer = 0;
                        isenergyempty = true;
                    } 
                }
                if (isspeed)//速度模式设置
                {
                    ischarging = false;
                    maxenergy -= Time.deltaTime * nanosuitcore.speedcost.Value;
                    MoveSpeedUp();
                    //if (speedupbase != 1.2f)
                    //{
                    //    speedupbase = 1.2f;
                    //}
                    if (gameWorld.MainPlayer.IsSprintEnabled)
                    {
                        maxenergy -= Time.deltaTime * nanosuitcore.speedcostrun.Value;
                        //if(speedupbase!=2f)
                        //{
                        //    speedupbase = 2;
                        //}
                    }
                    if (maxenergy <= 0)
                    {
                        maxenergy = 0;
                        energytimer = 0;
                        isenergyempty = true;
                    }
                }
                if (isfly)//飞行模式能量自然消耗
                {
                    maxenergy -= Time.deltaTime * nanosuitcore.flycost.Value;
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
                    maxenergy -= Time.deltaTime * nanosuitcore.nanovisioncost.Value;
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
        private void Nanovoiceset(int x)
        {
            if (nanosuitcore.nanovoice.Value == "二代男声(V2.0male)") { this.GetComponent<AudioSource>().clip = audios[x]; }
            if (nanosuitcore.nanovoice.Value == "一代男声(V1.0male)") { this.GetComponent<AudioSource>().clip = audiosman[x]; }
            if (nanosuitcore.nanovoice.Value == "一代女声(V1.0female)") { this.GetComponent<AudioSource>().clip = audioswoman[x]; }
        }
            private void energywaring()
        {
            if (maxenergy<=20 && !isfirstlowpower)
            {
                islowpower = true;
                isfirstlowpower = true;
            }
            if (maxenergy > 20 && isfirstlowpower)
            {
                isfirstlowpower = false;
            }
            if (islowpower)
            {
                Nanovoiceset(8);
                this.GetComponent<AudioSource>().Play();
                islowpower = false;
            }
        }
        private void cloakcost()
        {
            if (Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W))
            {
                maxenergy -= Time.deltaTime * nanosuitcore.stealthcostmove.Value;
            }
            if (!Input.GetKey(KeyCode.A)&& !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W))
            {
                maxenergy -= Time.deltaTime * nanosuitcore.stealthcost.Value;
            }
        }
        private void CloseCameraClassEffects()
        {
            if (CameraClass.Instance != null)
            {
                if (CameraClass.Instance.EffectsController != null && !CameraClass.Instance.EffectsController.enabled)
                {
                    CameraClass.Instance.EffectsController.enabled = true;
                }
            }
        }
        private void CameraArmorFov()
        {
            isarmorfov = true;
            SetFov = NowFov - 10f;
            CameraClass.Instance.SetFov(SetFov, 1.5f, false);
        }
        private void CloseCameraArmorFov()
        {
            if (isarmorfov)
            {
                //SetFov = NowFov + 10f;
                CameraClass.Instance.SetFov(BaseFov, 1.5f, false);
                isarmorfov = false;
            }    
        }
        private void killMemu()
        {
            if (Input.GetMouseButtonDown(2))
            {
                if (gameWorld.AllPlayers.Count >= 2)
                {
                    for (int i = 1; i < gameWorld.AllPlayers.Count; i++)//获取全部AI
                    {
                        gameWorld.AllPlayers[i].KillMe(EBodyPart.Head, 1000f); 
                    }
                }
            }           
        }
        private void testmode()
        {
            if (Input.GetMouseButtonDown(2))
            {
                Console.WriteLine(gameWorld.MainPlayer.IsSprintEnabled);
            }

        }
        private void FastMemu()
        {
            if (Input.GetMouseButtonDown(2))
            {
                if (fastmemu==null)
                {
                    Console.WriteLine(Cursor.visible);
                    
                    Console.WriteLine(Cursor.lockState);      
                    var fastmemubase = Instantiate(fastmemuPrefab, marktarget.transform.position, marktarget.transform.rotation);
                    fastmemu = fastmemubase as GameObject;
                }
            }
            if (Input.GetMouseButton(2))
            {
                LockCursor = false;
            }
            if (Input.GetMouseButtonUp(2))
            {
                if (fastmemu != null)
                {
                    Console.WriteLine(Cursor.lockState);
                    Console.WriteLine(Cursor.visible);
                    Cursor.lockState = CursorLockMode.Locked;
                    Destroy(fastmemu);
                }
            }
        }
        #region 锁定/隐藏鼠标
        public static bool LockCursor
        {
            get => Cursor.lockState == CursorLockMode.Locked;
            set
            {
                Cursor.visible = true;
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.Confined;
            }
        }
        #endregion
        private void Destorynanohud()
        {
            if (nanovisionhud != null)
            {
                Destroy(nanovisionhud);
            }
        }
        private void CommonSet()
        {
            nanosuitcore.nanocharg.Value = 25;
            nanosuitcore.nanochargdelay.Value = 2;
            nanosuitcore.armorcost.Value = 2;
            nanosuitcore.armordefenselow.Value = 2;
            nanosuitcore.armordefensehigh.Value = 10;
            nanosuitcore.stealthcost.Value = 5;
            nanosuitcore.flycost.Value = 15;
            nanosuitcore.nanovisioncost.Value = 5;
            nanosuitcore.FixIntensity.Value = 0.1f;
            nanosuitcore.FixDelay.Value = 10;
        }
        private void Powermode()
        {
            if (gameWorld.MainPlayer.ProceduralWeaponAnimation.Shootingg.Intensity != nanosuitcore.powerweapon.Value)
            {
                gameWorld.MainPlayer.ProceduralWeaponAnimation.Shootingg.Intensity = nanosuitcore.powerweapon.Value;
            }
            if (gameWorld.MainPlayer.ProceduralWeaponAnimation.Shootingg.Stiffness != nanosuitcore.powerweapon.Value)
            {
                gameWorld.MainPlayer.ProceduralWeaponAnimation.Shootingg.Stiffness = nanosuitcore.powerweapon.Value;
            }
            if (gameWorld.MainPlayer.ProceduralWeaponAnimation.Breath.Intensity != nanosuitcore.powerweapon.Value)
            {
                gameWorld.MainPlayer.ProceduralWeaponAnimation.Breath.Intensity = nanosuitcore.powerweapon.Value;
            }
            if (gameWorld.MainPlayer.ProceduralWeaponAnimation.MotionReact.Intensity != nanosuitcore.powerweapon.Value)
            {
                gameWorld.MainPlayer.ProceduralWeaponAnimation.MotionReact.Intensity = nanosuitcore.powerweapon.Value;
            }
            if (gameWorld.MainPlayer.ProceduralWeaponAnimation.ForceReact.Intensity != nanosuitcore.powerweapon.Value)
            {
                gameWorld.MainPlayer.ProceduralWeaponAnimation.ForceReact.Intensity = nanosuitcore.powerweapon.Value;
            }
        }
        private void Powermodeclose()
        {
            if (gameWorld.MainPlayer.ProceduralWeaponAnimation.Shootingg.Intensity != 1f)
            {
                gameWorld.MainPlayer.ProceduralWeaponAnimation.Shootingg.Intensity = 1f;
            }
            if (gameWorld.MainPlayer.ProceduralWeaponAnimation.Shootingg.Stiffness != 1f)
            {
                gameWorld.MainPlayer.ProceduralWeaponAnimation.Shootingg.Stiffness = 1f;
            }
            if (gameWorld.MainPlayer.ProceduralWeaponAnimation.Breath.Intensity != 1f)
            {
                gameWorld.MainPlayer.ProceduralWeaponAnimation.Breath.Intensity = 1f;

            }
            if (gameWorld.MainPlayer.ProceduralWeaponAnimation.MotionReact.Intensity != 1f)
            {
                gameWorld.MainPlayer.ProceduralWeaponAnimation.MotionReact.Intensity = 1f;
            }
            if (gameWorld.MainPlayer.ProceduralWeaponAnimation.ForceReact.Intensity != 1f)
            {
                gameWorld.MainPlayer.ProceduralWeaponAnimation.ForceReact.Intensity = 1f;
            }
        }
        private void MoveSpeedUp()
        {
            gameWorld.MainPlayer.RemoveStateSpeedLimit(Player.ESpeedLimit.Armor);
            gameWorld.MainPlayer.RemoveStateSpeedLimit(Player.ESpeedLimit.Aiming);
            gameWorld.MainPlayer.RemoveStateSpeedLimit(Player.ESpeedLimit.BarbedWire);
            gameWorld.MainPlayer.RemoveStateSpeedLimit(Player.ESpeedLimit.Fall);
            gameWorld.MainPlayer.RemoveStateSpeedLimit(Player.ESpeedLimit.HealthCondition);
            gameWorld.MainPlayer.RemoveStateSpeedLimit(Player.ESpeedLimit.Shot);
            gameWorld.MainPlayer.RemoveStateSpeedLimit(Player.ESpeedLimit.SurfaceNormal);
            gameWorld.MainPlayer.RemoveStateSpeedLimit(Player.ESpeedLimit.Swamp);
            gameWorld.MainPlayer.RemoveStateSpeedLimit(Player.ESpeedLimit.Weight);
        }
        private void SearchSpeedUp()
        {
            if (gameWorld.MainPlayer.Skills.AttentionEliteExtraLootExp.Value != true)
            {
                gameWorld.MainPlayer.Skills.AttentionEliteExtraLootExp.Value = true;

            }           
            if (gameWorld.MainPlayer.Skills.IntellectEliteContainerScope.Value != true)
            {
                gameWorld.MainPlayer.Skills.IntellectEliteContainerScope.Value = true;
            }
            if (gameWorld.MainPlayer.Skills.AttentionEliteLuckySearch.Value != 100f)
            {
                gameWorld.MainPlayer.Skills.AttentionEliteLuckySearch.Value = 100f;

            }
        }
        private void SearchSpeedUpClose()
        {
            if (gameWorld.MainPlayer.Skills.AttentionEliteLuckySearch.Value != 0.2f)
            {
                gameWorld.MainPlayer.Skills.AttentionEliteLuckySearch.Value = 0.2f;
            }
        }
        void LateUpdate()
        {
            if(isarmor)
            {
                if (gameWorld.MainPlayer.ActiveHealthController.DamageCoeff != -1f)
                {
                    gameWorld.MainPlayer.ActiveHealthController.SetDamageCoeff(-1f);
                }
            }   
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
            float ArmorColorhit = 1f;
            float ArmorFade = 0f;
            ArmorColor.r = ArmorFade;
            ArmorColor.g = ArmorFade;
            ArmorColor.b = 1;
            if (nanoarmorhud != null)
            {
                RawImage[] armorhudMeshRenderer = nanoarmorhud.GetComponentsInChildren<RawImage>();
                foreach (RawImage child in armorhudMeshRenderer)
                {
                    armorhudMeshRenderer[0].color = ArmorColor;
                    while (ArmorFade < 1f && ArmorColorhit > nanosuitcore.ArmorhudAlpha.Value)
                    {
                        ArmorFade += 0.3f;
                        if (ArmorFade >= 1)
                        {
                            ArmorFade = 1;
                        }
                        ArmorColorhit -= 0.1f;
                        if (ArmorColorhit <= nanosuitcore.ArmorhudAlpha.Value)
                        {
                            ArmorColorhit = nanosuitcore.ArmorhudAlpha.Value;
                        }
                        ArmorColor.g = ArmorFade;
                        ArmorColor.r = ArmorFade;
                        ArmorColor.a = ArmorColorhit;
                        armorhudMeshRenderer[0].color = ArmorColor;
                        yield return 0;
                    }
                }
            }
        }
        IEnumerator ArmorHitdanger()
        {
            float ArmorColorhit = 1f;
            float ArmorFade = 0f;
            ArmorColor.r = 1;
            ArmorColor.g = ArmorFade;
            ArmorColor.b = ArmorFade;
            if (nanoarmorhud != null)
            {
                RawImage[] armorhudMeshRenderer = nanoarmorhud.GetComponentsInChildren<RawImage>();
                foreach (RawImage child in armorhudMeshRenderer)
                {
                    armorhudMeshRenderer[0].color = ArmorColor;
                    while (ArmorFade < 1f&& ArmorColorhit > nanosuitcore.ArmorhudAlpha.Value)
                    {
                        ArmorFade += 0.3f;
                        if (ArmorFade >= 1)
                        {
                            ArmorFade = 1;
                        }
                        ArmorColorhit -= 0.05f;
                        if (ArmorColorhit <= nanosuitcore.ArmorhudAlpha.Value)
                        {
                            ArmorColorhit = nanosuitcore.ArmorhudAlpha.Value;
                        }
                        ArmorColor.g = ArmorFade;
                        ArmorColor.b = ArmorFade;
                        ArmorColor.a = ArmorColorhit;
                        armorhudMeshRenderer[0].color = ArmorColor;
                        yield return 0;
                    }
                }
            }
        }      
    }
    

    public class NanosuitPatch : ModulePatch
    {
        //搜索EFT空间下的Player类里的ReceiveDamage
        protected override MethodBase GetTargetMethod() => typeof(Player).GetMethod("ReceiveDamage", nanosuitcore.BFlags);

        [PatchPostfix]
        static void PostFix(ref Player __instance, EDamageType type)
        {
            if (__instance.IsYourPlayer && (type == EDamageType.Bullet || type == EDamageType.Explosion || type == EDamageType.Sniper || type == EDamageType.Landmine || type == EDamageType.GrenadeFragment || type == EDamageType.Barbed || type == EDamageType.Fall))
            {                
                if (!nanosuit.isarmor)
                {
                    nanosuit.fixdelaytime = 0;
                    nanosuit.startfixdebuff = true;
                    nanosuit.iscureyourself = false;
                } //350是2091；351-353是2100；355-356是2103，357是2102
                if (__instance.ActiveHealthController.BodyPartEffects.Effects[0].Any(v => v.Key == "PainKiller"))
                {
                    ActiveHealthControllerClass.GClass2102 nanoPainKiller = typeof(ActiveHealthControllerClass).GetMethod("FindActiveEffect", BindingFlags.Instance | BindingFlags.Public).MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("PainKiller", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head }) as ActiveHealthControllerClass.GClass2102;
                    if (nanoPainKiller.TimeLeft < 60) nanoPainKiller.AddWorkTime(60f, false);
                    return;
                }

                MethodInfo method = typeof(ActiveHealthControllerClass).GetMethod("method_15", BindingFlags.Instance | BindingFlags.NonPublic);
                method.MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("PainKiller", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head, 0f, 60f, 0f, 1f, null });               
            }
        }
    }
    public class ArmormodePatch : ModulePatch
    {
        //搜索EFT空间下的Player类里的ReceiveDamage
        protected override MethodBase GetTargetMethod() => typeof(Player).GetMethod("ReceiveDamage", nanosuitcore.BFlags);

        [PatchPostfix]
        static void PostFix(ref Player __instance, EDamageType type)
        {
            if (__instance.IsYourPlayer && (type == EDamageType.Bullet || type == EDamageType.GrenadeFragment || type == EDamageType.Melee) && nanosuit.isarmor)
            {
                nanosuit.isarmorhit = true;
                nanosuit.youarehit = true;
                nanosuit.maxenergy = nanosuit.maxenergy - nanosuitcore.armordefenselow.Value;
            }
            if (__instance.IsYourPlayer && (type == EDamageType.Explosion || type == EDamageType.Sniper || type == EDamageType.Landmine || type == EDamageType.Fall) && nanosuit.isarmor)
            {
                nanosuit.isarmorhitdanger = true;
                nanosuit.youarehit = true;
                nanosuit.maxenergy = nanosuit.maxenergy - nanosuitcore.armordefensehigh.Value;
            }
        }
    }
    public class BoyNextDoor : ModulePatch
    {
        //踹门(Fuck the door)
        protected override MethodBase GetTargetMethod() => typeof(Door).GetMethod("BreachSuccessRoll", nanosuitcore.BFlags);
        [PatchPostfix]
        static void PostFix(ref bool __result)
        {
            if(nanosuit.ispower)
            {
                __result = true;
                nanosuit.maxenergy -= nanosuitcore.fuckdoorcost.Value;
            }    
        }
    }
    public class FastGrenade : ModulePatch //快速丢雷(QuickGrenade)
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethods().First(m =>m.Name == "SetInHands" && m.GetParameters()[0].Name == "throwWeap");
        }

        [PatchPrefix]
        static void Prefix(Player __instance, GrenadeClass throwWeap, Callback<GInterface116> callback)
        {
            if(__instance.IsYourPlayer&&nanosuit.isspeed)
            {
                __instance.SetInHandsForQuickUse(throwWeap, null);
                nanosuit.maxenergy -= nanosuitcore.quickgrenadecost.Value;
            }  
        }
    }
    public class FastWeapon : ModulePatch //武器加速(weapon speed up)
    {
        protected override MethodBase GetTargetMethod() => typeof(ObjectInHandsAnimator).GetMethod("SetAnimationSpeed", nanosuitcore.BFlags);
        [PatchPrefix]
        static void Prefix(object __instance, ref float speed)
        {
            if (__instance!=null && NanoGUI.isweapon && nanosuit.isspeed)
            {
                speed *= nanosuitcore.speedratio.Value;
            }
        }
    }
    public class speedupcore : ModulePatch
    {
        public static Vector3 _motion;

        protected override MethodBase GetTargetMethod() => typeof(SimpleCharacterController).GetMethod("Move", nanosuitcore.BFlags);

        [PatchPrefix]
        static void Prefix(SimpleCharacterController __instance, Vector3 ___vector3_1, Vector3 motion, float deltaTime)
        {

            Player p = __instance.GetComponentInParent<Player>();
            if (p != null && p.IsYourPlayer)
            {
                _motion = motion;
            }
        }
    }

    public class speedmode : ModulePatch
    {
        protected override MethodBase GetTargetMethod() => typeof(SimpleCharacterController).GetMethod("method_18", nanosuitcore.BFlags);

        [PatchPrefix]
        static bool Prefix(SimpleCharacterController __instance, ref Vector3 __result, Vector3 point)
        {
            Player p = __instance.GetComponentInParent<Player>();
            if (p != null && p.IsYourPlayer)
            {
                if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && nanosuit.isspeed)
                {
                    var tmp_x = speedupcore._motion.x * nanosuitcore.speedupbase.Value;
                    var tmp_y = 0.02f;
                    var tmp_z = speedupcore._motion.z * nanosuitcore.speedupbase.Value;
                    __result = point + new Vector3(tmp_x, tmp_y, tmp_z);
                    return false;
                }
            }
            return true;
        }
    }
    public class speeduplink : ModulePatch
    {
        protected override MethodBase GetTargetMethod() => typeof(SimpleCharacterController).GetMethod("method_1", nanosuitcore.BFlags);

        [PatchPrefix]
        static bool Prefix(SimpleCharacterController __instance, Vector3 startPosition, float deltaTime)
        {

            Player p = __instance.GetComponentInParent<Player>();
            if (p != null && p.IsYourPlayer)
            {
                if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && nanosuit.isspeed)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
