using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class QuadroDeContratos : MonoBehaviour
{
    [Header("Banco de Dados")]
    public List<Contrato> todosOsContratos;

    [Header("Geração da UI")]
    public GameObject prefabBotaoContrato;
    public Transform localParaGerarBotoes; // O painel interno da janela
    public int maxContratosNoQuadro = 2;

    [Header("Configurações do Timer (Segundos)")]
    public float tempoMinimoGeracao = 5f;
    public float tempoMaximoGeracao = 15f;

    private GerenciadorLogica gerenciadorLogica;

    private List<GameObject> botoesGerados = new List<GameObject>();
    private List<Contrato> contratosAtivosNoQuadro = new List<Contrato>();

    void Start()
    {
        gerenciadorLogica = FindFirstObjectByType<GerenciadorLogica>();

        GerarUmContratoAleatorio();
        StartCoroutine(CronometroGeradorDeContratos());
    }

    private IEnumerator CronometroGeradorDeContratos()
    {
        while (true)
        {
            float tempoEspera = Random.Range(tempoMinimoGeracao, tempoMaximoGeracao);
            yield return new WaitForSeconds(tempoEspera);

            // Tenta gerar um contrato independentemente de a tela estar aberta ou fechada!
            GerarUmContratoAleatorio();
        }
    }

    public void GerarUmContratoAleatorio()
    {
        if (todosOsContratos == null || todosOsContratos.Count == 0) return;

        for (int i = botoesGerados.Count - 1; i >= 0; i--)
        {
            if (botoesGerados[i] == null || !botoesGerados[i].activeSelf)
            {
                botoesGerados.RemoveAt(i);
                contratosAtivosNoQuadro.RemoveAt(i);
            }
        }

        if (botoesGerados.Count >= maxContratosNoQuadro) return;

        List<Contrato> opcoesDisponiveis = new List<Contrato>();
        foreach (Contrato c in todosOsContratos)
        {
            if (!contratosAtivosNoQuadro.Contains(c))
            {
                opcoesDisponiveis.Add(c);
            }
        }

        if (opcoesDisponiveis.Count == 0) return;

        int indexSorteado = Random.Range(0, opcoesDisponiveis.Count);
        Contrato contratoSorteado = opcoesDisponiveis[indexSorteado];

        // A MÁGICA: A Unity cria o botão dentro do "localParaGerarBotoes".
        // Se a janela estiver fechada, o botão nasce invisível e espera o jogador abrir!
        GameObject novoBotaoObj = Instantiate(prefabBotaoContrato, localParaGerarBotoes);

        botoesGerados.Add(novoBotaoObj);
        contratosAtivosNoQuadro.Add(contratoSorteado);

        BotaoContratoUI scriptBotao = novoBotaoObj.GetComponent<BotaoContratoUI>();
        if (scriptBotao != null)
        {
            scriptBotao.ConfigurarBotao(contratoSorteado, gerenciadorLogica);
        }
    }
}