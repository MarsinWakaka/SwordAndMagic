using System.Collections;
using BattleSystem;
using Obsolete.DeprecatedBattleSystem;
using UnityEngine;
using Utility;
using Utility.Singleton;
using Random = UnityEngine.Random;

namespace DeprecatedBattleSystem
{
    public enum RacialType
    {
        Unknown,
        Player,
        Enemy
    }
    
    [RequireComponent(typeof(SpriteRenderer))]
    public class BattleUnit : MonoBehaviour
    {
        // 自身组件
        Transform _transform;
        SpriteRenderer _spriteRenderer;
        // 外部组件
        private BattleUnitStyle _battleUnitStyle;
        private BoxCollider2D _boxCol2D;
        private BattleManager _battleManager;

        public GameObject onTurnFXGo;

        #region 基本属性

        [Header("角色基于信息")]
        public RacialType racialType = RacialType.Unknown;
        [TextArea(2, 6)] public string info;
        [Header("最大生命值")]
        public int health;
        [Header("战斗属性")]
        public int minAttack;
        public int maxAttack;
        [Tooltip("每回合最大攻击次数")] public int maxAttackCount;
        [Tooltip("最大攻击范围")] public int attackRange;
        [Header("防御属性")]
        [Tooltip("每次轮到该单位时，刷新的防御值")] public int maxDefense;
        [Header("行动属性")]
        [Tooltip("每回合最大移动范围")] public int maxMoveRange;
        
        #endregion

        #region 音效与样式

        [Header("样式")]
        [Tooltip("攻击模式下选中敌人时的鼠标样式")] public Texture2D attackIcon;
        [SerializeField] private Color mouseEnterColorMultiplier = Color.white;
        private Color _originalColor;
        private Color _mouseEnterColor; // = _originalColor * mouseEnterColorMultiplier;
        
        [Header("音效")]
        [SerializeField] private AudioClip attackClip;
        [SerializeField] private AudioClip moveClip;
        [SerializeField] private AudioClip attackOnBodyClip;
        [SerializeField] private AudioClip attackOnShieldClip;

        #endregion
        
        // 点子 受否可被观察
        // 隐藏属性
        [HideInInspector] public string unitName;
        [HideInInspector] public Sprite sprite;
        [HideInInspector] public int curMoveRange;
        [HideInInspector] public int curAttackCount;
        [HideInInspector] public int curDefense;

        private void Start()
        {
            _transform = transform;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            sprite = _spriteRenderer.sprite;
            _battleUnitStyle = GetComponent<BattleUnitStyle>();
            _boxCol2D = GetComponent<BoxCollider2D>();
            
            _battleManager = FindFirstObjectByType<BattleManager>();
            unitName = gameObject.name;
            
            _originalColor = _spriteRenderer.color;
            _mouseEnterColor = _originalColor * mouseEnterColorMultiplier;
            
            InitAction();
        }

        /// 刷新各项点数
        private void InitAction()
        {
            curMoveRange = maxMoveRange;
            curAttackCount = maxAttackCount;
            curDefense = maxDefense;
            _battleUnitStyle.UpdateUI(this);
        }

        public void OnTurnEnter()
        {
            // 初始化行动点数
            InitAction();
            _battleManager.RefreshMoveRange(transform.position, curMoveRange);
            // 显示选中图标
            onTurnFXGo.SetActive(true);
        }

        public void OnTurnExit()
        {
            onTurnFXGo.SetActive(false);
        }
        
        private void OnMouseDown()
        {
            _battleManager.NotifyUnitClicked(this);
        }

        private void OnMouseEnter()
        {
            _battleManager.NotifyUnitMouseEnter(this);
            _spriteRenderer.color = _mouseEnterColor;
        }

        private void OnMouseExit()
        {
            _battleManager.NotifyUnitMouseExit(this);
            _spriteRenderer.color = _originalColor;
        }

        public void PlayAttackFX()
        {
            AudioManager.Instance.PlayAudio(attackClip);
            Debug.Log("Attack, " + unitName + " play attack FX.");
        }
        
        public void TakeDamage(int damage)
        {
            if (damage <= curDefense)
            {
                curDefense -= damage;
                AudioManager.Instance.PlayAudio(attackOnShieldClip);
            }
            else
            {
                // 破防
                damage -= curDefense;
                curDefense = 0;

                health -= damage;
                if (health <= 0)                                    // 死亡
                {
                    health = 0;
                    // 通知BattleManager单位死亡
                    _battleManager.RegisterDeath(this);
                    Dead();
                }
                else                                                // 受伤
                {
                    AudioManager.Instance.PlayAudio(attackOnBodyClip);
                }
            }
            
            _battleUnitStyle.UpdateUI(this);
        }

        // 移动玩家，并刷新移动范围显示
        public void Move(Vector2 newPos)
        {
            // 移动单位
            _transform.position = newPos;
            AudioManager.Instance.PlayAudio(moveClip);
        }

        private void Dead()
        {
            _boxCol2D.offset += new Vector2(-1000, -1000);
            StartCoroutine(DeadFX());
        }
        
        IEnumerator DeadFX()
        {
            yield return null;
            _boxCol2D.enabled = false;
            yield return null; // 等待一帧，确保boxCollider2D已经关闭，防止退出时修改颜色
            _spriteRenderer.color = Color.gray * _originalColor;
            // _spriteRenderer.color = Color.red;
            
            float deltaTime = 0f;
            float timer = 0f;
            
            float gravity = -9.8f;
            float yVelocity = 3f;
            float rotatetime = Mathf.Abs(yVelocity / gravity);
            // _transform.Rotate(Vector3.up, 90f);
            
            while (_transform.position.y > -5)
            {
                deltaTime = Time.deltaTime;
                timer += deltaTime;
                
                if (timer < rotatetime)
                {
                    _transform.localEulerAngles = new Vector3(0, 0,-90 * timer / rotatetime);
                }
                
                yVelocity += deltaTime * gravity;
                _transform.position += Vector3.up * (yVelocity * deltaTime);
                yield return null;
            }
            
            yield return null;
            Destroy(gameObject);
        }
        
        public int GetAttackForce()
        {
            return Random.Range(minAttack, maxAttack + 1);
        }

    }
}