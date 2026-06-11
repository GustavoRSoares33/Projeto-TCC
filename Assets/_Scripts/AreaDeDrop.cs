using UnityEngine;
using UnityEngine.EventSystems;

public class AreaDeDrop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // Verifica se o objeto que o mouse soltou tem o script BlocoArrastavel
        if (eventData.pointerDrag != null)
        {
            BlocoArrastavel bloco = eventData.pointerDrag.GetComponent<BlocoArrastavel>();

            if (bloco != null)
            {
                // Se for um bloco válido, define este painel como o novo "pai" dele
                bloco.paiOriginal = this.transform;
            }
        }
    }
}