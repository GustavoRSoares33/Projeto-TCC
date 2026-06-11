using UnityEngine;
using TMPro;
using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static Contrato;

[System.Serializable] // Isso faz a Unity mostrar essa estrutura no Inspector!
public struct MapaDeTelas
{
    public Contrato.TipoDeMissao tipoDaMissao;
    public GameObject painelDaMissao;
}

public class GerenciadorLogica : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text textoMissao;
    public Transform painelFerramentas;
    public TMP_Text textoData;

    public DateTime dataHora;

    [Header("Missão Atual")]
    public Contrato contratoAtivo;
    private GameObject botaoDoContratoAtual; // NOVA LINHA: Guarda quem chamou a missão

    [Header("Telas do Jogo")]
    public GameObject painelQuadroContratos;
    public GameObject painelCodigoVazado;
    public GameObject painelCodeReview;
    public GameObject painelLoja; // NOVA REFERÊNCIA: Arraste o Fundo_Loja para cá na Unity
    public GameObject painelAreaDeTrabalho;
    public GameObject painelPastaImagens;
    public GameObject fundoMesaTrabalho;

    [Header("Abas da Barra de Tarefas")]
    public GameObject abaTarefaContratos;
    public GameObject abaTarefaLoja;

    [Header("Sistema de Lacunas Dinâmicas")]
    public GameObject prefabTextoCodigo;
    public GameObject prefabBuracoLacuna;
    public Transform areaCodigoPai; // O seu objeto AreaCodigo com Horizontal Layout Group

    public GameObject prefabLinhaHorizontal; // Arraste o Prefab que acabou de criar para cá!

    // Lista auxiliar para o sistema lembrar as respostas na hora de validar
    private List<string> respostasEsperadasLacunas = new List<string>();

    [Header("Sistema de Code Review Dinâmico")]
    public GameObject prefabLinhaCodeReview; // O prefab de um Botão com um TextMeshPro dentro
    public Transform areaCodeReviewPai;      // O painel vazio onde as linhas vão ficar (com Vertical Layout Group)

    [Header("Sistema de Banco de Dados de Blocos")]
    [Tooltip("Escreva aqui todas as opções de blocos (ex: if, x, y, while, saldo_atual)")]
    public List<string> bancoDePalavrasDosBlocos;
    public GameObject prefabBlocoBase; // O ÚNICO prefab que você vai usar!
    public int quantidadeTotalNaBarra = 6;

    // Lista para lembrar quantos bugs o jogador já clicou nesta missão específica
    private List<string> bugsEncontradosNaMissao = new List<string>();

    [Header("Roteamento Dinâmico de Telas")]
    public List<MapaDeTelas> rotasDeMissoes;

    [Header("Configurações de Animação")]
    [Range(0.05f, 0.5f)] public float duracaoAnimacao = 0.15f;

    [Header("Sistema de Feedback")]
    public GameObject painelFeedback; // O novo painel visual de resultados
    public TMP_Text textoAcertos;
    public TMP_Text textoErros;
    public TMP_Text textoXP;
    public TMP_Text textoDinheiroBase;
    public TMP_Text textoPenalidades;
    public TMP_Text textoPagamentoTotal;

    public float valorPenalidadePorErro = 15f; // Quanto ele perde por cada clique errado
    private int errosCometidosNaMissao = 0;    // Contador interno
    public int limiteDeErros = 3;

    private List<GameObject> lacunasGeradasNaMissao = new List<GameObject>();

    private GameObject janelaAtivaAtual;

    void Start()
    {
        painelAreaDeTrabalho.SetActive(true);
        painelQuadroContratos.SetActive(false);
        painelLoja.SetActive(false);
        painelCodigoVazado.SetActive(false);
        painelCodeReview.SetActive(false);
        painelPastaImagens.SetActive(false);

        if (painelFeedback != null) painelFeedback.SetActive(false);
        if (fundoMesaTrabalho != null) fundoMesaTrabalho.SetActive(false);
        if (abaTarefaContratos != null) abaTarefaContratos.SetActive(false);
        if (abaTarefaLoja != null) abaTarefaLoja.SetActive(false);

        janelaAtivaAtual = null; // Limpa o estado
        ConfigurarHUDGlobal(false, false);
    }

    void Update()
    {
        textoData.text = Convert.ToString(DateTime.Now);
    }

    // Função para o botão do ícone de Contratos
    public void AbrirJanela(GameObject novaJanela)
    {
        if (novaJanela == null) return;

        if (novaJanela == painelQuadroContratos && contratoAtivo != null)
        {
            if (contratoAtivo.tipoDaMissao == Contrato.TipoDeMissao.PreencherLacuna)
                novaJanela = painelCodigoVazado;
            else if (contratoAtivo.tipoDaMissao == Contrato.TipoDeMissao.EncontrarBug)
                novaJanela = painelCodeReview;
        }

        if (novaJanela == janelaAtivaAtual)
        {
            // O jogador na verdade quer minimizar ela!
            MinimizarJanelaAtiva();
            return; // Interrompe a função aqui para impedir que ela abra de novo
        }

        if (janelaAtivaAtual != null) janelaAtivaAtual.SetActive(false);

        novaJanela.SetActive(true);
        janelaAtivaAtual = novaJanela;

        // O SEGREDO 1: Define quem vai sofrer a animação
        GameObject alvoAnimacao = novaJanela;

        if (fundoMesaTrabalho != null)
        {
            if (novaJanela.transform.IsChildOf(fundoMesaTrabalho.transform))
            {
                fundoMesaTrabalho.SetActive(true);
                alvoAnimacao = fundoMesaTrabalho; // A animação agora puxa a moldura inteira!
            }
            else
            {
                fundoMesaTrabalho.SetActive(false);
            }
        }

        // Dispara a animação no alvo correto
        StartCoroutine(AnimarJanelaCrescendo(alvoAnimacao));

        // Pega o componente de configuração para ligar e organizar a barra de tarefas
        ConfiguracaoApp config = novaJanela.GetComponent<ConfiguracaoApp>();
        if (config != null && config.abaDaBarraDeTarefas != null)
        {
            if (!config.abaDaBarraDeTarefas.activeSelf)
            {
                config.abaDaBarraDeTarefas.SetActive(true);
                config.abaDaBarraDeTarefas.transform.SetAsLastSibling();
            }
            else
            {
                config.abaDaBarraDeTarefas.SetActive(true);
            }
        }

        if (novaJanela == painelQuadroContratos || novaJanela == painelCodigoVazado || novaJanela == painelCodeReview)
        {
            ConfigurarHUDGlobal(true, false);
        }
        else if (novaJanela == painelLoja)
        {
            ConfigurarHUDGlobal(false, true);
        }
        else
        {
            ConfigurarHUDGlobal(false, false);
        }
    }

    // Função para os botões de "X" (Fechar janela)
    public void FecharJanelaAtiva()
    {
        if (janelaAtivaAtual == null) return;

        GameObject janelaParaFechar = janelaAtivaAtual;
        janelaAtivaAtual = null;

        ConfiguracaoApp config = janelaParaFechar.GetComponent<ConfiguracaoApp>();
        if (config != null && config.abaDaBarraDeTarefas != null)
        {
            config.abaDaBarraDeTarefas.SetActive(false);
        }

        // O SEGREDO 2: Descobre quem deve encolher na hora de fechar
        GameObject alvoParaEncolher = janelaParaFechar;
        if (fundoMesaTrabalho != null && janelaParaFechar.transform.IsChildOf(fundoMesaTrabalho.transform))
        {
            alvoParaEncolher = fundoMesaTrabalho;
        }

        // Passamos a janela lógica e o alvo visual para a Coroutine
        StartCoroutine(AnimarJanelaEncolhendo(janelaParaFechar, alvoParaEncolher));
    }

    public void MinimizarJanelaAtiva()
    {
        if (janelaAtivaAtual == null) return;

        GameObject janelaParaMinimizar = janelaAtivaAtual;
        janelaAtivaAtual = null;

        // O SEGREDO 3: Mesma lógica de descobrir o alvo para minimizar
        GameObject alvoParaEncolher = janelaParaMinimizar;
        if (fundoMesaTrabalho != null && janelaParaMinimizar.transform.IsChildOf(fundoMesaTrabalho.transform))
        {
            alvoParaEncolher = fundoMesaTrabalho;
        }

        StartCoroutine(AnimarJanelaEncolhendo(janelaParaMinimizar, alvoParaEncolher));
    }

    private void ConfigurarHUDGlobal(bool mostrarMissao, bool mostrarSaldo)
    {
        if (textoMissao != null) textoMissao.gameObject.SetActive(mostrarMissao);
        if (EconomiaManager.Instancia != null && EconomiaManager.Instancia.textoSaldo != null)
            EconomiaManager.Instancia.textoSaldo.gameObject.SetActive(mostrarSaldo);
    }

    private IEnumerator AnimarJanelaCrescendo(GameObject alvoVisual)
    {
        float tempo = 0;
        alvoVisual.transform.localScale = Vector3.zero;

        while (tempo < duracaoAnimacao)
        {
            tempo += Time.deltaTime;
            float progresso = tempo / duracaoAnimacao;
            float suavizacao = Mathf.SmoothStep(0f, 1f, progresso);

            alvoVisual.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, suavizacao);
            yield return null;
        }

        alvoVisual.transform.localScale = Vector3.one;
    }

    private IEnumerator AnimarJanelaEncolhendo(GameObject janelaLogica, GameObject alvoVisual)
    {
        if (alvoVisual == null) yield break;

        float tempo = 0;
        Vector3 escalaInicial = alvoVisual.transform.localScale;

        while (tempo < duracaoAnimacao)
        {
            if (alvoVisual == null) yield break;

            tempo += Time.deltaTime;
            float progresso = tempo / duracaoAnimacao;
            float suavizacao = Mathf.SmoothStep(0f, 1f, progresso);

            alvoVisual.transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, suavizacao);
            yield return null;
        }

        // --- A ANIMAÇÃO ACABOU AQUI ---

        // 1. Desliga o conteúdo interno (o jogo/app)
        if (janelaLogica != null)
        {
            janelaLogica.SetActive(false);
            janelaLogica.transform.localScale = Vector3.one;
        }

        // 2. Desliga a moldura e reseta o tamanho
        if (fundoMesaTrabalho != null)
        {
            fundoMesaTrabalho.SetActive(false);
            fundoMesaTrabalho.transform.localScale = Vector3.one;
        }

        // 3. Garante que o alvo visual principal resetou para não bugar no futuro
        alvoVisual.transform.localScale = Vector3.one;

        if (painelAreaDeTrabalho != null) painelAreaDeTrabalho.SetActive(true);

        ConfigurarHUDGlobal(false, false);
    }

    // --- NOVA FUNÇÃO: O jogador clicou no botão "Aceitar" do quadro ---
    public void AceitarContrato(Contrato contratoEscolhido, GameObject botaoClicado)
    {
        contratoAtivo = contratoEscolhido;
        errosCometidosNaMissao = 0;
        botaoDoContratoAtual = botaoClicado;
        painelQuadroContratos.SetActive(false);

        LimparMinigames();

        // --- NOVO SISTEMA DE ROTEAMENTO DINÂMICO ---
        GameObject telaAlvo = null;

        // O código olha a nossa lista e procura qual painel pertence a essa missão
        foreach (MapaDeTelas rota in rotasDeMissoes)
        {
            if (rota.tipoDaMissao == contratoAtivo.tipoDaMissao)
            {
                telaAlvo = rota.painelDaMissao;
                break; // Achou a tela? Para de procurar.
            }
        }

        if (contratoAtivo.tipoDaMissao == Contrato.TipoDeMissao.PreencherLacuna)
        {
            StartCoroutine(GerarLinhaDeCodigoComAtraso(telaAlvo));
        }
        else if (contratoAtivo.tipoDaMissao == Contrato.TipoDeMissao.EncontrarBug)
        {
            // Chama a nova corrotina que vamos criar abaixo!
            StartCoroutine(GerarCodeReviewComAtraso(telaAlvo));
        }

        if (textoMissao != null)
        {
            textoMissao.text = $"Contrato: {contratoAtivo.tituloDaMissao}";
            textoMissao.gameObject.SetActive(true);
        }
        if (EconomiaManager.Instancia != null)
            EconomiaManager.Instancia.textoSaldo.gameObject.SetActive(false);
    }

    // Essa função será chamada pelo botão "Validar Código"
    public void ValidarLacuna()
    {
        if (contratoAtivo == null) return;

        int errosNestaEntrega = 0; // Contador isolado para sabermos como ele entregou o tabuleiro

        // 1. Varre todas as lacunas geradas de uma vez só
        for (int i = 0; i < lacunasGeradasNaMissao.Count; i++)
        {
            Transform buraco = lacunasGeradasNaMissao[i].transform;

            // Cenário A: O jogador deixou o buraco vazio
            if (buraco.childCount == 0)
            {
                errosNestaEntrega++;
                Debug.Log($"<color=red>Erro!</color> A lacuna {i + 1} foi deixada vazia.");
            }
            else
            {
                // Cenário B: O jogador colocou uma peça, vamos ver se é a certa
                BlocoArrastavel bloco = buraco.GetChild(0).GetComponent<BlocoArrastavel>();

                if (bloco != null && bloco.idDoBloco != respostasEsperadasLacunas[i])
                {
                    errosNestaEntrega++;
                    Debug.Log($"<color=red>Código Incorreto!</color> A peça na lacuna {i + 1} está errada.");
                }
            }
        }

        // Adiciona os erros desta entrega no contador global da missão
        errosCometidosNaMissao += errosNestaEntrega;

        if (errosNestaEntrega == 0)
        {
            Debug.Log($"<color=green>Sucesso absoluto!</color> Todas as lacunas foram preenchidas corretamente.");
        }
        else
        {
            Debug.Log($"<color=orange>Contrato entregue com falhas!</color> Total de erros: {errosNestaEntrega}");
        }

        // 2. Independente de ele ter acertado tudo ou errado tudo, o contrato acaba AQUI!
        AbrirTelaDeFeedback();
    }

    // Atenção aos novos parâmetros da função!
    public void ValidarBug(string idLinhaClicada, GameObject botaoClicado)
    {
        if (contratoAtivo == null) return;

        bool ehUmBugReal = false;

        Image imgBotao = botaoClicado.GetComponent<Image>();
        Button componenteBotao = botaoClicado.GetComponent<Button>();

        // Verifica na lista dinâmica que geramos!
        foreach (string idCorreto in respostasEsperadasLacunas)
        {
            if (idLinhaClicada == idCorreto)
            {
                ehUmBugReal = true;
                break;
            }
        }

        if (ehUmBugReal)
        {
            // --- ACERTO VISUAL ---
            if (imgBotao != null) imgBotao.color = new Color(0.2f, 0.8f, 0.2f, 0.5f);
            if (componenteBotao != null) componenteBotao.interactable = false;

            if (!bugsEncontradosNaMissao.Contains(idLinhaClicada))
            {
                bugsEncontradosNaMissao.Add(idLinhaClicada);
            }

            // Vitória: Encontrou todos os bugs? (Usa o tamanho da lista dinâmica)
            if (bugsEncontradosNaMissao.Count >= respostasEsperadasLacunas.Count)
            {
                AbrirTelaDeFeedback();
            }
        }
        else
        {
            // --- ERRO VISUAL ---
            errosCometidosNaMissao++;

            if (imgBotao != null) imgBotao.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
            if (componenteBotao != null) componenteBotao.interactable = false;

            // --- A REGRA DA JUSTA CAUSA ---
            if (errosCometidosNaMissao >= limiteDeErros)
            {
                Debug.Log("<color=red>Demissão!</color> Você cometeu muitos erros. O contrato foi cancelado.");
                AbrirTelaDeFeedback();
            }
        }
    }

    private IEnumerator GerarLinhaDeCodigoComAtraso(GameObject telaDaMissao)
    {
        // 1. Limpa o lixo antigo
        foreach (Transform filho in areaCodigoPai) Destroy(filho.gameObject);
        lacunasGeradasNaMissao.Clear();
        respostasEsperadasLacunas.Clear(); // Limpa as respostas antigas

        // 2. Monta o código linha por linha
        foreach (LinhaDeContrato linha in contratoAtivo.linhasDoCodigo)
        {
            // Cria a "bandeja" horizontal para esta linha específica
            GameObject objLinha = Instantiate(prefabLinhaHorizontal, areaCodigoPai, false);

            for (int i = 0; i < linha.fragmentos.Length; i++)
            {
                // Injeta o texto na linha
                GameObject txtObj = Instantiate(prefabTextoCodigo, objLinha.transform, false);
                TMP_Text tmpTexto = txtObj.GetComponent<TMP_Text>();
                tmpTexto.text = linha.fragmentos[i];
                tmpTexto.ForceMeshUpdate();

                // Se houver uma resposta associada a este texto, injeta um buraco ao lado
                if (i < linha.respostasIDs.Length)
                {
                    GameObject buracoObj = Instantiate(prefabBuracoLacuna, objLinha.transform, false);
                    buracoObj.name = "Buraco_Lacuna";

                    lacunasGeradasNaMissao.Add(buracoObj);
                    // Salva a resposta que o jogador deve colocar neste buraco!
                    respostasEsperadasLacunas.Add(linha.respostasIDs[i]);
                }
            }
        }

        GerarBlocosAleatorios();

        yield return null;

        if (telaDaMissao != null)
        {
            telaDaMissao.SetActive(false);
            telaDaMissao.SetActive(true);
        }
    }

    private IEnumerator GerarCodeReviewComAtraso(GameObject telaDaMissao)
    {
        // 1. Limpeza da missão anterior
        foreach (Transform filho in areaCodeReviewPai) Destroy(filho.gameObject);
        bugsEncontradosNaMissao.Clear();
        respostasEsperadasLacunas.Clear(); // Reaproveitamos a lista para guardar os IDs das linhas com bugs!

        // 2. Monta as linhas de código dinamicamente baseadas na Matriz
        for (int i = 0; i < contratoAtivo.linhasDoCodigo.Length; i++)
        {
            GameObject linhaObj = Instantiate(prefabLinhaCodeReview, areaCodeReviewPai, false);
            TMP_Text tmpTexto = linhaObj.GetComponentInChildren<TMP_Text>();

            // Escreve o primeiro fragmento da linha como texto
            if (tmpTexto != null && contratoAtivo.linhasDoCodigo[i].fragmentos.Length > 0)
            {
                tmpTexto.text = $"{i + 1}: {contratoAtivo.linhasDoCodigo[i].fragmentos[0]}";
                tmpTexto.ForceMeshUpdate();
            }

            // SEGREDO: Se você colocou alguma "Resposta ID" no Inspector, essa linha é marcada como um BUG!
            if (contratoAtivo.linhasDoCodigo[i].respostasIDs.Length > 0)
            {
                respostasEsperadasLacunas.Add($"linha_{i}");
            }

            // Programa o botão via código
            Button btnLinha = linhaObj.GetComponent<Button>();
            if (btnLinha != null)
            {
                string idDestaLinha = $"linha_{i}";
                btnLinha.onClick.RemoveAllListeners();
                btnLinha.onClick.AddListener(() => ValidarBug(idDestaLinha, btnLinha.gameObject));
            }
        }

        yield return null;

        if (telaDaMissao != null)
        {
            telaDaMissao.SetActive(false);
            telaDaMissao.SetActive(true);
        }
    }

    private void LimparMinigames()
    {
        // 1. Limpa as sobras de botões dentro da barra de ferramentas
        if (painelFerramentas != null)
        {
            foreach (Transform filho in painelFerramentas) Destroy(filho.gameObject);
        }

        // 2. Destrói os objetos das lacunas geradas (eles serão recriados na próxima missão)
        foreach (GameObject buracoObj in lacunasGeradasNaMissao)
        {
            if (buracoObj != null) Destroy(buracoObj);
        }
        lacunasGeradasNaMissao.Clear();
    }

    private void AbrirTelaDeFeedback()
    {
        if (contratoAtivo == null || painelFeedback == null) return;

        // 1. Identifica o total de desafios que a missão tinha
        // Usamos uma proteção matemática para garantir que nunca dividiremos por zero
        int totalDeDesafios = respostasEsperadasLacunas.Count > 0 ? respostasEsperadasLacunas.Count : 1;

        // 2. Calcula os acertos reais do jogador
        int acertos = 0;
        if (contratoAtivo.tipoDaMissao == Contrato.TipoDeMissao.EncontrarBug)
        {
            acertos = bugsEncontradosNaMissao.Count;
        }
        else if (contratoAtivo.tipoDaMissao == Contrato.TipoDeMissao.PreencherLacuna)
        {
            acertos = Mathf.Max(0, respostasEsperadasLacunas.Count - errosCometidosNaMissao);
        }

        // --- O SEGREDO DA CORREÇÃO FINANCEIRA ---
        // Em vez de dar o dinheiro bruto, dividimos o prêmio pelo total de lacunas/bugs 
        // e multiplicamos apenas pelo que ele de fato acertou!
        float dinheiroProporcional = (contratoAtivo.pagamento / totalDeDesafios) * acertos;

        // Calcula a multa pelos erros cometidos
        float penalidadeTotal = errosCometidosNaMissao * valorPenalidadePorErro;

        // O total a receber agora é o dinheiro ganho por mérito menos as multas por descuido
        float totalReceber = Mathf.Max(0, dinheiroProporcional - penalidadeTotal);

        // 3. Preenche os Textos na Tela
        if (textoAcertos != null) textoAcertos.text = $"Acertos: {acertos}";
        if (textoErros != null) textoErros.text = $"Erros: {errosCometidosNaMissao}";
        if (textoXP != null) textoXP.text = $"XP Ganho: {contratoAtivo.xpGanho}";

        // Mostramos o ganho real de produção no texto de base
        if (textoDinheiroBase != null) textoDinheiroBase.text = $"Pagamento Base: + ${dinheiroProporcional:F2}";
        if (textoPenalidades != null) textoPenalidades.text = $"Penalidades: <color=red>- ${penalidadeTotal:F2}</color>";
        if (textoPagamentoTotal != null) textoPagamentoTotal.text = $"TOTAL RECEBIDO: <color=green>${totalReceber:F2}</color>";

        // 4. Paga o Jogador apenas se ele tiver saldo positivo para receber
        if (EconomiaManager.Instancia != null)
        {
            EconomiaManager.Instancia.AdicionarDinheiro(totalReceber);
        }

        // 5. Mostra a Tela
        if (painelCodeReview != null) painelCodeReview.SetActive(false);
        if (painelCodigoVazado != null) painelCodigoVazado.SetActive(false);

        painelFeedback.SetActive(true);
        janelaAtivaAtual = painelFeedback;
    }

    public void FecharFeedbackEVoltarAoQuadro()
    {
        // Limpa o contrato e desliga o painel
        contratoAtivo = null;
        if (painelFeedback != null) painelFeedback.SetActive(false);

        // Devolve o botão de missão para o pool ou desliga
        if (botaoDoContratoAtual != null)
        {
            botaoDoContratoAtual.SetActive(false);
            botaoDoContratoAtual = null;
        }

        // Limpeza visual
        LimparMinigames();

        // Volta para o quadro
        if (painelQuadroContratos != null) painelQuadroContratos.SetActive(true);
        janelaAtivaAtual = painelQuadroContratos;

        if (textoMissao != null) textoMissao.text = "Escolha um novo contrato.";
    }

    private void GerarBlocosAleatorios()
    {
        if (painelFerramentas == null || prefabBlocoBase == null || bancoDePalavrasDosBlocos.Count == 0) return;

        // 1. Limpa a barra da missão anterior
        foreach (Transform filho in painelFerramentas) Destroy(filho.gameObject);

        // Agora sorteamos PALAVRAS em vez de prefabs!
        List<string> palavrasSorteadas = new List<string>();

        // 2. OBRIGATÓRIO: Injeta as palavras corretas (IDs) da missão atual
        foreach (string idCerto in respostasEsperadasLacunas)
        {
            if (!palavrasSorteadas.Contains(idCerto))
            {
                palavrasSorteadas.Add(idCerto);
            }
        }

        // 3. DISTRAÇÕES: Cria uma cópia do banco de palavras e embaralha
        List<string> copiaBanco = new List<string>(bancoDePalavrasDosBlocos);

        for (int i = copiaBanco.Count - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i + 1);
            string temp = copiaBanco[i];
            copiaBanco[i] = copiaBanco[r];
            copiaBanco[r] = temp;
        }

        // Pega as palavras erradas até encher o limite da barra
        foreach (string palavraAleatoria in copiaBanco)
        {
            if (palavrasSorteadas.Count >= quantidadeTotalNaBarra) break;

            if (!palavrasSorteadas.Contains(palavraAleatoria))
            {
                palavrasSorteadas.Add(palavraAleatoria);
            }
        }

        // 4. EMBARALHAMENTO FINAL: Mistura a lista final de palavras
        for (int i = palavrasSorteadas.Count - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i + 1);
            string temp = palavrasSorteadas[i];
            palavrasSorteadas[i] = palavrasSorteadas[r];
            palavrasSorteadas[r] = temp;
        }

        // 5. CRIAÇÃO FÍSICA: Fabrica os blocos usando o Molde e a palavra sorteada
        foreach (string palavraFinal in palavrasSorteadas)
        {
            GameObject novoBloco = Instantiate(prefabBlocoBase, painelFerramentas, false);

            // Configura a ID lógica (para a função ValidarLacuna saber se está certo)
            BlocoArrastavel scriptBloco = novoBloco.GetComponent<BlocoArrastavel>();
            if (scriptBloco != null) scriptBloco.idDoBloco = palavraFinal;

            // Configura o visual (para o jogador ler o que está escrito na peça)
            TMP_Text textoDoBloco = novoBloco.GetComponentInChildren<TMP_Text>();
            if (textoDoBloco != null) textoDoBloco.text = palavraFinal;
        }
    }

}