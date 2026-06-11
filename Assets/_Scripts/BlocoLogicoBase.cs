using System.Collections;
using UnityEngine;

// Uma classe 'abstract' não pode ser colocada diretamente num objeto.
// Ela serve de molde para os outros blocos (If, Print, For, etc) herdarem dela.
public abstract class BlocoLogicoBase : MonoBehaviour
{
    // Usamos IEnumerator para poder criar aquele efeito de "passo a passo" no jogo
    public abstract IEnumerator ExecutarBloco();
}