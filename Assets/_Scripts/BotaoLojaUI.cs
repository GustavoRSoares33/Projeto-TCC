using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class BotaoLojaUI : MonoBehaviour
{
    [Header("Item à Venda")]
    public ItemHardware meuItem; // O ScriptableObject da peça

    [Header("Referências Visuais")]
    public TMP_Text textoDoBotao; // Arraste o filho (Text TMP) para cá no Inspector

    void Start()
    {
        // Atualiza o texto do botão com os dados reais do item assim que o jogo começa
        if (meuItem != null && textoDoBotao != null)
        {
            textoDoBotao.text = $"{meuItem.nomeDoItem}\nPreço: R$ {meuItem.preco} | Performance: +{meuItem.ganhoDePerformance}";
        }

        // Configura o clique
        Button botao = GetComponent<Button>();
        botao.onClick.AddListener(TentarComprar);
    }

    private void TentarComprar()
    {
        if (meuItem == null) return;

        // Chama a função de compra que já tínhamos criado no EconomiaManager!
        bool compraAprovada = EconomiaManager.Instancia.ComprarHardware(meuItem);

        if (compraAprovada)
        {
            Debug.Log($"<color=green>Item Comprado!</color> Você adquiriu: {meuItem.nomeDoItem}");

            // Opcional: Desligar o botão após a compra para não comprar duplicado
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("<color=red>Compra Negada!</color> Vá fazer mais contratos para juntar dinheiro.");
        }
    }
}