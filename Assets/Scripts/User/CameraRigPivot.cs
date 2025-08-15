using Esri.ArcGISMapsSDK.Components;
using UnityEngine;

public class MobileOrbitCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;   // assign in Inspector
    [SerializeField] Transform cam;      // Main Camera

    [Header("Distance settings (m)")]
    [SerializeField] float minDistance = 10f;
    [SerializeField] float maxDistance = 200f;
    [SerializeField] float zoomSpeed   = 0.15f;   // pinch sensitivity
    [SerializeField] float zoomSmooth  = 8f;       // larger = snappier

    [Header("Rotation & tilt")]
    [SerializeField] float rotateSpeed = 0.25f;    // horiz-swipe deg/px
    [SerializeField] float tiltSpeed   = 0.20f;    // vert-swipe deg/px
    [SerializeField] float tiltSmooth  = 8f;
    [SerializeField] float minPitch = 10f;         // look-down clamp
    [SerializeField] float maxPitch = 80f;         // look-up clamp

    float tgtDist = 60f, curDist = 60f;
    float tgtPitch = 30f, curPitch = 30f;
    float lastPinch;

    void LateUpdate()
    {
        if (!player || !cam) return;

        /* keep pivot glued to player */
        transform.position = player.position;

        bool isPinching = Input.touchCount == 2;

        /* ----- pinch (two fingers) → zoom ----- */
        if (isPinching)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);
            float cur = Vector2.Distance(t0.position, t1.position);

            if (lastPinch != 0)
                tgtDist = Mathf.Clamp(
                    tgtDist + (lastPinch - cur) * zoomSpeed,
                    minDistance, maxDistance);

            lastPinch = cur;
        }
        else lastPinch = 0;

        /* ----- one-finger swipe → rotate + tilt (no inversion) ----- */
        if (Input.touchCount == 1 &&
            Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 d = Input.GetTouch(0).deltaPosition;

            transform.Rotate(Vector3.up, d.x * rotateSpeed, Space.Self);

            tgtPitch = Mathf.Clamp(
                tgtPitch - d.y * tiltSpeed,   // swipe up ⇒ camera up
                minPitch, maxPitch);
        }

        /* smooth damping so motion feels fluid */
        curDist  = Mathf.Lerp(curDist,  tgtDist,  Time.deltaTime * zoomSmooth);
        curPitch = Mathf.Lerp(curPitch, tgtPitch, Time.deltaTime * tiltSmooth);

        /* spherical placement around the pivot */
        Vector3 offset = Quaternion.Euler(curPitch, 0, 0) * Vector3.back * curDist;
        cam.position   = transform.position + transform.rotation * offset;
        cam.LookAt(transform.position, Vector3.up);
    }
}