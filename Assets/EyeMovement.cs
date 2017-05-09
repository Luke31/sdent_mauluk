using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeMovement : MonoBehaviour
{
    private enum EyeState
    {
        Resetting,
        Shaking,
        Aiming,
        Looking,
        Returning
    }

    public float erraticness = 1f;
    public float maxLookingRadius = 0.4f;
    public float maxShakingRadius = 0.05f;
    public float lookingSpeed = 1f;
    public float lookingLinger = 1f;

    private EyeState _eyeState;
    private float _lookingTimer;
    private System.Random _random;
    private Vector3 _lookingPosition;
    private float _lookingLerp = 0;

    // Use this for initialization
    void Start()
    {
        _random = new System.Random(DateTime.Now.Millisecond);
    }

    // Update is called once per frame
    void Update()
    {
        switch (_eyeState)
        {
            case EyeState.Resetting:
                _lookingTimer = (float) _random.NextDouble() * 10f / erraticness;
                _eyeState = EyeState.Shaking;
                break;
            case EyeState.Shaking:
                _lookingTimer -= Time.deltaTime;
                transform.localPosition = new Vector3(
                    ((float) _random.NextDouble() * maxShakingRadius) - maxShakingRadius / 2f,
                    ((float) _random.NextDouble() * maxShakingRadius) - maxShakingRadius / 2f);

                if (_lookingTimer < 0)
                {
                    _lookingPosition = new Vector3(
                        ((float) _random.NextDouble() * maxLookingRadius) - maxLookingRadius / 2f,
                        ((float) _random.NextDouble() * maxLookingRadius) - maxLookingRadius / 2f);
                    _eyeState = EyeState.Aiming;
                    _lookingTimer = (float) _random.NextDouble() * 10f / erraticness;
                    _lookingLerp = 0;
                }
                break;
            case EyeState.Aiming:
                transform.localPosition = Vector3.Lerp(new Vector3(0, 0), _lookingPosition, _lookingLerp);
                _lookingLerp += Time.deltaTime * lookingSpeed;
                if (_lookingLerp >= 1)
                {
                    _eyeState = EyeState.Looking;
                    _lookingLerp = 0;
                    _lookingTimer = lookingLinger;
                }
                break;
            case EyeState.Looking:
                _lookingTimer -= Time.deltaTime;

                if (_lookingTimer < 0)
                {
                    _eyeState = EyeState.Returning;
                }
                break;
            case EyeState.Returning:
                transform.localPosition = Vector3.Lerp(_lookingPosition, new Vector3(0, 0), _lookingLerp);
                _lookingLerp += Time.deltaTime * lookingSpeed;
                if (_lookingLerp >= 1)
                {
                    _eyeState = EyeState.Resetting;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}