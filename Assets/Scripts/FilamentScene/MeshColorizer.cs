using UnityEngine;
using DG.Tweening;

public class MeshColorizer : MonoBehaviour
{
   public void UserOwner()
    {
        //Color newColor = ColorPallet.Inst.userOwner;

        //foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
        //    r.material.DOColor(new Color(newColor.r, newColor.g, newColor.b, 0.62f), 0.5f);
        //}
    }

    public void OtherOwner()
    {
        //Color newColor = ColorPallet.Inst.otherOwner;

        //foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
        //    r.material.DOColor(new Color(newColor.r, newColor.g, newColor.b, 0.62f), 0.5f);
        //}
    }

    public void NoOwner()
    {
        //Color newColor = ColorPallet.Inst.noOwner;

        //foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
        //    r.material.DOColor(new Color(newColor.r, newColor.g, newColor.b, r.material.color.a), 0.5f);
        //}
    }
}
