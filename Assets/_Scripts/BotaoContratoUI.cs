using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Globalization;

public class BotaoContratoUI : MonoBehaviour
{
    [Header("Informações da Mensagem")]
    public TMP_Text textoTitulo;
    public TMP_Text textoRemetente;
    public TMP_Text textoCategoria;

    [Header("Informações da Missão")]
    public TMP_Text textoValorDificuldade;
    public TMP_Text textoValorRecompensa;
    public TMP_Text textoValorXp;

    [Header("Componentes de Seleção")]
    public Button botaoSelecionar;
    public Image imagemCartao;

    [Header("Destaque do cartão selecionado")]
    [SerializeField] private Outline contornoSelecionado;

    [SerializeField]
    private Color corNormal =
        new Color32(38, 58, 61, 255);

    [SerializeField]
    private Color corSelecionado =
        new Color32(48, 77, 80, 255);

    [Header("Compatibilidade com o Prefab Antigo")]
    public Button botaoAceitar;

    private Contrato contratoDesteBotao;
    private GerenciadorLogica gerenciador;

    private readonly CultureInfo culturaBrasileira =
        new CultureInfo("pt-BR");

    // Evento utilizado pelo DebugMailUI para saber qual mensagem foi selecionada.
    public static event Action<Contrato, BotaoContratoUI> MensagemSelecionada;

    public Contrato ContratoAtual => contratoDesteBotao;

    public void ConfigurarBotao(
        Contrato contratoSorteado,
        GerenciadorLogica refGerenciador)
    {
        contratoDesteBotao = contratoSorteado;
        gerenciador = refGerenciador;

        DefinirSelecionado(false);

        if (contratoDesteBotao == null)
        {
            Debug.LogWarning("Não foi possível configurar o cartão: contrato nulo.");
            return;
        }

        AtualizarInformacoesVisuais();

        if (botaoSelecionar != null)
        {
            botaoSelecionar.onClick.RemoveListener(AoSelecionarMensagem);
            botaoSelecionar.onClick.AddListener(AoSelecionarMensagem);
        }
        else
        {
            Debug.LogWarning(
                $"O cartão {name} não possui o Button de seleção associado."
            );
        }

        if (imagemCartao != null)
            imagemCartao.color = corNormal;
    }

    private void AtualizarInformacoesVisuais()
    {
        if (textoTitulo != null)
        {
            textoTitulo.text = !string.IsNullOrWhiteSpace(contratoDesteBotao.assunto)
                ? contratoDesteBotao.assunto
                : contratoDesteBotao.tituloDaMissao;
        }

        if (textoRemetente != null)
        {
            textoRemetente.text = contratoDesteBotao.remetente;
        }

        if (textoCategoria != null)
        {
            if (contratoDesteBotao.categoria ==
                Contrato.CategoriaSolicitacao.Principal)
            {
                textoCategoria.text = "MISSÃO PRINCIPAL";
                textoCategoria.color = new Color32(39, 133, 232, 255);
            }
            else
            {
                textoCategoria.text = "MISSÃO SECUNDÁRIA";
                textoCategoria.color = new Color32(54, 184, 121, 255);
            }
        }

        if (textoValorDificuldade != null)
        {
            textoValorDificuldade.text =
                contratoDesteBotao.nivelDificuldade.ToString();
        }

        if (textoValorRecompensa != null)
        {
            textoValorRecompensa.text =
                $"R$ {contratoDesteBotao.pagamento.ToString("N2", culturaBrasileira)}";
        }

        if (textoValorXp != null)
        {
            textoValorXp.text = $"{contratoDesteBotao.xpGanho} XP";
        }
    }

    private void AoSelecionarMensagem()
    {
        if (contratoDesteBotao == null)
            return;

        MensagemSelecionada?.Invoke(contratoDesteBotao, this);
    }

    public void DefinirSelecionado(bool selecionado)
    {
        if (imagemCartao != null)
        {
            imagemCartao.color = selecionado
                ? corSelecionado
                : corNormal;
        }

        if (contornoSelecionado != null)
        {
            contornoSelecionado.enabled = selecionado;
        }
    }

    private void OnDestroy()
    {
        if (botaoSelecionar != null)
            botaoSelecionar.onClick.RemoveListener(AoSelecionarMensagem);
    }
}