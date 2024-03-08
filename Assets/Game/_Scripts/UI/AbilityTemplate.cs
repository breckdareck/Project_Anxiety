using Project_Anxiety.Game.Units;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AbilityTemplate : MonoBehaviour
{
    [FormerlySerializedAs("AbilityAttached")] public AbilityData abilityDataAttached;
    
    public Image AbilityImage;
    public TMP_Text AbilityNameText;
    public TMP_Text AbilityLevelText;
    public Button AbilityLevelUpButton;
}
