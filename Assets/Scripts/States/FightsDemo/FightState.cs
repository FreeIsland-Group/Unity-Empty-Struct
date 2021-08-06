namespace States
{
    // 战斗状态机
    public class FightState
    {
        public enum StateList
        {
            Exit, // 结束：已 GG，状态机离线
            Idle, // 等待：寻找下一个可移动/战斗的单位
            Move, // 移动：A 移动向 B
            Attack, // 战斗：A 移动向 B，AB 浮现伤害、AB 确认是否存活并处理、A 存活则返回
        }

        protected StateList state;

        public StateList GetState()
        {
            return state;
        }
        
        protected FightState()
        {
            // TODO: do something by init object
        }
        
        public virtual StateList Tick()
        {
            // TODO: do something with current StateList
            return StateList.Idle;
        }
        
        protected virtual StateList EndTick(StateList newState)
        {
            return newState;
        }
    }
}
