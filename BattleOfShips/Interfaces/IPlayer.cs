using System.Collections.Generic;

namespace BattleOfShips
{
    public interface IPlayer
    {
        public string Name { get; set; }
        public Ship[] shipsOwned { get; set; }
        public Stack<string> hits { get; set; }
        Stack<string> misses { get; set; }
        void SetFleetPosition();
        bool FireAMissile(IPlayer opponent);
        bool BeenHit(string aim);
        bool IsDefeated();
    }
}