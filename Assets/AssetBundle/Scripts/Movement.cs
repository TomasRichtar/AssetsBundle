using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private CharacterController _characterController;
    private bool _isRunning;
    [SerializeField] private Transform _camera;
    private float _turnSmoothTime = 2;
    private float _turnVelocity;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isRunning = !_isRunning;
            _moveSpeed = _isRunning ? 50 : 10;
        }
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (move.magnitude >=0.1f)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            _characterController.Move(moveDir.normalized * _moveSpeed * Time.deltaTime);
        }
    }
}
