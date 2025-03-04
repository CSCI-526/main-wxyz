using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Tower Defense/Enemy Data", order = 1)]
public class EnemyData : ScriptableObject
{   
    public Sprite enemySprite;
    public float maxHealth;
    public float moveSpeed;
    
    //Enemy damage to base
    public float damage;


}
