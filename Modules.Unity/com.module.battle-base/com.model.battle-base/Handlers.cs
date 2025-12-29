using ECS;

namespace ECSGame.BattleBase
{
    public interface IBattleBaseLevelChangedHandler
    {
        void OnBattleBaseLevelChanged(BattleBaseEntity entity, int oldLevel, int newLevel);
    }

    public interface IBattleBaseEnergyChangedHandler
    {
        void OnBattleBaseEnergyChanged(BattleBaseEntity entity, float currentEnergy, float maxEnergy);
    }

    public interface IBattleBasePartChangedHandler
    {
        void OnBattleBasePartChanged(BattleBaseEntity entity, BattleBasePartEntity part, bool isAdded);
    }
}
