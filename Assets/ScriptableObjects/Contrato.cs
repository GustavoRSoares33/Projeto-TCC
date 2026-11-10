
using UnityEngine;

// Criação de contratos pelo menu da Unity.
[CreateAssetMenu(fileName = "Novo Contrato", menuName = "Tycoon/Contrato de Lógica")]
public class Contrato : ScriptableObject
{
    public enum TipoDeMissao
    {
        PreencherLacuna,
        EncontrarBug
    }

    public enum Dificuldade
    {
        Estagiario,
        Junior,
        Pleno,
        Senior
    }

    // Secundaria fica primeiro para preservar os contratos antigos
    // como secundários por padrão.
    public enum CategoriaSolicitacao
    {
        Secundaria,
        Principal
    }

    [Header("Informações do E-mail")]
    public CategoriaSolicitacao categoria = CategoriaSolicitacao.Secundaria;

    public string remetente;
    public string assunto;

    [Tooltip("Ordem da missão na sequência principal. Exemplo: 1, 2, 3...")]
    [Min(1)]
    public int ordemMissaoPrincipal = 1;

    [Header("Informações da Missão")]
    public string tituloDaMissao;

    [TextArea]
    public string descricao;

    public Dificuldade nivelDificuldade;
    public TipoDeMissao tipoDaMissao;

    [Header("Recompensas")]
    public float pagamento;
    public int xpGanho;

    [System.Serializable]
    public struct LinhaDeContrato
    {
        [TextArea(1, 3)]
        public string[] fragmentos;

        public string[] respostasIDs;
    }

    [Header("Código Fonte do Desafio")]
    public LinhaDeContrato[] linhasDoCodigo;
}
