using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Project_Anxiety.Game.Units;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    #region Main HUD
    [Header("Main HUD")]
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private TMP_Text _mpText;
    [SerializeField] private TMP_Text _expText;

    [SerializeField] private Slider _healthBar;
    [SerializeField] private Slider _manaBar;
    [SerializeField] private Slider _expBar;
    #endregion

    #region Ability Window

    [FormerlySerializedAs("_skillWindowJobText")]
    [Header("Skill Window")] 
    [SerializeField] private TMP_Text _abilityWindowJobText;
    [FormerlySerializedAs("_skillContent")] [SerializeField] private GameObject _abilityContent;
    [FormerlySerializedAs("_skillTemplate")] [SerializeField] private GameObject _abilityTemplate;
    [FormerlySerializedAs("_skillPointsText")] [SerializeField] private TMP_Text _abilityPointsText;
    private List<GameObject> _abilities = new();
    #endregion
    
    #region Stat Window

    [Header("Stat Window")] 
    [SerializeField] private TMP_Text _playerNameStatsText;
    [SerializeField] private TMP_Text _playerJobStatsText;
    [SerializeField] private TMP_Text _playerLevelStatsText;
    [SerializeField] private TMP_Text _playerExpStatsText;
    [SerializeField] private TMP_Text _playerHpStatsText;
    [SerializeField] private TMP_Text _playerMpStatsText;
    
    [SerializeField] private TMP_Text _attributePointsText;

    [SerializeField] private TMP_Text _playerStrStatsText;
    [SerializeField] private TMP_Text _playerDexStatsText;
    [SerializeField] private TMP_Text _playerIntStatsText;
    [SerializeField] private TMP_Text _playerLukStatsText;

    [SerializeField] private Button _autoAssignButton;
    
    [SerializeField] private Button _strButton;
    [SerializeField] private Button _dexButton;
    [SerializeField] private Button _intButton;
    [SerializeField] private Button _lukButton;

    [SerializeField] private TMP_Text _attackText;
    [SerializeField] private TMP_Text _critRateText;
    [SerializeField] private TMP_Text _defenseText;
    [SerializeField] private TMP_Text _speedText;
    [SerializeField] private TMP_Text _jumpText;

    #endregion

    #region Windows
    [FormerlySerializedAs("_skillWindow")]
    [Header("Windows")]
    [SerializeField] private GameObject _abilityWindow;
    [SerializeField] private GameObject _statWindow;
    [SerializeField] private GameObject _detailWindow;
    [SerializeField] private GameObject _hotBarWindow;
    #endregion

    
    [SerializeField] private Player _player;
    private bool _hotBarMinimized = false;

    public GameObject AbilityWindow => _abilityWindow;
    public GameObject StatWindow => _statWindow;
    public GameObject HotBarWindow => _hotBarWindow;
    
    private void Update()
    {
        SetClientHUD();
        UpdateStatWindow();
        UpdateSkillWindow();
    }
    
    private void SetClientHUD()
    {
        _levelText.text = "Lv. " + _player.JobSystem.CurrentJob.JobLevel;
        _playerNameText.text = _player.PlayerName;

        _hpText.text = _player.Health.CurrentHp + "/" +
                       _player.Health.MaxHp;
        _mpText.text = _player.Health.CurrentMp + "/" +
                       _player.Health.MaxMp;
        _expText.text = _player.JobSystem.CurrentJob.CurrentExp + "/" +
                        _player.JobSystem.CurrentJob.ExpToNextLevel;

        _healthBar.maxValue = _player.Health.MaxHp;
        _healthBar.value = _player.Health.CurrentHp;

        _manaBar.maxValue = _player.Health.MaxMp;
        _manaBar.value = _player.Health.CurrentMp;

        _expBar.maxValue = _player.JobSystem.CurrentJob.ExpToNextLevel;
        _expBar.value = _player.JobSystem.CurrentJob.CurrentExp;
    }
    
    private void UpdateStatWindow()
    {
        if(!_player.PlayerUI._statWindow.activeSelf) return;
        _playerNameStatsText.text = _player.PlayerName;
        _playerJobStatsText.text = _player.JobSystem.CurrentJob.JobData.JobName;
        _playerLevelStatsText.text = _player.JobSystem.CurrentJob.JobLevel.ToString();
        _playerExpStatsText.text = _player.JobSystem.CurrentJob.CurrentExp + " (" +  Mathf.Round((float)_player.JobSystem.CurrentJob.CurrentExp/_player.JobSystem.CurrentJob.ExpToNextLevel * 100)  + "%)";
        _playerHpStatsText.text = _player.Health.CurrentHp + "/" + _player.Health.MaxHp;
        _playerMpStatsText.text = _player.Health.CurrentMp + "/" + _player.Health.MaxMp;
        _attributePointsText.text = _player.Stats.AttributePoint.ToString();
        
        _playerStrStatsText.text = SetText(AttributeType.Strength);
        _playerDexStatsText.text = SetText(AttributeType.Dexterity);
        _playerIntStatsText.text = SetText(AttributeType.Intelligence);
        _playerLukStatsText.text = SetText(AttributeType.Luck);

        if (_detailWindow.activeInHierarchy)
        {
            _attackText.text = $"{_player.PlayerCombatSystem.MinDamage} ~ {_player.PlayerCombatSystem.MaxDamage}";
            _critRateText.text = $"{(int)(_player.PlayerCombatSystem.TotalCritRate*100)}%";
            _defenseText.text = $"{_player.Health.TotalPhysicalDefense}";
            _speedText.text = $"{_player.CurrentMoveSpeed + _player.Stats.BonusMoveSpeed}";
            _jumpText.text = $"{_player.JumpImpulse + _player.Stats.BonusJumpImpulse}";
        }

        if (_player.Stats.AttributePoint > 0)
        {
            _autoAssignButton.interactable = true;
            _strButton.interactable = true;
            _dexButton.interactable = true;
            _intButton.interactable = true;
            _lukButton.interactable = true;
        }
        else
        {
            _autoAssignButton.interactable = false;
            _strButton.interactable = false;
            _dexButton.interactable = false;
            _intButton.interactable = false;
            _lukButton.interactable = false;
        }

        string SetText(AttributeType type)
        {
            var attribute = _player.Stats.Attributes.Find(x => x.AttributeType == type);
            return $"{attribute.TotalValue} ({attribute.BaseValue} + {attribute.CombinedBonusValue})";
        }
    }

    private void UpdateSkillWindow()
    {
        if(!_player.PlayerUI._abilityWindow.activeSelf) return;
        _abilityWindowJobText.text = _player.JobSystem.CurrentJob.JobData.JobName;
        if (_abilities.Count != _player.JobSystem.CurrentJob.JobData.JobAbilities.Count)
        {
            var sortedList = new List<AbilityData>(_player.JobSystem.CurrentJob.JobData.JobAbilities.OrderByDescending(x => x.GetType().ToString()).ThenBy(x => x.Name));
            foreach (AbilityData ability in sortedList)
            {
                GameObject tmp = Instantiate(_abilityTemplate, _abilityContent.transform);
                AbilityTemplate template = tmp.GetComponent<AbilityTemplate>();
                template.abilityDataAttached = ability;
                template.AbilityImage.sprite = ability.Sprite;
                template.AbilityNameText.text = ability.Name;
                template.AbilityLevelText.text = ability.Level.ToString();
                _abilities.Add(tmp);
            }
        }
        
        SetLevelUpButtonInteractable();
        
        _abilityPointsText.text = _player.Stats.AbilityPoint.ToString();
    }

    private void SetLevelUpButtonInteractable()
    {
        var state = _player.Stats.AbilityPoint > 0;
        foreach (var template in _abilities.Select(ability => ability.GetComponent<AbilityTemplate>()))
        {
            template.AbilityLevelUpButton.interactable = state;
        }
    }

    public void DetailButtonPressed()
    {
        _detailWindow.SetActive(!_detailWindow.activeSelf);
    }
    
    public void OnAutoAssignButton()
    {
        AttributeType mainAttribute = _player.JobSystem.CurrentJob.JobData.JobMainAttribute;
        int allAttributePoints = _player.Stats.AttributePoint;
        _player.Stats.UseAttributePoint(mainAttribute, allAttributePoints);
    }
    
    public void OnStrButtonAdd()
    {
        _player.Stats.UseAttributePoint(AttributeType.Strength,1);
    }
    
    public void OnDexButtonAdd()
    {
        _player.Stats.UseAttributePoint(AttributeType.Dexterity,1);
    }
    
    public void OnIntButtonAdd()
    {
        _player.Stats.UseAttributePoint(AttributeType.Intelligence,1);
    }
    
    public void OnLukButtonAdd()
    {
        _player.Stats.UseAttributePoint(AttributeType.Luck,1);
    }
    
    public void OnHotBarButtonClick()
    {
        var anim = HotBarWindow.GetComponent<Animation>();
        if (_hotBarMinimized)
        {
            anim.Stop();
            anim.Play("HotBarSlideLeft");
            _hotBarMinimized = !_hotBarMinimized;
        }
        else
        {
            anim.Stop();
            anim.Play("HotBarSlideRight");
            _hotBarMinimized = !_hotBarMinimized;
        }
    }
}
