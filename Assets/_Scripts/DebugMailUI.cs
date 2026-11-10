using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class DebugMailUI : MonoBehaviour
{
    [Header("Painel de Visualização")]
    [SerializeField] private GameObject painelVisualizacao;

    [Header("Informações do E-mail")]
    [SerializeField] private TMP_Text textoRemetente;
    [SerializeField] private TMP_Text textoAssunto;
    [SerializeField] private TMP_Text textoDescricao;

    [Header("Informações da Missão")]
    [SerializeField] private TMP_Text textoDificuldade;
    [SerializeField] private TMP_Text textoRecompensa;
    [SerializeField] private TMP_Text textoExperiencia;
    [SerializeField] private TMP_Text textoCategoria;

    [Header("Botão de Aceitar")]
    [SerializeField] private Button botaoAceitarMissao;

    [Header("Gerenciador do Jogo")]
    [SerializeField] private GerenciadorLogica gerenciadorLogica;

    private Contrato contratoSelecionado;
    private BotaoContratoUI cartaoSelecionado;

    private readonly CultureInfo culturaBrasileira =
        new CultureInfo("pt-BR");

    private void Awake()
    {
        if (gerenciadorLogica == null)
        {
            gerenciadorLogica =
                FindFirstObjectByType<GerenciadorLogica>();
        }

        if (botaoAceitarMissao != null)
        {
            botaoAceitarMissao.onClick.RemoveListener(AceitarMissao);
            botaoAceitarMissao.onClick.AddListener(AceitarMissao);
            botaoAceitarMissao.interactable = false;
        }
    }

    private void OnEnable()
    {
        BotaoContratoUI.MensagemSelecionada += AoSelecionarMensagem;
    }

    private void OnDisable()
    {
        BotaoContratoUI.MensagemSelecionada -= AoSelecionarMensagem;
    }

    private void Start()
    {
        LimparVisualizacao();
    }

    private void AoSelecionarMensagem(
        Contrato contrato,
        BotaoContratoUI cartao)
    {
        if (contrato == null || cartao == null)
            return;

        // Remove o destaque do cartão anteriormente selecionado.
        if (cartaoSelecionado != null)
        {
            cartaoSelecionado.DefinirSelecionado(false);
        }

        contratoSelecionado = contrato;
        cartaoSelecionado = cartao;

        // Destaca o cartão atual.
        cartaoSelecionado.DefinirSelecionado(true);

        AtualizarVisualizacao();

        if (painelVisualizacao != null)
        {
            painelVisualizacao.SetActive(true);
        }

        if (botaoAceitarMissao != null)
        {
            botaoAceitarMissao.interactable = true;
        }
    }

    private void AtualizarVisualizacao()
    {
        if (contratoSelecionado == null)
            return;

        if (textoRemetente != null)
        {
            textoRemetente.text =
                contratoSelecionado.remetente;
        }

        if (textoAssunto != null)
        {
            textoAssunto.text =
                !string.IsNullOrWhiteSpace(contratoSelecionado.assunto)
                ? contratoSelecionado.assunto
                : contratoSelecionado.tituloDaMissao;
        }

        if (textoDescricao != null)
        {
            textoDescricao.text =
                contratoSelecionado.descricao;
        }

        if (textoDificuldade != null)
        {
            textoDificuldade.text =
                $"Dificuldade: {contratoSelecionado.nivelDificuldade}";
        }

        if (textoRecompensa != null)
        {
            textoRecompensa.text =
                $"Recompensa: R$ {contratoSelecionado.pagamento.ToString("N2", culturaBrasileira)}";
        }

        if (textoExperiencia != null)
        {
            textoExperiencia.text =
                $"Experiência: {contratoSelecionado.xpGanho} XP";
        }

        if (textoCategoria != null)
        {
            textoCategoria.text =
                contratoSelecionado.categoria ==
                Contrato.CategoriaSolicitacao.Principal
                ? "MISSÃO PRINCIPAL"
                : "MISSÃO SECUNDÁRIA";
        }
    }

    private void AceitarMissao()
    {
        if (contratoSelecionado == null || cartaoSelecionado == null)
        {
            Debug.LogWarning(
                "Selecione uma missão antes de tentar aceitá-la."
            );
            return;
        }

        if (gerenciadorLogica == null)
        {
            Debug.LogError(
                "DebugMailUI não encontrou o GerenciadorLogica."
            );
            return;
        }

        gerenciadorLogica.AceitarContrato(
            contratoSelecionado,
            cartaoSelecionado.gameObject
        );
    }

    private void LimparVisualizacao()
    {
        contratoSelecionado = null;
        cartaoSelecionado = null;

        if (textoRemetente != null)
            textoRemetente.text = "";

        if (textoAssunto != null)
            textoAssunto.text = "Selecione uma mensagem";

        if (textoDescricao != null)
            textoDescricao.text = "";

        if (textoDificuldade != null)
            textoDificuldade.text = "";

        if (textoRecompensa != null)
            textoRecompensa.text = "";

        if (textoExperiencia != null)
            textoExperiencia.text = "";

        if (textoCategoria != null)
            textoCategoria.text = "";

        if (botaoAceitarMissao != null)
            botaoAceitarMissao.interactable = false;
    }
}