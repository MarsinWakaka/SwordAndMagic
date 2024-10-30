using System;
using System.Collections.Generic;
using BattleSystem.FactionSystem;
using UnityEngine;
using Utility.Singleton;

namespace Entity
{
    // TODO 通过EPPlus插件读取EXCEL数据来初始化实体列表
    // public class EntityFactory : SingletonMono<EntityFactory>
    // {
    //     [Header("实体设置")]
    //     [SerializeField] BaseEntity[] entities;
    //     
    //     private readonly Dictionary<int, BaseEntity> entityDict = new();
    //     public Transform cellRoot;
    //     public Transform entityRoot;
    //     
    //     protected override void Awake()
    //     {
    //         base.Awake();
    //         // 暂时替代角色信息的获取
    //         foreach (var entity in entities) entityDict.Add(entity.entityID, entity);
    //     }
    //
    //     public void CreateEntity(int entityClassID, Vector2 position)
    //     {
    //         if (!entityDict.TryGetValue(entityClassID, out var primitive))
    //         {
    //             Debug.LogError("实体不存在: " + entityClassID);
    //             return;
    //         }
    //         // 创建实体
    //         var entity = Instantiate(primitive, entityRoot);
    //         entity.Initialize(GenerateUniqueID(), position);
    //     }
    //
    //     #region 角色工厂区
    //     
    //     /// <summary>
    //     /// 角色创建工厂
    //     /// </summary>
    //     public void CreateCharacter(FactionType factionType, int entityClassID, Vector2 position)
    //     {
    //         if (!entityDict.TryGetValue(entityClassID, out var primitive)) {
    //             Debug.LogError("实体不存在: " + entityClassID);
    //             return;
    //         }
    //         // 创建实体
    //         if (primitive is not Player.Character charPrefab) {
    //             Debug.LogError($"实体不是角色: {entityClassID}, Type: {primitive.GetType()}");
    //             return;
    //         }
    //         
    //         var character = Instantiate(charPrefab, entityRoot);
    //         character.Initialize(GenerateUniqueID(), position,factionType);
    //     }
    //     
    //     #endregion
    //     
    //     private Guid GenerateUniqueID()
    //     {
    //         // 生成唯一ID，类似Java的UUID
    //         return Guid.NewGuid();
    //     }
    // }
}