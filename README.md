# Enemy Instruction
### EnemySpawn.cs
public GameObject enemyPrefab; --> 选择enemy的prefeb<br>
public void EnemySpawnConfigInit(); --> 初始化配置（主要是寻路）<br>
public IEnumerator SpawnWaves(); --> 协程调用，开始无限生成enemy<br>

### Enemy.cs
public void EnemyTakeDamage(float damage); --> enemy收到伤害<br>
public void SetMaxHealth(float maxHealth); --> 设置enemy血量<br>
public void EnemySlowEffect(float slowRatio, float duration); --> 施加减速状态(speed = speed*slowRatio)<br>
public void EnemyBurnEffect(float damagePerSecond, float duration) --> 施加灼烧状态<br>
# Firebase Instruction
由于firebase太大，无法上传github，如果想要运行的话，需要2个包，FirebaseInstallations.unitypackage和FirebaseDatabase.unitypackage<br>
这两个包可以在以下链接下载：<br>
https://drive.google.com/file/d/13FeupaGOvV4ZzlCxvd8tTqoBjjIX55Hc/view?usp=drive_link<br>
https://drive.google.com/file/d/1_la_YD2ORGmHQ2bUNyGCDyNrXA4spCzb/view?usp=drive_link<br>
下载后在unity中Asset导入Custom Package即可使用