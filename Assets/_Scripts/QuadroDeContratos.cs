
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuadroDeContratos : MonoBehaviour
{
    public enum FiltroMensagens
    {
        CaixaDeEntrada,
        Principais,
        Secundarias,
        Arquivadas
    }

    [Header("Geração da interface")]
    [SerializeField] private GameObject prefabBotaoContrato;
    [SerializeField] private Transform localParaGerarBotoes;

    [Header("Referências dos sistemas")]
    [SerializeField] private SistemaMissoes sistemaMissoes;
    [SerializeField] private GerenciadorLogica gerenciadorLogica;

    [Header("Estado da caixa de e-mails")]
    [SerializeField]
    private FiltroMensagens filtroAtual = FiltroMensagens.CaixaDeEntrada;

    [Header("Botões do menu lateral")]
    [SerializeField] private Button botaoCaixaEntrada;
    [SerializeField] private Button botaoPrincipais;
    [SerializeField] private Button botaoSecundarias;
    [SerializeField] private Button botaoArquivadas;

    [Header("Cores do menu")]
    [SerializeField]
    private Color corBotaoNormal =
        new Color32(38, 58, 61, 255);

    [SerializeField]
    private Color corBotaoSelecionado =
        new Color32(39, 133, 232, 255);

    private readonly List<GameObject> botoesGerados =
        new List<GameObject>();

    private void Awake()
    {
        if (sistemaMissoes == null)
            sistemaMissoes = SistemaMissoes.Instancia;

        if (gerenciadorLogica == null)
            gerenciadorLogica = FindFirstObjectByType<GerenciadorLogica>();
    }

    private void OnEnable()
    {
        if (sistemaMissoes == null)
            sistemaMissoes = SistemaMissoes.Instancia;

        if (sistemaMissoes != null)
            sistemaMissoes.MissoesAtualizadas += AtualizarQuadro;
    }

    private void OnDisable()
    {
        if (sistemaMissoes != null)
            sistemaMissoes.MissoesAtualizadas -= AtualizarQuadro;
    }

    private void Start()
    {
        AtualizarQuadro();
        AtualizarVisualBotoesMenu();
    }

    public void MostrarCaixaDeEntrada()
    {
        AlterarFiltro(FiltroMensagens.CaixaDeEntrada);
    }

    public void MostrarMissoesPrincipais()
    {
        AlterarFiltro(FiltroMensagens.Principais);
    }

    public void MostrarMissoesSecundarias()
    {
        AlterarFiltro(FiltroMensagens.Secundarias);
    }

    public void MostrarArquivadas()
    {
        AlterarFiltro(FiltroMensagens.Arquivadas);
    }

    private void AlterarFiltro(FiltroMensagens novoFiltro)
    {
        filtroAtual = novoFiltro;

        AtualizarVisualBotoesMenu();
        AtualizarQuadro();
    }

    public void AtualizarQuadro()
    {
        if (sistemaMissoes == null)
            sistemaMissoes = SistemaMissoes.Instancia;

        if (sistemaMissoes == null)
        {
            Debug.LogWarning(
                "QuadroDeContratos: SistemaMissoes não foi encontrado."
            );
            return;
        }

        if (prefabBotaoContrato == null || localParaGerarBotoes == null)
        {
            Debug.LogError(
                "QuadroDeContratos: configure o prefab e o local de geração dos cartões no Inspector."
            );
            return;
        }

        LimparCartoes();

        List<Contrato> contratos;

        if (filtroAtual == FiltroMensagens.Arquivadas)
        {
            contratos = sistemaMissoes.ObterMissoesArquivadas();
        }
        else
        {
            contratos = sistemaMissoes.ObterMissoesDisponiveis();
        }

        foreach (Contrato contrato in contratos)
        {
            if (contrato == null)
                continue;

            if (!ContratoPertenceAoFiltro(contrato))
                continue;

            CriarCartao(contrato);
        }
    }

    private void AtualizarVisualBotoesMenu()
    {
        DefinirCorBotao(
            botaoCaixaEntrada,
            filtroAtual == FiltroMensagens.CaixaDeEntrada
        );

        DefinirCorBotao(
            botaoPrincipais,
            filtroAtual == FiltroMensagens.Principais
        );

        DefinirCorBotao(
            botaoSecundarias,
            filtroAtual == FiltroMensagens.Secundarias
        );

        DefinirCorBotao(
            botaoArquivadas,
            filtroAtual == FiltroMensagens.Arquivadas
        );
    }

    private void DefinirCorBotao(Button botao, bool selecionado)
    {
        if (botao == null || botao.targetGraphic == null)
            return;

        botao.targetGraphic.color = selecionado
            ? corBotaoSelecionado
            : corBotaoNormal;
    }
    private bool ContratoPertenceAoFiltro(Contrato contrato)
    {
        switch (filtroAtual)
        {
            case FiltroMensagens.CaixaDeEntrada:
                return true;

            case FiltroMensagens.Principais:
                return contrato.categoria ==
                    Contrato.CategoriaSolicitacao.Principal;

            case FiltroMensagens.Secundarias:
                return contrato.categoria ==
                    Contrato.CategoriaSolicitacao.Secundaria;

            case FiltroMensagens.Arquivadas:
                return true;

            default:
                return false;
        }
    }

    private void CriarCartao(Contrato contrato)
    {
        GameObject novoCartao = Instantiate(
            prefabBotaoContrato,
            localParaGerarBotoes
        );

        botoesGerados.Add(novoCartao);

        BotaoContratoUI scriptBotao =
            novoCartao.GetComponent<BotaoContratoUI>();

        if (scriptBotao == null)
        {
            Debug.LogError(
                "O prefab do cartão não possui o componente BotaoContratoUI."
            );
            return;
        }

        scriptBotao.ConfigurarBotao(contrato, gerenciadorLogica);
    }

    private void LimparCartoes()
    {
        foreach (GameObject botao in botoesGerados)
        {
            if (botao != null)
                Destroy(botao);
        }

        botoesGerados.Clear();
    }
}
