
using System.Collections.Generic;
using UnityEngine;

public class SistemaMissoes : MonoBehaviour
{
    public static SistemaMissoes Instancia;

    [Header("Banco de missões")]
    [SerializeField] private List<Contrato> todosOsContratos = new List<Contrato>();

    [Header("Configuração das missões secundárias")]
    [Min(1)]
    [SerializeField] private int maxSecundariasDisponiveis = 3;

    [Min(1f)]
    [SerializeField] private float intervaloSecundarias = 600f;

    [Header("Estado atual")]
    [SerializeField] private float tempoParaProximaSecundaria;
    [SerializeField] private int ordemPrincipalConcluida;

    [System.Serializable]
    private class DadosMissoes
    {
        public List<string> disponiveis = new List<string>();
        public List<string> arquivadas = new List<string>();
        public int ordemPrincipalConcluida;
        public float tempoParaProximaSecundaria;
        public List<string> concluidasComSucesso = new List<string>();
    }

    // Missões que aparecem na caixa de entrada.
    private readonly List<Contrato> missoesDisponiveis = new List<Contrato>();

    // Missões que já foram concluídas ou arquivadas.
    private readonly List<Contrato> missoesArquivadas = new List<Contrato>();

    private readonly List<Contrato> missoesConcluidasComSucesso = new List<Contrato>();

    public event System.Action MissoesAtualizadas;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;

