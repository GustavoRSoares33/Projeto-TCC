using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Essas tags garantem que a Unity adicione esses componentes automaticamente no botão
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(LayoutElement))]
public class BlocoArrastavel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private LayoutElement layoutElement;

    // Guarda o pai original para saber de onde o bloco saiu
    [HideInInspector] public Transform paiOriginal;

    public string idDoBloco;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        layoutElement = GetComponent<LayoutElement>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Salva quem é o pai atual (Painel_Ferramentas ou Area_De_Codigo)
        paiOriginal = transform.parent;

        // Joga o bloco para o Canvas principal para ele renderizar por cima de tudo
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        // O Pulo do Gato 1: Tira o bloco do controle do LayoutGroup temporariamente
        layoutElement.ignoreLayout = true;

        // O Pulo do Gato 2: Desliga o bloqueio de raycast. 
        // Se isso for true, o próprio bloco impede o mouse de "enxergar" a área de soltar atrás dele!
        canvasGroup.blocksRaycasts = false;

        // Deixa o bloco meio transparente para dar um feedback visual legal
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Usa a posição que já vem embutida no evento do clique, 
        // funcionando perfeitamente no sistema novo ou no antigo!
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(paiOriginal, false);

        // Puxa pro centro do pai (seja ele o painel ou o buraco)
        rectTransform.anchoredPosition = Vector2.zero;

        // --- NOVA LÓGICA DE ENCAIXE E TAMANHO ---
        // Verifica pelo nome se o bloco pousou no buraco correto
        if (paiOriginal.name == "Buraco_Lacuna")
        {
            RectTransform rtPeca = GetComponent<RectTransform>();

            // 1. O TRUQUE DAS ÂNCORAS: Força o centro de referência da peça a ficar no meio absoluto
            rtPeca.anchorMin = new Vector2(0.5f, 0.5f);
            rtPeca.anchorMax = new Vector2(0.5f, 0.5f);
            rtPeca.pivot = new Vector2(0.5f, 0.5f);

            // 2. Agora sim, mandar para o (0,0) vai cravar o meio da peça no meio do buraco!
            rtPeca.anchoredPosition = Vector2.zero;

            // 3. Força o tamanho
            rtPeca.sizeDelta = new Vector2(250, 50);
        }
        else
        {
            // Opcional: Se ele for solto no vazio e voltar pro painel de ferramentas, 
            // você pode garantir que ele volte para o tamanho original do botão aqui.
            rectTransform.sizeDelta = new Vector2(100, 40); 
        }
        // ----------------------------------------

        layoutElement.ignoreLayout = false;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // FORÇA A UNITY A ARRUMAR A TELA NA MESMA HORA
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)paiOriginal);
    }
}