using UnityEngine;

public class MiniGameMouseOverSence : MonoBehaviour
{
    public int strikeSurfaceType;

    public void OnMouseEnter() {
        MiniGameGlobalRef.gunStrikeHeight = transform.position.y;
        MiniGameGlobalRef.ballStrikeSurfaceType = strikeSurfaceType;
    }

}
