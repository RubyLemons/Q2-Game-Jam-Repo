using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freelook : MonoBehaviour
{
    public Camera cam;
    public static Camera camStatic;

    [SerializeField] Transform[] animatedCams;

    [HideInInspector] public Vector2 mouseDelta;
    [SerializeField] float sens = 1;

    [Range(0, 90)] [SerializeField] int angleLimit = 90;
    [SerializeField] int angleOffset = 90;

    public bool maxLook;

    void Awake() {
        mouseDelta.y = angleOffset;

        camStatic = cam;
    }

    void Update()
    {
        if (!Tks.freelook) return;

        maxLook = (mouseDelta.y == -angleLimit + angleOffset || mouseDelta.y + angleOffset == angleLimit);

        mouseDelta += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * sens;
        mouseDelta.y = Mathf.Clamp(mouseDelta.y, -angleLimit + angleOffset, angleLimit + angleOffset);

        transform.rotation = Quaternion.Euler(Vector3.up * (mouseDelta.x));

        //camera
        cam.transform.localRotation = Quaternion.Euler(Vector3.left * (mouseDelta.y)) * AnimatedCameras();
    }

    Quaternion AnimatedCameras()
    {
        Vector3 product = Vector3.zero;

        for (int i = 0; i < animatedCams.Length; i++)
            product += animatedCams[i].localRotation.eulerAngles;

        return Quaternion.Euler(product);
    }

    public static void AddFov(float value)
    {
        camStatic.fieldOfView += value;
    }
}
