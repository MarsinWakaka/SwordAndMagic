﻿using System;
using System.Collections;
using System.Collections.Generic;
using GamePlaySystem.TileSystem;
using GamePlaySystem.TileSystem.Navigation;
using UnityEngine;

namespace GamePlaySystem.ControlCommand
{
    public struct CommandHandler
    {
        public readonly ICommand Command;
        public readonly Action OnCommandComplete;
        
        public CommandHandler(ICommand command, Action onCommandComplete)
        {
            Command = command;
            OnCommandComplete = onCommandComplete;
        }
    }
    
    public interface ICommandManager
    {
        void ExecuteCommand(ICommand command, Action onCommandComplete);
    }
    
    public class CommandManager : MonoBehaviour , ICommandManager
    {
        private readonly Queue<CommandHandler> commandQueue = new();
        public void ExecuteCommand(ICommand command, Action onCommandComplete)
        {
            commandQueue.Enqueue(new CommandHandler(command, onCommandComplete));
            if (commandQueue.Count == 1)
            {
                StartCoroutine(DoCommand());
            }
        }
        
        private readonly WaitForSeconds handleInterval = new(0.7f);
        private IEnumerator DoCommand()
        {
            while (commandQueue.Count > 0)
            {
                var commandHandle = commandQueue.Dequeue();
                var command = commandHandle.Command;
                switch (command)
                {
                    case FollowPathCommand followPathCommand:
                        yield return HandleFollowPathCommand(followPathCommand, commandHandle.OnCommandComplete);
                        break;
                    case BaseSkillCommand baseSkillCommand:
                        yield return HandleBaseSkillCommand(baseSkillCommand, commandHandle.OnCommandComplete);
                        break;
                }
                yield return handleInterval;
            }
        }

        private IEnumerator HandleBaseSkillCommand(BaseSkillCommand baseSkillCommand, Action onComplete)
        {
            var caster = baseSkillCommand.Caster;
            var targets = baseSkillCommand.Targets;
            var skillSlot = baseSkillCommand.ChosenSkillSlot;
            var skill = skillSlot.skill;
            skill.Execute(caster, targets);
            yield return handleInterval;
            // 计算技能消耗
            caster.Property.AP.Value -= skill.AP_Cost;
            caster.Property.SP.Value -= skill.SP_Cost;
            skillSlot.RemainCoolDown.Value = skill.coolDown;
            onComplete?.Invoke();
        }

        private readonly WaitForSeconds _moveInterval = new(0.3f);
        private IEnumerator HandleFollowPathCommand(FollowPathCommand command, Action onComplete)
        {
            var destNode = command.DestNode;
            var character = command.Character;
            List<PathNode> path = new();    // 包含起点和终点
            while (destNode != null)
            {
                path.Add(destNode);
                destNode = destNode.FromNode;
            }
            if (path.Count <= 1)
            {
                Debug.LogWarning("路径节点数量小于等于1, 如果不需要移动请不要生成命令");
                onComplete?.Invoke();
                yield break;
            }
            path.Reverse();
            var index = 0;
            var targetPos = Vector3.zero;
            var tileManager = ServiceLocator.Get<TileManager>();
            for(index = 1; index < path.Count; index++)
            {
                var node = path[index];
                targetPos.x = node.PosX;
                targetPos.y = node.PosY;
                if (!tileManager.CharacterMove( character, character.transform.position, targetPos)) {
                    break;
                }
                yield return _moveInterval;
            }
            character.Property.RWR.Value -= path[index - 1].Cost;   // 减去最后一个成功移动节点的消耗
            onComplete?.Invoke();
            yield return null;
        }
    }
}