using UnityEngine;

[RequireComponent(typeof(InputController))]
public class PlayerController : MonoBehaviour
{
    InputController _inputController;

    private void Awake()
    {
        TryGetComponent(out InputController inputController);
        this._inputController = inputController;
    }

    void Start()
    {
        
    }

    void Update()
    {
        Debug.Log(_inputController.MoveInput);
    }
}
