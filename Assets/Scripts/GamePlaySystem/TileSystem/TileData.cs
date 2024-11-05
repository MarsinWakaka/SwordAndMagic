using UnityEngine;
using UnityEngine.Serialization;

namespace Entity
{
    [CreateAssetMenu(fileName = "new TileData", menuName = "新建瓦片数据", order = 0)]
    public class TileData : ScriptableObject
    {
        public TileType tileType;   // 瓦片类型
        public string typeName;     // 瓦片名称
        public Sprite tileSprite;   // 瓦片贴图
        public string description;  // 瓦片描述
        public bool canBlockMove;   // 是否阻挡移动
        public bool canBlockAtk;    // 是否阻挡攻击
        public bool isAttackable;   // 是否可被攻击
        [FormerlySerializedAs("moveCost")] public int leaveCost = 1;        // 移动消耗
        public int durability;      // 耐久度 
        [Tooltip("此瓦片的推荐度（针对AI）")]
        public int score;           // 
    }
}