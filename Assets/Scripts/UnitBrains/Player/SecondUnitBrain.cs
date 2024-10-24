﻿using System.Collections.Generic;
using Codice.Client.Common.GameUI;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
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
        private List<Vector2Int> dangerousTarget = new List<Vector2Int>();


        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;

            float _temperature = GetTemperature();

            if (_temperature >= overheatTemperature)
            {
                return;
            }

            if (_temperature == 0)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }

            for (int i = 1; i <= _temperature; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }

            IncreaseTemperature();
        }

        public override Vector2Int GetNextStep()
        {
            if (dangerousTarget == null)
            {
                return Vector2Int.zero;
            }
            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {

            float minTarget = float.MaxValue;
            Vector2Int closestTarget = new Vector2Int();

            List<Vector2Int> result = new List<Vector2Int>();


            IEnumerable<Vector2Int> allTargets = GetAllTargets();

            if (allTargets == null)
            {
                result.Clear();
                result[0] = runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];
                Debug.Log(result);
            }

            foreach (var target in allTargets)
            {

                float distance = DistanceToOwnBase(target);
                if (distance < minTarget)
                {
                    minTarget = distance;
                    closestTarget = target;
                    dangerousTarget.Clear();

                }
            }

            if (!IsTargetInRange(closestTarget))
            {
                return new List<Vector2Int>();
            }

            if (minTarget != float.MaxValue)
            {
                dangerousTarget.Add(closestTarget);
                result.Add(closestTarget);
                return result;

            }
            result.Clear();
            return new List<Vector2Int>();
        }
        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown / 10);
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
            if (_overheated) return (int)OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}