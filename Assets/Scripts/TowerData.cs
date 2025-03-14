using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Tower Defense/Tower Data", order = 1)]
public class TowerData : ScriptableObject
{
    public int rankValue;  
    public Sprite towerSprite;          
    public float attackRange;         
    public float attackDamage;       
}
