using System.Collections.Generic;
using DefaultNamespace.Data;
using Prefab;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
    // 战斗状态：移动
    public class FightStateAttack : FightState
    {
        private enum AttackStep
        {
            Goto,
            Back,
            Shoot,
        };

        private AttackStep attackMove;
        private FightArmyItem fighter;
        private FightArmyItem enemy;
        private readonly float speedUnit;
        private readonly float speedBound;
        private readonly HeroData.Hero hero;
        private readonly HeroData.Hero enemyHero;
        private List<FightArmyItem> armyList;
        private Vector2 backPos;
        private Vector2 startPos;
        private Vector2 endPos;
        private Image throwImage;
        private GameObject anchorTest;
        private int loop;

        // 初始化
        public FightStateAttack(FightArmyItem fighter, FightArmyItem enemy, List<FightArmyItem> armyList,
            float speedUnit, float speedBound, HeroData.Hero hero, HeroData.Hero enemyHero, Image throwImage = null, GameObject anchorTest = null)
        {
            state = StateList.Attack;
            
            // 设定
            loop = 1200;
            this.anchorTest = anchorTest;
            this.hero = hero;
            this.enemyHero = enemyHero;
            this.fighter = fighter;
            this.enemy = enemy;
            this.armyList = armyList;
            this.speedUnit = speedUnit;
            this.speedBound = speedBound;
            this.throwImage = throwImage;
            
            Vector2 fighterVec = new Vector2(fighter.board.x, fighter.board.y);
            Vector2 enemyVec = new Vector2(enemy.board.x, enemy.board.y);
            Vector2 distanceVec = enemyVec - fighterVec;
            float distance = Mathf.Sqrt(Mathf.Pow(distanceVec.x, 2) + Mathf.Pow(distanceVec.y, 2)) + 1;

            if (distance < 2.05f)
            {
                attackMove = AttackStep.Goto;
                // 移动到敌人一半距离
                // 出发点(start)，结束点(enemyPos)，中点(end); 结束点 - 出发点 = 距离向量; 出发点 + 距离向量 / 2 = 中点
                backPos = fighter.gameObject.transform.position;
                startPos = fighter.gameObject.transform.position;
                Vector2 enemyPos = enemy.gameObject.transform.position;
                endPos = startPos + (enemyPos - startPos) / 2;
            }
            else if (distance > 2.05f)
            {
                attackMove = AttackStep.Shoot;
                // 设定远程物的面向角度
                throwImage.gameObject.transform.position = fighter.transform.position;
                throwImage.gameObject.SetActive(true);
                throwImage.transform.right = throwImage.transform.position - enemy.transform.position;
                // 移动到敌人身上
                startPos = throwImage.gameObject.transform.position;
                Vector2 enemyPos = enemy.gameObject.transform.position;
                endPos = startPos + (enemyPos - startPos);
            }
        }

        // 每帧行动
        public override StateList Tick()
        {
            float speed = speedUnit * speedBound;
            loop--;

            // 远程
            if (attackMove == AttackStep.Shoot)
            {
                startPos = throwImage.gameObject.transform.position;
                // 移动射击对象
                var distance = endPos - startPos;
                throwImage.gameObject.transform.position = startPos + distance.normalized * (speed * Time.deltaTime * 2.5f);
                // 射击对象与敌人接触的足够近，发起攻击
                if (distance.sqrMagnitude < speedBound * .01f || loop <= 0)
                {
                    throwImage.gameObject.SetActive(false); // 隐藏射击对象
                    float attack = (1 + hero.attack / 100) * fighter.attack;
                    float enemyDefence = (1 + enemyHero.defence / 100) * enemy.defence;
                    float enemyEvasion = (1 + enemyHero.evasion / 100) * enemy.evasion;
                    float precise = (1 + hero.precise / 100) * fighter.precise;

                    float rate = (enemyEvasion - precise) / enemyEvasion;
                    if ((enemyEvasion <= precise && Random.Range(0, 1f) > .01f) || Random.Range(0, 1f) > rate)
                    {
                        enemyDefence = (enemyDefence * 0.03f) / (1 + 0.03f * enemyDefence);
                        attack = attack * (1 - enemyDefence);
                    }
                    else
                    {
                        attack = 0;
                    }

                    enemy.ShowTakeBeating(attack);
                    enemy.hp -= attack;
                    enemy.progress.SetValue(enemy.hp);

                    // 判断敌人是否死亡，死亡则移除敌人对象；判断己方是否死亡，死亡则移除己方对象
                    if (enemy.hp <= 0)
                    {
                        enemy.board.isOccupy = false;
                        armyList.Remove(enemy);
                        Object.Destroy(enemy.gameObject);
                        enemy = null;
                    }
                    
                    if (fighter.hp <= 0)
                    {
                        fighter.board.isOccupy = false;
                        armyList.Remove(fighter);
                        Object.Destroy(fighter.gameObject);
                        fighter = null;
                    }
                    return EndTick(StateList.Idle);
                }
            }

            // 近战，贴身
            if (attackMove == AttackStep.Goto)
            {
                startPos = fighter.gameObject.transform.position;
                var distance = endPos - startPos;
                fighter.gameObject.transform.position = startPos + distance.normalized * (speed * Time.deltaTime);
                // 进攻
                if (distance.sqrMagnitude < speedBound * .01f || loop <= 0)
                {
                    loop = 1200;
                    fighter.gameObject.transform.position = new Vector3(endPos.x, endPos.y, 0);
                    
                    float attack = (1 + hero.attack / 100) * fighter.attack;
                    float enemyDefence = (1 + enemyHero.defence / 100) * enemy.defence;
                    float enemyEvasion = (1 + enemyHero.evasion / 100) * enemy.evasion;
                    float precise = (1 + hero.precise / 100) * fighter.precise;

                    float rate = (enemyEvasion - precise) / enemyEvasion;
                    if ((enemyEvasion <= precise && Random.Range(0, 1f) > .01f) || Random.Range(0, 1f) > rate)
                    {
                        enemyDefence = (enemyDefence * 0.03f) / (1 + 0.03f * enemyDefence);
                        attack = attack * (1 - enemyDefence);
                    }
                    else
                    {
                        attack = 0;
                    }
                    
                    enemy.ShowTakeBeating(attack);
                    enemy.hp -= attack;
                    enemy.progress.SetValue(enemy.hp);

                    // 判断敌人是否死亡，死亡则移除敌人对象；判断己方是否死亡，死亡则移除己方对象
                    if (enemy.hp <= 0)
                    {
                        enemy.board.isOccupy = false;
                        armyList.Remove(enemy);
                        Object.Destroy(enemy.gameObject);
                    }

                    if (fighter.hp <= 0)
                    {
                        fighter.board.isOccupy = false;
                        armyList.Remove(fighter);
                        Object.Destroy(fighter.gameObject);
                        fighter = null;
                        return EndTick(StateList.Idle);
                    }
                    
                    attackMove = AttackStep.Back;
                }
            }
            // 近战，退回
            if (attackMove == AttackStep.Back)
            {
                startPos = fighter.gameObject.transform.position;
                // 本方未死亡，撤回原定距离
                var distance = backPos - startPos;
                fighter.gameObject.transform.position = startPos + distance.normalized * (speed * Time.deltaTime);
                if (distance.sqrMagnitude < speedBound * .01f || loop <= 0)
                {
                    fighter.gameObject.transform.position = new Vector3(backPos.x, backPos.y, 0);
                    Debug.Log("进攻结束");
                    return EndTick(StateList.Idle);
                }
            }

            return EndTick(StateList.Attack);
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
