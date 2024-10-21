using System.Collections.Generic;
using System.Data;
using GluonGui.Dialog;
using Model.Runtime.Projectiles;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;
using static UnityEngine.GraphicsBuffer;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f; 
        private const float OverheatCooldown = 2f; 
        private float _temperature = 0f;
        private float _cooldownTime = 0f; 
        private bool _overheated;

        List<Vector2Int> outsideTarget = new List<Vector2Int>(); // Новое поле для целей вне зоны досягаемости, но которые ближе к базе, к ним и идем
        Vector2Int closesTarget = Vector2Int.zero;  // данная переменная будет хранить ближайшую цель //условеый pos
        List<Vector2Int> result = new List<Vector2Int>();



        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature; 
            GetTemperature();
            
            if (GetTemperature() <= overheatTemperature)
            {
                for (int i = 0; i < 3; i++)
                {
                    var projectile = CreateProjectile(forTarget);
                    AddProjectileToList(projectile, intoList);
                }
            }
            else
            {
                return;
            }

            IncreaseTemperature();
        }

        public override Vector2Int GetNextStep() //Этот метод возвращает следующий шаг агента. Однако, в данном случае метод просто вызывает метод GetNextStep из базового класса и возвращает его результат.
        {
            Vector2Int targetPosition;
            targetPosition = result.Count > 0 ? result[0] : unit.Pos;
            return IsTargetInRange(targetPosition) ? unit.Pos : unit.Pos.CalcNextStepTowards(targetPosition);
            //return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()//------------------------------------------------------------------
        {
            //------
            var allTargets = GetAllTargets();// получаем данные по всем целям через метод 

            float minDistance = float.MaxValue; //смотрим ближайшую цель за счет расчета максимального расстояния 

            foreach (Vector2Int target in allTargets)
            {
                float distance = DistanceToOwnBase(target); // проверяем ближайшую цель к базе и записываем ее как самую опасную 
                if (distance < minDistance) // если расстояние больше мин
                {
                    minDistance = distance;
                    closesTarget = target;
                }
            }

            if (minDistance < float.MaxValue) // После завершения цикла проверяется, было ли найдено хотя бы одно расстояние (т.е. minDistance все еще меньше максимального значения float.MaxValue).
            {
                result.Clear();  // Если да, то список result очищается, и в него добавляется ближайшая цель (closesTarget).
                result.Add(closesTarget); 
            }
            else
            {
                outsideTarget.Add(closesTarget); // если цель в не зоне видимости, то целью будут враги, которые ближе к базе
            }

            return result; // возвращаем значение. 


    }//-------------------------------------------------------------------------------------------------------------------------------

        public override void Update(float deltaTime, float time) // Этот метод обновляет состояние агента в зависимости от времени.
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature() 
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature() 
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}