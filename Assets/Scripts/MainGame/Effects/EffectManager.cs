using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    #region Public Properties

    public float AttackBonus { get; } // default: 0

    public float DefenceBonus { get; } // default: 0

    public int Actions { get; } // default: 2

    public bool Traversable { get; } // default: true

    public int LevelCap { get; } // default: 5

    #endregion
}
