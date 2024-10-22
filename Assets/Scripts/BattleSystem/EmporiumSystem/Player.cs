using System.Collections.Generic;
using BattleSystem.Entity;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace BattleSystem.Emporium
{
    public static class PlayerData
    {
        // 该方式的缺点是 自身存储不了数据
        // 要想存储数据，需要额外实现一个类，然后将数据存储在该类中
        public static readonly BindableProperty<int> Gold = new();
        public static int partySize = 0;
        public static int maxPartySize = 4;
    }
}