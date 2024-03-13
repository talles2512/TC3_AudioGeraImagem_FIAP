﻿using AudioGeraImagemAPI.Domain.Enums;
using MassTransit;

namespace AudioGeraImagemAPI.Domain.Entities
{
    [EntityName("Comando")]
    public class Comando : EntidadeBase
    {
        public byte[] Payload { get; set; }
        public string UrlAudio { get; set; }
        public string TextoProcessado { get; set; }
        public string UrlImagem { get; set; }
        public DateTime InstanteCriacao { get; set; }
        public DateTime InstanteAtualizacao { get; set; }
        public List<ProcessamentoComando> ProcessamentosComandos { get; set; }
    }
}
