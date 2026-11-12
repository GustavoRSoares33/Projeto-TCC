
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotaoLojaUI : MonoBehaviour
{
    [Header("Item da Loja")]
    public ItemHardware meuItem;

    [Header("Interface do Cartão")]
    public TMP_Text textoNome;
    public TMP_Text textoDescricao;
    public TMP_Text textoPreco;
    public TMP_Text textoEfeito;
    public TMP_Text textoEstado;

    public Image imagemIcone;

    [Header("Botão de Compra")]
    public Button botaoComprar;

    [Header("Estado Bloqueado")]
    public GameObject overlayBloqueado;
    public TMP_Text textoNivelBloqueio;

    private float ultimoSaldo;
    private int ultimoNivel;
    private bool ultimaSituacaoCompra;

    private void Start()
    {
        if (meuItem == null)
        {
            Debug.LogWarning(
                "BotaoLojaUI: nenhum ItemHardware configurado.",
                this
            );

            if (botaoComprar != null)
                botaoComprar.interactable = false;

            return;
        }

        if (botaoComprar == null)
        {
            Debug.LogError(
                "BotaoLojaUI: botão de compra não configurado.",
                this
            );

            return;
        }

        botaoComprar.onClick.AddListener(TentarComprar);

        AtualizarVisual();
        RegistrarEstadoAtual();
    }

    private void Update()
    {
        if (meuItem == null ||
            EconomiaManager.Instancia == null ||
            SistemaXP.Instancia == null ||
            SistemaUpgrades.Instancia == null)
            return;

        float saldoAtual =
            EconomiaManager.Instancia.saldoAtual;

        int nivelAtual =
            SistemaXP.Instancia.nivelAtual;

        bool comprado =
            SistemaUpgrades.Instancia.EstaComprado(meuItem);

        if (saldoAtual != ultimoSaldo ||
            nivelAtual != ultimoNivel ||
            comprado != ultimaSituacaoCompra)
        {
            AtualizarVisual();
            RegistrarEstadoAtual();
        }
    }

    private void TentarComprar()
    {
        if (meuItem == null ||
            EconomiaManager.Instancia == null)
            return;

        bool compraAprovada =
            EconomiaManager.Instancia.ComprarHardware(meuItem);

        if (compraAprovada)
        {
            Debug.Log(
                $"Compra realizada: {meuItem.nomeDoItem}"
            );
        }

        AtualizarVisual();
        RegistrarEstadoAtual();
    }


    private void AtualizarVisual()
    {
        if (meuItem == null)
            return;

        if (textoNome != null)
            textoNome.text = meuItem.nomeDoItem;

        if (textoDescricao != null)
            textoDescricao.text = meuItem.descricao;

        if (textoPreco != null)
            textoPreco.text = $"R$ {meuItem.preco:N2}";

        if (textoEfeito != null)
            textoEfeito.text = ObterDescricaoEfeito();

        if (imagemIcone != null)
            imagemIcone.sprite = meuItem.icone;

        if (SistemaUpgrades.Instancia == null ||
            SistemaXP.Instancia == null ||
            EconomiaManager.Instancia == null ||
            botaoComprar == null)
        {
            if (botaoComprar != null)
                botaoComprar.interactable = false;

            return;
        }

        bool comprado =
            SistemaUpgrades.Instancia.EstaComprado(meuItem);

        bool desbloqueado =
            SistemaUpgrades.Instancia.EstaDesbloqueado(meuItem);

        bool temDinheiro =
            EconomiaManager.Instancia.saldoAtual >= meuItem.preco;

        // Cartão bloqueado por nível
        if (!desbloqueado && !comprado)
        {
            if (overlayBloqueado != null)
                overlayBloqueado.SetActive(true);

            if (textoNivelBloqueio != null)
            {
                textoNivelBloqueio.text =
                    $"DESBLOQUEIA NO NÍVEL {meuItem.nivelMinimo}";
            }

            botaoComprar.interactable = false;

            return;
        }

        // Se não estiver bloqueado, remove a camada escura
        if (overlayBloqueado != null)
            overlayBloqueado.SetActive(false);

        if (comprado)
        {
            DefinirEstado("ADQUIRIDO", false, "#384A52");
        }
        else if (!temDinheiro)
        {
            DefinirEstado("DINHEIRO INSUFICIENTE", false, "#505B5D");
        }
        else
        {
            DefinirEstado("COMPRAR", true, "#315F50");
        }
    }

    private void DefinirEstado(
        string estado,
        bool podeComprar,
        string corHex)
    {
        botaoComprar.interactable = podeComprar;

        if (textoEstado != null)
            textoEstado.text = estado;

        if (botaoComprar.targetGraphic != null &&
            ColorUtility.TryParseHtmlString(
                corHex, out Color cor))
        {
            botaoComprar.targetGraphic.color = cor;
        }
    }


    private string ObterDescricaoEfeito()
    {
        float valor = meuItem.valorDoEfeito;

        switch (meuItem.tipoDeEfeito)
        {
            case ItemHardware.TipoEfeito.ErrosTolerados:
                return $"+{valor:F0} erros tolerados por missão";

            case ItemHardware.TipoEfeito.BonusTempoMinigame:
                return $"+{valor:F1} segundos nos minigames";

            case ItemHardware.TipoEfeito.ReducaoTempoEspera:
                return $"Reduz {valor:F1} segundos do tempo de espera";

            case ItemHardware.TipoEfeito.DicaPorMissao:
                return "Desbloqueia uma dica por missão";

            case ItemHardware.TipoEfeito.RevelarRegiaoSuspeita:
                return "Revela uma região suspeita do código";

            case ItemHardware.TipoEfeito.BonusXPPercentual:
                return $"+{valor:F0}% de XP nas missões";

            case ItemHardware.TipoEfeito.BonusDinheiroPercentual:
                return $"+{valor:F0}% de dinheiro nas missões";

            case ItemHardware.TipoEfeito.Nenhum:
            default:
                return "Melhoria de hardware";
        }
    }

    private void RegistrarEstadoAtual()
    {
        if (EconomiaManager.Instancia != null)
            ultimoSaldo = EconomiaManager.Instancia.saldoAtual;

        if (SistemaXP.Instancia != null)
            ultimoNivel = SistemaXP.Instancia.nivelAtual;

        if (SistemaUpgrades.Instancia != null && meuItem != null)
        {
            ultimaSituacaoCompra =
                SistemaUpgrades.Instancia.EstaComprado(meuItem);
        }
    }

    private void OnDestroy()
    {
        if (botaoComprar != null)
            botaoComprar.onClick.RemoveListener(TentarComprar);
    }
}
