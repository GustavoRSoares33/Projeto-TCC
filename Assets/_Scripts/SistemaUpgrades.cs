
using System.Collections.Generic;
using UnityEngine;

public class SistemaUpgrades : MonoBehaviour
{
    public static SistemaUpgrades Instancia;

    [Header("Banco de upgrades")]
    [SerializeField]
    private List<ItemHardware> todosOsItens = new List<ItemHardware>();

    // Itens que o jogador já comprou.
    private readonly List<ItemHardware> itensComprados = new List<ItemHardware>();

    [System.Serializable]
    private class DadosUpgrades
    {
        public List<string> itensComprados = new List<string>();
    }

    public event System.Action UpgradesAtualizados;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Verifica se o jogador já comprou determinado item.
    public bool EstaComprado(ItemHardware item)
    {
        return item != null && itensComprados.Contains(item);
    }

    // Verifica se o nível atual permite desbloquear o item.
    public bool EstaDesbloqueado(ItemHardware item)
    {
        if (item == null || SistemaXP.Instancia == null)
            return false;

        return SistemaXP.Instancia.nivelAtual >= item.nivelMinimo;
    }

    // Verifica se o jogador pode comprar o item.
    public bool PodeComprar(ItemHardware item)
    {
        return item != null
            && !EstaComprado(item)
            && EstaDesbloqueado(item);
    }

    // Registra a compra de um upgrade.
    public bool RegistrarCompra(ItemHardware item)
    {
        if (item == null)
        {
            Debug.LogWarning("SistemaUpgrades: item inválido.");
            return false;
        }

        if (EstaComprado(item))
        {
            Debug.LogWarning(
                $"O upgrade {item.nomeDoItem} já foi comprado."
            );
            return false;
        }

        if (!EstaDesbloqueado(item))
        {
            Debug.LogWarning(
                $"Nível insuficiente para desbloquear {item.nomeDoItem}."
            );
            return false;
        }

        if (!todosOsItens.Contains(item))
        {
            Debug.LogWarning(
                $"O item {item.nomeDoItem} não está cadastrado no SistemaUpgrades."
            );
            return false;
        }

        itensComprados.Add(item);

        Debug.Log(
            $"Upgrade adquirido: {item.nomeDoItem}"
        );

        AvisarAtualizacao();

        return true;
    }

    // Soma os efeitos de todos os upgrades comprados
    // que possuem determinado tipo de benefício.
    public float ObterValorTotalEfeito(ItemHardware.TipoEfeito efeito)
    {
        float total = 0f;

        foreach (ItemHardware item in itensComprados)
        {
            if (item == null)
                continue;

            if (item.tipoDeEfeito == efeito)
                total += item.valorDoEfeito;
        }

        return total;
    }

    // Verifica se existe pelo menos um upgrade com determinado efeito.
    public bool PossuiEfeito(ItemHardware.TipoEfeito efeito)
    {
        return ObterValorTotalEfeito(efeito) > 0f;
    }

    // Permite consultar a lista de upgrades comprados.
    public List<ItemHardware> ObterItensComprados()
    {
        return new List<ItemHardware>(itensComprados);
    }

    // Prepara os dados para o sistema de save.
    public string ExportarEstado()
    {
        DadosUpgrades dados = new DadosUpgrades();

        foreach (ItemHardware item in itensComprados)
        {
            if (item != null)
                dados.itensComprados.Add(item.name);
        }

        return JsonUtility.ToJson(dados);
    }

    // Restaura as compras salvas.
    public void ImportarEstado(string json)
    {
        if (string.IsNullOrEmpty(json))
            return;

        DadosUpgrades dados =
            JsonUtility.FromJson<DadosUpgrades>(json);

        if (dados == null)
        {
            Debug.LogWarning(
                "Não foi possível carregar os upgrades salvos."
            );
            return;
        }

        itensComprados.Clear();

        if (dados.itensComprados != null)
        {
            foreach (string nomeItem in dados.itensComprados)
            {
                ItemHardware item = todosOsItens.Find(
                    i => i != null && i.name == nomeItem
                );

                if (item != null && !itensComprados.Contains(item))
                {
                    itensComprados.Add(item);
                }
                else
                {
                    Debug.LogWarning(
                        $"Upgrade não encontrado no banco: {nomeItem}"
                    );
                }
            }
        }

        AvisarAtualizacao();

        Debug.Log("Upgrades carregados com sucesso!");
    }

    private void AvisarAtualizacao()
    {
        UpgradesAtualizados?.Invoke();
    }


    public void LimparCompras()
    {
        itensComprados.Clear();

        AvisarAtualizacao();

        Debug.Log("Todos os upgrades comprados foram resetados.");
    }

}
