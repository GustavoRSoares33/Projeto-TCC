
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

    [Header("Botão de Ação")]
    [SerializeField] private Button botaoAceitarMissao;
    [SerializeField] private TMP_Text textoBotaoAcao;

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

            if (textoBotaoAcao == null)
            {
                textoBotaoAcao =
                    botaoAceitarMissao.GetComponentInChildren<TMP_Text>(true);
            }
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

        if (cartaoSelecionado != null)
            cartaoSelecionado.DefinirSelecionado(false);

        contratoSelecionado = contrato;
        cartaoSelecionado = cartao;

        cartaoSelecionado.DefinirSelecionado(true);

        AtualizarVisualizacao();
        AtualizarBotaoAcao();

        if (painelVisualizacao != null)
            painelVisualizacao.SetActive(true);
    }

    private void AtualizarVisualizacao()
    {
        if (contratoSelecionado == null)
            return;

        if (textoRemetente != null)
            textoRemetente.text = contratoSelecionado.remetente;

        if (textoAssunto != null)
        {
            textoAssunto.text =
                !string.IsNullOrWhiteSpace(contratoSelecionado.assunto)
                ? contratoSelecionado.assunto
                : contratoSelecionado.tituloDaMissao;
        }

        if (textoDescricao != null)
            textoDescricao.text = contratoSelecionado.descricao;

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

    private void AtualizarBotaoAcao()
    {
        if (botaoAceitarMissao == null)
            return;

        bool arquivada =
            SistemaMissoes.Instancia != null &&
            SistemaMissoes.Instancia.EstaArquivada(contratoSelecionado);

        bool concluida =
            arquivada &&
            SistemaMissoes.Instancia.FoiConcluidaComSucesso(contratoSelecionado);

        botaoAceitarMissao.interactable = !arquivada;

        if (textoBotaoAcao != null)
        {
            if (concluida)
                textoBotaoAcao.text = "CONCLUÍDO";
            else if (arquivada)
                textoBotaoAcao.text = "ARQUIVADO";
            else
                textoBotaoAcao.text = "ACEITAR MISSÃO";
        }
    }

    private void AceitarMissao()
    {
        if (contratoSelecionado == null || cartaoSelecionado == null)
            return;

        if (SistemaMissoes.Instancia != null &&
            SistemaMissoes.Instancia.EstaArquivada(contratoSelecionado))
        {
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

    // Chamado quando o jogador retorna de uma missão.
    public void LimparSelecao()
    {
        if (cartaoSelecionado != null)
            cartaoSelecionado.DefinirSelecionado(false);

        LimparVisualizacao();
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
        {
            botaoAceitarMissao.interactable = false;

            if (textoBotaoAcao != null)
                textoBotaoAcao.text = "ACEITAR MISSÃO";
        }
    }
}
