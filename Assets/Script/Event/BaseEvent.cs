using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia
{
    /// <summary>
    /// 事件类的基类
    /// </summary>
    public abstract class BaseEvent
    {
        public GameDataBase.EventType eventType=GameDataBase.EventType.INVALID;
    }
}