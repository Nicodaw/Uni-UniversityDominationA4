using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    #region Public Properties

    public int AttackBonus { get; } // default: 0

    public int DefenceBonus { get; } // default: 0

    public int Actions { get; } // default: 2

    public bool Traversable { get; } // default: true

    public int LevelCap { get; } // default: 5

    #endregion
}
