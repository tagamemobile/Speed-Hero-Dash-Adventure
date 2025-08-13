using UnityEngine;
using System.Collections;
namespace TAGame
{
    public interface ICanTakeDamage
    {
        void TakeDamage(float damage, Vector2 force, GameObject instigator);
    }

}
