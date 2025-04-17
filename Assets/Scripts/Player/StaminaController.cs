using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;
using System.Collections;

public class StaminaController : MonoBehaviour
{
    public float staminaDecreaseRate = 10;
    public float staminaIncreaseRateWhileWalking = 10;
    public float staminaIncreaseRateWhileStanding = 20;
    public float jumpStaminaCost = 10;
    public float tiredSpeed = 1;
    public float tiredJumpHeight = 0.5f; 
    public bool isTired = false; // Variable para saber si el jugador está cansado
    public float recoverTime = 2; // Tiempo de recuperación

    private StarterAssetsInputs _input;
    private PlayerStats _playerStats;
    private FirstPersonController _firstPersonController;
    private Coroutine _staminaRecoveryCoroutine; // Variable para almacenar la corutina activa


    public bool jumped = false;
    public bool isRecoveringStamina = false;
    public bool isSpendingStamina = false;

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _playerStats = PlayerStats.Instance;
        _firstPersonController = GetComponent<FirstPersonController>();
    }

    private void Update()
    {
        UpdateStaminaWhileSprinting();

        if (isRecoveringStamina)
        {
            RecoverStamina(staminaIncreaseRateWhileWalking*2, staminaIncreaseRateWhileStanding*2);
            if (_playerStats.stamina >= _playerStats.maxStamina)
            {
                isRecoveringStamina = false;
            }
        } else if (!isTired)
        {
            RecoverStamina(staminaIncreaseRateWhileWalking, staminaIncreaseRateWhileStanding);
        }

        if (_playerStats.stamina <= 1 && !isTired)
        {
            _playerStats.stamina = 0;
            StartCoroutine(handleStaminaDepleted());
        }

        if (_input.jump && !isTired && _firstPersonController.Grounded && !jumped) 
        {
            handleJumping();
        }
    }

    private void UpdateStaminaWhileSprinting()
    {
        if (_input.sprint && !isTired && _input.move.magnitude > 0)
        {
            SpendStamina(staminaDecreaseRate * Time.deltaTime);
        }
        else if(isTired)
        {
            // Si la corutina aún no ha comenzado, la iniciamos
            if (_staminaRecoveryCoroutine == null)
            {
                _staminaRecoveryCoroutine = StartCoroutine(StaminaRecoveryTime());
            }
        }
    }

    public void SpendStamina(float amount)
    {
        isRecoveringStamina = false;
        _playerStats.DecreaseStamina(amount);
        StopStaminaRecovery();
    }

    public void RecoverStamina(float walkingIncrease, float standingIncrease)
    {
        if (_input.move.magnitude > 0)
        {
            _playerStats.IncreaseStamina(Time.deltaTime * walkingIncrease);
        }
        else
        {
            _playerStats.IncreaseStamina(Time.deltaTime * standingIncrease);
        }
    }

    private void handleJumping()
    {
        jumped = true;
        StartCoroutine(ResetJumpCooldown()); // Inicia la coroutine para resetear el estado de "jumped"
        SpendStamina(jumpStaminaCost);
    }

    private void StopStaminaRecovery()
    {
        if (_staminaRecoveryCoroutine != null)
        {
            StopCoroutine(_staminaRecoveryCoroutine);
            _staminaRecoveryCoroutine = null;
        }
    }
    

    private IEnumerator handleStaminaDepleted()
    {
        isTired = true; // El jugador está cansado
        float originalSprintSpeed = _firstPersonController.SprintSpeed;
        float originalWalkSpeed = _firstPersonController.MoveSpeed;
        float originalJumpHeight = _firstPersonController.JumpHeight;

        _firstPersonController.SprintSpeed = tiredSpeed;
        _firstPersonController.MoveSpeed = tiredSpeed;
        _firstPersonController.JumpHeight = tiredJumpHeight;

        yield return new WaitForSeconds(recoverTime+1);
        UnityEngine.Debug.Log("Stamina recovered");
        _firstPersonController.SprintSpeed = originalSprintSpeed;
        _firstPersonController.MoveSpeed = originalWalkSpeed;
        _firstPersonController.JumpHeight = originalJumpHeight;
        isTired = false;
    }

    private IEnumerator ResetJumpCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        jumped = false;
    }

    private IEnumerator StaminaRecoveryTime()
    {
        UnityEngine.Debug.Log("Stamina recovery started");
        yield return new WaitForSeconds(recoverTime);
        isRecoveringStamina = true; // Indicar que la stamina se está recuperando
        _staminaRecoveryCoroutine = null; // Resetear la referencia cuando la corutina termine
    }
}

