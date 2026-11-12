
using UnityEngine;
using TMPro;
using System.Globalization;

public class EconomiaManager : MonoBehaviour
{
    public static EconomiaManager Instancia;

    [Header("Dados do Jogador")]
    public float saldoAtual = 0f;

    [Header("Interface Visual")]
    [Tooltip("Texto de dinheiro exibido no painel do jogador no Desktop.")]
    public TMP_Text textoSaldoDesktop;

    [Tooltip("Texto de dinheiro exibido no painel da Loja.")]
    public TMP_Text textoSaldoLoja;

    private readonly CultureInfo culturaBrasileira =
        new CultureInfo("pt-BR");

    private void Awake()
    {
        if (Instancia == null)
            Instancia = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        AtualizarInterface();
    }

    public void AdicionarDinheiro(float valor)
    {
        if (valor <= 0f)
        {
            Debug.LogWarning(
                "O valor adicionado precisa ser maior que zero."
            );
            return;
        }

        saldoAtual += valor;

        Debug.Log(
            $"Pagamento recebido! Saldo atual: R$ {saldoAtual.ToString("N2", culturaBrasileira)}"
        );

        AtualizarInterface();
        SistemaSave.Salvar();
    }

    public bool ComprarHardware(ItemHardware item)
    {
        if (item == null)
        {
            Debug.LogWarning(
                "Não foi possível comprar: o item está nulo."
            );
            return false;
        }

        if (item.preco < 0f)
        {
            Debug.LogWarning(
                $"O item {item.nomeDoItem} possui um preço inválido."
            );
            return false;
        }

        // Verifica se o sistema de upgrades existe.
        if (SistemaUpgrades.Instancia == null)
        {
            Debug.LogError(
                "SistemaUpgrades não foi encontrado na cena."
            );
            return false;
        }

        // Impede a compra de um item já adquirido.
        if (SistemaUpgrades.Instancia.EstaComprado(item))
        {
            Debug.LogWarning(
                $"Você já possui o upgrade {item.nomeDoItem}."
            );
            return false;
        }

        // Verifica se o jogador já alcançou o nível necessário.
        if (!SistemaUpgrades.Instancia.EstaDesbloqueado(item))
        {
            Debug.Log(
                $"Nível insuficiente para comprar {item.nomeDoItem}. " +
                $"Nível necessário: {item.nivelMinimo}."
            );
            return false;
        }

        // Verifica se o item está cadastrado no sistema.
        if (!SistemaUpgrades.Instancia.PodeComprar(item))
        {
            Debug.LogWarning(
                $"Não foi possível comprar {item.nomeDoItem}. " +
                "Verifique o cadastro do upgrade."
            );
            return false;
        }

        // Verifica o saldo antes de realizar a compra.
        if (saldoAtual < item.preco)
        {
            Debug.Log(
                $"Saldo insuficiente para comprar {item.nomeDoItem}!"
            );
            return false;
        }

        // Registra a compra antes de descontar o dinheiro.
        bool compraRegistrada =
            SistemaUpgrades.Instancia.RegistrarCompra(item);

        if (!compraRegistrada)
        {
            Debug.LogWarning(
                $"Não foi possível registrar a compra de {item.nomeDoItem}."
            );
            return false;
        }

        // Desconta o dinheiro somente após registrar a compra.
        saldoAtual -= item.preco;

        Debug.Log(
            $"Upgrade comprado: {item.nomeDoItem}. " +
            $"Saldo restante: R$ {saldoAtual.ToString("N2", culturaBrasileira)}"
        );

        AtualizarInterface();

        SistemaSave.Salvar();

        return true;
    }

    public void AtualizarInterface()
    {
        string saldoFormatado =
            saldoAtual.ToString("N2", culturaBrasileira);

        if (textoSaldoDesktop != null)
        {
            textoSaldoDesktop.text = $"R$ {saldoFormatado}";
        }

        if (textoSaldoDesktop != null)
        {
            textoSaldoLoja.text = $"R$ {saldoFormatado}";
        }
    }
}
