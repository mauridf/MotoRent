using MotoRent.Domain.Enum;

namespace MotoRent.Domain.Entities
{
    public class Moto : BaseEntity
    {
        public Guid ModeloId { get; set; }
        public Modelo Modelo { get; set; }
        public int AnoFabricacao { get; set; }
        public int AnoModelo { get; set; }
        public string Placa { get; set; }
        public string Renavam { get; set; }
        public string Chassi { get; set; }
        public int Cilindrada { get; set; } // cc
        public TipoMotor TipoMotor { get; set; }
        public TipoCombustivel TipoCombustivel { get; set; }
        public decimal CapacidadeTanque { get; set; } // litros
        public decimal Peso { get; set; } // kg
        public string Cor { get; set; }
        public MotoStatus Status { get; set; }
        public decimal Quilometragem { get; set; }
        public DateTime? UltimaRevisao { get; set; }
        public DateTime? ProximaRevisao { get; set; }
        public DateTime DataAquisicao { get; set; }
        public decimal ValorAquisicao { get; set; }
        public decimal ValorLocacaoDiaria { get; set; }
        public decimal ValorCaucao { get; set; }
        public DateTime? LicenciamentoValidoAte { get; set; }
        public bool TemSeguro { get; set; }
        public TipoSeguro? TipoSeguro { get; set; }
        public DateTime? VencimentoSeguro { get; set; }
        public string ObservacoesDocumentacao { get; set; }
        public bool Ativo { get; set; }
        public ICollection<FotoMoto> Fotos { get; set; }
        public ICollection<Manutencao> Manutencoes { get; set; }
        public ICollection<Locacao> Locacoes { get; set; }
        public ICollection<Reserva> Reservas { get; set; }
    }
}
