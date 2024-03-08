using System.Collections.Generic;
using Project_Anxiety.Game.Units;

namespace Project_Anxiety.Game.Interfaces
{
    public interface IDamageable
    {
        public void TakeDamage(List<KeyValuePair<int, bool>> hits, CharacterBase fromCharacterBase);
    }
    
}

