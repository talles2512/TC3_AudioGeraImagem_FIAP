﻿namespace AudioGeraImagemWorker.Domain.Enums
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