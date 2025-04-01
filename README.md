# April 1， 2025
merge了Wenqiao-Wang分支上tutScene的内容，将文件统一放到了Assets/Script目录下  
添加了enemySpawn的个性化配置，在勾选 enableCustomConfig后，可以对每一波进行单独配置，包括：
1. enemyPrefab
2. enemyHealth
3. enemySpeed
4. enemyCoin
5. timeBetweenEnemy（一波内敌人中间的生成间隔）
6. timeAfterWave（波与波之间的时间）
7. enemyCount


# Enemy Instruction
## TODO:<br>
### about ranking
排行榜数据我写在了gameOver里面，获取并更新前三名的数据(按存活时间排序)，包括：姓名(默认Steve)，得分，存活时间
## EnemySpawn.cs
public GameObject enemyPrefab; --> 选择enemy的prefeb<br>
public void EnemySpawnConfigInit(); --> 初始化配置（主要是寻路）<br>
public IEnumerator SpawnWaves(); --> 协程调用，开始无限生成enemy<br>

### New Features:<br>
1. 生成“炮车”, 血量为1.8倍，移速0.8倍，金币2倍，在一波怪最后出现，数量不超过当前波次出怪数的1/3。<br>
2. 优化出怪间隔，使怪物密度增加更平滑。
<br>

## Enemy.cs<br>
### !!! bug: 我在做数据统计时发现了一个关于energy tower的问题，因为它每一帧造成伤害，导致会重复调用AddScore使分数虚高，但解决重复调用问题后UI出现问题，多个energy tower时会有一个tower的能量柱在Enemy死亡后不消失(为了解决不同类型伤害统计问题，我稍微改了下energy tower的文件)<br>
<br>
public EnemyData enemyData; --> 连接enemyData<br>
public UIManager uiManager; --> 连接UIManager(其实对UIManager进行单例设置更佳)<br>
public Transform[] waypoints; --> 寻路点数组<br>
(deprecate) public int index; --> Enemy生成序号<br>
(new) public float distance = 0f; --> Enemy到达终点的距离<br>
public bool IsAlive { get; protected set; } = true; --> Enemy存活bool值<br>
<br>
(old) public void EnemyTakeDamage(float damage); --> enemy收到伤害<br>
(new) public void EnemyTakeDamage(float damage, string type = "gun"); --> enemy收到伤害及其类型(现有burning, slow, energy, gun 4种)<br>
(abandon) public void SetMaxHealth(float maxHealth); --> 设置enemy血量<br>
(new) public void InitiateEnemy(Transform[] waypointList, float health, float speed, int c) --> 通过路径，血量，移速，金币价值初始化Enemy<br>
public void EnemySlowEffect(float slowRatio, float duration); --> 施加减速状态(speed = speed*slowRatio)(每秒20伤害)<br>
public void EnemyBurnEffect(float damagePerSecond, float duration) --> 施加灼烧状态<br>
<br>

# Firebase Instruction
由于firebase太大，无法上传github，如果想要运行的话，需要2个包，FirebaseInstallations.unitypackage和FirebaseDatabase.unitypackage<br>
这两个包可以在以下链接下载：<br>
https://drive.google.com/file/d/13FeupaGOvV4ZzlCxvd8tTqoBjjIX55Hc/view?usp=drive_link<br>
https://drive.google.com/file/d/1_la_YD2ORGmHQ2bUNyGCDyNrXA4spCzb/view?usp=drive_link<br>
下载后在unity中Asset导入Custom Package即可使用
同时需要再下载: install -> Unity 6 -> Add Module -> IOS build
