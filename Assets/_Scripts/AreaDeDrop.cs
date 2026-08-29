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
                // --- NOVA LÓGICA DE BLOQUEIO ---
                // Verifica se este objeto onde estamos soltando a peça é um buraco de código
                if (this.gameObject.name == "Buraco_Lacuna")
                {
                    // Se já tiver algum filho (peça) dentro deste buraco, rejeitamos a nova peça
                    if (transform.childCount > 0)
                    {
                        return;
                    }
                }

                // Se passou pelo teste acima (buraco vazio) ou se for o Painel de Ferramentas (que cabe vários),
                // define este painel como o novo "pai" do bloco.
                bloco.paiOriginal = this.transform;
            }
        }
    }
}