            // Inicializa o estado antes do carregamento do save.
            InicializarMissoes();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        AtualizarTemporizadorSecundarias();
    }

    private void InicializarMissoes()
    {
        missoesDisponiveis.Clear();
        missoesArquivadas.Clear();

        ordemPrincipalConcluida = 0;
        tempoParaProximaSecundaria = 0f;

        DesbloquearProximaMissaoPrincipal();

        AvisarAtualizacao();
    }

    private void AtualizarTemporizadorSecundarias()
    {
        if (Time.timeScale <= 0f)
            return;

        tempoParaProximaSecundaria += Time.deltaTime;

        if (tempoParaProximaSecundaria < intervaloSecundarias)
            return;

        // Se a caixa estiver cheia, aguarda uma vaga.
        if (ContarSecundariasDisponiveis() >= maxSecundariasDisponiveis)
            return;

        tempoParaProximaSecundaria = 0f;

        GerarMissaoSecundaria();
    }


    private void GerarMissaoSecundaria()
    {
        List<Contrato> opcoesDisponiveis = new List<Contrato>();

        foreach (Contrato contrato in todosOsContratos)
        {
            if (contrato == null)
                continue;

            if (contrato.categoria != Contrato.CategoriaSolicitacao.Secundaria)
                continue;

            // Impede que a mesma missão apareça duas vezes
            // simultaneamente na caixa de entrada.
            if (missoesDisponiveis.Contains(contrato))
                continue;

            opcoesDisponiveis.Add(contrato);
        }

        if (opcoesDisponiveis.Count == 0)
        {
            Debug.LogWarning(
                "SistemaMissoes: não existem missões secundárias disponíveis no banco de dados."
            );
            return;
        }

        int indiceSorteado = Random.Range(0, opcoesDisponiveis.Count);
        Contrato contratoSorteado = opcoesDisponiveis[indiceSorteado];

        // Remove o status anterior de arquivamento e conclusão.
        missoesArquivadas.Remove(contratoSorteado);
        missoesConcluidasComSucesso.Remove(contratoSorteado);

        // Adiciona novamente à caixa de entrada.
        missoesDisponiveis.Add(contratoSorteado);

        Debug.Log(
            $"Nova missão secundária recebida: {contratoSorteado.tituloDaMissao}"
        );

        AvisarAtualizacao();
        SistemaSave.Salvar();
    }

    private void DesbloquearProximaMissaoPrincipal()
    {
        Contrato proximaMissao = null;
        int menorOrdemEncontrada = int.MaxValue;

        foreach (Contrato contrato in todosOsContratos)
        {
            if (contrato == null)
                continue;

            if (contrato.categoria != Contrato.CategoriaSolicitacao.Principal)
                continue;

            int ordem = contrato.ordemMissaoPrincipal;

            if (ordem <= ordemPrincipalConcluida)
                continue;

            if (ordem < menorOrdemEncontrada)
            {
                menorOrdemEncontrada = ordem;
                proximaMissao = contrato;
            }
        }

        if (proximaMissao == null)
        {
            Debug.Log("Todas as missões principais foram concluídas.");
            return;
        }

        if (!missoesDisponiveis.Contains(proximaMissao))
        {
            missoesDisponiveis.Add(proximaMissao);

            Debug.Log(
                $"Missão principal desbloqueada: {proximaMissao.tituloDaMissao}"
            );
        }
    }

    public void RegistrarResultado(Contrato contrato, bool sucesso)
    {
        if (contrato == null)
        {
            Debug.LogWarning("SistemaMissoes: contrato inválido.");
            return;
        }

        if (!missoesDisponiveis.Contains(contrato))
        {
            Debug.LogWarning(
                "SistemaMissoes: a missão informada não está na caixa de entrada."
            );
            return;
        }

        if (contrato.categoria == Contrato.CategoriaSolicitacao.Principal)
        {
            // Se a principal falhar, ela continua disponível
            // para que o jogador possa tentar novamente.
            if (!sucesso)
            {
                Debug.Log(
                    $"Missão principal não concluída. Ela continuará disponível: {contrato.tituloDaMissao}"
                );

                AvisarAtualizacao();
                return;
            }

            missoesDisponiveis.Remove(contrato);

            if (!missoesArquivadas.Contains(contrato))
                missoesArquivadas.Add(contrato);

            if (!missoesConcluidasComSucesso.Contains(contrato))
                missoesConcluidasComSucesso.Add(contrato);

            ordemPrincipalConcluida = contrato.ordemMissaoPrincipal;

            Debug.Log(
                $"Missão principal concluída: {contrato.tituloDaMissao}"
            );

            DesbloquearProximaMissaoPrincipal();
        }
        else
        {
            // Missões secundárias são arquivadas tanto
            // quando concluídas quanto quando falham.
            missoesDisponiveis.Remove(contrato);

            if (!missoesArquivadas.Contains(contrato))
                missoesArquivadas.Add(contrato);

            if (sucesso)
            {
                if (!missoesConcluidasComSucesso.Contains(contrato))
                    missoesConcluidasComSucesso.Add(contrato);
            }
            else
            {
                missoesConcluidasComSucesso.Remove(contrato);
            }

            Debug.Log(
                sucesso
                    ? $"Missão secundária concluída: {contrato.tituloDaMissao}"
                    : $"Missão secundária arquivada após falha: {contrato.tituloDaMissao}"
            );
        }

        AvisarAtualizacao();
        SistemaSave.Salvar();
    }

    public List<Contrato> ObterMissoesDisponiveis()
    {
        return new List<Contrato>(missoesDisponiveis);
    }

    public List<Contrato> ObterMissoesArquivadas()
    {
        return new List<Contrato>(missoesArquivadas);
    }

    public bool EstaArquivada(Contrato contrato)
    {
        return contrato != null && missoesArquivadas.Contains(contrato);
    }

    public bool FoiConcluidaComSucesso(Contrato contrato)
    {
        return contrato != null &&
               missoesConcluidasComSucesso.Contains(contrato);
    }

    public int ContarSecundariasDisponiveis()
    {
        int total = 0;

        foreach (Contrato contrato in missoesDisponiveis)
        {
            if (contrato != null &&
                contrato.categoria == Contrato.CategoriaSolicitacao.Secundaria)
            {
                total++;
            }
        }

        return total;
    }

    private void AvisarAtualizacao()
    {
        MissoesAtualizadas?.Invoke();
    }

    // Permite que outros scripts consultem o tempo restante.
    public float ObterTempoRestanteSecundaria()
    {
        return Mathf.Max(0f, intervaloSecundarias - tempoParaProximaSecundaria);
    }

    public string ExportarEstado()
    {
        DadosMissoes dados = new DadosMissoes();

        foreach (Contrato contrato in missoesDisponiveis)
        {
            if (contrato != null)
                dados.disponiveis.Add(contrato.name);
        }

        foreach (Contrato contrato in missoesArquivadas)
        {
            if (contrato != null)
                dados.arquivadas.Add(contrato.name);
        }

        dados.ordemPrincipalConcluida = ordemPrincipalConcluida;
        dados.tempoParaProximaSecundaria = tempoParaProximaSecundaria;

        foreach (Contrato contrato in missoesConcluidasComSucesso)
        {
            if (contrato != null)
                dados.concluidasComSucesso.Add(contrato.name);
        }

        return JsonUtility.ToJson(dados);
    }

    public void ImportarEstado(string json)
    {
        if (string.IsNullOrEmpty(json))
            return;

        DadosMissoes dados = JsonUtility.FromJson<DadosMissoes>(json);

        if (dados == null)
        {
            Debug.LogWarning("Não foi possível interpretar o save das missões.");
            return;
        }

        missoesDisponiveis.Clear();
        missoesArquivadas.Clear();
        missoesConcluidasComSucesso.Clear();

        ordemPrincipalConcluida = dados.ordemPrincipalConcluida;

        tempoParaProximaSecundaria = Mathf.Clamp(
            dados.tempoParaProximaSecundaria,
            0f,
            intervaloSecundarias
        );

        foreach (string nomeContrato in dados.disponiveis)
        {
            Contrato contrato = todosOsContratos.Find(
                c => c != null && c.name == nomeContrato
            );

            if (contrato != null && !missoesDisponiveis.Contains(contrato))
            {
                missoesDisponiveis.Add(contrato);
            }
            else
            {
                Debug.LogWarning(
                    $"Contrato disponível não encontrado no banco: {nomeContrato}"
                );
            }
        }

        foreach (string nomeContrato in dados.arquivadas)
        {
            Contrato contrato = todosOsContratos.Find(
                c => c != null && c.name == nomeContrato
            );

            if (contrato != null && !missoesArquivadas.Contains(contrato))
            {
                missoesArquivadas.Add(contrato);
            }
            else
            {
                Debug.LogWarning(
                    $"Contrato arquivado não encontrado no banco: {nomeContrato}"
                );
            }
        }

        foreach (string nomeContrato in dados.concluidasComSucesso)
        {
            Contrato contrato = todosOsContratos.Find(
                c => c != null && c.name == nomeContrato
            );

            if (contrato != null &&
                !missoesConcluidasComSucesso.Contains(contrato))
            {
                missoesConcluidasComSucesso.Add(contrato);
            }
        }

        AvisarAtualizacao();

        Debug.Log("Estado das missões carregado com sucesso!");
    }

}


