using UnityEngine;
using TMPro; // NECESSÁRIO PARA MEXER COM TEXTOS MODERNOS

public class EconomiaManager : MonoBehaviour
{
    public static EconomiaManager Instancia;

    public float saldoAtual = 0f;

    [Header("Interface Visual")]
    public TMP_Text textoSaldo; // Arraste o seu "Texto_Saldo" para cá na Unity

    void Awake()
    {
        if (Instancia == null)
            Instancia = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Garante que o texto comece com o valor correto logo ao dar Play
        AtualizarTextoSaldo();
    }

    public void AdicionarDinheiro(float valor)
    {
        saldoAtual += valor;
        Debug.Log($"Pagamento recebido! Saldo atual: R$ {saldoAtual}");
        AtualizarTextoSaldo();
    }

    public bool ComprarHardware(ItemHardware item)
    {
        if (saldoAtual >= item.preco)
        {
            saldoAtual -= item.preco;
            AtualizarTextoSaldo();
            return true;
        }

        Debug.Log($"Saldo insuficiente para comprar {item.nomeDoItem}!");
        return false;
    }

    // Função separada para manter o código limpo (Clean Code)
    private void AtualizarTextoSaldo()
    {
        if (textoSaldo != null)
        {
            textoSaldo.text = $"Saldo: R$ {saldoAtual}";
        }
    }
}