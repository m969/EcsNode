using ECS;

namespace ECSGame.BattleBaseModule
{
    public interface IBattleBaseLevelChangedHandler : IDispatch
    {
        void OnBattleBaseLevelChanged(BattleBaseEntity entity, int oldLevel, int newLevel);
    }

    public interface IBattleBaseEnergyChangedHandler : IDispatch
    {
        void OnBattleBaseEnergyChanged(BattleBaseEntity entity, float currentEnergy, float maxEnergy);
    }

    public interface IBattleBasePartChangedHandler : IDispatch
    {
        void OnBattleBasePartChanged(BattleBaseEntity entity, BattleBasePartEntity part, bool isAdded);
    }
}
