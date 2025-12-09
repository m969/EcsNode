using ECS;
using System;

namespace ECSGame.AttackModule
{
    // 组件系统：伤害计算（纯函数式，静态方法）
    public class AttackDamageSystem : AComponentSystem<AttackAction, AttackDamageComponent>
    {
        /// <summary>
        /// 计算最终伤害
        /// </summary>
        public static bool TryCalculateDamage(AttackAction action, out int finalDamage, out DamageFailReason fail)
        {
            var dmg = action.GetComponent<AttackDamageComponent>();
            // 基础校验
            if (dmg == null)
            {
                finalDamage = 0;
                fail = DamageFailReason.AttributeMissing;
                return false;
            }

            try
            {
                checked
                {
                    switch (dmg.Formula)
                    {
                        case DamageFormulaType.Flat:
                            finalDamage = dmg.BaseDamage;
                            fail = DamageFailReason.None;
                            return true;
                        case DamageFormulaType.AttackMinusDefense:
                            // 占位：仅用 BaseDamage 表示已计算结果，真实Att/Def由外部插入扩展
                            finalDamage = Math.Max(0, dmg.BaseDamage);
                            fail = DamageFailReason.None;
                            return true;
                        case DamageFormulaType.AttackTimesRatio:
                            // 占位：BaseDamage 代表乘后结果
                            finalDamage = Math.Max(0, dmg.BaseDamage);
                            fail = DamageFailReason.None;
                            return true;
                        default:
                            finalDamage = 0;
                            fail = DamageFailReason.FormulaUnsupported;
                            return false;
                    }
                }
            }
            catch (OverflowException)
            {
                finalDamage = 0;
                fail = DamageFailReason.Overflow;
                return false;
            }
            catch (Exception)
            {
                finalDamage = 0;
                fail = DamageFailReason.InternalError;
                return false;
            }
        }
    }
}
