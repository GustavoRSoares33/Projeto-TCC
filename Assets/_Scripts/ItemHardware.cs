
using UnityEngine;

[CreateAssetMenu(fileName = "NovoHardware", menuName = "Tycoon/Item de Hardware")]
public class ItemHardware : ScriptableObject
{
    [Header("Informações Básicas")]
    public string nomeDoItem;
    public Sprite icone;

    [TextArea]
    public string descricao;

    [Header("Economia e Status")]
    [Min(0)]
    public float preco;

    [Header("Classificação do Item")]
    public TipoHardware tipoDePeca;

    public enum TipoHardware
    {
        Processador,
        PlacaDeVideo,
        MemoriaRAM,
        Cooler,
        Monitor,
        Teclado,
        Armazenamento,
        Software,
        Curso,
        Certificacao
    }

    [Header("Desbloqueio")]
    [Min(1)]
    public int nivelMinimo = 1;

    [Header("Efeito do Upgrade")]
    public TipoEfeito tipoDeEfeito;

    [Min(0)]
    public float valorDoEfeito = 0f;

    public enum TipoEfeito
    {
        Nenhum,
        ErrosTolerados,
        BonusTempoMinigame,
        ReducaoTempoEspera,
        DicaPorMissao,
        RevelarRegiaoSuspeita,
        BonusXPPercentual,
        BonusDinheiroPercentual
    }
}
