﻿using System.Collections.Generic;
using Model.Runtime.Projectiles;
using UnityEngine;

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

        public override Vector2Int GetNextStep()
        {
            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            List<Vector2Int> result = GetReachableTargets(); 

            float minDistance = float.MaxValue;

            foreach (var target in result)
            {
                var targetDistance = DistanceToOwnBase(target);
                if (targetDistance < minDistance)
                {
                    minDistance = targetDistance;
                }
                else
                {
                    result.Clear();
                    return result;
                }
                Debug.Log("target:" + target);
            }

            minDistance = result.Count;

            while (result.Count > 1)
            {
                result.RemoveAt(result.Count - 1);
            }
            return result;
        }

        public override void Update(float deltaTime, float time)
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

        private int GetTemperature() // метод получения температуры 
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature() // увелечение температуры 
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}