using System.Collections.Generic;
using Prefab;
using UnityEngine;

namespace States
{
    // 战斗状态：移动
    public class FightStateMove : FightState
    {
        private GameObject anchorTest;
        private FightArmyItem fighter;
        private List<FightBoardItem> routeList; // 路由列表
        private FightArmyItem enemy;
        private readonly float speedUnit;
        private readonly float speedBound;

        private int loop;
        private int routeIndex;

        // 初始化
        public FightStateMove(FightArmyItem fighter, List<FightBoardItem> routeList, float speedUnit, float speedBound, GameObject anchorTest)
        {
            state = StateList.Move;
            routeIndex = routeList.Count - 1;
            loop = 1200;

            this.anchorTest = anchorTest;
            this.fighter = fighter;
            this.routeList = routeList;
            this.speedUnit = speedUnit;
            this.speedBound = speedBound;
        }

        // 每帧行动
        public override StateList Tick()
        {
            loop--;
            var targetBoard = routeList[routeIndex];

            Vector2 nowPos = fighter.gameObject.transform.position;
            Vector2 newPos = targetBoard.gameObject.transform.position;
            var remainingDistance = newPos - nowPos;
            float speed = speedUnit * speedBound;

            anchorTest.transform.position = targetBoard.gameObject.transform.position;
            
            fighter.gameObject.transform.position =
                nowPos + remainingDistance.normalized * (speed * Time.deltaTime);
            if (remainingDistance.sqrMagnitude <= speedBound * .01f || loop <= 0)
            {
                fighter.gameObject.transform.position = new Vector3(newPos.x, newPos.y, 0);
                fighter.board.isOccupy = false;
                targetBoard.isOccupy = true;
                fighter.board = targetBoard;

                routeIndex--;
                fighter.power -= 1;
                Debug.Log("战士体力剩余: " + fighter.power);
            }

            if (fighter.power <= 0)
            {
                Debug.Log("无体力结束移动");
                return EndTick(StateList.Idle);
            }

            if (routeIndex < 0)
            {
                Debug.Log("移动结束，尚有体力，开始攻击");
                return EndTick(StateList.Attack);
            }

            return EndTick(StateList.Move);
        }

        // Tick 结束回调
        protected override StateList EndTick(StateList nextState)
        {
            if (nextState == StateList.Idle)
            {
                if (fighter) fighter.power = fighter.powerOrigin;
            }

            return nextState;
        }
    }
}
