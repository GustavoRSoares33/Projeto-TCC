using UnityEngine;
using TMPro;
using System.Globalization;

public class EconomiaManager : MonoBehaviour
{
    public static EconomiaManager Instancia;

    [Header("Dados do Jogador")]
    public float saldoAtual = 0f;

    [Header("Interface Visual")]
    [Tooltip("Texto de saldo usado pelas interfaces atuais, como a loja.")]
    public TMP_Text textoSaldo;

    [Tooltip("Texto de dinheiro exibido no painel do jogador no Desktop.")]
    public TMP_Text textoSaldoDesktop;

    private readonly CultureInfo culturaBrasileira = new CultureInfo("pt-BR");

    void Awake()
    {
        if (Instancia == null)
            Instancia = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Atualiza as duas interfaces assim que o jogo começa.
        AtualizarInterface();
    }

    public void AdicionarDinheiro(float valor)
    {
        if (valor <= 0f)
        {
            Debug.LogWarning("O valor adicionado precisa ser maior que zero.");
            return;
        }

        saldoAtual += valor;
        Debug.Log($"Pagamento recebido! Saldo atual: R$ {saldoAtual.ToString("N2", culturaBrasileira)}");
        AtualizarInterface();
        SistemaSave.Salvar();
    }

    public bool ComprarHardware(ItemHardware item)
    {
        if (item == null)
        {
            Debug.LogWarning("Não foi possível comprar: o item está nulo.");
            return false;
        }

        if (item.preco < 0f)
        {
            Debug.LogWarning($"O item {item.nomeDoItem} possui um preço inválido.");
            return false;
        }

        if (saldoAtual >= item.preco)
        {
            saldoAtual -= item.preco;
            AtualizarInterface();
            SistemaSave.Salvar();
            return true;
        }

        Debug.Log($"Saldo insuficiente para comprar {item.nomeDoItem}!");
        return false;
    }

    public void AtualizarInterface()
    {
        string saldoFormatado = saldoAtual.ToString("N2", culturaBrasileira);

        // Mantém funcionando o texto já usado pelas outras janelas.
        if (textoSaldo != null)
        {
            textoSaldo.text = $"Saldo: R$ {saldoFormatado}";
        }

        // Atualiza também o painel do jogador no Desktop.
        if (textoSaldoDesktop != null)
        {
            textoSaldoDesktop.text = $"$ {saldoFormatado}";
        }
    }
}
