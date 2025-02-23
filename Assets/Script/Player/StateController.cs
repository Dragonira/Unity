using UnityEngine;

public class StateController : MonoBehaviour
{
    private MovmentsState currentPlayer = MovmentsState.walking;

    public void ChangeState(MovmentsState movmentsState)
    {
        if (currentPlayer == movmentsState)
        {
            return;
        }
          currentPlayer = movmentsState; 
    }

    public MovmentsState GetMovmentsState()
    {
        return currentPlayer;
    }
}
