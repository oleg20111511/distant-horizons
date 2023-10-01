using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillCheckController : MonoBehaviour
{
    // Events
    public event System.Action SkillCheckPassed;

    [SerializeField] private float rotationDuration;
    [SerializeField] private SpriteRenderer spriteRenderer;
    

    private float skillCheckPosition;
    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }


    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.gameplay.SkillCheck.performed += OnSkillCheckAction;
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerControls.gameplay.SkillCheck.performed -= OnSkillCheckAction;
    }


    public void Initialize()
    {
        SetSkillCheckPosition();
        StartSpin();
    }


    private void SetSkillCheckPosition()
    {
        skillCheckPosition = Random.Range(0.5f, 1.5f);
        spriteRenderer.material.SetFloat("_CheckPosition", skillCheckPosition);
    }


    private void StartSpin()
    {
        spriteRenderer.material.SetFloat("_CursorPosition", 0.5f);
        spriteRenderer.material.DOFloat(1.5f, "_CursorPosition", rotationDuration).OnComplete(StartSpin).SetEase(Ease.Linear);
    }


    private void OnSkillCheckAction(InputAction.CallbackContext context)
    {
        spriteRenderer.material.DOKill();
        float currentPosition = spriteRenderer.material.GetFloat("_CursorPosition");
        float checkErrorMargin = spriteRenderer.material.GetFloat("_CheckThickness");
        if (currentPosition > skillCheckPosition - checkErrorMargin && currentPosition < skillCheckPosition + checkErrorMargin)
        {
            SkillCheckPassed?.Invoke();
            gameObject.SetActive(false);
        }
        else
        {
            Initialize();
        }
    }
}
