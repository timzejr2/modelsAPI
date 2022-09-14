using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;

namespace apimodels.models
{
    public class MaquetesLog : Maquete, ITableEntity
    {
        public MaquetesLog()
        {

        }

        public MaquetesLog(Maquete maquete, TipoAcao tipoAcao, string partitionKey, string rowKey)
        {
            base.Id = maquete.Id;
            base.NomeMaquete = maquete.NomeMaquete;
            base.DescricaoMaquete = maquete.DescricaoMaquete;
            base.ImgMaquete = maquete.ImgMaquete;
            base.ArquivoMaquete = maquete.ArquivoMaquete;
            base.MaqueteAtiva = maquete.MaqueteAtiva;
            base.IdUsuario = maquete.IdUsuario;
            base.DataPublicacao = maquete.DataPublicacao;
            TipoAcao = tipoAcao;
            JSON = JsonSerializer.Serialize(maquete);
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

    public TipoAcao TipoAcao { get; set; }
    public string JSON { get; set; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
}