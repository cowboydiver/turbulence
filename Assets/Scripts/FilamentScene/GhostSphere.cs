using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GhostSphere : MonoBehaviour
{
    #region Variables
    public Transform Transform { get { return transform; } }
    public float Radius { get; private set; }
    //public NewRadiusRing ringPrefab;
    //public FilamentMenu filamentMenu;

    float ringCoolDown = 0.2f;
    float currentRingCoolDown;
    
    Material material;
    Color color;
    #endregion
    #region Mono
    private void Update()
    {
        if(currentRingCoolDown > 0f)
        {
            currentRingCoolDown -= Time.deltaTime;
        }
        
    }
    #endregion

    #region Public Methods

    public void Init()
    {
        //if(filamentMenu == null){
        //    filamentMenu = FindObjectOfType<FilamentMenu>();
        //}

        currentRingCoolDown = ringCoolDown;

        //outlineMaterial = transform.GetComponent<MeshRenderer>().sharedMaterials[0];
        material = transform.GetComponent<MeshRenderer>().sharedMaterial;

        ResetSphere();
    }

    public void SetPosition(Vector3 position, float radius)
    {
        //if new radius is larger then current radius
        if(radius > Radius)
        {
            transform.position = position;
            transform.localScale = Vector3.one * radius * 2f;

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            Radius = radius;

            NewBiggestSphereRadius();

            //Update menu label
            //UIManager.Inst.GetMenu<FilamentMenu>(4).UpdateBestRadius(position, radius);
        }
    }

    public void SetOver(bool isOver)
    {
        //outlineMaterial.SetColor("_OutlineColor", (isOver ? new Color(0f, 1f, 1f) : Color.black));

        color = (isOver ? new Color(0f, 1f, 1f) : Color.white);
        color.a = material.color.a;
        material.color = color;
    }

    public void ResetSphere()
    {
        Radius = 0f;
        gameObject.SetActive(false);
        SetOver(false);

        //Update menu label
        //UIManager.Inst.GetMenu<FilamentMenu>(4).UpdateBestRadius(Vector3.zero, Radius);
    }

    public void NewBiggestSphereRadius()
    {
        if (currentRingCoolDown > 0f)
            return;

        //AudioManager.Inst.PlayCelebrateSound();
        currentRingCoolDown = ringCoolDown;
        //NewRadiusRing ring = Instantiate(ringPrefab, transform.position, Quaternion.identity);
        //ring.transform.localScale = transform.localScale;
        //ring.Celebrate();
        //filamentMenu.doneButton.GetComponent<LookAtMe>().enabled = true;
        //filamentMenu.nextButton.GetComponent<LookAtMe>().enabled = true;
        //FindObjectOfType<FilamentMenu>().dotIndicator.UserTakeOver();
    }
    
    public void FilamentConquered() {

        //AudioManager.Inst.PlayFilamentConquered();
        //NewRadiusRing ring = Instantiate(ringPrefab, transform.position, Quaternion.identity);
        //ring.transform.localScale = transform.localScale;
        //ring.CelebrateFilamentConquered();
    }

    #endregion
}