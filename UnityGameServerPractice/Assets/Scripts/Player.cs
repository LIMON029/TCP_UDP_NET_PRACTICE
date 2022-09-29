using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour
{
    public int id;
    public string username;

    private float moveSpeed = 5f / Constants.TICKS_PER_SEC;
    private bool[] inputs;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;

        inputs = new bool[4];
    }

    public void FixedUpdate()
    {
        Vector2 _inputDirection = Vector2.zero;

        if (inputs[0]) _inputDirection.y += 1;
        else if (inputs[1]) _inputDirection.y -= 1;
        else if (inputs[2]) _inputDirection.x -= 1;
        else if (inputs[3]) _inputDirection.x += 1;

        Move(_inputDirection);
    }

    public void Move(Vector2 _inputDirection)
    {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        transform.position += _moveDirection * moveSpeed;

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }
}
