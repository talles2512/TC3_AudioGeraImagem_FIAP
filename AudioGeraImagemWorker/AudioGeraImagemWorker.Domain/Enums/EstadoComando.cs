using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemWorker.Domain.Enums
{
    public enum EstadoComando
    {
        Recebido,
        SalvandoAudio,
        GerandoTexto,
        SalvandoTexto,
        GerandoImagem,
        SalvadoImagem,
        Finalizado,
        Falha
    }
}
