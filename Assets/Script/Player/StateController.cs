using UnityEngine;

public class StateController : MonoBehaviour
{
    private PlayerState currentPlayer = PlayerState.idle;

    public void ChangeState(PlayerState movmentsState)
    {
        if (currentPlayer == movmentsState)
        {
            return;
        }
          currentPlayer = movmentsState; 
    }

    public PlayerState GetCurrentState()
    {
        return currentPlayer;
    }
}
