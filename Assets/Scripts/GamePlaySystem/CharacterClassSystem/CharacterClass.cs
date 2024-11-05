using System;
using System.Collections.Generic;
using Data;
using Entity;
using GamePlaySystem.SkillSystem;
using UnityEngine;

namespace GamePlaySystem.CharacterClassSystem
{
    public enum ClassType
    {
        Barbarian,
        Bard,
        Cleric,
        Druid,
        Fighter,
        Monk,
        Paladin,
        Ranger,     // 游侠 | 猎人
        Rogue,
        Sorcerer,
        Warlock,
        Wizard
    }
    
    [Serializable]
    public struct CharacterClassData
    {
        public ClassType classType;
        public int level;
    }
    
    [Serializable]
    public class LevelUpEffect
    {
        [Header("属性点提升")]
        [Range(0, 5)] public int constitution;
        [Range(0, 5)] public int strength;
        [Range(0, 5)] public int intelligence;
        [Range(0, 5)] public int perception;
        [Range(0, 5)] public int dexterity;
        [Header("技能获得")]
        public List<BaseSkill> skillsGain;
    }
    
    [CreateAssetMenu(menuName = "新建角色职业", fileName = "New Character Class")]
    public class CharacterClass : ScriptableObject
    {
        public ClassType classType;
        public List<LevelUpEffect> levelUpEffects;
    }

    public class CharacterClassManager
    {
        private readonly Dictionary<ClassType, CharacterClass> classTable = new();
        public void Initialize(List<CharacterClass> classes) {
            foreach (var characterClass in classes) {
                classTable.Add(characterClass.classType, characterClass);
            }
        }
        public LevelUpEffect GetLevelUpEffect(ClassType type, int level) {
            return classTable[type].levelUpEffects[level];
        }
        
        public void CharacterLevelUp(CharacterProperty property, ClassType type)
        {
            CharacterClassData classData;
            if (property.classTable.Exists(x => x.classType == type))
            {
                classData = property.classTable.Find(x => x.classType == type);
                classData.level++;
            }
            else
            {
                classData = new CharacterClassData
                {
                    classType = type,
                    level = 1
                };
                property.classTable.Add(classData);
            }
            if (classData.level > classTable[type].levelUpEffects.Count)
            {
                Debug.LogError($"Character {property.characterName} has reached the max level of class {type}.");
                return;
            }
            ApplyLevelUpEffect(property, classTable[type].levelUpEffects[classData.level - 1]); // 从0开始
        }
        
        private void ApplyLevelUpEffect(CharacterProperty property, LevelUpEffect effect)
        {
            property.CON.Value += effect.constitution;
            property.STR.Value += effect.strength;
            property.INT.Value += effect.intelligence;
            property.PER.Value += effect.perception;
            property.DEX.Value += effect.dexterity;
            foreach (var skill in effect.skillsGain) {
                property.Skills.Add(skill);
            }
        }
        
        public void CharacterLevelDown(CharacterProperty property, ClassType type)
        {
            if (property.classTable.Exists(x => x.classType == type))
            {
                var classData = property.classTable.Find(x => x.classType == type);
                classData.level--;
                ApplyLevelDownEffect(property, classTable[type].levelUpEffects[classData.level - 1]); // 从0开始
            }
            else
            {
                Debug.LogError($"Character does not have this class {type}.");
            }
        }

        private void ApplyLevelDownEffect(CharacterProperty property, LevelUpEffect levelUpEffect)
        {
            property.CON.Value -= levelUpEffect.constitution;
            property.STR.Value -= levelUpEffect.strength;
            property.INT.Value -= levelUpEffect.intelligence;
            property.PER.Value -= levelUpEffect.perception;
            property.DEX.Value -= levelUpEffect.dexterity;
            foreach (var skill in levelUpEffect.skillsGain)
            {
                property.Skills.Remove(skill);
            }
        }
    }
}